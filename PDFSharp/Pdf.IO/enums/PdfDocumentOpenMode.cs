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

namespace PdfSharp.Pdf.IO
{
    /// <summary>
    /// Determines how a PDF document is opened. 
    /// </summary>
    public enum PdfDocumentOpenMode
    {
        /// <summary>
        /// The PDF stream is completely read into memory and can be modified. Pages can be deleted or
        /// inserted, but it is not possible to extract pages. This mode is useful for modifying an
        /// existing PDF document.
        /// </summary>
        Modify,

        /// <summary>
        /// The PDF stream is opened for importing pages from it. A document opened in this mode cannot
        /// be modified.
        /// </summary>
        Import,

        /// <summary>
        /// The PDF stream is completely read into memory, but cannot be modified. This mode preserves the
        /// original internal structure of the document and is useful for analyzing existing PDF files.
        /// </summary>
        ReadOnly,

        /// <summary>
        /// The PDF stream is partially read for information purposes only. The only valid operation is to
        /// call the Info property at the imported document. This option is very fast and needs less memory
        /// and is e.g. useful for browsing information about a collection of PDF documents in a user interface.
        /// </summary>
        InformationOnly,  // TODO: not yet implemented
    }
}
