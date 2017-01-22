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
using System.Text;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Determines the encoding of a PdfString or PdfStringObject.
    /// </summary>
    [Flags]
    public enum PdfStringEncoding
    {
        /// <summary>
        /// The characters of the string are actually bytes with an unknown or context specific meaning or encoding.
        /// With this encoding the 8 high bits of each character is zero.
        /// </summary>
        RawEncoding = PdfStringFlags.RawEncoding,

        /// <summary>
        /// Not yet used by PDFsharp.
        /// </summary>
        StandardEncoding = PdfStringFlags.StandardEncoding,

        /// <summary>
        /// The characters of the string are actually bytes with PDF document encoding.
        /// With this encoding the 8 high bits of each character is zero.
        /// </summary>
        // ReSharper disable InconsistentNaming because the name is spelled as in the Adobe reference.
        PDFDocEncoding = PdfStringFlags.PDFDocEncoding,
        // ReSharper restore InconsistentNaming

        /// <summary>
        /// The characters of the string are actually bytes with Windows ANSI encoding.
        /// With this encoding the 8 high bits of each character is zero.
        /// </summary>
        WinAnsiEncoding = PdfStringFlags.WinAnsiEncoding,

        /// <summary>
        /// Not yet used by PDFsharp.
        /// </summary>
        MacRomanEncoding = PdfStringFlags.MacExpertEncoding,

        /// <summary>
        /// Not yet used by PDFsharp.
        /// </summary>
        MacExpertEncoding = PdfStringFlags.MacExpertEncoding,

        /// <summary>
        /// The characters of the string are Unicode characters.
        /// </summary>
        Unicode = PdfStringFlags.Unicode,
    }

    /// <summary>
    /// Internal wrapper for PdfStringEncoding.
    /// </summary>
    [Flags]
    enum PdfStringFlags
    {
        // ReSharper disable InconsistentNaming
        RawEncoding = 0x00,
        StandardEncoding = 0x01,  // not used by PDFsharp
        PDFDocEncoding = 0x02,
        WinAnsiEncoding = 0x03,
        MacRomanEncoding = 0x04,  // not used by PDFsharp
        MacExpertEncoding = 0x05,  // not used by PDFsharp
        Unicode = 0x06,
        EncodingMask = 0x0F,

        HexLiteral = 0x80,
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Represents a direct text string value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PdfString : PdfItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfString"/> class.
        /// </summary>
        public PdfString()
        {
            // Redundant assignment.
            //_flags = PdfStringFlags.RawEncoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfString(string value)
        {
            CheckRawEncoding(value);
            _value = value;
            //_flags = PdfStringFlags.RawEncoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfString"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="encoding">The encoding.</param>
        public PdfString(string value, PdfStringEncoding encoding)
        {
            switch (encoding)
            {
                case PdfStringEncoding.RawEncoding:
                    CheckRawEncoding(value);
                    break;

                case PdfStringEncoding.StandardEncoding:
                    break;

                case PdfStringEncoding.PDFDocEncoding:
                    break;

                case PdfStringEncoding.WinAnsiEncoding:
                    CheckRawEncoding(value);
                    break;

                case PdfStringEncoding.MacRomanEncoding:
                    break;

                case PdfStringEncoding.Unicode:
                    break;

                default:
                    throw new ArgumentOutOfRangeException("encoding");
            }
            _value = value;
            //if ((flags & PdfStringFlags.EncodingMask) == 0)
            //  flags |= PdfStringFlags.PDFDocEncoding;
            _flags = (PdfStringFlags)encoding;
        }

        internal PdfString(string value, PdfStringFlags flags)
        {
            _value = value;
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
        /// Gets the encoding.
        /// </summary>
        public PdfStringEncoding Encoding
        {
            get { return (PdfStringEncoding)(_flags & PdfStringFlags.EncodingMask); }
        }

        /// <summary>
        /// Gets a value indicating whether the string is a hexadecimal literal.
        /// </summary>
        public bool HexLiteral
        {
            get { return (_flags & PdfStringFlags.HexLiteral) != 0; }
        }

        internal PdfStringFlags Flags
        {
            get { return _flags; }
        }
        readonly PdfStringFlags _flags;

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string Value
        {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get { return _value ?? ""; }
        }
        string _value;

        /// <summary>
        /// Gets or sets the string value for encryption purposes.
        /// </summary>
        internal byte[] EncryptionValue
        {
            // TODO: Unicode case is not handled!
            get { return _value == null ? new byte[0] : PdfEncoders.RawEncoding.GetBytes(_value); }
            // BUG: May lead to trouble with the value semantics of PdfString
            set { _value = PdfEncoders.RawEncoding.GetString(value, 0, value.Length); }
        }

        /// <summary>
        /// Returns the string.
        /// </summary>
        public override string ToString()
        {
#if true
            PdfStringEncoding encoding = (PdfStringEncoding)(_flags & PdfStringFlags.EncodingMask);
            string pdf = (_flags & PdfStringFlags.HexLiteral) == 0 ?
                PdfEncoders.ToStringLiteral(_value, encoding, null) :
                PdfEncoders.ToHexStringLiteral(_value, encoding, null);
            return pdf;
#else
            return _value;
#endif
        }

        /// <summary>
        /// Hack for document encoded bookmarks.
        /// </summary>
        public string ToStringFromPdfDocEncoded()
        {
            int length = _value.Length;
            char[] bytes = new char[length];
            for (int idx = 0; idx < length; idx++)
            {
                char ch = _value[idx];
                if (ch <= 255)
                {
                    bytes[idx] = Encode[ch];
                }
                else
                {
                    //Debug-Break.Break();
                    throw new InvalidOperationException("DocEncoded string contains char greater 255.");
                }
            }
            StringBuilder sb = new StringBuilder(length);
            for (int idx = 0; idx < length; idx++)
                sb.Append((char)bytes[idx]);
            return sb.ToString();
        }
        static readonly char[] Encode =
        {
            '\x00', '\x01', '\x02', '\x03', '\x04', '\x05', '\x06', '\x07', '\x08', '\x09', '\x0A', '\x0B', '\x0C', '\x0D', '\x0E', '\x0F',
            '\x10', '\x11', '\x12', '\x13', '\x14', '\x15', '\x16', '\x17', '\x18', '\x19', '\x1A', '\x1B', '\x1C', '\x1D', '\x1E', '\x1F',
            '\x20', '\x21', '\x22', '\x23', '\x24', '\x25', '\x26', '\x27', '\x28', '\x29', '\x2A', '\x2B', '\x2C', '\x2D', '\x2E', '\x2F',
            '\x30', '\x31', '\x32', '\x33', '\x34', '\x35', '\x36', '\x37', '\x38', '\x39', '\x3A', '\x3B', '\x3C', '\x3D', '\x3E', '\x3F',
            '\x40', '\x41', '\x42', '\x43', '\x44', '\x45', '\x46', '\x47', '\x48', '\x49', '\x4A', '\x4B', '\x4C', '\x4D', '\x4E', '\x4F',
            '\x50', '\x51', '\x52', '\x53', '\x54', '\x55', '\x56', '\x57', '\x58', '\x59', '\x5A', '\x5B', '\x5C', '\x5D', '\x5E', '\x5F',
            '\x60', '\x61', '\x62', '\x63', '\x64', '\x65', '\x66', '\x67', '\x68', '\x69', '\x6A', '\x6B', '\x6C', '\x6D', '\x6E', '\x6F',
            '\x70', '\x71', '\x72', '\x73', '\x74', '\x75', '\x76', '\x77', '\x78', '\x79', '\x7A', '\x7B', '\x7C', '\x7D', '\x7E', '\x7F',
            '\x2022', '\x2020', '\x2021', '\x2026', '\x2014', '\x2013', '\x0192', '\x2044', '\x2039', '\x203A', '\x2212', '\x2030', '\x201E', '\x201C', '\x201D', '\x2018',
            '\x2019', '\x201A', '\x2122', '\xFB01', '\xFB02', '\x0141', '\x0152', '\x0160', '\x0178', '\x017D', '\x0131', '\x0142', '\x0153', '\x0161', '\x017E', '\xFFFD',
            '\x20AC', '\xA1', '\xA2', '\xA3', '\xA4', '\xA5', '\xA6', '\xA7', '\xA8', '\xA9', '\xAA', '\xAB', '\xAC', '\xAD', '\xAE', '\xAF',
            '\xB0', '\xB1', '\xB2', '\xB3', '\xB4', '\xB5', '\xB6', '\xB7', '\xB8', '\xB9', '\xBA', '\xBB', '\xBC', '\xBD', '\xBE', '\xBF',
            '\xC0', '\xC1', '\xC2', '\xC3', '\xC4', '\xC5', '\xC6', '\xC7', '\xC8', '\xC9', '\xCA', '\xCB', '\xCC', '\xCD', '\xCE', '\xCF',
            '\xD0', '\xD1', '\xD2', '\xD3', '\xD4', '\xD5', '\xD6', '\xD7', '\xD8', '\xD9', '\xDA', '\xDB', '\xDC', '\xDD', '\xDE', '\xDF',
            '\xE0', '\xE1', '\xE2', '\xE3', '\xE4', '\xE5', '\xE6', '\xE7', '\xE8', '\xE9', '\xEA', '\xEB', '\xEC', '\xED', '\xEE', '\xEF',
            '\xF0', '\xF1', '\xF2', '\xF3', '\xF4', '\xF5', '\xF6', '\xF7', '\xF8', '\xF9', '\xFA', '\xFB', '\xFC', '\xFD', '\xFE', '\xFF',
        };

        static void CheckRawEncoding(string s)
        {
            if (String.IsNullOrEmpty(s))
                return;

            int length = s.Length;
            for (int idx = 0; idx < length; idx++)
            {
                Debug.Assert(s[idx] < 256, "RawString contains invalid character.");
            }
        }

        /// <summary>
        /// Writes the string DocEncoded.
        /// </summary>
        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }
    }
}
