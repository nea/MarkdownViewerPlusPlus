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

#if GDI
using System.Drawing;
using System.Drawing.Text;
#endif
#if WPF
using System.Windows.Media;
#endif
using PdfSharp.Drawing;

namespace PdfSharp.Fonts
{
    /// <summary>
    /// Parameters that affect font selection.
    /// </summary>
    class FontResolvingOptions
    {
        public FontResolvingOptions(XFontStyle fontStyle)
        {
            FontStyle = fontStyle;
        }

        public FontResolvingOptions(XFontStyle fontStyle, XStyleSimulations styleSimulations)
        {
            FontStyle = fontStyle;
            OverrideStyleSimulations = true;
            StyleSimulations = styleSimulations;
        }

        public bool IsBold
        {
            get { return (FontStyle & XFontStyle.Bold) == XFontStyle.Bold; }
        }

        public bool IsItalic
        {
            get { return (FontStyle & XFontStyle.Italic) == XFontStyle.Italic; }
        }

        public bool IsBoldItalic
        {
            get { return (FontStyle & XFontStyle.BoldItalic) == XFontStyle.BoldItalic; }
        }

        public bool MustSimulateBold
        {
            get { return (StyleSimulations & XStyleSimulations.BoldSimulation) == XStyleSimulations.BoldSimulation; }
        }

        public bool MustSimulateItalic
        {
            get { return (StyleSimulations & XStyleSimulations.ItalicSimulation) == XStyleSimulations.ItalicSimulation; }
        }

        public XFontStyle FontStyle;

        public bool OverrideStyleSimulations;

        public XStyleSimulations StyleSimulations;
    }
}
