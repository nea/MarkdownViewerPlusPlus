//
// PDFsharp - A library for processing PDF
//
// Authors:
//   Klaus Potzesny
//
// Copyright (c) 2005-2016 empira Software GmbH, Cologne Area (Germany)
//
// http://www.pdfsharp.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Holds all temporary information needed during rendering.
    /// </summary>
    class BarCodeRenderInfo
    {
        public BarCodeRenderInfo(XGraphics gfx, XBrush brush, XFont font, XPoint position)
        {
            Gfx = gfx;
            Brush = brush;
            Font = font;
            Position = position;
        }

        public XGraphics Gfx;
        public XBrush Brush;
        public XFont Font;
        public XPoint Position;
        public double BarHeight;
        public XPoint CurrPos;
        public int CurrPosInString;
        public double ThinBarWidth;
    }
}
