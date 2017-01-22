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
    /// Brushes for all the pre-defined colors.
    /// </summary>
    public static class XBrushes
    {
        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush AliceBlue
        {
#if USE_CACHE
            get { return _aliceBlue ?? (_aliceBlue = new XSolidBrush(XColors.AliceBlue, true)); }
#else
            get { return new XSolidBrush(XColors.AliceBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush AntiqueWhite
        {
#if USE_CACHE
            get { return _antiqueWhite ?? (_antiqueWhite = new XSolidBrush(XColors.AntiqueWhite, true)); }
#else
            get { return new XSolidBrush(XColors.AntiqueWhite, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Aqua
        {
#if USE_CACHE
            get { return _aqua ?? (_aqua = new XSolidBrush(XColors.Aqua, true)); }
#else
            get { return new XSolidBrush(XColors.Aqua, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Aquamarine
        {
#if USE_CACHE
            get { return _aquamarine ?? (_aquamarine = new XSolidBrush(XColors.Aquamarine, true)); }
#else
            get { return new XSolidBrush(XColors.Aquamarine, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Azure
        {
#if USE_CACHE
            get { return _azure ?? (_azure = new XSolidBrush(XColors.Azure, true)); }
#else
            get { return new XSolidBrush(XColors.Azure, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Beige
        {
#if USE_CACHE
            get { return _beige ?? (_beige = new XSolidBrush(XColors.Beige, true)); }
#else
            get { return new XSolidBrush(XColors.Beige, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Bisque
        {
#if USE_CACHE
            get { return _bisque ?? (_bisque = new XSolidBrush(XColors.Bisque, true)); }
#else
            get { return new XSolidBrush(XColors.Bisque, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Black
        {
#if USE_CACHE
            get { return _black ?? (_black = new XSolidBrush(XColors.Black, true)); }
#else
            get { return new XSolidBrush(XColors.Black, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush BlanchedAlmond
        {
#if USE_CACHE
            get { return _blanchedAlmond ?? (_blanchedAlmond = new XSolidBrush(XColors.BlanchedAlmond, true)); }
#else
            get { return new XSolidBrush(XColors.BlanchedAlmond, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Blue
        {
#if USE_CACHE
            get { return _blue ?? (_blue = new XSolidBrush(XColors.Blue, true)); }
#else
            get { return new XSolidBrush(XColors.Blue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush BlueViolet
        {
#if USE_CACHE
            get { return _blueViolet ?? (_blueViolet = new XSolidBrush(XColors.BlueViolet, true)); }
#else
            get { return new XSolidBrush(XColors.BlueViolet, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Brown
        {
#if USE_CACHE
            get { return _brown ?? (_brown = new XSolidBrush(XColors.Brown, true)); }
#else
            get { return new XSolidBrush(XColors.Brown, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush BurlyWood
        {
#if USE_CACHE
            get { return _burlyWood ?? (_burlyWood = new XSolidBrush(XColors.BurlyWood, true)); }
#else
            get { return new XSolidBrush(XColors.BurlyWood, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush CadetBlue
        {
#if USE_CACHE
            get { return _cadetBlue ?? (_cadetBlue = new XSolidBrush(XColors.CadetBlue, true)); }
#else
            get { return new XSolidBrush(XColors.CadetBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Chartreuse
        {
#if USE_CACHE
            get { return _chartreuse ?? (_chartreuse = new XSolidBrush(XColors.Chartreuse, true)); }
#else
            get { return new XSolidBrush(XColors.Chartreuse, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Chocolate
        {
#if USE_CACHE
            get { return _chocolate ?? (_chocolate = new XSolidBrush(XColors.Chocolate, true)); }
#else
            get { return new XSolidBrush(XColors.Chocolate, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Coral
        {
#if USE_CACHE
            get { return _coral ?? (_coral = new XSolidBrush(XColors.Coral, true)); }
#else
            get { return new XSolidBrush(XColors.Coral, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush CornflowerBlue
        {
#if USE_CACHE
            get { return _cornflowerBlue ?? (_cornflowerBlue = new XSolidBrush(XColors.CornflowerBlue, true)); }
#else
            get { return new XSolidBrush(XColors.CornflowerBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Cornsilk
        {
#if USE_CACHE
            get { return _cornsilk ?? (_cornsilk = new XSolidBrush(XColors.Cornsilk, true)); }
#else
            get { return new XSolidBrush(XColors.Cornsilk, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Crimson
        {
#if USE_CACHE
            get { return _crimson ?? (_crimson = new XSolidBrush(XColors.Crimson, true)); }
#else
            get { return new XSolidBrush(XColors.Crimson, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Cyan
        {
#if USE_CACHE
            get { return _cyan ?? (_cyan = new XSolidBrush(XColors.Cyan, true)); }
#else
            get { return new XSolidBrush(XColors.Cyan, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkBlue
        {
#if USE_CACHE
            get { return _darkBlue ?? (_darkBlue = new XSolidBrush(XColors.DarkBlue, true)); }
#else
            get { return new XSolidBrush(XColors.DarkBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkCyan
        {
#if USE_CACHE
            get { return _darkCyan ?? (_darkCyan = new XSolidBrush(XColors.DarkCyan, true)); }
#else
            get { return new XSolidBrush(XColors.DarkCyan, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkGoldenrod
        {
#if USE_CACHE
            get { return _darkGoldenrod ?? (_darkGoldenrod = new XSolidBrush(XColors.DarkGoldenrod, true)); }
#else
            get { return new XSolidBrush(XColors.DarkGoldenrod, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkGray
        {
#if USE_CACHE
            get { return _darkGray ?? (_darkGray = new XSolidBrush(XColors.DarkGray, true)); }
#else
            get { return new XSolidBrush(XColors.DarkGray, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkGreen
        {
#if USE_CACHE
            get { return _darkGreen ?? (_darkGreen = new XSolidBrush(XColors.DarkGreen, true)); }
#else
            get { return new XSolidBrush(XColors.DarkGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkKhaki
        {
#if USE_CACHE
            get { return _darkKhaki ?? (_darkKhaki = new XSolidBrush(XColors.DarkKhaki, true)); }
#else
            get { return new XSolidBrush(XColors.DarkKhaki, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkMagenta
        {
#if USE_CACHE
            get { return _darkMagenta ?? (_darkMagenta = new XSolidBrush(XColors.DarkMagenta, true)); }
#else
            get { return new XSolidBrush(XColors.DarkMagenta, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkOliveGreen
        {
#if USE_CACHE
            get { return _darkOliveGreen ?? (_darkOliveGreen = new XSolidBrush(XColors.DarkOliveGreen, true)); }
#else
            get { return new XSolidBrush(XColors.DarkOliveGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkOrange
        {
#if USE_CACHE
            get { return _darkOrange ?? (_darkOrange = new XSolidBrush(XColors.DarkOrange, true)); }
#else
            get { return new XSolidBrush(XColors.DarkOrange, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkOrchid
        {
#if USE_CACHE
            get { return _darkOrchid ?? (_darkOrchid = new XSolidBrush(XColors.DarkOrchid, true)); }
#else
            get { return new XSolidBrush(XColors.DarkOrchid, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkRed
        {
#if USE_CACHE
            get { return _darkRed ?? (_darkRed = new XSolidBrush(XColors.DarkRed, true)); }
#else
            get { return new XSolidBrush(XColors.DarkRed, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSalmon
        {
#if USE_CACHE
            get { return _darkSalmon ?? (_darkSalmon = new XSolidBrush(XColors.DarkSalmon, true)); }
#else
            get { return new XSolidBrush(XColors.DarkSalmon, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSeaGreen
        {
#if USE_CACHE
            get { return _darkSeaGreen ?? (_darkSeaGreen = new XSolidBrush(XColors.DarkSeaGreen, true)); }
#else
            get { return new XSolidBrush(XColors.DarkSeaGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSlateBlue
        {
#if USE_CACHE
            get { return _darkSlateBlue ?? (_darkSlateBlue = new XSolidBrush(XColors.DarkSlateBlue, true)); }
#else
            get { return new XSolidBrush(XColors.DarkSlateBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkSlateGray
        {
#if USE_CACHE
            get { return _darkSlateGray ?? (_darkSlateGray = new XSolidBrush(XColors.DarkSlateGray, true)); }
#else
            get { return new XSolidBrush(XColors.DarkSlateGray, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkTurquoise
        {
#if USE_CACHE
            get { return _darkTurquoise ?? (_darkTurquoise = new XSolidBrush(XColors.DarkTurquoise, true)); }
#else
            get { return new XSolidBrush(XColors.DarkTurquoise, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DarkViolet
        {
#if USE_CACHE
            get { return _darkViolet ?? (_darkViolet = new XSolidBrush(XColors.DarkViolet, true)); }
#else
            get { return new XSolidBrush(XColors.DarkViolet, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DeepPink
        {
#if USE_CACHE
            get { return _deepPink ?? (_deepPink = new XSolidBrush(XColors.DeepPink, true)); }
#else
            get { return new XSolidBrush(XColors.DeepPink, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DeepSkyBlue
        {
#if USE_CACHE
            get { return _deepSkyBlue ?? (_deepSkyBlue = new XSolidBrush(XColors.DeepSkyBlue, true)); }
#else
            get { return new XSolidBrush(XColors.DeepSkyBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DimGray
        {
#if USE_CACHE
            get { return _dimGray ?? (_dimGray = new XSolidBrush(XColors.DimGray, true)); }
#else
            get { return new XSolidBrush(XColors.DimGray, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush DodgerBlue
        {
#if USE_CACHE
            get { return _dodgerBlue ?? (_dodgerBlue = new XSolidBrush(XColors.DodgerBlue, true)); }
#else
            get { return new XSolidBrush(XColors.DodgerBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Firebrick
        {
#if USE_CACHE
            get { return _firebrick ?? (_firebrick = new XSolidBrush(XColors.Firebrick, true)); }
#else
            get { return new XSolidBrush(XColors.Firebrick, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush FloralWhite
        {
#if USE_CACHE
            get { return _floralWhite ?? (_floralWhite = new XSolidBrush(XColors.FloralWhite, true)); }
#else
            get { return new XSolidBrush(XColors.FloralWhite, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush ForestGreen
        {
#if USE_CACHE
            get { return _forestGreen ?? (_forestGreen = new XSolidBrush(XColors.ForestGreen, true)); }
#else
            get { return new XSolidBrush(XColors.ForestGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Fuchsia
        {
#if USE_CACHE
            get { return _fuchsia ?? (_fuchsia = new XSolidBrush(XColors.Fuchsia, true)); }
#else
            get { return new XSolidBrush(XColors.Fuchsia, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Gainsboro
        {
#if USE_CACHE
            get { return _gainsboro ?? (_gainsboro = new XSolidBrush(XColors.Gainsboro, true)); }
#else
            get { return new XSolidBrush(XColors.Gainsboro, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush GhostWhite
        {
#if USE_CACHE
            get { return _ghostWhite ?? (_ghostWhite = new XSolidBrush(XColors.GhostWhite, true)); }
#else
            get { return new XSolidBrush(XColors.GhostWhite, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Gold
        {
#if USE_CACHE
            get { return _gold ?? (_gold = new XSolidBrush(XColors.Gold, true)); }
#else
            get { return new XSolidBrush(XColors.Gold, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Goldenrod
        {
#if USE_CACHE
            get { return _goldenrod ?? (_goldenrod = new XSolidBrush(XColors.Goldenrod, true)); }
#else
            get { return new XSolidBrush(XColors.Goldenrod, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Gray
        {
#if USE_CACHE
            get { return _gray ?? (_gray = new XSolidBrush(XColors.Gray, true)); }
#else
            get { return new XSolidBrush(XColors.Gray, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Green
        {
#if USE_CACHE
            get { return _green ?? (_green = new XSolidBrush(XColors.Green, true)); }
#else
            get { return new XSolidBrush(XColors.Green, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush GreenYellow
        {
#if USE_CACHE
            get { return _greenYellow ?? (_greenYellow = new XSolidBrush(XColors.GreenYellow, true)); }
#else
            get { return new XSolidBrush(XColors.GreenYellow, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Honeydew
        {
#if USE_CACHE
            get { return _honeydew ?? (_honeydew = new XSolidBrush(XColors.Honeydew, true)); }
#else
            get { return new XSolidBrush(XColors.Honeydew, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush HotPink
        {
#if USE_CACHE
            get { return _hotPink ?? (_hotPink = new XSolidBrush(XColors.HotPink, true)); }
#else
            get { return new XSolidBrush(XColors.HotPink, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush IndianRed
        {
#if USE_CACHE
            get { return _indianRed ?? (_indianRed = new XSolidBrush(XColors.IndianRed, true)); }
#else
            get { return new XSolidBrush(XColors.IndianRed, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Indigo
        {
#if USE_CACHE
            get { return _indigo ?? (_indigo = new XSolidBrush(XColors.Indigo, true)); }
#else
            get { return new XSolidBrush(XColors.Indigo, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Ivory
        {
#if USE_CACHE
            get { return _ivory ?? (_ivory = new XSolidBrush(XColors.Ivory, true)); }
#else
            get { return new XSolidBrush(XColors.Ivory, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Khaki
        {
#if USE_CACHE
            get { return _khaki ?? (_khaki = new XSolidBrush(XColors.Khaki, true)); }
#else
            get { return new XSolidBrush(XColors.Khaki, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Lavender
        {
#if USE_CACHE
            get { return _lavender ?? (_lavender = new XSolidBrush(XColors.Lavender, true)); }
#else
            get { return new XSolidBrush(XColors.Lavender, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LavenderBlush
        {
#if USE_CACHE
            get { return _lavenderBlush ?? (_lavenderBlush = new XSolidBrush(XColors.LavenderBlush, true)); }
#else
            get { return new XSolidBrush(XColors.LavenderBlush, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LawnGreen
        {
#if USE_CACHE
            get { return _lawnGreen ?? (_lawnGreen = new XSolidBrush(XColors.LawnGreen, true)); }
#else
            get { return new XSolidBrush(XColors.LawnGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LemonChiffon
        {
#if USE_CACHE
            get { return _lemonChiffon ?? (_lemonChiffon = new XSolidBrush(XColors.LemonChiffon, true)); }
#else
            get { return new XSolidBrush(XColors.LemonChiffon, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightBlue
        {
#if USE_CACHE
            get { return _lightBlue ?? (_lightBlue = new XSolidBrush(XColors.LightBlue, true)); }
#else
            get { return new XSolidBrush(XColors.LightBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightCoral
        {
#if USE_CACHE
            get { return _lightCoral ?? (_lightCoral = new XSolidBrush(XColors.LightCoral, true)); }
#else
            get { return new XSolidBrush(XColors.LightCoral, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightCyan
        {
#if USE_CACHE
            get { return _lightCyan ?? (_lightCyan = new XSolidBrush(XColors.LightCyan, true)); }
#else
            get { return new XSolidBrush(XColors.LightCyan, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightGoldenrodYellow
        {
#if USE_CACHE
            get { return _lightGoldenrodYellow ?? (_lightGoldenrodYellow = new XSolidBrush(XColors.LightGoldenrodYellow, true)); }
#else
            get { return new XSolidBrush(XColors.LightGoldenrodYellow, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightGray
        {
#if USE_CACHE
            get { return _lightGray ?? (_lightGray = new XSolidBrush(XColors.LightGray, true)); }
#else
            get { return new XSolidBrush(XColors.LightGray, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightGreen
        {
#if USE_CACHE
            get { return _lightGreen ?? (_lightGreen = new XSolidBrush(XColors.LightGreen, true)); }
#else
            get { return new XSolidBrush(XColors.LightGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightPink
        {
#if USE_CACHE
            get { return _lightPink ?? (_lightPink = new XSolidBrush(XColors.LightPink, true)); }
#else
            get { return new XSolidBrush(XColors.LightPink, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSalmon
        {
#if USE_CACHE
            get { return _lightSalmon ?? (_lightSalmon = new XSolidBrush(XColors.LightSalmon, true)); }
#else
            get { return new XSolidBrush(XColors.LightSalmon, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSeaGreen
        {
#if USE_CACHE
            get { return _lightSeaGreen ?? (_lightSeaGreen = new XSolidBrush(XColors.LightSeaGreen, true)); }
#else
            get { return new XSolidBrush(XColors.LightSeaGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSkyBlue
        {
#if USE_CACHE
            get { return _lightSkyBlue ?? (_lightSkyBlue = new XSolidBrush(XColors.LightSkyBlue, true)); }
#else
            get { return new XSolidBrush(XColors.LightSkyBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSlateGray
        {
#if USE_CACHE
            get { return _lightSlateGray ?? (_lightSlateGray = new XSolidBrush(XColors.LightSlateGray, true)); }
#else
            get { return new XSolidBrush(XColors.LightSlateGray, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightSteelBlue
        {
#if USE_CACHE
            get { return _lightSteelBlue ?? (_lightSteelBlue = new XSolidBrush(XColors.LightSteelBlue, true)); }
#else
            get { return new XSolidBrush(XColors.LightSteelBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LightYellow
        {
#if USE_CACHE
            get { return _lightYellow ?? (_lightYellow = new XSolidBrush(XColors.LightYellow, true)); }
#else
            get { return new XSolidBrush(XColors.LightYellow, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Lime
        {
#if USE_CACHE
            get { return _lime ?? (_lime = new XSolidBrush(XColors.Lime, true)); }
#else
            get { return new XSolidBrush(XColors.Lime, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush LimeGreen
        {
#if USE_CACHE
            get { return _limeGreen ?? (_limeGreen = new XSolidBrush(XColors.LimeGreen, true)); }
#else
            get { return new XSolidBrush(XColors.LimeGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Linen
        {
#if USE_CACHE
            get { return _linen ?? (_linen = new XSolidBrush(XColors.Linen, true)); }
#else
            get { return new XSolidBrush(XColors.Linen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Magenta
        {
#if USE_CACHE
            get { return _magenta ?? (_magenta = new XSolidBrush(XColors.Magenta, true)); }
#else
            get { return new XSolidBrush(XColors.Magenta, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Maroon
        {
#if USE_CACHE
            get { return _maroon ?? (_maroon = new XSolidBrush(XColors.Maroon, true)); }
#else
            get { return new XSolidBrush(XColors.Maroon, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumAquamarine
        {
#if USE_CACHE
            get { return _mediumAquamarine ?? (_mediumAquamarine = new XSolidBrush(XColors.MediumAquamarine, true)); }
#else
            get { return new XSolidBrush(XColors.MediumAquamarine, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumBlue
        {
#if USE_CACHE
            get { return _mediumBlue ?? (_mediumBlue = new XSolidBrush(XColors.MediumBlue, true)); }
#else
            get { return new XSolidBrush(XColors.MediumBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumOrchid
        {
#if USE_CACHE
            get { return _mediumOrchid ?? (_mediumOrchid = new XSolidBrush(XColors.MediumOrchid, true)); }
#else
            get { return new XSolidBrush(XColors.MediumOrchid, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumPurple
        {
#if USE_CACHE
            get { return _mediumPurple ?? (_mediumPurple = new XSolidBrush(XColors.MediumPurple, true)); }
#else
            get { return new XSolidBrush(XColors.MediumPurple, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumSeaGreen
        {
#if USE_CACHE
            get { return _mediumSeaGreen ?? (_mediumSeaGreen = new XSolidBrush(XColors.MediumSeaGreen, true)); }
#else
            get { return new XSolidBrush(XColors.MediumSeaGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumSlateBlue
        {
#if USE_CACHE
            get { return _mediumSlateBlue ?? (_mediumSlateBlue = new XSolidBrush(XColors.MediumSlateBlue, true)); }
#else
            get { return new XSolidBrush(XColors.MediumSlateBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumSpringGreen
        {
#if USE_CACHE
            get { return _mediumSpringGreen ?? (_mediumSpringGreen = new XSolidBrush(XColors.MediumSpringGreen, true)); }
#else
            get { return new XSolidBrush(XColors.MediumSpringGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumTurquoise
        {
#if USE_CACHE
            get { return _mediumTurquoise ?? (_mediumTurquoise = new XSolidBrush(XColors.MediumTurquoise, true)); }
#else
            get { return new XSolidBrush(XColors.MediumTurquoise, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MediumVioletRed
        {
#if USE_CACHE
            get { return _mediumVioletRed ?? (_mediumVioletRed = new XSolidBrush(XColors.MediumVioletRed, true)); }
#else
            get { return new XSolidBrush(XColors.MediumVioletRed, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MidnightBlue
        {
#if USE_CACHE
            get { return _midnightBlue ?? (_midnightBlue = new XSolidBrush(XColors.MidnightBlue, true)); }
#else
            get { return new XSolidBrush(XColors.MidnightBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MintCream
        {
#if USE_CACHE
            get { return _mintCream ?? (_mintCream = new XSolidBrush(XColors.MintCream, true)); }
#else
            get { return new XSolidBrush(XColors.MintCream, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush MistyRose
        {
#if USE_CACHE
            get { return _mistyRose ?? (_mistyRose = new XSolidBrush(XColors.MistyRose, true)); }
#else
            get { return new XSolidBrush(XColors.MistyRose, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Moccasin
        {
#if USE_CACHE
            get { return _moccasin ?? (_moccasin = new XSolidBrush(XColors.Moccasin, true)); }
#else
            get { return new XSolidBrush(XColors.Moccasin, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush NavajoWhite
        {
#if USE_CACHE
            get { return _navajoWhite ?? (_navajoWhite = new XSolidBrush(XColors.NavajoWhite, true)); }
#else
            get { return new XSolidBrush(XColors.NavajoWhite, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Navy
        {
#if USE_CACHE
            get { return _navy ?? (_navy = new XSolidBrush(XColors.Navy, true)); }
#else
            get { return new XSolidBrush(XColors.Navy, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush OldLace
        {
#if USE_CACHE
            get { return _oldLace ?? (_oldLace = new XSolidBrush(XColors.OldLace, true)); }
#else
            get { return new XSolidBrush(XColors.OldLace, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Olive
        {
#if USE_CACHE
            get { return _olive ?? (_olive = new XSolidBrush(XColors.Olive, true)); }
#else
            get { return new XSolidBrush(XColors.Olive, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush OliveDrab
        {
#if USE_CACHE
            get { return _oliveDrab ?? (_oliveDrab = new XSolidBrush(XColors.OliveDrab, true)); }
#else
            get { return new XSolidBrush(XColors.OliveDrab, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Orange
        {
#if USE_CACHE
            get { return _orange ?? (_orange = new XSolidBrush(XColors.Orange, true)); }
#else
            get { return new XSolidBrush(XColors.Orange, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush OrangeRed
        {
#if USE_CACHE
            get { return _orangeRed ?? (_orangeRed = new XSolidBrush(XColors.OrangeRed, true)); }
#else
            get { return new XSolidBrush(XColors.OrangeRed, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Orchid
        {
#if USE_CACHE
            get { return _orchid ?? (_orchid = new XSolidBrush(XColors.Orchid, true)); }
#else
            get { return new XSolidBrush(XColors.Orchid, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleGoldenrod
        {
#if USE_CACHE
            get { return _paleGoldenrod ?? (_paleGoldenrod = new XSolidBrush(XColors.PaleGoldenrod, true)); }
#else
            get { return new XSolidBrush(XColors.PaleGoldenrod, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleGreen
        {
#if USE_CACHE
            get { return _paleGreen ?? (_paleGreen = new XSolidBrush(XColors.PaleGreen, true)); }
#else
            get { return new XSolidBrush(XColors.PaleGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleTurquoise
        {
#if USE_CACHE
            get { return _paleTurquoise ?? (_paleTurquoise = new XSolidBrush(XColors.PaleTurquoise, true)); }
#else
            get { return new XSolidBrush(XColors.PaleTurquoise, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PaleVioletRed
        {
#if USE_CACHE
            get { return _paleVioletRed ?? (_paleVioletRed = new XSolidBrush(XColors.PaleVioletRed, true)); }
#else
            get { return new XSolidBrush(XColors.PaleVioletRed, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PapayaWhip
        {
#if USE_CACHE
            get { return _papayaWhip ?? (_papayaWhip = new XSolidBrush(XColors.PapayaWhip, true)); }
#else
            get { return new XSolidBrush(XColors.PapayaWhip, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PeachPuff
        {
#if USE_CACHE
            get { return _peachPuff ?? (_peachPuff = new XSolidBrush(XColors.PeachPuff, true)); }
#else
            get { return new XSolidBrush(XColors.PeachPuff, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Peru
        {
#if USE_CACHE
            get { return _peru ?? (_peru = new XSolidBrush(XColors.Peru, true)); }
#else
            get { return new XSolidBrush(XColors.Peru, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Pink
        {
#if USE_CACHE
            get { return _pink ?? (_pink = new XSolidBrush(XColors.Pink, true)); }
#else
            get { return new XSolidBrush(XColors.Pink, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Plum
        {
#if USE_CACHE
            get { return _plum ?? (_plum = new XSolidBrush(XColors.Plum, true)); }
#else
            get { return new XSolidBrush(XColors.Plum, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush PowderBlue
        {
#if USE_CACHE
            get { return _powderBlue ?? (_powderBlue = new XSolidBrush(XColors.PowderBlue, true)); }
#else
            get { return new XSolidBrush(XColors.PowderBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Purple
        {
#if USE_CACHE
            get { return _purple ?? (_purple = new XSolidBrush(XColors.Purple, true)); }
#else
            get { return new XSolidBrush(XColors.Purple, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Red
        {
#if USE_CACHE
            get { return _red ?? (_red = new XSolidBrush(XColors.Red, true)); }
#else
            get { return new XSolidBrush(XColors.Red, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush RosyBrown
        {
#if USE_CACHE
            get { return _rosyBrown ?? (_rosyBrown = new XSolidBrush(XColors.RosyBrown, true)); }
#else
            get { return new XSolidBrush(XColors.RosyBrown, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush RoyalBlue
        {
#if USE_CACHE
            get { return _royalBlue ?? (_royalBlue = new XSolidBrush(XColors.RoyalBlue, true)); }
#else
            get { return new XSolidBrush(XColors.RoyalBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SaddleBrown
        {
#if USE_CACHE
            get { return _saddleBrown ?? (_saddleBrown = new XSolidBrush(XColors.SaddleBrown, true)); }
#else
            get { return new XSolidBrush(XColors.SaddleBrown, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Salmon
        {
#if USE_CACHE
            get { return _salmon ?? (_salmon = new XSolidBrush(XColors.Salmon, true)); }
#else
            get { return new XSolidBrush(XColors.Salmon, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SandyBrown
        {
#if USE_CACHE
            get { return _sandyBrown ?? (_sandyBrown = new XSolidBrush(XColors.SandyBrown, true)); }
#else
            get { return new XSolidBrush(XColors.SandyBrown, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SeaGreen
        {
#if USE_CACHE
            get { return _seaGreen ?? (_seaGreen = new XSolidBrush(XColors.SeaGreen, true)); }
#else
            get { return new XSolidBrush(XColors.SeaGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SeaShell
        {
#if USE_CACHE
            get { return _seaShell ?? (_seaShell = new XSolidBrush(XColors.SeaShell, true)); }
#else
            get { return new XSolidBrush(XColors.SeaShell, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Sienna
        {
#if USE_CACHE
            get { return _sienna ?? (_sienna = new XSolidBrush(XColors.Sienna, true)); }
#else
            get { return new XSolidBrush(XColors.Sienna, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Silver
        {
#if USE_CACHE
            get { return _silver ?? (_silver = new XSolidBrush(XColors.Silver, true)); }
#else
            get { return new XSolidBrush(XColors.Silver, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SkyBlue
        {
#if USE_CACHE
            get { return _skyBlue ?? (_skyBlue = new XSolidBrush(XColors.SkyBlue, true)); }
#else
            get { return new XSolidBrush(XColors.SkyBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SlateBlue
        {
#if USE_CACHE
            get { return _slateBlue ?? (_slateBlue = new XSolidBrush(XColors.SlateBlue, true)); }
#else
            get { return new XSolidBrush(XColors.SlateBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SlateGray
        {
#if USE_CACHE
            get { return _slateGray ?? (_slateGray = new XSolidBrush(XColors.SlateGray, true)); }
#else
            get { return new XSolidBrush(XColors.SlateGray, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Snow
        {
#if USE_CACHE
            get { return _snow ?? (_snow = new XSolidBrush(XColors.Snow, true)); }
#else
            get { return new XSolidBrush(XColors.Snow, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SpringGreen
        {
#if USE_CACHE
            get { return _springGreen ?? (_springGreen = new XSolidBrush(XColors.SpringGreen, true)); }
#else
            get { return new XSolidBrush(XColors.SpringGreen, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush SteelBlue
        {
#if USE_CACHE
            get { return _steelBlue ?? (_steelBlue = new XSolidBrush(XColors.SteelBlue, true)); }
#else
            get { return new XSolidBrush(XColors.SteelBlue, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Tan
        {
#if USE_CACHE
            get { return _tan ?? (_tan = new XSolidBrush(XColors.Tan, true)); }
#else
            get { return new XSolidBrush(XColors.Tan, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Teal
        {
#if USE_CACHE
            get { return _teal ?? (_teal = new XSolidBrush(XColors.Teal, true)); }
#else
            get { return new XSolidBrush(XColors.Teal, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Thistle
        {
#if USE_CACHE
            get { return _thistle ?? (_thistle = new XSolidBrush(XColors.Thistle, true)); }
#else
            get { return new XSolidBrush(XColors.Thistle, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Tomato
        {
#if USE_CACHE
            get { return _tomato ?? (_tomato = new XSolidBrush(XColors.Tomato, true)); }
#else
            get { return new XSolidBrush(XColors.Tomato, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Transparent
        {
#if USE_CACHE
            get { return _transparent ?? (_transparent = new XSolidBrush(XColors.Transparent, true)); }
#else
            get { return new XSolidBrush(XColors.Transparent, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Turquoise
        {
#if USE_CACHE
            get { return _turquoise ?? (_turquoise = new XSolidBrush(XColors.Turquoise, true)); }
#else
            get { return new XSolidBrush(XColors.Turquoise, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Violet
        {
#if USE_CACHE
            get { return _violet ?? (_violet = new XSolidBrush(XColors.Violet, true)); }
#else
            get { return new XSolidBrush(XColors.Violet, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Wheat
        {
#if USE_CACHE
            get { return _wheat ?? (_wheat = new XSolidBrush(XColors.Wheat, true)); }
#else
            get { return new XSolidBrush(XColors.Wheat, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush White
        {
#if USE_CACHE
            get { return _white ?? (_white = new XSolidBrush(XColors.White, true)); }
#else
            get { return new XSolidBrush(XColors.White, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush WhiteSmoke
        {
#if USE_CACHE
            get { return _whiteSmoke ?? (_whiteSmoke = new XSolidBrush(XColors.WhiteSmoke, true)); }
#else
            get { return new XSolidBrush(XColors.WhiteSmoke, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush Yellow
        {
#if USE_CACHE
            get { return _yellow ?? (_yellow = new XSolidBrush(XColors.Yellow, true)); }
#else
            get { return new XSolidBrush(XColors.Yellow, true); }
#endif
        }

        /// <summary>Gets a pre-defined XBrush object.</summary>
        public static XSolidBrush YellowGreen
        {
#if USE_CACHE
            get { return _yellowGreen ?? (_yellowGreen = new XSolidBrush(XColors.YellowGreen, true)); }
#else
            get { return new XSolidBrush(XColors.YellowGreen, true); }
#endif
        }

#if USE_CACHE
        static XSolidBrush _aliceBlue;
        static XSolidBrush _antiqueWhite;
        static XSolidBrush _aqua;
        static XSolidBrush _aquamarine;
        static XSolidBrush _azure;
        static XSolidBrush _beige;
        static XSolidBrush _bisque;
        static XSolidBrush _black;
        static XSolidBrush _blanchedAlmond;
        static XSolidBrush _blue;
        static XSolidBrush _blueViolet;
        static XSolidBrush _brown;
        static XSolidBrush _burlyWood;
        static XSolidBrush _cadetBlue;
        static XSolidBrush _chartreuse;
        static XSolidBrush _chocolate;
        static XSolidBrush _coral;
        static XSolidBrush _cornflowerBlue;
        static XSolidBrush _cornsilk;
        static XSolidBrush _crimson;
        static XSolidBrush _cyan;
        static XSolidBrush _darkBlue;
        static XSolidBrush _darkCyan;
        static XSolidBrush _darkGoldenrod;
        static XSolidBrush _darkGray;
        static XSolidBrush _darkGreen;
        static XSolidBrush _darkKhaki;
        static XSolidBrush _darkMagenta;
        static XSolidBrush _darkOliveGreen;
        static XSolidBrush _darkOrange;
        static XSolidBrush _darkOrchid;
        static XSolidBrush _darkRed;
        static XSolidBrush _darkSalmon;
        static XSolidBrush _darkSeaGreen;
        static XSolidBrush _darkSlateBlue;
        static XSolidBrush _darkSlateGray;
        static XSolidBrush _darkTurquoise;
        static XSolidBrush _darkViolet;
        static XSolidBrush _deepPink;
        static XSolidBrush _deepSkyBlue;
        static XSolidBrush _dimGray;
        static XSolidBrush _dodgerBlue;
        static XSolidBrush _firebrick;
        static XSolidBrush _floralWhite;
        static XSolidBrush _forestGreen;
        static XSolidBrush _fuchsia;
        static XSolidBrush _gainsboro;
        static XSolidBrush _ghostWhite;
        static XSolidBrush _gold;
        static XSolidBrush _goldenrod;
        static XSolidBrush _gray;
        static XSolidBrush _green;
        static XSolidBrush _greenYellow;
        static XSolidBrush _honeydew;
        static XSolidBrush _hotPink;
        static XSolidBrush _indianRed;
        static XSolidBrush _indigo;
        static XSolidBrush _ivory;
        static XSolidBrush _khaki;
        static XSolidBrush _lavender;
        static XSolidBrush _lavenderBlush;
        static XSolidBrush _lawnGreen;
        static XSolidBrush _lemonChiffon;
        static XSolidBrush _lightBlue;
        static XSolidBrush _lightCoral;
        static XSolidBrush _lightCyan;
        static XSolidBrush _lightGoldenrodYellow;
        static XSolidBrush _lightGray;
        static XSolidBrush _lightGreen;
        static XSolidBrush _lightPink;
        static XSolidBrush _lightSalmon;
        static XSolidBrush _lightSeaGreen;
        static XSolidBrush _lightSkyBlue;
        static XSolidBrush _lightSlateGray;
        static XSolidBrush _lightSteelBlue;
        static XSolidBrush _lightYellow;
        static XSolidBrush _lime;
        static XSolidBrush _limeGreen;
        static XSolidBrush _linen;
        static XSolidBrush _magenta;
        static XSolidBrush _maroon;
        static XSolidBrush _mediumAquamarine;
        static XSolidBrush _mediumBlue;
        static XSolidBrush _mediumOrchid;
        static XSolidBrush _mediumPurple;
        static XSolidBrush _mediumSeaGreen;
        static XSolidBrush _mediumSlateBlue;
        static XSolidBrush _mediumSpringGreen;
        static XSolidBrush _mediumTurquoise;
        static XSolidBrush _mediumVioletRed;
        static XSolidBrush _midnightBlue;
        static XSolidBrush _mintCream;
        static XSolidBrush _mistyRose;
        static XSolidBrush _moccasin;
        static XSolidBrush _navajoWhite;
        static XSolidBrush _navy;
        static XSolidBrush _oldLace;
        static XSolidBrush _olive;
        static XSolidBrush _oliveDrab;
        static XSolidBrush _orange;
        static XSolidBrush _orangeRed;
        static XSolidBrush _orchid;
        static XSolidBrush _paleGoldenrod;
        static XSolidBrush _paleGreen;
        static XSolidBrush _paleTurquoise;
        static XSolidBrush _paleVioletRed;
        static XSolidBrush _papayaWhip;
        static XSolidBrush _peachPuff;
        static XSolidBrush _peru;
        static XSolidBrush _pink;
        static XSolidBrush _plum;
        static XSolidBrush _powderBlue;
        static XSolidBrush _purple;
        static XSolidBrush _red;
        static XSolidBrush _rosyBrown;
        static XSolidBrush _royalBlue;
        static XSolidBrush _saddleBrown;
        static XSolidBrush _salmon;
        static XSolidBrush _sandyBrown;
        static XSolidBrush _seaGreen;
        static XSolidBrush _seaShell;
        static XSolidBrush _sienna;
        static XSolidBrush _silver;
        static XSolidBrush _skyBlue;
        static XSolidBrush _slateBlue;
        static XSolidBrush _slateGray;
        static XSolidBrush _snow;
        static XSolidBrush _springGreen;
        static XSolidBrush _steelBlue;
        static XSolidBrush _tan;
        static XSolidBrush _teal;
        static XSolidBrush _thistle;
        static XSolidBrush _tomato;
        static XSolidBrush _transparent;
        static XSolidBrush _turquoise;
        static XSolidBrush _violet;
        static XSolidBrush _wheat;
        static XSolidBrush _white;
        static XSolidBrush _whiteSmoke;
        static XSolidBrush _yellow;
        static XSolidBrush _yellowGreen;
#endif
    }
}
