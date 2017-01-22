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
    /// Specifies whether to compress JPEG images with the FlateDecode filter.
    /// </summary>
    public enum PdfUseFlateDecoderForJpegImages
    {
        /// <summary>
        /// PDFsharp will try FlateDecode and use it if it leads to a reduction in PDF file size.
        /// When FlateEncodeMode is set to BestCompression, this is more likely to reduce the file size,
        /// but it takes considerably more time to create the PDF file.
        /// </summary>
        Automatic,

        /// <summary>
        /// PDFsharp will never use FlateDecode - files may be a few bytes larger, but file creation is faster.
        /// </summary>
        Never,

        /// <summary>
        /// PDFsharp will always use FlateDecode, even if this leads to larger files;
        /// this option is meant for testing purposes only and should not be used for production code.
        /// </summary>
        Always,
    }
}