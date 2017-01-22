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

//using Fixed = System.Int32;
//using FWord = System.Int16;
//using UFWord = System.UInt16;
#if SILVERLIGHT
using PdfSharp;
#endif

namespace PdfSharp.Fonts.OpenType
{
    // TODO: Create a font driver for reading and writing OpenType font files.

    /// <summary>
    /// Base class for all OpenType tables used in PDFsharp.
    /// </summary>
    internal class OpenTypeFontTable : ICloneable
    {
        public OpenTypeFontTable(OpenTypeFontface fontData, string tag)
        {
            _fontData = fontData;
            if (fontData != null && fontData.TableDictionary.ContainsKey(tag))
                DirectoryEntry = fontData.TableDictionary[tag];
            else
                DirectoryEntry = new TableDirectoryEntry(tag);
            DirectoryEntry.FontTable = this;
        }

        /// <summary>
        /// Creates a deep copy of the current instance.
        /// </summary>
        public object Clone()
        {
            return DeepCopy();
        }

        protected virtual OpenTypeFontTable DeepCopy()
        {
            OpenTypeFontTable fontTable = (OpenTypeFontTable)MemberwiseClone();
            fontTable.DirectoryEntry.Offset = 0;
            fontTable.DirectoryEntry.FontTable = fontTable;
            return fontTable;
        }

        /// <summary>
        /// Gets the font image the table belongs to.
        /// </summary>
        public OpenTypeFontface FontData
        {
            get { return _fontData; }
        }
        internal OpenTypeFontface _fontData;

        public TableDirectoryEntry DirectoryEntry;

        /// <summary>
        /// When overridden in a derived class, prepares the font table to be compiled into its binary representation.
        /// </summary>
        public virtual void PrepareForCompilation()
        { }

        /// <summary>
        /// When overridden in a derived class, converts the font into its binary representation.
        /// </summary>
        public virtual void Write(OpenTypeFontWriter writer)
        { }

        /// <summary>
        /// Calculates the checksum of a table represented by its bytes.
        /// </summary>
        public static uint CalcChecksum(byte[] bytes)
        {
            Debug.Assert((bytes.Length & 3) == 0);
            // Cannot use Buffer.BlockCopy because 32-bit values are Big-endian in fonts.
            uint byte3, byte2, byte1, byte0;
            byte3 = byte2 = byte1 = byte0 = 0;
            int length = bytes.Length;
            for (int idx = 0; idx < length;)
            {
                byte3 += bytes[idx++];
                byte2 += bytes[idx++];
                byte1 += bytes[idx++];
                byte0 += bytes[idx++];
            }
            return (byte3 << 24) + (byte2 << 16) + (byte1 << 8) + byte0;
        }
    }
}
