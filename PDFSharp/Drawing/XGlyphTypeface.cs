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
#if CORE || GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiFontFamily = System.Drawing.FontFamily;
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
using WpfStyleSimulations = System.Windows.Media.StyleSimulations;
#endif
#if UWP
using Windows.UI.Xaml.Media;
#endif
using PdfSharp.Fonts;
using PdfSharp.Fonts.OpenType;

#pragma warning disable 649
#if SILVERLIGHT
#pragma warning disable 219
#endif
#if NETFX_CORE
#pragma warning disable 649
#endif

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Specifies a physical font face that corresponds to a font file on the disk or in memory.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    internal sealed class XGlyphTypeface
    {
        // Implementation Notes
        // XGlyphTypeface is the centerpiece for font management. There is a one to one relationship
        // between XFont an XGlyphTypeface.
        //
        // * Each XGlyphTypeface can belong to one or more XFont objects.
        // * An XGlyphTypeface hold an XFontFamily.
        // * XGlyphTypeface hold a reference to an OpenTypeFontface. 
        // * 
        //

        const string KeyPrefix = "tk:";  // "typeface key"

#if CORE || GDI
        XGlyphTypeface(string key, XFontFamily fontFamily, XFontSource fontSource, XStyleSimulations styleSimulations, GdiFont gdiFont)
        {
            _key = key;
            _fontFamily = fontFamily;
            _fontSource = fontSource;

            _fontface = OpenTypeFontface.CetOrCreateFrom(fontSource);
            Debug.Assert(ReferenceEquals(_fontSource.Fontface, _fontface));

            _gdiFont = gdiFont;

            _styleSimulations = styleSimulations;
            Initialize();
        }
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XGlyphTypeface"/> class by a font source.
        /// </summary>
        public XGlyphTypeface(XFontSource fontSource)
        {
            string familyName = fontSource.Fontface.name.Name;
            _fontFamily = new XFontFamily(familyName, false);
            _fontface = fontSource.Fontface;
            _isBold = _fontface.os2.IsBold;
            _isItalic = _fontface.os2.IsItalic;

            _key = ComputeKey(familyName, _isBold, _isItalic);
            //_fontFamily =xfont  FontFamilyCache.GetFamilyByName(familyName);
            _fontSource = fontSource;

            Initialize();
        }
#endif

#if WPF
        XGlyphTypeface(string key, XFontFamily fontFamily, XFontSource fontSource, XStyleSimulations styleSimulations, WpfTypeface wpfTypeface, WpfGlyphTypeface wpfGlyphTypeface)
        {
            _key = key;
            _fontFamily = fontFamily;
            _fontSource = fontSource;
            _styleSimulations = styleSimulations;

            _fontface = OpenTypeFontface.CetOrCreateFrom(fontSource);
            Debug.Assert(ReferenceEquals(_fontSource.Fontface, _fontface));

            _wpfTypeface = wpfTypeface;
            _wpfGlyphTypeface = wpfGlyphTypeface;

            Initialize();
        }
#endif

#if NETFX_CORE || UWP
        XGlyphTypeface(string key, XFontFamily fontFamily, XFontSource fontSource, XStyleSimulations styleSimulations)
        {
            _key = key;
            _fontFamily = fontFamily;
            _fontSource = fontSource;
            _styleSimulations = styleSimulations;

            _fontface = OpenTypeFontface.CetOrCreateFrom(fontSource);
            Debug.Assert(ReferenceEquals(_fontSource.Fontface, _fontface));

            //_wpfTypeface = wpfTypeface;
            //_wpfGlyphTypeface = wpfGlyphTypeface;

            Initialize();
        }
#endif

        public static XGlyphTypeface GetOrCreateFrom(string familyName, FontResolvingOptions fontResolvingOptions)
        {
            // Check cache for requested type face.
            string typefaceKey = ComputeKey(familyName, fontResolvingOptions);
            XGlyphTypeface glyphTypeface;
            if (GlyphTypefaceCache.TryGetGlyphTypeface(typefaceKey, out glyphTypeface))
            {
                // Just return existing one.
                return glyphTypeface;
            }

            // Resolve typeface by FontFactory.
            FontResolverInfo fontResolverInfo = FontFactory.ResolveTypeface(familyName, fontResolvingOptions, typefaceKey);
            if (fontResolverInfo == null)
            {
                // No fallback - just stop.
                throw new InvalidOperationException("No appropriate font found.");
            }

#if CORE || GDI
            GdiFont gdiFont = null;
#endif
#if WPF
            WpfFontFamily wpfFontFamily = null;
            WpfTypeface wpfTypeface = null;
            WpfGlyphTypeface wpfGlyphTypeface = null;
#endif
#if UWP
            // Nothing to do.
#endif
            // Now create the font family at the first.
            XFontFamily fontFamily;
            PlatformFontResolverInfo platformFontResolverInfo = fontResolverInfo as PlatformFontResolverInfo;
            if (platformFontResolverInfo != null)
            {
#if CORE || GDI
                // $TODO THHO Lock???
                // Reuse GDI+ font from platform font resolver.
                gdiFont = platformFontResolverInfo.GdiFont;
                fontFamily = XFontFamily.GetOrCreateFromGdi(gdiFont);
#endif
#if WPF
#if !SILVERLIGHT
                // Reuse WPF font family created from platform font resolver.
                wpfFontFamily = platformFontResolverInfo.WpfFontFamily;
                wpfTypeface = platformFontResolverInfo.WpfTypeface;
                wpfGlyphTypeface = platformFontResolverInfo.WpfGlyphTypeface;
                fontFamily = XFontFamily.GetOrCreateFromWpf(wpfFontFamily);
#else
                fontFamily = XFontFamily.GetOrCreateFromWpf(new WpfFontFamily(familyName));
#endif
#endif
#if NETFX_CORE || UWP
                fontFamily = null;
#endif
            }
            else
            {
                // Create new and exclusively used font family for custom font resolver retrieved font source.
                fontFamily = XFontFamily.CreateSolitary(fontResolverInfo.FaceName);
            }

            // We have a valid font resolver info. That means we also have an XFontSource object loaded in the cache.
            ////XFontSource fontSource = FontFactory.GetFontSourceByTypefaceKey(fontResolverInfo.FaceName);
            XFontSource fontSource = FontFactory.GetFontSourceByFontName(fontResolverInfo.FaceName);
            Debug.Assert(fontSource != null);

            // Each font source already contains its OpenTypeFontface.
#if CORE || GDI
            glyphTypeface = new XGlyphTypeface(typefaceKey, fontFamily, fontSource, fontResolverInfo.StyleSimulations, gdiFont);
#endif
#if WPF
            glyphTypeface = new XGlyphTypeface(typefaceKey, fontFamily, fontSource, fontResolverInfo.StyleSimulations, wpfTypeface, wpfGlyphTypeface);
#endif
#if NETFX_CORE || UWP
            glyphTypeface = new XGlyphTypeface(typefaceKey, fontFamily, fontSource, fontResolverInfo.StyleSimulations);
#endif
            GlyphTypefaceCache.AddGlyphTypeface(glyphTypeface);

            return glyphTypeface;
        }

#if CORE || GDI
        public static XGlyphTypeface GetOrCreateFromGdi(GdiFont gdiFont)
        {
            // $TODO THHO Lock???
            string typefaceKey = ComputeKey(gdiFont);
            XGlyphTypeface glyphTypeface;
            if (GlyphTypefaceCache.TryGetGlyphTypeface(typefaceKey, out glyphTypeface))
            {
                // We have the glyph typeface already in cache.
                return glyphTypeface;
            }

            XFontFamily fontFamily = XFontFamily.GetOrCreateFromGdi(gdiFont);
            XFontSource fontSource = XFontSource.GetOrCreateFromGdi(typefaceKey, gdiFont);

            // Check if styles must be simulated.
            XStyleSimulations styleSimulations = XStyleSimulations.None;
            if (gdiFont.Bold && !fontSource.Fontface.os2.IsBold)
                styleSimulations |= XStyleSimulations.BoldSimulation;
            if (gdiFont.Italic && !fontSource.Fontface.os2.IsItalic)
                styleSimulations |= XStyleSimulations.ItalicSimulation;

            glyphTypeface = new XGlyphTypeface(typefaceKey, fontFamily, fontSource, styleSimulations, gdiFont);
            GlyphTypefaceCache.AddGlyphTypeface(glyphTypeface);

            return glyphTypeface;
        }
#endif

#if WPF && !SILVERLIGHT
        public static XGlyphTypeface GetOrCreateFromWpf(WpfTypeface wpfTypeface)
        {
#if DEBUG
            if (wpfTypeface.FontFamily.Source == "Segoe UI Semilight")
                wpfTypeface.GetType();
#endif
            //string typefaceKey = ComputeKey(wpfTypeface);
            //XGlyphTypeface glyphTypeface;
            //if (GlyphTypefaceCache.TryGetGlyphTypeface(typefaceKey, out glyphTypeface))
            //{
            //    // We have the glyph typeface already in cache.
            //    return glyphTypeface;
            //}

            // Create WPF glyph typeface.
            WpfGlyphTypeface wpfGlyphTypeface;
            if (!wpfTypeface.TryGetGlyphTypeface(out wpfGlyphTypeface))
                return null;

            string typefaceKey = ComputeKey(wpfGlyphTypeface);

            string name1 = wpfGlyphTypeface.DesignerNames[FontHelper.CultureInfoEnUs];
            string name2 = wpfGlyphTypeface.FaceNames[FontHelper.CultureInfoEnUs];
            string name3 = wpfGlyphTypeface.FamilyNames[FontHelper.CultureInfoEnUs];
            string name4 = wpfGlyphTypeface.ManufacturerNames[FontHelper.CultureInfoEnUs];
            string name5 = wpfGlyphTypeface.Win32FaceNames[FontHelper.CultureInfoEnUs];
            string name6 = wpfGlyphTypeface.Win32FamilyNames[FontHelper.CultureInfoEnUs];

            XGlyphTypeface glyphTypeface;
            if (GlyphTypefaceCache.TryGetGlyphTypeface(typefaceKey, out glyphTypeface))
            {
                // We have the glyph typeface already in cache.
                return glyphTypeface;
            }

            XFontFamily fontFamily = XFontFamily.GetOrCreateFromWpf(wpfTypeface.FontFamily);
            XFontSource fontSource = XFontSource.GetOrCreateFromWpf(typefaceKey, wpfGlyphTypeface);

            glyphTypeface = new XGlyphTypeface(typefaceKey, fontFamily, fontSource,
                (XStyleSimulations)wpfGlyphTypeface.StyleSimulations,
                wpfTypeface, wpfGlyphTypeface);
            GlyphTypefaceCache.AddGlyphTypeface(glyphTypeface);

            return glyphTypeface;
        }
#endif

        public XFontFamily FontFamily
        {
            get { return _fontFamily; }
        }
        readonly XFontFamily _fontFamily;

        internal OpenTypeFontface Fontface
        {
            get { return _fontface; }
        }
        readonly OpenTypeFontface _fontface;

        public XFontSource FontSource
        {
            get { return _fontSource; }
        }
        readonly XFontSource _fontSource;






















        void Initialize()
        {
            _familyName = _fontface.name.Name;
            if (string.IsNullOrEmpty(_faceName) || _faceName.StartsWith("?"))
                _faceName = _familyName;
            _styleName = _fontface.name.Style;
            _displayName = _fontface.name.FullFontName;
            if (string.IsNullOrEmpty(_displayName))
            {
                _displayName = _familyName;
                if (string.IsNullOrEmpty(_styleName))
                    _displayName += " (" + _styleName + ")";
            }

            // Bold, as defined in OS/2 table.
            _isBold = _fontface.os2.IsBold;
            // Debug.Assert(_isBold == (_fontface.os2.usWeightClass > 400), "Check font weight.");

            // Italic, as defined in OS/2 table.
            _isItalic = _fontface.os2.IsItalic;
        }

        /// <summary>
        /// Gets the name of the font face. This can be a file name, an uri, or a GUID.
        /// </summary>
        internal string FaceName
        {
            get { return _faceName; }
        }
        string _faceName;

        /// <summary>
        /// Gets the English family name of the font, for example "Arial".
        /// </summary>
        public string FamilyName
        {
            get { return _familyName; }
        }
        string _familyName;

        /// <summary>
        /// Gets the English subfamily name of the font,
        /// for example "Bold".
        /// </summary>
        public string StyleName
        {
            get { return _styleName; }
        }
        string _styleName;

        /// <summary>
        /// Gets the English display name of the font,
        /// for example "Arial italic".
        /// </summary>
        public string DisplayName
        {
            get { return _displayName; }
        }
        string _displayName;

        /// <summary>
        /// Gets a value indicating whether the font weight is bold.
        /// </summary>
        public bool IsBold
        {
            get { return _isBold; }
        }
        bool _isBold;

        /// <summary>
        /// Gets a value indicating whether the font style is italic.
        /// </summary>
        public bool IsItalic
        {
            get { return _isItalic; }
        }
        bool _isItalic;

        public XStyleSimulations StyleSimulations
        {
            get { return _styleSimulations; }
        }
        XStyleSimulations _styleSimulations;

        /// <summary>
        /// Gets the suffix of the face name in a PDF font and font descriptor.
        /// The name based on the effective value of bold and italic from the OS/2 table.
        /// </summary>
        string GetFaceNameSuffix()
        {
            // Use naming of Microsoft Word.
            if (IsBold)
                return IsItalic ? ",BoldItalic" : ",Bold";
            return IsItalic ? ",Italic" : "";
        }

        internal string GetBaseName()
        {
            string name = DisplayName;
            int ich = name.IndexOf("bold", StringComparison.OrdinalIgnoreCase);
            if (ich > 0)
                name = name.Substring(0, ich) + name.Substring(ich + 4, name.Length - ich - 4);
            ich = name.IndexOf("italic", StringComparison.OrdinalIgnoreCase);
            if (ich > 0)
                name = name.Substring(0, ich) + name.Substring(ich + 6, name.Length - ich - 6);
            //name = name.Replace(" ", "");
            name = name.Trim();
            name += GetFaceNameSuffix();
            return name;
        }

        /// <summary>
        /// Computes the bijective key for a typeface.
        /// </summary>
        internal static string ComputeKey(string familyName, FontResolvingOptions fontResolvingOptions)
        {
            // Compute a human readable key.
            string simulationSuffix = "";
            if (fontResolvingOptions.OverrideStyleSimulations)
            {
                switch (fontResolvingOptions.StyleSimulations)
                {
                    case XStyleSimulations.BoldSimulation: simulationSuffix = "|b+/i-"; break;
                    case XStyleSimulations.ItalicSimulation: simulationSuffix = "|b-/i+"; break;
                    case XStyleSimulations.BoldItalicSimulation: simulationSuffix = "|b+/i+"; break;
                    case XStyleSimulations.None: break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            string key = KeyPrefix + familyName.ToLowerInvariant()
                + (fontResolvingOptions.IsItalic ? "/i" : "/n") // normal / oblique / italic  
                + (fontResolvingOptions.IsBold ? "/700" : "/400") + "/5" // Stretch.Normal
                + simulationSuffix;
            return key;
        }

        /// <summary>
        /// Computes the bijective key for a typeface.
        /// </summary>
        internal static string ComputeKey(string familyName, bool isBold, bool isItalic)
        {
            return ComputeKey(familyName, new FontResolvingOptions(FontHelper.CreateStyle(isBold, isItalic)));
        }

#if CORE || GDI
        internal static string ComputeKey(GdiFont gdiFont)
        {
            string name1 = gdiFont.Name;
            string name2 = gdiFont.OriginalFontName;
            string name3 = gdiFont.SystemFontName;

            string name = name1;
            GdiFontStyle style = gdiFont.Style;

            string key = KeyPrefix + name.ToLowerInvariant() + ((style & GdiFontStyle.Italic) == GdiFontStyle.Italic ? "/i" : "/n") + ((style & GdiFontStyle.Bold) == GdiFontStyle.Bold ? "/700" : "/400") + "/5"; // Stretch.Normal
            return key;
        }
#endif
#if WPF && !SILVERLIGHT
        internal static string ComputeKey(WpfGlyphTypeface wpfGlyphTypeface)
        {
            string name1 = wpfGlyphTypeface.DesignerNames[FontHelper.CultureInfoEnUs];
            string faceName = wpfGlyphTypeface.FaceNames[FontHelper.CultureInfoEnUs];
            string familyName = wpfGlyphTypeface.FamilyNames[FontHelper.CultureInfoEnUs];
            string name4 = wpfGlyphTypeface.ManufacturerNames[FontHelper.CultureInfoEnUs];
            string name5 = wpfGlyphTypeface.Win32FaceNames[FontHelper.CultureInfoEnUs];
            string name6 = wpfGlyphTypeface.Win32FamilyNames[FontHelper.CultureInfoEnUs];


            string name = familyName.ToLower() + '/' + faceName.ToLowerInvariant();
            string style = wpfGlyphTypeface.Style.ToString();
            string weight = wpfGlyphTypeface.Weight.ToString();
            string stretch = wpfGlyphTypeface.Stretch.ToString();
            string simulations = wpfGlyphTypeface.StyleSimulations.ToString();

            //string key = name + '/' + style + '/' + weight + '/' + stretch + '/' + simulations;

            string key = KeyPrefix + name + '/' + style.Substring(0, 1) + '/' + wpfGlyphTypeface.Weight.ToOpenTypeWeight().ToString(CultureInfo.InvariantCulture) + '/' + wpfGlyphTypeface.Stretch.ToOpenTypeStretch().ToString(CultureInfo.InvariantCulture);
            switch (wpfGlyphTypeface.StyleSimulations)
            {
                case WpfStyleSimulations.BoldSimulation: key += "|b+/i-"; break;
                case WpfStyleSimulations.ItalicSimulation: key += "|b-/i+"; break;
                case WpfStyleSimulations.BoldItalicSimulation: key += "|b+/i+"; break;
                case WpfStyleSimulations.None: break;
            }
            return key.ToLowerInvariant();
        }
#endif

        public string Key
        {
            get { return _key; }
        }
        readonly string _key;

#if CORE || GDI
        internal GdiFont GdiFont
        {
            get { return _gdiFont; }
        }

        private readonly GdiFont _gdiFont;
#endif

#if WPF
        internal WpfTypeface WpfTypeface
        {
            get { return _wpfTypeface; }
        }
        readonly WpfTypeface _wpfTypeface;

        internal WpfGlyphTypeface WpfGlyphTypeface
        {
            get { return _wpfGlyphTypeface; }
        }
        readonly WpfGlyphTypeface _wpfGlyphTypeface;
#endif

#if SILVERLIGHT_
    /// <summary>
    /// Gets the FontSource object used in Silverlight 4.
    /// </summary>
        public FontSource FontSource
        {
            get
            {
                if (_fontSource == null)
                {
#if true
                    MemoryStream stream = new MemoryStream(_fontface.FontData.Bytes);
                    _fontSource = new FontSource(stream);
#else
                    using (MemoryStream stream = new MemoryStream(_fontface.Data))
                    {
                        _fontSource = new FontSource(stream);
                    }
#endif
                }
                return _fontSource;
            }
        }
        FontSource _fontSource;
#endif

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        internal string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0} - {1} ({2})", FamilyName, StyleName, FaceName); }
        }
    }
}
