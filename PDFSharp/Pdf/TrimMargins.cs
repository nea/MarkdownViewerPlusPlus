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

using System.Diagnostics;
using PdfSharp.Drawing;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents trim margins added to the page.
    /// </summary>
    [DebuggerDisplay("(Left={left.Millimeter}mm, Right={right.Millimeter}mm, Top={top.Millimeter}mm, Bottom={bottom.Millimeter}mm)")]
    public sealed class TrimMargins
    {
        ///// <summary>
        ///// Clones this instance.
        ///// </summary>
        //public TrimMargins Clone()
        //{
        //  TrimMargins trimMargins = new TrimMargins();
        //  trimMargins.left = left;
        //  trimMargins.top = top;
        //  trimMargins.right = right;
        //  trimMargins.bottom = bottom;
        //  return trimMargins;
        //}

        /// <summary>
        /// Sets all four crop margins simultaneously.
        /// </summary>
        public XUnit All
        {
            set
            {
                _left = value;
                _right = value;
                _top = value;
                _bottom = value;
            }
        }

        /// <summary>
        /// Gets or sets the left crop margin.
        /// </summary>
        public XUnit Left
        {
            get { return _left; }
            set { _left = value; }
        }
        XUnit _left;

        /// <summary>
        /// Gets or sets the right crop margin.
        /// </summary>
        public XUnit Right
        {
            get { return _right; }
            set { _right = value; }
        }
        XUnit _right;

        /// <summary>
        /// Gets or sets the top crop margin.
        /// </summary>
        public XUnit Top
        {
            get { return _top; }
            set { _top = value; }
        }
        XUnit _top;

        /// <summary>
        /// Gets or sets the bottom crop margin.
        /// </summary>
        public XUnit Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }
        XUnit _bottom;

        /// <summary>
        /// Gets a value indicating whether this instance has at least one margin with a value other than zero.
        /// </summary>
        public bool AreSet
        {
            get { return _left.Value != 0 || _right.Value != 0 || _top.Value != 0 || _bottom.Value != 0; }
        }
    }
}
