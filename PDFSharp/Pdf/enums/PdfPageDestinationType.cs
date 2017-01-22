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

// ReSharper disable InconsistentNaming because we use PDF names.

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Specifies the type of a page destination in outline items, annotations, or actions..
    ///  </summary>
    public enum PdfPageDestinationType  // Reference: TABLE 8.2  Destination Syntax / Page 582
    {
        // Except for FitR the documentation text is outdated.

        /// <summary>
        /// Display the page with the coordinates (left, top) positioned at  the upper-left corner of
        /// the window and the contents of the page magnified by the factor zoom.
        /// </summary>
        Xyz,

        /// <summary>
        /// Display the page with its contents magnified just enough to fit the 
        /// entire page within the window both horizontally and vertically.
        /// </summary>
        Fit,

        /// <summary>
        /// Display the page with the vertical coordinate top positioned at the top edge of 
        /// the window and the contents of the page magnified just enough to fit the entire
        /// width of the page within the window.
        /// </summary>
        FitH,

        /// <summary>
        /// Display the page with the horizontal coordinate left positioned at the left edge of 
        /// the window and the contents of the page magnified just enough to fit the entire
        /// height of the page within the window.
        /// </summary>
        FitV,

        /// <summary>
        /// Display the page designated by page, with its contents magnified just enough to
        /// fit the rectangle specified by the coordinates left, bottom, right, and topentirely
        /// within the window both horizontally and vertically. If the required horizontal and
        /// vertical magnification factors are different, use the smaller of the two, centering
        /// the rectangle within the window in the other dimension. A null value for any of
        /// the parameters may result in unpredictable behavior.
        /// </summary>
        FitR,

        /// <summary>
        /// Display the page with its contents magnified just enough to fit the rectangle specified
        /// by the coordinates left, bottom, right, and topentirely within the window both 
        /// horizontally and vertically.
        /// </summary>
        FitB,

        /// <summary>
        /// Display the page with the vertical coordinate top positioned at the top edge of
        /// the window and the contents of the page magnified just enough to fit the entire
        /// width of its bounding box within the window.
        /// </summary>
        FitBH,

        /// <summary>
        /// Display the page with the horizontal coordinate left positioned at the left edge of
        /// the window and the contents of the page magnified just enough to fit the entire
        /// height of its bounding box within the window.
        /// </summary>
        FitBV,
    }
}
