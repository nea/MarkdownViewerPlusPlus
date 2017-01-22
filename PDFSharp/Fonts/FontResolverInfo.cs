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
using System.Globalization;
using PdfSharp.Drawing;
#if CORE
using System.Drawing;
#endif
#if GDI
using System.Drawing;
#endif
#if WPF
using System.Windows.Media;
#endif

namespace PdfSharp.Fonts
{
    // The English terms font, font family, typeface, glyph etc. are sometimes confusingly used.
    // Here a short clarification by Wikipedia.
    //
    // Wikipedia EN -> DE
    //     Font -> Schriftschnitt
    //     Computer font -> Font (Informationstechnik)
    //     Typeface (Font family) -> Schriftart / Schriftfamilie
    //     Glyph -> Glyphe 
    // 
    // It seems that typeface and font family are synonyms in english.
    // In WPF a family name is used as a term for a bunch of fonts that share the same
    // characteristics, like Univers or Times New Roman.
    // In WPF a fontface describes a request of a font of a particular font family, e.g.
    // Univers medium bold italic.
    // In WPF a glyph typeface is the result of requesting a typeface, i.e. a physical font
    // plus the information whether bold and/or italic should be simulated.
    // 
    // Wikipedia DE -> EN
    //     Schriftart -> Typeface
    //     Schriftschnitt -> Font
    //     Schriftfamilie -> ~   (means Font family)
    //     Schriftsippe -> Font superfamily
    //     Font -> Computer font
    // 
    // http://en.wikipedia.org/wiki/Font
    // http://en.wikipedia.org/wiki/Computer_font
    // http://en.wikipedia.org/wiki/Typeface
    // http://en.wikipedia.org/wiki/Glyph
    // http://en.wikipedia.org/wiki/Typographic_unit
    // 
    // FaceName: A unique and only internally used name of a glyph typeface. In other words the name of the font data that represents a specific font.
    // 
    // 

    /// <summary>
    /// Describes the physical font that must be used to render a particular XFont.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class FontResolverInfo
    {
        private const string KeyPrefix = "frik:";  // Font Resolver Info Key

        /// <summary>
        /// Initializes a new instance of the <see cref="FontResolverInfo"/> struct.
        /// </summary>
        /// <param name="faceName">The name that uniquely identifies the fontface.</param>
        public FontResolverInfo(string faceName) :
            this(faceName, false, false, 0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontResolverInfo"/> struct.
        /// </summary>
        /// <param name="faceName">The name that uniquely identifies the fontface.</param>
        /// <param name="mustSimulateBold">Set to <c>true</c> to simulate bold when rendered. Not implemented and must be false.</param>
        /// <param name="mustSimulateItalic">Set to <c>true</c> to simulate italic when rendered.</param>
        /// <param name="collectionNumber">Index of the font in a true type font collection.
        /// Not yet implemented and must be zero.
        /// </param>
        internal FontResolverInfo(string faceName, bool mustSimulateBold, bool mustSimulateItalic, int collectionNumber)
        {
            if (String.IsNullOrEmpty(faceName))
                throw new ArgumentNullException("faceName");
            if (collectionNumber != 0)
                throw new NotImplementedException("collectionNumber is not yet implemented and must be 0.");

            _faceName = faceName;
            _mustSimulateBold = mustSimulateBold;
            _mustSimulateItalic = mustSimulateItalic;
            _collectionNumber = collectionNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontResolverInfo"/> struct.
        /// </summary>
        /// <param name="faceName">The name that uniquely identifies the fontface.</param>
        /// <param name="mustSimulateBold">Set to <c>true</c> to simulate bold when rendered. Not implemented and must be false.</param>
        /// <param name="mustSimulateItalic">Set to <c>true</c> to simulate italic when rendered.</param>
        public FontResolverInfo(string faceName, bool mustSimulateBold, bool mustSimulateItalic)
            : this(faceName, mustSimulateBold, mustSimulateItalic, 0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontResolverInfo" /> struct.
        /// </summary>
        /// <param name="faceName">The name that uniquely identifies the fontface.</param>
        /// <param name="styleSimulations">The style simulation flags.</param>
        public FontResolverInfo(string faceName, XStyleSimulations styleSimulations)
            : this(faceName,
                  (styleSimulations & XStyleSimulations.BoldSimulation) == XStyleSimulations.BoldSimulation,
                  (styleSimulations & XStyleSimulations.ItalicSimulation) == XStyleSimulations.ItalicSimulation, 0)
        { }

        /// <summary>
        /// Gets the key for this object.
        /// </summary>
        internal string Key
        {
            get
            {
                return _key ?? (_key = KeyPrefix + _faceName.ToLowerInvariant()
                                       + '/' + (_mustSimulateBold ? "b+" : "b-") + (_mustSimulateItalic ? "i+" : "i-"));
            }
        }
        string _key;

        /// <summary>
        /// A name that uniquely identifies the font (not the family), e.g. the file name of the font. PDFsharp does not use this
        /// name internally, but passes it to the GetFont function of the IFontResolver interface to retrieve the font data.
        /// </summary>
        public string FaceName
        {
            get { return _faceName; }
        }
        readonly string _faceName;

        /// <summary>
        /// Indicates whether bold must be simulated. Bold simulation is not implemented in PDFsharp.
        /// </summary>
        public bool MustSimulateBold
        {
            get { return _mustSimulateBold; }
        }
        readonly bool _mustSimulateBold;

        /// <summary>
        /// Indicates whether italic must be simulated.
        /// </summary>
        public bool MustSimulateItalic
        {
            get { return _mustSimulateItalic; }
        }
        readonly bool _mustSimulateItalic;

        /// <summary>
        /// Gets the style simulation flags.
        /// </summary>
        public XStyleSimulations StyleSimulations
        {
            get { return (_mustSimulateBold ? XStyleSimulations.BoldSimulation : 0) | (_mustSimulateItalic ? XStyleSimulations.ItalicSimulation : 0); }
        }

        /// <summary>
        /// The number of the font in a Truetype font collection file. The number of the first font is 0.
        /// NOT YET IMPLEMENTED. Must be zero.
        /// </summary>
        internal int CollectionNumber  // TODO : Find a better name.
        {
            get { return _collectionNumber; }
        }
        readonly int _collectionNumber;

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        internal string DebuggerDisplay
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "FontResolverInfo: '{0}',{1}{2}", FaceName,
                    MustSimulateBold ? " simulate Bold" : "",
                    MustSimulateItalic ? " simulate Italic" : "");
            }
        }
    }
}
