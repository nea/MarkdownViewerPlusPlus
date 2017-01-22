#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   David Stephensen
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

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Represents the base class of all 2D codes.
    /// </summary>
    public abstract class MatrixCode : CodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatrixCode"/> class.
        /// </summary>
        public MatrixCode(string text, string encoding, int rows, int columns, XSize size)
            : base(text, size, CodeDirection.LeftToRight)
        {
            _encoding = encoding;
            if (String.IsNullOrEmpty(_encoding))
                _encoding = new String('a', Text.Length);

            if (columns < rows)
            {
                _rows = columns;
                _columns = rows;
            }
            else
            {
                _columns = columns;
                _rows = rows;
            }

            Text = text;
        }

        /// <summary>
        /// Gets or sets the encoding. docDaSt
        /// </summary>
        public string Encoding
        {
            get { return _encoding; }
            set
            {
                _encoding = value;
                _matrixImage = null;
            }
        }
        string _encoding;

        /// <summary>
        /// docDaSt
        /// </summary>
        public int Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                _matrixImage = null;
            }
        }
        int _columns;

        /// <summary>
        /// docDaSt
        /// </summary>
        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                _matrixImage = null;
            }
        }
        int _rows;

        /// <summary>
        /// docDaSt
        /// </summary>
        public new string Text
        {
            get { return base.Text; }
            set
            {
                base.Text = value;
                _matrixImage = null;
            }
        }

        internal XImage MatrixImage
        {
            get { return _matrixImage; }
            set { _matrixImage = value; }
        }
        XImage _matrixImage;

        /// <summary>
        /// When implemented in a derived class renders the 2D code.
        /// </summary>
        protected internal abstract void Render(XGraphics gfx, XBrush brush, XPoint center);

        /// <summary>
        /// Determines whether the specified string can be used as Text for this matrix code type.
        /// </summary>
        protected override void CheckCode(string text)
        { }
    }
}
