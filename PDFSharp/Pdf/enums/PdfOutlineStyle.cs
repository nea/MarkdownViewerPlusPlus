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

// Review: OK - StL/14-10-05

using System;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Specifies the font style for the outline (bookmark) text.
    ///  </summary>
    [Flags]
    public enum PdfOutlineStyle  // Reference:  TABLE 8.5 Ouline Item flags / Page 587
    {
        /// <summary>
        /// Outline text is displayed using a regular font.
        /// </summary>
        Regular = 0,

        /// <summary>
        /// Outline text is displayed using an italic font.
        /// </summary>
        Italic = 1,

        /// <summary>
        /// Outline text is displayed using a bold font.
        /// </summary>
        Bold = 2,

        /// <summary>
        /// Outline text is displayed using a bold and italic font.
        /// </summary>
        BoldItalic = 3,
    }
}
