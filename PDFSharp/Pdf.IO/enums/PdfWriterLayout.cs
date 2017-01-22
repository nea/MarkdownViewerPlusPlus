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
    /// Determines how the PDF output stream is formatted. Even all formats create valid PDF files,
    /// only Compact or Standard should be used for production purposes.
    /// </summary>
    public enum PdfWriterLayout
    {
        /// <summary>
        /// The PDF stream contains no unnecessary characters. This is default in release build.
        /// </summary>
        Compact,

        /// <summary>
        /// The PDF stream contains some superfluous line feeds, but is more readable.
        /// </summary>
        Standard,

        /// <summary>
        /// The PDF stream is indented to reflect the nesting levels of the objects. This is useful
        /// for analyzing PDF files, but increases the size of the file significantly.
        /// </summary>
        Indented,

        /// <summary>
        /// The PDF stream is indented to reflect the nesting levels of the objects and contains additional
        /// information about the PDFsharp objects. Furthermore content streams are not deflated. This 
        /// is useful for debugging purposes only and increases the size of the file significantly.
        /// </summary>
        Verbose,
    }
}
