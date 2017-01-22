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

using System;

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Internal base class for several bar code types.
    /// </summary>
    public abstract class ThickThinBarCode : BarCode  // TODO: The name is not optimal
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThickThinBarCode"/> class.
        /// </summary>
        public ThickThinBarCode(string code, XSize size, CodeDirection direction)
            : base(code, size, direction)
        { }

        internal override void InitRendering(BarCodeRenderInfo info)
        {
            base.InitRendering(info);
            CalcThinBarWidth(info);
            info.BarHeight = Size.Height;
            // HACK in ThickThinBarCode
            if (TextLocation != TextLocation.None)
                info.BarHeight *= 4.0 / 5;

#if DEBUG_
            XColor back = XColors.LightSalmon;
            back.A = 0.3;
            XSolidBrush brush = new XSolidBrush(back);
            info.Gfx.DrawRectangle(brush, new XRect(info.Center - size / 2, size));
#endif
            switch (Direction)
            {
                case CodeDirection.RightToLeft:
                    info.Gfx.RotateAtTransform(180, info.Position);
                    break;

                case CodeDirection.TopToBottom:
                    info.Gfx.RotateAtTransform(90, info.Position);
                    break;

                case CodeDirection.BottomToTop:
                    info.Gfx.RotateAtTransform(-90, info.Position);
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the ration between thick an thin lines. Must be between 2 and 3.
        /// Optimal and also default value is 2.6.
        /// </summary>
        public override double WideNarrowRatio
        {
            get { return _wideNarrowRatio; }
            set
            {
                if (value > 3 || value < 2)
                    throw new ArgumentOutOfRangeException("value", BcgSR.Invalid2of5Relation);
                _wideNarrowRatio = value;
            }
        }
        double _wideNarrowRatio = 2.6;

        /// <summary>
        /// Renders a thick or thin line for the bar code.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isThick">Determines whether a thick or a thin line is about to be rendered.</param>
        internal void RenderBar(BarCodeRenderInfo info, bool isThick)
        {
            double barWidth = GetBarWidth(info, isThick);
            double height = Size.Height;
            double xPos = info.CurrPos.X;
            double yPos = info.CurrPos.Y;

            switch (TextLocation)
            {
                case TextLocation.AboveEmbedded:
                    height -= info.Gfx.MeasureString(Text, info.Font).Height;
                    yPos += info.Gfx.MeasureString(Text, info.Font).Height;
                    break;
                case TextLocation.BelowEmbedded:
                    height -= info.Gfx.MeasureString(Text, info.Font).Height;
                    break;
            }

            XRect rect = new XRect(xPos, yPos, barWidth, height);
            info.Gfx.DrawRectangle(info.Brush, rect);
            info.CurrPos.X += barWidth;
        }

        /// <summary>
        /// Renders a thick or thin gap for the bar code.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isThick">Determines whether a thick or a thin gap is about to be rendered.</param>
        internal void RenderGap(BarCodeRenderInfo info, bool isThick)
        {
            info.CurrPos.X += GetBarWidth(info, isThick);
        }

        /// <summary>
        /// Renders a thick bar before or behind the code.
        /// </summary>
        internal void RenderTurboBit(BarCodeRenderInfo info, bool startBit)
        {
            if (startBit)
                info.CurrPos.X -= 0.5 + GetBarWidth(info, true);
            else
                info.CurrPos.X += 0.5; //GetBarWidth(info, true);

            RenderBar(info, true);

            if (startBit)
                info.CurrPos.X += 0.5; //GetBarWidth(info, true);
        }

        internal void RenderText(BarCodeRenderInfo info)
        {
            if (info.Font == null)
                info.Font = new XFont("Courier New", Size.Height / 6);
            XPoint center = info.Position + CalcDistance(Anchor, AnchorType.TopLeft, Size);

            switch (TextLocation)
            {
                case TextLocation.Above:
                    center = new XPoint(center.X, center.Y - info.Gfx.MeasureString(Text, info.Font).Height);
                    info.Gfx.DrawString(Text, info.Font, info.Brush, new XRect(center, Size), XStringFormats.TopCenter);
                    break;
                case TextLocation.AboveEmbedded:
                    info.Gfx.DrawString(Text, info.Font, info.Brush, new XRect(center, Size), XStringFormats.TopCenter);
                    break;
                case TextLocation.Below:
                    center = new XPoint(center.X, info.Gfx.MeasureString(Text, info.Font).Height + center.Y);
                    info.Gfx.DrawString(Text, info.Font, info.Brush, new XRect(center, Size), XStringFormats.BottomCenter);
                    break;
                case TextLocation.BelowEmbedded:
                    info.Gfx.DrawString(Text, info.Font, info.Brush, new XRect(center, Size), XStringFormats.BottomCenter);
                    break;
            }
        }

        /// <summary>
        /// Gets the width of a thick or a thin line (or gap). CalcLineWidth must have been called before.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isThick">Determines whether a thick line's with shall be returned.</param>
        internal double GetBarWidth(BarCodeRenderInfo info, bool isThick)
        {
            if (isThick)
                return info.ThinBarWidth * _wideNarrowRatio;
            return info.ThinBarWidth;
        }

        internal abstract void CalcThinBarWidth(BarCodeRenderInfo info);
    }
}
