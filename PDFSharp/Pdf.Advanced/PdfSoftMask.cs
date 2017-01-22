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
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows.Media;
#endif

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a PDF soft mask.
    /// </summary>
    public class PdfSoftMask : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfXObject"/> class.
        /// </summary>
        /// <param name="document">The document that owns the object.</param>
        public PdfSoftMask(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/Mask");
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public class Keys : KeysBase
        {
            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes;
            /// if present, must be Mask for a soft-mask dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "Mask")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) A subtype specifying the method to be used in deriving the mask values
            /// from the transparency group specified by the G entry:
            /// Alpha: Use the group’s computed alpha, disregarding its color.
            /// Luminosity: Convert the group’s computed color to a single-component luminosity value.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string S = "/S";

            /// <summary>
            /// (Required) A transparency group XObject to be used as the source of alpha
            /// or color values for deriving the mask. If the subtype S is Luminosity, the
            /// group attributes dictionary must contain a CS entry defining the color space
            /// in which the compositing computation is to be performed.
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Required)]
            public const string G = "/G";

            /// <summary>
            /// (Optional) An array of component values specifying the color to be used
            /// as the backdrop against which to composite the transparency group XObject G.
            /// This entry is consulted only if the subtype S is Luminosity. The array consists of
            /// n numbers, where n is the number of components in the color space specified
            /// by the CS entry in the group attributes dictionary.
            /// Default value: the color space’s initial value, representing black.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string BC = "/BC";

            /// <summary>
            /// (Optional) A function object specifying the transfer function to be used in
            /// deriving the mask values. The function accepts one input, the computed
            /// group alpha or luminosity (depending on the value of the subtype S), and
            /// returns one output, the resulting mask value. Both the input and output
            /// must be in the range 0.0 to 1.0; if the computed output falls outside this
            /// range, it is forced to the nearest valid value. The name Identity may be
            /// specified in place of a function object to designate the identity function.
            /// Default value: Identity.
            /// </summary>
            [KeyInfo(KeyType.FunctionOrName | KeyType.Optional)]
            public const string TR = "/TR";
        }
    }
}