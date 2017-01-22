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
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows.Media;
#endif

namespace PdfSharp.Drawing
{
#if true_ // unused
    /// <summary>
    /// Represents a segment of a path defined by a type and a set of points.
    /// </summary>
    internal sealed class XGraphicsPathItem
    {
        public XGraphicsPathItem(XGraphicsPathItemType type)
        {
            Type = type;
            Points = null;
        }

#if GDI
        public XGraphicsPathItem(XGraphicsPathItemType type, params PointF[] points)
        {
            Type = type;
            Points = XGraphics.MakeXPointArray(points, 0, points.Length);
        }
#endif

        public XGraphicsPathItem(XGraphicsPathItemType type, params XPoint[] points)
        {
            Type = type;
            Points = (XPoint[])points.Clone();
        }

        public XGraphicsPathItem Clone()
        {
            XGraphicsPathItem item = (XGraphicsPathItem)MemberwiseClone();
            item.Points = (XPoint[])Points.Clone();
            return item;
        }

        public XGraphicsPathItemType Type;
        public XPoint[] Points;
    }
#endif
}