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
using System.Collections;
using System.Text;
using System.IO;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using System.Collections.Generic;

namespace PdfSharp.Pdf.Annotations
{
    /// <summary>
    /// Represents the annotations array of a page.
    /// </summary>
    public sealed class PdfAnnotations : PdfArray
    {
        internal PdfAnnotations(PdfDocument document)
            : base(document)
        { }

        internal PdfAnnotations(PdfArray array)
            : base(array)
        { }

        /// <summary>
        /// Adds the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public void Add(PdfAnnotation annotation)
        {
            annotation.Document = Owner;
            Owner._irefTable.Add(annotation);
            Elements.Add(annotation.Reference);
        }

        /// <summary>
        /// Removes an annotation from the document.
        /// </summary>
        public void Remove(PdfAnnotation annotation)
        {
            if (annotation.Owner != Owner)
                throw new InvalidOperationException("The annotation does not belong to this document.");

            Owner.Internals.RemoveObject(annotation);
            Elements.Remove(annotation.Reference);
        }

        /// <summary>
        /// Removes all the annotations from the current page.
        /// </summary>
        public void Clear()
        {
            for (int idx = Count - 1; idx >= 0; idx--)
                Page.Annotations.Remove(_page.Annotations[idx]);
        }

        //public void Insert(int index, PdfAnnotation annotation)
        //{
        //  annotation.Document = Document;
        //  annotations.Insert(index, annotation);
        //}

        /// <summary>
        /// Gets the number of annotations in this collection.
        /// </summary>
        public int Count
        {
            get { return Elements.Count; }
        }

        /// <summary>
        /// Gets the <see cref="PdfSharp.Pdf.Annotations.PdfAnnotation"/> at the specified index.
        /// </summary>
        public PdfAnnotation this[int index]
        {
            get
            {
                PdfReference iref;
                PdfDictionary dict;
                PdfItem item = Elements[index];
                if ((iref = item as PdfReference) != null)
                {
                    Debug.Assert(iref.Value is PdfDictionary, "Reference to dictionary expected.");
                    dict = (PdfDictionary)iref.Value;
                }
                else
                {
                    Debug.Assert(item is PdfDictionary, "Dictionary expected.");
                    dict = (PdfDictionary)item;
                }
                PdfAnnotation annotation = dict as PdfAnnotation;
                if (annotation == null)
                {
                    annotation = new PdfGenericAnnotation(dict);
                    if (iref == null)
                        Elements[index] = annotation;
                }
                return annotation;
            }
        }

        //public PdfAnnotation this[int index]
        //{
        //  get 
        //  {
        //      //DMH 6/7/06
        //      //Broke this out to simplfy debugging
        //      //Use a generic annotation to access the Meta data
        //      //Assign this as the parent of the annotation
        //      PdfReference r = Elements[index] as PdfReference;
        //      PdfDictionary d = r.Value as PdfDictionary;
        //      PdfGenericAnnotation a = new PdfGenericAnnotation(d);
        //      a.Collection = this;
        //      return a;
        //  }
        //}

        /// <summary>
        /// Gets the page the annotations belongs to.
        /// </summary>
        internal PdfPage Page
        {
            get { return _page; }
            set { _page = value; }
        }
        PdfPage _page;

        /// <summary>
        /// Fixes the /P element in imported annotation.
        /// </summary>
        internal static void FixImportedAnnotation(PdfPage page)
        {
            PdfArray annots = page.Elements.GetArray(PdfPage.Keys.Annots);
            if (annots != null)
            {
                int count = annots.Elements.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    PdfDictionary annot = annots.Elements.GetDictionary(idx);
                    if (annot != null && annot.Elements.ContainsKey("/P"))
                        annot.Elements["/P"] = page.Reference;
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        public override IEnumerator<PdfItem> GetEnumerator()
        {
            return (IEnumerator<PdfItem>)new AnnotationsIterator(this);
        }
        // THHO4STLA: AnnotationsIterator: Implementation does not work http://forum.pdfsharp.net/viewtopic.php?p=3285#p3285
        // Code using the enumerator like this will crash:
            //foreach (var annotation in page.Annotations)
            //{
            //    annotation.GetType();
            //}

        //!!!new 2015-10-15: use PdfItem instead of PdfAnnotation. 
        // TODO Should we change this to "public new IEnumerator<PdfAnnotation> GetEnumerator()"?

        class AnnotationsIterator : IEnumerator<PdfItem/*PdfAnnotation*/>
        {
            public AnnotationsIterator(PdfAnnotations annotations)
            {
                _annotations = annotations;
                _index = -1;
            }

            public PdfItem/*PdfAnnotation*/ Current
            {
                get { return _annotations[_index]; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return ++_index < _annotations.Count;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {
                //throw new NotImplementedException();
            }

            readonly PdfAnnotations _annotations;
            int _index;
        }
    }
}
