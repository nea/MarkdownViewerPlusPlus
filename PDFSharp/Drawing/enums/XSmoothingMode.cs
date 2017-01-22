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

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Specifies whether smoothing (or antialiasing) is applied to lines and curves
    /// and the edges of filled areas.
    /// </summary>
    [Flags]
    public enum XSmoothingMode  // same values as System.Drawing.Drawing2D.SmoothingMode
    {
        // Not used in PDF rendering process.

        /// <summary>
        /// Specifies an invalid mode.
        /// </summary>
        Invalid = -1,

        /// <summary>
        /// Specifies the default mode.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Specifies high speed, low quality rendering.
        /// </summary>
        HighSpeed = 1,

        /// <summary>
        /// Specifies high quality, low speed rendering.
        /// </summary>
        HighQuality = 2,

        /// <summary>
        /// Specifies no antialiasing.
        /// </summary>
        None = 3,

        /// <summary>
        /// Specifies antialiased rendering.
        /// </summary>
        AntiAlias = 4,
    }
}
