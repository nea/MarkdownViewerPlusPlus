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

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Base class for all dictionaries that map resource names to objects.
    /// </summary>
    internal class PdfResourceMap : PdfDictionary //, IEnumerable
    {
        public PdfResourceMap()
        { }

        public PdfResourceMap(PdfDocument document)
            : base(document)
        { }

        protected PdfResourceMap(PdfDictionary dict)
            : base(dict)
        { }

        //    public int Count
        //    {
        //      get {return resources.Count;}
        //    }
        //
        //    public PdfObject this[string key]
        //    {
        //      get {return resources[key] as PdfObject;}
        //      set {resources[key] = value;}
        //    }

        /// <summary>
        /// Adds all imported resource names to the specified hashtable.
        /// </summary>
        internal void CollectResourceNames(Dictionary<string, object> usedResourceNames)
        {
            // ?TODO: Imported resources (e.g. fonts) can be reused, but I think this is rather difficult. Will be an issue in PDFsharp 2.0.
            PdfName[] names = Elements.KeyNames;
            foreach (PdfName name in names)
                usedResourceNames.Add(name.ToString(), null);
        }
    }
}
