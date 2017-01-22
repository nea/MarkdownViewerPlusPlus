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
using System.IO;
using PdfSharp.Pdf.IO;

namespace PdfSharp.Pdf.Internal
{
    /// <summary>
    /// Provides a thread-local cache for large objects.
    /// </summary>
    internal class ThreadLocalStorage // #???
    {
        public ThreadLocalStorage()
        {
            _importedDocuments = new Dictionary<string, PdfDocument.DocumentHandle>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddDocument(string path, PdfDocument document)
        {
            _importedDocuments.Add(path, document.Handle);
        }

        public void RemoveDocument(string path)
        {
            _importedDocuments.Remove(path);
        }

        public PdfDocument GetDocument(string path)
        {
            Debug.Assert(path.StartsWith("*") || Path.IsPathRooted(path), "Path must be full qualified.");

            PdfDocument document = null;
            PdfDocument.DocumentHandle handle;
            if (_importedDocuments.TryGetValue(path, out handle))
            {
                document = handle.Target;
                if (document == null)
                    RemoveDocument(path);
            }
            if (document == null)
            {
                document = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                _importedDocuments.Add(path, document.Handle);
            }
            return document;
        }

        public PdfDocument[] Documents
        {
            get
            {
                List<PdfDocument> list = new List<PdfDocument>();
                foreach (PdfDocument.DocumentHandle handle in _importedDocuments.Values)
                {
                    if (handle.IsAlive)
                        list.Add(handle.Target);
                }
                return list.ToArray();
            }
        }

        public void DetachDocument(PdfDocument.DocumentHandle handle)
        {
            if (handle.IsAlive)
            {
                foreach (string path in _importedDocuments.Keys)
                {
                    if (_importedDocuments[path] == handle)
                    {
                        _importedDocuments.Remove(path);
                        break;
                    }
                }
            }

            // Clean table
            bool itemRemoved = true;
            while (itemRemoved)
            {
                itemRemoved = false;
                foreach (string path in _importedDocuments.Keys)
                {
                    if (!_importedDocuments[path].IsAlive)
                    {
                        _importedDocuments.Remove(path);
                        itemRemoved = true;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Maps path to document handle.
        /// </summary>
        readonly Dictionary<string, PdfDocument.DocumentHandle> _importedDocuments;
    }
}