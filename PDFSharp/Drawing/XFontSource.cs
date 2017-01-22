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
using System.IO;
using System.Runtime.InteropServices;
using PdfSharp.Fonts;
#if CORE || GDI
using GdiFont = System.Drawing.Font;
using GdiFontStyle = System.Drawing.FontStyle;
#endif
#if WPF
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfTypeface = System.Windows.Media.Typeface;
using WpfGlyphTypeface = System.Windows.Media.GlyphTypeface;
#endif
using PdfSharp.Internal;
using PdfSharp.Fonts.OpenType;

namespace PdfSharp.Drawing
{
    /// <summary>
    /// The bytes of a font file.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    internal class XFontSource
    {
        // Implementation Notes
        // 
        // * XFontSource represents a single font (file) in memory.
        // * An XFontSource hold a reference to it OpenTypeFontface.
        // * To prevent large heap fragmentation this class must exists only once.
        // * TODO: ttcf

        // Signature of a true type collection font.
        const uint ttcf = 0x66637474;

        XFontSource(byte[] bytes, ulong key)
        {
            _fontName = null;
            _bytes = bytes;
            _key = key;
        }

        /// <summary>
        /// Gets an existing font source or creates a new one.
        /// A new font source is cached in font factory.
        /// </summary>
        public static XFontSource GetOrCreateFrom(byte[] bytes)
        {
            ulong key = FontHelper.CalcChecksum(bytes);
            XFontSource fontSource;
            if (!FontFactory.TryGetFontSourceByKey(key, out fontSource))
            {
                fontSource = new XFontSource(bytes, key);
                // Theoretically the font source could be created by a differend thread in the meantime.
                fontSource = FontFactory.CacheFontSource(fontSource);
            }
            return fontSource;
        }

#if CORE || GDI
        internal static XFontSource GetOrCreateFromGdi(string typefaceKey, GdiFont gdiFont)
        {
            byte[] bytes = ReadFontBytesFromGdi(gdiFont);
            XFontSource fontSource = GetOrCreateFrom(typefaceKey, bytes);
            return fontSource;
        }

        static byte[] ReadFontBytesFromGdi(GdiFont gdiFont)
        {
            // Weird: LastError is always 123 or 127. Comment out Debug.Assert.
            int error = Marshal.GetLastWin32Error();
            //Debug.Assert(error == 0);
            error = Marshal.GetLastWin32Error();
            //Debug.Assert(error == 0);

            IntPtr hfont = gdiFont.ToHfont();
#if true
            IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
#else
            NativeMethods.LOGFONT logFont = new NativeMethods.LOGFONT();
            logFont.lfHeight = 30;
            logFont.lfWidth = 0;
            logFont.lfEscapement = 0;
            logFont.lfOrientation = 0;
            logFont.lfWeight = 400;
            logFont.lfItalic = 0;
            logFont.lfUnderline = 0;
            logFont.lfStrikeOut = 0;
            logFont.lfCharSet = 0;
            logFont.lfOutPrecision = 0;
            logFont.lfClipPrecision = 0;
            logFont.lfQuality = 0;
            logFont.lfPitchAndFamily = 0;
            logFont.lfFaceName = "Arial";

            gdiFont.ToLogFont(logFont);

            hfont = NativeMethods.CreateFontIndirect(logFont);


            // IntPtr hdc = NativeMethods.CreateDC("DISPLAY", null, null, IntPtr.Zero);
            IntPtr hdc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
#endif
            error = Marshal.GetLastWin32Error();
            //Debug.Assert(error == 0);

            IntPtr oldFont = NativeMethods.SelectObject(hdc, hfont);
            error = Marshal.GetLastWin32Error();
            //Debug.Assert(error == 0);

            // Get size of the font file.
            bool isTtcf = false;
            // In Azure I get 0xc0000022
            int size = NativeMethods.GetFontData(hdc, 0, 0, null, 0);

            // Check for ntstatus.h: #define STATUS_ACCESS_DENIED             ((NTSTATUS)0xC0000022L)
            if ((uint)size == 0xc0000022)
                throw new InvalidOperationException("Microsoft Azure returns STATUS_ACCESS_DENIED ((NTSTATUS)0xC0000022L) from GetFontData. This is a bug in Azure. You must implement a FontResolver to circumvent this issue.");

            if (size == NativeMethods.GDI_ERROR)
            {
                // Assume that the font file is a true type collection.
                size = NativeMethods.GetFontData(hdc, ttcf, 0, null, 0);
                isTtcf = true;
            }
            error = Marshal.GetLastWin32Error();
            //Debug.Assert(error == 0);

            if (size == 0)
                throw new InvalidOperationException("Cannot retrieve font data.");

            byte[] bytes = new byte[size];
            int effectiveSize = NativeMethods.GetFontData(hdc, isTtcf ? ttcf : 0, 0, bytes, size);
            Debug.Assert(size == effectiveSize);
            // Clean up.
            NativeMethods.SelectObject(hdc, oldFont);
            NativeMethods.ReleaseDC(IntPtr.Zero, hdc);

            return bytes;
        }
#endif

#if WPF && !SILVERLIGHT
        internal static XFontSource GetOrCreateFromWpf(string typefaceKey, WpfGlyphTypeface wpfGlyphTypeface)
        {
            byte[] bytes = ReadFontBytesFromWpf(wpfGlyphTypeface);
            XFontSource fontSource = GetOrCreateFrom(typefaceKey, bytes);
            return fontSource;
        }

        internal static byte[] ReadFontBytesFromWpf(WpfGlyphTypeface wpfGlyphTypeface)
        {
            using (Stream fontStream = wpfGlyphTypeface.GetFontStream())
            {
                if (fontStream == null)
                    throw new InvalidOperationException("Cannot retrieve font data.");
                int size = (int)fontStream.Length;
                byte[] bytes = new byte[size];
                fontStream.Read(bytes, 0, size);
                return bytes;
            }
        }
#endif

        static XFontSource GetOrCreateFrom(string typefaceKey, byte[] fontBytes)
        {
            XFontSource fontSource;
            ulong key = FontHelper.CalcChecksum(fontBytes);
            if (FontFactory.TryGetFontSourceByKey(key, out fontSource))
            {
                // The font source already exists, but is not yet cached under the specified typeface key.
                FontFactory.CacheExistingFontSourceWithNewTypefaceKey(typefaceKey, fontSource);
            }
            else
            {
                // No font source exists. Create new one and cache it.
                fontSource = new XFontSource(fontBytes, key);
                FontFactory.CacheNewFontSource(typefaceKey, fontSource);
            }
            return fontSource;
        }

        public static XFontSource CreateCompiledFont(byte[] bytes)
        {
            XFontSource fontSource = new XFontSource(bytes, 0);
            return fontSource;
        }

        /// <summary>
        /// Gets or sets the fontface.
        /// </summary>
        internal OpenTypeFontface Fontface
        {
            get { return _fontface; }
            set
            {
                _fontface = value;
                _fontName = value.name.FullFontName;
            }
        }
        OpenTypeFontface _fontface;

        /// <summary>
        /// Gets the key that uniquely identifies this font source.
        /// </summary>
        internal ulong Key
        {
            get
            {
                if (_key == 0)
                    _key = FontHelper.CalcChecksum(Bytes);
                return _key;
            }
        }
        ulong _key;

        public void IncrementKey()
        {
            // HACK: Depends on implementation of CalcChecksum.
            // Increment check sum and keep length untouched.
            _key += 1ul << 32;
        }

        /// <summary>
        /// Gets the name of the font's name table.
        /// </summary>
        public string FontName
        {
            get { return _fontName; }
        }
        string _fontName;

        /// <summary>
        /// Gets the bytes of the font.
        /// </summary>
        public byte[] Bytes
        {
            get { return _bytes; }
        }
        readonly byte[] _bytes;

        public override int GetHashCode()
        {
            return (int)((Key >> 32) ^ Key);
        }

        public override bool Equals(object obj)
        {
            XFontSource fontSource = obj as XFontSource;
            if (fontSource == null)
                return false;
            return Key == fontSource.Key;
        }

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSha rper disable UnusedMember.Local
        internal string DebuggerDisplay
        // ReShar per restore UnusedMember.Local
        {
            // The key is converted to a value a human can remember during debugging.
            get { return String.Format(CultureInfo.InvariantCulture, "XFontSource: '{0}', keyhash={1}", FontName, Key % 99991 /* largest prime number less than 100000 */); }
        }
    }
}
