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

//using Fixed = System.Int32;
//using FWord = System.Int16;
//using UFWord = System.UInt16;

namespace PdfSharp.Fonts.OpenType
{
#if true_
    /// <summary>
    /// Generic font table. Not yet used
    /// </summary>
    internal class GenericFontTable : OpenTypeFontTable
    {
        public GenericFontTable(OpenTypeFontTable fontTable)
          : base(null, "xxxx")
        {
            DirectoryEntry.Tag = fontTable.DirectoryEntry.Tag;
            int length = fontTable.DirectoryEntry.Length;
            if (length > 0)
            {
                _table = new byte[length];
                Buffer.BlockCopy(fontTable.FontData.Data, fontTable.DirectoryEntry.Offset, _table, 0, length);
            }
        }

        public GenericFontTable(OpenTypeFontface fontData, string tag)
          : base(fontData, tag)
        {
            _fontData = fontData;
        }

        protected override OpenTypeFontTable DeepCopy()
        {
            GenericFontTable fontTable = (GenericFontTable)base.DeepCopy();
            fontTable._table = (byte[])_table.Clone();
            return fontTable;
        }

        byte[] _table;
    }
#endif
}
