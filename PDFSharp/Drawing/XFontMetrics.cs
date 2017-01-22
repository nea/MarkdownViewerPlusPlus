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

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Collects information of a font.
    /// </summary>
    public sealed class XFontMetrics
    {
        internal XFontMetrics(string name, int unitsPerEm, int ascent, int descent, int leading, int lineSpacing,
            int capHeight, int xHeight, int stemV, int stemH, int averageWidth, int maxWidth ,
            int underlinePosition, int underlineThickness, int strikethroughPosition, int strikethroughThickness)
        {
            _name = name;
            _unitsPerEm = unitsPerEm;
            _ascent = ascent;
            _descent = descent;
            _leading = leading;
            _lineSpacing = lineSpacing;
            _capHeight = capHeight;
            _xHeight = xHeight;
            _stemV = stemV;
            _stemH = stemH;
            _averageWidth = averageWidth;
            _maxWidth = maxWidth;
            _underlinePosition = underlinePosition;
            _underlineThickness = underlineThickness;
            _strikethroughPosition = strikethroughPosition;
            _strikethroughThickness = strikethroughThickness;
        }

        /// <summary>
        /// Gets the font name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
        readonly string _name;

        /// <summary>
        /// Gets the ascent value.
        /// </summary>
        public int UnitsPerEm
        {
            get { return _unitsPerEm; }
        }
        readonly int _unitsPerEm;

        /// <summary>
        /// Gets the ascent value.
        /// </summary>
        public int Ascent
        {
            get { return _ascent; }
        }
        readonly int _ascent;

        /// <summary>
        /// Gets the descent value.
        /// </summary>
        public int Descent
        {
            get { return _descent; }
        }
        readonly int _descent;

        /// <summary>
        /// Gets the average width.
        /// </summary>
        public int AverageWidth
        {
            get { return _averageWidth; }
        }
        readonly int _averageWidth;

        /// <summary>
        /// Gets the height of capital letters.
        /// </summary>
        public int CapHeight
        {
            get { return _capHeight; }
        }
        readonly int _capHeight;

        /// <summary>
        /// Gets the leading value.
        /// </summary>
        public int Leading
        {
            get { return _leading; }
        }
        readonly int _leading;

        /// <summary>
        /// Gets the line spacing value.
        /// </summary>
        public int LineSpacing
        {
            get { return _lineSpacing; }
        }
        readonly int _lineSpacing;

        /// <summary>
        /// Gets the maximum width of a character.
        /// </summary>
        public int MaxWidth
        {
            get { return _maxWidth; }
        }
        readonly int _maxWidth;

        /// <summary>
        /// Gets an internal value.
        /// </summary>
        public int StemH
        {
            get { return _stemH; }
        }
        readonly int _stemH;

        /// <summary>
        /// Gets an internal value.
        /// </summary>
        public int StemV
        {
            get { return _stemV; }
        }
        readonly int _stemV;

        /// <summary>
        /// Gets the height of a lower-case character.
        /// </summary>
        public int XHeight
        {
            get { return _xHeight; }
        }
        readonly int _xHeight;

        /// <summary>
        /// Gets the underline position.
        /// </summary>
        public int UnderlinePosition
        {
            get { return _underlinePosition; }
        }
        readonly int _underlinePosition;

        /// <summary>
        /// Gets the underline thicksness.
        /// </summary>
        public int UnderlineThickness
        {
            get { return _underlineThickness; }
        }
        readonly int _underlineThickness;

        /// <summary>
        /// Gets the strikethrough position.
        /// </summary>
        public int StrikethroughPosition
        {
            get { return _strikethroughPosition; }
        }
        readonly int _strikethroughPosition;

        /// <summary>
        /// Gets the strikethrough thicksness.
        /// </summary>
        public int StrikethroughThickness
        {
            get { return _strikethroughThickness; }
        }
        readonly int _strikethroughThickness;
    }
}
