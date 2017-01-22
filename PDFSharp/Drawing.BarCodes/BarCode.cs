#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Klaus Potzesny
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
using System.ComponentModel;

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Represents the base class of all bar codes.
    /// </summary>
    public abstract class BarCode : CodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BarCode"/> class.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <param name="direction"></param>
        public BarCode(string text, XSize size, CodeDirection direction)
            : base(text, size, direction)
        {
            Text = text;
            Size = size;
            Direction = direction;
        }

        /// <summary>
        /// Creates a bar code from the specified code type.
        /// </summary>
        public static BarCode FromType(CodeType type, string text, XSize size, CodeDirection direction)
        {
            switch (type)
            {
                case CodeType.Code2of5Interleaved:
                    return new Code2of5Interleaved(text, size, direction);

                case CodeType.Code3of9Standard:
                    return new Code3of9Standard(text, size, direction);

                default:
                    throw new InvalidEnumArgumentException("type", (int)type, typeof(CodeType));
            }
        }

        /// <summary>
        /// Creates a bar code from the specified code type.
        /// </summary>
        public static BarCode FromType(CodeType type, string text, XSize size)
        {
            return FromType(type, text, size, CodeDirection.LeftToRight);
        }

        /// <summary>
        /// Creates a bar code from the specified code type.
        /// </summary>
        public static BarCode FromType(CodeType type, string text)
        {
            return FromType(type, text, XSize.Empty, CodeDirection.LeftToRight);
        }

        /// <summary>
        /// Creates a bar code from the specified code type.
        /// </summary>
        public static BarCode FromType(CodeType type)
        {
            return FromType(type, String.Empty, XSize.Empty, CodeDirection.LeftToRight);
        }

        /// <summary>
        /// When overridden in a derived class gets or sets the wide narrow ratio.
        /// </summary>
        public virtual double WideNarrowRatio
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// Gets or sets the location of the text next to the bar code.
        /// </summary>
        public TextLocation TextLocation
        {
            get { return _textLocation; }
            set { _textLocation = value; }
        }
        TextLocation _textLocation;

        /// <summary>
        /// Gets or sets the length of the data that defines the bar code.
        /// </summary>
        public int DataLength
        {
            get { return _dataLength; }
            set { _dataLength = value; }
        }
        int _dataLength;

        /// <summary>
        /// Gets or sets the optional start character.
        /// </summary>
        public char StartChar
        {
            get { return _startChar; }
            set { _startChar = value; }
        }
        char _startChar;

        /// <summary>
        /// Gets or sets the optional end character.
        /// </summary>
        public char EndChar
        {
            get { return _endChar; }
            set { _endChar = value; }
        }
        char _endChar;

        /// <summary>
        /// Gets or sets a value indicating whether the turbo bit is to be drawn.
        /// (A turbo bit is something special to Kern (computer output processing) company (as far as I know))
        /// </summary>
        public virtual bool TurboBit
        {
            get { return _turboBit; }
            set { _turboBit = value; }
        }
        bool _turboBit;

        internal virtual void InitRendering(BarCodeRenderInfo info)
        {
            if (Text == null)
                throw new InvalidOperationException(BcgSR.BarCodeNotSet);

            if (Size.IsEmpty)
                throw new InvalidOperationException(BcgSR.EmptyBarCodeSize);
        }

        /// <summary>
        /// When defined in a derived class renders the code.
        /// </summary>
        protected internal abstract void Render(XGraphics gfx, XBrush brush, XFont font, XPoint position);
    }
}
