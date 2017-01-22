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

#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
#endif

// Not used in PDFsharp 1.x.

namespace PdfSharp.Drawing
{
#if true_  // PDFSHARP20
    /// <summary>
    /// Defines the density of a typeface, in terms of the lightness or heaviness of the strokes.
    /// </summary>
    [DebuggerDisplay("'{Weight}'")]
    public class XFontWeight : IFormattable
    {
        internal XFontWeight(int weight)
        {
            _weight = weight;
        }

        /// <summary>
        /// Gets the weight of the font, a value between 1 and 999.
        /// </summary>
        public int Weight
        {
            get { return (_weight); }
        }
        private readonly int _weight;

        //public static XFontWeight FromOpenTypeWeight(int weightValue)
        //{
        //  if (weightValue < 1 || weightValue > 999)
        //    throw new ArgumentOutOfRangeException("weightValue", "Parameter must be between 1 and 999.");
        //  return new XFontWeight(weightValue);
        //}

        /// <summary>
        /// Compares the specified font weights.
        /// </summary>
        public static int Compare(XFontWeight left, XFontWeight right)
        {
            return left._weight - right._weight;
        }

        /// <summary>
        /// Implements the operator &lt;.
        /// </summary>
        public static bool operator <(XFontWeight left, XFontWeight right)
        {
            return Compare(left, right) < 0;
        }

        /// <summary>
        /// Implements the operator &lt;=.
        /// </summary>
        public static bool operator <=(XFontWeight left, XFontWeight right)
        {
            return Compare(left, right) <= 0;
        }

        /// <summary>
        /// Implements the operator &gt;.
        /// </summary>
        public static bool operator >(XFontWeight left, XFontWeight right)
        {
            return Compare(left, right) > 0;
        }

        /// <summary>
        /// Implements the operator &gt;=.
        /// </summary>
        public static bool operator >=(XFontWeight left, XFontWeight right)
        {
            return Compare(left, right) >= 0;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        public static bool operator ==(XFontWeight left, XFontWeight right)
        {
            return Compare(left, right) == 0;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        public static bool operator !=(XFontWeight left, XFontWeight right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="XFontWeight"/> is equal to the current <see cref="XFontWeight"/>.
        /// </summary>
        public bool Equals(XFontWeight obj)
        {
            return this == obj;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        public override bool Equals(object obj)
        {
            return (obj is XFontWeight) && this == ((XFontWeight)obj);
        }

        /// <summary>
        /// Serves as a hash function for this type.
        /// </summary>
        public override int GetHashCode()
        {
            return Weight;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return ConvertToString(format, provider);
        }

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            provider = provider ?? CultureInfo.InvariantCulture;
            string str;
            if (!XFontWeights.FontWeightToString(Weight, out str))
                return Weight.ToString(format, provider);
            return str;
        }
    }
#endif
}
