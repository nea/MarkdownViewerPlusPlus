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
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows.Media;
#endif
#if UWP
using Microsoft.Graphics.Canvas.Brushes;
using UwpColor = Windows.UI.Color;
#endif

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Classes derived from this abstract base class define objects used to fill the 
    /// interiors of paths.
    /// </summary>
    public abstract class XBrush
    {
#if GDI
        internal abstract System.Drawing.Brush RealizeGdiBrush();

#if UseGdiObjects
        /// <summary>
        /// Converts from a System.Drawing.Brush.
        /// </summary>
        public static implicit operator XBrush(Brush brush)
        {
            XBrush xbrush;
            SolidBrush solidBrush;
            LinearGradientBrush lgBrush;
            if ((solidBrush = brush as SolidBrush) != null)
            {
                xbrush = new XSolidBrush(solidBrush.Color);
            }
            else if ((lgBrush = brush as LinearGradientBrush) != null)
            {
                // TODO: xbrush = new LinearGradientBrush(lgBrush.Rectangle, lgBrush.co(solidBrush.Color);
                throw new NotImplementedException("Brush type not yet supported by PDFsharp.");
            }
            else
            {
                throw new NotImplementedException("Brush type not supported by PDFsharp.");
            }
            return xbrush;
        }
#endif
#endif
#if WPF
        internal abstract System.Windows.Media.Brush RealizeWpfBrush();
#endif
#if UWP
        internal abstract ICanvasBrush RealizeCanvasBrush();
#endif
    }
}
