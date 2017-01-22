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
using System.Globalization;
using System.ComponentModel;
using System.Threading;
#if GDI
using System.Drawing;
#endif
#if WPF
using System.Windows.Media;
#endif

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Manages the localization of the color class.
    /// </summary>
    public class XColorResourceManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XColorResourceManager"/> class.
        /// </summary>
        public XColorResourceManager()
#if !NETFX_CORE && !UWP
            : this(Thread.CurrentThread.CurrentUICulture)
#else
            : this(CultureInfo.CurrentUICulture)
#endif
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XColorResourceManager"/> class.
        /// </summary>
        /// <param name="cultureInfo">The culture info.</param>
        public XColorResourceManager(CultureInfo cultureInfo)
        {
            _cultureInfo = cultureInfo;
        }

        readonly CultureInfo _cultureInfo;

#if DEBUG_
        static public void Test()
        {
            int kcc = XKnownColorTable.colorTable.Length;

            for (int idx = 0; idx < kcc; idx++)
            {
                uint argb = XKnownColorTable.colorTable[idx];
                ColorResourceInfo info = GetColorInfo((XKnownColor)idx);
                if ((int)info.KnownColor == -1)
                {
                    kcc.GetType();
                }
                else
                {
                    if (argb != info.Argb)
                    {
                        kcc.GetType();
                    }
                }
            }

            for (int idx = 0; idx < colorInfos.Length; idx++)
            {
                ColorResourceInfo c2 = colorInfos[idx];
                if (c2.Argb != c2.Color.Rgb)
                    c2.GetType();
            }
        }
#endif

        /// <summary>
        /// Gets a known color from an ARGB value. Throws an ArgumentException if the value is not a known color.
        /// </summary>
        public static XKnownColor GetKnownColor(uint argb)
        {
            XKnownColor knownColor = XKnownColorTable.GetKnownColor(argb);
            if ((int)knownColor == -1)
                throw new ArgumentException("The argument is not a known color", "argb");
            return knownColor;
        }

        /// <summary>
        /// Gets all known colors.
        /// </summary>
        /// <param name="includeTransparent">Indicates whether to include the color Transparent.</param>
        public static XKnownColor[] GetKnownColors(bool includeTransparent)
        {
            int count = colorInfos.Length;
            XKnownColor[] knownColor = new XKnownColor[count - (includeTransparent ? 0 : 1)];
            for (int idxIn = includeTransparent ? 0 : 1, idxOut = 0; idxIn < count; idxIn++, idxOut++)
                knownColor[idxOut] = colorInfos[idxIn].KnownColor;
            return knownColor;
        }

        /// <summary>
        /// Converts a known color to a localized color name.
        /// </summary>
        public string ToColorName(XKnownColor knownColor)
        {
            ColorResourceInfo colorInfo = GetColorInfo(knownColor);

            // Currently German only
            if (_cultureInfo.TwoLetterISOLanguageName == "de")
                return colorInfo.NameDE;

            return colorInfo.Name;
        }

        /// <summary>
        /// Converts a color to a localized color name or an ARGB value.
        /// </summary>
        public string ToColorName(XColor color)
        {
            string name;
            if (color.IsKnownColor)
                name = ToColorName(XKnownColorTable.GetKnownColor(color.Argb));
            else
                name = String.Format("{0}, {1}, {2}, {3}", (int)(255 * color.A), color.R, color.G, color.B);
            return name;
        }

        static ColorResourceInfo GetColorInfo(XKnownColor knownColor)
        {
            for (int idx = 0; idx < colorInfos.Length; idx++)
            {
                ColorResourceInfo colorInfo = colorInfos[idx];
                if (colorInfo.KnownColor == knownColor)
                    return colorInfo;
            }
            throw new InvalidEnumArgumentException("Enum is not an XKnownColor.");
        }

        // I found no official translation for the 140 pre-defined colors. Some folks made their own translations.
        // http://unnecessary.de/wuest/farbtab/farbtabelle-w.html
        // http://blog.patrickkempf.de/archives/2004/04/10/html-farben/
        // http://www.grafikwunder.de/Grafikecke/Farbtabelle/farbtabelle-006.php
        // Silke changed some German translations (women know more colors than men :-)
        internal static ColorResourceInfo[] colorInfos = new ColorResourceInfo[]
            {
              new ColorResourceInfo(XKnownColor.Transparent, XColors.Transparent, 0x00FFFFFF, "Transparent", "Transparent"),
              new ColorResourceInfo(XKnownColor.Black, XColors.Black, 0xFF000000, "Black", "Schwarz"),
              new ColorResourceInfo(XKnownColor.DarkSlateGray, XColors.DarkSlateGray, 0xFF8FBC8F, "Darkslategray", "Dunkles Schiefergrau"),
              new ColorResourceInfo(XKnownColor.SlateGray, XColors.SlateGray, 0xFF708090, "Slategray", "Schiefergrau"),
              new ColorResourceInfo(XKnownColor.LightSlateGray, XColors.LightSlateGray, 0xFF778899, "Lightslategray", "Helles Schiefergrau"),
              new ColorResourceInfo(XKnownColor.LightSteelBlue, XColors.LightSteelBlue, 0xFFB0C4DE, "Lightsteelblue", "Helles Stahlblau"),
              //new ColorResourceInfo(XKnownColor.DimGray, XColors.DimGray, 0xFF696969, "Dimgray", "Mattes Grau"),
              new ColorResourceInfo(XKnownColor.DimGray, XColors.DimGray, 0xFF696969, "Dimgray", "Gedecktes Grau"),
              new ColorResourceInfo(XKnownColor.Gray, XColors.Gray, 0xFF808080, "Gray", "Grau"),
              new ColorResourceInfo(XKnownColor.DarkGray, XColors.DarkGray, 0xFFA9A9A9, "Darkgray", "Dunkelgrau"),
              new ColorResourceInfo(XKnownColor.Silver, XColors.Silver, 0xFFC0C0C0, "Silver", "Silber"),
              //new ColorResourceInfo(XKnownColor.Gainsboro, XColors.Gainsboro, 0xFFDCDCDC, "Gainsboro", "Gainsboro"),
              new ColorResourceInfo(XKnownColor.Gainsboro, XColors.Gainsboro, 0xFFDCDCDC, "Gainsboro", "Helles Blaugrau"),
              //new ColorResourceInfo(XKnownColor.WhiteSmoke, XColors.WhiteSmoke, 0xFFF5F5F5, "Whitesmoke", "Rauchiges Weiß"),
              new ColorResourceInfo(XKnownColor.WhiteSmoke, XColors.WhiteSmoke, 0xFFF5F5F5, "Whitesmoke", "Rauchweiß"),
              //new ColorResourceInfo(XKnownColor.GhostWhite, XColors.GhostWhite, 0xFFF8F8FF, "Ghostwhite", "Geisterweiß"),
              new ColorResourceInfo(XKnownColor.GhostWhite, XColors.GhostWhite, 0xFFF8F8FF, "Ghostwhite", "Schattenweiß"),
              new ColorResourceInfo(XKnownColor.White, XColors.White, 0xFFFFFFFF, "White", "Weiß"),
              new ColorResourceInfo(XKnownColor.Snow, XColors.Snow, 0xFFFFFAFA, "Snow", "Schneeweiß"),
              new ColorResourceInfo(XKnownColor.Ivory, XColors.Ivory, 0xFFFFFFF0, "Ivory", "Elfenbein"),
              new ColorResourceInfo(XKnownColor.FloralWhite, XColors.FloralWhite, 0xFFFFFAF0, "Floralwhite", "Blütenweiß"),
              new ColorResourceInfo(XKnownColor.SeaShell, XColors.SeaShell, 0xFFFFF5EE, "Seashell", "Muschel"),
              //new ColorResourceInfo(XKnownColor.OldLace, XColors.OldLace, 0xFFFDF5E6, "Oldlace", "Altgold"),
              new ColorResourceInfo(XKnownColor.OldLace, XColors.OldLace, 0xFFFDF5E6, "Oldlace", "Altweiß"),
              //new ColorResourceInfo(XKnownColor.Linen, XColors.Linen, 0xFFFAF0E6, "Linen", "Leinenfarbe"),
              new ColorResourceInfo(XKnownColor.Linen, XColors.Linen, 0xFFFAF0E6, "Linen", "Leinen"),
              new ColorResourceInfo(XKnownColor.AntiqueWhite, XColors.AntiqueWhite, 0xFFFAEBD7, "Antiquewhite", "Antikes Weiß"),
              new ColorResourceInfo(XKnownColor.BlanchedAlmond, XColors.BlanchedAlmond, 0xFFFFEBCD, "Blanchedalmond", "Mandelweiß"),
              //new ColorResourceInfo(XKnownColor.PapayaWhip, XColors.PapayaWhip, 0xFFFFEFD5, "Papayawhip", "Cremiges Papaya"),
              new ColorResourceInfo(XKnownColor.PapayaWhip, XColors.PapayaWhip, 0xFFFFEFD5, "Papayawhip", "Papayacreme"),
              new ColorResourceInfo(XKnownColor.Beige, XColors.Beige, 0xFFF5F5DC, "Beige", "Beige"),
              new ColorResourceInfo(XKnownColor.Cornsilk, XColors.Cornsilk, 0xFFFFF8DC, "Cornsilk", "Mais"),
              //new ColorResourceInfo(XKnownColor.LightGoldenrodYellow, XColors.LightGoldenrodYellow, 0xFFFAFAD2, "Lightgoldenrodyellow", "Helles Goldrutengelb"),
              new ColorResourceInfo(XKnownColor.LightGoldenrodYellow, XColors.LightGoldenrodYellow, 0xFFFAFAD2, "Lightgoldenrodyellow", "Helles Goldgelb"),
              new ColorResourceInfo(XKnownColor.LightYellow, XColors.LightYellow, 0xFFFFFFE0, "Lightyellow", "Hellgelb"),
              new ColorResourceInfo(XKnownColor.LemonChiffon, XColors.LemonChiffon, 0xFFFFFACD, "Lemonchiffon", "Pastellgelb"),
              //new ColorResourceInfo(XKnownColor.PaleGoldenrod, XColors.PaleGoldenrod, 0xFFEEE8AA, "Palegoldenrod", "Blasse Goldrutenfarbe"),
              new ColorResourceInfo(XKnownColor.PaleGoldenrod, XColors.PaleGoldenrod, 0xFFEEE8AA, "Palegoldenrod", "Blasses Goldgelb"),
              new ColorResourceInfo(XKnownColor.Khaki, XColors.Khaki, 0xFFF0E68C, "Khaki", "Khaki"),
              new ColorResourceInfo(XKnownColor.Yellow, XColors.Yellow, 0xFFFFFF00, "Yellow", "Gelb"),
              new ColorResourceInfo(XKnownColor.Gold, XColors.Gold, 0xFFFFD700, "Gold", "Gold"),
              new ColorResourceInfo(XKnownColor.Orange, XColors.Orange, 0xFFFFA500, "Orange", "Orange"),
              new ColorResourceInfo(XKnownColor.DarkOrange, XColors.DarkOrange, 0xFFFF8C00, "Darkorange", "Dunkles Orange"),
              //new ColorResourceInfo(XKnownColor.Goldenrod, XColors.Goldenrod, 0xFFDAA520, "Goldenrod", "Goldrute"),
              new ColorResourceInfo(XKnownColor.Goldenrod, XColors.Goldenrod, 0xFFDAA520, "Goldenrod", "Goldgelb"),
              //new ColorResourceInfo(XKnownColor.DarkGoldenrod, XColors.DarkGoldenrod, 0xFFB8860B, "Darkgoldenrod", "Dunkle Goldrutenfarbe"),
              new ColorResourceInfo(XKnownColor.DarkGoldenrod, XColors.DarkGoldenrod, 0xFFB8860B, "Darkgoldenrod", "Dunkles Goldgelb"),
              new ColorResourceInfo(XKnownColor.Peru, XColors.Peru, 0xFFCD853F, "Peru", "Peru"),
              new ColorResourceInfo(XKnownColor.Chocolate, XColors.Chocolate, 0xFFD2691E, "Chocolate", "Schokolade"),
              new ColorResourceInfo(XKnownColor.SaddleBrown, XColors.SaddleBrown, 0xFF8B4513, "Saddlebrown", "Sattelbraun"),
              new ColorResourceInfo(XKnownColor.Sienna, XColors.Sienna, 0xFFA0522D, "Sienna", "Ocker"),
              new ColorResourceInfo(XKnownColor.Brown, XColors.Brown, 0xFFA52A2A, "Brown", "Braun"),
              new ColorResourceInfo(XKnownColor.DarkRed, XColors.DarkRed, 0xFF8B0000, "Darkred", "Dunkelrot"),
              new ColorResourceInfo(XKnownColor.Maroon, XColors.Maroon, 0xFF800000, "Maroon", "Kastanienbraun"),
              new ColorResourceInfo(XKnownColor.PaleTurquoise, XColors.PaleTurquoise, 0xFFAFEEEE, "Paleturquoise", "Blasses Türkis"),
              //new ColorResourceInfo(XKnownColor.Firebrick, XColors.Firebrick, 0xFFB22222, "Firebrick", "Ziegelfarbe"),
              new ColorResourceInfo(XKnownColor.Firebrick, XColors.Firebrick, 0xFFB22222, "Firebrick", "Ziegel"),
              new ColorResourceInfo(XKnownColor.IndianRed, XColors.IndianRed, 0xFFCD5C5C, "Indianred", "Indischrot"),
              new ColorResourceInfo(XKnownColor.Crimson, XColors.Crimson, 0xFFDC143C, "Crimson", "Karmesinrot"),
              new ColorResourceInfo(XKnownColor.Red, XColors.Red, 0xFFFF0000, "Red", "Rot"),
              //new ColorResourceInfo(XKnownColor.OrangeRed, XColors.OrangeRed, 0xFFFF4500, "Orangered", "Orangenrot"),
              new ColorResourceInfo(XKnownColor.OrangeRed, XColors.OrangeRed, 0xFFFF4500, "Orangered", "Orangerot"),
              //new ColorResourceInfo(XKnownColor.Tomato, XColors.Tomato, 0xFFFF6347, "Tomato", "Tomatenrot"),
              new ColorResourceInfo(XKnownColor.Tomato, XColors.Tomato, 0xFFFF6347, "Tomato", "Tomate"),
              new ColorResourceInfo(XKnownColor.Coral, XColors.Coral, 0xFFFF7F50, "Coral", "Koralle"),
              new ColorResourceInfo(XKnownColor.Salmon, XColors.Salmon, 0xFFFA8072, "Salmon", "Lachs"),
              new ColorResourceInfo(XKnownColor.LightCoral, XColors.LightCoral, 0xFFF08080, "Lightcoral", "Helles Korallenrot"),
              //new ColorResourceInfo(XKnownColor.DarkSalmon, XColors.DarkSalmon, 0xFFE9967A, "Darksalmon", "Dunkle Lachsfarbe"),
              new ColorResourceInfo(XKnownColor.DarkSalmon, XColors.DarkSalmon, 0xFFE9967A, "Darksalmon", "Dunkles Lachs"),
              //new ColorResourceInfo(XKnownColor.LightSalmon, XColors.LightSalmon, 0xFFFFA07A, "Lightsalmon", "Helle Lachsfarbe"),
              new ColorResourceInfo(XKnownColor.LightSalmon, XColors.LightSalmon, 0xFFFFA07A, "Lightsalmon", "Helles Lachs"),
              new ColorResourceInfo(XKnownColor.SandyBrown, XColors.SandyBrown, 0xFFF4A460, "Sandybrown", "Sandbraun"),
              //new ColorResourceInfo(XKnownColor.RosyBrown, XColors.RosyBrown, 0xFFBC8F8F, "Rosybrown", "Rosiges Braun"),
              new ColorResourceInfo(XKnownColor.RosyBrown, XColors.RosyBrown, 0xFFBC8F8F, "Rosybrown", "Rotbraun"),
              new ColorResourceInfo(XKnownColor.Tan, XColors.Tan, 0xFFD2B48C, "Tan", "Gelbbraun"),
              //new ColorResourceInfo(XKnownColor.BurlyWood, XColors.BurlyWood, 0xFFDEB887, "Burlywood", "Grobes Braun"),
              new ColorResourceInfo(XKnownColor.BurlyWood, XColors.BurlyWood, 0xFFDEB887, "Burlywood", "Kräftiges Sandbraun"),
              new ColorResourceInfo(XKnownColor.Wheat, XColors.Wheat, 0xFFF5DEB3, "Wheat", "Weizen"),
              new ColorResourceInfo(XKnownColor.PeachPuff, XColors.PeachPuff, 0xFFFFDAB9, "Peachpuff", "Pfirsich"),
              //new ColorResourceInfo(XKnownColor.NavajoWhite, XColors.NavajoWhite, 0xFFFFDEAD, "Navajowhite", "Navajoweiß"),
              new ColorResourceInfo(XKnownColor.NavajoWhite, XColors.NavajoWhite, 0xFFFFDEAD, "Navajowhite", "Orangeweiß"),
              //new ColorResourceInfo(XKnownColor.Bisque, XColors.Bisque, 0xFFFFE4C4, "Bisque", "Tomatencreme"),
              new ColorResourceInfo(XKnownColor.Bisque, XColors.Bisque, 0xFFFFE4C4, "Bisque", "Blasses Rotbraun"),
              //new ColorResourceInfo(XKnownColor.Moccasin, XColors.Moccasin, 0xFFFFE4B5, "Moccasin", "Moccasin"),
              new ColorResourceInfo(XKnownColor.Moccasin, XColors.Moccasin, 0xFFFFE4B5, "Moccasin", "Mokassin"),
              //new ColorResourceInfo(XKnownColor.LavenderBlush, XColors.LavenderBlush, 0xFFFFF0F5, "Lavenderblush", "Rosige Lavenderfarbe"),
              new ColorResourceInfo(XKnownColor.LavenderBlush, XColors.LavenderBlush, 0xFFFFF0F5, "Lavenderblush", "Roter Lavendel"),
              new ColorResourceInfo(XKnownColor.MistyRose, XColors.MistyRose, 0xFFFFE4E1, "Mistyrose", "Altrosa"),
              new ColorResourceInfo(XKnownColor.Pink, XColors.Pink, 0xFFFFC0CB, "Pink", "Rosa"),
              new ColorResourceInfo(XKnownColor.LightPink, XColors.LightPink, 0xFFFFB6C1, "Lightpink", "Hellrosa"),
              new ColorResourceInfo(XKnownColor.HotPink, XColors.HotPink, 0xFFFF69B4, "Hotpink", "Leuchtendes Rosa"),
              //// XKnownColor.Fuchsia removed because the same as XKnownColor.Magenta
              ////new ColorResourceInfo(XKnownColor.Fuchsia, XColors.Fuchsia, 0xFFFF00FF, "Fuchsia", "Fuchsie"),
              new ColorResourceInfo(XKnownColor.Magenta, XColors.Magenta, 0xFFFF00FF, "Magenta", "Magentarot"),
              new ColorResourceInfo(XKnownColor.DeepPink, XColors.DeepPink, 0xFFFF1493, "Deeppink", "Tiefrosa"),
              new ColorResourceInfo(XKnownColor.MediumVioletRed, XColors.MediumVioletRed, 0xFFC71585, "Mediumvioletred", "Mittleres Violettrot"),
              new ColorResourceInfo(XKnownColor.PaleVioletRed, XColors.PaleVioletRed, 0xFFDB7093, "Palevioletred", "Blasses Violettrot"),
              new ColorResourceInfo(XKnownColor.Plum, XColors.Plum, 0xFFDDA0DD, "Plum", "Pflaume"),
              new ColorResourceInfo(XKnownColor.Thistle, XColors.Thistle, 0xFFD8BFD8, "Thistle", "Distel"),
              //new ColorResourceInfo(XKnownColor.Lavender, XColors.Lavender, 0xFFE6E6FA, "Lavender", "Lavendelfarbe"),
              new ColorResourceInfo(XKnownColor.Lavender, XColors.Lavender, 0xFFE6E6FA, "Lavender", "Lavendel"),
              new ColorResourceInfo(XKnownColor.Violet, XColors.Violet, 0xFFEE82EE, "Violet", "Violett"),
              new ColorResourceInfo(XKnownColor.Orchid, XColors.Orchid, 0xFFDA70D6, "Orchid", "Orchidee"),
              new ColorResourceInfo(XKnownColor.DarkMagenta, XColors.DarkMagenta, 0xFF8B008B, "Darkmagenta", "Dunkles Magentarot"),
              new ColorResourceInfo(XKnownColor.Purple, XColors.Purple, 0xFF800080, "Purple", "Violett"),
              new ColorResourceInfo(XKnownColor.Indigo, XColors.Indigo, 0xFF4B0082, "Indigo", "Indigo"),
              new ColorResourceInfo(XKnownColor.BlueViolet, XColors.BlueViolet, 0xFF8A2BE2, "Blueviolet", "Blauviolett"),
              new ColorResourceInfo(XKnownColor.DarkViolet, XColors.DarkViolet, 0xFF9400D3, "Darkviolet", "Dunkles Violett"),
              //new ColorResourceInfo(XKnownColor.DarkOrchid, XColors.DarkOrchid, 0xFF9932CC, "Darkorchid", "Dunkle Orchideenfarbe"),
              new ColorResourceInfo(XKnownColor.DarkOrchid, XColors.DarkOrchid, 0xFF9932CC, "Darkorchid", "Dunkle Orchidee"),
              new ColorResourceInfo(XKnownColor.MediumPurple, XColors.MediumPurple, 0xFF9370DB, "Mediumpurple", "Mittleres Violett"),
              //new ColorResourceInfo(XKnownColor.MediumOrchid, XColors.MediumOrchid, 0xFFBA55D3, "Mediumorchid", "Mittlere Orchideenfarbe"),
              new ColorResourceInfo(XKnownColor.MediumOrchid, XColors.MediumOrchid, 0xFFBA55D3, "Mediumorchid", "Mittlere Orchidee"),
              new ColorResourceInfo(XKnownColor.MediumSlateBlue, XColors.MediumSlateBlue, 0xFF7B68EE, "Mediumslateblue", "Mittleres Schieferblau"),
              new ColorResourceInfo(XKnownColor.SlateBlue, XColors.SlateBlue, 0xFF6A5ACD, "Slateblue", "Schieferblau"),
              new ColorResourceInfo(XKnownColor.DarkSlateBlue, XColors.DarkSlateBlue, 0xFF483D8B, "Darkslateblue", "Dunkles Schiefergrau"),
              new ColorResourceInfo(XKnownColor.MidnightBlue, XColors.MidnightBlue, 0xFF191970, "Midnightblue", "Mitternachtsblau"),
              new ColorResourceInfo(XKnownColor.Navy, XColors.Navy, 0xFF000080, "Navy", "Marineblau"),
              new ColorResourceInfo(XKnownColor.DarkBlue, XColors.DarkBlue, 0xFF00008B, "Darkblue", "Dunkelblau"),
              new ColorResourceInfo(XKnownColor.LightGray, XColors.LightGray, 0xFFD3D3D3, "Lightgray", "Hellgrau"),
              new ColorResourceInfo(XKnownColor.MediumBlue, XColors.MediumBlue, 0xFF0000CD, "Mediumblue", "Mittelblau"),
              new ColorResourceInfo(XKnownColor.Blue, XColors.Blue, 0xFF0000FF, "Blue", "Blau"),
              new ColorResourceInfo(XKnownColor.RoyalBlue, XColors.RoyalBlue, 0xFF4169E1, "Royalblue", "Königsblau"),
              new ColorResourceInfo(XKnownColor.SteelBlue, XColors.SteelBlue, 0xFF4682B4, "Steelblue", "Stahlblau"),
              new ColorResourceInfo(XKnownColor.CornflowerBlue, XColors.CornflowerBlue, 0xFF6495ED, "Cornflowerblue", "Kornblumenblau"),
              new ColorResourceInfo(XKnownColor.DodgerBlue, XColors.DodgerBlue, 0xFF1E90FF, "Dodgerblue", "Dodger-Blau"),
              new ColorResourceInfo(XKnownColor.DeepSkyBlue, XColors.DeepSkyBlue, 0xFF00BFFF, "Deepskyblue", "Tiefes Himmelblau"),
              new ColorResourceInfo(XKnownColor.LightSkyBlue, XColors.LightSkyBlue, 0xFF87CEFA, "Lightskyblue", "Helles Himmelblau"),
              new ColorResourceInfo(XKnownColor.SkyBlue, XColors.SkyBlue, 0xFF87CEEB, "Skyblue", "Himmelblau"),
              new ColorResourceInfo(XKnownColor.LightBlue, XColors.LightBlue, 0xFFADD8E6, "Lightblue", "Hellblau"),
              //// XKnownColor.Aqua removed because the same as XKnownColor.Cyan
              ////new ColorResourceInfo(XKnownColor.Aqua, XColors.Aqua, 0xFF00FFFF, "Aqua", "Blaugrün"),
              new ColorResourceInfo(XKnownColor.Cyan, XColors.Cyan, 0xFF00FFFF, "Cyan", "Zyan"),
              new ColorResourceInfo(XKnownColor.PowderBlue, XColors.PowderBlue, 0xFFB0E0E6, "Powderblue", "Taubenblau"),
              new ColorResourceInfo(XKnownColor.LightCyan, XColors.LightCyan, 0xFFE0FFFF, "Lightcyan", "Helles Cyanblau"),
              new ColorResourceInfo(XKnownColor.AliceBlue, XColors.AliceBlue, 0xFFA0CE00, "Aliceblue", "Aliceblau"),
              new ColorResourceInfo(XKnownColor.Azure, XColors.Azure, 0xFFF0FFFF, "Azure", "Himmelblau"),
              //new ColorResourceInfo(XKnownColor.MintCream, XColors.MintCream, 0xFFF5FFFA, "Mintcream", "Cremige Pfefferminzfarbe"),
              new ColorResourceInfo(XKnownColor.MintCream, XColors.MintCream, 0xFFF5FFFA, "Mintcream", "Helles Pfefferminzgrün"),
              new ColorResourceInfo(XKnownColor.Honeydew, XColors.Honeydew, 0xFFF0FFF0, "Honeydew", "Honigmelone"),
              new ColorResourceInfo(XKnownColor.Aquamarine, XColors.Aquamarine, 0xFF7FFFD4, "Aquamarine", "Aquamarinblau"),
              new ColorResourceInfo(XKnownColor.Turquoise, XColors.Turquoise, 0xFF40E0D0, "Turquoise", "Türkis"),
              new ColorResourceInfo(XKnownColor.MediumTurquoise, XColors.MediumTurquoise, 0xFF48D1CC, "Mediumturqoise", "Mittleres Türkis"),
              new ColorResourceInfo(XKnownColor.DarkTurquoise, XColors.DarkTurquoise, 0xFF00CED1, "Darkturquoise", "Dunkles Türkis"),
              new ColorResourceInfo(XKnownColor.MediumAquamarine, XColors.MediumAquamarine, 0xFF66CDAA, "Mediumaquamarine", "Mittleres Aquamarinblau"),
              new ColorResourceInfo(XKnownColor.LightSeaGreen, XColors.LightSeaGreen, 0xFF20B2AA, "Lightseagreen", "Helles Seegrün"),
              new ColorResourceInfo(XKnownColor.DarkCyan, XColors.DarkCyan, 0xFF008B8B, "Darkcyan", "Dunkles Zyanblau"),
              //new ColorResourceInfo(XKnownColor.Teal, XColors.Teal, 0xFF008080, "Teal", "Entenbraun"),
              new ColorResourceInfo(XKnownColor.Teal, XColors.Teal, 0xFF008080, "Teal", "Entenblau"),
              new ColorResourceInfo(XKnownColor.CadetBlue, XColors.CadetBlue, 0xFF5F9EA0, "Cadetblue", "Kadettblau"),
              new ColorResourceInfo(XKnownColor.MediumSeaGreen, XColors.MediumSeaGreen, 0xFF3CB371, "Mediumseagreen", "Mittleres Seegrün"),
              new ColorResourceInfo(XKnownColor.DarkSeaGreen, XColors.DarkSeaGreen, 0xFF8FBC8F, "Darkseagreen", "Dunkles Seegrün"),
              new ColorResourceInfo(XKnownColor.LightGreen, XColors.LightGreen, 0xFF90EE90, "Lightgreen", "Hellgrün"),
              new ColorResourceInfo(XKnownColor.PaleGreen, XColors.PaleGreen, 0xFF98FB98, "Palegreen", "Blassgrün"),
              new ColorResourceInfo(XKnownColor.MediumSpringGreen, XColors.MediumSpringGreen, 0xFF00FA9A, "Mediumspringgreen", "Mittleres Frühlingsgrün"),
              new ColorResourceInfo(XKnownColor.SpringGreen, XColors.SpringGreen, 0xFF00FF7F, "Springgreen", "Frühlingsgrün"),
              new ColorResourceInfo(XKnownColor.Lime, XColors.Lime, 0xFF00FF00, "Lime", "Zitronengrün"),
              new ColorResourceInfo(XKnownColor.LimeGreen, XColors.LimeGreen, 0xFF32CD32, "Limegreen", "Gelbgrün"),
              new ColorResourceInfo(XKnownColor.SeaGreen, XColors.SeaGreen, 0xFF2E8B57, "Seagreen", "Seegrün"),
              new ColorResourceInfo(XKnownColor.ForestGreen, XColors.ForestGreen, 0xFF228B22, "Forestgreen", "Waldgrün"),
              new ColorResourceInfo(XKnownColor.Green, XColors.Green, 0xFF008000, "Green", "Grün"),
              new ColorResourceInfo(XKnownColor.LawnGreen, XColors.LawnGreen, 0xFF008000, "LawnGreen", "Grasgrün"),
              new ColorResourceInfo(XKnownColor.DarkGreen, XColors.DarkGreen, 0xFF006400, "Darkgreen", "Dunkelgrün"),
              //new ColorResourceInfo(XKnownColor.OliveDrab, XColors.OliveDrab, 0xFF6B8E23, "Olivedrab", "Olivfarbiges Graubraun"),
              new ColorResourceInfo(XKnownColor.OliveDrab, XColors.OliveDrab, 0xFF6B8E23, "Olivedrab", "Reife Olive"),
              new ColorResourceInfo(XKnownColor.DarkOliveGreen, XColors.DarkOliveGreen, 0xFF556B2F, "Darkolivegreen", "Dunkles Olivgrün"),
              new ColorResourceInfo(XKnownColor.Olive, XColors.Olive, 0xFF808000, "Olive", "Olivgrün"),
              new ColorResourceInfo(XKnownColor.DarkKhaki, XColors.DarkKhaki, 0xFFBDB76B, "Darkkhaki", "Dunkles Khaki"),
              new ColorResourceInfo(XKnownColor.YellowGreen, XColors.YellowGreen, 0xFF9ACD32, "Yellowgreen", "Gelbgrün"),
              new ColorResourceInfo(XKnownColor.Chartreuse, XColors.Chartreuse, 0xFF7FFF00, "Chartreuse", "Hellgrün"),
              new ColorResourceInfo(XKnownColor.GreenYellow, XColors.GreenYellow, 0xFFADFF2F, "Greenyellow", "Grüngelb"),
            };

        internal struct ColorResourceInfo
        {
            public ColorResourceInfo(XKnownColor knownColor, XColor color, uint argb, string name, string nameDE)
            {
                KnownColor = knownColor;
                Color = color;
                Argb = argb;
                Name = name;
                NameDE = nameDE;
            }
            public XKnownColor KnownColor;
            public XColor Color;
            public uint Argb;
            public string Name;
            // ReSharper disable once InconsistentNaming
            public string NameDE;
        }
    }
}