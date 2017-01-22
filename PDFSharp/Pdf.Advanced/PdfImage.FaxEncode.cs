#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//   Thomas Hövel
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

// Some routines were translated from LibTiff.
// LibTiff copyright notice:
// Copyright (c) 1988-1997 Sam Leffler
// Copyright (c) 1991-1997 Silicon Graphics, Inc.
//
// Permission to use, copy, modify, distribute, and sell this software and 
// its documentation for any purpose is hereby granted without fee, provided
// that (i) the above copyright notices and this permission notice appear in
// all copies of the software and related documentation, and (ii) the names of
// Sam Leffler and Silicon Graphics may not be used in any advertising or
// publicity relating to the software without the specific, prior written
// permission of Sam Leffler and Silicon Graphics.
//
// THE SOFTWARE IS PROVIDED "AS-IS" AND WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS, IMPLIED OR OTHERWISE, INCLUDING WITHOUT LIMITATION, ANY 
// WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE.  
//
// IN NO EVENT SHALL SAM LEFFLER OR SILICON GRAPHICS BE LIABLE FOR
// ANY SPECIAL, INCIDENTAL, INDIRECT OR CONSEQUENTIAL DAMAGES OF ANY KIND,
// OR ANY DAMAGES WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS,
// WHETHER OR NOT ADVISED OF THE POSSIBILITY OF DAMAGE, AND ON ANY THEORY OF 
// LIABILITY, ARISING OUT OF OR IN CONNECTION WITH THE USE OR PERFORMANCE 
// OF THIS SOFTWARE.

#endregion

#define USE_GOTO
using System;
using System.Diagnostics;

namespace PdfSharp.Pdf.Advanced
{
    partial class PdfImage
    {
        internal readonly static uint[] WhiteTerminatingCodes =
        {
              0x35, 8, //00110101 // 0
              0x07, 6, //000111
              0x07, 4, //0111
              0x08, 4, //1000
              0x0b, 4, //1011
              0x0c, 4, //1100
              0x0e, 4, //1110
              0x0f, 4, //1111
              0x13, 5, //10011
              0x14, 5, //10100
              0x07, 5, //00111    // 10
              0x08, 5, //01000
              0x08, 6, //001000
              0x03, 6, //000011
              0x34, 6, //110100
              0x35, 6, //110101
              0x2a, 6, //101010   // 16
              0x2b, 6, //101011
              0x27, 7, //0100111
              0x0c, 7, //0001100
              0x08, 7, //0001000  // 20
              0x17, 7, //0010111
              0x03, 7, //0000011
              0x04, 7, //0000100
              0x28, 7, //0101000
              0x2b, 7, //0101011
              0x13, 7, //0010011
              0x24, 7, //0100100
              0x18, 7, //0011000
              0x02, 8, //00000010
              0x03, 8, //00000011 // 30
              0x1a, 8, //00011010
              0x1b, 8, //00011011 // 32
              0x12, 8, //00010010
              0x13, 8, //00010011
              0x14, 8, //00010100
              0x15, 8, //00010101
              0x16, 8, //00010110
              0x17, 8, //00010111
              0x28, 8, //00101000
              0x29, 8, //00101001 // 40
              0x2a, 8, //00101010
              0x2b, 8, //00101011
              0x2c, 8, //00101100
              0x2d, 8, //00101101
              0x04, 8, //00000100
              0x05, 8, //00000101
              0x0a, 8, //00001010
              0x0b, 8, //00001011 // 48
              0x52, 8, //01010010
              0x53, 8, //01010011 // 50
              0x54, 8, //01010100
              0x55, 8, //01010101
              0x24, 8, //00100100
              0x25, 8, //00100101
              0x58, 8, //01011000
              0x59, 8, //01011001
              0x5a, 8, //01011010
              0x5b, 8, //01011011
              0x4a, 8, //01001010
              0x4b, 8, //01001011 // 60
              0x32, 8, //00110010
              0x33, 8, //00110011
              0x34, 8, //00110100 // 63
        };

        internal readonly static uint[] BlackTerminatingCodes =
        {
              0x37, 10, //0000110111   // 0
              0x02,  3, //010
              0x03,  2, //11
              0x02,  2, //10
              0x03,  3, //011
              0x03,  4, //0011
              0x02,  4, //0010
              0x03,  5, //00011
              0x05,  6, //000101
              0x04,  6, //000100
              0x04,  7, //0000100
              0x05,  7, //0000101
              0x07,  7, //0000111
              0x04,  8, //00000100
              0x07,  8, //00000111
              0x18,  9, //000011000
              0x17, 10, //0000010111   // 16
              0x18, 10, //0000011000
              0x08, 10, //0000001000
              0x67, 11, //00001100111
              0x68, 11, //00001101000
              0x6c, 11, //00001101100
              0x37, 11, //00000110111
              0x28, 11, //00000101000
              0x17, 11, //00000010111
              0x18, 11, //00000011000
              0xca, 12, //000011001010
              0xcb, 12, //000011001011
              0xcc, 12, //000011001100
              0xcd, 12, //000011001101
              0x68, 12, //000001101000 // 30
              0x69, 12, //000001101001
              0x6a, 12, //000001101010 // 32
              0x6b, 12, //000001101011
              0xd2, 12, //000011010010
              0xd3, 12, //000011010011
              0xd4, 12, //000011010100
              0xd5, 12, //000011010101
              0xd6, 12, //000011010110
              0xd7, 12, //000011010111
              0x6c, 12, //000001101100
              0x6d, 12, //000001101101
              0xda, 12, //000011011010
              0xdb, 12, //000011011011
              0x54, 12, //000001010100
              0x55, 12, //000001010101
              0x56, 12, //000001010110
              0x57, 12, //000001010111
              0x64, 12, //000001100100 // 48
              0x65, 12, //000001100101
              0x52, 12, //000001010010
              0x53, 12, //000001010011
              0x24, 12, //000000100100
              0x37, 12, //000000110111
              0x38, 12, //000000111000
              0x27, 12, //000000100111
              0x28, 12, //000000101000
              0x58, 12, //000001011000
              0x59, 12, //000001011001
              0x2b, 12, //000000101011
              0x2c, 12, //000000101100
              0x5a, 12, //000001011010
              0x66, 12, //000001100110
              0x67, 12, //000001100111 // 63
        };

        internal readonly static uint[] WhiteMakeUpCodes =
        {
              0x1b,  5, //11011 64          // 0
              0x12,  5, //10010 128
              0x17,  6, //010111 192
              0x37,  7, //0110111 256
              0x36,  8, //00110110 320
              0x37,  8, //00110111 384
              0x64,  8, //01100100 448
              0x65,  8, //01100101 512
              0x68,  8, //01101000 576
              0x67,  8, //01100111 640
              0xcc,  9, //011001100 704     // 10
              0xcd,  9, //011001101 768
              0xd2,  9, //011010010 832
              0xd3,  9, //011010011 896
              0xd4,  9, //011010100 960
              0xd5,  9, //011010101 1024
              0xd6,  9, //011010110 1088    // 16
              0xd7,  9, //011010111 1152
              0xd8,  9, //011011000 1216
              0xd9,  9, //011011001 1280
              0xda,  9, //011011010 1344
              0xdb,  9, //011011011 1408
              0x98,  9, //010011000 1472
              0x99,  9, //010011001 1536
              0x9a,  9, //010011010 1600
              0x18,  6, //011000    1664
              0x9b,  9, //010011011 1728
              // Common codes for white and black:
              0x08, 11, //00000001000 1792
              0x0c, 11, //00000001100 1856
              0x0d, 11, //00000001101 1920
              0x12, 12, //000000010010 1984
              0x13, 12, //000000010011 2048
              0x14, 12, //000000010100 2112 // 32
              0x15, 12, //000000010101 2176
              0x16, 12, //000000010110 2240
              0x17, 12, //000000010111 2304
              0x1c, 12, //000000011100 2368
              0x1d, 12, //000000011101 2432
              0x1e, 12, //000000011110 2496
              0x1f, 12, //000000011111 2560
              0x01, 12, //000000000001 EOL  // 40
        };

        internal readonly static uint[] BlackMakeUpCodes =
        {
              0x0f, 10, //0000001111    64   // 0
              0xc8, 12, //000011001000  128
              0xc9, 12, //000011001001  192
              0x5b, 12, //000001011011  256
              0x33, 12, //000000110011  320
              0x34, 12, //000000110100  384
              0x35, 12, //000000110101  448
              0x6c, 13, //0000001101100 512
              0x6d, 13, //0000001101101 576
              0x4a, 13, //0000001001010 640
              0x4b, 13, //0000001001011 704
              0x4c, 13, //0000001001100 768
              0x4d, 13, //0000001001101 832
              0x72, 13, //0000001110010 896
              0x73, 13, //0000001110011 960
              0x74, 13, //0000001110100 1024
              0x75, 13, //0000001110101 1088 // 16
              0x76, 13, //0000001110110 1152
              0x77, 13, //0000001110111 1216
              0x52, 13, //0000001010010 1280
              0x53, 13, //0000001010011 1344
              0x54, 13, //0000001010100 1408
              0x55, 13, //0000001010101 1472
              0x5a, 13, //0000001011010 1536
              0x5b, 13, //0000001011011 1600
              0x64, 13, //0000001100100 1664
              0x65, 13, //0000001100101 1728
              // Common codes for white and black:
              0x08, 11, //00000001000 1792
              0x0c, 11, //00000001100 1856
              0x0d, 11, //00000001101 1920
              0x12, 12, //000000010010 1984
              0x13, 12, //000000010011 2048
              0x14, 12, //000000010100 2112  // 32
              0x15, 12, //000000010101 2176
              0x16, 12, //000000010110 2240
              0x17, 12, //000000010111 2304
              0x1c, 12, //000000011100 2368
              0x1d, 12, //000000011101 2432
              0x1e, 12, //000000011110 2496
              0x1f, 12, //000000011111 2560
              0x01, 12, //000000000001 EOL   // 40
        };

        internal readonly static uint[] HorizontalCodes = { 0x1, 3 }; /* 001 */
        internal readonly static uint[] PassCodes = { 0x1, 4, }; /* 0001 */
        internal readonly static uint[] VerticalCodes =
        {
              0x03, 7, /* 0000 011 */
              0x03, 6, /* 0000 11 */
              0x03, 3, /* 011 */
              0x1,  1, /* 1 */
              0x2,  3, /* 010 */
              0x02, 6, /* 0000 10 */
              0x02, 7, /* 0000 010 */
        };

        readonly static uint[] _zeroRuns =
        {
              8, 7, 6, 6, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4, 4, 4,	/* 0x00 - 0x0f */
              3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,	/* 0x10 - 0x1f */
              2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,	/* 0x20 - 0x2f */
              2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,	/* 0x30 - 0x3f */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0x40 - 0x4f */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0x50 - 0x5f */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0x60 - 0x6f */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0x70 - 0x7f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x80 - 0x8f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x90 - 0x9f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0xa0 - 0xaf */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0xb0 - 0xbf */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0xc0 - 0xcf */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0xd0 - 0xdf */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0xe0 - 0xef */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0xf0 - 0xff */
        };

        readonly static uint[] _oneRuns =
        {
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x00 - 0x0f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x10 - 0x1f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x20 - 0x2f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x30 - 0x3f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x40 - 0x4f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x50 - 0x5f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x60 - 0x6f */
              0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,	/* 0x70 - 0x7f */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0x80 - 0x8f */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0x90 - 0x9f */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0xa0 - 0xaf */
              1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,	/* 0xb0 - 0xbf */
              2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,	/* 0xc0 - 0xcf */
              2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,	/* 0xd0 - 0xdf */
              3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,	/* 0xe0 - 0xef */
              4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 7, 8,	/* 0xf0 - 0xff */
        };

        /// <summary>
        /// Counts the consecutive one bits in an image line.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="bitsLeft">The bits left.</param>
        private static uint CountOneBits(BitReader reader, uint bitsLeft)
        {
            uint found = 0;
            for (;;)
            {
                uint bits;
                int @byte = reader.PeekByte(out bits);
                uint hits = _oneRuns[@byte];
                if (hits < bits)
                {
                    if (hits > 0)
                        reader.SkipBits(hits);
                    found += hits;
                    return found >= bitsLeft ? bitsLeft : found;
                }
                found += bits;
                if (found >= bitsLeft)
                    return bitsLeft;
                reader.NextByte();
            }
        }

        /// <summary>
        /// Counts the consecutive zero bits in an image line.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="bitsLeft">The bits left.</param>
        private static uint CountZeroBits(BitReader reader, uint bitsLeft)
        {
            uint found = 0;
            for (;;)
            {
                uint bits;
                int @byte = reader.PeekByte(out bits);
                uint hits = _zeroRuns[@byte];
                if (hits < bits)
                {
                    if (hits > 0)
                        reader.SkipBits(hits);
                    found += hits;
                    return found >= bitsLeft ? bitsLeft : found;
                }
                found += bits;
                if (found >= bitsLeft)
                    return bitsLeft;
                reader.NextByte();
            }
        }

        /// <summary>
        /// Returns the offset of the next bit in the range
        /// [bitStart..bitEnd] that is different from the
        /// specified color.  The end, bitEnd, is returned
        /// if no such bit exists.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="bitStart">The offset of the start bit.</param>
        /// <param name="bitEnd">The offset of the end bit.</param>
        /// <param name="searchOne">If set to <c>true</c> searches "one" (i. e. white), otherwise searches black.</param>
        /// <returns>The offset of the first non-matching bit.</returns>
        private static uint FindDifference(BitReader reader, uint bitStart, uint bitEnd, bool searchOne)
        {
            // Translated from LibTiff
            reader.SetPosition(bitStart);
            return (bitStart + (searchOne ? CountOneBits(reader, bitEnd - bitStart) : CountZeroBits(reader, bitEnd - bitStart)));
        }

        /// <summary>
        /// Returns the offset of the next bit in the range
        /// [bitStart..bitEnd] that is different from the
        /// specified color.  The end, bitEnd, is returned
        /// if no such bit exists.
        /// Like FindDifference, but also check the
        /// starting bit against the end in case start > end.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="bitStart">The offset of the start bit.</param>
        /// <param name="bitEnd">The offset of the end bit.</param>
        /// <param name="searchOne">If set to <c>true</c> searches "one" (i. e. white), otherwise searches black.</param>
        /// <returns>The offset of the first non-matching bit.</returns>
        private static uint FindDifferenceWithCheck(BitReader reader, uint bitStart, uint bitEnd, bool searchOne)
        {
            // Translated from LibTiff
            return ((bitStart < bitEnd) ? FindDifference(reader, bitStart, bitEnd, searchOne) : bitEnd);
        }

        /// <summary>
        /// 2d-encode a row of pixels. Consult the CCITT documentation for the algorithm.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="bytesFileOffset">Offset of image data in bitmap file.</param>
        /// <param name="imageBits">The bitmap file.</param>
        /// <param name="currentRow">Index of the current row.</param>
        /// <param name="referenceRow">Index of the reference row (0xffffffff if there is none).</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="bytesPerLineBmp">The bytes per line in the bitmap file.</param>
        static void FaxEncode2DRow(BitWriter writer, uint bytesFileOffset, byte[] imageBits, uint currentRow, uint referenceRow, uint width, uint height, uint bytesPerLineBmp)
        {
            // Translated from LibTiff
            uint bytesOffsetRead = bytesFileOffset + (height - 1 - currentRow) * bytesPerLineBmp;
            BitReader reader = new BitReader(imageBits, bytesOffsetRead, width);
            BitReader readerReference;
            if (referenceRow != 0xffffffff)
            {
                uint bytesOffsetReadReference = bytesFileOffset + (height - 1 - referenceRow) * bytesPerLineBmp;
                readerReference = new BitReader(imageBits, bytesOffsetReadReference, width);
            }
            else
            {
                byte[] tmpImageBits = new byte[bytesPerLineBmp];
                for (int i = 0; i < bytesPerLineBmp; ++i)
                    tmpImageBits[i] = 255;
                readerReference = new BitReader(tmpImageBits, 0, width);
            }

            uint a0 = 0;
            uint a1 = !reader.GetBit(0) ? 0 : FindDifference(reader, 0, width, true);
            uint b1 = !readerReference.GetBit(0) ? 0 : FindDifference(readerReference, 0, width, true);
            // ReSharper disable TooWideLocalVariableScope
            uint a2, b2;
            // ReSharper restore TooWideLocalVariableScope

            for (;;)
            {
                b2 = FindDifferenceWithCheck(readerReference, b1, width, readerReference.GetBit(b1));
                if (b2 >= a1)
                {
                    int d = (int)b1 - (int)a1;
                    if (!(-3 <= d && d <= 3))
                    {
                        /* horizontal mode */
                        a2 = FindDifferenceWithCheck(reader, a1, width, reader.GetBit(a1));
                        writer.WriteTableLine(HorizontalCodes, 0);

                        if (a0 + a1 == 0 || reader.GetBit(a0))
                        {
                            WriteSample(writer, a1 - a0, true);
                            WriteSample(writer, a2 - a1, false);
                        }
                        else
                        {
                            WriteSample(writer, a1 - a0, false);
                            WriteSample(writer, a2 - a1, true);
                        }
                        a0 = a2;
                    }
                    else
                    {
                        /* vertical mode */
                        writer.WriteTableLine(VerticalCodes, (uint)(d + 3));
                        a0 = a1;
                    }
                }
                else
                {
                    /* pass mode */
                    writer.WriteTableLine(PassCodes, 0);
                    a0 = b2;
                }
                if (a0 >= width)
                    break;
                bool bitA0 = reader.GetBit(a0);
                a1 = FindDifference(reader, a0, width, bitA0/*reader.GetBit(a0)*/);
                b1 = FindDifference(readerReference, a0, width, !bitA0/*reader.GetBit(a0)*/);
                b1 = FindDifferenceWithCheck(readerReference, b1, width, bitA0/*reader.GetBit(a0)*/);
            }
        }

        /// <summary>
        /// Encodes a bitonal bitmap using 1D CCITT fax encoding.
        /// </summary>
        /// <param name="imageData">Space reserved for the fax encoded bitmap. An exception will be thrown if this buffer is too small.</param>
        /// <param name="imageBits">The bitmap to be encoded.</param>
        /// <param name="bytesFileOffset">Offset of image data in bitmap file.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>The size of the fax encoded image (0 on failure).</returns>
        private static int DoFaxEncoding(ref byte[] imageData, byte[] imageBits, uint bytesFileOffset, uint width, uint height)
        {
            try
            {
                uint bytesPerLineBmp = ((width + 31) / 32) * 4;
                BitWriter writer = new BitWriter(ref imageData);
                for (uint y = 0; y < height; ++y)
                {
                    uint bytesOffsetRead = bytesFileOffset + (height - 1 - y) * bytesPerLineBmp;
                    BitReader reader = new BitReader(imageBits, bytesOffsetRead, width);
                    for (uint bitsRead = 0; bitsRead < width;)
                    {
                        uint white = CountOneBits(reader, width - bitsRead);
                        WriteSample(writer, white, true);
                        bitsRead += white;
                        if (bitsRead < width)
                        {
                            uint black = CountZeroBits(reader, width - bitsRead);
                            WriteSample(writer, black, false);
                            bitsRead += black;
                        }
                    }
                }
                writer.FlushBuffer();
                return writer.BytesWritten();
            }
            catch (Exception /*ex*/)
            {
                //ex.GetType();
                return 0;
            }
        }

        /// <summary>
        /// Encodes a bitonal bitmap using 2D group 4 CCITT fax encoding.
        /// </summary>
        /// <param name="imageData">Space reserved for the fax encoded bitmap. An exception will be thrown if this buffer is too small.</param>
        /// <param name="imageBits">The bitmap to be encoded.</param>
        /// <param name="bytesFileOffset">Offset of image data in bitmap file.</param>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <returns>The size of the fax encoded image (0 on failure).</returns>
        internal static int DoFaxEncodingGroup4(ref byte[] imageData, byte[] imageBits, uint bytesFileOffset, uint width, uint height)
        {
            try
            {
                uint bytesPerLineBmp = ((width + 31) / 32) * 4;
                BitWriter writer = new BitWriter(ref imageData);
                for (uint y = 0; y < height; ++y)
                {
                    FaxEncode2DRow(writer, bytesFileOffset, imageBits, y, (y != 0) ? y - 1 : 0xffffffff, width, height, bytesPerLineBmp);
                }
                writer.FlushBuffer();
                return writer.BytesWritten();
            }
            catch (Exception ex)
            {
                ex.GetType();
                return 0;
            }
        }

        /// <summary>
        /// Writes the image data.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="count">The count of bits (pels) to encode.</param>
        /// <param name="white">The color of the pels.</param>
        private static void WriteSample(BitWriter writer, uint count, bool white)
        {
            uint[] terminatingCodes = white ? WhiteTerminatingCodes : BlackTerminatingCodes;
            uint[] makeUpCodes = white ? WhiteMakeUpCodes : BlackMakeUpCodes;

            // The make-up code for 2560 will be written as often as required:
            while (count >= 2624)
            {
                writer.WriteTableLine(makeUpCodes, 39); // Magic: 2560
                count -= 2560;
            }
            // A make-up code for a multiple of 64 will be written if required:
            if (count > 63)
            {
                uint line = count / 64 - 1;
                writer.WriteTableLine(makeUpCodes, line);
                count -= (line + 1) * 64;
            }
            // And finally the terminating code for the remaining value (0 through 63):
            writer.WriteTableLine(terminatingCodes, count);
        }
    }

    /// <summary>
    /// The BitReader class is a helper to read bits from an in-memory bitmap file.
    /// </summary>
    class BitReader
    {
        readonly byte[] _imageBits;
        uint _bytesOffsetRead;
        readonly uint _bytesFileOffset;
        byte _buffer;
        uint _bitsInBuffer;
        readonly uint _bitsTotal; // Bits we may read (bits per image line)

        /// <summary>
        /// Initializes a new instance of the <see cref="BitReader"/> class.
        /// </summary>
        /// <param name="imageBits">The in-memory bitmap file.</param>
        /// <param name="bytesFileOffset">The offset of the line to read.</param>
        /// <param name="bits">The count of bits that may be read (i. e. the width of the image for normal usage).</param>
        internal BitReader(byte[] imageBits, uint bytesFileOffset, uint bits)
        {
            _imageBits = imageBits;
            _bytesFileOffset = bytesFileOffset;
            _bitsTotal = bits;
            _bytesOffsetRead = bytesFileOffset;
            _buffer = imageBits[_bytesOffsetRead];
            _bitsInBuffer = 8;
        }

        /// <summary>
        /// Sets the position within the line (needed for 2D encoding).
        /// </summary>
        /// <param name="position">The new position.</param>
        internal void SetPosition(uint position)
        {
            _bytesOffsetRead = _bytesFileOffset + (position >> 3);
            _buffer = _imageBits[_bytesOffsetRead];
            _bitsInBuffer = 8 - (position & 0x07);
        }

        /// <summary>
        /// Gets a single bit at the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>True if bit is set.</returns>
        internal bool GetBit(uint position)
        {
            if (position >= _bitsTotal)
                return false;
            SetPosition(position);
            uint dummy;
            return (PeekByte(out dummy) & 0x80) > 0;
        }

        /// <summary>
        /// Returns the bits that are in the buffer (without changing the position).
        /// Data is MSB aligned.
        /// </summary>
        /// <param name="bits">The count of bits that were returned (1 through 8).</param>
        /// <returns>The MSB aligned bits from the buffer.</returns>
        internal byte PeekByte(out uint bits)
        {
            // TODO: try to make this faster!
            if (_bitsInBuffer == 8)
            {
                bits = 8;
                return _buffer;
            }
            bits = _bitsInBuffer;
            return (byte)(_buffer << (int)(8 - _bitsInBuffer));
        }

        /// <summary>
        /// Moves the buffer to the next byte.
        /// </summary>
        internal void NextByte()
        {
            _buffer = _imageBits[++_bytesOffsetRead];
            _bitsInBuffer = 8;
        }

        /// <summary>
        /// "Removes" (eats) bits from the buffer.
        /// </summary>
        /// <param name="bits">The count of bits that were processed.</param>
        internal void SkipBits(uint bits)
        {
            Debug.Assert(bits <= _bitsInBuffer, "Buffer underrun");
            if (bits == _bitsInBuffer)
            {
                NextByte();
                return;
            }
            _bitsInBuffer -= bits;
        }
    }

    /// <summary>
    /// A helper class for writing groups of bits into an array of bytes.
    /// </summary>
    class BitWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitWriter"/> class.
        /// </summary>
        /// <param name="imageData">The byte array to be written to.</param>
        internal BitWriter(ref byte[] imageData)
        {
            _imageData = imageData;
        }

        /// <summary>
        /// Writes the buffered bits into the byte array.
        /// </summary>
        internal void FlushBuffer()
        {
            if (_bitsInBuffer > 0)
            {
                uint bits = 8 - _bitsInBuffer;
                WriteBits(0, bits);
            }
        }

        /// <summary>
        /// Masks for n bits in a byte (with n = 0 through 8).
        /// </summary>
        static readonly uint[] masks = { 0, 1, 3, 7, 15, 31, 63, 127, 255 };

        /// <summary>
        /// Writes bits to the byte array.
        /// </summary>
        /// <param name="value">The bits to be written (LSB aligned).</param>
        /// <param name="bits">The count of bits.</param>
        internal void WriteBits(uint value, uint bits)
        {
#if true
            // TODO: Try to make this faster!

            // If we have to write more bits than fit into the buffer, we fill
            // the buffer and call the same routine recursively for the rest.
#if USE_GOTO
            // Use GOTO instead of end recursion: (is this faster?)
            SimulateRecursion:
#endif
            if (bits + _bitsInBuffer > 8)
            {
                // We can't add all bits this time.
                uint bitsNow = 8 - _bitsInBuffer;
                uint bitsRemainder = bits - bitsNow;
                WriteBits(value >> (int)(bitsRemainder), bitsNow); // that fits
#if USE_GOTO
                bits = bitsRemainder;
                goto SimulateRecursion;
#else
        WriteBits(value, bitsRemainder);
        return;
#endif
            }

            _buffer = (_buffer << (int)bits) + (value & masks[bits]);
            _bitsInBuffer += bits;

            if (_bitsInBuffer == 8)
            {
                // The line below will sometimes throw a System.IndexOutOfRangeException while PDFsharp tries different formats for monochrome bitmaps (exception occurs if CCITT encoding requires more space than an uncompressed bitmap).
                _imageData[_bytesOffsetWrite] = (byte)_buffer;
                _bitsInBuffer = 0;
                ++_bytesOffsetWrite;
            }
#else
            // Simple implementation writing bit by bit:
            int mask = 1 << (int)(bits - 1);
            for (int b = 0; b < bits; ++b)
            {
                if ((value & mask) != 0)
                    buffer = (buffer << 1) + 1;
                else
                    buffer = buffer << 1;
                ++bitsInBuffer;
                mask /= 2;
                if (bitsInBuffer == 8)
                {
                    imageData[bytesOffsetWrite] = (byte)buffer;
                    bitsInBuffer = 0;
                    ++bytesOffsetWrite;
                }
            }
#endif
        }

        /// <summary>
        /// Writes a line from a look-up table.
        /// A "line" in the table are two integers, one containing the values, one containing the bit count.
        /// </summary>
        internal void WriteTableLine(uint[] table, uint line)
        {
            uint value = table[line * 2];
            uint bits = table[line * 2 + 1];
            WriteBits(value, bits);
        }

        [Obsolete]
        internal void WriteEOL()
        {
            // Not needed for PDF.
            WriteTableLine(PdfImage.WhiteMakeUpCodes, 40);
        }

        /// <summary>
        /// Flushes the buffer and returns the count of bytes written to the array.
        /// </summary>
        internal int BytesWritten()
        {
            FlushBuffer();
            return _bytesOffsetWrite;
        }

        int _bytesOffsetWrite;
        readonly byte[] _imageData;
        uint _buffer;
        uint _bitsInBuffer;
    }
}
