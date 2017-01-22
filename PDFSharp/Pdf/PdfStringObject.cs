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
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents an indirect text string value. This type is not used by PDFsharp. If it is imported from
    /// an external PDF file, the value is converted into a direct object.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PdfStringObject : PdfObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStringObject"/> class.
        /// </summary>
        public PdfStringObject()
        {
            _flags = PdfStringFlags.RawEncoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStringObject"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="value">The value.</param>
        public PdfStringObject(PdfDocument document, string value)
            : base(document)
        {
            _value = value;
            _flags = PdfStringFlags.RawEncoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfStringObject"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="encoding">The encoding.</param>
        public PdfStringObject(string value, PdfStringEncoding encoding)
        {
            _value = value;
            //if ((flags & PdfStringFlags.EncodingMask) == 0)
            //  flags |= PdfStringFlags.PDFDocEncoding;
            _flags = (PdfStringFlags)encoding;
        }

        internal PdfStringObject(string value, PdfStringFlags flags)
        {
            _value = value;
            //if ((flags & PdfStringFlags.EncodingMask) == 0)
            //  flags |= PdfStringFlags.PDFDocEncoding;
            _flags = flags;
        }

        /// <summary>
        /// Gets the number of characters in this string.
        /// </summary>
        public int Length
        {
            get { return _value == null ? 0 : _value.Length; }
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// </summary>
        public PdfStringEncoding Encoding
        {
            get { return (PdfStringEncoding)(_flags & PdfStringFlags.EncodingMask); }
            set { _flags = (_flags & ~PdfStringFlags.EncodingMask) | ((PdfStringFlags)value & PdfStringFlags.EncodingMask); }
        }

        /// <summary>
        /// Gets a value indicating whether the string is a hexadecimal literal.
        /// </summary>
        public bool HexLiteral
        {
            get { return (_flags & PdfStringFlags.HexLiteral) != 0; }
            set { _flags = value ? _flags | PdfStringFlags.HexLiteral : _flags & ~PdfStringFlags.HexLiteral; }
        }
        PdfStringFlags _flags;

        /// <summary>
        /// Gets or sets the value as string
        /// </summary>
        public string Value
        {
            get { return _value ?? ""; }
            set { _value = value ?? ""; }
        }
        string _value;

        /// <summary>
        /// Gets or sets the string value for encryption purposes.
        /// </summary>
        internal byte[] EncryptionValue
        {
            // TODO: Unicode case is not handled!
            get { return _value == null ? new byte[0] : PdfEncoders.RawEncoding.GetBytes(_value); }
            set { _value = PdfEncoders.RawEncoding.GetString(value, 0, value.Length); }
        }

        /// <summary>
        /// Returns the string.
        /// </summary>
        public override string ToString()
        {
            return _value;
        }

        /// <summary>
        /// Writes the string literal with encoding DOCEncoded.
        /// </summary>
        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(new PdfString(_value, _flags));
            writer.WriteEndObject();
        }
    }
}
