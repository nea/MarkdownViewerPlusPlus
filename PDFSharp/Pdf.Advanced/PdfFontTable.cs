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
using System.Collections.Generic;
using PdfSharp.Drawing;

namespace PdfSharp.Pdf.Advanced
{
    internal enum FontType
    {
        /// <summary>
        /// TrueType with WinAnsi encoding.
        /// </summary>
        TrueType = 1,

        /// <summary>
        /// TrueType with Identity-H or Identity-V encoding (unicode).
        /// </summary>
        Type0 = 2,
    }

    /// <summary>
    /// Contains all used fonts of a document.
    /// </summary>
    internal sealed class PdfFontTable : PdfResourceTable
    {
        /// <summary>
        /// Initializes a new instance of this class, which is a singleton for each document.
        /// </summary>
        public PdfFontTable(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Gets a PdfFont from an XFont. If no PdfFont already exists, a new one is created.
        /// </summary>
        public PdfFont GetFont(XFont font)
        {
            string selector = font.Selector;
            if (selector == null)
            {
                selector = ComputeKey(font); //new FontSelector(font);
                font.Selector = selector;
            }
            PdfFont pdfFont;
            if (!_fonts.TryGetValue(selector, out pdfFont))
            {
                if (font.Unicode)
                    pdfFont = new PdfType0Font(Owner, font, font.IsVertical);
                else
                    pdfFont = new PdfTrueTypeFont(Owner, font);
                //pdfFont.Document = _document;
                Debug.Assert(pdfFont.Owner == Owner);
                _fonts[selector] = pdfFont;
            }
            return pdfFont;
        }

#if true
        /// <summary>
        /// Gets a PdfFont from a font program. If no PdfFont already exists, a new one is created.
        /// </summary>
        public PdfFont GetFont(string idName, byte[] fontData)
        {
            Debug.Assert(false);
            //FontSelector selector = new FontSelector(idName);
            string selector = null; // ComputeKey(font); //new FontSelector(font);
            PdfFont pdfFont;
            if (!_fonts.TryGetValue(selector, out pdfFont))
            {
                //if (font.Unicode)
                pdfFont = new PdfType0Font(Owner, idName, fontData, false);
                //else
                //  pdfFont = new PdfTrueTypeFont(_owner, font);
                //pdfFont.Document = _document;
                Debug.Assert(pdfFont.Owner == Owner);
                _fonts[selector] = pdfFont;
            }
            return pdfFont;
        }
#endif

        /// <summary>
        /// Tries to gets a PdfFont from the font dictionary.
        /// Returns null if no such PdfFont exists.
        /// </summary>
        public PdfFont TryGetFont(string idName)
        {
            Debug.Assert(false);
            //FontSelector selector = new FontSelector(idName);
            string selector = null;
            PdfFont pdfFont;
            _fonts.TryGetValue(selector, out pdfFont);
            return pdfFont;
        }

        internal static string ComputeKey(XFont font)
        {
            XGlyphTypeface glyphTypeface = font.GlyphTypeface;
            string key = glyphTypeface.Fontface.FullFaceName.ToLowerInvariant() +
                (glyphTypeface.IsBold ? "/b" : "") + (glyphTypeface.IsItalic ? "/i" : "") + font.Unicode;
            return key;
        }

        /// <summary>
        /// Map from PdfFontSelector to PdfFont.
        /// </summary>
        readonly Dictionary<string, PdfFont> _fonts = new Dictionary<string, PdfFont>();

        public void PrepareForSave()
        {
            foreach (PdfFont font in _fonts.Values)
                font.PrepareForSave();
        }
    }
}
