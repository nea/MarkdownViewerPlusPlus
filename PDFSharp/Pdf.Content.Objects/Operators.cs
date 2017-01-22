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

using System.Diagnostics;
using System.Collections.Generic;

namespace PdfSharp.Pdf.Content.Objects
{
    /// <summary>
    /// Represents a PDF content stream operator description.
    /// </summary>
    public sealed class OpCode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpCode"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="opcodeName">The enum value of the operator.</param>
        /// <param name="operands">The number of operands.</param>
        /// <param name="postscript">The postscript equivalent, or null, if no such operation exists.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="description">The description from Adobe PDF Reference.</param>
        internal OpCode(string name, OpCodeName opcodeName, int operands, string postscript, OpCodeFlags flags, string description)
        {
            Name = name;
            OpCodeName = opcodeName;
            Operands = operands;
            Postscript = postscript;
            Flags = flags;
            Description = description;
        }

        /// <summary>
        /// The name of the operator.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The enum value of the operator.
        /// </summary>
        public readonly OpCodeName OpCodeName;

        /// <summary>
        /// The number of operands. -1 indicates a variable number of operands.
        /// </summary>
        public readonly int Operands;

        /// <summary>
        /// The flags.
        /// </summary>
        public readonly OpCodeFlags Flags;

        /// <summary>
        /// The postscript equivalent, or null, if no such operation exists.
        /// </summary>
        public readonly string Postscript;

        /// <summary>
        /// The description from Adobe PDF Reference.
        /// </summary>
        public readonly string Description;
    }

    /// <summary>
    /// Static class with all PDF op-codes.
    /// </summary>
    public static class OpCodes
    {
        /// <summary>
        /// Operators from name.
        /// </summary>
        /// <param name="name">The name.</param>
        static public COperator OperatorFromName(string name)
        {
            COperator op = null;
            OpCode opcode = StringToOpCode[name];
            if (opcode != null)
            {
                op = new COperator(opcode);
            }
            else
            {
                Debug.Assert(false, "Unknown operator in PDF content stream.");
            }
            return op;
        }

        /// <summary>
        /// Initializes the <see cref="OpCodes"/> class.
        /// </summary>
        static OpCodes()
        {
            StringToOpCode = new Dictionary<string, OpCode>();
            for (int idx = 0; idx < ops.Length; idx++)
            {
                OpCode op = ops[idx];
                StringToOpCode.Add(op.Name, op);
            }
        }
        static readonly Dictionary<string, OpCode> StringToOpCode;

        // ReSharper disable InconsistentNaming

        static readonly OpCode Dictionary = new OpCode("Dictionary", OpCodeName.Dictionary, -1, "name, dictionary", OpCodeFlags.None,
            "E.g.: /Name << ... >>");

        static readonly OpCode b = new OpCode("b", OpCodeName.b, 0, "closepath, fill, stroke", OpCodeFlags.None,
            "Close, fill, and stroke path using nonzero winding number");

        static readonly OpCode B = new OpCode("B", OpCodeName.B, 0, "fill, stroke", OpCodeFlags.None,
            "Fill and stroke path using nonzero winding number rule");

        static readonly OpCode bx = new OpCode("b*", OpCodeName.bx, 0, "closepath, eofill, stroke", OpCodeFlags.None,
            "Close, fill, and stroke path using even-odd rule");

        static readonly OpCode Bx = new OpCode("B*", OpCodeName.Bx, 0, "eofill, stroke", OpCodeFlags.None,
            "Fill and stroke path using even-odd rule");

        static readonly OpCode BDC = new OpCode("BDC", OpCodeName.BDC, -1, null, OpCodeFlags.None,
            "(PDF 1.2) Begin marked-content sequence with property list");

        static readonly OpCode BI = new OpCode("BI", OpCodeName.BI, 0, null, OpCodeFlags.None,
            "Begin inline image object");

        static readonly OpCode BMC = new OpCode("BMC", OpCodeName.BMC, 1, null, OpCodeFlags.None,
            "(PDF 1.2) Begin marked-content sequence");

        static readonly OpCode BT = new OpCode("BT", OpCodeName.BT, 0, null, OpCodeFlags.None,
            "Begin text object");

        static readonly OpCode BX = new OpCode("BX", OpCodeName.BX, 0, null, OpCodeFlags.None,
            "(PDF 1.1) Begin compatibility section");

        static readonly OpCode c = new OpCode("c", OpCodeName.c, 6, "curveto", OpCodeFlags.None,
            "Append curved segment to path (three control points)");

        static readonly OpCode cm = new OpCode("cm", OpCodeName.cm, 6, "concat", OpCodeFlags.None,
            "Concatenate matrix to current transformation matrix");

        static readonly OpCode CS = new OpCode("CS", OpCodeName.CS, 1, "setcolorspace", OpCodeFlags.None,
            "(PDF 1.1) Set color space for stroking operations");

        static readonly OpCode cs = new OpCode("cs", OpCodeName.cs, 1, "setcolorspace", OpCodeFlags.None,
            "(PDF 1.1) Set color space for nonstroking operations");

        static readonly OpCode d = new OpCode("d", OpCodeName.d, 2, "setdash", OpCodeFlags.None,
            "Set line dash pattern");

        static readonly OpCode d0 = new OpCode("d0", OpCodeName.d0, 2, "setcharwidth", OpCodeFlags.None,
            "Set glyph width in Type 3 font");

        static readonly OpCode d1 = new OpCode("d1", OpCodeName.d1, 6, "setcachedevice", OpCodeFlags.None,
            "Set glyph width and bounding box in Type 3 font");

        static readonly OpCode Do = new OpCode("Do", OpCodeName.Do, 1, null, OpCodeFlags.None,
            "Invoke named XObject");

        static readonly OpCode DP = new OpCode("DP", OpCodeName.DP, 2, null, OpCodeFlags.None,
            "(PDF 1.2) Define marked-content point with property list");

        static readonly OpCode EI = new OpCode("EI", OpCodeName.EI, 0, null, OpCodeFlags.None,
            "End inline image object");

        static readonly OpCode EMC = new OpCode("EMC", OpCodeName.EMC, 0, null, OpCodeFlags.None,
            "(PDF 1.2) End marked-content sequence");

        static readonly OpCode ET = new OpCode("ET", OpCodeName.ET, 0, null, OpCodeFlags.None,
            "End text object");

        static readonly OpCode EX = new OpCode("EX", OpCodeName.EX, 0, null, OpCodeFlags.None,
            "(PDF 1.1) End compatibility section");

        static readonly OpCode f = new OpCode("f", OpCodeName.f, 0, "fill", OpCodeFlags.None,
            "Fill path using nonzero winding number rule");

        static readonly OpCode F = new OpCode("F", OpCodeName.F, 0, "fill", OpCodeFlags.None,
            "Fill path using nonzero winding number rule (obsolete)");

        static readonly OpCode fx = new OpCode("f*", OpCodeName.fx, 0, "eofill", OpCodeFlags.None,
            "Fill path using even-odd rule");

        static readonly OpCode G = new OpCode("G", OpCodeName.G, 1, "setgray", OpCodeFlags.None,
            "Set gray level for stroking operations");

        static readonly OpCode g = new OpCode("g", OpCodeName.g, 1, "setgray", OpCodeFlags.None,
            "Set gray level for nonstroking operations");

        static readonly OpCode gs = new OpCode("gs", OpCodeName.gs, 1, null, OpCodeFlags.None,
            "(PDF 1.2) Set parameters from graphics state parameter dictionary");

        static readonly OpCode h = new OpCode("h", OpCodeName.h, 0, "closepath", OpCodeFlags.None,
            "Close subpath");

        static readonly OpCode i = new OpCode("i", OpCodeName.i, 1, "setflat", OpCodeFlags.None,
            "Set flatness tolerance");

        static readonly OpCode ID = new OpCode("ID", OpCodeName.ID, 0, null, OpCodeFlags.None,
            "Begin inline image data");

        static readonly OpCode j = new OpCode("j", OpCodeName.j, 1, "setlinejoin", OpCodeFlags.None,
            "Set line join style");

        static readonly OpCode J = new OpCode("J", OpCodeName.J, 1, "setlinecap", OpCodeFlags.None,
            "Set line cap style");

        static readonly OpCode K = new OpCode("K", OpCodeName.K, 4, "setcmykcolor", OpCodeFlags.None,
            "Set CMYK color for stroking operations");

        static readonly OpCode k = new OpCode("k", OpCodeName.k, 4, "setcmykcolor", OpCodeFlags.None,
            "Set CMYK color for nonstroking operations");

        static readonly OpCode l = new OpCode("l", OpCodeName.l, 2, "lineto", OpCodeFlags.None,
            "Append straight line segment to path");

        static readonly OpCode m = new OpCode("m", OpCodeName.m, 2, "moveto", OpCodeFlags.None,
            "Begin new subpath");

        static readonly OpCode M = new OpCode("M", OpCodeName.M, 1, "setmiterlimit", OpCodeFlags.None,
            "Set miter limit");

        static readonly OpCode MP = new OpCode("MP", OpCodeName.MP, 1, null, OpCodeFlags.None,
            "(PDF 1.2) Define marked-content point");

        static readonly OpCode n = new OpCode("n", OpCodeName.n, 0, null, OpCodeFlags.None,
            "End path without filling or stroking");

        static readonly OpCode q = new OpCode("q", OpCodeName.q, 0, "gsave", OpCodeFlags.None,
            "Save graphics state");

        static readonly OpCode Q = new OpCode("Q", OpCodeName.Q, 0, "grestore", OpCodeFlags.None,
            "Restore graphics state");

        static readonly OpCode re = new OpCode("re", OpCodeName.re, 4, null, OpCodeFlags.None,
            "Append rectangle to path");

        static readonly OpCode RG = new OpCode("RG", OpCodeName.RG, 3, "setrgbcolor", OpCodeFlags.None,
            "Set RGB color for stroking operations");

        static readonly OpCode rg = new OpCode("rg", OpCodeName.rg, 3, "setrgbcolor", OpCodeFlags.None,
            "Set RGB color for nonstroking operations");

        static readonly OpCode ri = new OpCode("ri", OpCodeName.ri, 1, null, OpCodeFlags.None,
            "Set color rendering intent");

        static readonly OpCode s = new OpCode("s", OpCodeName.s, 0, "closepath,stroke", OpCodeFlags.None,
            "Close and stroke path");

        static readonly OpCode S = new OpCode("S", OpCodeName.S, 0, "stroke", OpCodeFlags.None,
            "Stroke path");

        static readonly OpCode SC = new OpCode("SC", OpCodeName.SC, -1, "setcolor", OpCodeFlags.None,
            "(PDF 1.1) Set color for stroking operations");

        static readonly OpCode sc = new OpCode("sc", OpCodeName.sc, -1, "setcolor", OpCodeFlags.None,
            "(PDF 1.1) Set color for nonstroking operations");

        static readonly OpCode SCN = new OpCode("SCN", OpCodeName.SCN, -1, "setcolor", OpCodeFlags.None,
            "(PDF 1.2) Set color for stroking operations (ICCBased and special color spaces)");

        static readonly OpCode scn = new OpCode("scn", OpCodeName.scn, -1, "setcolor", OpCodeFlags.None,
            "(PDF 1.2) Set color for nonstroking operations (ICCBased and special color spaces)");

        static readonly OpCode sh = new OpCode("sh", OpCodeName.sh, 1, "shfill", OpCodeFlags.None,
            "(PDF 1.3) Paint area defined by shading pattern");

        static readonly OpCode Tx = new OpCode("T*", OpCodeName.Tx, 0, null, OpCodeFlags.None,
            "Move to start of next text line");

        static readonly OpCode Tc = new OpCode("Tc", OpCodeName.Tc, 1, null, OpCodeFlags.None,
            "Set character spacing");

        static readonly OpCode Td = new OpCode("Td", OpCodeName.Td, 2, null, OpCodeFlags.None,
            "Move text position");

        static readonly OpCode TD = new OpCode("TD", OpCodeName.TD, 2, null, OpCodeFlags.None,
            "Move text position and set leading");

        static readonly OpCode Tf = new OpCode("Tf", OpCodeName.Tf, 2, "selectfont", OpCodeFlags.None,
            "Set text font and size");

        static readonly OpCode Tj = new OpCode("Tj", OpCodeName.Tj, 1, "show", OpCodeFlags.TextOut,
            "Show text");

        static readonly OpCode TJ = new OpCode("TJ", OpCodeName.TJ, 1, null, OpCodeFlags.TextOut,
            "Show text, allowing individual glyph positioning");

        static readonly OpCode TL = new OpCode("TL", OpCodeName.TL, 1, null, OpCodeFlags.None,
            "Set text leading");

        static readonly OpCode Tm = new OpCode("Tm", OpCodeName.Tm, 6, null, OpCodeFlags.None,
            "Set text matrix and text line matrix");

        static readonly OpCode Tr = new OpCode("Tr", OpCodeName.Tr, 1, null, OpCodeFlags.None,
            "Set text rendering mode");

        static readonly OpCode Ts = new OpCode("Ts", OpCodeName.Ts, 1, null, OpCodeFlags.None,
            "Set text rise");

        static readonly OpCode Tw = new OpCode("Tw", OpCodeName.Tw, 1, null, OpCodeFlags.None,
            "Set word spacing");

        static readonly OpCode Tz = new OpCode("Tz", OpCodeName.Tz, 1, null, OpCodeFlags.None,
            "Set horizontal text scaling");

        static readonly OpCode v = new OpCode("v", OpCodeName.v, 4, "curveto", OpCodeFlags.None,
            "Append curved segment to path (initial point replicated)");

        static readonly OpCode w = new OpCode("w", OpCodeName.w, 1, "setlinewidth", OpCodeFlags.None,
            "Set line width");

        static readonly OpCode W = new OpCode("W", OpCodeName.W, 0, "clip", OpCodeFlags.None,
            "Set clipping path using nonzero winding number rule");

        static readonly OpCode Wx = new OpCode("W*", OpCodeName.Wx, 0, "eoclip", OpCodeFlags.None,
            "Set clipping path using even-odd rule");

        static readonly OpCode y = new OpCode("y", OpCodeName.y, 4, "curveto", OpCodeFlags.None,
            "Append curved segment to path (final point replicated)");

        static readonly OpCode QuoteSingle = new OpCode("'", OpCodeName.QuoteSingle, 1, null, OpCodeFlags.TextOut,
            "Move to next line and show text");

        static readonly OpCode QuoteDbl = new OpCode("\"", OpCodeName.QuoteDbl, 3, null, OpCodeFlags.TextOut,
            "Set word and character spacing, move to next line, and show text");

        /// <summary>
        /// Array of all OpCodes.
        /// </summary>
        static readonly OpCode[] ops = // new OpCode[]
            { 
                // Must be defined behind the code above to ensure that the values are initialized.
                Dictionary,
                b, B, bx, Bx, BDC, BI, BMC, BT, BX, c, cm, CS, cs, d, d0, d1, Do,
                DP, EI, EMC, ET, EX, f, F, fx, G, g, gs, h, i, ID, j, J, K, k, l, m, M, MP,
                n, q, Q, re, RG, rg, ri, s, S, SC, sc, SCN, scn, sh,
                Tx, Tc, Td, TD, Tf, Tj, TJ, TL, Tm, Tr, Ts, Tw, Tz, v, w, W, Wx, y,
                QuoteSingle, QuoteDbl
            };
        // ReSharper restore InconsistentNaming
    }
}
