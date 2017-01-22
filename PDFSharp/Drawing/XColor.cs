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
using System.ComponentModel;
#if GDI
using System.Drawing;
#endif
#if WPF
using WpfColor = System.Windows.Media.Color;
#endif
#if UWP
using UwpColor = Windows.UI.Color;
#endif


// ReSharper disable RedundantNameQualifier

namespace PdfSharp.Drawing
{
    ///<summary>
    /// Represents a RGB, CMYK, or gray scale color.
    /// </summary>
    [DebuggerDisplay("clr=(A={A}, R={R}, G={G}, B={B} C={C}, M={M}, Y={Y}, K={K})")]
    public struct XColor
    {
        XColor(uint argb)
        {
            _cs = XColorSpace.Rgb;
            _a = (byte)((argb >> 24) & 0xff) / 255f;
            _r = (byte)((argb >> 16) & 0xff);
            _g = (byte)((argb >> 8) & 0xff);
            _b = (byte)(argb & 0xff);
            _c = 0;
            _m = 0;
            _y = 0;
            _k = 0;
            _gs = 0;
            RgbChanged();
            //_cs.GetType(); // Suppress warning
        }

        XColor(byte alpha, byte red, byte green, byte blue)
        {
            _cs = XColorSpace.Rgb;
            _a = alpha / 255f;
            _r = red;
            _g = green;
            _b = blue;
            _c = 0;
            _m = 0;
            _y = 0;
            _k = 0;
            _gs = 0;
            RgbChanged();
            //_cs.GetType(); // Suppress warning
        }

        XColor(double alpha, double cyan, double magenta, double yellow, double black)
        {
            _cs = XColorSpace.Cmyk;
            _a = (float)(alpha > 1 ? 1 : (alpha < 0 ? 0 : alpha));
            _c = (float)(cyan > 1 ? 1 : (cyan < 0 ? 0 : cyan));
            _m = (float)(magenta > 1 ? 1 : (magenta < 0 ? 0 : magenta));
            _y = (float)(yellow > 1 ? 1 : (yellow < 0 ? 0 : yellow));
            _k = (float)(black > 1 ? 1 : (black < 0 ? 0 : black));
            _r = 0;
            _g = 0;
            _b = 0;
            _gs = 0f;
            CmykChanged();
        }

        XColor(double cyan, double magenta, double yellow, double black)
            : this(1.0, cyan, magenta, yellow, black)
        { }

        XColor(double gray)
        {
            _cs = XColorSpace.GrayScale;
            if (gray < 0)
                _gs = 0;
            else if (gray > 1)
                _gs = 1;
            _gs = (float)gray;

            _a = 1;
            _r = 0;
            _g = 0;
            _b = 0;
            _c = 0;
            _m = 0;
            _y = 0;
            _k = 0;
            GrayChanged();
        }

#if GDI
        XColor(System.Drawing.Color color)
            : this(color.A, color.R, color.G, color.B)
        { }
#endif

#if WPF
        XColor(WpfColor color)
            : this(color.A, color.R, color.G, color.B)
        { }
#endif

#if GDI
        XColor(KnownColor knownColor)
            : this(System.Drawing.Color.FromKnownColor(knownColor))
        { }
#endif

#if UWP
        XColor(UwpColor color)
            : this(color.A, color.R, color.G, color.B)
        { }
#endif

        internal XColor(XKnownColor knownColor)
            : this(XKnownColorTable.KnownColorToArgb(knownColor))
        { }

        /// <summary>
        /// Creates an XColor structure from a 32-bit ARGB value.
        /// </summary>
        public static XColor FromArgb(int argb)
        {
            return new XColor((byte)(argb >> 24), (byte)(argb >> 16), (byte)(argb >> 8), (byte)(argb));
        }

        /// <summary>
        /// Creates an XColor structure from a 32-bit ARGB value.
        /// </summary>
        public static XColor FromArgb(uint argb)
        {
            return new XColor((byte)(argb >> 24), (byte)(argb >> 16), (byte)(argb >> 8), (byte)(argb));
        }

        // from System.Drawing.Color
        //public static XColor FromArgb(int alpha, Color baseColor);
        //public static XColor FromArgb(int red, int green, int blue);
        //public static XColor FromArgb(int alpha, int red, int green, int blue);
        //public static XColor FromKnownColor(KnownColor color);
        //public static XColor FromName(string name);

        /// <summary>
        /// Creates an XColor structure from the specified 8-bit color values (red, green, and blue).
        /// The alpha value is implicitly 255 (fully opaque).
        /// </summary>
        public static XColor FromArgb(int red, int green, int blue)
        {
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new XColor(255, (byte)red, (byte)green, (byte)blue);
        }

        /// <summary>
        /// Creates an XColor structure from the four ARGB component (alpha, red, green, and blue) values.
        /// </summary>
        public static XColor FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha, "alpha");
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new XColor((byte)alpha, (byte)red, (byte)green, (byte)blue);
        }

#if GDI
        /// <summary>
        /// Creates an XColor structure from the specified System.Drawing.Color.
        /// </summary>
        public static XColor FromArgb(System.Drawing.Color color)
        {
            return new XColor(color);
        }
#endif

#if WPF
        /// <summary>
        /// Creates an XColor structure from the specified System.Drawing.Color.
        /// </summary>
        public static XColor FromArgb(WpfColor color)
        {
            return new XColor(color);
        }
#endif

#if UWP
        /// <summary>
        /// Creates an XColor structure from the specified Windows.UI.Color.
        /// </summary>
        public static XColor FromArgb(UwpColor color)
        {
            return new XColor(color);
        }
#endif

        /// <summary>
        /// Creates an XColor structure from the specified alpha value and color.
        /// </summary>
        public static XColor FromArgb(int alpha, XColor color)
        {
            color.A = ((byte)alpha) / 255.0;
            return color;
        }

#if GDI
        /// <summary>
        /// Creates an XColor structure from the specified alpha value and color.
        /// </summary>
        public static XColor FromArgb(int alpha, System.Drawing.Color color)
        {
            // Cast required to use correct constructor.
            return new XColor((byte)alpha, color.R, color.G, color.B);
        }
#endif

#if WPF
        /// <summary>
        /// Creates an XColor structure from the specified alpha value and color.
        /// </summary>
        public static XColor FromArgb(int alpha, WpfColor color)
        {
            // Cast required to use correct constructor.
            return new XColor((byte)alpha, color.R, color.G, color.B);
        }
#endif

#if UWP
        /// <summary>
        /// Creates an XColor structure from the specified alpha value and color.
        /// </summary>
        public static XColor FromArgb(int alpha, UwpColor color)
        {
            // Cast required to use correct constructor.
            return new XColor((byte)alpha, color.R, color.G, color.B);
        }
#endif

        /// <summary>
        /// Creates an XColor structure from the specified CMYK values.
        /// </summary>
        public static XColor FromCmyk(double cyan, double magenta, double yellow, double black)
        {
            return new XColor(cyan, magenta, yellow, black);
        }

        /// <summary>
        /// Creates an XColor structure from the specified CMYK values.
        /// </summary>
        public static XColor FromCmyk(double alpha, double cyan, double magenta, double yellow, double black)
        {
            return new XColor(alpha, cyan, magenta, yellow, black);
        }

        /// <summary>
        /// Creates an XColor structure from the specified gray value.
        /// </summary>
        public static XColor FromGrayScale(double grayScale)
        {
            return new XColor(grayScale);
        }

        /// <summary>
        /// Creates an XColor from the specified pre-defined color.
        /// </summary>
        public static XColor FromKnownColor(XKnownColor color)
        {
            return new XColor(color);
        }

#if GDI
        /// <summary>
        /// Creates an XColor from the specified pre-defined color.
        /// </summary>
        public static XColor FromKnownColor(KnownColor color)
        {
            return new XColor(color);
        }
#endif

        /// <summary>
        /// Creates an XColor from the specified name of a pre-defined color.
        /// </summary>
        public static XColor FromName(string name)
        {
#if GDI
            // The implementation in System.Drawing.dll is interesting. It uses a ColorConverter
            // with hash tables, locking mechanisms etc. I'm not sure what problems that solves.
            // So I don't use the source, but the reflection.
            try
            {
                return new XColor((KnownColor)Enum.Parse(typeof(KnownColor), name, true));
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch { }
            // ReSharper restore EmptyGeneralCatchClause
#endif
            return Empty;
        }

        /// <summary>
        /// Gets or sets the color space to be used for PDF generation.
        /// </summary>
        public XColorSpace ColorSpace
        {
            get { return _cs; }
            set
            {
                if (!Enum.IsDefined(typeof(XColorSpace), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(XColorSpace));
                _cs = value;
            }
        }

        /// <summary>
        /// Indicates whether this XColor structure is uninitialized.
        /// </summary>
        public bool IsEmpty
        {
            get { return this == Empty; }
        }

#if GDI
#if UseGdiObjects
        /// <summary>
        /// Implicit conversion from Color to XColor
        /// </summary>
        public static implicit operator XColor(Color color)
        {
            return new XColor(color);
        }
#endif

        ///<summary>
        /// Creates a System.Drawing.Color object from this color.
        /// </summary>
        public System.Drawing.Color ToGdiColor()
        {
            return System.Drawing.Color.FromArgb((int)(_a * 255), _r, _g, _b);
        }
#endif

#if WPF
        ///<summary>
        /// Creates a WpfColor object from this color.
        /// </summary>
        public WpfColor ToWpfColor()
        {
            return WpfColor.FromArgb((byte)(_a * 255), _r, _g, _b);
        }
#endif

#if UWP
        ///<summary>
        /// Creates a Windows.UI.Color object from this color.
        /// </summary>
        public UwpColor ToUwpColor()
        {
            return UwpColor.FromArgb((byte)(_a * 255), _r, _g, _b);
        }
#endif

        /// <summary>
        /// Determines whether the specified object is a Color structure and is equivalent to this 
        /// Color structure.
        /// </summary>
        public override bool Equals(object obj)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (obj is XColor)
            {
                XColor color = (XColor)obj;
                if (_r == color._r && _g == color._g && _b == color._b &&
                  _c == color._c && _m == color._m && _y == color._y && _k == color._k &&
                  _gs == color._gs)
                {
                    return _a == color._a;
                }
            }
            return false;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyFieldInGetHashCode
            return ((byte)(_a * 255)) ^ _r ^ _g ^ _b;
            // ReSharper restore NonReadonlyFieldInGetHashCode
        }

        /// <summary>
        /// Determines whether two colors are equal.
        /// </summary>
        public static bool operator ==(XColor left, XColor right)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (left._r == right._r && left._g == right._g && left._b == right._b &&
                left._c == right._c && left._m == right._m && left._y == right._y && left._k == right._k &&
                left._gs == right._gs)
            {
                return left._a == right._a;
            }
            return false;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Determines whether two colors are not equal.
        /// </summary>
        public static bool operator !=(XColor left, XColor right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets a value indicating whether this color is a known color.
        /// </summary>
        public bool IsKnownColor
        {
            get { return XKnownColorTable.IsKnownColor(Argb); }
        }

        /// <summary>
        /// Gets the hue-saturation-brightness (HSB) hue value, in degrees, for this color.
        /// </summary>
        /// <returns>The hue, in degrees, of this color. The hue is measured in degrees, ranging from 0 through 360, in HSB color space.</returns>
        public double GetHue()
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if ((_r == _g) && (_g == _b))
                return 0;

            double value1 = _r / 255.0;
            double value2 = _g / 255.0;
            double value3 = _b / 255.0;
            double value7 = 0;
            double value4 = value1;
            double value5 = value1;
            if (value2 > value4)
                value4 = value2;

            if (value3 > value4)
                value4 = value3;

            if (value2 < value5)
                value5 = value2;

            if (value3 < value5)
                value5 = value3;

            double value6 = value4 - value5;
            if (value1 == value4)
                value7 = (value2 - value3) / value6;
            else if (value2 == value4)
                value7 = 2f + ((value3 - value1) / value6);
            else if (value3 == value4)
                value7 = 4f + ((value1 - value2) / value6);

            value7 *= 60;
            if (value7 < 0)
                value7 += 360;
            return value7;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Gets the hue-saturation-brightness (HSB) saturation value for this color.
        /// </summary>
        /// <returns>The saturation of this color. The saturation ranges from 0 through 1, where 0 is grayscale and 1 is the most saturated.</returns>
        public double GetSaturation()
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            double value1 = _r / 255.0;
            double value2 = _g / 255.0;
            double value3 = _b / 255.0;
            double value7 = 0;
            double value4 = value1;
            double value5 = value1;
            if (value2 > value4)
                value4 = value2;

            if (value3 > value4)
                value4 = value3;

            if (value2 < value5)
                value5 = value2;

            if (value3 < value5)
                value5 = value3;

            if (value4 == value5)
                return value7;

            double value6 = (value4 + value5) / 2;
            if (value6 <= 0.5)
                return (value4 - value5) / (value4 + value5);
            return (value4 - value5) / ((2f - value4) - value5);
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Gets the hue-saturation-brightness (HSB) brightness value for this color.
        /// </summary>
        /// <returns>The brightness of this color. The brightness ranges from 0 through 1, where 0 represents black and 1 represents white.</returns>
        public double GetBrightness()
        {
            double value1 = _r / 255.0;
            double value2 = _g / 255.0;
            double value3 = _b / 255.0;
            double value4 = value1;
            double value5 = value1;
            if (value2 > value4)
                value4 = value2;

            if (value3 > value4)
                value4 = value3;

            if (value2 < value5)
                value5 = value2;

            if (value3 < value5)
                value5 = value3;

            return (value4 + value5) / 2;
        }

        ///<summary>
        /// One of the RGB values changed; recalculate other color representations.
        /// </summary>
        void RgbChanged()
        {
            // ReSharper disable LocalVariableHidesMember
            _cs = XColorSpace.Rgb;
            int c = 255 - _r;
            int m = 255 - _g;
            int y = 255 - _b;
            int k = Math.Min(c, Math.Min(m, y));
            if (k == 255)
                _c = _m = _y = 0;
            else
            {
                float black = 255f - k;
                _c = (c - k) / black;
                _m = (m - k) / black;
                _y = (y - k) / black;
            }
            _k = _gs = k / 255f;
            // ReSharper restore LocalVariableHidesMember
        }

        ///<summary>
        /// One of the CMYK values changed; recalculate other color representations.
        /// </summary>
        void CmykChanged()
        {
            _cs = XColorSpace.Cmyk;
            float black = _k * 255;
            float factor = 255f - black;
            _r = (byte)(255 - Math.Min(255f, _c * factor + black));
            _g = (byte)(255 - Math.Min(255f, _m * factor + black));
            _b = (byte)(255 - Math.Min(255f, _y * factor + black));
            _gs = (float)(1 - Math.Min(1.0, 0.3f * _c + 0.59f * _m + 0.11 * _y + _k));
        }

        ///<summary>
        /// The gray scale value changed; recalculate other color representations.
        /// </summary>
        void GrayChanged()
        {
            _cs = XColorSpace.GrayScale;
            _r = (byte)(_gs * 255);
            _g = (byte)(_gs * 255);
            _b = (byte)(_gs * 255);
            _c = 0;
            _m = 0;
            _y = 0;
            _k = 1 - _gs;
        }

        // Properties

        /// <summary>
        /// Gets or sets the alpha value the specifies the transparency. 
        /// The value is in the range from 1 (opaque) to 0 (completely transparent).
        /// </summary>
        public double A
        {
            get { return _a; }
            set
            {
                if (value < 0)
                    _a = 0;
                else if (value > 1)
                    _a = 1;
                else
                    _a = (float)value;
            }
        }

        /// <summary>
        /// Gets or sets the red value.
        /// </summary>
        public byte R
        {
            get { return _r; }
            set { _r = value; RgbChanged(); }
        }

        /// <summary>
        /// Gets or sets the green value.
        /// </summary>
        public byte G
        {
            get { return _g; }
            set { _g = value; RgbChanged(); }
        }

        /// <summary>
        /// Gets or sets the blue value.
        /// </summary>
        public byte B
        {
            get { return _b; }
            set { _b = value; RgbChanged(); }
        }

        /// <summary>
        /// Gets the RGB part value of the color. Internal helper function.
        /// </summary>
        internal uint Rgb
        {
            get { return ((uint)_r << 16) | ((uint)_g << 8) | _b; }
        }

        /// <summary>
        /// Gets the ARGB part value of the color. Internal helper function.
        /// </summary>
        internal uint Argb
        {
            get { return ((uint)(_a * 255) << 24) | ((uint)_r << 16) | ((uint)_g << 8) | _b; }
        }

        /// <summary>
        /// Gets or sets the cyan value.
        /// </summary>
        public double C
        {
            get { return _c; }
            set
            {
                if (value < 0)
                    _c = 0;
                else if (value > 1)
                    _c = 1;
                else
                    _c = (float)value;
                CmykChanged();
            }
        }

        /// <summary>
        /// Gets or sets the magenta value.
        /// </summary>
        public double M
        {
            get { return _m; }
            set
            {
                if (value < 0)
                    _m = 0;
                else if (value > 1)
                    _m = 1;
                else
                    _m = (float)value;
                CmykChanged();
            }
        }

        /// <summary>
        /// Gets or sets the yellow value.
        /// </summary>
        public double Y
        {
            get { return _y; }
            set
            {
                if (value < 0)
                    _y = 0;
                else if (value > 1)
                    _y = 1;
                else
                    _y = (float)value;
                CmykChanged();
            }
        }

        /// <summary>
        /// Gets or sets the black (or key) value.
        /// </summary>
        public double K
        {
            get { return _k; }
            set
            {
                if (value < 0)
                    _k = 0;
                else if (value > 1)
                    _k = 1;
                else
                    _k = (float)value;
                CmykChanged();
            }
        }

        /// <summary>
        /// Gets or sets the gray scale value.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public double GS
        // ReSharper restore InconsistentNaming
        {
            get { return _gs; }
            set
            {
                if (value < 0)
                    _gs = 0;
                else if (value > 1)
                    _gs = 1;
                else
                    _gs = (float)value;
                GrayChanged();
            }
        }

        /// <summary>
        /// Represents the null color.
        /// </summary>
        public static XColor Empty;

        ///<summary>
        /// Special property for XmlSerializer only.
        /// </summary>
        public string RgbCmykG
        {
            get
            {
                return String.Format(CultureInfo.InvariantCulture,
                  "{0};{1};{2};{3};{4};{5};{6};{7};{8}", _r, _g, _b, _c, _m, _y, _k, _gs, _a);
            }
            set
            {
                string[] values = value.Split(';');
                _r = byte.Parse(values[0], CultureInfo.InvariantCulture);
                _g = byte.Parse(values[1], CultureInfo.InvariantCulture);
                _b = byte.Parse(values[2], CultureInfo.InvariantCulture);
                _c = float.Parse(values[3], CultureInfo.InvariantCulture);
                _m = float.Parse(values[4], CultureInfo.InvariantCulture);
                _y = float.Parse(values[5], CultureInfo.InvariantCulture);
                _k = float.Parse(values[6], CultureInfo.InvariantCulture);
                _gs = float.Parse(values[7], CultureInfo.InvariantCulture);
                _a = float.Parse(values[8], CultureInfo.InvariantCulture);
            }
        }

        static void CheckByte(int val, string name)
        {
            if (val < 0 || val > 0xFF)
                throw new ArgumentException(PSSR.InvalidValue(val, name, 0, 255));
        }

        XColorSpace _cs;

        float _a;  // alpha

        byte _r;   // \
        byte _g;   // |--- RGB
        byte _b;   // /

        float _c;  // \
        float _m;  // |--- CMYK
        float _y;  // |
        float _k;  // /

        float _gs; // >--- gray scale
    }
}
