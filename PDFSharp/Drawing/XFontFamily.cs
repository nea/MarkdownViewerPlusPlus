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
#if CORE || GDI
using System.Drawing;
using GdiFont = System.Drawing.Font;
using GdiFontFamily = System.Drawing.FontFamily;
using GdiFontStyle = System.Drawing.FontStyle;
#endif
#if WPF
using System.Windows.Media;
using System.Windows.Markup;
using WpfFontFamily = System.Windows.Media.FontFamily;
using WpfFontStyle = System.Windows.FontStyle;
#endif
using PdfSharp.Fonts;
using PdfSharp.Fonts.OpenType;

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Defines a group of typefaces having a similar basic design and certain variations in styles.
    /// </summary>
    public sealed class XFontFamily
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XFontFamily"/> class.
        /// </summary>
        /// <param name="familyName">The family name of a font.</param>
        public XFontFamily(string familyName)
        {
            FamilyInternal = FontFamilyInternal.GetOrCreateFromName(familyName, true);
        }

        internal XFontFamily(string familyName, bool createPlatformObjects)
        {
            FamilyInternal = FontFamilyInternal.GetOrCreateFromName(familyName, createPlatformObjects);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XFontFamily"/> class from FontFamilyInternal.
        /// </summary>
        XFontFamily(FontFamilyInternal fontFamilyInternal)
        {
            FamilyInternal = fontFamilyInternal;
        }

#if CORE || GDI
        //public XFontFamily(GdiFontFamily gdiFontFamily)
        //{
        //    FamilyInternal = FontFamilyInternal.GetOrCreateFromGdi(gdiFontFamily);
        //}
#endif

#if WPF
        //public XFontFamily(WpfFontFamily wpfFontFamily)
        //{
        //    FamilyInternal = FontFamilyInternal.GetOrCreateFromWpf(wpfFontFamily);
        //    //// HACK
        //    //int idxHash = _name.LastIndexOf('#');
        //    //if (idxHash > 0)
        //    //    _name = _name.Substring(idxHash + 1);
        //    //_wpfFamily = family;
        //}
#endif

        internal static XFontFamily CreateFromName_not_used(string name, bool createPlatformFamily)
        {
            XFontFamily fontFamily = new XFontFamily(name);
            if (createPlatformFamily)
            {
#if GDI
                //fontFamily._gdiFamily = new System.Drawing.FontFamily(name);
#endif
#if WPF
                //fontFamily._wpfFamily = new System.Windows.Media.FontFamily(name);
#endif
            }
            return fontFamily;
        }

        /// <summary>
        /// An XGlyphTypeface for a font souce that comes from a custom font resolver
        /// creates a solitary font family exclusively for it.
        /// </summary>
        internal static XFontFamily CreateSolitary(string name)
        {
            // Custom font resolver face names must not clash with platform family names.
            FontFamilyInternal fontFamilyInternal = FontFamilyCache.GetFamilyByName(name);
            if (fontFamilyInternal == null)
            {
                fontFamilyInternal = FontFamilyInternal.GetOrCreateFromName(name, false);
                fontFamilyInternal = FontFamilyCache.CacheOrGetFontFamily(fontFamilyInternal);
            }

            // Create font family and save it in cache. Do not try to create platform objects.
            return new XFontFamily(fontFamilyInternal);

            //// Custom font resolver face names must not clash with platform family names.
            //if (FontFamilyCache.GetFamilyByName(name) != null)
            //{
            //    // User must rename its font face to resolve naming confilict.
            //    throw new InvalidOperationException(String.Format("Font face name {0} clashs with existing family name.", name));
            //}

            //// Create font family and save it in cache. Do not try to create platform objects.
            //FontFamilyInternal fontFamilyInternal = FontFamilyInternal.GetOrCreateFromName(name, false);
            //fontFamilyInternal = FontFamilyCache.CacheFontFamily(fontFamilyInternal);
            //return new XFontFamily(fontFamilyInternal);
        }

#if CORE || GDI
        internal static XFontFamily GetOrCreateFromGdi(GdiFont font)
        {
            FontFamilyInternal fontFamilyInternal = FontFamilyInternal.GetOrCreateFromGdi(font.FontFamily);
            return new XFontFamily(fontFamilyInternal);
        }
#endif

#if WPF
        internal static XFontFamily GetOrCreateFromWpf(WpfFontFamily wpfFontFamily)
        {
            FontFamilyInternal fontFamilyInternal = FontFamilyInternal.GetOrCreateFromWpf(wpfFontFamily);
            return new XFontFamily(fontFamilyInternal);
        }
#endif
#if SILVERLIGHT
        //internal static XFontFamily CreateFromWpf(System.Windows.Media.FontFamily wpfFontFamily)
        //{
        //    XFontFamily fontFamily = new XFontFamily(wpfFontFamily.FamilyNames[XmlLanguage.GetLanguage("en")]);
        //    fontFamily._wpfFamily = wpfFontFamily;
        //    return fontFamily;
        //}
#endif

        /// <summary>
        /// Gets the name of the font family.
        /// </summary>
        public string Name
        {
            get { return FamilyInternal.Name; }
        }

#if true__
        public double LineSpacing
        {
            get
            {
                WpfFamily.FamilyTypefaces[0].UnderlineThickness
            }
        }

#endif

        /// <summary>
        /// Returns the cell ascent, in design units, of the XFontFamily object of the specified style.
        /// </summary>
        public int GetCellAscent(XFontStyle style)
        {
            OpenTypeDescriptor descriptor = (OpenTypeDescriptor)FontDescriptorCache.GetOrCreateDescriptor(Name, style);
            int result = descriptor.Ascender;
#if DEBUG_ && GDI
            int gdiValue = _gdiFamily.GetCellAscent((FontStyle)style);
            Debug.Assert(gdiValue == result);
#endif
            return result;
        }

        /// <summary>
        /// Returns the cell descent, in design units, of the XFontFamily object of the specified style.
        /// </summary>
        public int GetCellDescent(XFontStyle style)
        {
            OpenTypeDescriptor descriptor = (OpenTypeDescriptor)FontDescriptorCache.GetOrCreateDescriptor(Name, style);
            int result = descriptor.Descender;
#if DEBUG_ && GDI
            int gdiValue = _gdiFamily.GetCellDescent((FontStyle)style);
            Debug.Assert(gdiValue == result);
#endif
            return result;
        }

        /// <summary>
        /// Gets the height, in font design units, of the em square for the specified style.
        /// </summary>
        public int GetEmHeight(XFontStyle style)
        {
            OpenTypeDescriptor descriptor = (OpenTypeDescriptor)FontDescriptorCache.GetOrCreateDescriptor(Name, style);
            int result = descriptor.UnitsPerEm;
#if DEBUG_ && GDI
            int gdiValue = _gdiFamily.GetEmHeight((FontStyle)style);
            Debug.Assert(gdiValue == result);
#endif
#if DEBUG_
            int headValue = descriptor.FontFace.head.unitsPerEm;
            Debug.Assert(headValue == result);
#endif
            return result;
        }

        /// <summary>
        /// Returns the line spacing, in design units, of the FontFamily object of the specified style.
        /// The line spacing is the vertical distance between the base lines of two consecutive lines of text.
        /// </summary>
        public int GetLineSpacing(XFontStyle style)
        {
            OpenTypeDescriptor descriptor = (OpenTypeDescriptor)FontDescriptorCache.GetOrCreateDescriptor(Name, style);
            int result = descriptor.LineSpacing;
#if DEBUG_ && GDI
            int gdiValue = _gdiFamily.GetLineSpacing((FontStyle)style);
            Debug.Assert(gdiValue == result);
#endif
#if DEBUG_ && WPF && !SILVERLIGHT
            int wpfValue = (int)Math.Round(_wpfFamily.LineSpacing * GetEmHeight(style));
            Debug.Assert(wpfValue == result);
#endif
            return result;
        }

        //public string GetName(int language);

        /// <summary>
        /// Indicates whether the specified FontStyle enumeration is available.
        /// </summary>
        public bool IsStyleAvailable(XFontStyle style)
        {
            XGdiFontStyle xStyle = ((XGdiFontStyle)style) & XGdiFontStyle.BoldItalic;
#if CORE
            throw new InvalidOperationException("In CORE build it is the responsibility of the developer to provide all required font faces.");
#endif
#if GDI && !WPF
            if (GdiFamily != null)
                return GdiFamily.IsStyleAvailable((GdiFontStyle)xStyle);
            return false;
#endif
#if WPF && !GDI
            if (WpfFamily != null)
                return FontHelper.IsStyleAvailable(this, xStyle);
            return false;
#endif
#if WPF && GDI
#if DEBUG
            //bool gdiResult = _gdiFamily.IsStyle Available((FontStyle)style);
            //bool wpfResult = FontHelper.IsStyle Available(this, style);
            //// TODOWPF: check when fails
            //Debug.Assert(gdiResult == wpfResult, "GDI+ and WPF provide different values.");
#endif
            return FontHelper.IsStyleAvailable(this, xStyle);
#endif
#if NETFX_CORE || UWP
            throw new InvalidOperationException("In NETFX_CORE build it is the responsibility of the developer to provide all required font faces.");
#endif
        }

        /// <summary>
        /// Returns an array that contains all the FontFamily objects associated with the current graphics context.
        /// </summary>
        [Obsolete("Use platform API directly.")]
        public static XFontFamily[] Families
        {
            get
            {
                throw new InvalidOperationException("Obsolete and not implemted any more.");
            }
        }

        /// <summary>
        /// Returns an array that contains all the FontFamily objects available for the specified 
        /// graphics context.
        /// </summary>
        [Obsolete("Use platform API directly.")]
        public static XFontFamily[] GetFamilies(XGraphics graphics)
        {
            throw new InvalidOperationException("Obsolete and not implemted any more.");
        }

#if GDI
        /// <summary>
        /// Gets the underlying GDI+ font family object.
        /// Is null if the font was created by a font resolver.
        /// </summary>
        internal GdiFontFamily GdiFamily
        {
            get { return FamilyInternal.GdiFamily; }
        }
#endif

#if WPF
        /// <summary>
        /// Gets the underlying WPF font family object.
        /// Is null if the font was created by a font resolver.
        /// </summary>
        internal WpfFontFamily WpfFamily
        {
            get { return FamilyInternal.WpfFamily; }
        }
#endif

        /// <summary>
        /// The implementation sigleton of font family;
        /// </summary>
        internal FontFamilyInternal FamilyInternal;
    }
}
