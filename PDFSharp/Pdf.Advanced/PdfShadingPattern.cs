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
#if GDI
using System.Drawing;
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows.Media;
#endif
using PdfSharp.Drawing;
using PdfSharp.Drawing.Pdf;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a shading pattern dictionary.
    /// </summary>
    public sealed class PdfShadingPattern : PdfDictionaryWithContentStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfShadingPattern"/> class.
        /// </summary>
        public PdfShadingPattern(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/Pattern");
            Elements[Keys.PatternType] = new PdfInteger(2);
        }

        /// <summary>
        /// Setups the shading pattern from the specified brush.
        /// </summary>
        internal void SetupFromBrush(XLinearGradientBrush brush, XMatrix matrix, XGraphicsPdfRenderer renderer)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            PdfShading shading = new PdfShading(_document);
            shading.SetupFromBrush(brush, renderer);
            Elements[Keys.Shading] = shading;
            //Elements[Keys.Matrix] = new PdfLiteral("[" + PdfEncoders.ToString(matrix) + "]");
            Elements.SetMatrix(Keys.Matrix, matrix);
        }

        /// <summary>
        /// Common keys for all streams.
        /// </summary>
        internal sealed new class Keys : PdfDictionaryWithContentStream.Keys
        {
            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes; if present,
            /// must be Pattern for a pattern dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) A code identifying the type of pattern that this dictionary describes;
            /// must be 2 for a shading pattern.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string PatternType = "/PatternType";

            /// <summary>
            /// (Required) A shading object (see below) defining the shading pattern’s gradient fill.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required)]
            public const string Shading = "/Shading";

            /// <summary>
            /// (Optional) An array of six numbers specifying the pattern matrix.
            /// Default value: the identity matrix [1 0 0 1 0 0].
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Matrix = "/Matrix";

            /// <summary>
            /// (Optional) A graphics state parameter dictionary containing graphics state parameters
            /// to be put into effect temporarily while the shading pattern is painted. Any parameters
            /// that are not so specified are inherited from the graphics state that was in effect
            /// at the beginning of the content stream in which the pattern is defined as a resource.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string ExtGState = "/ExtGState";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta
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
