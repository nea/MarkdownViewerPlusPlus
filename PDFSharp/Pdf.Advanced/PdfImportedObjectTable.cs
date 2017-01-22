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

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents the imported objects of an external document. Used to cache objects that are
    /// already imported when a PdfFormXObject is added to a page.
    /// </summary>
    internal sealed class PdfImportedObjectTable
    {
        /// <summary>
        /// Initializes a new instance of this class with the document the objects are imported from.
        /// </summary>
        public PdfImportedObjectTable(PdfDocument owner, PdfDocument externalDocument)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            if (externalDocument == null)
                throw new ArgumentNullException("externalDocument");
            _owner = owner;
            _externalDocumentHandle = externalDocument.Handle;
            _xObjects = new PdfFormXObject[externalDocument.PageCount];
        }
        readonly PdfFormXObject[] _xObjects;

        /// <summary>
        /// Gets the document this table belongs to.
        /// </summary>
        public PdfDocument Owner
        {
            get { return _owner; }
        }
        readonly PdfDocument _owner;

        /// <summary>
        /// Gets the external document, or null, if the external document is garbage collected.
        /// </summary>
        public PdfDocument ExternalDocument
        {
            get { return _externalDocumentHandle.IsAlive ? _externalDocumentHandle.Target : null; }
        }
        readonly PdfDocument.DocumentHandle _externalDocumentHandle;

        public PdfFormXObject GetXObject(int pageNumber)
        {
            return _xObjects[pageNumber - 1];
        }

        public void SetXObject(int pageNumber, PdfFormXObject xObject)
        {
            _xObjects[pageNumber - 1] = xObject;
        }

        /// <summary>
        /// Indicates whether the specified object is already imported.
        /// </summary>
        public bool Contains(PdfObjectID externalID)
        {
            return _externalIDs.ContainsKey(externalID.ToString());
        }

        /// <summary>
        /// Adds a cloned object to this table.
        /// </summary>
        /// <param name="externalID">The object identifier in the foreign object.</param>
        /// <param name="iref">The cross reference to the clone of the foreign object, which belongs to
        /// this document. In general the clone has a different object identifier.</param>
        public void Add(PdfObjectID externalID, PdfReference iref)
        {
            _externalIDs[externalID.ToString()] = iref;
        }

        /// <summary>
        /// Gets the cloned object that corresponds to the specified external identifier.
        /// </summary>
        public PdfReference this[PdfObjectID externalID]
        {
            get { return _externalIDs[externalID.ToString()]; }
        }

        /// <summary>
        /// Maps external object identifiers to cross reference entries of the importing document
        /// {PdfObjectID -> PdfReference}.
        /// </summary>
        readonly Dictionary<string, PdfReference> _externalIDs = new Dictionary<string, PdfReference>();
    }
}
