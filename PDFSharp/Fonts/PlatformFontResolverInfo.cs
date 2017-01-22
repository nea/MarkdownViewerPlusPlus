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

#if CORE || GDI
using System.Drawing;
using GdiFont = System.Drawing.Font;

#endif
#if WPF
using System.Windows.Media;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfTypeface = System.Windows.Media.Typeface;
using WpfGlyphTypeface = System.Windows.Media.GlyphTypeface;
#endif

namespace PdfSharp.Fonts
{
    /// <summary>
    /// Represents a font resolver info created by the platform font resolver.
    /// </summary>
    internal class PlatformFontResolverInfo : FontResolverInfo
    {
#if CORE || GDI
        public PlatformFontResolverInfo(string faceName, bool mustSimulateBold, bool mustSimulateItalic, GdiFont gdiFont)
            : base(faceName, mustSimulateBold, mustSimulateItalic)
        {
            _gdiFont = gdiFont;
        }
#endif
#if WPF
        public PlatformFontResolverInfo(string faceName, bool mustSimulateBold, bool mustSimulateItalic, WpfFontFamily wpfFontFamily,
            WpfTypeface wpfTypeface, WpfGlyphTypeface wpfGlyphTypeface)
            : base(faceName, mustSimulateBold, mustSimulateItalic)
        {
            _wpfFontFamily = wpfFontFamily;
            _wpfTypeface = wpfTypeface;
            _wpfGlyphTypeface = wpfGlyphTypeface;
        }
#endif

#if CORE || GDI
        public Font GdiFont
        {
            get { return _gdiFont; }
        }
        readonly Font _gdiFont;
#endif
#if WPF
        public WpfFontFamily WpfFontFamily
        {
            get { return _wpfFontFamily; }
        }
        readonly WpfFontFamily _wpfFontFamily;

        public WpfTypeface WpfTypeface
        {
            get { return _wpfTypeface; }
        }
        readonly WpfTypeface _wpfTypeface;

        public WpfGlyphTypeface WpfGlyphTypeface
        {
            get { return _wpfGlyphTypeface; }
        }
        readonly WpfGlyphTypeface _wpfGlyphTypeface;
#endif
#if NETFX_CORE || UWP
        public PlatformFontResolverInfo(string faceName, bool mustSimulateBold, bool mustSimulateItalic)
            : base(faceName, mustSimulateBold, mustSimulateItalic)
        {
            //_gdiFont = gdiFont;
        }
#endif
    }
}
