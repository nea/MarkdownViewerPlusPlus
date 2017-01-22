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
#endif
#if WPF
using System.Windows.Media;
#endif

namespace PdfSharp.Drawing
{
    internal class XKnownColorTable
    {
        internal static uint[] ColorTable;

        public static uint KnownColorToArgb(XKnownColor color)
        {
            if (ColorTable == null)
                InitColorTable();
            if (color <= XKnownColor.YellowGreen)
                return ColorTable[(int)color];
            return 0;
        }

        public static bool IsKnownColor(uint argb)
        {
            for (int idx = 0; idx < ColorTable.Length; idx++)
            {
                if (ColorTable[idx] == argb)
                    return true;
            }
            return false;
        }

        public static XKnownColor GetKnownColor(uint argb)
        {
            for (int idx = 0; idx < ColorTable.Length; idx++)
            {
                if (ColorTable[idx] == argb)
                    return (XKnownColor)idx;
            }
            return (XKnownColor)(-1);
        }

        private static void InitColorTable()
        {
            // Same values as in GDI+ and System.Windows.Media.XColors
            // Note that Magenta is the same as Fuchsia and Zyan is the same as Aqua.
            uint[] colors = new uint[141];
            colors[0] = 0xFFF0F8FF;  // AliceBlue
            colors[1] = 0xFFFAEBD7;  // AntiqueWhite
            colors[2] = 0xFF00FFFF;  // Aqua
            colors[3] = 0xFF7FFFD4;  // Aquamarine
            colors[4] = 0xFFF0FFFF;  // Azure
            colors[5] = 0xFFF5F5DC;  // Beige
            colors[6] = 0xFFFFE4C4;  // Bisque
            colors[7] = 0xFF000000;  // Black
            colors[8] = 0xFFFFEBCD;  // BlanchedAlmond
            colors[9] = 0xFF0000FF;  // Blue
            colors[10] = 0xFF8A2BE2;  // BlueViolet
            colors[11] = 0xFFA52A2A;  // Brown
            colors[12] = 0xFFDEB887;  // BurlyWood
            colors[13] = 0xFF5F9EA0;  // CadetBlue
            colors[14] = 0xFF7FFF00;  // Chartreuse
            colors[15] = 0xFFD2691E;  // Chocolate
            colors[16] = 0xFFFF7F50;  // Coral
            colors[17] = 0xFF6495ED;  // CornflowerBlue
            colors[18] = 0xFFFFF8DC;  // Cornsilk
            colors[19] = 0xFFDC143C;  // Crimson
            colors[20] = 0xFF00FFFF;  // Cyan
            colors[21] = 0xFF00008B;  // DarkBlue
            colors[22] = 0xFF008B8B;  // DarkCyan
            colors[23] = 0xFFB8860B;  // DarkGoldenrod
            colors[24] = 0xFFA9A9A9;  // DarkGray
            colors[25] = 0xFF006400;  // DarkGreen
            colors[26] = 0xFFBDB76B;  // DarkKhaki
            colors[27] = 0xFF8B008B;  // DarkMagenta
            colors[28] = 0xFF556B2F;  // DarkOliveGreen
            colors[29] = 0xFFFF8C00;  // DarkOrange
            colors[30] = 0xFF9932CC;  // DarkOrchid
            colors[31] = 0xFF8B0000;  // DarkRed
            colors[32] = 0xFFE9967A;  // DarkSalmon
            colors[33] = 0xFF8FBC8B;  // DarkSeaGreen
            colors[34] = 0xFF483D8B;  // DarkSlateBlue
            colors[35] = 0xFF2F4F4F;  // DarkSlateGray
            colors[36] = 0xFF00CED1;  // DarkTurquoise
            colors[37] = 0xFF9400D3;  // DarkViolet
            colors[38] = 0xFFFF1493;  // DeepPink
            colors[39] = 0xFF00BFFF;  // DeepSkyBlue
            colors[40] = 0xFF696969;  // DimGray
            colors[41] = 0xFF1E90FF;  // DodgerBlue
            colors[42] = 0xFFB22222;  // Firebrick
            colors[43] = 0xFFFFFAF0;  // FloralWhite
            colors[44] = 0xFF228B22;  // ForestGreen
            colors[45] = 0xFFFF00FF;  // Fuchsia
            colors[46] = 0xFFDCDCDC;  // Gainsboro
            colors[47] = 0xFFF8F8FF;  // GhostWhite
            colors[48] = 0xFFFFD700;  // Gold
            colors[49] = 0xFFDAA520;  // Goldenrod
            colors[50] = 0xFF808080;  // Gray
            colors[51] = 0xFF008000;  // Green
            colors[52] = 0xFFADFF2F;  // GreenYellow
            colors[53] = 0xFFF0FFF0;  // Honeydew
            colors[54] = 0xFFFF69B4;  // HotPink
            colors[55] = 0xFFCD5C5C;  // IndianRed
            colors[56] = 0xFF4B0082;  // Indigo
            colors[57] = 0xFFFFFFF0;  // Ivory
            colors[58] = 0xFFF0E68C;  // Khaki
            colors[59] = 0xFFE6E6FA;  // Lavender
            colors[60] = 0xFFFFF0F5;  // LavenderBlush
            colors[61] = 0xFF7CFC00;  // LawnGreen
            colors[62] = 0xFFFFFACD;  // LemonChiffon
            colors[63] = 0xFFADD8E6;  // LightBlue
            colors[64] = 0xFFF08080;  // LightCoral
            colors[65] = 0xFFE0FFFF;  // LightCyan
            colors[66] = 0xFFFAFAD2;  // LightGoldenrodYellow
            colors[67] = 0xFFD3D3D3;  // LightGray
            colors[68] = 0xFF90EE90;  // LightGreen
            colors[69] = 0xFFFFB6C1;  // LightPink
            colors[70] = 0xFFFFA07A;  // LightSalmon
            colors[71] = 0xFF20B2AA;  // LightSeaGreen
            colors[72] = 0xFF87CEFA;  // LightSkyBlue
            colors[73] = 0xFF778899;  // LightSlateGray
            colors[74] = 0xFFB0C4DE;  // LightSteelBlue
            colors[75] = 0xFFFFFFE0;  // LightYellow
            colors[76] = 0xFF00FF00;  // Lime
            colors[77] = 0xFF32CD32;  // LimeGreen
            colors[78] = 0xFFFAF0E6;  // Linen
            colors[79] = 0xFFFF00FF;  // Magenta
            colors[80] = 0xFF800000;  // Maroon
            colors[81] = 0xFF66CDAA;  // MediumAquamarine
            colors[82] = 0xFF0000CD;  // MediumBlue
            colors[83] = 0xFFBA55D3;  // MediumOrchid
            colors[84] = 0xFF9370DB;  // MediumPurple
            colors[85] = 0xFF3CB371;  // MediumSeaGreen
            colors[86] = 0xFF7B68EE;  // MediumSlateBlue
            colors[87] = 0xFF00FA9A;  // MediumSpringGreen
            colors[88] = 0xFF48D1CC;  // MediumTurquoise
            colors[89] = 0xFFC71585;  // MediumVioletRed
            colors[90] = 0xFF191970;  // MidnightBlue
            colors[91] = 0xFFF5FFFA;  // MintCream
            colors[92] = 0xFFFFE4E1;  // MistyRose
            colors[93] = 0xFFFFE4B5;  // Moccasin
            colors[94] = 0xFFFFDEAD;  // NavajoWhite
            colors[95] = 0xFF000080;  // Navy
            colors[96] = 0xFFFDF5E6;  // OldLace
            colors[97] = 0xFF808000;  // Olive
            colors[98] = 0xFF6B8E23;  // OliveDrab
            colors[99] = 0xFFFFA500;  // Orange
            colors[100] = 0xFFFF4500;  // OrangeRed
            colors[101] = 0xFFDA70D6;  // Orchid
            colors[102] = 0xFFEEE8AA;  // PaleGoldenrod
            colors[103] = 0xFF98FB98;  // PaleGreen
            colors[104] = 0xFFAFEEEE;  // PaleTurquoise
            colors[105] = 0xFFDB7093;  // PaleVioletRed
            colors[106] = 0xFFFFEFD5;  // PapayaWhip
            colors[107] = 0xFFFFDAB9;  // PeachPuff
            colors[108] = 0xFFCD853F;  // Peru
            colors[109] = 0xFFFFC0CB;  // Pink
            colors[110] = 0xFFDDA0DD;  // Plum
            colors[111] = 0xFFB0E0E6;  // PowderBlue
            colors[112] = 0xFF800080;  // Purple
            colors[113] = 0xFFFF0000;  // Red
            colors[114] = 0xFFBC8F8F;  // RosyBrown
            colors[115] = 0xFF4169E1;  // RoyalBlue
            colors[116] = 0xFF8B4513;  // SaddleBrown
            colors[117] = 0xFFFA8072;  // Salmon
            colors[118] = 0xFFF4A460;  // SandyBrown
            colors[119] = 0xFF2E8B57;  // SeaGreen
            colors[120] = 0xFFFFF5EE;  // SeaShell
            colors[121] = 0xFFA0522D;  // Sienna
            colors[122] = 0xFFC0C0C0;  // Silver
            colors[123] = 0xFF87CEEB;  // SkyBlue
            colors[124] = 0xFF6A5ACD;  // SlateBlue
            colors[125] = 0xFF708090;  // SlateGray
            colors[126] = 0xFFFFFAFA;  // Snow
            colors[127] = 0xFF00FF7F;  // SpringGreen
            colors[128] = 0xFF4682B4;  // SteelBlue
            colors[129] = 0xFFD2B48C;  // Tan
            colors[130] = 0xFF008080;  // Teal
            colors[131] = 0xFFD8BFD8;  // Thistle
            colors[132] = 0xFFFF6347;  // Tomato
            colors[133] = 0x00FFFFFF;  // Transparent
            colors[134] = 0xFF40E0D0;  // Turquoise
            colors[135] = 0xFFEE82EE;  // Violet
            colors[136] = 0xFFF5DEB3;  // Wheat
            colors[137] = 0xFFFFFFFF;  // White
            colors[138] = 0xFFF5F5F5;  // WhiteSmoke
            colors[139] = 0xFFFFFF00;  // Yellow
            colors[140] = 0xFF9ACD32;  // YellowGreen

            ColorTable = colors;
        }
    }
}
