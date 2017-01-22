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

using System.Collections.Generic;

namespace PdfSharp.Pdf.Internal
{
#if true_
    /// <summary>
    /// Provides a thread-local cache for large objects.
    /// </summary>
    internal class GlobalObjectTable_not_in_use
    {
        public GlobalObjectTable_not_in_use()
        { }

        public void AttatchDocument(PdfDocument.DocumentHandle handle)
        {
            lo ck (_documentHandles)
            {
                _documentHandles.Add(handle);
            }
        }

        public void DetatchDocument(PdfDocument.DocumentHandle handle)
        {
            lo ck (_documentHandles)
            {
                // Notify other documents about detach
                int count = _documentHandles.Count;
                for (int idx = 0; idx < count; idx++)
                {
                    if (((PdfDocument.DocumentHandle)_documentHandles[idx]).IsAlive)
                    {
                        PdfDocument target = ((PdfDocument.DocumentHandle)_documentHandles[idx]).Target;
                        if (target != null)
                            target.OnExternalDocumentFinalized(handle);
                    }
                }

                // Clean up table
                for (int idx = 0; idx < _documentHandles.Count; idx++)
                {
                    PdfDocument target = ((PdfDocument.DocumentHandle)_documentHandles[idx]).Target;
                    if (target == null)
                    {
                        _documentHandles.RemoveAt(idx);
                        idx--;
                    }
                }
            }

            //lo ck (documents)
            //{
            //  int index = IndexOf(document);
            //  if (index != -1)
            //  {
            //    documents.RemoveAt(index);
            //    int count = documents.Count;
            //    for (int idx = 0; idx < count; idx++)
            //    {
            //      PdfDocument target = ((WeakReference)documents[idx]).Target as PdfDocument;
            //      if (target != null)
            //        target.OnExternalDocumentFinalized(document);
            //    }

            //    for (int idx = 0; idx < documents.Count; idx++)
            //    {
            //      PdfDocument target = ((WeakReference)documents[idx]).Target as PdfDocument;
            //      if (target == null)
            //      {
            //        documents.RemoveAt(idx);
            //        idx--;
            //      }
            //    }
            //  }
            //}
        }

        //int IndexOf(PdfDocument.Handle handle)
        //{
        //  int count = documents.Count;
        //  for (int idx = 0; idx < count; idx++)
        //  {
        //    if ((PdfDocument.Handle)documents[idx] == handle)
        //      return idx;
        //    //if (Object.ReferenceEquals(((WeakReference)documents[idx]).Target, document))
        //    //  return idx;
        //  }
        //  return -1;
        //}

        /// <summary>
        /// Array of handles to all documents.
        /// </summary>
        readonly List<object> _documentHandles = new List<object>();
    }
#endif
}
