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
using System.Collections;
using System.Globalization;
using System.Text;
using System.IO;
using PdfSharp.Drawing;
using PdfSharp.Internal;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents text that is written 'as it is' into the PDF stream. This class can lead to invalid PDF files.
    /// E.g. strings in a literal are not encrypted when the document is saved with a password.
    /// </summary>
    public sealed class PdfLiteral : PdfItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfLiteral"/> class.
        /// </summary>
        public PdfLiteral()
        { }

        /// <summary>
        /// Initializes a new instance with the specified string.
        /// </summary>
        public PdfLiteral(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Initializes a new instance with the culture invariant formatted specified arguments.
        /// </summary>
        public PdfLiteral(string format, params object[] args)
        {
            _value = PdfEncoders.Format(format, args);
        }

        /// <summary>
        /// Creates a literal from an XMatrix
        /// </summary>
        public static PdfLiteral FromMatrix(XMatrix matrix)
        {
            return new PdfLiteral("[" + PdfEncoders.ToString(matrix) + "]");
        }

        /// <summary>
        /// Gets the value as litaral string.
        /// </summary>
        public string Value
        {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get { return _value; }
        }
        readonly string _value = String.Empty;

        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        public override string ToString()
        {
            return _value;
        }

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }
    }
}
