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

#define ITALIC_SIMULATION

using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
#endif
#if NETFX_CORE
using Windows.UI.Xaml.Media;
using SysPoint = Windows.Foundation.Point;
using SysSize = Windows.Foundation.Size;
#endif
using PdfSharp.Fonts.OpenType;
using PdfSharp.Internal;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Internal;
using PdfSharp.Pdf.Advanced;

// ReSharper disable RedundantNameQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace PdfSharp.Drawing.Pdf
{
    /// <summary>
    /// Represents a drawing surface for PdfPages.
    /// </summary>
    internal class XGraphicsPdfRenderer : IXGraphicsRenderer
    {
        public XGraphicsPdfRenderer(PdfPage page, XGraphics gfx, XGraphicsPdfPageOptions options)
        {
            _page = page;
            _colorMode = page._document.Options.ColorMode;
            _options = options;
            _gfx = gfx;
            _content = new StringBuilder();
            page.RenderContent._pdfRenderer = this;
            _gfxState = new PdfGraphicsState(this);
        }

        public XGraphicsPdfRenderer(XForm form, XGraphics gfx)
        {
            _form = form;
            _colorMode = form.Owner.Options.ColorMode;
            _gfx = gfx;
            _content = new StringBuilder();
            form.PdfRenderer = this;
            _gfxState = new PdfGraphicsState(this);
        }

        /// <summary>
        /// Gets the content created by this renderer.
        /// </summary>
        string GetContent()
        {
            EndPage();
            return _content.ToString();
        }

        public XGraphicsPdfPageOptions PageOptions
        {
            get { return _options; }
        }

        public void Close()
        {
            if (_page != null)
            {
                PdfContent content2 = _page.RenderContent;
                content2.CreateStream(PdfEncoders.RawEncoding.GetBytes(GetContent()));

                _gfx = null;
                _page.RenderContent._pdfRenderer = null;
                _page.RenderContent = null;
                _page = null;
            }
            else if (_form != null)
            {
                _form._pdfForm.CreateStream(PdfEncoders.RawEncoding.GetBytes(GetContent()));
                _gfx = null;
                _form.PdfRenderer = null;
                _form = null;
            }
        }

        // --------------------------------------------------------------------------------------------

        #region  Drawing

        //void SetPageLayout(down, point(0, 0), unit

        // ----- DrawLine -----------------------------------------------------------------------------

        /// <summary>
        /// Strokes a single connection of two points.
        /// </summary>
        public void DrawLine(XPen pen, double x1, double y1, double x2, double y2)
        {
            DrawLines(pen, new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2) });
        }

        // ----- DrawLines ----------------------------------------------------------------------------

        /// <summary>
        /// Strokes a series of connected points.
        /// </summary>
        public void DrawLines(XPen pen, XPoint[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            int count = points.Length;
            if (count == 0)
                return;

            Realize(pen);

            const string format = Config.SignificantFigures4;
            AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", points[0].X, points[0].Y);
            for (int idx = 1; idx < count; idx++)
                AppendFormatPoint("{0:" + format + "} {1:" + format + "} l\n", points[idx].X, points[idx].Y);
            _content.Append("S\n");
        }

        // ----- DrawBezier ---------------------------------------------------------------------------

        public void DrawBezier(XPen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4)
        {
            DrawBeziers(pen, new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2), new XPoint(x3, y3), new XPoint(x4, y4) });
        }

        // ----- DrawBeziers --------------------------------------------------------------------------

        public void DrawBeziers(XPen pen, XPoint[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            int count = points.Length;
            if (count == 0)
                return;

            if ((count - 1) % 3 != 0)
                throw new ArgumentException("Invalid number of points for bezier curves. Number must fulfil 4+3n.", "points");

            Realize(pen);

            const string format = Config.SignificantFigures4;
            AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", points[0].X, points[0].Y);
            for (int idx = 1; idx < count; idx += 3)
                AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
                    points[idx].X, points[idx].Y,
                    points[idx + 1].X, points[idx + 1].Y,
                    points[idx + 2].X, points[idx + 2].Y);

            AppendStrokeFill(pen, null, XFillMode.Alternate, false);
        }

        // ----- DrawCurve ----------------------------------------------------------------------------

        public void DrawCurve(XPen pen, XPoint[] points, double tension)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            int count = points.Length;
            if (count == 0)
                return;
            if (count < 2)
                throw new ArgumentException("Not enough points", "points");

            // See http://pubpages.unh.edu/~cs770/a5/cardinal.html  // Link is down...
            tension /= 3;

            Realize(pen);

            const string format = Config.SignificantFigures4;
            AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", points[0].X, points[0].Y);
            if (count == 2)
            {
                // Just draws a line.
                AppendCurveSegment(points[0], points[0], points[1], points[1], tension);
            }
            else
            {
                AppendCurveSegment(points[0], points[0], points[1], points[2], tension);
                for (int idx = 1; idx < count - 2; idx++)
                    AppendCurveSegment(points[idx - 1], points[idx], points[idx + 1], points[idx + 2], tension);
                AppendCurveSegment(points[count - 3], points[count - 2], points[count - 1], points[count - 1], tension);
            }
            AppendStrokeFill(pen, null, XFillMode.Alternate, false);
        }

        // ----- DrawArc ------------------------------------------------------------------------------

        public void DrawArc(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            Realize(pen);

            AppendPartialArc(x, y, width, height, startAngle, sweepAngle, PathStart.MoveTo1st, new XMatrix());
            AppendStrokeFill(pen, null, XFillMode.Alternate, false);
        }

        // ----- DrawRectangle ------------------------------------------------------------------------

        public void DrawRectangle(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush");

            const string format = Config.SignificantFigures3;

            Realize(pen, brush);
            //AppendFormat123("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} re\n", x, y, width, -height);
            AppendFormatRect("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} re\n", x, y + height, width, height);

            if (pen != null && brush != null)
                _content.Append("B\n");
            else if (pen != null)
                _content.Append("S\n");
            else
                _content.Append("f\n");
        }

        // ----- DrawRectangles -----------------------------------------------------------------------

        public void DrawRectangles(XPen pen, XBrush brush, XRect[] rects)
        {
            int count = rects.Length;
            for (int idx = 0; idx < count; idx++)
            {
                XRect rect = rects[idx];
                DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        // ----- DrawRoundedRectangle -----------------------------------------------------------------

        public void DrawRoundedRectangle(XPen pen, XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRoundedRectangle(x, y, width, height, ellipseWidth, ellipseHeight);
            DrawPath(pen, brush, path);
        }

        // ----- DrawEllipse --------------------------------------------------------------------------

        public void DrawEllipse(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            Realize(pen, brush);

            // Useful information is here http://home.t-online.de/home/Robert.Rossmair/ellipse.htm (note: link was dead on November 2, 2015)
            // or here http://www.whizkidtech.redprince.net/bezier/circle/
            // Deeper but more difficult: http://www.tinaja.com/cubic01.asp
            XRect rect = new XRect(x, y, width, height);
            double δx = rect.Width / 2;
            double δy = rect.Height / 2;
            double fx = δx * Const.κ;
            double fy = δy * Const.κ;
            double x0 = rect.X + δx;
            double y0 = rect.Y + δy;

            // Approximate an ellipse by drawing four cubic splines.
            const string format = Config.SignificantFigures4;
            AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", x0 + δx, y0);
            AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
              x0 + δx, y0 + fy, x0 + fx, y0 + δy, x0, y0 + δy);
            AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
              x0 - fx, y0 + δy, x0 - δx, y0 + fy, x0 - δx, y0);
            AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
              x0 - δx, y0 - fy, x0 - fx, y0 - δy, x0, y0 - δy);
            AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
              x0 + fx, y0 - δy, x0 + δx, y0 - fy, x0 + δx, y0);
            AppendStrokeFill(pen, brush, XFillMode.Winding, true);
        }

        // ----- DrawPolygon --------------------------------------------------------------------------

        public void DrawPolygon(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            Realize(pen, brush);

            int count = points.Length;
            if (points.Length < 2)
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));

            const string format = Config.SignificantFigures4;
            AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", points[0].X, points[0].Y);
            for (int idx = 1; idx < count; idx++)
                AppendFormatPoint("{0:" + format + "} {1:" + format + "} l\n", points[idx].X, points[idx].Y);

            AppendStrokeFill(pen, brush, fillmode, true);
        }

        // ----- DrawPie ------------------------------------------------------------------------------

        public void DrawPie(XPen pen, XBrush brush, double x, double y, double width, double height,
          double startAngle, double sweepAngle)
        {
            Realize(pen, brush);

            const string format = Config.SignificantFigures4;
            AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", x + width / 2, y + height / 2);
            AppendPartialArc(x, y, width, height, startAngle, sweepAngle, PathStart.LineTo1st, new XMatrix());
            AppendStrokeFill(pen, brush, XFillMode.Alternate, true);
        }

        // ----- DrawClosedCurve ----------------------------------------------------------------------

        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, double tension, XFillMode fillmode)
        {
            int count = points.Length;
            if (count == 0)
                return;
            if (count < 2)
                throw new ArgumentException("Not enough points.", "points");

            // Simply tried out. Not proofed why it is correct.
            tension /= 3;

            Realize(pen, brush);

            const string format = Config.SignificantFigures4;
            AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", points[0].X, points[0].Y);
            if (count == 2)
            {
                // Just draw a line.
                AppendCurveSegment(points[0], points[0], points[1], points[1], tension);
            }
            else
            {
                AppendCurveSegment(points[count - 1], points[0], points[1], points[2], tension);
                for (int idx = 1; idx < count - 2; idx++)
                    AppendCurveSegment(points[idx - 1], points[idx], points[idx + 1], points[idx + 2], tension);
                AppendCurveSegment(points[count - 3], points[count - 2], points[count - 1], points[0], tension);
                AppendCurveSegment(points[count - 2], points[count - 1], points[0], points[1], tension);
            }
            AppendStrokeFill(pen, brush, fillmode, true);
        }

        // ----- DrawPath -----------------------------------------------------------------------------

        public void DrawPath(XPen pen, XBrush brush, XGraphicsPath path)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen");

#if CORE
            Realize(pen, brush);
            AppendPath(path._corePath);
            AppendStrokeFill(pen, brush, path.FillMode, false);
#endif
#if GDI && !WPF
            Realize(pen, brush);
            AppendPath(path._gdipPath);
            AppendStrokeFill(pen, brush, path.FillMode, false);
#endif
#if WPF && !GDI
            Realize(pen, brush);
            AppendPath(path._pathGeometry);
            AppendStrokeFill(pen, brush, path.FillMode, false);
#endif
#if WPF && GDI
            Realize(pen, brush);
            if (_gfx.TargetContext == XGraphicTargetContext.GDI)
                AppendPath(path._gdipPath);
            else
                AppendPath(path._pathGeometry);
            AppendStrokeFill(pen, brush, path.FillMode, false);
#endif
#if NETFX_CORE
            Realize(pen, brush);
            AppendPath(path._pathGeometry);
            AppendStrokeFill(pen, brush, path.FillMode, false);
#endif
        }

        // ----- DrawString ---------------------------------------------------------------------------

        public void DrawString(string s, XFont font, XBrush brush, XRect rect, XStringFormat format)
        {
            double x = rect.X;
            double y = rect.Y;

            double lineSpace = font.GetHeight();
            double cyAscent = lineSpace * font.CellAscent / font.CellSpace;
            double cyDescent = lineSpace * font.CellDescent / font.CellSpace;
            double width = _gfx.MeasureString(s, font).Width;

            //bool bold = (font.Style & XFontStyle.Bold) != 0;
            //bool italic = (font.Style & XFontStyle.Italic) != 0;
            bool italicSimulation = (font.GlyphTypeface.StyleSimulations & XStyleSimulations.ItalicSimulation) != 0;
            bool boldSimulation = (font.GlyphTypeface.StyleSimulations & XStyleSimulations.BoldSimulation) != 0;
            bool strikeout = (font.Style & XFontStyle.Strikeout) != 0;
            bool underline = (font.Style & XFontStyle.Underline) != 0;

            Realize(font, brush, boldSimulation ? 2 : 0);

            switch (format.Alignment)
            {
                case XStringAlignment.Near:
                    // nothing to do
                    break;

                case XStringAlignment.Center:
                    x += (rect.Width - width) / 2;
                    break;

                case XStringAlignment.Far:
                    x += rect.Width - width;
                    break;
            }
            if (Gfx.PageDirection == XPageDirection.Downwards)
            {
                switch (format.LineAlignment)
                {
                    case XLineAlignment.Near:
                        y += cyAscent;
                        break;

                    case XLineAlignment.Center:
                        // TODO: Use CapHeight. PDFlib also uses 3/4 of ascent
                        y += (cyAscent * 3 / 4) / 2 + rect.Height / 2;
                        break;

                    case XLineAlignment.Far:
                        y += -cyDescent + rect.Height;
                        break;

                    case XLineAlignment.BaseLine:
                        // Nothing to do.
                        break;
                }
            }
            else
            {
                switch (format.LineAlignment)
                {
                    case XLineAlignment.Near:
                        y += cyDescent;
                        break;

                    case XLineAlignment.Center:
                        // TODO: Use CapHeight. PDFlib also uses 3/4 of ascent
                        y += -(cyAscent * 3 / 4) / 2 + rect.Height / 2;
                        break;

                    case XLineAlignment.Far:
                        y += -cyAscent + rect.Height;
                        break;

                    case XLineAlignment.BaseLine:
                        // Nothing to do.
                        break;
                }
            }

            PdfFont realizedFont = _gfxState._realizedFont;
            Debug.Assert(realizedFont != null);
            realizedFont.AddChars(s);

            const string format2 = Config.SignificantFigures4;
            OpenTypeDescriptor descriptor = realizedFont.FontDescriptor._descriptor;

            string text = null;
            if (font.Unicode)
            {
                StringBuilder sb = new StringBuilder();
                bool isSymbolFont = descriptor.FontFace.cmap.symbol;
                for (int idx = 0; idx < s.Length; idx++)
                {
                    char ch = s[idx];
                    if (isSymbolFont)
                    {
                        // Remap ch for symbol fonts.
                        ch = (char)(ch | (descriptor.FontFace.os2.usFirstCharIndex & 0xFF00));  // @@@ refactor
                    }
                    int glyphID = descriptor.CharCodeToGlyphIndex(ch);
                    sb.Append((char)glyphID);
                }
                s = sb.ToString();

                byte[] bytes = PdfEncoders.RawUnicodeEncoding.GetBytes(s);
                bytes = PdfEncoders.FormatStringLiteral(bytes, true, false, true, null);
                text = PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
            }
            else
            {
                byte[] bytes = PdfEncoders.WinAnsiEncoding.GetBytes(s);
                text = PdfEncoders.ToStringLiteral(bytes, false, null);
            }

            // Map absolute position to PDF world space.
            XPoint pos = new XPoint(x, y);
            pos = WorldToView(pos);

            double verticalOffset = 0;
            if (boldSimulation)
            {
                // Adjust baseline in case of bold simulation???
                // No, because this would change the center of the glyphs.
                //verticalOffset = font.Size * Const.BoldEmphasis / 2;
            }

#if ITALIC_SIMULATION
            if (italicSimulation)
            {
                if (_gfxState.ItalicSimulationOn)
                {
                    AdjustTdOffset(ref pos, verticalOffset, true);
                    AppendFormatArgs("{0:" + format2 + "} {1:" + format2 + "} Td\n{2} Tj\n", pos.X, pos.Y, text);
                }
                else
                {
                    // Italic simulation is done by skewing characters 20° to the right.
                    XMatrix m = new XMatrix(1, 0, Const.ItalicSkewAngleSinus, 1, pos.X, pos.Y);
                    AppendFormatArgs("{0:" + format2 + "} {1:" + format2 + "} {2:" + format2 + "} {3:" + format2 + "} {4:" + format2 + "} {5:" + format2 + "} Tm\n{6} Tj\n",
                        m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY, text);
                    _gfxState.ItalicSimulationOn = true;
                    AdjustTdOffset(ref pos, verticalOffset, false);
                }
            }
            else
            {
                if (_gfxState.ItalicSimulationOn)
                {
                    XMatrix m = new XMatrix(1, 0, 0, 1, pos.X, pos.Y);
                    AppendFormatArgs("{0:" + format2 + "} {1:" + format2 + "} {2:" + format2 + "} {3:" + format2 + "} {4:" + format2 + "} {5:" + format2 + "} Tm\n{6} Tj\n",
                        m.M11, m.M12, m.M21, m.M22, m.OffsetX, m.OffsetY, text);
                    _gfxState.ItalicSimulationOn = false;
                    AdjustTdOffset(ref pos, verticalOffset, false);
                }
                else
                {
                    AdjustTdOffset(ref pos, verticalOffset, false);
                    AppendFormatArgs("{0:" + format2 + "} {1:" + format2 + "} Td {2} Tj\n", pos.X, pos.Y, text);
                }
            }
#else
                AdjustTextMatrix(ref pos);
                AppendFormat2("{0:" + format2 + "} {1:" + format2 + "} Td {2} Tj\n", pos.X, pos.Y, text);
#endif
            if (underline)
            {
                double underlinePosition = lineSpace * realizedFont.FontDescriptor._descriptor.UnderlinePosition / font.CellSpace;
                double underlineThickness = lineSpace * realizedFont.FontDescriptor._descriptor.UnderlineThickness / font.CellSpace;
                //DrawRectangle(null, brush, x, y - underlinePosition, width, underlineThickness);
                double underlineRectY = Gfx.PageDirection == XPageDirection.Downwards
                    ? y - underlinePosition
                    : y + underlinePosition - underlineThickness;
                DrawRectangle(null, brush, x, underlineRectY, width, underlineThickness);
            }

            if (strikeout)
            {
                double strikeoutPosition = lineSpace * realizedFont.FontDescriptor._descriptor.StrikeoutPosition / font.CellSpace;
                double strikeoutSize = lineSpace * realizedFont.FontDescriptor._descriptor.StrikeoutSize / font.CellSpace;
                //DrawRectangle(null, brush, x, y - strikeoutPosition - strikeoutSize, width, strikeoutSize);
                double strikeoutRectY = Gfx.PageDirection == XPageDirection.Downwards
                    ? y - strikeoutPosition
                    : y + strikeoutPosition - strikeoutSize;
                DrawRectangle(null, brush, x, strikeoutRectY, width, strikeoutSize);
            }
        }

        // ----- DrawImage ----------------------------------------------------------------------------

        //public void DrawImage(Image image, Point point);
        //public void DrawImage(Image image, PointF point);
        //public void DrawImage(Image image, Point[] destPoints);
        //public void DrawImage(Image image, PointF[] destPoints);
        //public void DrawImage(Image image, Rectangle rect);
        //public void DrawImage(Image image, RectangleF rect);
        //public void DrawImage(Image image, int x, int y);
        //public void DrawImage(Image image, float x, float y);
        //public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, Rectangle destRect, Rectangle srcRect, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, RectangleF destRect, RectangleF srcRect, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, float x, float y, RectangleF srcRect, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr);
        //public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr);
        //public void DrawImage(Image image, int x, int y, int width, int height);
        //public void DrawImage(Image image, float x, float y, float width, float height);
        //public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);
        //public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);
        //public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit);
        //public void DrawImage(Image image, Point[] destPoints, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);
        //public void DrawImage(Image image, PointF[] destPoints, RectangleF srcRect, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback, int callbackData);
        //public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr);
        //public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs);
        //public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttr, DrawImageAbort callback);
        //public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback);
        //public void DrawImage(Image image, Rectangle destRect, int srcX, int srcY, int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes imageAttrs, DrawImageAbort callback, IntPtr callbackData);
        //public void DrawImage(Image image, Rectangle destRect, float srcX, float srcY, float srcWidth, float srcHeight, GraphicsUnit srcUnit, ImageAttributes

        public void DrawImage(XImage image, double x, double y, double width, double height)
        {
            const string format = Config.SignificantFigures4;

            string name = Realize(image);
            if (!(image is XForm))
            {
                if (_gfx.PageDirection == XPageDirection.Downwards)
                {
                    AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm {4} Do Q\n",
                        x, y + height, width, height, name);
                }
                else
                {
                    AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm {4} Do Q\n",
                        x, y, width, height, name);
                }
            }
            else
            {
                BeginPage();

                XForm form = (XForm)image;
                form.Finish();

                PdfFormXObject pdfForm = Owner.FormTable.GetForm(form);

                double cx = width / image.PointWidth;
                double cy = height / image.PointHeight;

                if (cx != 0 && cy != 0)
                {
                    XPdfForm xForm = image as XPdfForm;
                    if (_gfx.PageDirection == XPageDirection.Downwards)
                    {
                        // If we have an XPdfForm, then we take the MediaBox into account.
                        double xDraw = x;
                        double yDraw = y;
                        if (xForm != null)
                        {
                            // Yes, it is an XPdfForm - adjust the position where the page will be drawn.
                            xDraw -= xForm.Page.MediaBox.X1;
                            yDraw += xForm.Page.MediaBox.Y1;
                        }
                        AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm 100 Tz {4} Do Q\n",
                            xDraw, yDraw + height, cx, cy, name);
                    }
                    else
                    {
                        // TODO Translation for MediaBox.
                        AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm {4} Do Q\n",
                            x, y, cx, cy, name);
                    }
                }
            }
        }

        // TODO: incomplete - srcRect not used
        public void DrawImage(XImage image, XRect destRect, XRect srcRect, XGraphicsUnit srcUnit)
        {
            const string format = Config.SignificantFigures4;

            double x = destRect.X;
            double y = destRect.Y;
            double width = destRect.Width;
            double height = destRect.Height;

            string name = Realize(image);
            if (!(image is XForm))
            {
                if (_gfx.PageDirection == XPageDirection.Downwards)
                {
                    AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm {4} Do\nQ\n",
                        x, y + height, width, height, name);
                }
                else
                {
                    AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm {4} Do Q\n",
                        x, y, width, height, name);
                }
            }
            else
            {
                BeginPage();

                XForm form = (XForm)image;
                form.Finish();

                PdfFormXObject pdfForm = Owner.FormTable.GetForm(form);

                double cx = width / image.PointWidth;
                double cy = height / image.PointHeight;

                if (cx != 0 && cy != 0)
                {
                    XPdfForm xForm = image as XPdfForm;
                    if (_gfx.PageDirection == XPageDirection.Downwards)
                    {
                        double xDraw = x;
                        double yDraw = y;
                        if (xForm != null)
                        {
                            // Yes, it is an XPdfForm - adjust the position where the page will be drawn.
                            xDraw -= xForm.Page.MediaBox.X1;
                            yDraw += xForm.Page.MediaBox.Y1;
                        }
                        AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm {4} Do Q\n",
                            xDraw, yDraw + height, cx, cy, name);
                    }
                    else
                    {
                        // TODO Translation for MediaBox.
                        AppendFormatImage("q {2:" + format + "} 0 0 {3:" + format + "} {0:" + format + "} {1:" + format + "} cm {4} Do Q\n",
                            x, y, cx, cy, name);
                    }
                }
            }
        }

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Save and Restore

        /// <summary>
        /// Clones the current graphics state and push it on a stack.
        /// </summary>
        public void Save(XGraphicsState state)
        {
            // Before saving, the current transformation matrix must be completely realized.
            BeginGraphicMode();
            RealizeTransform();
            // Associate the XGraphicsState with the current PdgGraphicsState.
            _gfxState.InternalState = state.InternalState;
            SaveState();
        }

        public void Restore(XGraphicsState state)
        {
            BeginGraphicMode();
            RestoreState(state.InternalState);
        }

        public void BeginContainer(XGraphicsContainer container, XRect dstrect, XRect srcrect, XGraphicsUnit unit)
        {
            // Before saving, the current transformation matrix must be completely realized.
            BeginGraphicMode();
            RealizeTransform();
            _gfxState.InternalState = container.InternalState;
            SaveState();
        }

        public void EndContainer(XGraphicsContainer container)
        {
            BeginGraphicMode();
            RestoreState(container.InternalState);
        }

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Transformation

        //public void SetPageTransform(XPageDirection direction, XPoint origion, XGraphicsUnit unit)
        //{
        //  if (_gfxStateStack.Count > 0)
        //    throw new InvalidOperationException("PageTransformation can be modified only when the graphics stack is empty.");

        //  throw new NotImplementedException("SetPageTransform");
        //}

        public XMatrix Transform
        {
            get
            {
                if (_gfxState.UnrealizedCtm.IsIdentity)
                    return _gfxState.EffectiveCtm;
                return _gfxState.UnrealizedCtm * _gfxState.RealizedCtm;
            }
        }

        public void AddTransform(XMatrix value, XMatrixOrder matrixOrder)
        {
            _gfxState.AddTransform(value, matrixOrder);
        }

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Clipping

        public void SetClip(XGraphicsPath path, XCombineMode combineMode)
        {
            if (path == null)
                throw new NotImplementedException("SetClip with no path.");

            // Ensure that the graphics state stack level is at least 2, because otherwise an error
            // occurs when someone set the clip region before something was drawn.
            if (_gfxState.Level < GraphicsStackLevelWorldSpace)
                RealizeTransform();  // TODO: refactor this function

            if (combineMode == XCombineMode.Replace)
            {
                if (_clipLevel != 0)
                {
                    if (_clipLevel != _gfxState.Level)
                        throw new NotImplementedException("Cannot set new clip region in an inner graphic state level.");
                    else
                        ResetClip();
                }
                _clipLevel = _gfxState.Level;
            }
            else if (combineMode == XCombineMode.Intersect)
            {
                if (_clipLevel == 0)
                    _clipLevel = _gfxState.Level;
            }
            else
            {
                Debug.Assert(false, "Invalid XCombineMode in internal function.");
            }
            _gfxState.SetAndRealizeClipPath(path);
        }

        /// <summary>
        /// Sets the clip path empty. Only possible if graphic state level has the same value as it has when
        /// the first time SetClip was invoked.
        /// </summary>
        public void ResetClip()
        {
            // No clip level means no clipping occurs and nothing is to do.
            if (_clipLevel == 0)
                return;

            // Only at the clipLevel the clipping can be reset.
            if (_clipLevel != _gfxState.Level)
                throw new NotImplementedException("Cannot reset clip region in an inner graphic state level.");

            // Must be in graphical mode before popping the graphics state.
            BeginGraphicMode();

            // Save InternalGraphicsState and transformation of the current graphical state.
            InternalGraphicsState state = _gfxState.InternalState;
            XMatrix ctm = _gfxState.EffectiveCtm;
            // Empty clip path by switching back to the previous state.
            RestoreState();
            SaveState();
            // Save internal state
            _gfxState.InternalState = state;
            // Restore CTM
            // TODO: check rest of clip
            //GfxState.Transform = ctm;
        }

        /// <summary>
        /// The nesting level of the PDF graphics state stack when the clip region was set to non empty.
        /// Because of the way PDF is made the clip region can only be reset at this level.
        /// </summary>
        int _clipLevel;

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Miscellaneous

        /// <summary>
        /// Writes a comment to the PDF content stream. May be useful for debugging purposes.
        /// </summary>
        public void WriteComment(string comment)
        {
            comment = comment.Replace("\n", "\n% ");
            // TODO: Some more checks necessary?
            Append("% " + comment + "\n");
        }

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Append to PDF stream

        /// <summary>
        /// Appends one or up to five Bézier curves that interpolate the arc.
        /// </summary>
        void AppendPartialArc(double x, double y, double width, double height, double startAngle, double sweepAngle, PathStart pathStart, XMatrix matrix)
        {
            // Normalize the angles
            double α = startAngle;
            if (α < 0)
                α = α + (1 + Math.Floor((Math.Abs(α) / 360))) * 360;
            else if (α > 360)
                α = α - Math.Floor(α / 360) * 360;
            Debug.Assert(α >= 0 && α <= 360);

            double β = sweepAngle;
            if (β < -360)
                β = -360;
            else if (β > 360)
                β = 360;

            if (α == 0 && β < 0)
                α = 360;
            else if (α == 360 && β > 0)
                α = 0;

            // Is it possible that the arc is small starts and ends in same quadrant?
            bool smallAngle = Math.Abs(β) <= 90;

            β = α + β;
            if (β < 0)
                β = β + (1 + Math.Floor((Math.Abs(β) / 360))) * 360;

            bool clockwise = sweepAngle > 0;
            int startQuadrant = Quadrant(α, true, clockwise);
            int endQuadrant = Quadrant(β, false, clockwise);

            if (startQuadrant == endQuadrant && smallAngle)
                AppendPartialArcQuadrant(x, y, width, height, α, β, pathStart, matrix);
            else
            {
                int currentQuadrant = startQuadrant;
                bool firstLoop = true;
                do
                {
                    if (currentQuadrant == startQuadrant && firstLoop)
                    {
                        double ξ = currentQuadrant * 90 + (clockwise ? 90 : 0);
                        AppendPartialArcQuadrant(x, y, width, height, α, ξ, pathStart, matrix);
                    }
                    else if (currentQuadrant == endQuadrant)
                    {
                        double ξ = currentQuadrant * 90 + (clockwise ? 0 : 90);
                        AppendPartialArcQuadrant(x, y, width, height, ξ, β, PathStart.Ignore1st, matrix);
                    }
                    else
                    {
                        double ξ1 = currentQuadrant * 90 + (clockwise ? 0 : 90);
                        double ξ2 = currentQuadrant * 90 + (clockwise ? 90 : 0);
                        AppendPartialArcQuadrant(x, y, width, height, ξ1, ξ2, PathStart.Ignore1st, matrix);
                    }

                    // Don't stop immediately if arc is greater than 270 degrees
                    if (currentQuadrant == endQuadrant && smallAngle)
                        break;

                    smallAngle = true;

                    if (clockwise)
                        currentQuadrant = currentQuadrant == 3 ? 0 : currentQuadrant + 1;
                    else
                        currentQuadrant = currentQuadrant == 0 ? 3 : currentQuadrant - 1;

                    firstLoop = false;
                } while (true);
            }
        }

        /// <summary>
        /// Gets the quadrant (0 through 3) of the specified angle. If the angle lies on an edge
        /// (0, 90, 180, etc.) the result depends on the details how the angle is used.
        /// </summary>
        int Quadrant(double φ, bool start, bool clockwise)
        {
            Debug.Assert(φ >= 0);
            if (φ > 360)
                φ = φ - Math.Floor(φ / 360) * 360;

            int quadrant = (int)(φ / 90);
            if (quadrant * 90 == φ)
            {
                if ((start && !clockwise) || (!start && clockwise))
                    quadrant = quadrant == 0 ? 3 : quadrant - 1;
            }
            else
                quadrant = clockwise ? ((int)Math.Floor(φ / 90)) % 4 : (int)Math.Floor(φ / 90);
            return quadrant;
        }

        /// <summary>
        /// Appends a Bézier curve for an arc within a quadrant.
        /// </summary>
        void AppendPartialArcQuadrant(double x, double y, double width, double height, double α, double β, PathStart pathStart, XMatrix matrix)
        {
            Debug.Assert(α >= 0 && α <= 360);
            Debug.Assert(β >= 0);
            if (β > 360)
                β = β - Math.Floor(β / 360) * 360;
            Debug.Assert(Math.Abs(α - β) <= 90);

            // Scanling factor
            double δx = width / 2;
            double δy = height / 2;

            // Center of ellipse
            double x0 = x + δx;
            double y0 = y + δy;

            // We have the following quarters:
            //     |
            //   2 | 3
            // ----+-----
            //   1 | 0
            //     |
            // If the angles lie in quarter 2 or 3, their values are subtracted by 180 and the
            // resulting curve is reflected at the center. This algorithm works as expected (simply tried out).
            // There may be a mathematically more elegant solution...
            bool reflect = false;
            if (α >= 180 && β >= 180)
            {
                α -= 180;
                β -= 180;
                reflect = true;
            }

            double sinα, sinβ;
            if (width == height)
            {
                // Circular arc needs no correction.
                α = α * Calc.Deg2Rad;
                β = β * Calc.Deg2Rad;
            }
            else
            {
                // Elliptic arc needs the angles to be adjusted such that the scaling transformation is compensated.
                α = α * Calc.Deg2Rad;
                sinα = Math.Sin(α);
                if (Math.Abs(sinα) > 1E-10)
                    α = Math.PI / 2 - Math.Atan(δy * Math.Cos(α) / (δx * sinα));
                β = β * Calc.Deg2Rad;
                sinβ = Math.Sin(β);
                if (Math.Abs(sinβ) > 1E-10)
                    β = Math.PI / 2 - Math.Atan(δy * Math.Cos(β) / (δx * sinβ));
            }

            double κ = 4 * (1 - Math.Cos((α - β) / 2)) / (3 * Math.Sin((β - α) / 2));
            sinα = Math.Sin(α);
            double cosα = Math.Cos(α);
            sinβ = Math.Sin(β);
            double cosβ = Math.Cos(β);

            const string format = Config.SignificantFigures3;
            XPoint pt1, pt2, pt3;
            if (!reflect)
            {
                // Calculation for quarter 0 and 1
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        pt1 = matrix.Transform(new XPoint(x0 + δx * cosα, y0 + δy * sinα));
                        AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", pt1.X, pt1.Y);
                        break;

                    case PathStart.LineTo1st:
                        pt1 = matrix.Transform(new XPoint(x0 + δx * cosα, y0 + δy * sinα));
                        AppendFormatPoint("{0:" + format + "} {1:" + format + "} l\n", pt1.X, pt1.Y);
                        break;

                    case PathStart.Ignore1st:
                        break;
                }
                pt1 = matrix.Transform(new XPoint(x0 + δx * (cosα - κ * sinα), y0 + δy * (sinα + κ * cosα)));
                pt2 = matrix.Transform(new XPoint(x0 + δx * (cosβ + κ * sinβ), y0 + δy * (sinβ - κ * cosβ)));
                pt3 = matrix.Transform(new XPoint(x0 + δx * cosβ, y0 + δy * sinβ));
                AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
                  pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y);
            }
            else
            {
                // Calculation for quarter 2 and 3.
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        pt1 = matrix.Transform(new XPoint(x0 - δx * cosα, y0 - δy * sinα));
                        AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", pt1.X, pt1.Y);
                        break;

                    case PathStart.LineTo1st:
                        pt1 = matrix.Transform(new XPoint(x0 - δx * cosα, y0 - δy * sinα));
                        AppendFormatPoint("{0:" + format + "} {1:" + format + "} l\n", pt1.X, pt1.Y);
                        break;

                    case PathStart.Ignore1st:
                        break;
                }
                pt1 = matrix.Transform(new XPoint(x0 - δx * (cosα - κ * sinα), y0 - δy * (sinα + κ * cosα)));
                pt2 = matrix.Transform(new XPoint(x0 - δx * (cosβ + κ * sinβ), y0 - δy * (sinβ - κ * cosβ)));
                pt3 = matrix.Transform(new XPoint(x0 - δx * cosβ, y0 - δy * sinβ));
                AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
                    pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y);
            }
        }

#if WPF || NETFX_CORE
        void AppendPartialArc(SysPoint point1, SysPoint point2, double rotationAngle,
            SysSize size, bool isLargeArc, SweepDirection sweepDirection, PathStart pathStart)
        {
            const string format = Config.SignificantFigures4;

            Debug.Assert(pathStart == PathStart.Ignore1st);

            int pieces;
            PointCollection points = GeometryHelper.ArcToBezier(point1.X, point1.Y, size.Width, size.Height, rotationAngle, isLargeArc,
              sweepDirection == SweepDirection.Clockwise, point2.X, point2.Y, out pieces);

            int count = points.Count;
            int start = count % 3 == 1 ? 1 : 0;
            if (start == 1)
                AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", points[0].X, points[0].Y);
            for (int idx = start; idx < count; idx += 3)
                AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
                  points[idx].X, points[idx].Y,
                  points[idx + 1].X, points[idx + 1].Y,
                  points[idx + 2].X, points[idx + 2].Y);
        }
#endif

        /// <summary>
        /// Appends a Bézier curve for a cardinal spline through pt1 and pt2.
        /// </summary>
        void AppendCurveSegment(XPoint pt0, XPoint pt1, XPoint pt2, XPoint pt3, double tension3)
        {
            const string format = Config.SignificantFigures4;
            AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
                pt1.X + tension3 * (pt2.X - pt0.X), pt1.Y + tension3 * (pt2.Y - pt0.Y),
                pt2.X - tension3 * (pt3.X - pt1.X), pt2.Y - tension3 * (pt3.Y - pt1.Y),
                pt2.X, pt2.Y);
        }

#if CORE_
        /// <summary>
        /// Appends the content of a GraphicsPath object.
        /// </summary>
        internal void AppendPath(GraphicsPath path)
        {
            int count = path.PointCount;
            if (count == 0)
                return;
            PointF[] points = path.PathPoints;
            Byte[] types = path.PathTypes;

            for (int idx = 0; idx < count; idx++)
            {
                // From GDI+ documentation:
                const byte PathPointTypeStart = 0; // move
                const byte PathPointTypeLine = 1; // line
                const byte PathPointTypeBezier = 3; // default Bezier (= cubic Bezier)
                const byte PathPointTypePathTypeMask = 0x07; // type mask (lowest 3 bits).
                //const byte PathPointTypeDashMode = 0x10; // currently in dash mode.
                //const byte PathPointTypePathMarker = 0x20; // a marker for the path.
                const byte PathPointTypeCloseSubpath = 0x80; // closed flag

                byte type = types[idx];
                switch (type & PathPointTypePathTypeMask)
                {
                    case PathPointTypeStart:
                        //PDF_moveto(pdf, points[idx].X, points[idx].Y);
                        AppendFormat("{0:" + format + "} {1:" + format + "} m\n", points[idx].X, points[idx].Y);
                        break;

                    case PathPointTypeLine:
                        //PDF_lineto(pdf, points[idx].X, points[idx].Y);
                        AppendFormat("{0:" + format + "} {1:" + format + "} l\n", points[idx].X, points[idx].Y);
                        if ((type & PathPointTypeCloseSubpath) != 0)
                            Append("h\n");
                        break;

                    case PathPointTypeBezier:
                        Debug.Assert(idx + 2 < count);
                        //PDF_curveto(pdf, points[idx].X, points[idx].Y, 
                        //                 points[idx + 1].X, points[idx + 1].Y, 
                        //                 points[idx + 2].X, points[idx + 2].Y);
                        AppendFormat("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n", points[idx].X, points[idx].Y,
                            points[++idx].X, points[idx].Y, points[++idx].X, points[idx].Y);
                        if ((types[idx] & PathPointTypeCloseSubpath) != 0)
                            Append("h\n");
                        break;
                }
            }
        }
#endif

#if CORE
        /// <summary>
        /// Appends the content of a GraphicsPath object.
        /// </summary>
        internal void AppendPath(CoreGraphicsPath path)
        {
            AppendPath(path.PathPoints, path.PathTypes);
            //XPoint[] points = path.PathPoints;
            //Byte[] types = path.PathTypes;

            //int count = points.Length;
            //if (count == 0)
            //    return;

            //for (int idx = 0; idx < count; idx++)
            //{
            //    // From GDI+ documentation:
            //    const byte PathPointTypeStart = 0; // move
            //    const byte PathPointTypeLine = 1; // line
            //    const byte PathPointTypeBezier = 3; // default Bezier (= cubic Bezier)
            //    const byte PathPointTypePathTypeMask = 0x07; // type mask (lowest 3 bits).
            //    //const byte PathPointTypeDashMode = 0x10; // currently in dash mode.
            //    //const byte PathPointTypePathMarker = 0x20; // a marker for the path.
            //    const byte PathPointTypeCloseSubpath = 0x80; // closed flag

            //    byte type = types[idx];
            //    switch (type & PathPointTypePathTypeMask)
            //    {
            //        case PathPointTypeStart:
            //            //PDF_moveto(pdf, points[idx].X, points[idx].Y);
            //            AppendFormat("{0:" + format + "} {1:" + format + "} m\n", points[idx].X, points[idx].Y);
            //            break;

            //        case PathPointTypeLine:
            //            //PDF_lineto(pdf, points[idx].X, points[idx].Y);
            //            AppendFormat("{0:" + format + "} {1:" + format + "} l\n", points[idx].X, points[idx].Y);
            //            if ((type & PathPointTypeCloseSubpath) != 0)
            //                Append("h\n");
            //            break;

            //        case PathPointTypeBezier:
            //            Debug.Assert(idx + 2 < count);
            //            //PDF_curveto(pdf, points[idx].X, points[idx].Y, 
            //            //                 points[idx + 1].X, points[idx + 1].Y, 
            //            //                 points[idx + 2].X, points[idx + 2].Y);
            //            AppendFormat("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n", points[idx].X, points[idx].Y,
            //                points[++idx].X, points[idx].Y, points[++idx].X, points[idx].Y);
            //            if ((types[idx] & PathPointTypeCloseSubpath) != 0)
            //                Append("h\n");
            //            break;
            //    }
            //}
        }
#endif

#if GDI
        /// <summary>
        /// Appends the content of a GraphicsPath object.
        /// </summary>
        internal void AppendPath(GraphicsPath path)
        {
#if true
            AppendPath(XGraphics.MakeXPointArray(path.PathPoints, 0, path.PathPoints.Length), path.PathTypes);
#else
            int count = path.PointCount;
            if (count == 0)
                return;
            PointF[] points = path.PathPoints;
            Byte[] types = path.PathTypes;

            for (int idx = 0; idx < count; idx++)
            {
                // From GDI+ documentation:
                const byte PathPointTypeStart = 0; // move
                const byte PathPointTypeLine = 1; // line
                const byte PathPointTypeBezier = 3; // default Bezier (= cubic Bezier)
                const byte PathPointTypePathTypeMask = 0x07; // type mask (lowest 3 bits).
                //const byte PathPointTypeDashMode = 0x10; // currently in dash mode.
                //const byte PathPointTypePathMarker = 0x20; // a marker for the path.
                const byte PathPointTypeCloseSubpath = 0x80; // closed flag

                byte type = types[idx];
                switch (type & PathPointTypePathTypeMask)
                {
                    case PathPointTypeStart:
                        //PDF_moveto(pdf, points[idx].X, points[idx].Y);
                        AppendFormat("{0:" + format + "} {1:" + format + "} m\n", points[idx].X, points[idx].Y);
                        break;

                    case PathPointTypeLine:
                        //PDF_lineto(pdf, points[idx].X, points[idx].Y);
                        AppendFormat("{0:" + format + "} {1:" + format + "} l\n", points[idx].X, points[idx].Y);
                        if ((type & PathPointTypeCloseSubpath) != 0)
                            Append("h\n");
                        break;

                    case PathPointTypeBezier:
                        Debug.Assert(idx + 2 < count);
                        //PDF_curveto(pdf, points[idx].X, points[idx].Y, 
                        //                 points[idx + 1].X, points[idx + 1].Y, 
                        //                 points[idx + 2].X, points[idx + 2].Y);
                        AppendFormat("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n", points[idx].X, points[idx].Y,
                            points[++idx].X, points[idx].Y, points[++idx].X, points[idx].Y);
                        if ((types[idx] & PathPointTypeCloseSubpath) != 0)
                            Append("h\n");
                        break;
                }
            }
#endif
        }
#endif

#if CORE || GDI
        void AppendPath(XPoint[] points, Byte[] types)
        {
            const string format = Config.SignificantFigures4;
            int count = points.Length;
            if (count == 0)
                return;

            for (int idx = 0; idx < count; idx++)
            {
                // ReSharper disable InconsistentNaming
                // From GDI+ documentation:
                const byte PathPointTypeStart = 0; // move
                const byte PathPointTypeLine = 1; // line
                const byte PathPointTypeBezier = 3; // default Bezier (= cubic Bezier)
                const byte PathPointTypePathTypeMask = 0x07; // type mask (lowest 3 bits).
                //const byte PathPointTypeDashMode = 0x10; // currently in dash mode.
                //const byte PathPointTypePathMarker = 0x20; // a marker for the path.
                const byte PathPointTypeCloseSubpath = 0x80; // closed flag
                // ReSharper restore InconsistentNaming

                byte type = types[idx];
                switch (type & PathPointTypePathTypeMask)
                {
                    case PathPointTypeStart:
                        //PDF_moveto(pdf, points[idx].X, points[idx].Y);
                        AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", points[idx].X, points[idx].Y);
                        break;

                    case PathPointTypeLine:
                        //PDF_lineto(pdf, points[idx].X, points[idx].Y);
                        AppendFormatPoint("{0:" + format + "} {1:" + format + "} l\n", points[idx].X, points[idx].Y);
                        if ((type & PathPointTypeCloseSubpath) != 0)
                            Append("h\n");
                        break;

                    case PathPointTypeBezier:
                        Debug.Assert(idx + 2 < count);
                        //PDF_curveto(pdf, points[idx].X, points[idx].Y, 
                        //                 points[idx + 1].X, points[idx + 1].Y, 
                        //                 points[idx + 2].X, points[idx + 2].Y);
                        AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n", points[idx].X, points[idx].Y,
                            points[++idx].X, points[idx].Y, points[++idx].X, points[idx].Y);
                        if ((types[idx] & PathPointTypeCloseSubpath) != 0)
                            Append("h\n");
                        break;
                }
            }
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Appends the content of a PathGeometry object.
        /// </summary>
        internal void AppendPath(PathGeometry geometry)
        {
            const string format = Config.SignificantFigures4;

            foreach (PathFigure figure in geometry.Figures)
            {
#if DEBUG
                //#warning For DdlGBE_Chart_Layout (WPF) execution stucks at this Assertion.
                // The empty Figure is added via XGraphicsPath.CurrentPathFigure Getter.
                // Some methods like XGraphicsPath.AddRectangle() or AddLine() use this emtpy Figure to add Segments, others like AddEllipse() don't.
                // Here, _pathGeometry.AddGeometry() of course ignores this first Figure and adds a second.
                // Encapsulate relevant Add methods to delete a first emty Figure or move the Addition of an first empty Figure to a GetOrCreateCurrentPathFigure() or simply remove Assertion?
                // Look for:
                // MAOS4STLA: CurrentPathFigure.


                if (figure.Segments.Count == 0)
                    42.GetType();
                Debug.Assert(figure.Segments.Count > 0);
#endif
                // Skip the Move if the segment is empty. Workaround for empty segments. Empty segments should not occur (see Debug.Assert above).
                if (figure.Segments.Count > 0)
                {
                    // Move to start point.
                    SysPoint currentPoint = figure.StartPoint;
                    AppendFormatPoint("{0:" + format + "} {1:" + format + "} m\n", currentPoint.X, currentPoint.Y);

                    foreach (PathSegment segment in figure.Segments)
                    {
                        Type type = segment.GetType();
                        if (type == typeof(LineSegment))
                        {
                            // Draw a single line.
                            SysPoint point = ((LineSegment)segment).Point;
                            currentPoint = point;
                            AppendFormatPoint("{0:" + format + "} {1:" + format + "} l\n", point.X, point.Y);
                        }
                        else if (type == typeof(PolyLineSegment))
                        {
                            // Draw connected lines.
                            PointCollection points = ((PolyLineSegment)segment).Points;
                            foreach (SysPoint point in points)
                            {
                                currentPoint = point;  // I forced myself not to optimize this assignment.
                                AppendFormatPoint("{0:" + format + "} {1:" + format + "} l\n", point.X, point.Y);
                            }
                        }
                        else if (type == typeof(BezierSegment))
                        {
                            // Draw Bézier curve.
                            BezierSegment seg = (BezierSegment)segment;
                            SysPoint point1 = seg.Point1;
                            SysPoint point2 = seg.Point2;
                            SysPoint point3 = seg.Point3;
                            AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
                                point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y);
                            currentPoint = point3;
                        }
                        else if (type == typeof(PolyBezierSegment))
                        {
                            // Draw connected Bézier curves.
                            PointCollection points = ((PolyBezierSegment)segment).Points;
                            int count = points.Count;
                            if (count > 0)
                            {
                                Debug.Assert(count % 3 == 0, "Number of Points in PolyBezierSegment are not a multiple of 3.");
                                for (int idx = 0; idx < count - 2; idx += 3)
                                {
                                    SysPoint point1 = points[idx];
                                    SysPoint point2 = points[idx + 1];
                                    SysPoint point3 = points[idx + 2];
                                    AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} c\n",
                                        point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y);
                                }
                                currentPoint = points[count - 1];
                            }
                        }
                        else if (type == typeof(ArcSegment))
                        {
                            // Draw arc.
                            ArcSegment seg = (ArcSegment)segment;
                            AppendPartialArc(currentPoint, seg.Point, seg.RotationAngle, seg.Size, seg.IsLargeArc, seg.SweepDirection, PathStart.Ignore1st);
                            currentPoint = seg.Point;
                        }
                        else if (type == typeof(QuadraticBezierSegment))
                        {
                            QuadraticBezierSegment seg = (QuadraticBezierSegment)segment;
                            currentPoint = seg.Point2;
                            // TODOWPF: Undone because XGraphics has no such curve type
                            throw new NotImplementedException("AppendPath with QuadraticBezierSegment.");
                        }
                        else if (type == typeof(PolyQuadraticBezierSegment))
                        {
                            PolyQuadraticBezierSegment seg = (PolyQuadraticBezierSegment)segment;
                            currentPoint = seg.Points[seg.Points.Count - 1];
                            // TODOWPF: Undone because XGraphics has no such curve type
                            throw new NotImplementedException("AppendPath with PolyQuadraticBezierSegment.");
                        }
                    }
                    if (figure.IsClosed)
                        Append("h\n");
                }
            }
        }
#endif

        internal void Append(string value)
        {
            _content.Append(value);
        }

        internal void AppendFormatArgs(string format, params object[] args)
        {
            _content.AppendFormat(CultureInfo.InvariantCulture, format, args);
#if DEBUG
            string dummy = _content.ToString();
            dummy = dummy.Substring(Math.Max(0, dummy.Length - 100));
            dummy.GetType();
#endif
        }

        internal void AppendFormatString(string format, string s)
        {
            _content.AppendFormat(CultureInfo.InvariantCulture, format, s);
        }

        internal void AppendFormatFont(string format, string s, double d)
        {
            _content.AppendFormat(CultureInfo.InvariantCulture, format, s, d);
        }

        internal void AppendFormatInt(string format, int n)
        {
            _content.AppendFormat(CultureInfo.InvariantCulture, format, n);
        }

        internal void AppendFormatDouble(string format, double d)
        {
            _content.AppendFormat(CultureInfo.InvariantCulture, format, d);
        }

        internal void AppendFormatPoint(string format, double x, double y)
        {
            XPoint result = WorldToView(new XPoint(x, y));
            _content.AppendFormat(CultureInfo.InvariantCulture, format, result.X, result.Y);
        }

        internal void AppendFormatRect(string format, double x, double y, double width, double height)
        {
            XPoint point1 = WorldToView(new XPoint(x, y));
            _content.AppendFormat(CultureInfo.InvariantCulture, format, point1.X, point1.Y, width, height);
        }

        internal void AppendFormat3Points(string format, double x1, double y1, double x2, double y2, double x3, double y3)
        {
            XPoint point1 = WorldToView(new XPoint(x1, y1));
            XPoint point2 = WorldToView(new XPoint(x2, y2));
            XPoint point3 = WorldToView(new XPoint(x3, y3));
            _content.AppendFormat(CultureInfo.InvariantCulture, format, point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y);
        }

        internal void AppendFormat(string format, XPoint point)
        {
            XPoint result = WorldToView(point);
            _content.AppendFormat(CultureInfo.InvariantCulture, format, result.X, result.Y);
        }

        internal void AppendFormat(string format, double x, double y, string s)
        {
            XPoint result = WorldToView(new XPoint(x, y));
            _content.AppendFormat(CultureInfo.InvariantCulture, format, result.X, result.Y, s);
        }

        internal void AppendFormatImage(string format, double x, double y, double width, double height, string name)
        {
            XPoint result = WorldToView(new XPoint(x, y));
            _content.AppendFormat(CultureInfo.InvariantCulture, format, result.X, result.Y, width, height, name);
        }

        void AppendStrokeFill(XPen pen, XBrush brush, XFillMode fillMode, bool closePath)
        {
            if (closePath)
                _content.Append("h ");

            if (fillMode == XFillMode.Winding)
            {
                if (pen != null && brush != null)
                    _content.Append("B\n");
                else if (pen != null)
                    _content.Append("S\n");
                else
                    _content.Append("f\n");
            }
            else
            {
                if (pen != null && brush != null)
                    _content.Append("B*\n");
                else if (pen != null)
                    _content.Append("S\n");
                else
                    _content.Append("f*\n");
            }
        }
        #endregion

        // --------------------------------------------------------------------------------------------

        #region Realizing graphical state

        /// <summary>
        /// Initializes the default view transformation, i.e. the transformation from the user page
        /// space to the PDF page space.
        /// </summary>
        void BeginPage()
        {
            if (_gfxState.Level == GraphicsStackLevelInitial)
            {
                // TODO: Is PageOriging and PageScale (== Viewport) useful? Or just public DefaultViewMatrix (like Presentation Manager has had)
                // May be a BeginContainer(windows, viewport) is useful for userer that are not familar with maxtrix transformations.

                // Flip page horizontally and mirror text.

                // PDF uses a standard right-handed Cartesian coordinate system with the y axis directed up
                // and the rotation counterclockwise. Windows uses the opposite convertion with y axis
                // directed down and rotation clockwise. When I started with PDFsharp I flipped pages horizontally
                // and then mirrored text to compensate the effect that the fipping turns text upside down.
                // I found this technique during analysis of PDF documents generated with PDFlib. Unfortunately
                // this technique leads to several problems with programms that compose or view PDF documents
                // generated with PDFsharp.
                // In PDFsharp 1.4 I implement a revised technique that does not need text mirroring any more.

                DefaultViewMatrix = new XMatrix();
                if (_gfx.PageDirection == XPageDirection.Downwards)
                {
                    // Take TrimBox into account.
                    PageHeightPt = Size.Height;
                    XPoint trimOffset = new XPoint();
                    if (_page != null && _page.TrimMargins.AreSet)
                    {
                        PageHeightPt += _page.TrimMargins.Top.Point + _page.TrimMargins.Bottom.Point;
                        trimOffset = new XPoint(_page.TrimMargins.Left.Point, _page.TrimMargins.Top.Point);
                    }

                    // Scale with page units.
                    switch (_gfx.PageUnit)
                    {
                        case XGraphicsUnit.Point:
                            // Factor is 1.
                            // DefaultViewMatrix.ScalePrepend(XUnit.PointFactor);
                            break;

                        case XGraphicsUnit.Presentation:
                            DefaultViewMatrix.ScalePrepend(XUnit.PresentationFactor);
                            break;

                        case XGraphicsUnit.Inch:
                            DefaultViewMatrix.ScalePrepend(XUnit.InchFactor);
                            break;

                        case XGraphicsUnit.Millimeter:
                            DefaultViewMatrix.ScalePrepend(XUnit.MillimeterFactor);
                            break;

                        case XGraphicsUnit.Centimeter:
                            DefaultViewMatrix.ScalePrepend(XUnit.CentimeterFactor);
                            break;
                    }

                    if (trimOffset != new XPoint())
                    {
                        Debug.Assert(_gfx.PageUnit == XGraphicsUnit.Point, "With TrimMargins set the page units must be Point. Ohter cases nyi.");
                        DefaultViewMatrix.TranslatePrepend(trimOffset.X, -trimOffset.Y);
                    }

                    // Save initial graphic state.
                    SaveState();

                    // Set default page transformation, if any.
                    if (!DefaultViewMatrix.IsIdentity)
                    {
                        Debug.Assert(_gfxState.RealizedCtm.IsIdentity);
                        //_gfxState.RealizedCtm = DefaultViewMatrix;
                        const string format = Config.SignificantFigures7;
                        double[] cm = DefaultViewMatrix.GetElements();
                        AppendFormatArgs("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} cm ",
                                     cm[0], cm[1], cm[2], cm[3], cm[4], cm[5]);
                    }

                    // Set page transformation
                    //double[] cm = DefaultViewMatrix.GetElements();
                    //AppendFormat("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} cm ",
                    //  cm[0], cm[1], cm[2], cm[3], cm[4], cm[5]);
                }
                else
                {
                    // Scale with page units.
                    switch (_gfx.PageUnit)
                    {
                        case XGraphicsUnit.Point:
                            // Factor is 1.
                            // DefaultViewMatrix.ScalePrepend(XUnit.PointFactor);
                            break;

                        case XGraphicsUnit.Presentation:
                            DefaultViewMatrix.ScalePrepend(XUnit.PresentationFactor);
                            break;

                        case XGraphicsUnit.Inch:
                            DefaultViewMatrix.ScalePrepend(XUnit.InchFactor);
                            break;

                        case XGraphicsUnit.Millimeter:
                            DefaultViewMatrix.ScalePrepend(XUnit.MillimeterFactor);
                            break;

                        case XGraphicsUnit.Centimeter:
                            DefaultViewMatrix.ScalePrepend(XUnit.CentimeterFactor);
                            break;
                    }

                    // Save initial graphic state.
                    SaveState();
                    // Set page transformation.
                    const string format = Config.SignificantFigures7;
                    double[] cm = DefaultViewMatrix.GetElements();
                    AppendFormat3Points("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} cm ",
                        cm[0], cm[1], cm[2], cm[3], cm[4], cm[5]);
                }
            }
        }

        /// <summary>
        /// Ends the content stream, i.e. ends the text mode and balances the graphic state stack.
        /// </summary>
        void EndPage()
        {
            if (_streamMode == StreamMode.Text)
            {
                _content.Append("ET\n");
                _streamMode = StreamMode.Graphic;
            }

            while (_gfxStateStack.Count != 0)
                RestoreState();
        }

        /// <summary>
        /// Begins the graphic mode (i.e. ends the text mode).
        /// </summary>
        internal void BeginGraphicMode()
        {
            if (_streamMode != StreamMode.Graphic)
            {
                if (_streamMode == StreamMode.Text)
                    _content.Append("ET\n");

                _streamMode = StreamMode.Graphic;
            }
        }

        /// <summary>
        /// Begins the graphic mode (i.e. ends the text mode).
        /// </summary>
        internal void BeginTextMode()
        {
            if (_streamMode != StreamMode.Text)
            {
                _streamMode = StreamMode.Text;
                _content.Append("BT\n");
                // Text matrix is empty after BT
                _gfxState.RealizedTextPosition = new XPoint();
                _gfxState.ItalicSimulationOn = false;
            }
        }

        StreamMode _streamMode;

        /// <summary>
        /// Makes the specified pen and brush to the current graphics objects.
        /// </summary>
        private void Realize(XPen pen, XBrush brush)
        {
            BeginPage();
            BeginGraphicMode();
            RealizeTransform();

            if (pen != null)
                _gfxState.RealizePen(pen, _colorMode); // page.document.Options.ColorMode);

            if (brush != null)
            {
                // Render mode is 0 except for bold simulation.
                _gfxState.RealizeBrush(brush, _colorMode, 0, 0); // page.document.Options.ColorMode);
            }
        }

        /// <summary>
        /// Makes the specified pen to the current graphics object.
        /// </summary>
        void Realize(XPen pen)
        {
            Realize(pen, null);
        }

        /// <summary>
        /// Makes the specified brush to the current graphics object.
        /// </summary>
        void Realize(XBrush brush)
        {
            Realize(null, brush);
        }

        /// <summary>
        /// Makes the specified font and brush to the current graphics objects.
        /// </summary>
        void Realize(XFont font, XBrush brush, int renderingMode)
        {
            BeginPage();
            RealizeTransform();
            BeginTextMode();
            _gfxState.RealizeFont(font, brush, renderingMode);
        }

        /// <summary>
        /// PDFsharp uses the Td operator to set the text position. Td just sets the offset of the text matrix
        /// and produces lesser code as Tm.
        /// </summary>
        /// <param name="pos">The absolute text position.</param>
        /// <param name="dy">The dy.</param>
        /// <param name="adjustSkew">true if skewing for italic simulation is currently on.</param>
        void AdjustTdOffset(ref XPoint pos, double dy, bool adjustSkew)
        {
            pos.Y += dy;
            // Reference: TABLE 5.5  Text-positioning operators / Page 406
            XPoint posSave = pos;
            // Map from absolute to relative position.
            pos = pos - new XVector(_gfxState.RealizedTextPosition.X, _gfxState.RealizedTextPosition.Y);
            if (adjustSkew)
            {
                // In case that italic simulation is on X must be adjusted according to Y offset. Weird but works :-)
                pos.X -= Const.ItalicSkewAngleSinus * pos.Y;
            }
            _gfxState.RealizedTextPosition = posSave;
        }

        /// <summary>
        /// Makes the specified image to the current graphics object.
        /// </summary>
        string Realize(XImage image)
        {
            BeginPage();
            BeginGraphicMode();
            RealizeTransform();

            // The transparency set for a brush also applies to images. Set opacity to 100% so image will be drawn without transparency.
            _gfxState.RealizeNonStrokeTransparency(1, _colorMode);

            XForm form = image as XForm;
            return form != null ? GetFormName(form) : GetImageName(image);
        }

        /// <summary>
        /// Realizes the current transformation matrix, if necessary.
        /// </summary>
        void RealizeTransform()
        {
            BeginPage();

            if (_gfxState.Level == GraphicsStackLevelPageSpace)
            {
                BeginGraphicMode();
                SaveState();
            }

            //if (gfxState.MustRealizeCtm)
            if (!_gfxState.UnrealizedCtm.IsIdentity)
            {
                BeginGraphicMode();
                _gfxState.RealizeCtm();
            }
        }

        /// <summary>
        /// Convert a point from Windows world space to PDF world space.
        /// </summary>
        internal XPoint WorldToView(XPoint point)
        {
            // If EffectiveCtm is not yet realized InverseEffectiveCtm is invalid.
            Debug.Assert(_gfxState.UnrealizedCtm.IsIdentity, "Somewhere a RealizeTransform is missing.");
#if true
            // See in #else case why this is correct.
            XPoint pt = _gfxState.WorldTransform.Transform(point);
            return _gfxState.InverseEffectiveCtm.Transform(new XPoint(pt.X, PageHeightPt / DefaultViewMatrix.M22 - pt.Y));
#else
            // Get inverted PDF world transform matrix.
            XMatrix invers = _gfxState.EffectiveCtm;
            invers.Invert();

            // Apply transform in Windows world space.
            XPoint pt1 = _gfxState.WorldTransform.Transform(point);
#if true
            // Do the transformation (see #else case) in one step.
            XPoint pt2 = new XPoint(pt1.X, PageHeightPt / DefaultViewMatrix.M22 - pt1.Y);
#else
            // Replicable version

            // Apply default transformation.
            pt1.X = pt1.X * DefaultViewMatrix.M11;
            pt1.Y = pt1.Y * DefaultViewMatrix.M22;

            // Convert from Windows space to PDF space.
            XPoint pt2 = new XPoint(pt1.X, PageHeightPt - pt1.Y);

            pt2.X = pt2.X / DefaultViewMatrix.M11;
            pt2.Y = pt2.Y / DefaultViewMatrix.M22;
#endif
            XPoint pt3 = invers.Transform(pt2);
            return pt3;
#endif
        }
        #endregion

#if GDI
        [Conditional("DEBUG")]
        void DumpPathData(PathData pathData)
        {
            XPoint[] points = new XPoint[pathData.Points.Length];
            for (int i = 0; i < points.Length; i++)
                points[i] = new XPoint(pathData.Points[i].X, pathData.Points[i].Y);

            DumpPathData(points, pathData.Types);
        }
#endif
#if CORE || GDI
        [Conditional("DEBUG")]
        void DumpPathData(XPoint[] points, byte[] types)
        {
            int count = points.Length;
            for (int idx = 0; idx < count; idx++)
            {
                string info = PdfEncoders.Format("{0:X}   {1:####0.000} {2:####0.000}", types[idx], points[idx].X, points[idx].Y);
                Debug.WriteLine(info, "PathData");
            }
        }
#endif

        /// <summary>
        /// Gets the owning PdfDocument of this page or form.
        /// </summary>
        internal PdfDocument Owner
        {
            get
            {
                if (_page != null)
                    return _page.Owner;
                return _form.Owner;
            }
        }

        internal XGraphics Gfx
        {
            get { return _gfx; }
        }

        /// <summary>
        /// Gets the PdfResources of this page or form.
        /// </summary>
        internal PdfResources Resources
        {
            get
            {
                if (_page != null)
                    return _page.Resources;
                return _form.Resources;
            }
        }

        /// <summary>
        /// Gets the size of this page or form.
        /// </summary>
        internal XSize Size
        {
            get
            {
                if (_page != null)
                    return new XSize(_page.Width, _page.Height);
                return _form.Size;
            }
        }

        /// <summary>
        /// Gets the resource name of the specified font within this page or form.
        /// </summary>
        internal string GetFontName(XFont font, out PdfFont pdfFont)
        {
            if (_page != null)
                return _page.GetFontName(font, out pdfFont);
            return _form.GetFontName(font, out pdfFont);
        }

        /// <summary>
        /// Gets the resource name of the specified image within this page or form.
        /// </summary>
        internal string GetImageName(XImage image)
        {
            if (_page != null)
                return _page.GetImageName(image);
            return _form.GetImageName(image);
        }

        /// <summary>
        /// Gets the resource name of the specified form within this page or form.
        /// </summary>
        internal string GetFormName(XForm form)
        {
            if (_page != null)
                return _page.GetFormName(form);
            return _form.GetFormName(form);
        }

        internal PdfPage _page;
        internal XForm _form;
        internal PdfColorMode _colorMode;
        XGraphicsPdfPageOptions _options;
        XGraphics _gfx;
        readonly StringBuilder _content;

        /// <summary>
        /// The q/Q nesting level is 0.
        /// </summary>
        const int GraphicsStackLevelInitial = 0;

        /// <summary>
        /// The q/Q nesting level is 1.
        /// </summary>
        const int GraphicsStackLevelPageSpace = 1;

        /// <summary>
        /// The q/Q nesting level is 2.
        /// </summary>
        const int GraphicsStackLevelWorldSpace = 2;

        #region PDF Graphics State

        /// <summary>
        /// Saves the current graphical state.
        /// </summary>
        void SaveState()
        {
            Debug.Assert(_streamMode == StreamMode.Graphic, "Cannot save state in text mode.");

            _gfxStateStack.Push(_gfxState);
            _gfxState = _gfxState.Clone();
            _gfxState.Level = _gfxStateStack.Count;
            Append("q\n");
        }

        /// <summary>
        /// Restores the previous graphical state.
        /// </summary>
        void RestoreState()
        {
            Debug.Assert(_streamMode == StreamMode.Graphic, "Cannot restore state in text mode.");

            _gfxState = _gfxStateStack.Pop();
            Append("Q\n");
        }

        PdfGraphicsState RestoreState(InternalGraphicsState state)
        {
            int count = 1;
            PdfGraphicsState top = _gfxStateStack.Pop();
            while (top.InternalState != state)
            {
                Append("Q\n");
                count++;
                top = _gfxStateStack.Pop();
            }
            Append("Q\n");
            _gfxState = top;
            return top;
        }

        /// <summary>
        /// The current graphical state.
        /// </summary>
        PdfGraphicsState _gfxState;

        /// <summary>
        /// The graphical state stack.
        /// </summary>
        readonly Stack<PdfGraphicsState> _gfxStateStack = new Stack<PdfGraphicsState>();

        #endregion

        /// <summary>
        /// The height of the PDF page in point including the trim box.
        /// </summary>
        public double PageHeightPt;

        /// <summary>
        /// The final transformation from the world space to the default page space.
        /// </summary>
        public XMatrix DefaultViewMatrix;
    }
}