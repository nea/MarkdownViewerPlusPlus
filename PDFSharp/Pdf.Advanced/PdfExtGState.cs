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

using System.Globalization;

#if GDI
using System.Drawing;
using System.Drawing.Imaging;
#endif
#if WPF
#endif

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents an extended graphics state object.
    /// </summary>
    public sealed class PdfExtGState : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfExtGState"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public PdfExtGState(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/ExtGState");

#if true_
            //AIS false
            //BM /Normal
            //ca 1
            //CA 1
            //op false
            //OP false
            //OPM 1
            //SA true
            //SMask /None
            //Type /ExtGState

            Elements.SetValue(Keys.AIS, new PdfBoolean(false)); // The alpha source
            Elements.SetName("/BM", "Normal");
            Elements.SetValue(Keys.op, new PdfBoolean(false));
            Elements.SetValue(Keys.OP, new PdfBoolean(false));
            Elements.SetValue(Keys.OPM, new PdfInteger(1));
            Elements.SetValue("/SA", new PdfBoolean(true));
            Elements.SetName("/SMask", "None");
#endif
            //#if OP_HACK
            //            Elements.SetValue(Keys.op, new PdfBoolean(false));
            //            Elements.SetValue(Keys.OP, new PdfBoolean(false));
            //            Elements.SetValue(Keys.OPM, new PdfInteger(1));
            //#endif
        }

        /// <summary>
        /// Used in Edf.Xps.
        /// </summary>
        internal void SetDefault1()
        {
            //<<
            //  /AIS false
            //  /BM /Normal
            //  /ca 1
            //  /CA 1
            //  /op false
            //  /OP false
            //  /OPM 1
            //  /SA true
            //  /SMask /None
            //  /Type /ExtGState
            //>>
            Elements.SetBoolean(Keys.AIS, false);
            if (Elements.ContainsKey(Keys.BM)) Elements.SetName(Keys.BM, "/Normal");
            StrokeAlpha = 1;
            NonStrokeAlpha = 1;
            Elements.SetBoolean(Keys.op, false);
            Elements.SetBoolean(Keys.OP, false);
            Elements.SetBoolean(Keys.SA, true);
            Elements.SetName(Keys.SMask, "/None");
        }

        /// <summary>
        /// Used in Edf.Xps.
        /// ...for shading patterns
        /// </summary>
        internal void SetDefault2()
        {
            //<<
            //  /AIS false
            //  /BM /Normal
            //  /ca 1
            //  /CA 1
            //  /op true
            //  /OP true
            //  /OPM 1
            //  /SA true
            //  /SMask /None
            //  /Type /ExtGState
            //>>
            Elements.SetBoolean(Keys.AIS, false);
            Elements.SetName(Keys.BM, "/Normal");
            StrokeAlpha = 1;
            NonStrokeAlpha = 1;
            Elements.SetBoolean(Keys.op, true);
            Elements.SetBoolean(Keys.OP, true);
            Elements.SetInteger(Keys.OPM, 1);
            Elements.SetBoolean(Keys.SA, true);
            Elements.SetName(Keys.SMask, "/None");
        }

        /// <summary>
        /// Sets the alpha value for stroking operations.
        /// </summary>
        public double StrokeAlpha
        {
            set
            {
                _strokeAlpha = value;
                Elements.SetReal(Keys.CA, value);
                UpdateKey();
            }
        }
        double _strokeAlpha;

        /// <summary>
        /// Sets the alpha value for nonstroking operations.
        /// </summary>
        public double NonStrokeAlpha
        {
            set
            {
                _nonStrokeAlpha = value;
                Elements.SetReal(Keys.ca, value);
                UpdateKey();
            }
        }
        double _nonStrokeAlpha;

        /// <summary>
        /// Sets the overprint value for stroking operations.
        /// </summary>
        public bool StrokeOverprint
        {
            set
            {
                _strokeOverprint = value;
                Elements.SetBoolean(Keys.OP, value);
                UpdateKey();
            }
        }
        bool _strokeOverprint;

        /// <summary>
        /// Sets the overprint value for nonstroking operations.
        /// </summary>
        public bool NonStrokeOverprint
        {
            set
            {
                _nonStrokeOverprint = value;
                Elements.SetBoolean(Keys.op, value);
                UpdateKey();
            }
        }
        bool _nonStrokeOverprint;

        /// <summary>
        /// Sets a soft mask object.
        /// </summary>
        public PdfSoftMask SoftMask
        {
            set { Elements.SetReference(Keys.SMask, value); }
        }

        internal string Key
        {
            get { return _key; }
        }

        void UpdateKey()
        {
            _key = ((int)(1000 * _strokeAlpha)).ToString(CultureInfo.InvariantCulture) +
                         ((int)(1000 * _nonStrokeAlpha)).ToString(CultureInfo.InvariantCulture) +
                         (_strokeOverprint ? "S" : "s") + (_nonStrokeOverprint ? "N" : "n");
        }
        string _key;

        internal static string MakeKey(double alpha, bool overPaint)
        {
            string key = ((int)(1000 * alpha)).ToString(CultureInfo.InvariantCulture) + (overPaint ? "O" : "0");
            return key;
        }

        /// <summary>
        /// Common keys for all streams.
        /// </summary>
        internal sealed class Keys : KeysBase
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes;
            /// must be ExtGState for a graphics state parameter dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Type = "/Type";

            /// <summary>
            /// (Optional; PDF 1.3) The line width (see “Line Width” on page 185).
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string LW = "/LW";

            /// <summary>
            /// (Optional; PDF 1.3) The line cap style.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string LC = "/LC";

            /// <summary>
            /// (Optional; PDF 1.3) The line join style.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string LJ = "/LJ";

            /// <summary>
            /// (Optional; PDF 1.3) The miter limit.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string ML = "/ML";

            /// <summary>
            /// (Optional; PDF 1.3) The line dash pattern, expressed as an array of the form
            /// [dashArray dashPhase], where dashArray is itself an array and dashPhase is an integer.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string D = "/D";

            /// <summary>
            /// (Optional; PDF 1.3) The name of the rendering intent.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string RI = "/RI";

            /// <summary>
            /// (Optional) A flag specifying whether to apply overprint. In PDF 1.2 and earlier,
            /// there is a single overprint parameter that applies to all painting operations.
            /// Beginning with PDF 1.3, there are two separate overprint parameters: one for stroking 
            /// and one for all other painting operations. Specifying an OP entry sets both parameters
            /// unless there is also an op entry in the same graphics state parameter dictionary, in
            /// which case the OP entry sets only the overprint parameter for stroking.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string OP = "/OP";

            /// <summary>
            /// (Optional; PDF 1.3) A flag specifying whether to apply overprint for painting operations
            /// other than stroking. If this entry is absent, the OP entry, if any, sets this parameter.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string op = "/op";

            /// <summary>
            /// (Optional; PDF 1.3) The overprint mode.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string OPM = "/OPM";

            /// <summary>
            /// (Optional; PDF 1.3) An array of the form [font size], where font is an indirect
            /// reference to a font dictionary and size is a number expressed in text space units.
            /// These two objects correspond to the operands of the Tf operator; however,
            /// the first operand is an indirect object reference instead of a resource name.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Font = "/Font";

            /// <summary>
            /// (Optional) The black-generation function, which maps the interval [0.0 1.0]
            /// to the interval [0.0 1.0].
            /// </summary>
            [KeyInfo(KeyType.Function | KeyType.Optional)]
            public const string BG = "/BG";

            /// <summary>
            /// (Optional; PDF 1.3) Same as BG except that the value may also be the name Default,
            /// denoting the black-generation function that was in effect at the start of the page.
            /// If both BG and BG2 are present in the same graphics state parameter dictionary, 
            /// BG2 takes precedence.
            /// </summary>
            [KeyInfo(KeyType.FunctionOrName | KeyType.Optional)]
            public const string BG2 = "/BG2";

            /// <summary>
            /// (Optional) The undercolor-removal function, which maps the interval
            /// [0.0 1.0] to the interval [-1.0 1.0].
            /// </summary>
            [KeyInfo(KeyType.Function | KeyType.Optional)]
            public const string UCR = "/UCR";

            /// <summary>
            /// (Optional; PDF 1.3) Same as UCR except that the value may also be the name Default,
            /// denoting the undercolor-removal function that was in effect at the start of the page.
            /// If both UCR and UCR2 are present in the same graphics state parameter dictionary, 
            /// UCR2 takes precedence.
            /// </summary>
            [KeyInfo(KeyType.FunctionOrName | KeyType.Optional)]
            public const string UCR2 = "/UCR2";

            //TR  function, array, or name
            //TR2 function, array, or name
            //HT  dictionary, stream, or name
            //FL  number
            //SM  number

            /// <summary>
            /// (Optional) A flag specifying whether to apply automatic stroke adjustment.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string SA = "/SA";

            /// <summary>
            /// (Optional; PDF 1.4) The current blend mode to be used in the transparent imaging model.
            /// </summary>
            [KeyInfo(KeyType.NameOrArray | KeyType.Optional)]
            public const string BM = "/BM";

            /// <summary>
            /// (Optional; PDF 1.4) The current soft mask, specifying the mask shape or
            /// mask opacity values to be used in the transparent imaging model.
            /// </summary>
            [KeyInfo(KeyType.NameOrDictionary | KeyType.Optional)]
            public const string SMask = "/SMask";

            /// <summary>
            /// (Optional; PDF 1.4) The current stroking alpha constant, specifying the constant 
            /// shape or constant opacity value to be used for stroking operations in the transparent
            /// imaging model.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string CA = "/CA";

            /// <summary>
            /// (Optional; PDF 1.4) Same as CA, but for nonstroking operations.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string ca = "/ca";

            /// <summary>
            /// (Optional; PDF 1.4) The alpha source flag (“alpha is shape”), specifying whether 
            /// the current soft mask and alpha constant are to be interpreted as shape values (true)
            /// or opacity values (false).
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string AIS = "/AIS";

            /// <summary>
            /// (Optional; PDF 1.4) The text knockout flag, which determines the behavior of 
            /// overlapping glyphs within a text object in the transparent imaging model.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string TK = "/TK";

            // ReSharper restore InconsistentNaming

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
