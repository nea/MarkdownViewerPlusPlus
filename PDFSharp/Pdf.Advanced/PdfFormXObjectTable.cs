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
using System.Collections.Generic;
using System.Globalization;
using PdfSharp.Drawing;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Contains all external PDF files from which PdfFormXObjects are imported into the current document.
    /// </summary>
    internal sealed class PdfFormXObjectTable : PdfResourceTable
    {
        // The name PdfFormXObjectTable is technically not correct, because in contrast to PdfFontTable
        // or PdfImageTable this class holds no PdfFormXObject objects. Actually it holds instances of
        // the class ImportedObjectTable, one for each external document. The PdfFormXObject instances
        // are not cached, because they hold a transformation matrix that make them unique. If the user
        // wants to use a particual page of a PdfFormXObject more than once, he must reuse the object
        // before he changes the PageNumber or the transformation matrix. In other words this class
        // caches the indirect objects of an external form, not the form itself.

        /// <summary>
        /// Initializes a new instance of this class, which is a singleton for each document.
        /// </summary>
        public PdfFormXObjectTable(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Gets a PdfFormXObject from an XPdfForm. Because the returned objects must be unique, always
        /// a new instance of PdfFormXObject is created if none exists for the specified form. 
        /// </summary>
        public PdfFormXObject GetForm(XForm form)
        {
            // If the form already has a PdfFormXObject, return it.
            if (form._pdfForm != null)
            {
                Debug.Assert(form.IsTemplate, "An XPdfForm must not have a PdfFormXObject.");
                if (ReferenceEquals(form._pdfForm.Owner, Owner))
                    return form._pdfForm;
                //throw new InvalidOperationException("Because of a current limitation of PDFsharp an XPdfForm object can be used only within one single PdfDocument.");

                // Dispose PdfFromXObject when document has changed
                form._pdfForm = null;
            }

            XPdfForm pdfForm = form as XPdfForm;
            if (pdfForm != null)
            {
                // Is the external PDF file from which is imported already known for the current document?
                Selector selector = new Selector(form);
                PdfImportedObjectTable importedObjectTable;
                if (!_forms.TryGetValue(selector, out importedObjectTable))
                {
                    // No: Get the external document from the form and create ImportedObjectTable.
                    PdfDocument doc = pdfForm.ExternalDocument;
                    importedObjectTable = new PdfImportedObjectTable(Owner, doc);
                    _forms[selector] = importedObjectTable;
                }

                PdfFormXObject xObject = importedObjectTable.GetXObject(pdfForm.PageNumber);
                if (xObject == null)
                {
                    xObject = new PdfFormXObject(Owner, importedObjectTable, pdfForm);
                    importedObjectTable.SetXObject(pdfForm.PageNumber, xObject);
                }
                return xObject;
            }
            Debug.Assert(form.GetType() == typeof(XForm));
            form._pdfForm = new PdfFormXObject(Owner, form);
            return form._pdfForm;
        }

        /// <summary>
        /// Gets the imported object table.
        /// </summary>
        public PdfImportedObjectTable GetImportedObjectTable(PdfPage page)
        {
            // Is the external PDF file from which is imported already known for the current document?
            Selector selector = new Selector(page);
            PdfImportedObjectTable importedObjectTable;
            if (!_forms.TryGetValue(selector, out importedObjectTable))
            {
                importedObjectTable = new PdfImportedObjectTable(Owner, page.Owner);
                _forms[selector] = importedObjectTable;
            }
            return importedObjectTable;
        }

        /// <summary>
        /// Gets the imported object table.
        /// </summary>
        public PdfImportedObjectTable GetImportedObjectTable(PdfDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            // Is the external PDF file from which is imported already known for the current document?
            Selector selector = new Selector(document);
            PdfImportedObjectTable importedObjectTable;
            if (!_forms.TryGetValue(selector, out importedObjectTable))
            {
                // Create new table for document.
                importedObjectTable = new PdfImportedObjectTable(Owner, document);
                _forms[selector] = importedObjectTable;
            }
            return importedObjectTable;
        }

        public void DetachDocument(PdfDocument.DocumentHandle handle)
        {
            if (handle.IsAlive)
            {
                foreach (Selector selector in _forms.Keys)
                {
                    PdfImportedObjectTable table = _forms[selector];
                    if (table.ExternalDocument != null && table.ExternalDocument.Handle == handle)
                    {
                        _forms.Remove(selector);
                        break;
                    }
                }
            }

            // Clean table
            bool itemRemoved = true;
            while (itemRemoved)
            {
                itemRemoved = false;
                foreach (Selector selector in _forms.Keys)
                {
                    PdfImportedObjectTable table = _forms[selector];
                    if (table.ExternalDocument == null)
                    {
                        _forms.Remove(selector);
                        itemRemoved = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Map from Selector to PdfImportedObjectTable.
        /// </summary>
        readonly Dictionary<Selector, PdfImportedObjectTable> _forms = new Dictionary<Selector, PdfImportedObjectTable>();

        /// <summary>
        /// A collection of information that uniquely identifies a particular ImportedObjectTable.
        /// </summary>
        public class Selector
        {
            /// <summary>
            /// Initializes a new instance of FormSelector from an XPdfForm.
            /// </summary>
            public Selector(XForm form)
            {
                // HACK: just use full path to identify
                _path = form._path.ToLowerInvariant();
            }

            /// <summary>
            /// Initializes a new instance of FormSelector from a PdfPage.
            /// </summary>
            public Selector(PdfPage page)
            {
                PdfDocument owner = page.Owner;
                _path = "*" + owner.Guid.ToString("B");
                _path = _path.ToLowerInvariant();
            }

            public Selector(PdfDocument document)
            {
                _path = "*" + document.Guid.ToString("B");
                _path = _path.ToLowerInvariant();
            }

            public string Path
            {
                get { return _path; }
                set { _path = value; }
            }
            string _path;

            public override bool Equals(object obj)
            {
                Selector selector = obj as Selector;
                if (obj == null)
                    return false;
                return _path == selector._path; ;
            }

            public override int GetHashCode()
            {
                return _path.GetHashCode();
            }
        }
    }
}
