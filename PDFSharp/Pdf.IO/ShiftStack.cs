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
using System.Collections.Generic;
using System.Diagnostics;

namespace PdfSharp.Pdf.IO
{
    /// <summary>
    /// Represents the stack for the shift-reduce parser. It seems that it is only needed for
    /// reduction of indirect references.
    /// </summary>
    internal class ShiftStack
    {
        // TODO: make Lexer.PeekChars(20) and scan for 'R' to detect indirect references

        public ShiftStack()
        {
            _items = new List<PdfItem>();
        }

        public PdfItem[] ToArray(int start, int length)
        {
            PdfItem[] items = new PdfItem[length];
            for (int i = 0, j = start; i < length; i++, j++)
                items[i] = _items[j];
            return items;
        }

        /// <summary>
        /// Gets the stack pointer index.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public int SP
        // ReSharper restore InconsistentNaming
        {
            get { return _sp; }
        }

        /// <summary>
        /// Gets the value at the specified index. Valid index is in range 0 up to sp-1.
        /// </summary>
        public PdfItem this[int index]
        {
            get
            {
                if (index >= _sp)
                    throw new ArgumentOutOfRangeException("index", index, "Value greater than stack index.");
                return _items[index];
            }
        }

        /// <summary>
        /// Gets an item relative to the current stack pointer. The index must be a negative value (-1, -2, etc.).
        /// </summary>
        public PdfItem GetItem(int relativeIndex)
        {
            if (relativeIndex >= 0 || -relativeIndex > _sp)
                throw new ArgumentOutOfRangeException("relativeIndex", relativeIndex, "Value out of stack range.");
            return _items[_sp + relativeIndex];
        }

        /// <summary>
        /// Gets an item relative to the current stack pointer. The index must be a negative value (-1, -2, etc.).
        /// </summary>
        public int GetInteger(int relativeIndex)
        {
            if (relativeIndex >= 0 || -relativeIndex > _sp)
                throw new ArgumentOutOfRangeException("relativeIndex", relativeIndex, "Value out of stack range.");
            return ((PdfInteger)_items[_sp + relativeIndex]).Value;
        }

        /// <summary>
        /// Pushes the specified item onto the stack.
        /// </summary>
        public void Shift(PdfItem item)
        {
            Debug.Assert(item != null);
            _items.Add(item);
            _sp++;
        }

        /// <summary>
        /// Replaces the last 'count' items with the specified item.
        /// </summary>
        public void Reduce(int count)
        {
            if (count > _sp)
                throw new ArgumentException("count causes stack underflow.");
            _items.RemoveRange(_sp - count, count);
            _sp -= count;
        }

        /// <summary>
        /// Replaces the last 'count' items with the specified item.
        /// </summary>
        public void Reduce(PdfItem item, int count)
        {
            Debug.Assert(item != null);
            Reduce(count);
            _items.Add(item);
            _sp++;
        }

        /// <summary>
        /// The stack pointer index. Points to the next free item.
        /// </summary>
        int _sp;

        /// <summary>
        /// An array representing the stack.
        /// </summary>
        readonly List<PdfItem> _items;
    }
}
