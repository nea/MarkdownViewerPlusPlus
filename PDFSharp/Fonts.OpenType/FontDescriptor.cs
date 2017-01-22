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
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
#endif
using PdfSharp.Pdf.Internal;
using PdfSharp.Fonts;
#if !EDF_CORE
using PdfSharp.Drawing;
#endif

#pragma warning disable 0649

namespace PdfSharp.Fonts.OpenType
{
    // TODO: Needs to be refactored #???
    /// <summary>
    /// Base class for all font descriptors.
    /// Currently only OpenTypeDescriptor is derived from this base class.
    /// </summary>
    internal class FontDescriptor
    {
        protected FontDescriptor(string key)
        {
            _key = key;
        }

        public string Key
        {
            get { return _key; }
        }
        readonly string _key;







        ///// <summary>
        ///// 
        ///// </summary>
        //public string FontFile
        //{
        //  get { return _fontFile; }
        //  private set { _fontFile = value; }  // BUG: never set
        //}
        //string _fontFile;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string FontType
        //{
        //  get { return _fontType; }
        //  private set { _fontType = value; }  // BUG: never set
        //}
        //string _fontType;

        /// <summary>
        /// 
        /// </summary>
        public string FontName
        {
            get { return _fontName; }
            protected set { _fontName = value; }
        }
        string _fontName;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string FullName
        //{
        //    get { return _fullName; }
        //    private set { _fullName = value; }  // BUG: never set
        //}
        //string _fullName;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string FamilyName
        //{
        //    get { return _familyName; }
        //    private set { _familyName = value; }  // BUG: never set
        //}
        //string _familyName;

        /// <summary>
        /// 
        /// </summary>
        public string Weight
        {
            get { return _weight; }
            private set { _weight = value; }  // BUG: never set
        }
        string _weight;

        /// <summary>
        /// Gets a value indicating whether this instance belongs to a bold font.
        /// </summary>
        public virtual bool IsBoldFace
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float ItalicAngle
        {
            get { return _italicAngle; }
            protected set { _italicAngle = value; }
        }
        float _italicAngle;

        /// <summary>
        /// Gets a value indicating whether this instance belongs to an italic font.
        /// </summary>
        public virtual bool IsItalicFace
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int XMin
        {
            get { return _xMin; }
            protected set { _xMin = value; }
        }
        int _xMin;

        /// <summary>
        /// 
        /// </summary>
        public int YMin
        {
            get { return _yMin; }
            protected set { _yMin = value; }
        }
        int _yMin;

        /// <summary>
        /// 
        /// </summary>
        public int XMax
        {
            get { return _xMax; }
            protected set { _xMax = value; }
        }
        int _xMax;

        /// <summary>
        /// 
        /// </summary>
        public int YMax
        {
            get { return _yMax; }
            protected set { _yMax = value; }
        }
        int _yMax;

        /// <summary>
        /// 
        /// </summary>
        public bool IsFixedPitch
        {
            get { return _isFixedPitch; }
            private set { _isFixedPitch = value; }  // BUG: never set
        }
        bool _isFixedPitch;

        //Rect FontBBox;

        /// <summary>
        /// 
        /// </summary>
        public int UnderlinePosition
        {
            get { return _underlinePosition; }
            protected set { _underlinePosition = value; }
        }
        int _underlinePosition;

        /// <summary>
        /// 
        /// </summary>
        public int UnderlineThickness
        {
            get { return _underlineThickness; }
            protected set { _underlineThickness = value; }
        }
        int _underlineThickness;

        /// <summary>
        /// 
        /// </summary>
        public int StrikeoutPosition
        {
            get { return _strikeoutPosition; }
            protected set { _strikeoutPosition = value; }
        }
        int _strikeoutPosition;

        /// <summary>
        /// 
        /// </summary>
        public int StrikeoutSize
        {
            get { return _strikeoutSize; }
            protected set { _strikeoutSize = value; }
        }
        int _strikeoutSize;

        /// <summary>
        /// 
        /// </summary>
        public string Version
        {
            get { return _version; }
            private set { _version = value; }  // BUG: never set
        }
        string _version;

        ///// <summary>
        ///// 
        ///// </summary>
        //public string Notice
        //{
        //  get { return Notice; }
        //}
        //protected string notice;

        /// <summary>
        /// 
        /// </summary>
        public string EncodingScheme
        {
            get { return _encodingScheme; }
            private set { _encodingScheme = value; }  // BUG: never set
        }
        string _encodingScheme;

        /// <summary>
        /// 
        /// </summary>
        public int UnitsPerEm
        {
            get { return _unitsPerEm; }
            protected set { _unitsPerEm = value; }
        }
        int _unitsPerEm;

        /// <summary>
        /// 
        /// </summary>
        public int CapHeight
        {
            get { return _capHeight; }
            protected set { _capHeight = value; }
        }
        int _capHeight;

        /// <summary>
        /// 
        /// </summary>
        public int XHeight
        {
            get { return _xHeight; }
            protected set { _xHeight = value; }
        }
        int _xHeight;

        /// <summary>
        /// 
        /// </summary>
        public int Ascender
        {
            get { return _ascender; }
            protected set { _ascender = value; }
        }
        int _ascender;

        /// <summary>
        /// 
        /// </summary>
        public int Descender
        {
            get { return _descender; }
            protected set { _descender = value; }
        }
        int _descender;

        /// <summary>
        /// 
        /// </summary>
        public int Leading
        {
            get { return _leading; }
            protected set { _leading = value; }
        }
        int _leading;

        /// <summary>
        /// 
        /// </summary>
        public int Flags
        {
            get { return _flags; }
            private set { _flags = value; }  // BUG: never set
        }
        int _flags;

        /// <summary>
        /// 
        /// </summary>
        public int StemV
        {
            get { return _stemV; }
            protected set { _stemV = value; }
        }
        int _stemV;

        /// <summary>
        /// 
        /// </summary>
        public int LineSpacing
        {
            get { return _lineSpacing; }
            protected set { _lineSpacing = value; }
        }
        int _lineSpacing;


        internal static string ComputeKey(XFont font)
        {
            return font.GlyphTypeface.Key;
            //return ComputeKey(font.GlyphTypeface.Fontface.FullFaceName, font.Style);
            //XGlyphTypeface glyphTypeface = font.GlyphTypeface;
            //string key = glyphTypeface.Fontface.FullFaceName.ToLowerInvariant() +
            //    (glyphTypeface.IsBold ? "/b" : "") + (glyphTypeface.IsItalic ? "/i" : "");
            //return key;
        }

        internal static string ComputeKey(string name, XFontStyle style)
        {
            return ComputeKey(name,
                (style & XFontStyle.Bold) == XFontStyle.Bold,
                (style & XFontStyle.Italic) == XFontStyle.Italic);
        }

        internal static string ComputeKey(string name, bool isBold, bool isItalic)
        {
            string key = name.ToLowerInvariant() + '/'
                + (isBold ? "b" : "") + (isItalic ? "i" : "");
            return key;
        }

        internal static string ComputeKey(string name)
        {
            string key = name.ToLowerInvariant();
            return key;
        }
    }
}