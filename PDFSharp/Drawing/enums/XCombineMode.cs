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

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Specifies how different clipping regions can be combined.
    /// </summary>
    public enum XCombineMode  // Same values as System.Drawing.Drawing2D.CombineMode.
    {
        /// <summary>
        /// One clipping region is replaced by another.
        /// </summary>
        Replace = 0,

        /// <summary>
        /// Two clipping regions are combined by taking their intersection.
        /// </summary>
        Intersect = 1,

        /// <summary>
        /// Not yet implemented in PDFsharp.
        /// </summary>
        Union = 2,

        /// <summary>
        /// Not yet implemented in PDFsharp.
        /// </summary>
        Xor = 3,

        /// <summary>
        /// Not yet implemented in PDFsharp.
        /// </summary>
        Exclude = 4,

        /// <summary>
        /// Not yet implemented in PDFsharp.
        /// </summary>
        Complement = 5,
    }
}
