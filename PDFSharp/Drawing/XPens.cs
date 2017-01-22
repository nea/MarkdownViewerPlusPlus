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

// ReSharper disable UnusedMember.Global

#define USE_CACHE_is_not_thread_safe

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Pens for all the pre-defined colors.
    /// </summary>
    public static class XPens
    {
        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen AliceBlue
        {
#if USE_CACHE
            get { return _aliceBlue ?? (_aliceBlue = new XPen(XColors.AliceBlue, 1, true)); }
#else
            get { return new XPen(XColors.AliceBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen AntiqueWhite
        {
#if USE_CACHE
            get { return _antiqueWhite ?? (_antiqueWhite = new XPen(XColors.AntiqueWhite, 1, true)); }
#else
            get { return new XPen(XColors.AntiqueWhite, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Aqua
        {
#if USE_CACHE
            get { return _aqua ?? (_aqua = new XPen(XColors.Aqua, 1, true)); }
#else
            get { return new XPen(XColors.Aqua, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Aquamarine
        {
#if USE_CACHE
            get { return _aquamarine ?? (_aquamarine = new XPen(XColors.Aquamarine, 1, true)); }
#else
            get { return new XPen(XColors.Aquamarine, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Azure
        {
#if USE_CACHE
            get { return _azure ?? (_azure = new XPen(XColors.Azure, 1, true)); }
#else
            get { return new XPen(XColors.Azure, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Beige
        {
#if USE_CACHE
            get { return _beige ?? (_beige = new XPen(XColors.Beige, 1, true)); }
#else
            get { return new XPen(XColors.Beige, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Bisque
        {
#if USE_CACHE
            get { return _bisque ?? (_bisque = new XPen(XColors.Bisque, 1, true)); }
#else
            get { return new XPen(XColors.Bisque, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Black
        {
#if USE_CACHE
            get { return _black ?? (_black = new XPen(XColors.Black, 1, true)); }
#else
            get { return new XPen(XColors.Black, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen BlanchedAlmond
        {
#if USE_CACHE
            get { return _blanchedAlmond ?? (_blanchedAlmond = new XPen(XColors.BlanchedAlmond, 1, true)); }
#else
            get { return new XPen(XColors.BlanchedAlmond, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Blue
        {
#if USE_CACHE
            get { return _blue ?? (_blue = new XPen(XColors.Blue, 1, true)); }
#else
            get { return new XPen(XColors.Blue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen BlueViolet
        {
#if USE_CACHE
            get { return _blueViolet ?? (_blueViolet = new XPen(XColors.BlueViolet, 1, true)); }
#else
            get { return new XPen(XColors.BlueViolet, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Brown
        {
#if USE_CACHE
            get { return _brown ?? (_brown = new XPen(XColors.Brown, 1, true)); }
#else
            get { return new XPen(XColors.Brown, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen BurlyWood
        {
#if USE_CACHE
            get { return _burlyWood ?? (_burlyWood = new XPen(XColors.BurlyWood, 1, true)); }
#else
            get { return new XPen(XColors.BurlyWood, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen CadetBlue
        {
#if USE_CACHE
            get { return _cadetBlue ?? (_cadetBlue = new XPen(XColors.CadetBlue, 1, true)); }
#else
            get { return new XPen(XColors.CadetBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Chartreuse
        {
#if USE_CACHE
            get { return _chartreuse ?? (_chartreuse = new XPen(XColors.Chartreuse, 1, true)); }
#else
            get { return new XPen(XColors.Chartreuse, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Chocolate
        {
#if USE_CACHE
            get { return _chocolate ?? (_chocolate = new XPen(XColors.Chocolate, 1, true)); }
#else
            get { return new XPen(XColors.Chocolate, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Coral
        {
#if USE_CACHE
            get { return _coral ?? (_coral = new XPen(XColors.Coral, 1, true)); }
#else
            get { return new XPen(XColors.Coral, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen CornflowerBlue
        {
#if USE_CACHE
            get { return _cornflowerBlue ?? (_cornflowerBlue = new XPen(XColors.CornflowerBlue, 1, true)); }
#else
            get { return new XPen(XColors.CornflowerBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Cornsilk
        {
#if USE_CACHE
            get { return _cornsilk ?? (_cornsilk = new XPen(XColors.Cornsilk, 1, true)); }
#else
            get { return new XPen(XColors.Cornsilk, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Crimson
        {
#if USE_CACHE
            get { return _crimson ?? (_crimson = new XPen(XColors.Crimson, 1, true)); }
#else
            get { return new XPen(XColors.Crimson, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Cyan
        {
#if USE_CACHE
            get { return _cyan ?? (_cyan = new XPen(XColors.Cyan, 1, true)); }
#else
            get { return new XPen(XColors.Cyan, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkBlue
        {
#if USE_CACHE
            get { return _darkBlue ?? (_darkBlue = new XPen(XColors.DarkBlue, 1, true)); }
#else
            get { return new XPen(XColors.DarkBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkCyan
        {
#if USE_CACHE
            get { return _darkCyan ?? (_darkCyan = new XPen(XColors.DarkCyan, 1, true)); }
#else
            get { return new XPen(XColors.DarkCyan, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkGoldenrod
        {
#if USE_CACHE
            get { return _darkGoldenrod ?? (_darkGoldenrod = new XPen(XColors.DarkGoldenrod, 1, true)); }
#else
            get { return new XPen(XColors.DarkGoldenrod, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkGray
        {
#if USE_CACHE
            get { return _darkGray ?? (_darkGray = new XPen(XColors.DarkGray, 1, true)); }
#else
            get { return new XPen(XColors.DarkGray, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkGreen
        {
#if USE_CACHE
            get { return _darkGreen ?? (_darkGreen = new XPen(XColors.DarkGreen, 1, true)); }
#else
            get { return new XPen(XColors.DarkGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkKhaki
        {
#if USE_CACHE
            get { return _darkKhaki ?? (_darkKhaki = new XPen(XColors.DarkKhaki, 1, true)); }
#else
            get { return new XPen(XColors.DarkKhaki, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkMagenta
        {
#if USE_CACHE
            get { return _darkMagenta ?? (_darkMagenta = new XPen(XColors.DarkMagenta, 1, true)); }
#else
            get { return new XPen(XColors.DarkMagenta, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkOliveGreen
        {
#if USE_CACHE
            get { return _darkOliveGreen ?? (_darkOliveGreen = new XPen(XColors.DarkOliveGreen, 1, true)); }
#else
            get { return new XPen(XColors.DarkOliveGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkOrange
        {
#if USE_CACHE
            get { return _darkOrange ?? (_darkOrange = new XPen(XColors.DarkOrange, 1, true)); }
#else
            get { return new XPen(XColors.DarkOrange, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkOrchid
        {
#if USE_CACHE
            get { return _darkOrchid ?? (_darkOrchid = new XPen(XColors.DarkOrchid, 1, true)); }
#else
            get { return new XPen(XColors.DarkOrchid, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkRed
        {
#if USE_CACHE
            get { return _darkRed ?? (_darkRed = new XPen(XColors.DarkRed, 1, true)); }
#else
            get { return new XPen(XColors.DarkRed, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSalmon
        {
#if USE_CACHE
            get { return _darkSalmon ?? (_darkSalmon = new XPen(XColors.DarkSalmon, 1, true)); }
#else
            get { return new XPen(XColors.DarkSalmon, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSeaGreen
        {
#if USE_CACHE
            get { return _darkSeaGreen ?? (_darkSeaGreen = new XPen(XColors.DarkSeaGreen, 1, true)); }
#else
            get { return new XPen(XColors.DarkSeaGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSlateBlue
        {
#if USE_CACHE
            get { return _darkSlateBlue ?? (_darkSlateBlue = new XPen(XColors.DarkSlateBlue, 1, true)); }
#else
            get { return new XPen(XColors.DarkSlateBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkSlateGray
        {
#if USE_CACHE
            get { return _darkSlateGray ?? (_darkSlateGray = new XPen(XColors.DarkSlateGray, 1, true)); }
#else
            get { return new XPen(XColors.DarkSlateGray, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkTurquoise
        {
#if USE_CACHE
            get { return _darkTurquoise ?? (_darkTurquoise = new XPen(XColors.DarkTurquoise, 1, true)); }
#else
            get { return new XPen(XColors.DarkTurquoise, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DarkViolet
        {
#if USE_CACHE
            get { return _darkViolet ?? (_darkViolet = new XPen(XColors.DarkViolet, 1, true)); }
#else
            get { return new XPen(XColors.DarkViolet, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DeepPink
        {
#if USE_CACHE
            get { return _deepPink ?? (_deepPink = new XPen(XColors.DeepPink, 1, true)); }
#else
            get { return new XPen(XColors.DeepPink, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DeepSkyBlue
        {
#if USE_CACHE
            get { return _deepSkyBlue ?? (_deepSkyBlue = new XPen(XColors.DeepSkyBlue, 1, true)); }
#else
            get { return new XPen(XColors.DeepSkyBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DimGray
        {
#if USE_CACHE
            get { return _dimGray ?? (_dimGray = new XPen(XColors.DimGray, 1, true)); }
#else
            get { return new XPen(XColors.DimGray, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen DodgerBlue
        {
#if USE_CACHE
            get { return _dodgerBlue ?? (_dodgerBlue = new XPen(XColors.DodgerBlue, 1, true)); }
#else
            get { return new XPen(XColors.DodgerBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Firebrick
        {
#if USE_CACHE
            get { return _firebrick ?? (_firebrick = new XPen(XColors.Firebrick, 1, true)); }
#else
            get { return new XPen(XColors.Firebrick, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen FloralWhite
        {
#if USE_CACHE
            get { return _floralWhite ?? (_floralWhite = new XPen(XColors.FloralWhite, 1, true)); }
#else
            get { return new XPen(XColors.FloralWhite, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen ForestGreen
        {
#if USE_CACHE
            get { return _forestGreen ?? (_forestGreen = new XPen(XColors.ForestGreen, 1, true)); }
#else
            get { return new XPen(XColors.ForestGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Fuchsia
        {
#if USE_CACHE
            get { return _fuchsia ?? (_fuchsia = new XPen(XColors.Fuchsia, 1, true)); }
#else
            get { return new XPen(XColors.Fuchsia, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Gainsboro
        {
#if USE_CACHE
            get { return _gainsboro ?? (_gainsboro = new XPen(XColors.Gainsboro, 1, true)); }
#else
            get { return new XPen(XColors.Gainsboro, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen GhostWhite
        {
#if USE_CACHE
            get { return _ghostWhite ?? (_ghostWhite = new XPen(XColors.GhostWhite, 1, true)); }
#else
            get { return new XPen(XColors.GhostWhite, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Gold
        {
#if USE_CACHE
            get { return _gold ?? (_gold = new XPen(XColors.Gold, 1, true)); }
#else
            get { return new XPen(XColors.Gold, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Goldenrod
        {
#if USE_CACHE
            get { return _goldenrod ?? (_goldenrod = new XPen(XColors.Goldenrod, 1, true)); }
#else
            get { return new XPen(XColors.Goldenrod, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Gray
        {
#if USE_CACHE
            get { return _gray ?? (_gray = new XPen(XColors.Gray, 1, true)); }
#else
            get { return new XPen(XColors.Gray, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Green
        {
#if USE_CACHE
            get { return _green ?? (_green = new XPen(XColors.Green, 1, true)); }
#else
            get { return new XPen(XColors.Green, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen GreenYellow
        {
#if USE_CACHE
            get { return _greenYellow ?? (_greenYellow = new XPen(XColors.GreenYellow, 1, true)); }
#else
            get { return new XPen(XColors.GreenYellow, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Honeydew
        {
#if USE_CACHE
            get { return _honeydew ?? (_honeydew = new XPen(XColors.Honeydew, 1, true)); }
#else
            get { return new XPen(XColors.Honeydew, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen HotPink
        {
#if USE_CACHE
            get { return _hotPink ?? (_hotPink = new XPen(XColors.HotPink, 1, true)); }
#else
            get { return new XPen(XColors.HotPink, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen IndianRed
        {
#if USE_CACHE
            get { return _indianRed ?? (_indianRed = new XPen(XColors.IndianRed, 1, true)); }
#else
            get { return new XPen(XColors.IndianRed, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Indigo
        {
#if USE_CACHE
            get { return _indigo ?? (_indigo = new XPen(XColors.Indigo, 1, true)); }
#else
            get { return new XPen(XColors.Indigo, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Ivory
        {
#if USE_CACHE
            get { return _ivory ?? (_ivory = new XPen(XColors.Ivory, 1, true)); }
#else
            get { return new XPen(XColors.Ivory, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Khaki
        {
#if USE_CACHE
            get { return _khaki ?? (_khaki = new XPen(XColors.Khaki, 1, true)); }
#else
            get { return new XPen(XColors.Khaki, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Lavender
        {
#if USE_CACHE
            get { return _lavender ?? (_lavender = new XPen(XColors.Lavender, 1, true)); }
#else
            get { return new XPen(XColors.Lavender, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LavenderBlush
        {
#if USE_CACHE
            get { return _lavenderBlush ?? (_lavenderBlush = new XPen(XColors.LavenderBlush, 1, true)); }
#else
            get { return new XPen(XColors.LavenderBlush, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LawnGreen
        {
#if USE_CACHE
            get { return _lawnGreen ?? (_lawnGreen = new XPen(XColors.LawnGreen, 1, true)); }
#else
            get { return new XPen(XColors.LawnGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LemonChiffon
        {
#if USE_CACHE
            get { return _lemonChiffon ?? (_lemonChiffon = new XPen(XColors.LemonChiffon, 1, true)); }
#else
            get { return new XPen(XColors.LemonChiffon, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightBlue
        {
#if USE_CACHE
            get { return _lightBlue ?? (_lightBlue = new XPen(XColors.LightBlue, 1, true)); }
#else
            get { return new XPen(XColors.LightBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightCoral
        {
#if USE_CACHE
            get { return _lightCoral ?? (_lightCoral = new XPen(XColors.LightCoral, 1, true)); }
#else
            get { return new XPen(XColors.LightCoral, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightCyan
        {
#if USE_CACHE
            get { return _lightCyan ?? (_lightCyan = new XPen(XColors.LightCyan, 1, true)); }
#else
            get { return new XPen(XColors.LightCyan, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightGoldenrodYellow
        {
#if USE_CACHE
            get { return _lightGoldenrodYellow ?? (_lightGoldenrodYellow = new XPen(XColors.LightGoldenrodYellow, 1, true)); }
#else
            get { return new XPen(XColors.LightGoldenrodYellow, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightGray
        {
#if USE_CACHE
            get { return _lightGray ?? (_lightGray = new XPen(XColors.LightGray, 1, true)); }
#else
            get { return new XPen(XColors.LightGray, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightGreen
        {
#if USE_CACHE
            get { return _lightGreen ?? (_lightGreen = new XPen(XColors.LightGreen, 1, true)); }
#else
            get { return new XPen(XColors.LightGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightPink
        {
#if USE_CACHE
            get { return _lightPink ?? (_lightPink = new XPen(XColors.LightPink, 1, true)); }
#else
            get { return new XPen(XColors.LightPink, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSalmon
        {
#if USE_CACHE
            get { return _lightSalmon ?? (_lightSalmon = new XPen(XColors.LightSalmon, 1, true)); }
#else
            get { return new XPen(XColors.LightSalmon, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSeaGreen
        {
#if USE_CACHE
            get { return _lightSeaGreen ?? (_lightSeaGreen = new XPen(XColors.LightSeaGreen, 1, true)); }
#else
            get { return new XPen(XColors.LightSeaGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSkyBlue
        {
#if USE_CACHE
            get { return _lightSkyBlue ?? (_lightSkyBlue = new XPen(XColors.LightSkyBlue, 1, true)); }
#else
            get { return new XPen(XColors.LightSkyBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSlateGray
        {
#if USE_CACHE
            get { return _lightSlateGray ?? (_lightSlateGray = new XPen(XColors.LightSlateGray, 1, true)); }
#else
            get { return new XPen(XColors.LightSlateGray, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightSteelBlue
        {
#if USE_CACHE
            get { return _lightSteelBlue ?? (_lightSteelBlue = new XPen(XColors.LightSteelBlue, 1, true)); }
#else
            get { return new XPen(XColors.LightSteelBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LightYellow
        {
#if USE_CACHE
            get { return _lightYellow ?? (_lightYellow = new XPen(XColors.LightYellow, 1, true)); }
#else
            get { return new XPen(XColors.LightYellow, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Lime
        {
#if USE_CACHE
            get { return _lime ?? (_lime = new XPen(XColors.Lime, 1, true)); }
#else
            get { return new XPen(XColors.Lime, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen LimeGreen
        {
#if USE_CACHE
            get { return _limeGreen ?? (_limeGreen = new XPen(XColors.LimeGreen, 1, true)); }
#else
            get { return new XPen(XColors.LimeGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Linen
        {
#if USE_CACHE
            get { return _linen ?? (_linen = new XPen(XColors.Linen, 1, true)); }
#else
            get { return new XPen(XColors.Linen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Magenta
        {
#if USE_CACHE
            get { return _magenta ?? (_magenta = new XPen(XColors.Magenta, 1, true)); }
#else
            get { return new XPen(XColors.Magenta, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Maroon
        {
#if USE_CACHE
            get { return _maroon ?? (_maroon = new XPen(XColors.Maroon, 1, true)); }
#else
            get { return new XPen(XColors.Maroon, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumAquamarine
        {
#if USE_CACHE
            get { return _mediumAquamarine ?? (_mediumAquamarine = new XPen(XColors.MediumAquamarine, 1, true)); }
#else
            get { return new XPen(XColors.MediumAquamarine, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumBlue
        {
#if USE_CACHE
            get { return _mediumBlue ?? (_mediumBlue = new XPen(XColors.MediumBlue, 1, true)); }
#else
            get { return new XPen(XColors.MediumBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumOrchid
        {
#if USE_CACHE
            get { return _mediumOrchid ?? (_mediumOrchid = new XPen(XColors.MediumOrchid, 1, true)); }
#else
            get { return new XPen(XColors.MediumOrchid, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumPurple
        {
#if USE_CACHE
            get { return _mediumPurple ?? (_mediumPurple = new XPen(XColors.MediumPurple, 1, true)); }
#else
            get { return new XPen(XColors.MediumPurple, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumSeaGreen
        {
#if USE_CACHE
            get { return _mediumSeaGreen ?? (_mediumSeaGreen = new XPen(XColors.MediumSeaGreen, 1, true)); }
#else
            get { return new XPen(XColors.MediumSeaGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumSlateBlue
        {
#if USE_CACHE
            get { return _mediumSlateBlue ?? (_mediumSlateBlue = new XPen(XColors.MediumSlateBlue, 1, true)); }
#else
            get { return new XPen(XColors.MediumSlateBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumSpringGreen
        {
#if USE_CACHE
            get { return _mediumSpringGreen ?? (_mediumSpringGreen = new XPen(XColors.MediumSpringGreen, 1, true)); }
#else
            get { return new XPen(XColors.MediumSpringGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumTurquoise
        {
#if USE_CACHE
            get { return _mediumTurquoise ?? (_mediumTurquoise = new XPen(XColors.MediumTurquoise, 1, true)); }
#else
            get { return new XPen(XColors.MediumTurquoise, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MediumVioletRed
        {
#if USE_CACHE
            get { return _mediumVioletRed ?? (_mediumVioletRed = new XPen(XColors.MediumVioletRed, 1, true)); }
#else
            get { return new XPen(XColors.MediumVioletRed, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MidnightBlue
        {
#if USE_CACHE
            get { return _midnightBlue ?? (_midnightBlue = new XPen(XColors.MidnightBlue, 1, true)); }
#else
            get { return new XPen(XColors.MidnightBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MintCream
        {
#if USE_CACHE
            get { return _mintCream ?? (_mintCream = new XPen(XColors.MintCream, 1, true)); }
#else
            get { return new XPen(XColors.MintCream, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen MistyRose
        {
#if USE_CACHE
            get { return _mistyRose ?? (_mistyRose = new XPen(XColors.MistyRose, 1, true)); }
#else
            get { return new XPen(XColors.MistyRose, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Moccasin
        {
#if USE_CACHE
            get { return _moccasin ?? (_moccasin = new XPen(XColors.Moccasin, 1, true)); }
#else
            get { return new XPen(XColors.Moccasin, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen NavajoWhite
        {
#if USE_CACHE
            get { return _navajoWhite ?? (_navajoWhite = new XPen(XColors.NavajoWhite, 1, true)); }
#else
            get { return new XPen(XColors.NavajoWhite, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Navy
        {
#if USE_CACHE
            get { return _navy ?? (_navy = new XPen(XColors.Navy, 1, true)); }
#else
            get { return new XPen(XColors.Navy, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen OldLace
        {
#if USE_CACHE
            get { return _oldLace ?? (_oldLace = new XPen(XColors.OldLace, 1, true)); }
#else
            get { return new XPen(XColors.OldLace, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Olive
        {
#if USE_CACHE
            get { return _olive ?? (_olive = new XPen(XColors.Olive, 1, true)); }
#else
            get { return new XPen(XColors.Olive, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen OliveDrab
        {
#if USE_CACHE
            get { return _oliveDrab ?? (_oliveDrab = new XPen(XColors.OliveDrab, 1, true)); }
#else
            get { return new XPen(XColors.OliveDrab, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Orange
        {
#if USE_CACHE
            get { return _orange ?? (_orange = new XPen(XColors.Orange, 1, true)); }
#else
            get { return new XPen(XColors.Orange, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen OrangeRed
        {
#if USE_CACHE
            get { return _orangeRed ?? (_orangeRed = new XPen(XColors.OrangeRed, 1, true)); }
#else
            get { return new XPen(XColors.OrangeRed, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Orchid
        {
#if USE_CACHE
            get { return _orchid ?? (_orchid = new XPen(XColors.Orchid, 1, true)); }
#else
            get { return new XPen(XColors.Orchid, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleGoldenrod
        {
#if USE_CACHE
            get { return _paleGoldenrod ?? (_paleGoldenrod = new XPen(XColors.PaleGoldenrod, 1, true)); }
#else
            get { return new XPen(XColors.PaleGoldenrod, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleGreen
        {
#if USE_CACHE
            get { return _paleGreen ?? (_paleGreen = new XPen(XColors.PaleGreen, 1, true)); }
#else
            get { return new XPen(XColors.PaleGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleTurquoise
        {
#if USE_CACHE
            get { return _paleTurquoise ?? (_paleTurquoise = new XPen(XColors.PaleTurquoise, 1, true)); }
#else
            get { return new XPen(XColors.PaleTurquoise, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PaleVioletRed
        {
#if USE_CACHE
            get { return _paleVioletRed ?? (_paleVioletRed = new XPen(XColors.PaleVioletRed, 1, true)); }
#else
            get { return new XPen(XColors.PaleVioletRed, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PapayaWhip
        {
#if USE_CACHE
            get { return _papayaWhip ?? (_papayaWhip = new XPen(XColors.PapayaWhip, 1, true)); }
#else
            get { return new XPen(XColors.PapayaWhip, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PeachPuff
        {
#if USE_CACHE
            get { return _peachPuff ?? (_peachPuff = new XPen(XColors.PeachPuff, 1, true)); }
#else
            get { return new XPen(XColors.PeachPuff, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Peru
        {
#if USE_CACHE
            get { return _peru ?? (_peru = new XPen(XColors.Peru, 1, true)); }
#else
            get { return new XPen(XColors.Peru, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Pink
        {
#if USE_CACHE
            get { return _pink ?? (_pink = new XPen(XColors.Pink, 1, true)); }
#else
            get { return new XPen(XColors.Pink, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Plum
        {
#if USE_CACHE
            get { return _plum ?? (_plum = new XPen(XColors.Plum, 1, true)); }
#else
            get { return new XPen(XColors.Plum, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen PowderBlue
        {
#if USE_CACHE
            get { return _powderBlue ?? (_powderBlue = new XPen(XColors.PowderBlue, 1, true)); }
#else
            get { return new XPen(XColors.PowderBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Purple
        {
#if USE_CACHE
            get { return _purple ?? (_purple = new XPen(XColors.Purple, 1, true)); }
#else
            get { return new XPen(XColors.Purple, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Red
        {
#if USE_CACHE
            get { return _red ?? (_red = new XPen(XColors.Red, 1, true)); }
#else
            get { return new XPen(XColors.Red, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen RosyBrown
        {
#if USE_CACHE
            get { return _rosyBrown ?? (_rosyBrown = new XPen(XColors.RosyBrown, 1, true)); }
#else
            get { return new XPen(XColors.RosyBrown, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen RoyalBlue
        {
#if USE_CACHE
            get { return _royalBlue ?? (_royalBlue = new XPen(XColors.RoyalBlue, 1, true)); }
#else
            get { return new XPen(XColors.RoyalBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SaddleBrown
        {
#if USE_CACHE
            get { return _saddleBrown ?? (_saddleBrown = new XPen(XColors.SaddleBrown, 1, true)); }
#else
            get { return new XPen(XColors.SaddleBrown, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Salmon
        {
#if USE_CACHE
            get { return _salmon ?? (_salmon = new XPen(XColors.Salmon, 1, true)); }
#else
            get { return new XPen(XColors.Salmon, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SandyBrown
        {
#if USE_CACHE
            get { return _sandyBrown ?? (_sandyBrown = new XPen(XColors.SandyBrown, 1, true)); }
#else
            get { return new XPen(XColors.SandyBrown, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SeaGreen
        {
#if USE_CACHE
            get { return _seaGreen ?? (_seaGreen = new XPen(XColors.SeaGreen, 1, true)); }
#else
            get { return new XPen(XColors.SeaGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SeaShell
        {
#if USE_CACHE
            get { return _seaShell ?? (_seaShell = new XPen(XColors.SeaShell, 1, true)); }
#else
            get { return new XPen(XColors.SeaShell, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Sienna
        {
#if USE_CACHE
            get { return _sienna ?? (_sienna = new XPen(XColors.Sienna, 1, true)); }
#else
            get { return new XPen(XColors.Sienna, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Silver
        {
#if USE_CACHE
            get { return _silver ?? (_silver = new XPen(XColors.Silver, 1, true)); }
#else
            get { return new XPen(XColors.Silver, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SkyBlue
        {
#if USE_CACHE
            get { return _skyBlue ?? (_skyBlue = new XPen(XColors.SkyBlue, 1, true)); }
#else
            get { return new XPen(XColors.SkyBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SlateBlue
        {
#if USE_CACHE
            get { return _slateBlue ?? (_slateBlue = new XPen(XColors.SlateBlue, 1, true)); }
#else
            get { return new XPen(XColors.SlateBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SlateGray
        {
#if USE_CACHE
            get { return _slateGray ?? (_slateGray = new XPen(XColors.SlateGray, 1, true)); }
#else
            get { return new XPen(XColors.SlateGray, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Snow
        {
#if USE_CACHE
            get { return _snow ?? (_snow = new XPen(XColors.Snow, 1, true)); }
#else
            get { return new XPen(XColors.Snow, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SpringGreen
        {
#if USE_CACHE
            get { return _springGreen ?? (_springGreen = new XPen(XColors.SpringGreen, 1, true)); }
#else
            get { return new XPen(XColors.SpringGreen, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen SteelBlue
        {
#if USE_CACHE
            get { return _steelBlue ?? (_steelBlue = new XPen(XColors.SteelBlue, 1, true)); }
#else
            get { return new XPen(XColors.SteelBlue, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Tan
        {
#if USE_CACHE
            get { return _tan ?? (_tan = new XPen(XColors.Tan, 1, true)); }
#else
            get { return new XPen(XColors.Tan, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Teal
        {
#if USE_CACHE
            get { return _teal ?? (_teal = new XPen(XColors.Teal, 1, true)); }
#else
            get { return new XPen(XColors.Teal, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Thistle
        {
#if USE_CACHE
            get { return _thistle ?? (_thistle = new XPen(XColors.Thistle, 1, true)); }
#else
            get { return new XPen(XColors.Thistle, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Tomato
        {
#if USE_CACHE
            get { return _tomato ?? (_tomato = new XPen(XColors.Tomato, 1, true)); }
#else
            get { return new XPen(XColors.Tomato, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Transparent
        {
#if USE_CACHE
            get { return _transparent ?? (_transparent = new XPen(XColors.Transparent, 1, true)); }
#else
            get { return new XPen(XColors.Transparent, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Turquoise
        {
#if USE_CACHE
            get { return _turquoise ?? (_turquoise = new XPen(XColors.Turquoise, 1, true)); }
#else
            get { return new XPen(XColors.Turquoise, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Violet
        {
#if USE_CACHE
            get { return _violet ?? (_violet = new XPen(XColors.Violet, 1, true)); }
#else
            get { return new XPen(XColors.Violet, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Wheat
        {
#if USE_CACHE
            get { return _wheat ?? (_wheat = new XPen(XColors.Wheat, 1, true)); }
#else
            get { return new XPen(XColors.Wheat, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen White
        {
#if USE_CACHE
            get { return _white ?? (_white = new XPen(XColors.White, 1, true)); }
#else
            get { return new XPen(XColors.White, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen WhiteSmoke
        {
#if USE_CACHE
            get { return _whiteSmoke ?? (_whiteSmoke = new XPen(XColors.WhiteSmoke, 1, true)); }
#else
            get { return new XPen(XColors.WhiteSmoke, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen Yellow
        {
#if USE_CACHE
            get { return _yellow ?? (_yellow = new XPen(XColors.Yellow, 1, true)); }
#else
            get { return new XPen(XColors.Yellow, 1, true); }
#endif
        }

        /// <summary>Gets a pre-defined XPen object.</summary>
        public static XPen YellowGreen
        {
#if USE_CACHE
            get { return _yellowGreen ?? (_yellowGreen = new XPen(XColors.YellowGreen, 1, true)); }
#else
            get { return new XPen(XColors.YellowGreen, 1, true); }
#endif
        }

#if USE_CACHE
        static XPen _aliceBlue;
        static XPen _antiqueWhite;
        static XPen _aqua;
        static XPen _aquamarine;
        static XPen _azure;
        static XPen _beige;
        static XPen _bisque;
        static XPen _black;
        static XPen _blanchedAlmond;
        static XPen _blue;
        static XPen _blueViolet;
        static XPen _brown;
        static XPen _burlyWood;
        static XPen _cadetBlue;
        static XPen _chartreuse;
        static XPen _chocolate;
        static XPen _coral;
        static XPen _cornflowerBlue;
        static XPen _cornsilk;
        static XPen _crimson;
        static XPen _cyan;
        static XPen _darkBlue;
        static XPen _darkCyan;
        static XPen _darkGoldenrod;
        static XPen _darkGray;
        static XPen _darkGreen;
        static XPen _darkKhaki;
        static XPen _darkMagenta;
        static XPen _darkOliveGreen;
        static XPen _darkOrange;
        static XPen _darkOrchid;
        static XPen _darkRed;
        static XPen _darkSalmon;
        static XPen _darkSeaGreen;
        static XPen _darkSlateBlue;
        static XPen _darkSlateGray;
        static XPen _darkTurquoise;
        static XPen _darkViolet;
        static XPen _deepPink;
        static XPen _deepSkyBlue;
        static XPen _dimGray;
        static XPen _dodgerBlue;
        static XPen _firebrick;
        static XPen _floralWhite;
        static XPen _forestGreen;
        static XPen _fuchsia;
        static XPen _gainsboro;
        static XPen _ghostWhite;
        static XPen _gold;
        static XPen _goldenrod;
        static XPen _gray;
        static XPen _green;
        static XPen _greenYellow;
        static XPen _honeydew;
        static XPen _hotPink;
        static XPen _indianRed;
        static XPen _indigo;
        static XPen _ivory;
        static XPen _khaki;
        static XPen _lavender;
        static XPen _lavenderBlush;
        static XPen _lawnGreen;
        static XPen _lemonChiffon;
        static XPen _lightBlue;
        static XPen _lightCoral;
        static XPen _lightCyan;
        static XPen _lightGoldenrodYellow;
        static XPen _lightGray;
        static XPen _lightGreen;
        static XPen _lightPink;
        static XPen _lightSalmon;
        static XPen _lightSeaGreen;
        static XPen _lightSkyBlue;
        static XPen _lightSlateGray;
        static XPen _lightSteelBlue;
        static XPen _lightYellow;
        static XPen _lime;
        static XPen _limeGreen;
        static XPen _linen;
        static XPen _magenta;
        static XPen _maroon;
        static XPen _mediumAquamarine;
        static XPen _mediumBlue;
        static XPen _mediumOrchid;
        static XPen _mediumPurple;
        static XPen _mediumSeaGreen;
        static XPen _mediumSlateBlue;
        static XPen _mediumSpringGreen;
        static XPen _mediumTurquoise;
        static XPen _mediumVioletRed;
        static XPen _midnightBlue;
        static XPen _mintCream;
        static XPen _mistyRose;
        static XPen _moccasin;
        static XPen _navajoWhite;
        static XPen _navy;
        static XPen _oldLace;
        static XPen _olive;
        static XPen _oliveDrab;
        static XPen _orange;
        static XPen _orangeRed;
        static XPen _orchid;
        static XPen _paleGoldenrod;
        static XPen _paleGreen;
        static XPen _paleTurquoise;
        static XPen _paleVioletRed;
        static XPen _papayaWhip;
        static XPen _peachPuff;
        static XPen _peru;
        static XPen _pink;
        static XPen _plum;
        static XPen _powderBlue;
        static XPen _purple;
        static XPen _red;
        static XPen _rosyBrown;
        static XPen _royalBlue;
        static XPen _saddleBrown;
        static XPen _salmon;
        static XPen _sandyBrown;
        static XPen _seaGreen;
        static XPen _seaShell;
        static XPen _sienna;
        static XPen _silver;
        static XPen _skyBlue;
        static XPen _slateBlue;
        static XPen _slateGray;
        static XPen _snow;
        static XPen _springGreen;
        static XPen _steelBlue;
        static XPen _tan;
        static XPen _teal;
        static XPen _thistle;
        static XPen _tomato;
        static XPen _transparent;
        static XPen _turquoise;
        static XPen _violet;
        static XPen _wheat;
        static XPen _white;
        static XPen _whiteSmoke;
        static XPen _yellow;
        static XPen _yellowGreen;
#endif
    }
}
