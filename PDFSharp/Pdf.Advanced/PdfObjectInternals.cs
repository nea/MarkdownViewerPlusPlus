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

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Provides access to the internal PDF object data structures. This class prevents the public
    /// interfaces from pollution with to much internal functions.
    /// </summary>
    public class PdfObjectInternals
    {
        internal PdfObjectInternals(PdfObject obj)
        {
            _obj = obj;
        }
        readonly PdfObject _obj;

        /// <summary>
        /// Gets the object identifier. Returns PdfObjectID.Empty for direct objects.
        /// </summary>
        public PdfObjectID ObjectID
        {
            get { return _obj.ObjectID; }
        }

        /// <summary>
        /// Gets the object number.
        /// </summary>
        public int ObjectNumber
        {
            get { return _obj.ObjectID.ObjectNumber; }
        }

        /// <summary>
        /// Gets the generation number.
        /// </summary>
        public int GenerationNumber
        {
            get { return _obj.ObjectID.GenerationNumber; }
        }

        /// <summary>
        /// Gets the name of the current type.
        /// Not a very useful property, but can be used for data binding.
        /// </summary>
        public string TypeID
        {
            get
            {
                if (_obj is PdfArray)
                    return "array";
                if (_obj is PdfDictionary)
                    return "dictionary";
                return _obj.GetType().Name;
            }
        }
    }
}
