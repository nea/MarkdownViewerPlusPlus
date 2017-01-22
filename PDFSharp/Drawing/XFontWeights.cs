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

// Not used in PDFsharp 1.x.

namespace PdfSharp.Drawing
{
    enum FontWeightValues
    {
        Thin = 100,
        ExtraLight = 200,
        Light = 300,
        Normal = 400,
        Medium = 500,
        SemiBold = 600,
        Bold = 700,
        ExtraBold = 800,
        Black = 900,
        ExtraBlack = 950,
    }

#if true_  // PDFSHARP20
    /// <summary>
    /// Defines a set of static predefined XFontWeight values.
    /// </summary>
    public static class XFontWeights
    {
        internal static bool FontWeightStringToKnownWeight(string s, IFormatProvider provider, ref XFontWeight fontWeight)
        {
            int num;
            switch (s.ToLower())
            {
                case "thin":
                    fontWeight = Thin;
                    return true;

                case "extralight":
                    fontWeight = ExtraLight;
                    return true;

                case "ultralight":
                    fontWeight = UltraLight;
                    return true;

                case "light":
                    fontWeight = Light;
                    return true;

                case "normal":
                    fontWeight = Normal;
                    return true;

                case "regular":
                    fontWeight = Regular;
                    return true;

                case "medium":
                    fontWeight = Medium;
                    return true;

                case "semibold":
                    fontWeight = SemiBold;
                    return true;

                case "demibold":
                    fontWeight = DemiBold;
                    return true;

                case "bold":
                    fontWeight = Bold;
                    return true;

                case "extrabold":
                    fontWeight = ExtraBold;
                    return true;

                case "ultrabold":
                    fontWeight = UltraBold;
                    return true;

                case "heavy":
                    fontWeight = Heavy;
                    return true;

                case "black":
                    fontWeight = Black;
                    return true;

                case "extrablack":
                    fontWeight = ExtraBlack;
                    return true;

                case "ultrablack":
                    fontWeight = UltraBlack;
                    return true;
            }

            if (Int32.TryParse(s, NumberStyles.Integer, provider, out num))
            {
                fontWeight = new XFontWeight(num);
                return true;
            }
            return false;
        }

        internal static bool FontWeightToString(int weight, out string convertedValue)
        {
            switch (weight)
            {
                case 100:
                    convertedValue = "Thin";
                    return true;

                case 200:
                    convertedValue = "ExtraLight";
                    return true;

                case 300:
                    convertedValue = "Light";
                    return true;

                case 400:
                    convertedValue = "Normal";
                    return true;

                case 500:
                    convertedValue = "Medium";
                    return true;

                case 600:
                    convertedValue = "SemiBold";
                    return true;

                case 700:
                    convertedValue = "Bold";
                    return true;

                case 800:
                    convertedValue = "ExtraBold";
                    return true;

                case 900:
                    convertedValue = "Black";
                    return true;

                case 950:
                    convertedValue = "ExtraBlack";
                    return true;
            }
            convertedValue = null;
            return false;
        }

        /// <summary>
        /// Specifies a "Thin" font weight.
        /// </summary>
        public static XFontWeight Thin
        {
            get { return new XFontWeight(100); }
        }

        /// <summary>
        /// Specifies a "ExtraLight" font weight.
        /// </summary>
        public static XFontWeight ExtraLight
        {
            get { return new XFontWeight(200); }
        }

        /// <summary>
        /// Specifies a "UltraLight" font weight.
        /// </summary>
        public static XFontWeight UltraLight
        {
            get { return new XFontWeight(200); }
        }

        /// <summary>
        /// Specifies a "Light" font weight.
        /// </summary>
        public static XFontWeight Light
        {
            get { return new XFontWeight(300); }
        }

        /// <summary>
        /// Specifies a "Normal" font weight.
        /// </summary>
        public static XFontWeight Normal
        {
            get { return new XFontWeight(400); }
        }

        /// <summary>
        /// Specifies a "Regular" font weight.
        /// </summary>
        public static XFontWeight Regular
        {
            get { return new XFontWeight(400); }
        }

        /// <summary>
        /// Specifies a "Medium" font weight.
        /// </summary>
        public static XFontWeight Medium
        {
            get { return new XFontWeight(500); }
        }

        /// <summary>
        /// Specifies a "SemiBold" font weight.
        /// </summary>
        public static XFontWeight SemiBold
        {
            get { return new XFontWeight(600); }
        }

        /// <summary>
        /// Specifies a "DemiBold" font weight.
        /// </summary>
        public static XFontWeight DemiBold
        {
            get { return new XFontWeight(600); }
        }

        /// <summary>
        /// Specifies a "Bold" font weight.
        /// </summary>
        public static XFontWeight Bold
        {
            get { return new XFontWeight(700); }
        }

        /// <summary>
        /// Specifies a "ExtraBold" font weight.
        /// </summary>
        public static XFontWeight ExtraBold
        {
            get { return new XFontWeight(800); }
        }

        /// <summary>
        /// Specifies a "UltraBold" font weight.
        /// </summary>
        public static XFontWeight UltraBold
        {
            get { return new XFontWeight(800); }
        }

        /// <summary>
        /// Specifies a "Heavy" font weight.
        /// </summary>
        public static XFontWeight Heavy
        {
            get { return new XFontWeight(900); }
        }

        /// <summary>
        /// Specifies a "Black" font weight.
        /// </summary>
        public static XFontWeight Black
        {
            get { return new XFontWeight(900); }
        }

        /// <summary>
        /// Specifies a "ExtraBlack" font weight.
        /// </summary>
        public static XFontWeight ExtraBlack
        {
            get { return new XFontWeight(950); }
        }

        /// <summary>
        /// Specifies a "UltraBlack" font weight.
        /// </summary>
        public static XFontWeight UltraBlack
        {
            get { return new XFontWeight(950); }
        }
    }
#endif
}