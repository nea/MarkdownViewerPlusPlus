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

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Implementation of the Code 2 of 5 bar code.
    /// </summary>
    public class Code2of5Interleaved : ThickThinBarCode
    {
        /// <summary>
        /// Initializes a new instance of Interleaved2of5.
        /// </summary>
        public Code2of5Interleaved()
            : base("", XSize.Empty, CodeDirection.LeftToRight)
        {}

        /// <summary>
        /// Initializes a new instance of Interleaved2of5.
        /// </summary>
        public Code2of5Interleaved(string code)
            : base(code, XSize.Empty, CodeDirection.LeftToRight)
        {}

        /// <summary>
        /// Initializes a new instance of Interleaved2of5.
        /// </summary>
        public Code2of5Interleaved(string code, XSize size)
            : base(code, size, CodeDirection.LeftToRight)
        {}

        /// <summary>
        /// Initializes a new instance of Interleaved2of5.
        /// </summary>
        public Code2of5Interleaved(string code, XSize size, CodeDirection direction)
            : base(code, size, direction)
        {}

        /// <summary>
        /// Returns an array of size 5 that represents the thick (true) and thin (false) lines or spaces
        /// representing the specified digit.
        /// </summary>
        /// <param name="digit">The digit to represent.</param>
        static bool[] ThickAndThinLines(int digit)
        {
            return Lines[digit];
        }
        static bool[][] Lines = 
        {
            new bool[] {false, false, true, true, false},
            new bool[] {true, false, false, false, true},
            new bool[] {false, true, false, false, true},
            new bool[] {true, true, false, false, false},
            new bool[] {false, false, true, false, true},
            new bool[] {true, false, true, false, false},
            new bool[] {false, true, true, false, false},
            new bool[] {false, false, false, true, true},
            new bool[] {true, false, false, true, false},
            new bool[] {false, true, false, true, false},
        };

        /// <summary>
        /// Renders the bar code.
        /// </summary>
        protected internal override void Render(XGraphics gfx, XBrush brush, XFont font, XPoint position)
        {
            XGraphicsState state = gfx.Save();

            BarCodeRenderInfo info = new BarCodeRenderInfo(gfx, brush, font, position);
            InitRendering(info);
            info.CurrPosInString = 0;
            //info.CurrPos = info.Center - Size / 2;
            info.CurrPos = position - CodeBase.CalcDistance(AnchorType.TopLeft, Anchor, Size);

            if (TurboBit)
                RenderTurboBit(info, true);
            RenderStart(info);
            while (info.CurrPosInString < Text.Length)
                RenderNextPair(info);
            RenderStop(info);
            if (TurboBit)
                RenderTurboBit(info, false);
            if (TextLocation != TextLocation.None)
                RenderText(info);

            gfx.Restore(state);
        }

        /// <summary>
        /// Calculates the thick and thin line widths,
        /// taking into account the required rendering size.
        /// </summary>
        internal override void CalcThinBarWidth(BarCodeRenderInfo info)
        {
            /*
             * The total width is the sum of the following parts:
             * Starting lines      = 4 * thin
             *  +
             * Code Representation = (2 * thick + 3 * thin) * code.Length
             *  +
             * Stopping lines      =  1 * thick + 2 * thin
             * 
             * with r = relation ( = thick / thin), this results in
             * 
             * Total width = (6 + r + (2 * r + 3) * text.Length) * thin
             */
            double thinLineAmount = 6 + WideNarrowRatio + (2 * WideNarrowRatio + 3) * Text.Length;
            info.ThinBarWidth = Size.Width / thinLineAmount;
        }

        private void RenderStart(BarCodeRenderInfo info)
        {
            RenderBar(info, false);
            RenderGap(info, false);
            RenderBar(info, false);
            RenderGap(info, false);
        }

        private void RenderStop(BarCodeRenderInfo info)
        {
            RenderBar(info, true);
            RenderGap(info, false);
            RenderBar(info, false);
        }

        /// <summary>
        /// Renders the next digit pair as bar code element.
        /// </summary>
        private void RenderNextPair(BarCodeRenderInfo info)
        {
            int digitForLines = int.Parse(Text[info.CurrPosInString].ToString());
            int digitForGaps = int.Parse(Text[info.CurrPosInString + 1].ToString());
            bool[] linesArray = Lines[digitForLines];
            bool[] gapsArray = Lines[digitForGaps];
            for (int idx = 0; idx < 5; ++idx)
            {
                RenderBar(info, linesArray[idx]);
                RenderGap(info, gapsArray[idx]);
            }
            info.CurrPosInString += 2;
        }

        /// <summary>
        /// Checks the code to be convertible into an interleaved 2 of 5 bar code.
        /// </summary>
        /// <param name="text">The code to be checked.</param>
        protected override void CheckCode(string text)
        {
#if true_
      if (text == null)
        throw new ArgumentNullException("text");

      if (text == "")
        throw new ArgumentException(BcgSR.Invalid2Of5Code(text));

      if (text.Length % 2 != 0)
        throw new ArgumentException(BcgSR.Invalid2Of5Code(text));

      foreach (char ch in text)
      {
        if (!Char.IsDigit(ch))
          throw new ArgumentException(BcgSR.Invalid2Of5Code(text));
      }
#endif
        }
    }
}
