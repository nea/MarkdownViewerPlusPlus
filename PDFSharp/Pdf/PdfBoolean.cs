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

using System.Diagnostics;
using PdfSharp.Pdf.IO;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents a direct boolean value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PdfBoolean : PdfItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBoolean"/> class.
        /// </summary>
        public PdfBoolean()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfBoolean"/> class.
        /// </summary>
        public PdfBoolean(bool value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets the value of this instance as boolean value.
        /// </summary>
        public bool Value
        {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get { return _value; }
        }
        readonly bool _value;

        /// <summary>
        /// A pre-defined value that represents <c>true</c>.
        /// </summary>
        public static readonly PdfBoolean True = new PdfBoolean(true);

        /// <summary>
        /// A pre-defined value that represents <c>false</c>.
        /// </summary>
        public static readonly PdfBoolean False = new PdfBoolean(false);

        /// <summary>
        /// Returns 'false' or 'true'.
        /// </summary>
        public override string ToString()
        {
            return _value ? bool.TrueString : bool.FalseString;
        }

        /// <summary>
        /// Writes 'true' or 'false'.
        /// </summary>
        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }
    }
}
