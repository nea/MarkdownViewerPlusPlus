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
using System.Diagnostics;
using System.Collections.Generic;
using PdfSharp.Fonts.OpenType;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Fonts
{
    /// <summary>
    /// Helper class that determines the characters used in a particular font.
    /// </summary>
    internal class CMapInfo
    {
        public CMapInfo(OpenTypeDescriptor descriptor)
        {
            Debug.Assert(descriptor != null);
            _descriptor = descriptor;
        }
        internal OpenTypeDescriptor _descriptor;

        /// <summary>
        /// Adds the characters of the specified string to the hashtable.
        /// </summary>
        public void AddChars(string text)
        {
            if (text != null)
            {
                bool symbol = _descriptor.FontFace.cmap.symbol;
                int length = text.Length;
                for (int idx = 0; idx < length; idx++)
                {
                    char ch = text[idx];
                    if (!CharacterToGlyphIndex.ContainsKey(ch))
                    {
                        char ch2 = ch;
                        if (symbol)
                        {
                            // Remap ch for symbol fonts.
                            ch2 = (char)(ch | (_descriptor.FontFace.os2.usFirstCharIndex & 0xFF00));  // @@@ refactor
                        }
                        int glyphIndex = _descriptor.CharCodeToGlyphIndex(ch2);
                        CharacterToGlyphIndex.Add(ch, glyphIndex);
                        GlyphIndices[glyphIndex] = null;
                        MinChar = (char)Math.Min(MinChar, ch);
                        MaxChar = (char)Math.Max(MaxChar, ch);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the glyphIndices to the hashtable.
        /// </summary>
        public void AddGlyphIndices(string glyphIndices)
        {
            if (glyphIndices != null)
            {
                int length = glyphIndices.Length;
                for (int idx = 0; idx < length; idx++)
                {
                    int glyphIndex = glyphIndices[idx];
                    GlyphIndices[glyphIndex] = null;
                }
            }
        }

        /// <summary>
        /// Adds a ANSI characters.
        /// </summary>
        internal void AddAnsiChars()
        {
            byte[] ansi = new byte[256 - 32];
            for (int idx = 0; idx < 256 - 32; idx++)
                ansi[idx] = (byte)(idx + 32);
#if EDF_CORE
            string text = null; // PdfEncoders.WinAnsiEncoding.GetString(ansi, 0, ansi.Length);
#else
            string text = PdfEncoders.WinAnsiEncoding.GetString(ansi, 0, ansi.Length);
#endif
            AddChars(text);
        }

        internal bool Contains(char ch)
        {
            return CharacterToGlyphIndex.ContainsKey(ch);
        }

        public char[] Chars
        {
            get
            {
                char[] chars = new char[CharacterToGlyphIndex.Count];
                CharacterToGlyphIndex.Keys.CopyTo(chars, 0);
                Array.Sort(chars);
                return chars;
            }
        }

        public int[] GetGlyphIndices()
        {
            int[] indices = new int[GlyphIndices.Count];
            GlyphIndices.Keys.CopyTo(indices, 0);
            Array.Sort(indices);
            return indices;
        }

        public char MinChar = char.MaxValue;
        public char MaxChar = char.MinValue;
        public Dictionary<char, int> CharacterToGlyphIndex = new Dictionary<char, int>();
        public Dictionary<int, object> GlyphIndices = new Dictionary<int, object>();
    }
}
