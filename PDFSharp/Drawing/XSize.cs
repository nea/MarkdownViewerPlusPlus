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

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Represents a pair of floating-point numbers, typically the width and height of a
    /// graphical object.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [Serializable, StructLayout(LayoutKind.Sequential)] //, ValueSerializer(typeof(SizeValueSerializer)), TypeConverter(typeof(SizeConverter))]
    public struct XSize : IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the XPoint class with the specified values.
        /// </summary>
        public XSize(double width, double height)
        {
            if (width < 0 || height < 0)
                throw new ArgumentException("WidthAndHeightCannotBeNegative"); //SR.Get(SRID.Size_WidthAndHeightCannotBeNegative, new object[0]));

            _width = width;
            _height = height;
        }

        /// <summary>
        /// Determines whether two size objects are equal.
        /// </summary>
        public static bool operator ==(XSize size1, XSize size2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return size1.Width == size2.Width && size1.Height == size2.Height;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Determines whether two size objects are not equal.
        /// </summary>
        public static bool operator !=(XSize size1, XSize size2)
        {
            return !(size1 == size2);
        }

        /// <summary>
        /// Indicates whether this two instance are equal.
        /// </summary>
        public static bool Equals(XSize size1, XSize size2)
        {
            if (size1.IsEmpty)
                return size2.IsEmpty;
            return size1.Width.Equals(size2.Width) && size1.Height.Equals(size2.Height);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        public override bool Equals(object o)
        {
            if (!(o is XSize))
                return false;
            return Equals(this, (XSize)o);
        }

        /// <summary>
        /// Indicates whether this instance and a specified size are equal.
        /// </summary>
        public bool Equals(XSize value)
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
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        /// <summary>
        /// Parses the size from a string.
        /// </summary>
        public static XSize Parse(string source)
        {
            XSize empty;
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            TokenizerHelper helper = new TokenizerHelper(source, cultureInfo);
            string str = helper.NextTokenRequired();
            if (str == "Empty")
                empty = Empty;
            else
                empty = new XSize(Convert.ToDouble(str, cultureInfo), Convert.ToDouble(helper.NextTokenRequired(), cultureInfo));
            helper.LastTokenRequired();
            return empty;
        }

#if GDI
        /// <summary>
        /// Converts this XSize to a PointF.
        /// </summary>
        public PointF ToPointF()
        {
            return new PointF((float)_width, (float)_height);
        }
#endif

        /// <summary>
        /// Converts this XSize to an XPoint.
        /// </summary>
        public XPoint ToXPoint()
        {
            return new XPoint(_width, _height);
        }

        /// <summary>
        /// Converts this XSize to an XVector.
        /// </summary>
        public XVector ToXVector()
        {
            return new XVector(_width, _height);
        }

#if GDI
        /// <summary>
        /// Converts this XSize to a SizeF.
        /// </summary>
        public SizeF ToSizeF()
        {
            return new SizeF((float)_width, (float)_height);
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Converts this XSize to a System.Windows.Size.
        /// </summary>
        public SysSize ToSize()
        {
            return new SysSize(_width, _height);
        }
#endif

#if GDI
        /// <summary>
        /// Creates an XSize from a System.Drawing.Size.
        /// </summary>
        public static XSize FromSize(System.Drawing.Size size)
        {
            return new XSize(size.Width, size.Height);
        }

        /// <summary>
        /// Implicit conversion from XSize to System.Drawing.Size. The conversion must be implicit because the
        /// WinForms designer uses it.
        /// </summary>
        public static implicit operator XSize(System.Drawing.Size size)
        {
            return new XSize(size.Width, size.Height);
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Creates an XSize from a System.Drawing.Size.
        /// </summary>
        public static XSize FromSize(SysSize size)
        {
            return new XSize(size.Width, size.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Creates an XSize from a System.Drawing.Size.
        /// </summary>
        public static XSize FromSizeF(SizeF size)
        {
            return new XSize(size.Width, size.Height);
        }
#endif

        /// <summary>
        /// Converts this XSize to a human readable string.
        /// </summary>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        /// <summary>
        /// Converts this XSize to a human readable string.
        /// </summary>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(null, provider);
        }

        /// <summary>
        /// Converts this XSize to a human readable string.
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
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { numericListSeparator, _width, _height });
            // ReSharper restore FormatStringProblem
        }

        /// <summary>
        /// Returns an empty size, i.e. a size with a width or height less than 0.
        /// </summary>
        public static XSize Empty
        {
            get { return s_empty; }
        }
        static readonly XSize s_empty;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return _width < 0; }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public double Width
        {
            get { return _width; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("CannotModifyEmptySize"); //SR.Get(SRID.Size_CannotModifyEmptySize, new object[0]));
                if (value < 0)
                    throw new ArgumentException("WidthCannotBeNegative"); //SR.Get(SRID.Size_WidthCannotBeNegative, new object[0]));
                _width = value;
            }
        }
        double _width;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public double Height
        {
            get { return _height; }
            set
            {
                if (IsEmpty)
                    throw new InvalidOperationException("CannotModifyEmptySize"); // SR.Get(SRID.Size_CannotModifyEmptySize, new object[0]));
                if (value < 0)
                    throw new ArgumentException("HeightCannotBeNegative"); //SR.Get(SRID.Size_HeightCannotBeNegative, new object[0]));
                _height = value;
            }
        }
        double _height;

        /// <summary>
        /// Performs an explicit conversion from XSize to XVector.
        /// </summary>
        public static explicit operator XVector(XSize size)
        {
            return new XVector(size._width, size._height);
        }

        /// <summary>
        /// Performs an explicit conversion from XSize to XPoint.
        /// </summary>
        public static explicit operator XPoint(XSize size)
        {
            return new XPoint(size._width, size._height);
        }

#if WPF || NETFX_CORE
        /// <summary>
        /// Performs an explicit conversion from Size to XSize.
        /// </summary>
        public static explicit operator XSize(SysSize size)
        {
            return new XSize(size.Width, size.Height);
        }
#endif

        private static XSize CreateEmptySize()
        {
            XSize size = new XSize();
            size._width = double.NegativeInfinity;
            size._height = double.NegativeInfinity;
            return size;
        }

        static XSize()
        {
            s_empty = CreateEmptySize();
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
                return String.Format(CultureInfo.InvariantCulture,
                    "size=({2}{0:" + format + "}, {1:" + format + "})",
                    _width, _height, IsEmpty ? "Empty " : "");
            }
        }
    }
}
