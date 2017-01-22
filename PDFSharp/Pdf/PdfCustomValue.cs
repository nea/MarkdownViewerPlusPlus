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

namespace PdfSharp.Pdf
{
    /// <summary>
    /// This class is intended for empira internal use only and may change or drop in future releases.
    /// </summary>
    public class PdfCustomValue : PdfDictionary
    {
        /// <summary>
        /// This function is intended for empira internal use only.
        /// </summary>
        public PdfCustomValue()
        {
            CreateStream(new byte[] { });
        }

        /// <summary>
        /// This function is intended for empira internal use only.
        /// </summary>
        public PdfCustomValue(byte[] bytes)
        {
            CreateStream(bytes);
        }

        internal PdfCustomValue(PdfDocument document)
            : base(document)
        {
            CreateStream(new byte[] { });
        }

        internal PdfCustomValue(PdfDictionary dict)
            : base(dict)
        {
            // TODO: uncompress stream
        }

        /// <summary>
        /// This property is intended for empira internal use only.
        /// </summary>
        public PdfCustomValueCompressionMode CompressionMode;

        /// <summary>
        /// This property is intended for empira internal use only.
        /// </summary>
        public byte[] Value
        {
            get { return Stream.Value; }
            set { Stream.Value = value; }
        }
    }
}
