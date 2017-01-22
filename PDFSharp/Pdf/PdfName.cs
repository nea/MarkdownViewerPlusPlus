#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
//
// Copyright (c) 2005-20165 empira Software GmbH, Cologne Area (Germany)
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
using PdfSharp.Pdf.IO;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents a PDF name value.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public sealed class PdfName : PdfItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfName"/> class.
        /// </summary>
        public PdfName()
        {
            _value = "/";  // Empty name.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfName"/> class.
        /// Parameter value always must start with a '/'.
        /// </summary>
        public PdfName(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.Length == 0 || value[0] != '/')
                throw new ArgumentException(PSSR.NameMustStartWithSlash);

            _value = value;
        }

        /// <summary>
        /// Determines whether the specified object is equal to this name.
        /// </summary>
        public override bool Equals(object obj)
        {
            return _value.Equals(obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Gets the name as a string.
        /// </summary>
        public string Value
        {
            // This class must behave like a value type. Therefore it cannot be changed (like System.String).
            get { return _value; }
        }
        readonly string _value;

        /// <summary>
        /// Returns the name. The string always begins with a slash.
        /// </summary>
        public override string ToString()
        {
            return _value;
        }

        /// <summary>
        /// Determines whether the specified name and string are equal.
        /// </summary>
        public static bool operator ==(PdfName name, string str)
        {
            if (ReferenceEquals(name, null))
                return str == null;

            return name._value == str;
        }

        /// <summary>
        /// Determines whether the specified name and string are not equal.
        /// </summary>
        public static bool operator !=(PdfName name, string str)
        {
            if (ReferenceEquals(name, null))
                return str != null;

            return name._value != str;
        }

        /// <summary>
        /// Represents the empty name.
        /// </summary>
        public static readonly PdfName Empty = new PdfName("/");

        /// <summary>
        /// Writes the name including the leading slash.
        /// </summary>
        internal override void WriteObject(PdfWriter writer)
        {
            // TODO: what if unicode character are part of the name? 
            writer.Write(this);
        }

        /// <summary>
        /// Gets the comparer for this type.
        /// </summary>
        public static PdfXNameComparer Comparer
        {
            get { return new PdfXNameComparer(); }
        }

        /// <summary>
        /// Implements a comparer that compares PdfName objects.
        /// </summary>
        public class PdfXNameComparer : IComparer<PdfName>
        {
            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="l">The first object to compare.</param>
            /// <param name="r">The second object to compare.</param>
            public int Compare(PdfName l, PdfName r)
            {
#if true_
#else
                if (l != null)
                {
                    if (r != null)
                        return String.Compare(l._value, r._value, StringComparison.Ordinal);
                    return -1;
                }
                if (r != null)
                    return 1;
                return 0;
#endif
            }
        }
    }
}
