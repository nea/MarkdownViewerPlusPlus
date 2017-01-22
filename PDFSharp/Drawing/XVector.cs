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
using PdfSharp.Internal;
#if GDI
using System.Drawing;
#endif
#if WPF
using System.Windows.Media;
#endif

#pragma warning disable 1591

#if !EDF_CORE
namespace PdfSharp.Drawing
#else
namespace Edf.Drawing
#endif
{
    /// <summary>
    /// Represents a two-dimensional vector specified by x- and y-coordinates.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct XVector : IFormattable
    {
        public XVector(double x, double y)
        {
            _x = x;
            _y = y;
        }

        public static bool operator ==(XVector vector1, XVector vector2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return vector1._x == vector2._x && vector1._y == vector2._y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static bool operator !=(XVector vector1, XVector vector2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return vector1._x != vector2._x || vector1._y != vector2._y;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static bool Equals(XVector vector1, XVector vector2)
        {
            if (vector1.X.Equals(vector2.X))
                return vector1.Y.Equals(vector2.Y);
            return false;
        }

        public override bool Equals(object o)
        {
            if (!(o is XVector))
                return false;
            return Equals(this, (XVector)o);
        }

        public bool Equals(XVector value)
        {
            return Equals(this, value);
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return _x.GetHashCode() ^ _y.GetHashCode();
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        public static XVector Parse(string source)
        {
            TokenizerHelper helper = new TokenizerHelper(source, CultureInfo.InvariantCulture);
            string str = helper.NextTokenRequired();
            XVector vector = new XVector(Convert.ToDouble(str, CultureInfo.InvariantCulture), Convert.ToDouble(helper.NextTokenRequired(), CultureInfo.InvariantCulture));
            helper.LastTokenRequired();
            return vector;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }
        double _x;

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }
        double _y;

        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(null, provider);
        }

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return ConvertToString(format, provider);
        }

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            const char numericListSeparator = ',';
            provider = provider ?? CultureInfo.InvariantCulture;
            // ReSharper disable once FormatStringProblem
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", numericListSeparator, _x, _y);
        }

        public double Length
        {
            get { return Math.Sqrt(_x * _x + _y * _y); }
        }

        public double LengthSquared
        {
            get { return _x * _x + _y * _y; }
        }

        public void Normalize()
        {
            this = this / Math.Max(Math.Abs(_x), Math.Abs(_y));
            this = this / Length;
        }

        public static double CrossProduct(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static double AngleBetween(XVector vector1, XVector vector2)
        {
            double y = vector1._x * vector2._y - vector2._x * vector1._y;
            double x = vector1._x * vector2._x + vector1._y * vector2._y;
            return (Math.Atan2(y, x) * 57.295779513082323);
        }

        public static XVector operator -(XVector vector)
        {
            return new XVector(-vector._x, -vector._y);
        }

        public void Negate()
        {
            _x = -_x;
            _y = -_y;
        }

        public static XVector operator +(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x + vector2._x, vector1._y + vector2._y);
        }

        public static XVector Add(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x + vector2._x, vector1._y + vector2._y);
        }

        public static XVector operator -(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x - vector2._x, vector1._y - vector2._y);
        }

        public static XVector Subtract(XVector vector1, XVector vector2)
        {
            return new XVector(vector1._x - vector2._x, vector1._y - vector2._y);
        }

        public static XPoint operator +(XVector vector, XPoint point)
        {
            return new XPoint(point.X + vector._x, point.Y + vector._y);
        }

        public static XPoint Add(XVector vector, XPoint point)
        {
            return new XPoint(point.X + vector._x, point.Y + vector._y);
        }

        public static XVector operator *(XVector vector, double scalar)
        {
            return new XVector(vector._x * scalar, vector._y * scalar);
        }

        public static XVector Multiply(XVector vector, double scalar)
        {
            return new XVector(vector._x * scalar, vector._y * scalar);
        }

        public static XVector operator *(double scalar, XVector vector)
        {
            return new XVector(vector._x * scalar, vector._y * scalar);
        }

        public static XVector Multiply(double scalar, XVector vector)
        {
            return new XVector(vector._x * scalar, vector._y * scalar);
        }

        public static XVector operator /(XVector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        public static XVector Divide(XVector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        public static XVector operator *(XVector vector, XMatrix matrix)
        {
            return matrix.Transform(vector);
        }

        public static XVector Multiply(XVector vector, XMatrix matrix)
        {
            return matrix.Transform(vector);
        }

        public static double operator *(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        public static double Multiply(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        public static double Determinant(XVector vector1, XVector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }

        public static explicit operator XSize(XVector vector)
        {
            return new XSize(Math.Abs(vector._x), Math.Abs(vector._y));
        }

        public static explicit operator XPoint(XVector vector)
        {
            return new XPoint(vector._x, vector._y);
        }

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
                return string.Format(CultureInfo.InvariantCulture, "vector=({0:" + format + "}, {1:" + format + "})", _x, _y);
            }
        }
    }
}
