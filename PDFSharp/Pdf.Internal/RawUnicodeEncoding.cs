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

using System.Text;

namespace PdfSharp.Pdf.Internal
{
    /// <summary>
    /// An encoder for Unicode strings. 
    /// (That means, a character represents a glyph index.)
    /// </summary>
    internal sealed class RawUnicodeEncoding : Encoding
    {
        public RawUnicodeEncoding()
        { }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            // Each character represents exactly an ushort value, which is a glyph index.
            return 2 * count;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int count = charCount; count > 0; charIndex++, count--)
            {
                char ch = chars[charIndex];
                bytes[byteIndex++] = (byte)(ch >> 8);
                bytes[byteIndex++] = (byte)ch;
            }
            return charCount * 2;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count / 2;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int count = byteCount; count > 0; byteIndex += 2, charIndex++, count--)
            {
                chars[charIndex] = (char)((int)bytes[byteIndex] << 8 + (int)bytes[byteIndex + 1]);
            }
            return byteCount;
        }

        public override int GetMaxByteCount(int charCount)
        {
            return charCount * 2;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount / 2;
        }
    }
}
