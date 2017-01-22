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

namespace PdfSharp.Pdf.Filters
{
    /// <summary>
    /// Implements the ASCII85Decode filter.
    /// </summary>
    public class Ascii85Decode : Filter
    {
        // Reference: 3.3.2  ASCII85Decode Filter / Page 69

        /// <summary>
        /// Encodes the specified data.
        /// </summary>
        public override byte[] Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int length = data.Length;  // length == 0 is must not be treated as a special case.
            int words = length / 4;
            int rest = length - (words * 4);
            byte[] result = new byte[words * 5 + (rest == 0 ? 0 : rest + 1) + 2];

            int idxIn = 0, idxOut = 0;
            int wCount = 0;
            while (wCount < words)
            {
                uint val = ((uint)data[idxIn++] << 24) + ((uint)data[idxIn++] << 16) + ((uint)data[idxIn++] << 8) + data[idxIn++];
                if (val == 0)
                {
                    result[idxOut++] = (byte)'z';
                }
                else
                {
                    byte c5 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c4 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c3 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c2 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c1 = (byte)(val + '!');

                    result[idxOut++] = c1;
                    result[idxOut++] = c2;
                    result[idxOut++] = c3;
                    result[idxOut++] = c4;
                    result[idxOut++] = c5;
                }
                wCount++;
            }
            if (rest == 1)
            {
                uint val = (uint)data[idxIn] << 24;
                val /= 85 * 85 * 85;
                byte c2 = (byte)(val % 85 + '!');
                val /= 85;
                byte c1 = (byte)(val + '!');

                result[idxOut++] = c1;
                result[idxOut++] = c2;
            }
            else if (rest == 2)
            {
                uint val = ((uint)data[idxIn++] << 24) + ((uint)data[idxIn] << 16);
                val /= 85 * 85;
                byte c3 = (byte)(val % 85 + '!');
                val /= 85;
                byte c2 = (byte)(val % 85 + '!');
                val /= 85;
                byte c1 = (byte)(val + '!');

                result[idxOut++] = c1;
                result[idxOut++] = c2;
                result[idxOut++] = c3;
            }
            else if (rest == 3)
            {
                uint val = ((uint)data[idxIn++] << 24) + ((uint)data[idxIn++] << 16) + ((uint)data[idxIn] << 8);
                val /= 85;
                byte c4 = (byte)(val % 85 + '!');
                val /= 85;
                byte c3 = (byte)(val % 85 + '!');
                val /= 85;
                byte c2 = (byte)(val % 85 + '!');
                val /= 85;
                byte c1 = (byte)(val + '!');

                result[idxOut++] = c1;
                result[idxOut++] = c2;
                result[idxOut++] = c3;
                result[idxOut++] = c4;
            }
            result[idxOut++] = (byte)'~';
            result[idxOut++] = (byte)'>';

            if (idxOut < result.Length)
                Array.Resize(ref result, idxOut);

            return result;
        }

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        public override byte[] Decode(byte[] data, FilterParms parms)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int idx;
            int length = data.Length;
            int zCount = 0;
            int idxOut = 0;
            for (idx = 0; idx < length; idx++)
            {
                char ch = (char)data[idx];
                if (ch >= '!' && ch <= 'u')
                    data[idxOut++] = (byte)ch;
                else if (ch == 'z')
                {
                    data[idxOut++] = (byte)ch;
                    zCount++;
                }
                else if (ch == '~')
                {
                    if ((char)data[idx + 1] != '>')
                        throw new ArgumentException("Illegal character.", "data");
                    break;
                }
                // ingnore unknown character
            }
            // Loop not ended with break?
            if (idx == length)
                throw new ArgumentException("Illegal character.", "data");

            length = idxOut;
            int nonZero = length - zCount;
            int byteCount = 4 * (zCount + (nonZero / 5)); // full 4 byte blocks

            int remainder = nonZero % 5;
            if (remainder == 1)
                throw new InvalidOperationException("Illegal character.");

            if (remainder != 0)
                byteCount += remainder - 1;

            byte[] output = new byte[byteCount];

            idxOut = 0;
            idx = 0;
            while (idx + 4 < length)
            {
                char ch = (char)data[idx];
                if (ch == 'z')
                {
                    idx++;
                    idxOut += 4;
                }
                else
                {
                    // TODO: check 
                    long value =
                      (long)(data[idx++] - '!') * (85 * 85 * 85 * 85) +
                      (uint)(data[idx++] - '!') * (85 * 85 * 85) +
                      (uint)(data[idx++] - '!') * (85 * 85) +
                      (uint)(data[idx++] - '!') * 85 +
                      (uint)(data[idx++] - '!');

                    if (value > UInt32.MaxValue)
                        throw new InvalidOperationException("Value of group greater than 2 power 32 - 1.");

                    output[idxOut++] = (byte)(value >> 24);
                    output[idxOut++] = (byte)(value >> 16);
                    output[idxOut++] = (byte)(value >> 8);
                    output[idxOut++] = (byte)value;
                }
            }

            // I have found no appropriate algorithm, so I write my own. In some rare cases the value must not
            // increased by one, but I cannot found a general formula or a proof.
            // All possible cases are tested programmatically.
            if (remainder == 2) // one byte
            {
                uint value =
                  (uint)(data[idx++] - '!') * (85 * 85 * 85 * 85) +
                  (uint)(data[idx] - '!') * (85 * 85 * 85);

                // Always increase if not zero (tried out).
                if (value != 0)
                    value += 0x01000000;

                output[idxOut] = (byte)(value >> 24);
            }
            else if (remainder == 3) // two bytes
            {
                int idxIn = idx;
                uint value =
                  (uint)(data[idx++] - '!') * (85 * 85 * 85 * 85) +
                  (uint)(data[idx++] - '!') * (85 * 85 * 85) +
                  (uint)(data[idx] - '!') * (85 * 85);

                if (value != 0)
                {
                    value &= 0xFFFF0000;
                    uint val = value / (85 * 85);
                    byte c3 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c2 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c1 = (byte)(val + '!');
                    if (c1 != data[idxIn] || c2 != data[idxIn + 1] || c3 != data[idxIn + 2])
                    {
                        value += 0x00010000;
                        //Count2++;
                    }
                }
                output[idxOut++] = (byte)(value >> 24);
                output[idxOut] = (byte)(value >> 16);
            }
            else if (remainder == 4) // three bytes
            {
                int idxIn = idx;
                uint value =
                  (uint)(data[idx++] - '!') * (85 * 85 * 85 * 85) +
                  (uint)(data[idx++] - '!') * (85 * 85 * 85) +
                  (uint)(data[idx++] - '!') * (85 * 85) +
                  (uint)(data[idx] - '!') * 85;

                if (value != 0)
                {
                    value &= 0xFFFFFF00;
                    uint val = value / 85;
                    byte c4 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c3 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c2 = (byte)(val % 85 + '!');
                    val /= 85;
                    byte c1 = (byte)(val + '!');
                    if (c1 != data[idxIn] || c2 != data[idxIn + 1] || c3 != data[idxIn + 2] || c4 != data[idxIn + 3])
                    {
                        value += 0x00000100;
                        //Count3++;
                    }
                }
                output[idxOut++] = (byte)(value >> 24);
                output[idxOut++] = (byte)(value >> 16);
                output[idxOut] = (byte)(value >> 8);
            }
            return output;
        }
    }
}
