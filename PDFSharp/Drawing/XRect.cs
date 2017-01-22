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
using System.Runtime.InteropServices;
#if CORE
#endif
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
using SysRect = System.Windows.Rect;
#endif
#if NETFX_CORE
using Windows.UI.Xaml.Media;
using SysPoint = Windows.Foundation.Point;
using SysSize = Windows.Foundation.Size;
using SysRect = Windows.Foundation.Rect;
#endif
#if !EDF_CORE
using PdfSharp.Internal;
#else
using PdfSharp.Internal;
#endif

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Stores a set of four floating-point numbers that represent the location and size of a rectangle.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [Serializable, StructLayout(LayoutKind.Sequential)] // , ValueSerializer(typeof(RectValueSerializer)), TypeConverter(typeof(RectConverter))]
    public struct XRect : IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(double x, double y, double width, double height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException("WidthAndHeightCannotBeNegative"); //SR.Get(SRID.Size_WidthAndHeightCannotBeNegative, new object[0]));
            _x = x;
            _y = y;
            _width = width;
            _height = height;
        }

        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(XPoint point1, XPoint point2)
        {
            _x = Math.Min(point1.X, point2.X);
            _y = Math.Min(point1.Y, point2.Y);
            _width = Math.Max(Math.Max(point1.X, point2.X) - _x, 0);
            _height = Math.Max(Math.Max(point1.Y, point2.Y) - _y, 0);
        }

        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(XPoint point, XVector vector)
            : this(point, point + vector)
        { }

        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(XPoint location, XSize size)
        {
            if (size.IsEmpty)
                this = s_empty;
            else
            {
                _x = location.X;
                _y = location.Y;
                _width = size.Width;
                _height = size.Height;
            }
        }

        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(XSize size)
        {
            if (size.IsEmpty)
                this = s_empty;
            else
            {
                _x = _y = 0;
                _width = size.Width;
                _height = size.Height;
            }
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(PointF location, SizeF size)
        {
            _x = location.X;
            _y = location.Y;
            _width = size.Width;
            _height = size.Height;
        }
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(RectangleF rect)
        {
            _x = rect.X;
            _y = rect.Y;
            _width = rect.Width;
            _height = rect.Height;
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Initializes a new instance of the XRect class.
        /// </summary>
        public XRect(SysRect rect)
        {
            _x = rect.X;
            _y = rect.Y;
            _width = rect.Width;
            _height = rect.Height;
        }
#endif

        /// <summary>
        /// Creates a rectangle from for straight lines.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public static XRect FromLTRB(double left, double top, double right, double bottom)
        // ReSharper restore InconsistentNaming
        {
            return new XRect(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Determines whether the two rectangles are equal.
        /// </summary>
        public static bool operator ==(XRect rect1, XRect rect2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return rect1.X == rect2.X && rect1.Y == rect2.Y && rect1.Width == rect2.Width && rect1.Height == rect2.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Determines whether the two rectangles are not equal.
        /// </summary>
        public static bool operator !=(XRect rect1, XRect rect2)
        {
            return !(rect1 == rect2);
        }

        /// <summary>
        /// Determines whether the two rectangles are equal.
        /// </summary>
        public static bool Equals(XRect rect1, XRect rect2)
        {
            if (rect1.IsEmpty)
                return rect2.IsEmpty;
            return rect1.X.Equals(rect2.X) && rect1.Y.Equals(rect2.Y) && rect1.Width.Equals(rect2.Width) && rect1.Height.Equals(rect2.Height);
        }

        /// <summary>
        /// Determines whether this instance and the specified object are equal.
        /// </summary>
        public override bool Equals(object o)
        {
            if (!(o is XRect))
                return false;
            return Equals(this, (XRect)o);
        }

        /// <summary>
        /// Determines whether this instance and the specified rect are equal.
        /// </summary>
        public bool Equals(XRect value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            if (IsEmpty)
                return 0;
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        /// <summary>
        /// Parses the rectangle from a string.
        /// </summary>
        public static XRect Parse(string source)
        {
            XRect empty;
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            TokenizerHelper helper = new TokenizerHelper(source, cultureInfo);
            string str = helper.NextTokenRequired();
            if (str == "Empty")
                empty = Empty;
            else
                empty = new XRect(Convert.ToDouble(str, cultureInfo), Convert.ToDouble(helper.NextTokenRequired(), cultureInfo), Convert.ToDouble(helper.NextTokenRequired(), cultureInfo), Convert.ToDouble(helper.NextTokenRequired(), cultureInfo));
            helper.LastTokenRequired();
            return empty;
        }

        /// <summary>
        /// Converts this XRect to a human readable string.
        /// </summary>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        /// <summary>
        /// Converts this XRect to a human readable string.
        /// </summary>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(null, provider);
        }

        /// <summary>
        /// Converts this XRect to a human readable string.
        /// </summary>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return ConvertToString(format, provider);
        }

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            if (IsEmpty)
                return "Empty";
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            provider = provider ?? CultureInfo.InvariantCulture;
            // ReSharper disable FormatStringProblem
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}", new object[] { numericListSeparator, _x, _y, _width, _height });
            // ReSharper restore FormatStringProblem
        }

        /// <summary>
        /// Gets the empty rectangle.
        /// </summary>
        public static XRect Empty
        {
            get { return s_empty; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _width < 0; }
        }

        /// <summary>
        /// Gets or sets the location of the rectangle.
        /// </summary>
        public XPoint Location
        {
            get { return new XPoint(_x, _y); }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("CannotModifyEmptyRect"); //SR.Get(SRID.Rect_CannotModifyEmptyRect, new object[0]));
                _x = value.X;
                _y = value.Y;
            }
        }

        /// <summary>
        /// Gets or sets the size of the rectangle.
        /// </summary>
        //[Browsable(false)]
        public XSize Size
        {
            get
            {
                if (IsEmpty)
                    return XSize.Empty;
                return new XSize(_width, _height);
            }
            set
            {
                if (value.IsEmpty)
                    this = s_empty;
                else
                {
                    if (IsEmpty)
                        throw new InvalidOperationException("CannotModifyEmptyRect"); //SR.Get(SRID.Rect_CannotModifyEmptyRect, new object[0]));
                    _width = value.Width;
                    _height = value.Height;
                }
            }
        }

        /// <summary>
        /// Gets or sets the X value of the rectangle.
        /// </summary>
        public double X
        {
            get { return _x; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("CannotModifyEmptyRect"); //SR.Get(SRID.Rect_CannotModifyEmptyRect, new object[0]));
                _x = value;
            }
        }
        double _x;

        /// <summary>
        /// Gets or sets the Y value of the rectangle.
        /// </summary>
        public double Y
        {
            get { return _y; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("CannotModifyEmptyRect"); //SR.Get(SRID.Rect_CannotModifyEmptyRect, new object[0]));
                _y = value;
            }
        }
        double _y;

        /// <summary>
        /// Gets or sets the width of the rectangle.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("CannotModifyEmptyRect"); //SR.Get(SRID.Rect_CannotModifyEmptyRect, new object[0]));
                if (value < 0)
                    throw new ArgumentException("WidthCannotBeNegative"); //SR.Get(SRID.Size_WidthCannotBeNegative, new object[0]));

                _width = value;
            }
        }
        double _width;

        /// <summary>
        /// Gets or sets the height of the rectangle.
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("CannotModifyEmptyRect"); //SR.Get(SRID.Rect_CannotModifyEmptyRect, new object[0]));
                if (value < 0)
                    throw new ArgumentException("WidthCannotBeNegative"); //SR.Get(SRID.Size_WidthCannotBeNegative, new object[0]));
                _height = value;
            }
        }
        double _height;

        /// <summary>
        /// Gets the x-axis value of the left side of the rectangle. 
        /// </summary>
        public double Left
        {
            get { return _x; }
        }

        /// <summary>
        /// Gets the y-axis value of the top side of the rectangle. 
        /// </summary>
        public double Top
        {
            get { return _y; }
        }

        /// <summary>
        /// Gets the x-axis value of the right side of the rectangle. 
        /// </summary>
        public double Right
        {
            get
            {
                if (IsEmpty)
                    return double.NegativeInfinity;
                return _x + _width;
            }
        }

        /// <summary>
        /// Gets the y-axis value of the bottom side of the rectangle. 
        /// </summary>
        public double Bottom
        {
            get
            {
                if (IsEmpty)
                    return double.NegativeInfinity;
                return _y + _height;
            }
        }

        /// <summary>
        /// Gets the position of the top-left corner of the rectangle. 
        /// </summary>
        public XPoint TopLeft
        {
            get { return new XPoint(Left, Top); }
        }

        /// <summary>
        /// Gets the position of the top-right corner of the rectangle. 
        /// </summary>
        public XPoint TopRight
        {
            get { return new XPoint(Right, Top); }
        }

        /// <summary>
        /// Gets the position of the bottom-left corner of the rectangle. 
        /// </summary>
        public XPoint BottomLeft
        {
            get { return new XPoint(Left, Bottom); }
        }

        /// <summary>
        /// Gets the position of the bottom-right corner of the rectangle. 
        /// </summary>
        public XPoint BottomRight
        {
            get { return new XPoint(Right, Bottom); }
        }

        /// <summary>
        /// Gets the center of the rectangle.
        /// </summary>
        //[Browsable(false)]
        public XPoint Center
        {
            get { return new XPoint(_x + _width / 2, _y + _height / 2); }
        }

        /// <summary>
        /// Indicates whether the rectangle contains the specified point. 
        /// </summary>
        public bool Contains(XPoint point)
        {
            return Contains(point.X, point.Y);
        }

        /// <summary>
        /// Indicates whether the rectangle contains the specified point. 
        /// </summary>
        public bool Contains(double x, double y)
        {
            if (IsEmpty)
                return false;
            return ContainsInternal(x, y);
        }

        /// <summary>
        /// Indicates whether the rectangle contains the specified rectangle. 
        /// </summary>
        public bool Contains(XRect rect)
        {
            return !IsEmpty && !rect.IsEmpty &&
              _x <= rect._x && _y <= rect._y &&
              _x + _width >= rect._x + rect._width && _y + _height >= rect._y + rect._height;
        }

        /// <summary>
        /// Indicates whether the specified rectangle intersects with the current rectangle.
        /// </summary>
        public bool IntersectsWith(XRect rect)
        {
            return !IsEmpty && !rect.IsEmpty &&
                rect.Left <= Right && rect.Right >= Left &&
                rect.Top <= Bottom && rect.Bottom >= Top;
        }

        /// <summary>
        /// Sets current rectangle to the intersection of the current rectangle and the specified rectangle.
        /// </summary>
        public void Intersect(XRect rect)
        {
            if (!IntersectsWith(rect))
                this = Empty;
            else
            {
                double left = Math.Max(Left, rect.Left);
                double top = Math.Max(Top, rect.Top);
                _width = Math.Max(Math.Min(Right, rect.Right) - left, 0.0);
                _height = Math.Max(Math.Min(Bottom, rect.Bottom) - top, 0.0);
                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Returns the intersection of two rectangles.
        /// </summary>
        public static XRect Intersect(XRect rect1, XRect rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        /// <summary>
        /// Sets current rectangle to the union of the current rectangle and the specified rectangle.
        /// </summary>
        public void Union(XRect rect)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (IsEmpty)
                this = rect;
            else if (!rect.IsEmpty)
            {
                double left = Math.Min(Left, rect.Left);
                double top = Math.Min(Top, rect.Top);
                if (rect.Width == Double.PositiveInfinity || Width == Double.PositiveInfinity)
                    _width = Double.PositiveInfinity;
                else
                {
                    double right = Math.Max(Right, rect.Right);
                    _width = Math.Max(right - left, 0.0);
                }

                if (rect.Height == Double.PositiveInfinity || _height == Double.PositiveInfinity)
                    _height = Double.PositiveInfinity;
                else
                {
                    double bottom = Math.Max(Bottom, rect.Bottom);
                    _height = Math.Max(bottom - top, 0.0);
                }
                _x = left;
                _y = top;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Returns the union of two rectangles.
        /// </summary>
        public static XRect Union(XRect rect1, XRect rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        /// <summary>
        /// Sets current rectangle to the union of the current rectangle and the specified point.
        /// </summary>
        public void Union(XPoint point)
        {
            Union(new XRect(point, point));
        }

        /// <summary>
        /// Returns the intersection of a rectangle and a point.
        /// </summary>
        public static XRect Union(XRect rect, XPoint point)
        {
            rect.Union(new XRect(point, point));
            return rect;
        }

        /// <summary>
        /// Moves a rectangle by the specified amount.
        /// </summary>
        public void Offset(XVector offsetVector)
        {
            if (IsEmpty)
                throw new InvalidOperationException("CannotCallMethod"); //SR.Get(SRID.Rect_CannotCallMethod, new object[0]));
            _x += offsetVector.X;
            _y += offsetVector.Y;
        }

        /// <summary>
        /// Moves a rectangle by the specified amount.
        /// </summary>
        public void Offset(double offsetX, double offsetY)
        {
            if (IsEmpty)
                throw new InvalidOperationException("CannotCallMethod"); //SR.Get(SRID.Rect_CannotCallMethod, new object[0]));
            _x += offsetX;
            _y += offsetY;
        }

        /// <summary>
        /// Returns a rectangle that is offset from the specified rectangle by using the specified vector. 
        /// </summary>
        public static XRect Offset(XRect rect, XVector offsetVector)
        {
            rect.Offset(offsetVector.X, offsetVector.Y);
            return rect;
        }

        /// <summary>
        /// Returns a rectangle that is offset from the specified rectangle by using specified horizontal and vertical amounts. 
        /// </summary>
        public static XRect Offset(XRect rect, double offsetX, double offsetY)
        {
            rect.Offset(offsetX, offsetY);
            return rect;
        }

        /// <summary>
        /// Translates the rectangle by adding the specified point.
        /// </summary>
        //[Obsolete("Use Offset.")]
        public static XRect operator +(XRect rect, XPoint point)
        {
            return new XRect(rect._x + point.X, rect.Y + point.Y, rect._width, rect._height);
        }

        /// <summary>
        /// Translates the rectangle by subtracting the specified point.
        /// </summary>
        //[Obsolete("Use Offset.")]
        public static XRect operator -(XRect rect, XPoint point)
        {
            return new XRect(rect._x - point.X, rect.Y - point.Y, rect._width, rect._height);
        }

        /// <summary>
        /// Expands the rectangle by using the specified Size, in all directions.
        /// </summary>
        public void Inflate(XSize size)
        {
            Inflate(size.Width, size.Height);
        }

        /// <summary>
        /// Expands or shrinks the rectangle by using the specified width and height amounts, in all directions.
        /// </summary>
        public void Inflate(double width, double height)
        {
            if (IsEmpty)
                throw new InvalidOperationException("CannotCallMethod"); //SR.Get(SRID.Rect_CannotCallMethod, new object[0]));
            _x -= width;
            _y -= height;
            _width += width;
            _width += width;
            _height += height;
            _height += height;
            if (_width < 0 || _height < 0)
                this = s_empty;
        }

        /// <summary>
        /// Returns the rectangle that results from expanding the specified rectangle by the specified Size, in all directions.
        /// </summary>
        public static XRect Inflate(XRect rect, XSize size)
        {
            rect.Inflate(size.Width, size.Height);
            return rect;
        }

        /// <summary>
        /// Creates a rectangle that results from expanding or shrinking the specified rectangle by the specified width and height amounts, in all directions.
        /// </summary>
        public static XRect Inflate(XRect rect, double width, double height)
        {
            rect.Inflate(width, height);
            return rect;
        }

        /// <summary>
        /// Returns the rectangle that results from applying the specified matrix to the specified rectangle.
        /// </summary>
        public static XRect Transform(XRect rect, XMatrix matrix)
        {
            XMatrix.MatrixHelper.TransformRect(ref rect, ref matrix);
            return rect;
        }

        /// <summary>
        /// Transforms the rectangle by applying the specified matrix.
        /// </summary>
        public void Transform(XMatrix matrix)
        {
            XMatrix.MatrixHelper.TransformRect(ref this, ref matrix);
        }

        /// <summary>
        /// Multiplies the size of the current rectangle by the specified x and y values.
        /// </summary>
        public void Scale(double scaleX, double scaleY)
        {
            if (!IsEmpty)
            {
                _x *= scaleX;
                _y *= scaleY;
                _width *= scaleX;
                _height *= scaleY;
                if (scaleX < 0)
                {
                    _x += _width;
                    _width *= -1.0;
                }
                if (scaleY < 0)
                {
                    _y += _height;
                    _height *= -1.0;
                }
            }
        }

#if CORE  // Internal version in CORE build.
#if UseGdiObjects
        /// <summary>
        /// Converts this instance to a System.Drawing.RectangleF.
        /// </summary>
        internal RectangleF ToRectangleF()
        {
            return new RectangleF((float)_x, (float)_y, (float)_width, (float)_height);
        }
#endif
#endif

#if GDI
        /// <summary>
        /// Converts this instance to a System.Drawing.RectangleF.
        /// </summary>
        public RectangleF ToRectangleF()
        {
            return new RectangleF((float)_x, (float)_y, (float)_width, (float)_height);
        }
#endif

#if GDI
        /// <summary>
        /// Performs an implicit  conversion from a System.Drawing.Rectangle to an XRect.
        /// </summary>
        public static implicit operator XRect(Rectangle rect)
        {
            return new XRect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Performs an implicit  conversion from a System.Drawing.RectangleF to an XRect.
        /// </summary>
        public static implicit operator XRect(RectangleF rect)
        {
            return new XRect(rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Performs an implicit conversion from System.Windows.Rect to XRect.
        /// </summary>
        public static implicit operator XRect(SysRect rect)
        {
            return new XRect(rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        bool ContainsInternal(double x, double y)
        {
            return x >= _x && x - _width <= _x && y >= _y && y - _height <= _y;
        }

        static XRect CreateEmptyRect()
        {
            XRect rect = new XRect();
            rect._x = double.PositiveInfinity;
            rect._y = double.PositiveInfinity;
            rect._width = double.NegativeInfinity;
            rect._height = double.NegativeInfinity;
            return rect;
        }

        static XRect()
        {
            s_empty = CreateEmptyRect();
        }

        private static readonly XRect s_empty;

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        /// <value>The debugger display.</value>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
                const string format = Config.SignificantFigures10;
                return String.Format(CultureInfo.InvariantCulture,
                    "rect=({0:" + format + "}, {1:" + format + "}, {2:" + format + "}, {3:" + format + "})",
                    _x, _y, _width, _height);
            }
        }
    }
}
