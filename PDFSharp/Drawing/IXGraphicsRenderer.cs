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
    /// <summary>
    /// Represents an abstract drawing surface for PdfPages.
    /// </summary>
    internal interface IXGraphicsRenderer
    {
        void Close();

        #region Drawing

        ///// <summary>
        ///// Fills the entire drawing surface with the specified color.
        ///// </summary>
        //[Obsolete("Will be removed.")]
        //void Clear(XColor color);

        /// <summary>
        /// Draws a straight line.
        /// </summary>
        void DrawLine(XPen pen, double x1, double y1, double x2, double y2);

        /// <summary>
        /// Draws a series of straight lines.
        /// </summary>
        void DrawLines(XPen pen, XPoint[] points);

        /// <summary>
        /// Draws a Bézier spline.
        /// </summary>
        void DrawBezier(XPen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4);

        /// <summary>
        /// Draws a series of Bézier splines.
        /// </summary>
        void DrawBeziers(XPen pen, XPoint[] points);

        /// <summary>
        /// Draws a cardinal spline.
        /// </summary>
        void DrawCurve(XPen pen, XPoint[] points, double tension);

        /// <summary>
        /// Draws an arc.
        /// </summary>
        void DrawArc(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle);

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        void DrawRectangle(XPen pen, XBrush brush, double x, double y, double width, double height);

        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        void DrawRectangles(XPen pen, XBrush brush, XRect[] rects);

        /// <summary>
        /// Draws a rectangle with rounded corners.
        /// </summary>
        void DrawRoundedRectangle(XPen pen, XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight);

        /// <summary>
        /// Draws an ellipse.
        /// </summary>
        void DrawEllipse(XPen pen, XBrush brush, double x, double y, double width, double height);

        /// <summary>
        /// Draws a polygon.
        /// </summary>
        void DrawPolygon(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode);

        /// <summary>
        /// Draws a pie.
        /// </summary>
        void DrawPie(XPen pen, XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle);

        /// <summary>
        /// Draws a cardinal spline.
        /// </summary>
        void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, double tension, XFillMode fillmode);

        /// <summary>
        /// Draws a graphical path.
        /// </summary>
        void DrawPath(XPen pen, XBrush brush, XGraphicsPath path);

        /// <summary>
        /// Draws a series of glyphs identified by the specified text and font.
        /// </summary>
        void DrawString(string s, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format);

        /// <summary>
        /// Draws an image.
        /// </summary>
        void DrawImage(XImage image, double x, double y, double width, double height);
        void DrawImage(XImage image, XRect destRect, XRect srcRect, XGraphicsUnit srcUnit);

        #endregion

        #region Save and Restore

        /// <summary>
        /// Saves the current graphics state without changing it.
        /// </summary>
        void Save(XGraphicsState state);

        /// <summary>
        /// Restores the specified graphics state.
        /// </summary>
        void Restore(XGraphicsState state);

        /// <summary>
        /// 
        /// </summary>
        void BeginContainer(XGraphicsContainer container, XRect dstrect, XRect srcrect, XGraphicsUnit unit);

        /// <summary>
        /// 
        /// </summary>
        void EndContainer(XGraphicsContainer container);

        #endregion

        #region Transformation

        /// <summary>
        /// Gets or sets the transformation matrix.
        /// </summary>
        //XMatrix Transform {get; set;}

        void AddTransform(XMatrix transform, XMatrixOrder matrixOrder);

        #endregion

        #region Clipping

        void SetClip(XGraphicsPath path, XCombineMode combineMode);

        void ResetClip();

        #endregion

        #region Miscellaneous

        /// <summary>
        /// Writes a comment to the output stream. Comments have no effect on the rendering of the output.
        /// </summary>
        void WriteComment(string comment);

        #endregion
    }
}
