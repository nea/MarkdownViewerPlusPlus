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
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a shading dictionary.
    /// </summary>
    public sealed class PdfShading : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfShading"/> class.
        /// </summary>
        public PdfShading(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Setups the shading from the specified brush.
        /// </summary>
        internal void SetupFromBrush(XLinearGradientBrush brush, XGraphicsPdfRenderer renderer)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            PdfColorMode colorMode = _document.Options.ColorMode;
            XColor color1 = ColorSpaceHelper.EnsureColorMode(colorMode, brush._color1);
            XColor color2 = ColorSpaceHelper.EnsureColorMode(colorMode, brush._color2);

            PdfDictionary function = new PdfDictionary();

            Elements[Keys.ShadingType] = new PdfInteger(2);
            if (colorMode != PdfColorMode.Cmyk)
                Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
            else
                Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");

            double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
            if (brush._useRect)
            {
                XPoint pt1 = renderer.WorldToView(brush._rect.TopLeft);
                XPoint pt2 = renderer.WorldToView(brush._rect.BottomRight);

                switch (brush._linearGradientMode)
                {
                    case XLinearGradientMode.Horizontal:
                        x1 = pt1.X;
                        y1 = pt1.Y;
                        x2 = pt2.X;
                        y2 = pt1.Y;
                        break;

                    case XLinearGradientMode.Vertical:
                        x1 = pt1.X;
                        y1 = pt1.Y;
                        x2 = pt1.X;
                        y2 = pt2.Y;
                        break;

                    case XLinearGradientMode.ForwardDiagonal:
                        x1 = pt1.X;
                        y1 = pt1.Y;
                        x2 = pt2.X;
                        y2 = pt2.Y;
                        break;

                    case XLinearGradientMode.BackwardDiagonal:
                        x1 = pt2.X;
                        y1 = pt1.Y;
                        x2 = pt1.X;
                        y2 = pt2.Y;
                        break;
                }
            }
            else
            {
                XPoint pt1 = renderer.WorldToView(brush._point1);
                XPoint pt2 = renderer.WorldToView(brush._point2);

                x1 = pt1.X;
                y1 = pt1.Y;
                x2 = pt2.X;
                y2 = pt2.Y;
            }

            const string format = Config.SignificantFigures3;
            Elements[Keys.Coords] = new PdfLiteral("[{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "}]", x1, y1, x2, y2);

            //Elements[Keys.Background] = new PdfRawItem("[0 1 1]");
            //Elements[Keys.Domain] = 
            Elements[Keys.Function] = function;
            //Elements[Keys.Extend] = new PdfRawItem("[true true]");

            string clr1 = "[" + PdfEncoders.ToString(color1, colorMode) + "]";
            string clr2 = "[" + PdfEncoders.ToString(color2, colorMode) + "]";

            function.Elements["/FunctionType"] = new PdfInteger(2);
            function.Elements["/C0"] = new PdfLiteral(clr1);
            function.Elements["/C1"] = new PdfLiteral(clr2);
            function.Elements["/Domain"] = new PdfLiteral("[0 1]");
            function.Elements["/N"] = new PdfInteger(1);
        }

        /// <summary>
        /// Common keys for all streams.
        /// </summary>
        internal sealed class Keys : KeysBase
        {
            /// <summary>
            /// (Required) The shading type:
            /// 1 Function-based shading
            /// 2 Axial shading
            /// 3 Radial shading
            /// 4 Free-form Gouraud-shaded triangle mesh
            /// 5 Lattice-form Gouraud-shaded triangle mesh
            /// 6 Coons patch mesh
            /// 7 Tensor-product patch mesh
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string ShadingType = "/ShadingType";

            /// <summary>
            /// (Required) The color space in which color values are expressed. This may be any device, 
            /// CIE-based, or special color space except a Pattern space.
            /// </summary>
            [KeyInfo(KeyType.NameOrArray | KeyType.Required)]
            public const string ColorSpace = "/ColorSpace";

            /// <summary>
            /// (Optional) An array of color components appropriate to the color space, specifying
            /// a single background color value. If present, this color is used, before any painting 
            /// operation involving the shading, to fill those portions of the area to be painted 
            /// that lie outside the bounds of the shading object. In the opaque imaging model, 
            /// the effect is as if the painting operation were performed twice: first with the 
            /// background color and then with the shading.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Background = "/Background";

            /// <summary>
            /// (Optional) An array of four numbers giving the left, bottom, right, and top coordinates, 
            /// respectively, of the shading’s bounding box. The coordinates are interpreted in the 
            /// shading’s target coordinate space. If present, this bounding box is applied as a temporary 
            /// clipping boundary when the shading is painted, in addition to the current clipping path
            /// and any other clipping boundaries in effect at that time.
            /// </summary>
            [KeyInfo(KeyType.Rectangle | KeyType.Optional)]
            public const string BBox = "/BBox";

            /// <summary>
            /// (Optional) A flag indicating whether to filter the shading function to prevent aliasing 
            /// artifacts. The shading operators sample shading functions at a rate determined by the 
            /// resolution of the output device. Aliasing can occur if the function is not smooth—that
            /// is, if it has a high spatial frequency relative to the sampling rate. Anti-aliasing can
            /// be computationally expensive and is usually unnecessary, since most shading functions
            /// are smooth enough or are sampled at a high enough frequency to avoid aliasing effects.
            /// Anti-aliasing may not be implemented on some output devices, in which case this flag
            /// is ignored.
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string AntiAlias = "/AntiAlias";

            // ---- Type 2 ----------------------------------------------------------

            /// <summary>
            /// (Required) An array of four numbers [x0 y0 x1 y1] specifying the starting and
            /// ending coordinates of the axis, expressed in the shading’s target coordinate space.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Required)]
            public const string Coords = "/Coords";

            /// <summary>
            /// (Optional) An array of two numbers [t0 t1] specifying the limiting values of a
            /// parametric variable t. The variable is considered to vary linearly between these
            /// two values as the color gradient varies between the starting and ending points of
            /// the axis. The variable t becomes the input argument to the color function(s).
            /// Default value: [0.0 1.0].
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Domain = "/Domain";

            /// <summary>
            /// (Required) A 1-in, n-out function or an array of n 1-in, 1-out functions (where n
            /// is the number of color components in the shading dictionary’s color space). The
            /// function(s) are called with values of the parametric variable t in the domain defined
            /// by the Domain entry. Each function’s domain must be a superset of that of the shading
            /// dictionary. If the value returned by the function for a given color component is out
            /// of range, it is adjusted to the nearest valid value.
            /// </summary>
            [KeyInfo(KeyType.Function | KeyType.Required)]
            public const string Function = "/Function";

            /// <summary>
            /// (Optional) An array of two boolean values specifying whether to extend the shading
            /// beyond the starting and ending points of the axis, respectively.
            /// Default value: [false false].
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Extend = "/Extend";

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
