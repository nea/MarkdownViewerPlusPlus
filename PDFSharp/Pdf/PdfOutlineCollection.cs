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
using System.Collections;
using PdfSharp.Drawing;

// Review: CountOpen does not work. - StL/14-10-05

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents a collection of outlines.
    /// </summary>
    public class PdfOutlineCollection : PdfObject, ICollection<PdfOutline>, IList<PdfOutline>
    {
        /// <summary>
        /// Can only be created as part of PdfOutline.
        /// </summary>
        internal PdfOutlineCollection(PdfDocument document, PdfOutline parent)
            : base(document)
        {
            _parent = parent;
        }

        /// <summary>
        /// Indicates whether the outline collection has at least one entry.
        /// </summary>
        [Obsolete("Use 'Count > 0' - HasOutline will throw exception.")]
        public bool HasOutline  // DELETE: 15-10-01
        {
            get
            {
                //return Count > 0;
                throw new InvalidOperationException("Use 'Count > 0'");
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the collection.
        /// </summary>
        public bool Remove(PdfOutline item)
        {
            if (_outlines.Remove(item))
            {
                RemoveFromOutlinesTree(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the number of entries in this collection.
        /// </summary>
        public int Count
        {
            get { return _outlines.Count; }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Adds the specified outline.
        /// </summary>
        public void Add(PdfOutline outline)
        {
            if (outline == null)
                throw new ArgumentNullException("outline");

            // DestinationPage is optional. PDFsharp does not yet support outlines with action ("/A") instead of destination page ("/DEST")
            if (outline.DestinationPage != null && !ReferenceEquals(Owner, outline.DestinationPage.Owner))
                throw new ArgumentException("Destination page must belong to this document.");

            //// TODO check the parent problems...
            ////outline.Document = Owner;
            ////outline.Parent = _parent;
            ////Owner._irefTable.Add(outline);

            AddToOutlinesTree(outline);
            _outlines.Add(outline);

            if (outline.Opened)
            {
                outline = _parent;
                while (outline != null)
                {
                    outline.OpenCount++;
                    outline = outline.Parent;
                }
            }
        }

        /// <summary>
        /// Removes all elements form the collection.
        /// </summary>
        public void Clear()
        {
            if (Count > 0)
            {
                PdfOutline[] array = new PdfOutline[Count];
                _outlines.CopyTo(array);
                _outlines.Clear();
                foreach (PdfOutline item in array)
                {
                    RemoveFromOutlinesTree(item);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified element is in the collection.
        /// </summary>
        public bool Contains(PdfOutline item)
        {
            return _outlines.Contains(item);
        }

        /// <summary>
        /// Copies the collection to an array, starting at the specified index of the target array.
        /// </summary>
        public void CopyTo(PdfOutline[] array, int arrayIndex)
        {
            _outlines.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Adds the specified outline entry.
        /// </summary>
        /// <param name="title">The outline text.</param>
        /// <param name="destinationPage">The destination page.</param>
        /// <param name="opened">Specifies whether the node is displayed expanded (opened) or collapsed.</param>
        /// <param name="style">The font style used to draw the outline text.</param>
        /// <param name="textColor">The color used to draw the outline text.</param>
        public PdfOutline Add(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style, XColor textColor)
        {
            PdfOutline outline = new PdfOutline(title, destinationPage, opened, style, textColor);
            Add(outline);
            return outline;
        }

        /// <summary>
        /// Adds the specified outline entry.
        /// </summary>
        /// <param name="title">The outline text.</param>
        /// <param name="destinationPage">The destination page.</param>
        /// <param name="opened">Specifies whether the node is displayed expanded (opened) or collapsed.</param>
        /// <param name="style">The font style used to draw the outline text.</param>
        public PdfOutline Add(string title, PdfPage destinationPage, bool opened, PdfOutlineStyle style)
        {
            PdfOutline outline = new PdfOutline(title, destinationPage, opened, style);
            Add(outline);
            return outline;
        }

        /// <summary>
        /// Adds the specified outline entry.
        /// </summary>
        /// <param name="title">The outline text.</param>
        /// <param name="destinationPage">The destination page.</param>
        /// <param name="opened">Specifies whether the node is displayed expanded (opened) or collapsed.</param>
        public PdfOutline Add(string title, PdfPage destinationPage, bool opened)
        {
            PdfOutline outline = new PdfOutline(title, destinationPage, opened);
            Add(outline);
            return outline;
        }

        /// <summary>
        /// Creates a PdfOutline and adds it into the outline collection.
        /// </summary>
        public PdfOutline Add(string title, PdfPage destinationPage)
        {
            PdfOutline outline = new PdfOutline(title, destinationPage);
            Add(outline);
            return outline;
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        public int IndexOf(PdfOutline item)
        {
            return _outlines.IndexOf(item);
        }

        /// <summary>
        /// Inserts the item at the specified index.
        /// </summary>
        public void Insert(int index, PdfOutline outline)
        {
            if (outline == null)
                throw new ArgumentNullException("outline");
            if (index < 0 || index >= _outlines.Count)
                throw new ArgumentOutOfRangeException("index", index, PSSR.OutlineIndexOutOfRange);

            AddToOutlinesTree(outline);
            _outlines.Insert(index, outline);
        }

        /// <summary>
        /// Removes the outline item at the specified index.
        /// </summary>
        public void RemoveAt(int index)
        {
            PdfOutline outline = _outlines[index];
            _outlines.RemoveAt(index);
            RemoveFromOutlinesTree(outline);
        }

        /// <summary>
        /// Gets the <see cref="PdfSharp.Pdf.PdfOutline"/> at the specified index.
        /// </summary>
        public PdfOutline this[int index]
        {
            get
            {
                if (index < 0 || index >= _outlines.Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.OutlineIndexOutOfRange);
                return _outlines[index];
            }
            set
            {
                if (index < 0 || index >= _outlines.Count)
                    throw new ArgumentOutOfRangeException("index", index, PSSR.OutlineIndexOutOfRange);
                if (value == null)
                    throw new ArgumentOutOfRangeException("value", null, PSSR.SetValueMustNotBeNull);

                AddToOutlinesTree(value);
                _outlines[index] = value;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the outline collection.
        /// </summary>
        public IEnumerator<PdfOutline> GetEnumerator()
        {
            return _outlines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal int CountOpen()
        {
            int count = 0;
            //foreach (PdfOutline outline in _outlines)
            //    count += outline.CountOpen();
            return count;
        }

        void AddToOutlinesTree(PdfOutline outline)
        {
            if (outline == null)
                throw new ArgumentNullException("outline");

            // DestinationPage is optional. PDFsharp does not yet support outlines with action ("/A") instead of destination page ("/DEST")
            if (outline.DestinationPage != null && !ReferenceEquals(Owner, outline.DestinationPage.Owner))
                throw new ArgumentException("Destination page must belong to this document.");

            // TODO check the parent problems...
            outline.Document = Owner;
            outline.Parent = _parent;

            //_outlines.Add(outline);
            if (!Owner._irefTable.Contains(outline.ObjectID))
                Owner._irefTable.Add(outline);
            else
            {
                outline.GetType();
            }

            //if (outline.Opened)
            //{
            //    outline = _parent;
            //    while (outline != null)
            //    {
            //        outline.OpenCount++;
            //        outline = outline.Parent;
            //    }
            //}
        }

        void RemoveFromOutlinesTree(PdfOutline outline)
        {
            if (outline == null)
                throw new ArgumentNullException("outline");

            // TODO check the parent problems...
            //outline.Document = Owner;
            outline.Parent = null;

            Owner._irefTable.Remove(outline.Reference);
        }

        /// <summary>
        /// The parent outine of this collection.
        /// </summary>
        readonly PdfOutline _parent;

        readonly List<PdfOutline> _outlines = new List<PdfOutline>();
    }
}
