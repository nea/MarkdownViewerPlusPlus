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

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Specifies the encoding schema used for an XFont when converted into PDF.
    /// </summary>
    public enum PdfFontEncoding
    {
        // TABLE

        /// <summary>
        /// Cause a font to use Windows-1252 encoding to encode text rendered with this font.
        /// Same as Windows1252 encoding.
        /// </summary>
        WinAnsi = 0,

        ///// <summary>
        ///// Cause a font to use Windows-1252 (aka WinAnsi) encoding to encode text rendered with this font.
        ///// </summary>
        //Windows1252 = 0,

        /// <summary>
        /// Cause a font to use Unicode encoding to encode text rendered with this font.
        /// </summary>
        Unicode = 1,

        /// <summary>
        /// Unicode encoding.
        /// </summary>
        [Obsolete("Use WinAnsi or Unicode")]
        Automatic = 1,  // Force Unicode when used.

        // Implementation note: PdfFontEncoding uses incorrect terms.
        // WinAnsi correspond to WinAnsiEncoding, while Unicode uses glyph indices.
        // Furthermre the term WinAnsi is an oxymoron.
        // Reference: TABLE  D.1 Latin-text encodings / Page 996
    }
}
