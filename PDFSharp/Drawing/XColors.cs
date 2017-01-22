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
    ///<summary>
    /// Represents a set of 141 pre-defined RGB colors. Incidentally the values are the same
    /// as in System.Drawing.Color.
    /// </summary>
    public static class XColors
    {
        ///<summary>Gets a predefined color.</summary>
        public static XColor AliceBlue { get { return new XColor(XKnownColor.AliceBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor AntiqueWhite { get { return new XColor(XKnownColor.AntiqueWhite); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Aqua { get { return new XColor(XKnownColor.Aqua); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Aquamarine { get { return new XColor(XKnownColor.Aquamarine); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Azure { get { return new XColor(XKnownColor.Azure); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Beige { get { return new XColor(XKnownColor.Beige); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Bisque { get { return new XColor(XKnownColor.Bisque); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Black { get { return new XColor(XKnownColor.Black); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor BlanchedAlmond { get { return new XColor(XKnownColor.BlanchedAlmond); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Blue { get { return new XColor(XKnownColor.Blue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor BlueViolet { get { return new XColor(XKnownColor.BlueViolet); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Brown { get { return new XColor(XKnownColor.Brown); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor BurlyWood { get { return new XColor(XKnownColor.BurlyWood); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor CadetBlue { get { return new XColor(XKnownColor.CadetBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Chartreuse { get { return new XColor(XKnownColor.Chartreuse); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Chocolate { get { return new XColor(XKnownColor.Chocolate); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Coral { get { return new XColor(XKnownColor.Coral); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor CornflowerBlue { get { return new XColor(XKnownColor.CornflowerBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Cornsilk { get { return new XColor(XKnownColor.Cornsilk); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Crimson { get { return new XColor(XKnownColor.Crimson); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Cyan { get { return new XColor(XKnownColor.Cyan); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkBlue { get { return new XColor(XKnownColor.DarkBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkCyan { get { return new XColor(XKnownColor.DarkCyan); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkGoldenrod { get { return new XColor(XKnownColor.DarkGoldenrod); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkGray { get { return new XColor(XKnownColor.DarkGray); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkGreen { get { return new XColor(XKnownColor.DarkGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkKhaki { get { return new XColor(XKnownColor.DarkKhaki); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkMagenta { get { return new XColor(XKnownColor.DarkMagenta); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkOliveGreen { get { return new XColor(XKnownColor.DarkOliveGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkOrange { get { return new XColor(XKnownColor.DarkOrange); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkOrchid { get { return new XColor(XKnownColor.DarkOrchid); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkRed { get { return new XColor(XKnownColor.DarkRed); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkSalmon { get { return new XColor(XKnownColor.DarkSalmon); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkSeaGreen { get { return new XColor(XKnownColor.DarkSeaGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkSlateBlue { get { return new XColor(XKnownColor.DarkSlateBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkSlateGray { get { return new XColor(XKnownColor.DarkSlateGray); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkTurquoise { get { return new XColor(XKnownColor.DarkTurquoise); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DarkViolet { get { return new XColor(XKnownColor.DarkViolet); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DeepPink { get { return new XColor(XKnownColor.DeepPink); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DeepSkyBlue { get { return new XColor(XKnownColor.DeepSkyBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DimGray { get { return new XColor(XKnownColor.DimGray); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor DodgerBlue { get { return new XColor(XKnownColor.DodgerBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Firebrick { get { return new XColor(XKnownColor.Firebrick); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor FloralWhite { get { return new XColor(XKnownColor.FloralWhite); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor ForestGreen { get { return new XColor(XKnownColor.ForestGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Fuchsia { get { return new XColor(XKnownColor.Fuchsia); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Gainsboro { get { return new XColor(XKnownColor.Gainsboro); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor GhostWhite { get { return new XColor(XKnownColor.GhostWhite); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Gold { get { return new XColor(XKnownColor.Gold); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Goldenrod { get { return new XColor(XKnownColor.Goldenrod); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Gray { get { return new XColor(XKnownColor.Gray); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Green { get { return new XColor(XKnownColor.Green); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor GreenYellow { get { return new XColor(XKnownColor.GreenYellow); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Honeydew { get { return new XColor(XKnownColor.Honeydew); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor HotPink { get { return new XColor(XKnownColor.HotPink); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor IndianRed { get { return new XColor(XKnownColor.IndianRed); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Indigo { get { return new XColor(XKnownColor.Indigo); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Ivory { get { return new XColor(XKnownColor.Ivory); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Khaki { get { return new XColor(XKnownColor.Khaki); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Lavender { get { return new XColor(XKnownColor.Lavender); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LavenderBlush { get { return new XColor(XKnownColor.LavenderBlush); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LawnGreen { get { return new XColor(XKnownColor.LawnGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LemonChiffon { get { return new XColor(XKnownColor.LemonChiffon); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightBlue { get { return new XColor(XKnownColor.LightBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightCoral { get { return new XColor(XKnownColor.LightCoral); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightCyan { get { return new XColor(XKnownColor.LightCyan); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightGoldenrodYellow { get { return new XColor(XKnownColor.LightGoldenrodYellow); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightGray { get { return new XColor(XKnownColor.LightGray); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightGreen { get { return new XColor(XKnownColor.LightGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightPink { get { return new XColor(XKnownColor.LightPink); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightSalmon { get { return new XColor(XKnownColor.LightSalmon); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightSeaGreen { get { return new XColor(XKnownColor.LightSeaGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightSkyBlue { get { return new XColor(XKnownColor.LightSkyBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightSlateGray { get { return new XColor(XKnownColor.LightSlateGray); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightSteelBlue { get { return new XColor(XKnownColor.LightSteelBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LightYellow { get { return new XColor(XKnownColor.LightYellow); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Lime { get { return new XColor(XKnownColor.Lime); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor LimeGreen { get { return new XColor(XKnownColor.LimeGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Linen { get { return new XColor(XKnownColor.Linen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Magenta { get { return new XColor(XKnownColor.Magenta); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Maroon { get { return new XColor(XKnownColor.Maroon); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumAquamarine { get { return new XColor(XKnownColor.MediumAquamarine); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumBlue { get { return new XColor(XKnownColor.MediumBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumOrchid { get { return new XColor(XKnownColor.MediumOrchid); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumPurple { get { return new XColor(XKnownColor.MediumPurple); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumSeaGreen { get { return new XColor(XKnownColor.MediumSeaGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumSlateBlue { get { return new XColor(XKnownColor.MediumSlateBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumSpringGreen { get { return new XColor(XKnownColor.MediumSpringGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumTurquoise { get { return new XColor(XKnownColor.MediumTurquoise); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MediumVioletRed { get { return new XColor(XKnownColor.MediumVioletRed); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MidnightBlue { get { return new XColor(XKnownColor.MidnightBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MintCream { get { return new XColor(XKnownColor.MintCream); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor MistyRose { get { return new XColor(XKnownColor.MistyRose); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Moccasin { get { return new XColor(XKnownColor.Moccasin); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor NavajoWhite { get { return new XColor(XKnownColor.NavajoWhite); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Navy { get { return new XColor(XKnownColor.Navy); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor OldLace { get { return new XColor(XKnownColor.OldLace); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Olive { get { return new XColor(XKnownColor.Olive); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor OliveDrab { get { return new XColor(XKnownColor.OliveDrab); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Orange { get { return new XColor(XKnownColor.Orange); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor OrangeRed { get { return new XColor(XKnownColor.OrangeRed); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Orchid { get { return new XColor(XKnownColor.Orchid); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor PaleGoldenrod { get { return new XColor(XKnownColor.PaleGoldenrod); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor PaleGreen { get { return new XColor(XKnownColor.PaleGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor PaleTurquoise { get { return new XColor(XKnownColor.PaleTurquoise); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor PaleVioletRed { get { return new XColor(XKnownColor.PaleVioletRed); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor PapayaWhip { get { return new XColor(XKnownColor.PapayaWhip); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor PeachPuff { get { return new XColor(XKnownColor.PeachPuff); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Peru { get { return new XColor(XKnownColor.Peru); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Pink { get { return new XColor(XKnownColor.Pink); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Plum { get { return new XColor(XKnownColor.Plum); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor PowderBlue { get { return new XColor(XKnownColor.PowderBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Purple { get { return new XColor(XKnownColor.Purple); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Red { get { return new XColor(XKnownColor.Red); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor RosyBrown { get { return new XColor(XKnownColor.RosyBrown); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor RoyalBlue { get { return new XColor(XKnownColor.RoyalBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SaddleBrown { get { return new XColor(XKnownColor.SaddleBrown); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Salmon { get { return new XColor(XKnownColor.Salmon); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SandyBrown { get { return new XColor(XKnownColor.SandyBrown); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SeaGreen { get { return new XColor(XKnownColor.SeaGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SeaShell { get { return new XColor(XKnownColor.SeaShell); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Sienna { get { return new XColor(XKnownColor.Sienna); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Silver { get { return new XColor(XKnownColor.Silver); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SkyBlue { get { return new XColor(XKnownColor.SkyBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SlateBlue { get { return new XColor(XKnownColor.SlateBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SlateGray { get { return new XColor(XKnownColor.SlateGray); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Snow { get { return new XColor(XKnownColor.Snow); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SpringGreen { get { return new XColor(XKnownColor.SpringGreen); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor SteelBlue { get { return new XColor(XKnownColor.SteelBlue); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Tan { get { return new XColor(XKnownColor.Tan); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Teal { get { return new XColor(XKnownColor.Teal); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Thistle { get { return new XColor(XKnownColor.Thistle); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Tomato { get { return new XColor(XKnownColor.Tomato); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Transparent { get { return new XColor(XKnownColor.Transparent); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Turquoise { get { return new XColor(XKnownColor.Turquoise); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Violet { get { return new XColor(XKnownColor.Violet); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Wheat { get { return new XColor(XKnownColor.Wheat); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor White { get { return new XColor(XKnownColor.White); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor WhiteSmoke { get { return new XColor(XKnownColor.WhiteSmoke); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor Yellow { get { return new XColor(XKnownColor.Yellow); } }

        ///<summary>Gets a predefined color.</summary>
        public static XColor YellowGreen { get { return new XColor(XKnownColor.YellowGreen); } }
    }
}
