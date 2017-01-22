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

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Specifies the page layout to be used by a viewer when the document is opened.
    /// </summary>
    public enum PdfPageLayout
    {
        /// <summary>
        /// Display one page at a time.
        /// </summary>
        SinglePage,

        /// <summary>
        /// Display the pages in one column.
        /// </summary>
        OneColumn,

        /// <summary>
        /// Display the pages in two columns, with oddnumbered pages on the left.
        /// </summary>
        TwoColumnLeft,

        /// <summary>
        /// Display the pages in two columns, with oddnumbered pages on the right.
        /// </summary>
        TwoColumnRight,

        /// <summary>
        /// (PDF 1.5) Display the pages two at a time, with odd-numbered pages on the left.
        /// </summary>
        TwoPageLeft,

        /// <summary>
        /// (PDF 1.5) Display the pages two at a time, with odd-numbered pages on the right.
        /// </summary>
        TwoPageRight,
    }
}
