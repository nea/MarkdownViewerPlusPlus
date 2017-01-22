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
using PdfSharp.Internal;
using PdfSharp.Pdf;

namespace PdfSharp.Fonts
{
    /// <summary>
    /// Provides functionality to specify information about the handling of fonts in the current application domain.
    /// </summary>
    public static class GlobalFontSettings
    {
        /// <summary>
        /// The name of the default font.
        /// </summary>
        public const string DefaultFontName = "PlatformDefault";

        /// <summary>
        /// Gets or sets the global font resolver for the current application domain.
        /// This static function must be called only once and before any font operation was executed by PDFsharp.
        /// If this is not easily to obtain, e.g. because your code is running on a web server, you must provide the
        /// same instance of your font resolver in every subsequent setting of this property.
        /// In a web application set the font resolver in Global.asax.
        /// </summary>
        public static IFontResolver FontResolver
        {
            get { return _fontResolver; }
            set
            {
                // Cannot remove font resolver.
                if (value == null)
                    throw new ArgumentNullException();

                try
                {
                    Lock.EnterFontFactory();
                    // Ignore multiple setting e.g. in a web application.
                    if (ReferenceEquals(_fontResolver, value))
                        return;

                    if (FontFactory.HasFontSources)
                        throw new InvalidOperationException("Must not change font resolver after is was once used.");

                    _fontResolver = value;
                }
                finally { Lock.ExitFontFactory(); }
            }
        }
        static IFontResolver _fontResolver;

        /// <summary>
        /// Gets or sets the default font encoding used for XFont objects where encoding is not explicitly specified.
        /// If it is not set, the default value is PdfFontEncoding.Unicode.
        /// If you are sure your document contains only Windows-1252 characters (see https://en.wikipedia.org/wiki/Windows-1252) 
        /// set default encoding to PdfFontEncodingj.Windows1252.
        /// Must be set only once per app domain.
        /// </summary>
        public static PdfFontEncoding DefaultFontEncoding
        {
            get
            {
                if (!_fontEncodingInitialized)
                    DefaultFontEncoding = PdfFontEncoding.Unicode;
                return _fontEncoding;
            }
            set
            {
                try
                {
                    Lock.EnterFontFactory();
                    if (_fontEncodingInitialized)
                    {
                        // Ignore multiple setting e.g. in a web application.
                        if (_fontEncoding == value)
                            return;
                        throw new InvalidOperationException("Must not change DefaultFontEncoding after is was set once.");
                    }

                    _fontEncoding = value;
                    _fontEncodingInitialized = true;
                }
                finally { Lock.ExitFontFactory(); }
            }
        }
        static PdfFontEncoding _fontEncoding;
        static bool _fontEncodingInitialized;
    }
}