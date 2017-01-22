#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
// http://sourceforge.net/projects/pdfsharp
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Diagnostics;
#if GDI
using System.Drawing;
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows.Media;
#endif
using PdfSharp.Drawing;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents an external form object (e.g. an imported page).
    /// </summary>
    public sealed class PdfFormXObject : PdfXObject, IContentStream
    {
        internal PdfFormXObject(PdfDocument thisDocument)
            : base(thisDocument)
        {
            Elements.SetName(Keys.Type, "/XObject");
            Elements.SetName(Keys.Subtype, "/Form");
        }

        internal PdfFormXObject(PdfDocument thisDocument, XForm form)
            : base(thisDocument)
        {
            // BUG: form is not used
            Elements.SetName(Keys.Type, "/XObject");
            Elements.SetName(Keys.Subtype, "/Form");

            //if (form.IsTemplate)
            //{ }
        }

        internal double DpiX
        {
            get { return _dpiX; }
            set { _dpiX = value; }
        }
        double _dpiX = 72;

        internal double DpiY
        {
            get { return _dpiY; }
            set { _dpiY = value; }
        }
        double _dpiY = 72;

        internal PdfFormXObject(PdfDocument thisDocument, PdfImportedObjectTable importedObjectTable, XPdfForm form)
            : base(thisDocument)
        {
            Debug.Assert(ReferenceEquals(thisDocument, importedObjectTable.Owner));
            Elements.SetName(Keys.Type, "/XObject");
            Elements.SetName(Keys.Subtype, "/Form");

            if (form.IsTemplate)
            {
                Debug.Assert(importedObjectTable == null);
                // TODO more initialization here???
                return;
            }
            Debug.Assert(importedObjectTable != null);

            XPdfForm pdfForm = form;
            // Get import page
            PdfPages importPages = importedObjectTable.ExternalDocument.Pages;
            if (pdfForm.PageNumber < 1 || pdfForm.PageNumber > importPages.Count)
                PSSR.ImportPageNumberOutOfRange(pdfForm.PageNumber, importPages.Count, form._path);
            PdfPage importPage = importPages[pdfForm.PageNumber - 1];

            // Import resources
            PdfItem res = importPage.Elements["/Resources"];
            if (res != null) // unlikely but possible
            {
#if true
                // Get root object
                PdfObject root;
                if (res is PdfReference)
                    root = ((PdfReference)res).Value;
                else
                    root = (PdfDictionary)res;

                root = ImportClosure(importedObjectTable, thisDocument, root);
                // If the root was a direct object, make it indirect.
                if (root.Reference == null)
                    thisDocument._irefTable.Add(root);

                Debug.Assert(root.Reference != null);
                Elements["/Resources"] = root.Reference;
#else
                // Get transitive closure
                PdfObject[] resources = importPage.Owner.Internals.GetClosure(resourcesRoot);
                int count = resources.Length;
#if DEBUG_
                for (int idx = 0; idx < count; idx++)
                {
                    Debug.Assert(resources[idx].XRef != null);
                    Debug.Assert(resources[idx].XRef.Document != null);
                    Debug.Assert(resources[idx].Document != null);
                    if (resources[idx].ObjectID.ObjectNumber == 12)
                        GetType();
                }
#endif
                // 1st step. Already imported objects are reused and new ones are cloned.
                for (int idx = 0; idx < count; idx++)
                {
                    PdfObject obj = resources[idx];
                    if (importedObjectTable.Contains(obj.ObjectID))
                    {
                        // external object was already imported
                        PdfReference iref = importedObjectTable[obj.ObjectID];
                        Debug.Assert(iref != null);
                        Debug.Assert(iref.Value != null);
                        Debug.Assert(iref.Document == Owner);
                        // replace external object by the already clone counterpart
                        resources[idx] = iref.Value;
                    }
                    else
                    {
                        // External object was not imported ealier and must be cloned
                        PdfObject clone = obj.Clone();
                        Debug.Assert(clone.Reference == null);
                        clone.Document = Owner;
                        if (obj.Reference != null)
                        {
                            // add it to this (the importer) document
                            Owner.irefTable.Add(clone);
                            Debug.Assert(clone.Reference != null);
                            // save old object identifier
                            importedObjectTable.Add(obj.ObjectID, clone.Reference);
                            //Debug.WriteLine("Cloned: " + obj.ObjectID.ToString());
                        }
                        else
                        {
                            // The root object (the /Resources value) is not an indirect object
                            Debug.Assert(idx == 0);
                            // add it to this (the importer) document
                            Owner.irefTable.Add(clone);
                            Debug.Assert(clone.Reference != null);
                        }
                        // replace external object by its clone
                        resources[idx] = clone;
                    }
                }
#if DEBUG_
        for (int idx = 0; idx < count; idx++)
        {
          Debug.Assert(resources[idx].XRef != null);
          Debug.Assert(resources[idx].XRef.Document != null);
          Debug.Assert(resources[idx].Document != null);
          if (resources[idx].ObjectID.ObjectNumber == 12)
            GetType();
        }
#endif

                // 2nd step. Fix up indirect references that still refers to the import document.
                for (int idx = 0; idx < count; idx++)
                {
                    PdfObject obj = resources[idx];
                    Debug.Assert(obj.Owner != null);
                    FixUpObject(importedObjectTable, importedObjectTable.Owner, obj);
                }

                // Set resources key to the root of the clones
                Elements["/Resources"] = resources[0].Reference;
#endif
            }

            // Take /Rotate into account
            PdfRectangle rect = importPage.Elements.GetRectangle(PdfPage.Keys.MediaBox);
            int rotate = importPage.Elements.GetInteger(PdfPage.Keys.Rotate);
            //rotate = 0;
            if (rotate == 0)
            {
                // Set bounding box to media box
                Elements["/BBox"] = rect;
            }
            else
            {
                // TODO: Have to adjust bounding box? (I think not, but I'm not sure -> wait for problem)
                Elements["/BBox"] = rect;

                // Rotate the image such that it is upright
                XMatrix matrix = new XMatrix();
                double width = rect.Width;
                double height = rect.Height;
                matrix.RotateAtPrepend(-rotate, new XPoint(width / 2, height / 2));

                // Translate the image such that its center lies on the center of the rotated bounding box
                double offset = (height - width) / 2;
                if (height > width)
                    matrix.TranslatePrepend(offset, offset);
                else
                    matrix.TranslatePrepend(-offset, -offset);

                //string item = "[" + PdfEncoders.ToString(matrix) + "]";
                //Elements[Keys.Matrix] = new PdfLiteral(item);
                Elements.SetMatrix(Keys.Matrix, matrix);
            }

            // Preserve filter because the content keeps unmodified
            PdfContent content = importPage.Contents.CreateSingleContent();
#if !DEBUG
            content.Compressed = true;
#endif
            PdfItem filter = content.Elements["/Filter"];
            if (filter != null)
                Elements["/Filter"] = filter.Clone();

            // (no cloning needed because the bytes keep untouched)
            Stream = content.Stream; // new PdfStream(bytes, this);
            Elements.SetInteger("/Length", content.Stream.Value.Length);
        }

        /// <summary>
        /// Gets the PdfResources object of this form.
        /// </summary>
        public PdfResources Resources
        {
            get
            {
                if (_resources == null)
                    _resources = (PdfResources)Elements.GetValue(Keys.Resources, VCF.Create);
                return _resources;
            }
        }
        PdfResources _resources;

        PdfResources IContentStream.Resources
        {
            get { return Resources; }
        }

        internal string GetFontName(XFont font, out PdfFont pdfFont)
        {
            pdfFont = _document.FontTable.GetFont(font);
            Debug.Assert(pdfFont != null);
            string name = Resources.AddFont(pdfFont);
            return name;
        }

        string IContentStream.GetFontName(XFont font, out PdfFont pdfFont)
        {
            return GetFontName(font, out pdfFont);
        }

        /// <summary>
        /// Gets the resource name of the specified font data within this form XObject.
        /// </summary>
        internal string GetFontName(string idName, byte[] fontData, out PdfFont pdfFont)
        {
            pdfFont = _document.FontTable.GetFont(idName, fontData);
            Debug.Assert(pdfFont != null);
            string name = Resources.AddFont(pdfFont);
            return name;
        }

        string IContentStream.GetFontName(string idName, byte[] fontData, out PdfFont pdfFont)
        {
            return GetFontName(idName, fontData, out pdfFont);
        }

        string IContentStream.GetImageName(XImage image)
        {
            throw new NotImplementedException();
        }

        string IContentStream.GetFormName(XForm form)
        {
            throw new NotImplementedException();
        }

#if keep_code_some_time_as_reference
        /// <summary>
        /// Replace all indirect references to external objects by their cloned counterparts
        /// owned by the importer document.
        /// </summary>
        void FixUpObject_old(PdfImportedObjectTable iot, PdfObject value)
        {
            // TODO: merge with PdfXObject.FixUpObject
            PdfDictionary dict;
            PdfArray array;
            if ((dict = value as PdfDictionary) != null)
            {
                // Set document for cloned direct objects
                if (dict.Owner == null)
                    dict.Document = Owner;
                else
                    Debug.Assert(dict.Owner == Owner);

                // Search for indirect references in all keys
                PdfName[] names = dict.Elements.KeyNames;
                foreach (PdfName name in names)
                {
                    PdfItem item = dict.Elements[name];
                    // Is item an iref?
                    PdfReference iref = item as PdfReference;
                    if (iref != null)
                    {
                        // Does the iref already belong to this document?
                        if (iref.Document == Owner)
                        {
                            // Yes: fine
                            continue;
                        }
                        else
                        {
                            Debug.Assert(iref.Document == iot.ExternalDocument);
                            // No: replace with iref of cloned object
                            PdfReference newXRef = iot[iref.ObjectID];
                            Debug.Assert(newXRef != null);
                            Debug.Assert(newXRef.Document == Owner);
                            dict.Elements[name] = newXRef;
                        }
                    }
                    else if (item is PdfObject)
                    {
                        // Fix up inner objects
                        FixUpObject_old(iot, (PdfObject)item);
                    }
                }
            }
            else if ((array = value as PdfArray) != null)
            {
                // Set document for cloned direct objects
                if (array.Owner == null)
                    array.Document = Owner;
                else
                    Debug.Assert(array.Owner == Owner);

                // Search for indirect references in all array elements
                int count = array.Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfItem item = array.Elements[idx];
                    // Is item an iref?
                    PdfReference iref = item as PdfReference;
                    if (iref != null)
                    {
                        // Does the iref belongs to this document?
                        if (iref.Document == Owner)
                        {
                            // Yes: fine
                            continue;
                        }
                        else
                        {
                            Debug.Assert(iref.Document == iot.ExternalDocument);
                            // No: replace with iref of cloned object
                            PdfReference newXRef = iot[iref.ObjectID];
                            Debug.Assert(newXRef != null);
                            Debug.Assert(newXRef.Document == Owner);
                            array.Elements[idx] = newXRef;
                        }
                    }
                    else if (item is PdfObject)
                    {
                        // Fix up inner objects
                        FixUpObject_old(iot, (PdfObject)item);
                    }
                }
            }
        }
#endif

        //    /// <summary>
        //    /// Returns ???
        //    /// </summary>
        //    public override string ToString()
        //    {
        //      return "Form";
        //    }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public sealed new class Keys : PdfXObject.Keys
        {
            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes; if present,
            /// must be XObject for a form XObject.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The type of XObject that this dictionary describes; must be Form
            /// for a form XObject.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string Subtype = "/Subtype";

            /// <summary>
            /// (Optional) A code identifying the type of form XObject that this dictionary
            /// describes. The only valid value defined at the time of publication is 1.
            /// Default value: 1.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string FormType = "/FormType";

            /// <summary>
            /// (Required) An array of four numbers in the form coordinate system, giving the 
            /// coordinates of the left, bottom, right, and top edges, respectively, of the 
            /// form XObject’s bounding box. These boundaries are used to clip the form XObject
            /// and to determine its size for caching.
            /// </summary>
            [KeyInfo(KeyType.Rectangle | KeyType.Required)]
            public const string BBox = "/BBox";

            /// <summary>
            /// (Optional) An array of six numbers specifying the form matrix, which maps
            /// form space into user space.
            /// Default value: the identity matrix [1 0 0 1 0 0].
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Matrix = "/Matrix";

            /// <summary>
            /// (Optional but strongly recommended; PDF 1.2) A dictionary specifying any
            /// resources (such as fonts and images) required by the form XObject.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PdfResources))]
            public const string Resources = "/Resources";

            /// <summary>
            /// (Optional; PDF 1.4) A group attributes dictionary indicating that the contents
            /// of the form XObject are to be treated as a group and specifying the attributes
            /// of that group (see Section 4.9.2, “Group XObjects”).
            /// Note: If a Ref entry (see below) is present, the group attributes also apply to the
            /// external page imported by that entry, which allows such an imported page to be
            /// treated as a group without further modification.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string Group = "/Group";

            // further keys:
            //Ref
            //Metadata
            //PieceInfo
            //LastModified
            //StructParent
            //StructParents
            //OPI
            //OC
            //Name

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta
            {
                get { return _meta ?? (_meta = CreateMeta(typeof(Keys))); }
            }
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta
        {
            get { return Keys.Meta; }
        }
    }
}
