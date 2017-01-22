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

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents the PDF document information dictionary.
    /// </summary>
    public sealed class PdfDocumentInformation : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfDocumentInformation"/> class.
        /// </summary>
        public PdfDocumentInformation(PdfDocument document)
            : base(document)
        { }

        internal PdfDocumentInformation(PdfDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets or sets the document's title.
        /// </summary>
        public string Title
        {
            get { return Elements.GetString(Keys.Title); }
            set { Elements.SetString(Keys.Title, value); }
        }

        /// <summary>
        /// Gets or sets the name of the person who created the document.
        /// </summary>
        public string Author
        {
            get { return Elements.GetString(Keys.Author); }
            set { Elements.SetString(Keys.Author, value); }
        }

        /// <summary>
        /// Gets or sets the name of the subject of the document.
        /// </summary>
        public string Subject
        {
            get { return Elements.GetString(Keys.Subject); }
            set { Elements.SetString(Keys.Subject, value); }
        }

        /// <summary>
        /// Gets or sets keywords associated with the document.
        /// </summary>
        public string Keywords
        {
            get { return Elements.GetString(Keys.Keywords); }
            set { Elements.SetString(Keys.Keywords, value); }
        }

        /// <summary>
        /// Gets or sets the name of the application (for example, MigraDoc) that created the document.
        /// </summary>
        public string Creator
        {
            get { return Elements.GetString(Keys.Creator); }
            set { Elements.SetString(Keys.Creator, value); }
        }

        /// <summary>
        /// Gets the producer application (for example, PDFsharp).
        /// </summary>
        public string Producer
        {
            get { return Elements.GetString(Keys.Producer); }
        }

        /// <summary>
        /// Gets or sets the creation date of the document.
        /// Breaking Change: If the date is not set in a PDF file DateTime.MinValue is returned.
        /// </summary>
        public DateTime CreationDate
        {
            get { return Elements.GetDateTime(Keys.CreationDate, DateTime.MinValue); }
            set { Elements.SetDateTime(Keys.CreationDate, value); }
        }

        /// <summary>
        /// Gets or sets the modification date of the document.
        /// Breaking Change: If the date is not set in a PDF file DateTime.MinValue is returned.
        /// </summary>
        public DateTime ModificationDate
        {
            get { return Elements.GetDateTime(Keys.ModDate, DateTime.MinValue); }
            set { Elements.SetDateTime(Keys.ModDate, value); }
        }

        // TODO CustomProperties and meta data

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : KeysBase
        {
            /// <summary>
            /// (Optional; PDF 1.1) The document’s title.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string Title = "/Title";

            /// <summary>
            /// (Optional) The name of the person who created the document.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string Author = "/Author";

            /// <summary>
            /// (Optional; PDF 1.1) The subject of the document.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string Subject = "/Subject";

            /// <summary>
            /// (Optional; PDF 1.1) Keywords associated with the document.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string Keywords = "/Keywords";

            /// <summary>
            /// (Optional) If the document was converted to PDF from another format,
            /// the name of the application (for example, empira MigraDoc) that created the
            /// original document from which it was converted.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string Creator = "/Creator";

            /// <summary>
            /// (Optional) If the document was converted to PDF from another format,
            /// the name of the application (for example, this library) that converted it to PDF.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string Producer = "/Producer";

            /// <summary>
            /// (Optional) The date and time the document was created, in human-readable form.
            /// </summary>
            [KeyInfo(KeyType.Date | KeyType.Optional)]
            public const string CreationDate = "/CreationDate";

            /// <summary>
            /// (Required if PieceInfo is present in the document catalog; otherwise optional; PDF 1.1)
            /// The date and time the document was most recently modified, in human-readable form.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string ModDate = "/ModDate";

            /// <summary>
            /// (Optional; PDF 1.3) A name object indicating whether the document has been modified 
            /// to include trapping information.
            /// </summary>
            [KeyInfo("1.3", KeyType.Name | KeyType.Optional)]
            public const string Trapped = "/Trapped";

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
