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
using PdfSharp.Fonts.OpenType;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// The PDF font descriptor flags.
    /// </summary>
    [Flags]
    enum PdfFontDescriptorFlags
    {
        /// <summary>
        /// All glyphs have the same width (as opposed to proportional or variable-pitch
        /// fonts, which have different widths).
        /// </summary>
        FixedPitch = 1 << 0,

        /// <summary>
        /// Glyphs have serifs, which are short strokes drawn at an angle on the top and
        /// bottom of glyph stems. (Sans serif fonts do not have serifs.)
        /// </summary>
        Serif = 1 << 1,

        /// <summary>
        /// Font contains glyphs outside the Adobe standard Latin character set. This
        /// flag and the Nonsymbolic flag cannot both be set or both be clear.
        /// </summary>
        Symbolic = 1 << 2,

        /// <summary>
        /// Glyphs resemble cursive handwriting.
        /// </summary>
        Script = 1 << 3,

        /// <summary>
        /// Font uses the Adobe standard Latin character set or a subset of it.
        /// </summary>
        Nonsymbolic = 1 << 5,

        /// <summary>
        /// Glyphs have dominant vertical strokes that are slanted.
        /// </summary>
        Italic = 1 << 6,

        /// <summary>
        /// Font contains no lowercase letters; typically used for display purposes,
        /// such as for titles or headlines.
        /// </summary>
        AllCap = 1 << 16,

        /// <summary>
        /// Font contains both uppercase and lowercase letters. The uppercase letters are
        /// similar to those in the regular version of the same typeface family. The glyphs
        /// for the lowercase letters have the same shapes as the corresponding uppercase
        /// letters, but they are sized and their proportions adjusted so that they have the
        /// same size and stroke weight as lowercase glyphs in the same typeface family.
        /// </summary>
        SmallCap = 1 << 17,

        /// <summary>
        /// Determines whether bold glyphs are painted with extra pixels even at very small
        /// text sizes.
        /// </summary>
        ForceBold = 1 << 18,
    }

    /// <summary>
    /// A PDF font descriptor specifies metrics and other attributes of a simple font, 
    /// as distinct from the metrics of individual glyphs.
    /// </summary>
    public sealed class PdfFontDescriptor : PdfDictionary
    {
        internal PdfFontDescriptor(PdfDocument document, OpenTypeDescriptor descriptor)
            : base(document)
        {
            _descriptor = descriptor;
            Elements.SetName(Keys.Type, "/FontDescriptor");

            Elements.SetInteger(Keys.Ascent, _descriptor.DesignUnitsToPdf(_descriptor.Ascender));
            Elements.SetInteger(Keys.CapHeight, _descriptor.DesignUnitsToPdf(_descriptor.CapHeight));
            Elements.SetInteger(Keys.Descent, _descriptor.DesignUnitsToPdf(_descriptor.Descender));
            Elements.SetInteger(Keys.Flags, (int)FlagsFromDescriptor(_descriptor));
            Elements.SetRectangle(Keys.FontBBox, new PdfRectangle(
              _descriptor.DesignUnitsToPdf(_descriptor.XMin),
              _descriptor.DesignUnitsToPdf(_descriptor.YMin),
              _descriptor.DesignUnitsToPdf(_descriptor.XMax),
              _descriptor.DesignUnitsToPdf(_descriptor.YMax)));
            // not here, done in PdfFont later... 
            //Elements.SetName(Keys.FontName, "abc"); //descriptor.FontName);
            Elements.SetReal(Keys.ItalicAngle, _descriptor.ItalicAngle);
            Elements.SetInteger(Keys.StemV, _descriptor.StemV);
            Elements.SetInteger(Keys.XHeight, _descriptor.DesignUnitsToPdf(_descriptor.XHeight));
        }

        //HACK OpenTypeDescriptor descriptor
        internal OpenTypeDescriptor _descriptor;

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        public string FontName
        {
            get { return Elements.GetName(Keys.FontName); }
            set { Elements.SetName(Keys.FontName, value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is symbol font.
        /// </summary>
        public bool IsSymbolFont
        {
            get { return _isSymbolFont; }
        }
        bool _isSymbolFont;

        // HACK FlagsFromDescriptor(OpenTypeDescriptor descriptor)
        PdfFontDescriptorFlags FlagsFromDescriptor(OpenTypeDescriptor descriptor)
        {
            PdfFontDescriptorFlags flags = 0;
            _isSymbolFont = descriptor.FontFace.cmap.symbol;
            flags |= descriptor.FontFace.cmap.symbol ? PdfFontDescriptorFlags.Symbolic : PdfFontDescriptorFlags.Nonsymbolic;
            return flags;
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public sealed class Keys : KeysBase
        {
            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes; must be
            /// FontDescriptor for a font descriptor.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "FontDescriptor")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The PostScript name of the font. This name should be the same as the 
            /// value of BaseFont in the font or CIDFont dictionary that refers to this font descriptor.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string FontName = "/FontName";

            /// <summary>
            /// (Optional; PDF 1.5; strongly recommended for Type 3 fonts in Tagged PDF documents)
            /// A string specifying the preferred font family name. For example, for the font 
            /// Times Bold Italic, the FontFamily is Times.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string FontFamily = "/FontFamily";

            /// <summary>
            /// (Optional; PDF 1.5; strongly recommended for Type 3 fonts in Tagged PDF documents)
            /// The font stretch value. It must be one of the following names (ordered from 
            /// narrowest to widest): UltraCondensed, ExtraCondensed, Condensed, SemiCondensed, 
            /// Normal, SemiExpanded, Expanded, ExtraExpanded or UltraExpanded.
            /// Note: The specific interpretation of these values varies from font to font. 
            /// For example, Condensed in one font may appear most similar to Normal in another.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string FontStretch = "/FontStretch";

            /// <summary>
            /// (Optional; PDF 1.5; strongly recommended for Type 3 fonts in Tagged PDF documents)
            /// The weight (thickness) component of the fully-qualified font name or font specifier.
            /// The possible values are 100, 200, 300, 400, 500, 600, 700, 800, or 900, where each
            /// number indicates a weight that is at least as dark as its predecessor. A value of 
            /// 400 indicates a normal weight; 700 indicates bold.
            /// Note: The specific interpretation of these values varies from font to font. 
            /// For example, 300 in one font may appear most similar to 500 in another.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string FontWeight = "/FontWeight";

            /// <summary>
            /// (Required) A collection of flags defining various characteristics of the font.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Flags = "/Flags";

            /// <summary>
            /// (Required, except for Type 3 fonts) A rectangle (see Section 3.8.4, “Rectangles”),
            /// expressed in the glyph coordinate system, specifying the font bounding box. This 
            /// is the smallest rectangle enclosing the shape that would result if all of the 
            /// glyphs of the font were placed with their origins coincident and then filled.
            /// </summary>
            [KeyInfo(KeyType.Rectangle | KeyType.Required)]
            public const string FontBBox = "/FontBBox";

            /// <summary>
            /// (Required) The angle, expressed in degrees counterclockwise from the vertical, of
            /// the dominant vertical strokes of the font. (For example, the 9-o’clock position is 90 
            /// degrees, and the 3-o’clock position is –90 degrees.) The value is negative for fonts 
            /// that slope to the right, as almost all italic fonts do.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Required)]
            public const string ItalicAngle = "/ItalicAngle";

            /// <summary>
            /// (Required, except for Type 3 fonts) The maximum height above the baseline reached 
            /// by glyphs in this font, excluding the height of glyphs for accented characters.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Required)]
            public const string Ascent = "/Ascent";

            /// <summary>
            /// (Required, except for Type 3 fonts) The maximum depth below the baseline reached 
            /// by glyphs in this font. The value is a negative number.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Required)]
            public const string Descent = "/Descent";

            /// <summary>
            /// (Optional) The spacing between baselines of consecutive lines of text.
            /// Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string Leading = "/Leading";

            /// <summary>
            /// (Required for fonts that have Latin characters, except for Type 3 fonts) The vertical
            /// coordinate of the top of flat capital letters, measured from the baseline.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Required)]
            public const string CapHeight = "/CapHeight";

            /// <summary>
            /// (Optional) The font’s x height: the vertical coordinate of the top of flat nonascending
            /// lowercase letters (like the letter x), measured from the baseline, in fonts that have 
            /// Latin characters. Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string XHeight = "/XHeight";

            /// <summary>
            /// (Required, except for Type 3 fonts) The thickness, measured horizontally, of the dominant 
            /// vertical stems of glyphs in the font.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Required)]
            public const string StemV = "/StemV";

            /// <summary>
            /// (Optional) The thickness, measured vertically, of the dominant horizontal stems 
            /// of glyphs in the font. Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string StemH = "/StemH";

            /// <summary>
            /// (Optional) The average width of glyphs in the font. Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string AvgWidth = "/AvgWidth";

            /// <summary>
            /// (Optional) The maximum width of glyphs in the font. Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string MaxWidth = "/MaxWidth";

            /// <summary>
            /// (Optional) The width to use for character codes whose widths are not specified in a 
            /// font dictionary’s Widths array. This has a predictable effect only if all such codes 
            /// map to glyphs whose actual widths are the same as the value of the MissingWidth entry.
            /// Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string MissingWidth = "/MissingWidth";

            /// <summary>
            /// (Optional) A stream containing a Type 1 font program.
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Optional)]
            public const string FontFile = "/FontFile";

            /// <summary>
            /// (Optional; PDF 1.1) A stream containing a TrueType font program.
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Optional)]
            public const string FontFile2 = "/FontFile2";

            /// <summary>
            /// (Optional; PDF 1.2) A stream containing a font program whose format is specified 
            /// by the Subtype entry in the stream dictionary.
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Optional)]
            public const string FontFile3 = "/FontFile3";

            /// <summary>
            /// (Optional; meaningful only in Type 1 fonts; PDF 1.1) A string listing the character
            /// names defined in a font subset. The names in this string must be in PDF syntax—that is,
            /// each name preceded by a slash (/). The names can appear in any order. The name .notdef
            /// should be omitted; it is assumed to exist in the font subset. If this entry is absent,
            /// the only indication of a font subset is the subset tag in the FontName entry.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string CharSet = "/CharSet";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta
            {
                get
                {
                    if (_meta == null)
                        _meta = CreateMeta(typeof(Keys));
                    return _meta;
                }
            }
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta
        {
            get { return Keys.Meta; }
        }
    }
}
