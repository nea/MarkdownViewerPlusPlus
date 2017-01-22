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

using System.IO;
using PdfSharp.Pdf.Content.Objects;

namespace PdfSharp.Pdf.Content
{
    /// <summary>
    /// Represents the functionality for reading PDF content streams.
    /// </summary>
    public static class ContentReader
    {
        /// <summary>
        /// Reads the content stream(s) of the specified page.
        /// </summary>
        /// <param name="page">The page.</param>
        static public CSequence ReadContent(PdfPage page)
        {
            CParser parser = new CParser(page);
            CSequence sequence = parser.ReadContent();

            return sequence;
        }

        /// <summary>
        /// Reads the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        static public CSequence ReadContent(byte[] content)
        {
            CParser parser = new CParser(content);
            CSequence sequence = parser.ReadContent();
            return sequence;
        }

        /// <summary>
        /// Reads the specified content.
        /// </summary>
        /// <param name="content">The content.</param>
        static public CSequence ReadContent(MemoryStream content)
        {
            CParser parser = new CParser(content);
            CSequence sequence = parser.ReadContent();
            return sequence;
        }
    }
}
