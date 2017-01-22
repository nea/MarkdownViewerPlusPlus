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
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Security;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a PDF trailer dictionary. Even though trailers are dictionaries they never have a cross
    /// reference entry in PdfReferenceTable.
    /// </summary>
    internal class PdfTrailer : PdfDictionary  // Reference: 3.4.4  File Trailer / Page 96
    {
        /// <summary>
        /// Initializes a new instance of PdfTrailer.
        /// </summary>
        public PdfTrailer(PdfDocument document)
            : base(document)
        {
            _document = document;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTrailer"/> class from a <see cref="PdfCrossReferenceStream"/>.
        /// </summary>
        public PdfTrailer(PdfCrossReferenceStream trailer)
            : base(trailer._document)
        {
            _document = trailer._document;

            // /ID [<09F877EBF282E9408ED1882A9A21D9F2><2A4938E896006F499AC1C2EA7BFB08E4>]
            // /Info 7 0 R
            // /Root 1 0 R
            // /Size 10

            PdfReference iref = trailer.Elements.GetReference(Keys.Info);
            if (iref != null)
                Elements.SetReference(Keys.Info, iref);

            Elements.SetReference(Keys.Root, trailer.Elements.GetReference(Keys.Root));

            Elements.SetInteger(Keys.Size, trailer.Elements.GetInteger(Keys.Size));

            PdfArray id = trailer.Elements.GetArray(Keys.ID);
            if (id != null)
                Elements.SetValue(Keys.ID, id);
        }

        public int Size
        {
            get { return Elements.GetInteger(Keys.Size); }
            set { Elements.SetInteger(Keys.Size, value); }
        }

        // TODO: needed when linearized...
        //public int Prev
        //{
        //  get {return Elements.GetInteger(Keys.Prev);}
        //}

        public PdfDocumentInformation Info
        {
            get { return (PdfDocumentInformation)Elements.GetValue(Keys.Info, VCF.CreateIndirect); }
        }

        /// <summary>
        /// (Required; must be an indirect reference)
        /// The catalog dictionary for the PDF document contained in the file.
        /// </summary>
        public PdfCatalog Root
        {
            get { return (PdfCatalog)Elements.GetValue(PdfTrailer.Keys.Root, VCF.CreateIndirect); }
        }

        /// <summary>
        /// Gets the first or second document identifier.
        /// </summary>
        public string GetDocumentID(int index)
        {
            if (index < 0 || index > 1)
                throw new ArgumentOutOfRangeException("index", index, "Index must be 0 or 1.");

            PdfArray array = Elements[Keys.ID] as PdfArray;
            if (array == null || array.Elements.Count < 2)
                return "";
            PdfItem item = array.Elements[index];
            if (item is PdfString)
                return ((PdfString)item).Value;
            return "";
        }

        /// <summary>
        /// Sets the first or second document identifier.
        /// </summary>
        public void SetDocumentID(int index, string value)
        {
            if (index < 0 || index > 1)
                throw new ArgumentOutOfRangeException("index", index, "Index must be 0 or 1.");

            PdfArray array = Elements[Keys.ID] as PdfArray;
            if (array == null || array.Elements.Count < 2)
                array = CreateNewDocumentIDs();
            array.Elements[index] = new PdfString(value, PdfStringFlags.HexLiteral);
        }

        /// <summary>
        /// Creates and sets two identical new document IDs.
        /// </summary>
        internal PdfArray CreateNewDocumentIDs()
        {
            PdfArray array = new PdfArray(_document);
            byte[] docID = Guid.NewGuid().ToByteArray();
            string id = PdfEncoders.RawEncoding.GetString(docID, 0, docID.Length);
            array.Elements.Add(new PdfString(id, PdfStringFlags.HexLiteral));
            array.Elements.Add(new PdfString(id, PdfStringFlags.HexLiteral));
            Elements[Keys.ID] = array;
            return array;
        }

        /// <summary>
        /// Gets the standard security handler.
        /// </summary>
        public PdfStandardSecurityHandler SecurityHandler
        {
            get
            {
                if (_securityHandler == null)
                    _securityHandler = (PdfStandardSecurityHandler)Elements.GetValue(Keys.Encrypt, VCF.CreateIndirect);
                return _securityHandler;
            }
        }
        internal PdfStandardSecurityHandler _securityHandler;

        internal override void WriteObject(PdfWriter writer)
        {
            // Delete /XRefStm entry, if any.
            // HACK: 
            _elements.Remove(Keys.XRefStm);

            // Don't encrypt myself
            PdfStandardSecurityHandler securityHandler = writer.SecurityHandler;
            writer.SecurityHandler = null;
            base.WriteObject(writer);
            writer.SecurityHandler = securityHandler;
        }

        /// <summary>
        /// Replace temporary irefs by their correct counterparts from the iref table.
        /// </summary>
        internal void Finish()
        {
            // /Root
            PdfReference iref = _document._trailer.Elements[Keys.Root] as PdfReference;
            if (iref != null && iref.Value == null)
            {
                iref = _document._irefTable[iref.ObjectID];
                Debug.Assert(iref.Value != null);
                _document._trailer.Elements[Keys.Root] = iref;
            }

            // /Info
            iref = _document._trailer.Elements[PdfTrailer.Keys.Info] as PdfReference;
            if (iref != null && iref.Value == null)
            {
                iref = _document._irefTable[iref.ObjectID];
                Debug.Assert(iref.Value != null);
                _document._trailer.Elements[Keys.Info] = iref;
            }

            // /Encrypt
            iref = _document._trailer.Elements[Keys.Encrypt] as PdfReference;
            if (iref != null)
            {
                iref = _document._irefTable[iref.ObjectID];
                Debug.Assert(iref.Value != null);
                _document._trailer.Elements[Keys.Encrypt] = iref;

                // The encryption dictionary (security handler) was read in before the XRefTable construction 
                // was completed. The next lines fix that state (it took several hours to find these bugs...).
                iref.Value = _document._trailer._securityHandler;
                _document._trailer._securityHandler.Reference = iref;
                iref.Value.Reference = iref;
            }

            Elements.Remove(Keys.Prev);

            Debug.Assert(_document._irefTable.IsUnderConstruction == false);
            _document._irefTable.IsUnderConstruction = false;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase  // Reference: TABLE 3.13  Entries in the file trailer dictionary / Page 97
        {
            /// <summary>
            /// (Required; must not be an indirect reference) The total number of entries in the file’s 
            /// cross-reference table, as defined by the combination of the original section and all
            /// update sections. Equivalently, this value is 1 greater than the highest object number
            /// used in the file.
            /// Note: Any object in a cross-reference section whose number is greater than this value is
            /// ignored and considered missing.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Size = "/Size";

            /// <summary>
            /// (Present only if the file has more than one cross-reference section; must not be an indirect
            /// reference) The byte offset from the beginning of the file to the beginning of the previous 
            /// cross-reference section.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Prev = "/Prev";

            /// <summary>
            /// (Required; must be an indirect reference) The catalog dictionary for the PDF document
            /// contained in the file.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required, typeof(PdfCatalog))]
            public const string Root = "/Root";

            /// <summary>
            /// (Required if document is encrypted; PDF 1.1) The document’s encryption dictionary.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PdfStandardSecurityHandler))]
            public const string Encrypt = "/Encrypt";

            /// <summary>
            /// (Optional; must be an indirect reference) The document’s information dictionary.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional, typeof(PdfDocumentInformation))]
            public const string Info = "/Info";

            /// <summary>
            /// (Optional, but strongly recommended; PDF 1.1) An array of two strings constituting
            /// a file identifier for the file. Although this entry is optional, 
            /// its absence might prevent the file from functioning in some workflows
            /// that depend on files being uniquely identified.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string ID = "/ID";

            /// <summary>
            /// (Optional) The byte offset from the beginning of the file of a cross-reference stream.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string XRefStm = "/XRefStm";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            public static DictionaryMeta Meta
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
