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

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a PDF transparency group XObject.
    /// </summary>
    public sealed class PdfTransparencyGroupAttributes : PdfGroupAttributes
    {
        internal PdfTransparencyGroupAttributes(PdfDocument thisDocument)
            : base(thisDocument)
        {
            Elements.SetName(Keys.S, "/Transparency");
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public sealed new class Keys : PdfGroupAttributes.Keys
        {
            /// <summary>
            /// (Sometimes required, as discussed below)
            /// The group color space, which is used for the following purposes:
            /// • As the color space into which colors are converted when painted into the group
            /// • As the blending color space in which objects are composited within the group
            /// • As the color space of the group as a whole when it in turn is painted as an object onto its backdrop
            /// The group color space may be any device or CIE-based color space that
            /// treats its components as independent additive or subtractive values in the
            /// range 0.0 to 1.0, subject to the restrictions described in Section 7.2.3, “Blending Color Space.”
            /// These restrictions exclude Lab and lightness-chromaticity ICCBased color spaces,
            /// as well as the special color spaces Pattern, Indexed, Separation, and DeviceN.
            /// Device color spaces are subject to remapping according to the DefaultGray,
            /// DefaultRGB, and DefaultCMYK entries in the ColorSpace subdictionary of the
            /// current resource dictionary.
            /// Ordinarily, the CS entry is allowed only for isolated transparency groups
            /// (those for which I, below, is true), and even then it is optional. However,
            /// this entry is required in the group attributes dictionary for any transparency
            /// group XObject that has no parent group or page from which to inherit — in
            /// particular, one that is the value of the G entry in a soft-mask dictionary of
            /// subtype Luminosity.
            /// In addition, it is always permissible to specify CS in the group attributes
            /// dictionary associated with a page object, even if I is false or absent. In the
            /// normal case in which the page is imposed directly on the output medium,
            /// the page group is effectively isolated regardless of the I value, and the 
            /// specified CS value is therefore honored. But if the page is in turn used as an
            /// element of some other page and if the group is non-isolated, CS is ignored
            /// and the color space is inherited from the actual backdrop with which the
            /// page is composited.
            /// Default value: the color space of the parent group or page into which this
            /// transparency group is painted. (The parent’s color space in turn can be
            /// either explicitly specified or inherited.)
            /// </summary>
            [KeyInfo(KeyType.NameOrArray | KeyType.Optional)]
            public const string CS = "/CS";

            /// <summary>
            /// (Optional) A flag specifying whether the transparency group is isolated.
            /// If this flag is true, objects within the group are composited against a fully
            /// transparent initial backdrop; if false, they are composited against the
            /// group’s backdrop.
            /// Default value: false.
            /// In the group attributes dictionary for a page, the interpretation of this
            /// entry is slightly altered. In the normal case in which the page is imposed
            /// directly on the output medium, the page group is effectively isolated and
            /// the specified I value is ignored. But if the page is in turn used as an
            /// element of some other page, it is treated as if it were a transparency
            /// group XObject; the I value is interpreted in the normal way to determine
            /// whether the page group is isolated.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string I = "/I";

            /// <summary>
            /// (Optional) A flag specifying whether the transparency group is a knockout
            /// group. If this flag is false, later objects within the group are composited
            /// with earlier ones with which they overlap; if true, they are composited with
            /// the group’s initial backdrop and overwrite (“knock out”) any earlier
            /// overlapping objects.
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string K = "/K";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static new DictionaryMeta Meta
            {
                get { return _meta ?? (_meta = CreateMeta(typeof(Keys))); }
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
