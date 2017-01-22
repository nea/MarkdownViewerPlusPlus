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
#if GDI
using System.Drawing;
#endif
#if WPF
using System.Windows.Media;
#endif
using PdfSharp.Drawing;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents a PDF rectangle value, that is internally an array with 4 real values.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public sealed class PdfRectangle : PdfItem
    {
        // This class must behave like a value type. Therefore it cannot be changed (like System.String).

        /// <summary>
        /// Initializes a new instance of the PdfRectangle class.
        /// </summary>
        public PdfRectangle()
        { }

        /// <summary>
        /// Initializes a new instance of the PdfRectangle class with two points specifying
        /// two diagonally opposite corners. Notice that in contrast to GDI+ convention the 
        /// 3rd and the 4th parameter specify a point and not a width. This is so much confusing
        /// that this function is for internal use only.
        /// </summary>
        internal PdfRectangle(double x1, double y1, double x2, double y2)
        {
            _x1 = x1;
            _y1 = y1;
            _x2 = x2;
            _y2 = y2;
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the PdfRectangle class with two points specifying
        /// two diagonally opposite corners.
        /// </summary>
        public PdfRectangle(PointF pt1, PointF pt2)
        {
            _x1 = pt1.X;
            _y1 = pt1.Y;
            _x2 = pt2.X;
            _y2 = pt2.Y;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the PdfRectangle class with two points specifying
        /// two diagonally opposite corners.
        /// </summary>
        public PdfRectangle(XPoint pt1, XPoint pt2)
        {
            _x1 = pt1.X;
            _y1 = pt1.Y;
            _x2 = pt2.X;
            _y2 = pt2.Y;
        }

#if GDI
        /// <summary>
        /// Initializes a new instance of the PdfRectangle class with the specified location and size.
        /// </summary>
        public PdfRectangle(PointF pt, SizeF size)
        {
            _x1 = pt.X;
            _y1 = pt.Y;
            _x2 = pt.X + size.Width;
            _y2 = pt.Y + size.Height;
        }
#endif

        /// <summary>
        /// Initializes a new instance of the PdfRectangle class with the specified location and size.
        /// </summary>
        public PdfRectangle(XPoint pt, XSize size)
        {
            _x1 = pt.X;
            _y1 = pt.Y;
            _x2 = pt.X + size.Width;
            _y2 = pt.Y + size.Height;
        }

        /// <summary>
        /// Initializes a new instance of the PdfRectangle class with the specified XRect.
        /// </summary>
        public PdfRectangle(XRect rect)
        {
            _x1 = rect.X;
            _y1 = rect.Y;
            _x2 = rect.X + rect.Width;
            _y2 = rect.Y + rect.Height;
        }

        /// <summary>
        /// Initializes a new instance of the PdfRectangle class with the specified PdfArray.
        /// </summary>
        internal PdfRectangle(PdfItem item)
        {
            if (item == null || item is PdfNull)
                return;

            if (item is PdfReference)
                item = ((PdfReference)item).Value;

            PdfArray array = item as PdfArray;
            if (array == null)
                throw new InvalidOperationException(PSSR.UnexpectedTokenInPdfFile);

            _x1 = array.Elements.GetReal(0);
            _y1 = array.Elements.GetReal(1);
            _x2 = array.Elements.GetReal(2);
            _y2 = array.Elements.GetReal(3);
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        public new PdfRectangle Clone()
        {
            return (PdfRectangle)Copy();
        }

        /// <summary>
        /// Implements cloning this instance.
        /// </summary>
        protected override object Copy()
        {
            PdfRectangle rect = (PdfRectangle)base.Copy();
            return rect;
        }

        /// <summary>
        /// Tests whether all coordinate are zero.
        /// </summary>
        public bool IsEmpty
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            get { return _x1 == 0 && _y1 == 0 && _x2 == 0 && _y2 == 0; }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Tests whether the specified object is a PdfRectangle and has equal coordinates.
        /// </summary>
        public override bool Equals(object obj)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            PdfRectangle rectangle = obj as PdfRectangle;
            if (rectangle != null)
            {
                PdfRectangle rect = rectangle;
                return rect._x1 == _x1 && rect._y1 == _y1 && rect._x2 == _x2 && rect._y2 == _y2;
            }
            return false;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        public override int GetHashCode()
        {
            // This code is from System.Drawing...
            return (int)(((((uint)_x1) ^ ((((uint)_y1) << 13) |
              (((uint)_y1) >> 0x13))) ^ ((((uint)_x2) << 0x1a) |
              (((uint)_x2) >> 6))) ^ ((((uint)_y2) << 7) |
              (((uint)_y2) >> 0x19)));
        }

        /// <summary>
        /// Tests whether two structures have equal coordinates.
        /// </summary>
        public static bool operator ==(PdfRectangle left, PdfRectangle right)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            // use: if (Object.ReferenceEquals(left, null))
            if ((object)left != null)
            {
                if ((object)right != null)
                    return left._x1 == right._x1 && left._y1 == right._y1 && left._x2 == right._x2 && left._y2 == right._y2;
                return false;
            }
            return (object)right == null;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Tests whether two structures differ in one or more coordinates.
        /// </summary>
        public static bool operator !=(PdfRectangle left, PdfRectangle right)
        {
            return !(left == right);
        }

        /// <summary>
        /// Gets or sets the x-coordinate of the first corner of this PdfRectangle.
        /// </summary>
        public double X1
        {
            get { return _x1; }
        }
        readonly double _x1;

        /// <summary>
        /// Gets or sets the y-coordinate of the first corner of this PdfRectangle.
        /// </summary>
        public double Y1
        {
            get { return _y1; }
        }
        readonly double _y1;

        /// <summary>
        /// Gets or sets the x-coordinate of the second corner of this PdfRectangle.
        /// </summary>
        public double X2
        {
            get { return _x2; }
        }
        readonly double _x2;

        /// <summary>
        /// Gets or sets the y-coordinate of the second corner of this PdfRectangle.
        /// </summary>
        public double Y2
        {
            get { return _y2; }
        }
        readonly double _y2;

        /// <summary>
        /// Gets X2 - X1.
        /// </summary>
        public double Width
        {
            get { return _x2 - _x1; }
        }

        /// <summary>
        /// Gets Y2 - Y1.
        /// </summary>
        public double Height
        {
            get { return _y2 - _y1; }
        }

        /// <summary>
        /// Gets or sets the coordinates of the first point of this PdfRectangle.
        /// </summary>
        public XPoint Location
        {
            get { return new XPoint(_x1, _y1); }
        }

        /// <summary>
        /// Gets or sets the size of this PdfRectangle.
        /// </summary>
        public XSize Size
        {
            get { return new XSize(_x2 - _x1, _y2 - _y1); }
        }

#if GDI
        /// <summary>
        /// Determines if the specified point is contained within this PdfRectangle.
        /// </summary>
        public bool Contains(PointF pt)
        {
            return Contains(pt.X, pt.Y);
        }
#endif

        /// <summary>
        /// Determines if the specified point is contained within this PdfRectangle.
        /// </summary>
        public bool Contains(XPoint pt)
        {
            return Contains(pt.X, pt.Y);
        }

        /// <summary>
        /// Determines if the specified point is contained within this PdfRectangle.
        /// </summary>
        public bool Contains(double x, double y)
        {
            // Treat rectangle inclusive/inclusive.
            return _x1 <= x && x <= _x2 && _y1 <= y && y <= _y2;
        }

#if GDI
        /// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this PdfRectangle.
        /// </summary>
        public bool Contains(RectangleF rect)
        {
            return _x1 <= rect.X && (rect.X + rect.Width) <= _x2 &&
              _y1 <= rect.Y && (rect.Y + rect.Height) <= _y2;
        }
#endif

        /// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this PdfRectangle.
        /// </summary>
        public bool Contains(XRect rect)
        {
            return _x1 <= rect.X && (rect.X + rect.Width) <= _x2 &&
              _y1 <= rect.Y && (rect.Y + rect.Height) <= _y2;
        }

        /// <summary>
        /// Determines if the rectangular region represented by rect is entirely contained within this PdfRectangle.
        /// </summary>
        public bool Contains(PdfRectangle rect)
        {
            return _x1 <= rect._x1 && rect._x2 <= _x2 &&
              _y1 <= rect._y1 && rect._y2 <= _y2;
        }

        /// <summary>
        /// Returns the rectangle as an XRect object.
        /// </summary>
        public XRect ToXRect()
        {
            return new XRect(_x1, _y1, Width, Height);
        }

        /// <summary>
        /// Returns the rectangle as a string in the form «[x1 y1 x2 y2]».
        /// </summary>
        public override string ToString()
        {
            const string format = Config.SignificantFigures3;
            return PdfEncoders.Format("[{0:" + format + "} {1:" + format + "} {2:" + format + "} {3:" + format + "}]", _x1, _y1, _x2, _y2);
        }

        /// <summary>
        /// Writes the rectangle.
        /// </summary>
        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
                const string format = Config.SignificantFigures10;
                return String.Format(CultureInfo.InvariantCulture,
                    "X1={0:" + format + "}, X2={1:" + format + "}, Y1={2:" + format + "}, Y2={3:" + format + "}", _x1, _y1, X2, _y2);
            }
        }

#if false // This object is considered as immutable.
    //    /// <summary>
    //    /// Adjusts the location of this PdfRectangle by the specified amount.
    //    /// </summary>
    //    public void Offset(PointF pos)
    //    {
    //      Offset(pos.X, pos.Y);
    //    }
    //
    //    /// <summary>
    //    /// Adjusts the location of this PdfRectangle by the specified amount.
    //    /// </summary>
    //    public void Offset(double x, double y)
    //    {
    //      _x1 += x;
    //      _y1 += y;
    //      _x2 += x;
    //      _y2 += y;
    //    }
    //
    //    /// <summary>
    //    /// Inflates this PdfRectangle by the specified amount.
    //    /// </summary>
    //    public void Inflate(double x, double y)
    //    {
    //      _x1 -= x;
    //      _y1 -= y;
    //      _x2 += x;
    //      _y2 += y;
    //    }
    //
    //    /// <summary>
    //    /// Inflates this PdfRectangle by the specified amount.
    //    /// </summary>
    //    public void Inflate(SizeF size)
    //    {
    //      Inflate(size.Width, size.Height);
    //    }
    //
    //    /// <summary>
    //    /// Creates and returns an inflated copy of the specified PdfRectangle.
    //    /// </summary>
    //    public static PdfRectangle Inflate(PdfRectangle rect, double x, double y)
    //    {
    //      rect.Inflate(x, y);
    //      return rect;
    //    }
    //
    //    /// <summary>
    //    /// Replaces this PdfRectangle with the intersection of itself and the specified PdfRectangle.
    //    /// </summary>
    //    public void Intersect(PdfRectangle rect)
    //    {
    //      PdfRectangle rect2 = PdfRectangle.Intersect(rect, this);
    //      _x1 = rect2.x1;
    //      _y1 = rect2.y1;
    //      _x2 = rect2.x2;
    //      _y2 = rect2.y2;
    //    }
    //
    //    /// <summary>
    //    /// Returns a PdfRectangle that represents the intersection of two rectangles. If there is no intersection,
    //    /// an empty PdfRectangle is returned.
    //    /// </summary>
    //    public static PdfRectangle Intersect(PdfRectangle rect1, PdfRectangle rect2)
    //    {
    //      double xx1 = Math.Max(rect1.x1, rect2.x1);
    //      double xx2 = Math.Min(rect1.x2, rect2.x2);
    //      double yy1 = Math.Max(rect1.y1, rect2.y1);
    //      double yy2 = Math.Min(rect1.y2, rect2.y2);
    //      if (xx2 >= xx1 && yy2 >= yy1)
    //        return new PdfRectangle(xx1, yy1, xx2, yy2);
    //      return PdfRectangle.Empty;
    //    }
    //
    //    /// <summary>
    //    /// Determines if this rectangle intersects with the specified PdfRectangle.
    //    /// </summary>
    //    public bool IntersectsWith(PdfRectangle rect)
    //    {
    //      return rect.x1 < _x2 && _x1 < rect.x2 && rect.y1 < _y2 && _y1 < rect.y2;
    //    }
    //
    //    /// <summary>
    //    /// Creates the smallest rectangle that can contain both of two specified rectangles.
    //    /// </summary>
    //    public static PdfRectangle Union(PdfRectangle rect1, PdfRectangle rect2)
    //    {
    //      return new PdfRectangle(
    //        Math.Min(rect1.x1, rect2.x1), Math.Max(rect1.x2, rect2.x2),
    //        Math.Min(rect1.y1, rect2.y1), Math.Max(rect1.y2, rect2.y2));
    //    }
#endif

        /// <summary>
        /// Represents an empty PdfRectangle.
        /// </summary>
        public static readonly PdfRectangle Empty = new PdfRectangle();
    }
}
