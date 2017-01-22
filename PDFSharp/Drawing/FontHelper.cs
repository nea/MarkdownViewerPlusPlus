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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
#if CORE || GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiFontFamily = System.Drawing.FontFamily;
using GdiFont = System.Drawing.Font;
using GdiFontStyle = System.Drawing.FontStyle;
#endif
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Markup;
using WpfFontStyle = System.Windows.FontStyle;
using WpfFontWeight = System.Windows.FontWeight;
using WpfBrush = System.Windows.Media.Brush;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfTypeface = System.Windows.Media.Typeface;
using WpfGlyphTypeface = System.Windows.Media.GlyphTypeface;
#endif
#if NETFX_CORE
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endif
using PdfSharp.Fonts;
using PdfSharp.Fonts.OpenType;

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Bunch of functions that do not have a better place.
    /// </summary>
    static class FontHelper
    {
        /// <summary>
        /// Measure string directly from font data.
        /// </summary>
        public static XSize MeasureString(string text, XFont font, XStringFormat stringFormat)
        {
            XSize size = new XSize();

            OpenTypeDescriptor descriptor = FontDescriptorCache.GetOrCreateDescriptorFor(font) as OpenTypeDescriptor;
            if (descriptor != null)
            {
                // Height is the sum of ascender and descender.
                size.Height = (descriptor.Ascender + descriptor.Descender) * font.Size / font.UnitsPerEm;
                Debug.Assert(descriptor.Ascender > 0);

                bool symbol = descriptor.FontFace.cmap.symbol;
                int length = text.Length;
                int width = 0;
                for (int idx = 0; idx < length; idx++)
                {
                    char ch = text[idx];
                    // HACK: Unclear what to do here.
                    if (ch < 32)
                        continue;

                    if (symbol)
                    {
                        // Remap ch for symbol fonts.
                        ch = (char)(ch | (descriptor.FontFace.os2.usFirstCharIndex & 0xFF00));  // @@@ refactor
                        // Used | instead of + because of: http://pdfsharp.codeplex.com/workitem/15954
                    }
                    int glyphIndex = descriptor.CharCodeToGlyphIndex(ch);
                    width += descriptor.GlyphIndexToWidth(glyphIndex);
                }
                // What? size.Width = width * font.Size * (font.Italic ? 1 : 1) / descriptor.UnitsPerEm;
                size.Width = width * font.Size / descriptor.UnitsPerEm;

                // Adjust bold simulation.
                if ((font.GlyphTypeface.StyleSimulations & XStyleSimulations.BoldSimulation) == XStyleSimulations.BoldSimulation)
                {
                    // Add 2% of the em-size for each character.
                    // Unsure how to deal with white space. Currently count as regular character.
                    size.Width += length * font.Size * Const.BoldEmphasis;
                }
            }
            Debug.Assert(descriptor != null, "No OpenTypeDescriptor.");
            return size;
        }

#if CORE || GDI
        public static GdiFont CreateFont(string familyName, double emSize, GdiFontStyle style, out XFontSource fontSource)
        {
            fontSource = null;
            // ReSharper disable once JoinDeclarationAndInitializer
            GdiFont font;

            // Use font resolver in CORE build. XPrivateFontCollection exists only in GDI and WPF build.
#if GDI
            // Try private font collection first.
            font = XPrivateFontCollection.TryCreateFont(familyName, emSize, style, out fontSource);
            if (font != null)
            {
                // Get font source is different for this font because Win32 does not know it.
                return font;
            }
#endif
            // Create ordinary Win32 font.
            font = new GdiFont(familyName, (float)emSize, style, GraphicsUnit.World);
            return font;
        }
#endif

#if WPF
#if !SILVERLIGHT
        public static readonly CultureInfo CultureInfoEnUs = CultureInfo.GetCultureInfo("en-US");
        public static readonly XmlLanguage XmlLanguageEnUs = XmlLanguage.GetLanguage("en-US");
#endif
        /// <summary>
        /// Creates a typeface.
        /// </summary>
        public static Typeface CreateTypeface(WpfFontFamily family, XFontStyle style)
        {
            // BUG: does not work with fonts that have others than the four default styles
            WpfFontStyle fontStyle = FontStyleFromStyle(style);
            WpfFontWeight fontWeight = FontWeightFromStyle(style);
#if !SILVERLIGHT
            WpfTypeface typeface = new WpfTypeface(family, fontStyle, fontWeight, FontStretches.Normal);
#else
            WpfTypeface typeface = null;
#endif
            return typeface;
        }

#if !SILVERLIGHT
        /// <summary>
        /// Creates the formatted text.
        /// </summary>
        public static FormattedText CreateFormattedText(string text, Typeface typeface, double emSize, WpfBrush brush)
        {
            //FontFamily fontFamily = new FontFamily(testFontName);
            //typeface = new Typeface(fontFamily, FontStyles.Normal, FontWeights.Bold, FontStretches.Condensed);
            //List<Typeface> typefaces = new List<Typeface>(fontFamily.GetTypefaces());
            //typefaces.GetType();
            //typeface = s_typefaces[0];

            // BUG: does not work with fonts that have others than the four default styles
            FormattedText formattedText = new FormattedText(text, new CultureInfo("en-us"), FlowDirection.LeftToRight, typeface, emSize, brush);
            // .NET 4.0 feature new NumberSubstitution(), TextFormattingMode.Display);
            //formattedText.SetFontWeight(FontWeights.Bold);
            //formattedText.SetFontStyle(FontStyles.Oblique);
            //formattedText.SetFontStretch(FontStretches.Condensed);
            return formattedText;
        }
#endif

#if SILVERLIGHT_
        /// <summary>
        /// Creates the TextBlock.
        /// </summary>
        public static TextBlock CreateTextBlock(string text, XGlyphTypeface glyphTypeface, double emSize, Brush brush)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.FontFamily = glyphTypeface.FontFamily;
            textBlock.FontSource = glyphTypeface.FontSource;
            textBlock.FontSize = emSize;
            textBlock.FontWeight = glyphTypeface.IsBold ? FontWeights.Bold : FontWeights.Normal;
            textBlock.FontStyle = glyphTypeface.IsItalic ? FontStyles.Italic : FontStyles.Normal;
            textBlock.Foreground = brush;
            textBlock.Text = text;

            return textBlock;
        }
#endif

        /// <summary>
        /// Simple hack to make it work...
        /// </summary>
        public static WpfFontStyle FontStyleFromStyle(XFontStyle style)
        {
            switch (style & XFontStyle.BoldItalic)  // Mask out Underline, Strikeout, etc.
            {
                case XFontStyle.Regular:
                    return FontStyles.Normal;

                case XFontStyle.Bold:
                    return FontStyles.Normal;

                case XFontStyle.Italic:
                    return FontStyles.Italic;

                case XFontStyle.BoldItalic:
                    return FontStyles.Italic;
            }
            return FontStyles.Normal;
        }

        /// <summary>
        /// Simple hack to make it work...
        /// </summary>
        public static FontWeight FontWeightFromStyle(XFontStyle style)
        {
            switch (style)
            {
                case XFontStyle.Regular:
                    return FontWeights.Normal;

                case XFontStyle.Bold:
                    return FontWeights.Bold;

                case XFontStyle.Italic:
                    return FontWeights.Normal;

                case XFontStyle.BoldItalic:
                    return FontWeights.Bold;
            }
            return FontWeights.Normal;
        }

        /// <summary>
        /// Determines whether the style is available as a glyph type face in the specified font family, i.e. the specified style is not simulated.
        /// </summary>
        public static bool IsStyleAvailable(XFontFamily family, XGdiFontStyle style)
        {
            style &= XGdiFontStyle.BoldItalic;
#if !SILVERLIGHT
            // TODOWPF: check for correctness
            // FontDescriptor descriptor = FontDescriptorCache.GetOrCreateDescriptor(family.Name, style);
            //XFontMetrics metrics = descriptor.FontMetrics;

            // style &= XFontStyle.Regular | XFontStyle.Bold | XFontStyle.Italic | XFontStyle.BoldItalic; // same as XFontStyle.BoldItalic
            List<WpfTypeface> typefaces = new List<WpfTypeface>(family.WpfFamily.GetTypefaces());
            foreach (WpfTypeface typeface in typefaces)
            {
                bool bold = typeface.Weight == FontWeights.Bold;
                bool italic = typeface.Style == FontStyles.Italic;
                switch (style)
                {
                    case XGdiFontStyle.Regular:
                        if (!bold && !italic)
                            return true;
                        break;

                    case XGdiFontStyle.Bold:
                        if (bold && !italic)
                            return true;
                        break;

                    case XGdiFontStyle.Italic:
                        if (!bold && italic)
                            return true;
                        break;

                    case XGdiFontStyle.BoldItalic:
                        if (bold && italic)
                            return true;
                        break;
                }
                //////                typeface.sty
                //////                bool available = false;
                //////                GlyphTypeface glyphTypeface;
                //////                if (typeface.TryGetGlyphTypeface(out glyphTypeface))
                //////                {
                //////#if DEBUG_
                //////                    glyphTypeface.GetType();
                //////#endif
                //////                    available = true;
                //////                }
                //////                if (available)
                //////                    return true;
            }
            return false;
#else
            return true; // AGHACK
#endif
        }
#endif

        /// <summary>
        /// Calculates an Adler32 checksum combined with the buffer length
        /// in a 64 bit unsigned integer.
        /// </summary>
        public static ulong CalcChecksum(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            const uint prime = 65521; // largest prime smaller than 65536
            uint s1 = 0;
            uint s2 = 0;
            int length = buffer.Length;
            int offset = 0;
            while (length > 0)
            {
                int n = 3800;
                if (n > length)
                    n = length;
                length -= n;
                while (--n >= 0)
                {
                    s1 += buffer[offset++];
                    s2 = s2 + s1;
                }
                s1 %= prime;
                s2 %= prime;
            }
            //return ((ulong)((ulong)(((ulong)s2 << 16) | (ulong)s1)) << 32) | (ulong)buffer.Length;
            ulong ul1 = (ulong)s2 << 16;
            ul1 = ul1 | s1;
            ulong ul2 = (ulong)buffer.Length;
            return (ul1 << 32) | ul2;
        }

        public static XFontStyle CreateStyle(bool isBold, bool isItalic)
        {
            return (isBold ? XFontStyle.Bold : 0) | (isItalic ? XFontStyle.Italic : 0);
        }
    }
}
