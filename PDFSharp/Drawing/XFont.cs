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

// #??? Clean up

using System;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;
#if CORE || GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using GdiFontFamily = System.Drawing.FontFamily;
using GdiFont = System.Drawing.Font;
using GdiFontStyle = System.Drawing.FontStyle;
#endif
#if WPF
using System.Windows.Markup;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfTypeface = System.Windows.Media.Typeface;
using WpfGlyphTypeface = System.Windows.Media.GlyphTypeface;
#endif
#if UWP
using UwpFontFamily = Windows.UI.Xaml.Media.FontFamily;
#endif
using PdfSharp.Fonts;
using PdfSharp.Fonts.OpenType;
using PdfSharp.Internal;
using PdfSharp.Pdf;

#if SILVERLIGHT
#pragma warning disable 649
#endif
// ReSharper disable ConvertToAutoProperty

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Defines an object used to draw text.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public sealed class XFont
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class.
        /// </summary>
        /// <param name="familyName">Name of the font family.</param>
        /// <param name="emSize">The em size.</param>
        public XFont(string familyName, double emSize)
            : this(familyName, emSize, XFontStyle.Regular, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class.
        /// </summary>
        /// <param name="familyName">Name of the font family.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        public XFont(string familyName, double emSize, XFontStyle style)
            : this(familyName, emSize, style, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class.
        /// </summary>
        /// <param name="familyName">Name of the font family.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        /// <param name="pdfOptions">Additional PDF options.</param>
        public XFont(string familyName, double emSize, XFontStyle style, XPdfFontOptions pdfOptions)
        {
            _familyName = familyName;
            _emSize = emSize;
            _style = style;
            _pdfOptions = pdfOptions;
            Initialize();
        }

        internal XFont(string familyName, double emSize, XFontStyle style, XPdfFontOptions pdfOptions, XStyleSimulations styleSimulations)
        {
            _familyName = familyName;
            _emSize = emSize;
            _style = style;
            _pdfOptions = pdfOptions;
            OverrideStyleSimulations = true;
            StyleSimulations = styleSimulations;
            Initialize();
        }

#if CORE || GDI
        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.FontFamily.
        /// </summary>
        /// <param name="fontFamily">The System.Drawing.FontFamily.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        public XFont(GdiFontFamily fontFamily, double emSize, XFontStyle style)
            : this(fontFamily, emSize, style, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.FontFamily.
        /// </summary>
        /// <param name="fontFamily">The System.Drawing.FontFamily.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        /// <param name="pdfOptions">Additional PDF options.</param>
        public XFont(GdiFontFamily fontFamily, double emSize, XFontStyle style, XPdfFontOptions pdfOptions)
        {
            _familyName = fontFamily.Name;
            _gdiFontFamily = fontFamily;
            _emSize = emSize;
            _style = style;
            _pdfOptions = pdfOptions;
            InitializeFromGdi();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.Font.
        /// </summary>
        /// <param name="font">The System.Drawing.Font.</param>
        public XFont(GdiFont font)
            : this(font, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.Font.
        /// </summary>
        /// <param name="font">The System.Drawing.Font.</param>
        /// <param name="pdfOptions">Additional PDF options.</param>
        public XFont(GdiFont font, XPdfFontOptions pdfOptions)
        {
            if (font.Unit != GraphicsUnit.World)
                throw new ArgumentException("Font must use GraphicsUnit.World.");
            _gdiFont = font;
            Debug.Assert(font.Name == font.FontFamily.Name);
            _familyName = font.Name;
            _emSize = font.Size;
            _style = FontStyleFrom(font);
            _pdfOptions = pdfOptions;
            InitializeFromGdi();
        }
#endif

#if WPF && !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Windows.Media.FontFamily.
        /// </summary>
        /// <param name="fontFamily">The System.Windows.Media.FontFamily.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        public XFont(WpfFontFamily fontFamily, double emSize, XFontStyle style)
            : this(fontFamily, emSize, style, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.FontFamily.
        /// </summary>
        /// <param name="fontFamily">The System.Windows.Media.FontFamily.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        /// <param name="pdfOptions">Additional PDF options.</param>
        public XFont(WpfFontFamily fontFamily, double emSize, XFontStyle style, XPdfFontOptions pdfOptions)
        {
#if !SILVERLIGHT
            _familyName = fontFamily.FamilyNames[XmlLanguage.GetLanguage("en-US")];
#else
            // Best we can do in Silverlight.
            _familyName = fontFamily.Source;
#endif
            _wpfFontFamily = fontFamily;
            _emSize = emSize;
            _style = style;
            _pdfOptions = pdfOptions;
            InitializeFromWpf();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont" /> class from a System.Windows.Media.Typeface.
        /// </summary>
        /// <param name="typeface">The System.Windows.Media.Typeface.</param>
        /// <param name="emSize">The em size.</param>
        public XFont(WpfTypeface typeface, double emSize)
            : this(typeface, emSize, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Windows.Media.Typeface.
        /// </summary>
        /// <param name="typeface">The System.Windows.Media.Typeface.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="pdfOptions">Additional PDF options.</param>
        public XFont(WpfTypeface typeface, double emSize, XPdfFontOptions pdfOptions)
        {
            _wpfTypeface = typeface;
            //Debug.Assert(font.Name == font.FontFamily.Name);
            //_familyName = font.Name;
            _emSize = emSize;
            _pdfOptions = pdfOptions;
            InitializeFromWpf();
        }
#endif

#if UWP_
        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.FontFamily.
        /// </summary>
        /// <param name="fontFamily">The System.Drawing.FontFamily.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        public XFont(UwpFontFamily fontFamily, double emSize, XFontStyle style)
            : this(fontFamily, emSize, style, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.FontFamily.
        /// </summary>
        /// <param name="fontFamily">The System.Drawing.FontFamily.</param>
        /// <param name="emSize">The em size.</param>
        /// <param name="style">The font style.</param>
        /// <param name="pdfOptions">Additional PDF options.</param>
        public XFont(UwpFontFamily fontFamily, double emSize, XFontStyle style, XPdfFontOptions pdfOptions)
        {
            _familyName = fontFamily.Source;
            _gdiFontFamily = fontFamily;
            _emSize = emSize;
            _style = style;
            _pdfOptions = pdfOptions;
            InitializeFromGdi();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.Font.
        /// </summary>
        /// <param name="font">The System.Drawing.Font.</param>
        public XFont(GdiFont font)
            : this(font, new XPdfFontOptions(GlobalFontSettings.DefaultFontEncoding))
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFont"/> class from a System.Drawing.Font.
        /// </summary>
        /// <param name="font">The System.Drawing.Font.</param>
        /// <param name="pdfOptions">Additional PDF options.</param>
        public XFont(GdiFont font, XPdfFontOptions pdfOptions)
        {
            if (font.Unit != GraphicsUnit.World)
                throw new ArgumentException("Font must use GraphicsUnit.World.");
            _gdiFont = font;
            Debug.Assert(font.Name == font.FontFamily.Name);
            _familyName = font.Name;
            _emSize = font.Size;
            _style = FontStyleFrom(font);
            _pdfOptions = pdfOptions;
            InitializeFromGdi();
        }
#endif

        //// Methods
        //public Font(Font prototype, FontStyle newStyle);
        //public Font(FontFamily family, float emSize);
        //public Font(string familyName, float emSize);
        //public Font(FontFamily family, float emSize, FontStyle style);
        //public Font(FontFamily family, float emSize, GraphicsUnit unit);
        //public Font(string familyName, float emSize, FontStyle style);
        //public Font(string familyName, float emSize, GraphicsUnit unit);
        //public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit);
        //public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit);
        ////public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet);
        ////public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet);
        ////public Font(FontFamily family, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont);
        ////public Font(string familyName, float emSize, FontStyle style, GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont);
        //public object Clone();
        //private static FontFamily CreateFontFamilyWithFallback(string familyName);
        //private void Dispose(bool disposing);
        //public override bool Equals(object obj);
        //protected override void Finalize();
        //public static Font FromHdc(IntPtr hdc);
        //public static Font FromHfont(IntPtr hfont);
        //public static Font FromLogFont(object lf);
        //public static Font FromLogFont(object lf, IntPtr hdc);
        //public override int GetHashCode();

        /// <summary>
        /// Initializes this instance by computing the glyph typeface, font family, font source and TrueType fontface.
        /// (PDFsharp currently only deals with TrueType fonts.)
        /// </summary>
        void Initialize()
        {
#if DEBUG
            if (_familyName == "Segoe UI Semilight" && (_style & XFontStyle.BoldItalic) == XFontStyle.Italic)
                GetType();
#endif

            FontResolvingOptions fontResolvingOptions = OverrideStyleSimulations
                ? new FontResolvingOptions(_style, StyleSimulations)
                : new FontResolvingOptions(_style);

            // HACK: 'PlatformDefault' is used in unit test code.
            if (StringComparer.OrdinalIgnoreCase.Compare(_familyName, GlobalFontSettings.DefaultFontName) == 0)
            {
#if CORE || GDI || WPF
                _familyName = "Calibri";
#endif
            }

            // In principle an XFont is an XGlyphTypeface plus an em-size.
            _glyphTypeface = XGlyphTypeface.GetOrCreateFrom(_familyName, fontResolvingOptions);
#if GDI  // TODO: In CORE build it is not necessary to create a GDI font at all
            // Create font by using font family.
            XFontSource fontSource;  // Not needed here.
            _gdiFont = FontHelper.CreateFont(_familyName, (float)_emSize, (GdiFontStyle)(_style & XFontStyle.BoldItalic), out fontSource);
#endif
#if WPF && !SILVERLIGHT  // Pure WPF
            _wpfFontFamily = _glyphTypeface.FontFamily.WpfFamily;
            _wpfTypeface = _glyphTypeface.WpfTypeface;

            if (_wpfFontFamily == null)
                _wpfFontFamily = new WpfFontFamily(Name);

            if (_wpfTypeface == null)
                _wpfTypeface = FontHelper.CreateTypeface(WpfFontFamily, _style);
#endif
#if WPF && SILVERLIGHT_  // Pure Silverlight 5
            if (GlyphTypeface == null)
            {
                //Debug.Assert(Typeface == null);
                // #P F C
                //GlyphTypeface = XPrivateFontCollection.TryGetXGlyphTypeface(Name, _style);
                //if (GlyphTypeface == null)
                //{
                //    // HACK: Just make it work...
                //    GlyphTypeface = GlobalFontSettings.TryGetXGlyphTypeface(Name, _style, out Data);
                //}
#if DEBUG
                if (GlyphTypeface == null)
                    throw new Exception("No font: " + Name);
#endif
                _wpfFamily = GlyphTypeface.FontFamily;
            }

            //if (Family == null)
            //  Family = new System.Windows.Media.FontFamily(Name);

            //if (Typeface == null)
            //  Typeface = FontHelper.CreateTypeface(Family, _style);
#endif
            CreateDescriptorAndInitializeFontMetrics();
        }

#if CORE || GDI
        /// <summary>
        /// A GDI+ font object is used to setup the internal font objects.
        /// </summary>
        void InitializeFromGdi()
        {
            try
            {
                Lock.EnterFontFactory();
                if (_gdiFontFamily != null)
                {
                    // Create font based on its family.
                    _gdiFont = new Font(_gdiFontFamily, (float)_emSize, (GdiFontStyle)_style, GraphicsUnit.World);
                }

                if (_gdiFont != null)
                {
#if DEBUG_
                    string name1 = _gdiFont.Name;
                    string name2 = _gdiFont.OriginalFontName;
                    string name3 = _gdiFont.SystemFontName;
#endif
                    _familyName = _gdiFont.FontFamily.Name;
                    // TODO: _glyphTypeface = XGlyphTypeface.GetOrCreateFrom(_gdiFont);
                }
                else
                {
                    Debug.Assert(false);
                }

                if (_glyphTypeface == null)
                    _glyphTypeface = XGlyphTypeface.GetOrCreateFromGdi(_gdiFont);

                CreateDescriptorAndInitializeFontMetrics();
            }
            finally { Lock.ExitFontFactory(); }
        }
#endif

#if WPF && !SILVERLIGHT
        void InitializeFromWpf()
        {
            if (_wpfFontFamily != null)
            {
                _wpfTypeface = FontHelper.CreateTypeface(_wpfFontFamily, _style);
            }

            if (_wpfTypeface != null)
            {
                _familyName = _wpfTypeface.FontFamily.FamilyNames[XmlLanguage.GetLanguage("en-US")];
                _glyphTypeface = XGlyphTypeface.GetOrCreateFromWpf(_wpfTypeface);
            }
            else
            {
                Debug.Assert(false);
            }

            if (_glyphTypeface == null)
                _glyphTypeface = XGlyphTypeface.GetOrCreateFrom(_familyName, new FontResolvingOptions(_style));

            CreateDescriptorAndInitializeFontMetrics();
        }
#endif

        /// <summary>
        /// Code separated from Metric getter to make code easier to debug.
        /// (Setup properties in their getters caused side effects during debugging because Visual Studio calls a getter
        /// to early to show its value in a debugger window.)
        /// </summary>
        void CreateDescriptorAndInitializeFontMetrics()  // TODO: refactor
        {
            Debug.Assert(_fontMetrics == null, "InitializeFontMetrics() was already called.");
            _descriptor = (OpenTypeDescriptor)FontDescriptorCache.GetOrCreateDescriptorFor(this); //_familyName, _style, _glyphTypeface.Fontface);
            _fontMetrics = new XFontMetrics(_descriptor.FontName, _descriptor.UnitsPerEm, _descriptor.Ascender, _descriptor.Descender,
                _descriptor.Leading, _descriptor.LineSpacing, _descriptor.CapHeight, _descriptor.XHeight, _descriptor.StemV, 0, 0, 0,
                _descriptor.UnderlinePosition, _descriptor.UnderlineThickness, _descriptor.StrikeoutPosition, _descriptor.StrikeoutSize);

            XFontMetrics fm = Metrics;

            // Already done in CreateDescriptorAndInitializeFontMetrics.
            //if (_descriptor == null)
            //    _descriptor = (OpenTypeDescriptor)FontDescriptorStock.Global.CreateDescriptor(this);  //(Name, (XGdiFontStyle)Font.Style);

            UnitsPerEm = _descriptor.UnitsPerEm;
            CellAscent = _descriptor.Ascender;
            CellDescent = _descriptor.Descender;
            CellSpace = _descriptor.LineSpacing;

#if DEBUG_ && GDI
            int gdiValueUnitsPerEm = Font.FontFamily.GetEmHeight(Font.Style);
            Debug.Assert(gdiValueUnitsPerEm == UnitsPerEm);
            int gdiValueAscent = Font.FontFamily.GetCellAscent(Font.Style);
            Debug.Assert(gdiValueAscent == CellAscent);
            int gdiValueDescent = Font.FontFamily.GetCellDescent(Font.Style);
            Debug.Assert(gdiValueDescent == CellDescent);
            int gdiValueLineSpacing = Font.FontFamily.GetLineSpacing(Font.Style);
            Debug.Assert(gdiValueLineSpacing == CellSpace);
#endif
#if DEBUG_ && WPF && !SILVERLIGHT
            int wpfValueLineSpacing = (int)Math.Round(Family.LineSpacing * _descriptor.UnitsPerEm);
            Debug.Assert(wpfValueLineSpacing == CellSpace);
#endif
            Debug.Assert(fm.UnitsPerEm == _descriptor.UnitsPerEm);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Gets the XFontFamily object associated with this XFont object.
        /// </summary>
        [Browsable(false)]
        public XFontFamily FontFamily
        {
            get { return _glyphTypeface.FontFamily; }
        }

        /// <summary>
        /// WRONG: Gets the face name of this Font object.
        /// Indeed it returns the font family name.
        /// </summary>
        // [Obsolete("This function returns the font family name, not the face name. Use xxx.FontFamily.Name or xxx.FaceName")]
        public string Name
        {
            get { return _glyphTypeface.FontFamily.Name; }
        }

        internal string FaceName
        {
            get { return _glyphTypeface.FaceName; }
        }

        /// <summary>
        /// Gets the em-size of this font measured in the unit of this font object.
        /// </summary>
        public double Size
        {
            get { return _emSize; }
        }
        readonly double _emSize;

        /// <summary>
        /// Gets style information for this Font object.
        /// </summary>
        [Browsable(false)]
        public XFontStyle Style
        {
            get { return _style; }
        }
        readonly XFontStyle _style;

        /// <summary>
        /// Indicates whether this XFont object is bold.
        /// </summary>
        public bool Bold
        {
            get { return (_style & XFontStyle.Bold) == XFontStyle.Bold; }
        }

        /// <summary>
        /// Indicates whether this XFont object is italic.
        /// </summary>
        public bool Italic
        {
            get { return (_style & XFontStyle.Italic) == XFontStyle.Italic; }
        }

        /// <summary>
        /// Indicates whether this XFont object is stroke out.
        /// </summary>
        public bool Strikeout
        {
            get { return (_style & XFontStyle.Strikeout) == XFontStyle.Strikeout; }
        }

        /// <summary>
        /// Indicates whether this XFont object is underlined.
        /// </summary>
        public bool Underline
        {
            get { return (_style & XFontStyle.Underline) == XFontStyle.Underline; }
        }

        /// <summary>
        /// Temporary HACK for XPS to PDF converter.
        /// </summary>
        internal bool IsVertical
        {
            get { return _isVertical; }
            set { _isVertical = value; }
        }
        bool _isVertical;


        /// <summary>
        /// Gets the PDF options of the font.
        /// </summary>
        public XPdfFontOptions PdfOptions
        {
            get { return _pdfOptions ?? (_pdfOptions = new XPdfFontOptions()); }
        }
        XPdfFontOptions _pdfOptions;

        /// <summary>
        /// Indicates whether this XFont is encoded as Unicode.
        /// </summary>
        internal bool Unicode
        {
            get { return _pdfOptions != null && _pdfOptions.FontEncoding == PdfFontEncoding.Unicode; }
        }

        /// <summary>
        /// Gets the cell space for the font. The CellSpace is the line spacing, the sum of CellAscent and CellDescent and optionally some extra space.
        /// </summary>
        public int CellSpace
        {
            get { return _cellSpace; }
            internal set { _cellSpace = value; }
        }
        int _cellSpace;

        /// <summary>
        /// Gets the cell ascent, the area above the base line that is used by the font.
        /// </summary>
        public int CellAscent
        {
            get { return _cellAscent; }
            internal set { _cellAscent = value; }
        }
        int _cellAscent;

        /// <summary>
        /// Gets the cell descent, the area below the base line that is used by the font.
        /// </summary>
        public int CellDescent
        {
            get { return _cellDescent; }
            internal set { _cellDescent = value; }
        }
        int _cellDescent;

        /// <summary>
        /// Gets the font metrics.
        /// </summary>
        /// <value>The metrics.</value>
        public XFontMetrics Metrics
        {
            get
            {
                // Code moved to InitializeFontMetrics().
                //if (_fontMetrics == null)
                //{
                //    FontDescriptor descriptor = FontDescriptorStock.Global.CreateDescriptor(this);
                //    _fontMetrics = new XFontMetrics(descriptor.FontName, descriptor.UnitsPerEm, descriptor.Ascender, descriptor.Descender,
                //        descriptor.Leading, descriptor.LineSpacing, descriptor.CapHeight, descriptor.XHeight, descriptor.StemV, 0, 0, 0);
                //}
                Debug.Assert(_fontMetrics != null, "InitializeFontMetrics() not yet called.");
                return _fontMetrics;
            }
        }
        XFontMetrics _fontMetrics;

        /// <summary>
        /// Returns the line spacing, in pixels, of this font. The line spacing is the vertical distance
        /// between the base lines of two consecutive lines of text. Thus, the line spacing includes the
        /// blank space between lines along with the height of the character itself.
        /// </summary>
        public double GetHeight()
        {
            double value = CellSpace * _emSize / UnitsPerEm;
#if CORE || NETFX_CORE || UWP
            return value;
#endif
#if GDI && !WPF
#if DEBUG_
            double gdiValue = Font.GetHeight();
            Debug.Assert(DoubleUtil.AreRoughlyEqual(gdiValue, value, 5));
#endif
            return value;
#endif
#if WPF && !GDI
            return value;
#endif
#if WPF && GDI  // Testing only
            return value;
#endif
        }

        /// <summary>
        /// Returns the line spacing, in the current unit of a specified Graphics object, of this font.
        /// The line spacing is the vertical distance between the base lines of two consecutive lines of
        /// text. Thus, the line spacing includes the blank space between lines along with the height of
        /// </summary>
        [Obsolete("Use GetHeight() without parameter.")]
        public double GetHeight(XGraphics graphics)
        {
#if true
            throw new InvalidOperationException("Honestly: Use GetHeight() without parameter!");
#else
#if CORE || NETFX_CORE
            double value = CellSpace * _emSize / UnitsPerEm;
            return value;
#endif
#if GDI && !WPF
            if (graphics._gfx != null)  // #MediumTrust
            {
                double value = Font.GetHeight(graphics._gfx);
                Debug.Assert(value == Font.GetHeight(graphics._gfx.DpiY));
                double value2 = CellSpace * _emSize / UnitsPerEm;
                Debug.Assert(value - value2 < 1e-3, "??");
                return Font.GetHeight(graphics._gfx);
            }
            return CellSpace * _emSize / UnitsPerEm;
#endif
#if WPF && !GDI
            double value = CellSpace * _emSize / UnitsPerEm;
            return value;
#endif
#if GDI && WPF  // Testing only
            if (graphics.TargetContext == XGraphicTargetContext.GDI)
            {
#if DEBUG
                double value = Font.GetHeight(graphics._gfx);

                // 2355*(0.3/2048)*96 = 33.11719 
                double myValue = CellSpace * (_emSize / (96 * UnitsPerEm)) * 96;
                myValue = CellSpace * _emSize / UnitsPerEm;
                //Debug.Assert(value == myValue, "??");
                //Debug.Assert(value - myValue < 1e-3, "??");
#endif
                return Font.GetHeight(graphics._gfx);
            }

            if (graphics.TargetContext == XGraphicTargetContext.WPF)
            {
                double value = CellSpace * _emSize / UnitsPerEm;
                return value;
            }
            // ReSharper disable HeuristicUnreachableCode
            Debug.Fail("Either GDI or WPF.");
            return 0;
            // ReSharper restore HeuristicUnreachableCode
#endif
#endif
        }

        /// <summary>
        /// Gets the line spacing of this font.
        /// </summary>
        [Browsable(false)]
        public int Height
        {
            // Implementation from System.Drawing.Font.cs
            get { return (int)Math.Ceiling(GetHeight()); }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        internal XGlyphTypeface GlyphTypeface
        {
            get { return _glyphTypeface; }
        }
        XGlyphTypeface _glyphTypeface;


        internal OpenTypeDescriptor Descriptor
        {
            get { return _descriptor; }
            private set { _descriptor = value; }
        }
        OpenTypeDescriptor _descriptor;


        internal string FamilyName
        {
            get { return _familyName; }
        }
        string _familyName;


        internal int UnitsPerEm
        {
            get { return _unitsPerEm; }
            private set { _unitsPerEm = value; }
        }
        internal int _unitsPerEm;

        /// <summary>
        /// Override style simulations by using the value of StyleSimulations.
        /// </summary>
        internal bool OverrideStyleSimulations;

        /// <summary>
        /// Used to enforce style simulations by renderer. For development purposes only.
        /// </summary>
        internal XStyleSimulations StyleSimulations;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


#if CORE || GDI
        /// <summary>
        /// Gets the GDI family.
        /// </summary>
        /// <value>The GDI family.</value>
        public GdiFontFamily GdiFontFamily
        {
            get { return _gdiFontFamily; }
        }
        readonly GdiFontFamily _gdiFontFamily;

        internal GdiFont GdiFont
        {
            get { return _gdiFont; }
        }
        Font _gdiFont;

        internal static XFontStyle FontStyleFrom(GdiFont font)
        {
            return
              (font.Bold ? XFontStyle.Bold : 0) |
              (font.Italic ? XFontStyle.Italic : 0) |
              (font.Strikeout ? XFontStyle.Strikeout : 0) |
              (font.Underline ? XFontStyle.Underline : 0);
        }

#if true || UseGdiObjects
        /// <summary>
        /// Implicit conversion form Font to XFont
        /// </summary>
        public static implicit operator XFont(GdiFont font)
        {
            return new XFont(font);
        }
#endif
#endif

#if WPF
        /// <summary>
        /// Gets the WPF font family.
        /// Can be null.
        /// </summary>
        internal WpfFontFamily WpfFontFamily
        {
            get { return _wpfFontFamily; }
        }
        WpfFontFamily _wpfFontFamily;

        internal WpfTypeface WpfTypeface
        {
            get { return _wpfTypeface; }
        }
        WpfTypeface _wpfTypeface;
#endif

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Cache PdfFontTable.FontSelector to speed up finding the right PdfFont
        /// if this font is used more than once.
        /// </summary>
        internal string Selector
        {
            get { return _selector; }
            set { _selector = value; }
        }
        string _selector;

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get { return String.Format(CultureInfo.InvariantCulture, "font=('{0}' {1:0.##})", Name, Size); }
        }
    }
}
