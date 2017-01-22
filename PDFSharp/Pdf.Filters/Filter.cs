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

using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf.Filters
{
    /// <summary>
    /// Reserved for future extension.
    /// </summary>
    public class FilterParms
    {
        // not yet used
    }

    /// <summary>
    /// Base class for all stream filters
    /// </summary>
    public abstract class Filter
    {
        /// <summary>
        /// When implemented in a derived class encodes the specified data.
        /// </summary>
        public abstract byte[] Encode(byte[] data);

        /// <summary>
        /// Encodes a raw string.
        /// </summary>
        public virtual byte[] Encode(string rawString)
        {
            byte[] bytes = PdfEncoders.RawEncoding.GetBytes(rawString);
            bytes = Encode(bytes);
            return bytes;
        }

        /// <summary>
        /// When implemented in a derived class decodes the specified data.
        /// </summary>
        public abstract byte[] Decode(byte[] data, FilterParms parms);

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        public byte[] Decode(byte[] data)
        {
            return Decode(data, null);
        }

        /// <summary>
        /// Decodes to a raw string.
        /// </summary>
        public virtual string DecodeToString(byte[] data, FilterParms parms)
        {
            byte[] bytes = Decode(data, parms);
            string text = PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
            return text;
        }

        /// <summary>
        /// Decodes to a raw string.
        /// </summary>
        public string DecodeToString(byte[] data)
        {
            return DecodeToString(data, null);
        }

        /// <summary>
        /// Removes all white spaces from the data. The function assumes that the bytes are characters.
        /// </summary>
        protected byte[] RemoveWhiteSpace(byte[] data)
        {
            int count = data.Length;
            int j = 0;
            for (int i = 0; i < count; i++, j++)
            {
                switch (data[i])
                {
                    case (byte)Chars.NUL:  // 0 Null
                    case (byte)Chars.HT:   // 9 Tab
                    case (byte)Chars.LF:   // 10 Line feed
                    case (byte)Chars.FF:   // 12 Form feed
                    case (byte)Chars.CR:   // 13 Carriage return
                    case (byte)Chars.SP:   // 32 Space
                        j--;
                        break;

                    default:
                        if (i != j)
                            data[j] = data[i];
                        break;
                }
            }
            if (j < count)
            {
                byte[] temp = data;
                data = new byte[j];
                for (int idx = 0; idx < j; idx++)
                    data[idx] = temp[idx];
            }
            return data;
        }
    }
}
