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
using System.ComponentModel;
using PdfSharp.Internal;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiLinearGradientBrush =System.Drawing.Drawing2D.LinearGradientBrush;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
using SysRect = System.Windows.Rect;
using WpfBrush = System.Windows.Media.Brush;
#endif
#if UWP
using Windows.UI;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
#endif

// ReSharper disable RedundantNameQualifier because it is required for hybrid build

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Defines a Brush with a linear gradient.
    /// </summary>
    public sealed class XLinearGradientBrush : XBrush
    {
        //internal XLinearGradientBrush();

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(System.Drawing.Point point1, System.Drawing.Point point2, XColor color1, XColor color2)
            : this(new XPoint(point1), new XPoint(point2), color1, color2)
        { }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(SysPoint point1, SysPoint point2, XColor color1, XColor color2)
            : this(new XPoint(point1), new XPoint(point2), color1, color2)
        { }
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(PointF point1, PointF point2, XColor color1, XColor color2)
            : this(new XPoint(point1), new XPoint(point2), color1, color2)
        { }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XPoint point1, XPoint point2, XColor color1, XColor color2)
        {
            _point1 = point1;
            _point2 = point2;
            _color1 = color1;
            _color2 = color2;
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
            : this(new XRect(rect), color1, color2, linearGradientMode)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
            : this(new XRect(rect), color1, color2, linearGradientMode)
        { }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(Rect rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
            : this(new XRect(rect), color1, color2, linearGradientMode)
        { }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="XLinearGradientBrush"/> class.
        /// </summary>
        public XLinearGradientBrush(XRect rect, XColor color1, XColor color2, XLinearGradientMode linearGradientMode)
        {
            if (!Enum.IsDefined(typeof(XLinearGradientMode), linearGradientMode))
                throw new InvalidEnumArgumentException("linearGradientMode", (int)linearGradientMode, typeof(XLinearGradientMode));

            if (rect.Width == 0 || rect.Height == 0)
                throw new ArgumentException("Invalid rectangle.", "rect");

            _useRect = true;
            _color1 = color1;
            _color2 = color2;
            _rect = rect;
            _linearGradientMode = linearGradientMode;
        }

        // TODO: 
        //public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, double angle);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle);
        //public XLinearGradientBrush(Rectangle rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);
        //public XLinearGradientBrush(RectangleF rect, XColor color1, XColor color2, double angle, bool isAngleScaleable);

        //private Blend _GetBlend();
        //private ColorBlend _GetInterpolationColors();
        //private XColor[] _GetLinearColors();
        //private RectangleF _GetRectangle();
        //private Matrix _GetTransform();
        //private WrapMode _GetWrapMode();
        //private void _SetBlend(Blend blend);
        //private void _SetInterpolationColors(ColorBlend blend);
        //private void _SetLinearColors(XColor color1, XColor color2);
        //private void _SetTransform(Matrix matrix);
        //private void _SetWrapMode(WrapMode wrapMode);

        //public override object Clone();

        /// <summary>
        /// Gets or sets an XMatrix that defines a local geometric transform for this LinearGradientBrush.
        /// </summary>
        public XMatrix Transform
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        /// <summary>
        /// Translates the brush with the specified offset.
        /// </summary>
        public void TranslateTransform(double dx, double dy)
        {
            _matrix.TranslatePrepend(dx, dy);
        }

        /// <summary>
        /// Translates the brush with the specified offset.
        /// </summary>
        public void TranslateTransform(double dx, double dy, XMatrixOrder order)
        {
            _matrix.Translate(dx, dy, order);
        }

        /// <summary>
        /// Scales the brush with the specified scalars.
        /// </summary>
        public void ScaleTransform(double sx, double sy)
        {
            _matrix.ScalePrepend(sx, sy);
        }

        /// <summary>
        /// Scales the brush with the specified scalars.
        /// </summary>
        public void ScaleTransform(double sx, double sy, XMatrixOrder order)
        {
            _matrix.Scale(sx, sy, order);
        }

        /// <summary>
        /// Rotates the brush with the specified angle.
        /// </summary>
        public void RotateTransform(double angle)
        {
            _matrix.RotatePrepend(angle);
        }

        /// <summary>
        /// Rotates the brush with the specified angle.
        /// </summary>
        public void RotateTransform(double angle, XMatrixOrder order)
        {
            _matrix.Rotate(angle, order);
        }

        /// <summary>
        /// Multiply the brush transformation matrix with the specified matrix.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix)
        {
            _matrix.Prepend(matrix);
        }

        /// <summary>
        /// Multiply the brush transformation matrix with the specified matrix.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix, XMatrixOrder order)
        {
            _matrix.Multiply(matrix, order);
        }

        /// <summary>
        /// Resets the brush transformation matrix with identity matrix.
        /// </summary>
        public void ResetTransform()
        {
            _matrix = new XMatrix();
        }

        //public void SetBlendTriangularShape(double focus);
        //public void SetBlendTriangularShape(double focus, double scale);
        //public void SetSigmaBellShape(double focus);
        //public void SetSigmaBellShape(double focus, double scale);

#if GDI
        internal override System.Drawing.Brush RealizeGdiBrush()
        {
            //if (dirty)
            //{
            //  if (brush == null)
            //    brush = new SolidBrush(color.ToGdiColor());
            //  else
            //  {
            //    brush.Color = color.ToGdiColor();
            //  }
            //  dirty = false;
            //}

            // TODO: use dirty to optimize code
            GdiLinearGradientBrush brush;
            try
            {
                Lock.EnterGdiPlus();
                if (_useRect)
                {
                    brush = new GdiLinearGradientBrush(_rect.ToRectangleF(),
                        _color1.ToGdiColor(), _color2.ToGdiColor(), (LinearGradientMode)_linearGradientMode);
                }
                else
                {
                    brush = new GdiLinearGradientBrush(
                        _point1.ToPointF(), _point2.ToPointF(),
                        _color1.ToGdiColor(), _color2.ToGdiColor());
                }
                if (!_matrix.IsIdentity)
                    brush.Transform = _matrix.ToGdiMatrix();
                //brush.WrapMode = WrapMode.Clamp;
            }
            finally { Lock.ExitGdiPlus(); }
            return brush;
        }
#endif

#if WPF
        internal override WpfBrush RealizeWpfBrush()
        {
            //if (dirty)
            //{
            //  if (brush == null)
            //    brush = new SolidBrush(color.ToGdiColor());
            //  else
            //  {
            //    brush.Color = color.ToGdiColor();
            //  }
            //  dirty = false;
            //}

            System.Windows.Media.LinearGradientBrush brush;
            if (_useRect)
            {
#if !SILVERLIGHT
                brush = new System.Windows.Media.LinearGradientBrush(_color1.ToWpfColor(), _color2.ToWpfColor(), new SysPoint(0, 0), new SysPoint(1, 1));// rect.TopLeft, this.rect.BottomRight);
                //brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect.ToRectangleF(),
                //  color1.ToGdiColor(), color2.ToGdiColor(), (LinearGradientMode)linearGradientMode);
#else
                GradientStop gs1 = new GradientStop();
                gs1.Color = _color1.ToWpfColor();
                gs1.Offset = 0;

                GradientStop gs2 = new GradientStop();
                gs2.Color = _color2.ToWpfColor();
                gs2.Offset = 1;

                GradientStopCollection gsc = new GradientStopCollection();
                gsc.Add(gs1);
                gsc.Add(gs2);

                brush = new LinearGradientBrush(gsc, 0);
                brush.StartPoint = new Point(0, 0);
                brush.EndPoint = new Point(1, 1);
#endif

            }
            else
            {
#if !SILVERLIGHT
                brush = new System.Windows.Media.LinearGradientBrush(_color1.ToWpfColor(), _color2.ToWpfColor(), _point1, _point2);
                //brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                //  point1.ToPointF(), point2.ToPointF(),
                //  color1.ToGdiColor(), color2.ToGdiColor());
#else
                GradientStop gs1 = new GradientStop();
                gs1.Color = _color1.ToWpfColor();
                gs1.Offset = 0;

                GradientStop gs2 = new GradientStop();
                gs2.Color = _color2.ToWpfColor();
                gs2.Offset = 1;

                GradientStopCollection gsc = new GradientStopCollection();
                gsc.Add(gs1);
                gsc.Add(gs2);

                brush = new LinearGradientBrush(gsc, 0);
                brush.StartPoint = _point1;
                brush.EndPoint = _point2;
#endif
            }
            if (!_matrix.IsIdentity)
            {
#if !SILVERLIGHT
                brush.Transform = new MatrixTransform(_matrix.ToWpfMatrix());
#else
                MatrixTransform transform = new MatrixTransform();
                transform.Matrix = _matrix.ToWpfMatrix();
                brush.Transform = transform;
#endif
            }
            return brush;
        }
#endif

#if UWP
        internal override ICanvasBrush RealizeCanvasBrush()
        {
            ICanvasBrush brush;

            brush = new CanvasSolidColorBrush(CanvasDevice.GetSharedDevice(), Colors.RoyalBlue);

            return brush;
        }
#endif

        //public Blend Blend { get; set; }
        //public bool GammaCorrection { get; set; }
        //public ColorBlend InterpolationColors { get; set; }
        //public XColor[] LinearColors { get; set; }
        //public RectangleF Rectangle { get; }
        //public WrapMode WrapMode { get; set; }
        //private bool interpolationColorsWasSet;

        internal bool _useRect;
        internal XPoint _point1, _point2;
        internal XColor _color1, _color2;
        internal XRect _rect;
        internal XLinearGradientMode _linearGradientMode;

        internal XMatrix _matrix;
    }
}