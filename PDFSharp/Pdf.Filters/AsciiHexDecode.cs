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
    /// Implements the ASCIIHexDecode filter.
    /// </summary>
    public class AsciiHexDecode : Filter
    {
        // Reference: 3.3.1  ASCIIHexDecode Filter / Page 69

        /// <summary>
        /// Encodes the specified data.
        /// </summary>
        public override byte[] Encode(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            int count = data.Length;
            byte[] bytes = new byte[2 * count];
            for (int i = 0, j = 0; i < count; i++)
            {
                byte b = data[i];
                bytes[j++] = (byte)((b >> 4) + ((b >> 4) < 10 ? (byte)'0' : (byte)('A' - 10)));
                bytes[j++] = (byte)((b & 0xF) + ((b & 0xF) < 10 ? (byte)'0' : (byte)('A' - 10)));
            }
            return bytes;
        }

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        public override byte[] Decode(byte[] data, FilterParms parms)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            data = RemoveWhiteSpace(data);
            int count = data.Length;
            // Ignore EOD (end of data) character.
            // EOD can be anywhere in the stream, but makes sense only at the end of the stream.
            if (count > 0 && data[count - 1] == '>')
                --count;
            if (count % 2 == 1)
            {
                count++;
                byte[] temp = data;
                data = new byte[count];
                temp.CopyTo(data, 0);
            }
            count >>= 1;
            byte[] bytes = new byte[count];
            for (int i = 0, j = 0; i < count; i++)
            {
                // Must support 0-9, A-F, a-f - "Any other characters cause an error."
                byte hi = data[j++];
                byte lo = data[j++];
                if (hi >= 'a' && hi <= 'f')
                    hi -= 32;
                if (lo >= 'a' && lo <= 'f')
                    lo -= 32;
                // TODO Throw on invalid characters. Stop when encountering EOD. Add one more byte if EOD is the lo byte.
                bytes[i] = (byte)((hi > '9' ? hi - '7'/*'A' + 10*/: hi - '0') * 16 + (lo > '9' ? lo - '7'/*'A' + 10*/: lo - '0'));
            }
            return bytes;
        }
    }
}
