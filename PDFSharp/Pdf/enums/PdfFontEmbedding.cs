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
    /// Specifies the embedding options of an XFont when converted into PDF.
    /// Font embedding is not optional anymore. So Always is the only option.
    /// </summary>
    public enum PdfFontEmbedding
    {
        /// <summary>
        /// All fonts are embedded.
        /// </summary>
        Always,

        /// <summary>
        /// Fonts are not embedded. This is not an option anymore.
        /// </summary>
        [Obsolete("Fonts must always be embedded.")]
        None,

        /// <summary>
        /// Unicode fonts are embedded, WinAnsi fonts are not embedded.
        /// </summary>
        [Obsolete("Fonts must always be embedded.")]
        Default,

        /// <summary>
        /// Not yet implemented.
        /// </summary>
        [Obsolete("Fonts must always be embedded.")]
        Automatic,
    }
}
