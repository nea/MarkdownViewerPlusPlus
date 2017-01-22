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
using System.Diagnostics;
using System.Globalization;
using System.Text;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
#if WPF
#endif
using PdfSharp.Internal;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Internal;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace PdfSharp.Drawing.Pdf
{
    /// <summary>
    /// Represents the current PDF graphics state.
    /// </summary>
    /// <remarks>
    /// Completely revised for PDFsharp 1.4.
    /// </remarks>
    internal sealed class PdfGraphicsState : ICloneable
    {
        public PdfGraphicsState(XGraphicsPdfRenderer renderer)
        {
            _renderer = renderer;
        }
        readonly XGraphicsPdfRenderer _renderer;

        public PdfGraphicsState Clone()
        {
            PdfGraphicsState state = (PdfGraphicsState)MemberwiseClone();
            return state;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        internal int Level;

        internal InternalGraphicsState InternalState;

        public void PushState()
        {
            // BeginGraphic
            _renderer.Append("q/n");
        }

        public void PopState()
        {
            //BeginGraphic
            _renderer.Append("Q/n");
        }

        #region Stroke

        double _realizedLineWith = -1;
        int _realizedLineCap = -1;
        int _realizedLineJoin = -1;
        double _realizedMiterLimit = -1;
        XDashStyle _realizedDashStyle = (XDashStyle)(-1);
        string _realizedDashPattern;
        XColor _realizedStrokeColor = XColor.Empty;
        bool _realizedStrokeOverPrint;

        public void RealizePen(XPen pen, PdfColorMode colorMode)
        {
            const string frmt2 = Config.SignificantFigures2;
            const string format = Config.SignificantFigures3;
            XColor color = pen.Color;
            bool overPrint = pen.Overprint;
            color = ColorSpaceHelper.EnsureColorMode(colorMode, color);

            if (_realizedLineWith != pen._width)
            {
                _renderer.AppendFormatArgs("{0:" + format + "} w\n", pen._width);
                _realizedLineWith = pen._width;
            }

            if (_realizedLineCap != (int)pen._lineCap)
            {
                _renderer.AppendFormatArgs("{0} J\n", (int)pen._lineCap);
                _realizedLineCap = (int)pen._lineCap;
            }

            if (_realizedLineJoin != (int)pen._lineJoin)
            {
                _renderer.AppendFormatArgs("{0} j\n", (int)pen._lineJoin);
                _realizedLineJoin = (int)pen._lineJoin;
            }

            if (_realizedLineCap == (int)XLineJoin.Miter)
            {
                if (_realizedMiterLimit != (int)pen._miterLimit && (int)pen._miterLimit != 0)
                {
                    _renderer.AppendFormatInt("{0} M\n", (int)pen._miterLimit);
                    _realizedMiterLimit = (int)pen._miterLimit;
                }
            }

            if (_realizedDashStyle != pen._dashStyle || pen._dashStyle == XDashStyle.Custom)
            {
                double dot = pen.Width;
                double dash = 3 * dot;

                // Line width 0 is not recommended but valid.
                XDashStyle dashStyle = pen.DashStyle;
                if (dot == 0)
                    dashStyle = XDashStyle.Solid;

                switch (dashStyle)
                {
                    case XDashStyle.Solid:
                        _renderer.Append("[]0 d\n");
                        break;

                    case XDashStyle.Dash:
                        _renderer.AppendFormatArgs("[{0:" + frmt2 + "} {1:" + frmt2 + "}]0 d\n", dash, dot);
                        break;

                    case XDashStyle.Dot:
                        _renderer.AppendFormatArgs("[{0:" + frmt2 + "}]0 d\n", dot);
                        break;

                    case XDashStyle.DashDot:
                        _renderer.AppendFormatArgs("[{0:" + frmt2 + "} {1:" + frmt2 + "} {1:" + frmt2 + "} {1:" + frmt2 + "}]0 d\n", dash, dot);
                        break;

                    case XDashStyle.DashDotDot:
                        _renderer.AppendFormatArgs("[{0:" + frmt2 + "} {1:" + frmt2 + "} {1:" + frmt2 + "} {1:" + frmt2 + "} {1:" + frmt2 + "} {1:" + frmt2 + "}]0 d\n", dash, dot);
                        break;

                    case XDashStyle.Custom:
                        {
                            StringBuilder pdf = new StringBuilder("[", 256);
                            int len = pen._dashPattern == null ? 0 : pen._dashPattern.Length;
                            for (int idx = 0; idx < len; idx++)
                            {
                                if (idx > 0)
                                    pdf.Append(' ');
                                pdf.Append(PdfEncoders.ToString(pen._dashPattern[idx] * pen._width));
                            }
                            // Make an even number of values look like in GDI+
                            if (len > 0 && len % 2 == 1)
                            {
                                pdf.Append(' ');
                                pdf.Append(PdfEncoders.ToString(0.2 * pen._width));
                            }
                            pdf.AppendFormat(CultureInfo.InvariantCulture, "]{0:" + format + "} d\n", pen._dashOffset * pen._width);
                            string pattern = pdf.ToString();

                            // BUG: drice2@ageone.de reported a realizing problem
                            // HACK: I remove the if clause
                            //if (_realizedDashPattern != pattern)
                            {
                                _realizedDashPattern = pattern;
                                _renderer.Append(pattern);
                            }
                        }
                        break;
                }
                _realizedDashStyle = dashStyle;
            }

            if (colorMode != PdfColorMode.Cmyk)
            {
                if (_realizedStrokeColor.Rgb != color.Rgb)
                {
                    _renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Rgb));
                    _renderer.Append(" RG\n");
                }
            }
            else
            {
                if (!ColorSpaceHelper.IsEqualCmyk(_realizedStrokeColor, color))
                {
                    _renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Cmyk));
                    _renderer.Append(" K\n");
                }
            }

            if (_renderer.Owner.Version >= 14 && (_realizedStrokeColor.A != color.A || _realizedStrokeOverPrint != overPrint))
            {
                PdfExtGState extGState = _renderer.Owner.ExtGStateTable.GetExtGStateStroke(color.A, overPrint);
                string gs = _renderer.Resources.AddExtGState(extGState);
                _renderer.AppendFormatString("{0} gs\n", gs);

                // Must create transparency group.
                if (_renderer._page != null && color.A < 1)
                    _renderer._page.TransparencyUsed = true;
            }
            _realizedStrokeColor = color;
            _realizedStrokeOverPrint = overPrint;
        }

        #endregion

        #region Fill

        XColor _realizedFillColor = XColor.Empty;
        bool _realizedNonStrokeOverPrint;

        public void RealizeBrush(XBrush brush, PdfColorMode colorMode, int renderingMode, double fontEmSize)
        {
            // Rendering mode 2 is used for bold simulation.
            // Reference: TABLE 5.3  Text rendering modes / Page 402

            XSolidBrush solidBrush = brush as XSolidBrush;
            if (solidBrush != null)
            {
                XColor color = solidBrush.Color;
                bool overPrint = solidBrush.Overprint;

                if (renderingMode == 0)
                {
                    RealizeFillColor(color, overPrint, colorMode);
                }
                else if (renderingMode == 2)
                {
                    // Come here in case of bold simulation.
                    RealizeFillColor(color, false, colorMode);
                    //color = XColors.Green;
                    RealizePen(new XPen(color, fontEmSize * Const.BoldEmphasis), colorMode);
                }
                else
                    throw new InvalidOperationException("Only rendering modes 0 and 2 are currently supported.");
            }
            else
            {
                if (renderingMode != 0)
                    throw new InvalidOperationException("Rendering modes other than 0 can only be used with solid color brushes.");

                XLinearGradientBrush gradientBrush = brush as XLinearGradientBrush;
                if (gradientBrush != null)
                {
                    Debug.Assert(UnrealizedCtm.IsIdentity, "Must realize ctm first.");
                    XMatrix matrix = _renderer.DefaultViewMatrix;
                    matrix.Prepend(EffectiveCtm);
                    PdfShadingPattern pattern = new PdfShadingPattern(_renderer.Owner);
                    pattern.SetupFromBrush(gradientBrush, matrix, _renderer);
                    string name = _renderer.Resources.AddPattern(pattern);
                    _renderer.AppendFormatString("/Pattern cs\n", name);
                    _renderer.AppendFormatString("{0} scn\n", name);

                    // Invalidate fill color.
                    _realizedFillColor = XColor.Empty;
                }
            }
        }

        private void RealizeFillColor(XColor color, bool overPrint, PdfColorMode colorMode)
        {
            color = ColorSpaceHelper.EnsureColorMode(colorMode, color);

            if (colorMode != PdfColorMode.Cmyk)
            {
                if (_realizedFillColor.IsEmpty || _realizedFillColor.Rgb != color.Rgb)
                {
                    _renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Rgb));
                    _renderer.Append(" rg\n");
                }
            }
            else
            {
                Debug.Assert(colorMode == PdfColorMode.Cmyk);

                if (_realizedFillColor.IsEmpty || !ColorSpaceHelper.IsEqualCmyk(_realizedFillColor, color))
                {
                    _renderer.Append(PdfEncoders.ToString(color, PdfColorMode.Cmyk));
                    _renderer.Append(" k\n");
                }
            }

            if (_renderer.Owner.Version >= 14 && (_realizedFillColor.A != color.A || _realizedNonStrokeOverPrint != overPrint))
            {

                PdfExtGState extGState = _renderer.Owner.ExtGStateTable.GetExtGStateNonStroke(color.A, overPrint);
                string gs = _renderer.Resources.AddExtGState(extGState);
                _renderer.AppendFormatString("{0} gs\n", gs);

                // Must create transparency group.
                if (_renderer._page != null && color.A < 1)
                    _renderer._page.TransparencyUsed = true;
            }
            _realizedFillColor = color;
            _realizedNonStrokeOverPrint = overPrint;
        }

        internal void RealizeNonStrokeTransparency(double transparency, PdfColorMode colorMode)
        {
            XColor color = _realizedFillColor;
            color.A = transparency;
            RealizeFillColor(color, _realizedNonStrokeOverPrint, colorMode);
        }

        #endregion

        #region Text

        internal PdfFont _realizedFont;
        string _realizedFontName = String.Empty;
        double _realizedFontSize;
        int _realizedRenderingMode;  // Reference: TABLE 5.2  Text state operators / Page 398
        double _realizedCharSpace;  // Reference: TABLE 5.2  Text state operators / Page 398

        public void RealizeFont(XFont font, XBrush brush, int renderingMode)
        {
            const string format = Config.SignificantFigures3;

            // So far rendering mode 0 (fill text) and 2 (fill, then stroke text) only.
            RealizeBrush(brush, _renderer._colorMode, renderingMode, font.Size); // _renderer.page.document.Options.ColorMode);

            // Realize rendering mode.
            if (_realizedRenderingMode != renderingMode)
            {
                _renderer.AppendFormatInt("{0} Tr\n", renderingMode);
                _realizedRenderingMode = renderingMode;
            }

            // Realize character spacing.
            if (_realizedRenderingMode == 0)
            {
                if (_realizedCharSpace != 0)
                {
                    _renderer.Append("0 Tc\n");
                    _realizedCharSpace = 0;
                }
            }
            else  // _realizedRenderingMode is 2.
            {
                double charSpace = font.Size * Const.BoldEmphasis;
                if (_realizedCharSpace != charSpace)
                {
                    _renderer.AppendFormatDouble("{0:" + format + "} Tc\n", charSpace);
                    _realizedCharSpace = charSpace;
                }
            }

            _realizedFont = null;
            string fontName = _renderer.GetFontName(font, out _realizedFont);
            if (fontName != _realizedFontName || _realizedFontSize != font.Size)
            {
                if (_renderer.Gfx.PageDirection == XPageDirection.Downwards)
                    _renderer.AppendFormatFont("{0} {1:" + format + "} Tf\n", fontName, font.Size);
                else
                    _renderer.AppendFormatFont("{0} {1:" + format + "} Tf\n", fontName, font.Size);
                _realizedFontName = fontName;
                _realizedFontSize = font.Size;
            }
        }

        public XPoint RealizedTextPosition;

        /// <summary>
        /// Indicates that the text transformation matrix currently skews 20° to the right.
        /// </summary>
        public bool ItalicSimulationOn;

        #endregion

        #region Transformation

        /// <summary>
        /// The already realized part of the current transformation matrix.
        /// </summary>
        public XMatrix RealizedCtm;

        /// <summary>
        /// The not yet realized part of the current transformation matrix.
        /// </summary>
        public XMatrix UnrealizedCtm;

        /// <summary>
        /// Product of RealizedCtm and UnrealizedCtm.
        /// </summary>
        public XMatrix EffectiveCtm;

        /// <summary>
        /// Inverse of EffectiveCtm used for transformation.
        /// </summary>
        public XMatrix InverseEffectiveCtm;

        public XMatrix WorldTransform;

        ///// <summary>
        ///// The world transform in PDF world space.
        ///// </summary>
        //public XMatrix EffectiveCtm
        //{
        //  get
        //  {
        //    //if (MustRealizeCtm)
        //    if (!UnrealizedCtm.IsIdentity)
        //    {
        //      XMatrix matrix = RealizedCtm;
        //      matrix.Prepend(UnrealizedCtm);
        //      return matrix;
        //    }
        //    return RealizedCtm;
        //  }
        //  //set
        //  //{
        //  //  XMatrix matrix = realizedCtm;
        //  //  matrix.Invert();
        //  //  matrix.Prepend(value);
        //  //  unrealizedCtm = matrix;
        //  //  MustRealizeCtm = !unrealizedCtm.IsIdentity;
        //  //}
        //}

        public void AddTransform(XMatrix value, XMatrixOrder matrixOrder)
        {
            // TODO: User matrixOrder
#if DEBUG
            if (matrixOrder == XMatrixOrder.Append)
                throw new NotImplementedException("XMatrixOrder.Append");
#endif
            XMatrix transform = value;
            if (_renderer.Gfx.PageDirection == XPageDirection.Downwards)
            {
                // Take chirality into account and
                // invert the direction of rotation.
                transform.M12 = -value.M12;
                transform.M21 = -value.M21;
            }
            UnrealizedCtm.Prepend(transform);

            WorldTransform.Prepend(value);
        }

        /// <summary>
        /// Realizes the CTM.
        /// </summary>
        public void RealizeCtm()
        {
            //if (MustRealizeCtm)
            if (!UnrealizedCtm.IsIdentity)
            {
                Debug.Assert(!UnrealizedCtm.IsIdentity, "mrCtm is unnecessarily set.");

                const string format = Config.SignificantFigures7;

                double[] matrix = UnrealizedCtm.GetElements();
                // Use up to six decimal digits to prevent round up problems.
                _renderer.AppendFormatArgs("{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "} {4:" + format + "} {5:" + format + "} cm\n",
                    matrix[0], matrix[1], matrix[2], matrix[3], matrix[4], matrix[5]);

                RealizedCtm.Prepend(UnrealizedCtm);
                UnrealizedCtm = new XMatrix();
                EffectiveCtm = RealizedCtm;
                InverseEffectiveCtm = EffectiveCtm;
                InverseEffectiveCtm.Invert();
            }
        }
        #endregion

        #region Clip Path

        public void SetAndRealizeClipRect(XRect clipRect)
        {
            XGraphicsPath clipPath = new XGraphicsPath();
            clipPath.AddRectangle(clipRect);
            RealizeClipPath(clipPath);
        }

        public void SetAndRealizeClipPath(XGraphicsPath clipPath)
        {
            RealizeClipPath(clipPath);
        }

        void RealizeClipPath(XGraphicsPath clipPath)
        {
#if CORE
            DiagnosticsHelper.HandleNotImplemented("RealizeClipPath");
#endif
#if GDI
            // Do not render an empty path.
            if (clipPath._gdipPath.PointCount < 0)
                return;
#endif
#if WPF
            // Do not render an empty path.
            if (clipPath._pathGeometry.Bounds.IsEmpty)
                return;
#endif
            _renderer.BeginGraphicMode();
            RealizeCtm();
#if CORE
            _renderer.AppendPath(clipPath._corePath);
#endif
#if GDI && !WPF
            _renderer.AppendPath(clipPath._gdipPath);
#endif
#if WPF && !GDI
            _renderer.AppendPath(clipPath._pathGeometry);
#endif
#if WPF && GDI
            if (_renderer.Gfx.TargetContext == XGraphicTargetContext.GDI)
                _renderer.AppendPath(clipPath._gdipPath);
            else
                _renderer.AppendPath(clipPath._pathGeometry);
#endif
            _renderer.Append(clipPath.FillMode == XFillMode.Winding ? "W n\n" : "W* n\n");
        }

        #endregion
    }
}
