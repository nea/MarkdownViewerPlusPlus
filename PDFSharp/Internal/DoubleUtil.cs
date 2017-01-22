#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Microsoft
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
using System.Runtime.InteropServices;
#if !EDF_CORE
using PdfSharp.Drawing;
#else
using PdfSharp.Drawing;
#endif

#if !EDF_CORE
namespace PdfSharp.Internal
#else
namespace Edf.Internal
#endif
{
    /// <summary>
    /// Some floating point utilities. Partially reflected from WPF, later equalized with original source code.
    /// </summary>
    internal static class DoubleUtil
    {
        const double Epsilon = 2.2204460492503131E-16; // smallest such that 1.0 + Epsilon != 1.0
        private const double TenTimesEpsilon = 10.0 * Epsilon;
        const float FloatMinimum = 1.175494E-38f;

        /// <summary>
        /// Indicates whether the values are so close that they can be considered as equal.
        /// </summary>
        public static bool AreClose(double value1, double value2)
        {
            //if (value1 == value2)
            if (value1.Equals(value2))
                return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < Epsilon 
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * Epsilon;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// Indicates whether the values are so close that they can be considered as equal.
        /// </summary>
        public static bool AreRoughlyEqual(double value1, double value2, int decimalPlace)
        {
            if (value1 == value2)
                return true;
            return Math.Abs(value1 - value2) < decs[decimalPlace];
        }
        static readonly double[] decs = { 1, 1E-1, 1E-2, 1E-3, 1E-4, 1E-5, 1E-6, 1E-7, 1E-8, 1E-9, 1E-10, 1E-11, 1E-12, 1E-13, 1E-14, 1E-15, 1E-16 };

        /// <summary>
        /// Indicates whether the values are so close that they can be considered as equal.
        /// </summary>
        public static bool AreClose(XPoint point1, XPoint point2)
        {
            return AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y);
        }

        /// <summary>
        /// Indicates whether the values are so close that they can be considered as equal.
        /// </summary>
        public static bool AreClose(XRect rect1, XRect rect2)
        {
            if (rect1.IsEmpty)
                return rect2.IsEmpty;
            return !rect2.IsEmpty && AreClose(rect1.X, rect2.X) && AreClose(rect1.Y, rect2.Y) &&
              AreClose(rect1.Height, rect2.Height) && AreClose(rect1.Width, rect2.Width);
        }

        /// <summary>
        /// Indicates whether the values are so close that they can be considered as equal.
        /// </summary>
        public static bool AreClose(XSize size1, XSize size2)
        {
            return AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height);
        }

        /// <summary>
        /// Indicates whether the values are so close that they can be considered as equal.
        /// </summary>
        public static bool AreClose(XVector vector1, XVector vector2)
        {
            return AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y);
        }

        /// <summary>
        /// Indicates whether value1 is greater than value2 and the values are not close to each other.
        /// </summary>
        public static bool GreaterThan(double value1, double value2)
        {
            return value1 > value2 && !AreClose(value1, value2);
        }

        /// <summary>
        /// Indicates whether value1 is greater than value2 or the values are close to each other.
        /// </summary>
        public static bool GreaterThanOrClose(double value1, double value2)
        {
            return value1 > value2 || AreClose(value1, value2);
        }

        /// <summary>
        /// Indicates whether value1 is less than value2 and the values are not close to each other.
        /// </summary>
        public static bool LessThan(double value1, double value2)
        {
            return value1 < value2 && !AreClose(value1, value2);
        }

        /// <summary>
        /// Indicates whether value1 is less than value2 or the values are close to each other.
        /// </summary>
        public static bool LessThanOrClose(double value1, double value2)
        {
            return value1 < value2 || AreClose(value1, value2);
        }

        /// <summary>
        /// Indicates whether the value is between 0 and 1 or close to 0 or 1.
        /// </summary>
        public static bool IsBetweenZeroAndOne(double value)
        {
            return GreaterThanOrClose(value, 0) && LessThanOrClose(value, 1);
        }

        /// <summary>
        /// Indicates whether the value is not a number.
        /// </summary>
        public static bool IsNaN(double value)
        {
            NanUnion t = new NanUnion();
            t.DoubleValue = value;

            ulong exp = t.UintValue & 0xfff0000000000000;
            ulong man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }

        /// <summary>
        /// Indicates whether at least one of the four rectangle values is not a number.
        /// </summary>
        public static bool RectHasNaN(XRect r)
        {
            return IsNaN(r.X) || IsNaN(r.Y) || IsNaN(r.Height) || IsNaN(r.Width);
        }

        /// <summary>
        /// Indicates whether the value is 1 or close to 1.
        /// </summary>
        public static bool IsOne(double value)
        {
            return Math.Abs(value - 1.0) < TenTimesEpsilon;
        }

        /// <summary>
        /// Indicates whether the value is 0 or close to 0.
        /// </summary>
        public static bool IsZero(double value)
        {
            return Math.Abs(value) < TenTimesEpsilon;
        }

        /// <summary>
        /// Converts a double to integer.
        /// </summary>
        public static int DoubleToInt(double value)
        {
            return 0 < value ? (int)(value + 0.5) : (int)(value - 0.5);
        }

        [StructLayout(LayoutKind.Explicit)]
        struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal readonly ulong UintValue;
        }
    }
}
