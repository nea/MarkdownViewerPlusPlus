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
#endif
#if WPF
using System.Windows;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
#endif
#if NETFX_CORE
using Windows.UI.Xaml.Media;
using SysPoint = Windows.Foundation.Point;
using SysSize = Windows.Foundation.Size;
#endif
#if !EDF_CORE
using PdfSharp.Internal;
#else
using PdfSharp.Internal;
#endif

#if !EDF_CORE
namespace PdfSharp.Drawing
#else
namespace Edf.Drawing
#endif
{
    /// <summary>
    /// Represents a pair of floating point x- and y-coordinates that defines a point
    /// in a two-dimensional plane.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]  // TypeConverter(typeof(PointConverter)), ValueSerializer(typeof(PointValueSerializer))]
    public struct XPoint : IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the XPoint class with the specified coordinates.
        /// </summary>
        public XPoint(double x, double y)
        {
            _x = x;
            _y = y;
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the XPoint class with the specified point.
        /// </summary>
        public XPoint(System.Drawing.Point point)
        {
            _x = point.X;
            _y = point.Y;
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Initializes a new instance of the XPoint class with the specified point.
        /// </summary>
        public XPoint(SysPoint point)
        {
            _x = point.X;
            _y = point.Y;
        }
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the XPoint class with the specified point.
        /// </summary>
        public XPoint(PointF point)
        {
            _x = point.X;
            _y = point.Y;
        }
#endif

        /// <summary>
        /// Determines whether two points are equal.
        /// </summary>
        public static bool operator ==(XPoint point1, XPoint point2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return point1._x == point2._x && point1._y == point2._y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Determines whether two points are not equal.
        /// </summary>
        public static bool operator !=(XPoint point1, XPoint point2)
        {
            return !(point1 == point2);
        }

        /// <summary>
        /// Indicates whether the specified points are equal.
        /// </summary>
        public static bool Equals(XPoint point1, XPoint point2)
        {
            return point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        public override bool Equals(object o)
        {
            if (!(o is XPoint))
                return false;
            return Equals(this, (XPoint)o);
        }

        /// <summary>
        /// Indicates whether this instance and a specified point are equal.
        /// </summary>
        public bool Equals(XPoint value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        /// <summary>
        /// Parses the point from a string.
        /// </summary>
        public static XPoint Parse(string source)
        {
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            TokenizerHelper helper = new TokenizerHelper(source, cultureInfo);
            string str = helper.NextTokenRequired();
            XPoint point = new XPoint(Convert.ToDouble(str, cultureInfo), Convert.ToDouble(helper.NextTokenRequired(), cultureInfo));
            helper.LastTokenRequired();
            return point;
        }

        /// <summary>
        /// Parses an array of points from a string.
        /// </summary>
        public static XPoint[] ParsePoints(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            // TODO: Reflect reliabel implementation from Avalon
            // TODOWPF
            string[] values = value.Split(' ');
            int count = values.Length;
            XPoint[] points = new XPoint[count];
            for (int idx = 0; idx < count; idx++)
                points[idx] = Parse(values[idx]);
            return points;
        }

        /// <summary>
        /// Gets the x-coordinate of this XPoint.
        /// </summary>
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        double _x;

        /// <summary>
        /// Gets the x-coordinate of this XPoint.
        /// </summary>
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
        double _y;

#if CORE
#if UseGdiObjects
        /// <summary>
        /// Converts this XPoint to a System.Drawing.Point.
        /// </summary>
        public PointF ToPointF()
        {
            return new PointF((float)_x, (float)_y);
        }
#endif
#endif

#if GDI
        /// <summary>
        /// Converts this XPoint to a System.Drawing.Point.
        /// </summary>
        public PointF ToPointF()
        {
            return new PointF((float)_x, (float)_y);
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Converts this XPoint to a System.Windows.Point.
        /// </summary>
        public SysPoint ToPoint()
        {
            return new SysPoint(_x, _y);
        }
#endif

        /// <summary>
        /// Converts this XPoint to a human readable string.
        /// </summary>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        /// <summary>
        /// Converts this XPoint to a human readable string.
        /// </summary>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(null, provider);
        }

        /// <summary>
        /// Converts this XPoint to a human readable string.
        /// </summary>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return ConvertToString(format, provider);
        }

        /// <summary>
        /// Implements ToString.
        /// </summary>
        internal string ConvertToString(string format, IFormatProvider provider)
        {
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            provider = provider ?? CultureInfo.InvariantCulture;
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { numericListSeparator, _x, _y });
        }

        /// <summary>
        /// Offsets the x and y value of this point.
        /// </summary>
        public void Offset(double offsetX, double offsetY)
        {
            _x += offsetX;
            _y += offsetY;
        }

        /// <summary>
        /// Adds a point and a vector.
        /// </summary>
        public static XPoint operator +(XPoint point, XVector vector)
        {
            return new XPoint(point._x + vector.X, point._y + vector.Y);
        }

        /// <summary>
        /// Adds a point and a size.
        /// </summary>
        public static XPoint operator +(XPoint point, XSize size) // TODO: make obsolete
        {
            return new XPoint(point._x + size.Width, point._y + size.Height);
        }

        /// <summary>
        /// Adds a point and a vector.
        /// </summary>
        public static XPoint Add(XPoint point, XVector vector)
        {
            return new XPoint(point._x + vector.X, point._y + vector.Y);
        }

        /// <summary>
        /// Subtracts a vector from a point.
        /// </summary>
        public static XPoint operator -(XPoint point, XVector vector)
        {
            return new XPoint(point._x - vector.X, point._y - vector.Y);
        }

        /// <summary>
        /// Subtracts a vector from a point.
        /// </summary>
        public static XPoint Subtract(XPoint point, XVector vector)
        {
            return new XPoint(point._x - vector.X, point._y - vector.Y);
        }

        /// <summary>
        /// Subtracts a point from a point.
        /// </summary>
        public static XVector operator -(XPoint point1, XPoint point2)
        {
            return new XVector(point1._x - point2._x, point1._y - point2._y);
        }

        /// <summary>
        /// Subtracts a size from a point.
        /// </summary>
        [Obsolete("Use XVector instead of XSize as second parameter.")]
        public static XPoint operator -(XPoint point, XSize size) // TODO: make obsolete
        {
            return new XPoint(point._x - size.Width, point._y - size.Height);
        }

        /// <summary>
        /// Subtracts a point from a point.
        /// </summary>
        public static XVector Subtract(XPoint point1, XPoint point2)
        {
            return new XVector(point1._x - point2._x, point1._y - point2._y);
        }

        /// <summary>
        /// Multiplies a point with a matrix.
        /// </summary>
        public static XPoint operator *(XPoint point, XMatrix matrix)
        {
            return matrix.Transform(point);
        }

        /// <summary>
        /// Multiplies a point with a matrix.
        /// </summary>
        public static XPoint Multiply(XPoint point, XMatrix matrix)
        {
            return matrix.Transform(point);
        }

        /// <summary>
        /// Multiplies a point with a scalar value.
        /// </summary>
        public static XPoint operator *(XPoint point, double value)
        {
            return new XPoint(point._x * value, point._y * value);
        }

        /// <summary>
        /// Multiplies a point with a scalar value.
        /// </summary>
        public static XPoint operator *(double value, XPoint point)
        {
            return new XPoint(value * point._x, value * point._y);
        }

        /// <summary>
        /// Performs an explicit conversion from XPoint to XSize.
        /// </summary>
        public static explicit operator XSize(XPoint point)
        {
            return new XSize(Math.Abs(point._x), Math.Abs(point._y));
        }

        /// <summary>
        /// Performs an explicit conversion from XPoint to XVector.
        /// </summary>
        public static explicit operator XVector(XPoint point)
        {
            return new XVector(point._x, point._y);
        }

#if WPF || NETFX_CORE
        /// <summary>
        /// Performs an implicit conversion from XPoint to Point.
        /// </summary>
        public static implicit operator SysPoint(XPoint point)
        {
            return new SysPoint(point.X, point.Y);
        }

        /// <summary>
        /// Performs an implicit conversion from Point to XPoint.
        /// </summary>
        public static implicit operator XPoint(SysPoint point)
        {
            return new XPoint(point.X, point.Y);
        }
#endif

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
                const string format = Config.SignificantFigures10;
                return String.Format(CultureInfo.InvariantCulture, "point=({0:" + format + "}, {1:" + format + "})", _x, _y);
            }
        }
    }
}
