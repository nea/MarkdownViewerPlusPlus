#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   David Stephensen
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
using System.Diagnostics;
#if GDI
using System.Drawing;
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
#endif

// WPFHACK
#pragma warning disable 162

namespace PdfSharp.Drawing.BarCodes
{
    /// <summary>
    /// Creates the XImage object for a DataMatrix.
    /// </summary>
    internal class DataMatrixImage
    {
        public static XImage GenerateMatrixImage(string text, string encoding, int rows, int columns)
        {
            DataMatrixImage dataMatrixImage = new DataMatrixImage(text, encoding, rows, columns);
            return dataMatrixImage.DrawMatrix();
        }

        public DataMatrixImage(string text, string encoding, int rows, int columns)
        {
            _text = text;
            _encoding = encoding;
            _rows = rows;
            _columns = columns;
        }

        string _encoding;
        readonly string _text;
        readonly int _rows;
        readonly int _columns;

        /// <summary>
        /// Possible ECC200 Matrixes
        /// </summary>
        static Ecc200Block[] ecc200Sizes =
    {
      new Ecc200Block( 10,  10, 10, 10,    3,   3,  5),    //
      new Ecc200Block( 12,  12, 12, 12,    5,   5,  7),    //
      new Ecc200Block(  8,  18,  8, 18,    5,   5,  7),    //
      new Ecc200Block( 14,  14, 14, 14,    8,   8, 10),    //
      new Ecc200Block(  8,  32,  8, 16,   10,  10, 11),    //
      new Ecc200Block( 16,  16, 16, 16,   12,  12, 12),    //
      new Ecc200Block( 12,  26, 12, 26,   16,  16, 14),    //
      new Ecc200Block( 18,  18, 18, 18,   18,  18, 14),    //
      new Ecc200Block( 20,  20, 20, 20,   22,  22, 18),    //
      new Ecc200Block( 12,  36, 12, 18,   22,  22, 18),    //
      new Ecc200Block( 22,  22, 22, 22,   30,  30, 20),    // Post
      new Ecc200Block( 16,  36, 16, 18,   32,  32, 24),    //
      new Ecc200Block( 24,  24, 24, 24,   36,  36, 24),    //
      new Ecc200Block( 26,  26, 26, 26,   44,  44, 28),    // Post
      new Ecc200Block( 16,  48, 16, 24,   49,  49, 28),    //
      new Ecc200Block( 32,  32, 16, 16,   62,  62, 36),    //
      new Ecc200Block( 36,  36, 18, 18,   86,  86, 42),    //
      new Ecc200Block( 40,  40, 20, 20,  114, 114, 48),    //
      new Ecc200Block( 44,  44, 22, 22,  144, 144, 56),    //
      new Ecc200Block( 48,  48, 24, 24,  174, 174, 68),    //
      new Ecc200Block( 52,  52, 26, 26,  204, 102, 42),    //
      new Ecc200Block( 64,  64, 16, 16,  280, 140, 56),    //
      new Ecc200Block( 72,  72, 18, 18,  368,  92, 36),    //
      new Ecc200Block( 80,  80, 20, 20,  456, 114, 48),    //
      new Ecc200Block( 88,  88, 22, 22,  576, 144, 56),    //
      new Ecc200Block( 96,  96, 24, 24,  696, 174, 68),    //
      new Ecc200Block(104, 104, 26, 26,  816, 136, 56),    //
      new Ecc200Block(120, 120, 20, 20, 1050, 175, 68),    //
      new Ecc200Block(132, 132, 22, 22, 1304, 163, 62),    //
      new Ecc200Block(144, 144, 24, 24, 1558, 156, 62),    // 156*4+155*2
      new Ecc200Block(  0,   0,  0,  0,    0,    0, 0)     // terminate
    };

        public XImage DrawMatrix()
        {
            return CreateImage(DataMatrix(), _rows, _columns);
        }

        /// <summary>
        /// Creates the DataMatrix code.
        /// </summary>
        internal char[] DataMatrix()
        {
            int matrixColumns = _columns;
            int matrixRows = _rows;
            int ecc = 200;
            if (String.IsNullOrEmpty(_encoding))
                _encoding = new String('a', _text.Length);
            int len = 0;
            int maxlen = 0;
            int ecclen = 0;
            char[] grid = null;

            if (matrixColumns != 0 && matrixRows != 0 && (matrixColumns & 1) != 0 && (matrixRows & 1) != 0 && ecc == 200)
                throw new ArgumentException(BcgSR.DataMatrixNotSupported);

            grid = Iec16022Ecc200(matrixColumns, matrixRows, _encoding, _text.Length, _text, len, maxlen, ecclen);

            if (grid == null || matrixColumns == 0)
                throw new ArgumentException(BcgSR.DataMatrixNull); //DaSt: ever happen?
            return grid;
        }

        /// <summary>
        /// Encodes the DataMatrix.
        /// </summary>
        internal char[] Iec16022Ecc200(int columns, int rows, string encoding, int barcodeLength, string barcode, int len, int max, int ecc)
        {
            char[] binary = new char[3000];  // encoded raw data and ecc to place in barcode
            Ecc200Block matrix = new Ecc200Block(0, 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < 3000; i++)
                binary[i] = (char)0;

            foreach (Ecc200Block eccmatrix in ecc200Sizes)
            {
                matrix = eccmatrix;
                if (matrix.Width == columns && matrix.Height == rows)
                    break;
            }

            if (matrix.Width == 0)
                throw new ArgumentException(BcgSR.DataMatrixInvalid(columns, rows));

            if (!Ecc200Encode(ref binary, matrix.Bytes, barcode, barcodeLength, encoding, ref len))
                throw new ArgumentException(BcgSR.DataMatrixTooBig);

            // ecc code
            Ecc200(binary, matrix.Bytes, matrix.DataBlock, matrix.RSBlock);
            // placement
            int x;
            int y;
            int NR;
            int[] places;
            int NC = columns - 2 * (columns / matrix.CellWidth);
            NR = rows - 2 * (rows / matrix.CellHeight);
            places = new int[NC * NR];
            Ecc200Placement(ref places, NR, NC);
            char[] grid = new char[columns * rows];
            for (y = 0; y < rows; y += matrix.CellHeight)
            {
                for (x = 0; x < columns; x++)
                    grid[y * columns + x] = (char)1;
                for (x = 0; x < columns; x += 2)
                    grid[(y + matrix.CellHeight - 1) * columns + x] = (char)1;
            }

            for (x = 0; x < columns; x += matrix.CellWidth)
            {
                for (y = 0; y < rows; y++)
                    grid[y * columns + x] = (char)1;
                for (y = 0; y < rows; y += 2)
                    grid[y * columns + x + matrix.CellWidth - 1] = (char)1;
            }

            for (y = 0; y < NR; y++)
            {
                for (x = 0; x < NC; x++)
                {
                    int v = places[(NR - y - 1) * NC + x];
                    if (v == 1 || v > 7 && ((binary[(v >> 3) - 1] & (1 << (v & 7))) != 0))
                        grid[(1 + y + 2 * (y / (matrix.CellHeight - 2))) * columns + 1 + x + 2 * (x / (matrix.CellWidth - 2))] = (char)1;
                }
            }
            return grid;
        }

        /// <summary>
        /// Encodes the barcode with the DataMatrix ECC200 Encoding.
        /// </summary>
        internal bool Ecc200Encode(ref char[] t, int targetLength, string s, int sourceLength, string encoding, ref int len)
        {
            char enc = 'a';              // start in ASCII encoding mode
            int targetposition = 0;
            int sourceposition = 0;
            if (encoding.Length < sourceLength)
                return false;

            // do the encoding
            while (sourceposition < sourceLength && targetposition < targetLength)
            {
                char newenc = enc;        // suggest new encoding
                if (targetLength - targetposition <= 1 && (enc == 'c' || enc == 't') || targetLength - targetposition <= 2 && enc == 'x')
                    enc = 'a';             // auto revert to ASCII
#if !SILVERLIGHT
                // StL: Who wrote this nonsense?
                //newenc = char.Parse(encoding[sourceposition].ToString(CultureInfo.InvariantCulture).ToLower());
                newenc = char.ToLower(encoding[sourceposition]);
#else
        throw new NotImplementedException("char.Parse");
#endif
                switch (newenc)
                {                         // encode character
                    case 'c':                // C40
                    case 't':                // Text
                    case 'x':                // X12
                        {
                            char[] output = new char[6];
                            char p = (char)0;

                            string e = null;
                            string s2 = "!\"#$%&'()*+,-./:;<=>?@[\\]_";
                            string s3 = null;
                            if (newenc == 'c')
                            {
                                e = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                                s3 = "`abcdefghijklmnopqrstuvwxyz{|}~±";
                            }
                            if (newenc == 't')
                            {
                                e = " 0123456789abcdefghijklmnopqrstuvwxyz";
                                s3 = "`ABCDEFGHIJKLMNOPQRSTUVWXYZ{|}~±";
                            }
                            if (newenc == 'x')
                                e = " 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ\r*>";
                            do
                            {
                                char c = s[sourceposition++];
                                char w;
                                if ((c & 0x80) != 0)
                                {
                                    if (newenc == 'x')
                                    {
                                        //                     fprintf (stderr, "Cannot encode char 0x%02X in X12\n", c);
                                        return false;
                                    }
                                    c &= (char)0x7f;
                                    output[p++] = (char)1;
                                    output[p++] = (char)30;
                                }
                                w = e.IndexOf(c) == -1 ? (char)0 : e[e.IndexOf(c)];
                                if (w != (char)0)
                                    output[p++] = (char)((e.IndexOf(w) + 3) % 40);
                                else
                                {
                                    if (newenc == 'x')
                                    {
                                        //fprintf (stderr, "Cannot encode char 0x%02X in X12\n", c);
                                        return false;
                                    }
                                    if (c < 32)
                                    {             // shift 1
                                        output[p++] = (char)0;
                                        output[p++] = c;
                                    }
                                    else
                                    {
                                        w = s2.IndexOf(c) == -1 ? (char)0 : (char)s2.IndexOf(c);
                                        if (w != (char)0)
                                        {          // shift 2
                                            output[p++] = (char)1;
                                            output[p++] = w;
                                        }
                                        else
                                        {
                                            w = s3.IndexOf(c) == -1 ? (char)0 : (char)s3.IndexOf(c);
                                            if (w != (char)0)
                                            {
                                                output[p++] = (char)2;
                                                output[p++] = w;
                                            }
                                            else
                                                //fprintf (stderr, "Could not encode 0x%02X, should not happen\n", c);
                                                return false;
                                        }
                                    }
                                }

                                if (p == 2 && targetposition + 2 == targetLength && sourceposition == sourceLength)
                                    output[p++] = (char)0; // shift 1 pad at end
                                while (p >= 3)
                                {
                                    int v = output[0] * 1600 + output[1] * 40 + output[2] + 1;
                                    if (enc != newenc)
                                    {
                                        if (enc == 'c' || enc == 't' || enc == 'x')
                                            t[targetposition++] = (char)254;  // escape C40/text/X12
                                        else if (enc == 'x')
                                            t[targetposition++] = (char)0x7C; // escape EDIFACT
                                        if (newenc == 'c')
                                            t[targetposition++] = (char)230;
                                        if (newenc == 't')
                                            t[targetposition++] = (char)239;
                                        if (newenc == 'x')
                                            t[targetposition++] = (char)238;
                                        enc = newenc;
                                    }
                                    t[targetposition++] = (char)(v >> 8);
                                    t[targetposition++] = (char)(v & 0xFF);
                                    p -= (char)3;
                                    output[0] = output[3];
                                    output[1] = output[4];
                                    output[2] = output[5];
                                }
                            }
                            while (p != (char)0 && sourceposition < sourceLength);
                        }
                        break;
                    case 'e':                // EDIFACT
                        {
                            char[] output = new char[4];
                            char p = (char)0;
                            if (enc != newenc)
                            {                   // can only be from C40/Text/X12
                                t[targetposition++] = (char)254;
                                enc = 'a';
                            }
                            while (sourceposition < sourceLength && /*encoding[sourceposition].ToString(CultureInfo.InvariantCulture).ToLower() == "e"*/
                              char.ToLower(encoding[sourceposition]) == 'e' && p < 4)
                                output[p++] = s[sourceposition++];
                            if (p < 4)
                            {
                                output[p++] = (char)0x1F;
                                enc = 'a';
                            }                   // termination
                            t[targetposition] = (char)((s[0] & 0x3F) << 2);
                            t[targetposition++] |= (char)((s[1] & 0x30) >> 4);
                            t[targetposition] = (char)((s[1] & 0x0F) << 4);
                            if (p == 2)
                                targetposition++;
                            else
                            {
                                t[targetposition++] |= (char)((s[2] & 0x3C) >> 2);
                                t[targetposition] = (char)((s[2] & 0x03) << 6);
                                t[targetposition++] |= (char)(s[3] & 0x3F);
                            }
                        }
                        break;
                    case 'a':                // ASCII
                        if (enc != newenc)
                        {
                            if (enc == 'c' || enc == 't' || enc == 'x')
                                t[targetposition++] = (char)254;   // escape C40/text/X12
                            else
                                t[targetposition++] = (char)0x7C;  // escape EDIFACT
                        }
                        enc = 'a';
                        if (sourceLength - sourceposition >= 2 && char.IsDigit(s[sourceposition]) && char.IsDigit(s[sourceposition + 1]))
                        {
                            t[targetposition++] = (char)((s[sourceposition] - '0') * 10 + s[sourceposition + 1] - '0' + 130);
                            sourceposition += 2;
                        }
                        else if (s[sourceposition] > 127)
                        {
                            t[targetposition++] = (char)235;
                            t[targetposition++] = (char)(s[sourceposition++] - 127);
                        }
                        else
                            t[targetposition++] = (char)(s[sourceposition++] + 1);
                        break;
                    case 'b':                // Binary
                        {
                            int l = 0;          // how much to encode
                            if (encoding != null)
                            {
                                int p;
                                for (p = sourceposition; p < sourceLength && /*encoding[p].ToString(CultureInfo.InvariantCulture).ToLower() == "b"*/ char.ToLower(encoding[p]) == 'b'; p++)
                                    l++;
                            }
                            t[targetposition++] = (char)231;      // base256
                            if (l < 250)
                            {
                                t[targetposition] = (char)State255(l, targetposition);
                                targetposition++;
                            }
                            else
                            {
                                t[targetposition] = (char)State255(249 + (l / 250), targetposition);
                                targetposition++;
                                t[targetposition] = (char)State255(l % 250, targetposition);
                                targetposition++;
                            }
                            while (l-- != 0 && targetposition < targetLength)
                            {
                                t[targetposition] = (char)State255(s[sourceposition++], targetposition);
                                targetposition++;
                            }
                            enc = 'a';          // reverse to ASCII at end
                        }
                        break;
                        //      default:
                        //         fprintf (stderr, "Unknown encoding %c\n", newenc);
                        //         return 0;              // failed
                }
            }
            if (len != 0)
                len = targetposition;
            if (targetposition < targetLength && enc != 'a')
            {
                if (enc == 'c' || enc == 'x' || enc == 't')
                    t[targetposition++] = (char)254;         // escape X12/C40/Text
                else
                    t[targetposition++] = (char)0x7C;        // escape EDIFACT
            }

            if (targetposition < targetLength)
                t[targetposition++] = (char)129;            // pad

            while (targetposition < targetLength)
            {                            // more padding
                int v = 129 + (((targetposition + 1) * 149) % 253) + 1;       // see Annex H
                if (v > 254)
                    v -= 254;
                t[targetposition++] = (char)v;
            }
            if (targetposition > targetLength || sourceposition < sourceLength)
                return false;                 // did not fit
            return true;                    // OK 
        }

        int State255(int value, int position)
        {
            return ((value + (((position + 1) * 149) % 255) + 1) % 256);
        }

        /// <summary>
        /// Places the data in the right positions according to Annex M of the ECC200 specification.
        /// </summary>
        void Ecc200Placement(ref int[] array, int NR, int NC)
        {
            int r;
            int c;
            int p;

            // invalidate
            for (r = 0; r < NR; r++)
                for (c = 0; c < NC; c++)
                    array[r * NC + c] = 0;
            // start
            p = 1;
            r = 4;
            c = 0;
            do
            {
                // check corner
                if (r == NR && (c == 0))
                    Ecc200PlacementCornerA(ref array, NR, NC, p++);
                if (r == NR - 2 && c == 0 && ((NC % 4) != 0))
                    Ecc200PlacementCornerB(ref array, NR, NC, p++);
                if (r == NR - 2 && c == 0 && ((NC % 8) == 4))
                    Ecc200PlacementCornerC(ref array, NR, NC, p++);
                if (r == NR + 4 && c == 2 && ((NC % 8) == 0))
                    Ecc200PlacementCornerD(ref array, NR, NC, p++);
                // up/right
                do
                {
                    if (r < NR && c >= 0 && array[r * NC + c] == 0)
                        Ecc200PlacementBlock(ref array, NR, NC, r, c, p++);
                    r -= 2;
                    c += 2;
                }
                while (r >= 0 && c < NC);
                r++;
                c += 3;
                // down/left
                do
                {
                    if (r >= 0 && c < NC && array[r * NC + c] == 0)
                        Ecc200PlacementBlock(ref array, NR, NC, r, c, p++);
                    r += 2;
                    c -= 2;
                }
                while (r < NR && c >= 0);
                r += 3;
                c++;
            }
            while (r < NR || c < NC);
            // unfilled corner
            if (array[NR * NC - 1] == 0)
                array[NR * NC - 1] = array[NR * NC - NC - 2] = 1;
        }

        /// <summary>
        /// Places the ECC200 bits in the right positions.
        /// </summary>
        void Ecc200PlacementBit(ref int[] array, int NR, int NC, int r, int c, int p, int b)
        {
            if (r < 0)
            {
                r += NR;
                c += 4 - ((NR + 4) % 8);
            }
            if (c < 0)
            {
                c += NC;
                r += 4 - ((NC + 4) % 8);
            }
            array[r * NC + c] = (p << 3) + b;
        }

        void Ecc200PlacementBlock(ref int[] array, int NR, int NC, int r, int c, int p)
        {
            Ecc200PlacementBit(ref array, NR, NC, r - 2, c - 2, p, 7);
            Ecc200PlacementBit(ref array, NR, NC, r - 2, c - 1, p, 6);
            Ecc200PlacementBit(ref array, NR, NC, r - 1, c - 2, p, 5);
            Ecc200PlacementBit(ref array, NR, NC, r - 1, c - 1, p, 4);
            Ecc200PlacementBit(ref array, NR, NC, r - 1, c - 0, p, 3);
            Ecc200PlacementBit(ref array, NR, NC, r - 0, c - 2, p, 2);
            Ecc200PlacementBit(ref array, NR, NC, r - 0, c - 1, p, 1);
            Ecc200PlacementBit(ref array, NR, NC, r - 0, c - 0, p, 0);
        }

        void Ecc200PlacementCornerA(ref int[] array, int NR, int NC, int p)
        {
            Ecc200PlacementBit(ref array, NR, NC, NR - 1, 0, p, 7);
            Ecc200PlacementBit(ref array, NR, NC, NR - 1, 1, p, 6);
            Ecc200PlacementBit(ref array, NR, NC, NR - 1, 2, p, 5);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 2, p, 4);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 1, p, 3);
            Ecc200PlacementBit(ref array, NR, NC, 1, NC - 1, p, 2);
            Ecc200PlacementBit(ref array, NR, NC, 2, NC - 1, p, 1);
            Ecc200PlacementBit(ref array, NR, NC, 3, NC - 1, p, 0);
        }

        void Ecc200PlacementCornerB(ref int[] array, int NR, int NC, int p)
        {
            Ecc200PlacementBit(ref array, NR, NC, NR - 3, 0, p, 7);
            Ecc200PlacementBit(ref array, NR, NC, NR - 2, 0, p, 6);
            Ecc200PlacementBit(ref array, NR, NC, NR - 1, 0, p, 5);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 4, p, 4);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 3, p, 3);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 2, p, 2);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 1, p, 1);
            Ecc200PlacementBit(ref array, NR, NC, 1, NC - 1, p, 0);
        }

        void Ecc200PlacementCornerC(ref int[] array, int NR, int NC, int p)
        {
            Ecc200PlacementBit(ref array, NR, NC, NR - 3, 0, p, 7);
            Ecc200PlacementBit(ref array, NR, NC, NR - 2, 0, p, 6);
            Ecc200PlacementBit(ref array, NR, NC, NR - 1, 0, p, 5);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 2, p, 4);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 1, p, 3);
            Ecc200PlacementBit(ref array, NR, NC, 1, NC - 1, p, 2);
            Ecc200PlacementBit(ref array, NR, NC, 2, NC - 1, p, 1);
            Ecc200PlacementBit(ref array, NR, NC, 3, NC - 1, p, 0);
        }

        void Ecc200PlacementCornerD(ref int[] array, int NR, int NC, int p)
        {
            Ecc200PlacementBit(ref array, NR, NC, NR - 1, 0, p, 7);
            Ecc200PlacementBit(ref array, NR, NC, NR - 1, NC - 1, p, 6);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 3, p, 5);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 2, p, 4);
            Ecc200PlacementBit(ref array, NR, NC, 0, NC - 1, p, 3);
            Ecc200PlacementBit(ref array, NR, NC, 1, NC - 3, p, 2);
            Ecc200PlacementBit(ref array, NR, NC, 1, NC - 2, p, 1);
            Ecc200PlacementBit(ref array, NR, NC, 1, NC - 1, p, 0);
        }

        /// <summary>
        /// Calculate and append the Reed Solomon Code.
        /// </summary>
        void Ecc200(char[] binary, int bytes, int datablock, int rsblock)
        {
            int blocks = (bytes + 2) / datablock;
            int b;
            InitGalois(0x12d);
            InitReedSolomon(rsblock, 1);
            for (b = 0; b < blocks; b++)
            {
                int[] buf = new int[256];
                int[] ecc = new int[256];
                int n,
                  p = 0;
                for (n = b; n < bytes; n += blocks)
                    buf[p++] = binary[n];
                EncodeReedSolomon(p, buf, ref ecc);
                p = rsblock - 1;          // comes back reversed
                for (n = b; n < rsblock * blocks; n += blocks)
                    binary[bytes + n] = (char)ecc[p--];
            }
        }

        static int gfpoly;
        static int symsize;             // in bits
        static int logmod;              // 2**symsize - 1
        static int rlen;

        static int[] log = null;
        static int[] alog = null;
        static int[] rspoly = null;

        /// <summary>
        /// Initialize the Galois Field.
        /// </summary>
        /// <param name="poly"></param>
        public static void InitGalois(int poly)
        {
            int m;
            int b;
            int p;
            int v;

            // Return storage from previous setup
            if (log != null)
            {
                log = null;
                alog = null;
                rspoly = null;
            }
            // Find the top bit, and hence the symbol size
            for (b = 1, m = 0; b <= poly; b <<= 1)
                m++;
            b >>= 1;
            m--;
            gfpoly = poly;
            symsize = m;

            // Calculate the log/alog tables
            logmod = (1 << m) - 1;
            log = new int[logmod + 1];
            alog = new int[logmod];

            for (p = 1, v = 0; v < logmod; v++)
            {
                alog[v] = p;
                log[p] = v;
                p <<= 1;
                if ((p & b) != 0) //DaSt: check!
                    p ^= poly;
            }
        }

        /// <summary>
        /// Initializes the Reed-Solomon Encoder.
        /// </summary>
        public static void InitReedSolomon(int nsym, int index)
        {
            int i;
            int k;

            if (rspoly != null)
                rspoly = null;
            rspoly = new int[nsym + 1];

            rlen = nsym;

            rspoly[0] = 1;
            for (i = 1; i <= nsym; i++)
            {
                rspoly[i] = 1;
                for (k = i - 1; k > 0; k--)
                {
                    if (rspoly[k] != 0) //DaSt: check!
                        rspoly[k] = alog[(log[rspoly[k]] + index) % logmod];
                    rspoly[k] ^= rspoly[k - 1];
                }
                rspoly[0] = alog[(log[rspoly[0]] + index) % logmod];
                index++;
            }
        }

        /// <summary>
        /// Encodes the Reed-Solomon encoding
        /// </summary>
        public void EncodeReedSolomon(int length, int[] data, ref int[] result)
        {
            int i;
            int k;
            int m;
            for (i = 0; i < rlen; i++)
                result[i] = 0;
            for (i = 0; i < length; i++)
            {
                m = result[rlen - 1] ^ data[i];
                for (k = rlen - 1; k > 0; k--)
                {
                    if ((m != 0) && (rspoly[k] != 0)) //DaSt: check!
                        result[k] = result[k - 1] ^ alog[(log[m] + log[rspoly[k]]) % logmod];
                    else
                        result[k] = result[k - 1];
                }
                if ((m != 0) && (rspoly[0] != 0)) //DaSt: check!
                    result[0] = alog[(log[m] + log[rspoly[0]]) % logmod];
                else
                    result[0] = 0;
            }
        }

        /// <summary>
        /// Creates a DataMatrix image object.
        /// </summary>
        /// <param name="code">A hex string like "AB 08 C3...".</param>
        /// <param name="size">I.e. 26 for a 26x26 matrix</param>
        public XImage CreateImage(char[] code, int size)//(string code, int size)
        {
            return CreateImage(code, size, size, 10);
        }

        /// <summary>
        /// Creates a DataMatrix image object.
        /// </summary>
        public XImage CreateImage(char[] code, int rows, int columns)
        {
            return CreateImage(code, rows, columns, 10);
        }

        /// <summary>
        /// Creates a DataMatrix image object.
        /// </summary>
        public XImage CreateImage(char[] code, int rows, int columns, int pixelsize)
        {
#if GDI
            Bitmap bm = new Bitmap(columns * pixelsize, rows * pixelsize);
            using (Graphics gfx = Graphics.FromImage(bm))
            {
                gfx.FillRectangle(System.Drawing.Brushes.White, new Rectangle(0, 0, columns * pixelsize, rows * pixelsize));

                for (int i = rows - 1; i >= 0; i--)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (code[((rows - 1) - i) * columns + j] == (char)1)
                            gfx.FillRectangle(System.Drawing.Brushes.Black, j * pixelsize, i * pixelsize, pixelsize, pixelsize);
                    }
                }
            }
            XImage image = XImage.FromGdiPlusImage(bm);
            image.Interpolate = false;
            return image;
#endif
#if WPF
            // WPFHACK
            return null;
#endif
#if CORE || NETFX_CORE || UWP
            return null;
#endif
        }

        struct Ecc200Block
        {
            public readonly int Height;
            public readonly int Width;
            public readonly int CellHeight;
            public readonly int CellWidth;
            public readonly int Bytes;
            public readonly int DataBlock;
            public readonly int RSBlock;

            public Ecc200Block(int h, int w, int ch, int cw, int bytes, int dataBlock, int rsBlock)
            {
                Height = h;
                Width = w;
                CellHeight = ch;
                CellWidth = cw;
                Bytes = bytes;
                DataBlock = dataBlock;
                RSBlock = rsBlock;
            }
        }
    }
}