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
using System.Runtime.InteropServices;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
#endif
#if WPF
using System.Windows;
using System.Windows.Media;
#endif
#if !EDF_CORE
using PdfSharp.Internal;
#else
using PdfSharp.Internal;
#endif

// ReSharper disable RedundantNameQualifier
#if !EDF_CORE
namespace PdfSharp.Drawing
#else
namespace Edf.Drawing
#endif
{
    /// <summary>
    /// Represents a 3-by-3 matrix that represents an affine 2D transformation.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    [Serializable, StructLayout(LayoutKind.Sequential)] //, TypeConverter(typeof(MatrixConverter)), ValueSerializer(typeof(MatrixValueSerializer))]
    public struct XMatrix : IFormattable
    {
        [Flags]
        internal enum XMatrixTypes
        {
            Identity = 0,
            Translation = 1,
            Scaling = 2,
            Unknown = 4
        }

        /// <summary>
        /// Initializes a new instance of the XMatrix struct.
        /// </summary>
        public XMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            _m11 = m11;
            _m12 = m12;
            _m21 = m21;
            _m22 = m22;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _type = XMatrixTypes.Unknown;
            //_padding = 0;
            DeriveMatrixType();
        }

        /// <summary>
        /// Gets the identity matrix. 
        /// </summary>
        public static XMatrix Identity
        {
            get { return s_identity; }
        }

        /// <summary>
        /// Sets this matrix into an identity matrix.
        /// </summary>
        public void SetIdentity()
        {
            _type = XMatrixTypes.Identity;
        }

        /// <summary>
        /// Gets a value indicating whether this matrix instance is the identity matrix.
        /// </summary>
        public bool IsIdentity
        {
            get
            {
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (_type == XMatrixTypes.Identity)
                    return true;
                if (_m11 == 1.0 && _m12 == 0 && _m21 == 0 && _m22 == 1.0 && _offsetX == 0 && _offsetY == 0)
                {
                    _type = XMatrixTypes.Identity;
                    return true;
                }
                return false;
                // ReSharper restore CompareOfFloatsByEqualityOperator
            }
        }

        ///// <summary>
        ///// Gets an array of double values that represents the elements of this matrix.
        ///// </summary>
        //[Obsolete("Use GetElements().")]
        //public double[] Elements
        //{
        //  get { return GetElements(); }
        //}

        /// <summary>
        /// Gets an array of double values that represents the elements of this matrix.
        /// </summary>
        public double[] GetElements()
        {
            if (_type == XMatrixTypes.Identity)
                return new double[] { 1, 0, 0, 1, 0, 0 };
            return new double[] { _m11, _m12, _m21, _m22, _offsetX, _offsetY };
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        public static XMatrix operator *(XMatrix trans1, XMatrix trans2)
        {
            MatrixHelper.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        public static XMatrix Multiply(XMatrix trans1, XMatrix trans2)
        {
            MatrixHelper.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        /// <summary>
        /// Appends the specified matrix to this matrix. 
        /// </summary>
        public void Append(XMatrix matrix)
        {
            this *= matrix;
        }

        /// <summary>
        /// Prepends the specified matrix to this matrix. 
        /// </summary>
        public void Prepend(XMatrix matrix)
        {
            this = matrix * this;
        }

        /// <summary>
        /// Appends the specified matrix to this matrix. 
        /// </summary>
        [Obsolete("Use Append.")]
        public void Multiply(XMatrix matrix)
        {
            Append(matrix);
        }

        /// <summary>
        /// Prepends the specified matrix to this matrix. 
        /// </summary>
        [Obsolete("Use Prepend.")]
        public void MultiplyPrepend(XMatrix matrix)
        {
            Prepend(matrix);
        }

        /// <summary>
        /// Multiplies this matrix with the specified matrix.
        /// </summary>
        public void Multiply(XMatrix matrix, XMatrixOrder order)
        {
            if (_type == XMatrixTypes.Identity)
                this = CreateIdentity();

            // Must use properties, the fields can be invalid if the matrix is identity matrix.
            double t11 = M11;
            double t12 = M12;
            double t21 = M21;
            double t22 = M22;
            double tdx = OffsetX;
            double tdy = OffsetY;

            if (order == XMatrixOrder.Append)
            {
                _m11 = t11 * matrix.M11 + t12 * matrix.M21;
                _m12 = t11 * matrix.M12 + t12 * matrix.M22;
                _m21 = t21 * matrix.M11 + t22 * matrix.M21;
                _m22 = t21 * matrix.M12 + t22 * matrix.M22;
                _offsetX = tdx * matrix.M11 + tdy * matrix.M21 + matrix.OffsetX;
                _offsetY = tdx * matrix.M12 + tdy * matrix.M22 + matrix.OffsetY;
            }
            else
            {
                _m11 = t11 * matrix.M11 + t21 * matrix.M12;
                _m12 = t12 * matrix.M11 + t22 * matrix.M12;
                _m21 = t11 * matrix.M21 + t21 * matrix.M22;
                _m22 = t12 * matrix.M21 + t22 * matrix.M22;
                _offsetX = t11 * matrix.OffsetX + t21 * matrix.OffsetY + tdx;
                _offsetY = t12 * matrix.OffsetX + t22 * matrix.OffsetY + tdy;
            }
            DeriveMatrixType();
        }

        /// <summary>
        /// Appends a translation of the specified offsets to this matrix.
        /// </summary>
        [Obsolete("Use TranslateAppend or TranslatePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Translate(double offsetX, double offsetY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //if (_type == XMatrixTypes.Identity)
            //{
            //  SetMatrix(1.0, 0, 0, 1.0, offsetX, offsetY, XMatrixTypes.Translation);
            //}
            //else if (_type == XMatrixTypes.Unknown)
            //{
            //  _offsetX += offsetX;
            //  _offsetY += offsetY;
            //}
            //else
            //{
            //  _offsetX += offsetX;
            //  _offsetY += offsetY;
            //  _type |= XMatrixTypes.Translation;
            //}
        }

        /// <summary>
        /// Appends a translation of the specified offsets to this matrix.
        /// </summary>
        public void TranslateAppend(double offsetX, double offsetY) // TODO: will become default
        {
            if (_type == XMatrixTypes.Identity)
            {
                SetMatrix(1, 0, 0, 1, offsetX, offsetY, XMatrixTypes.Translation);
            }
            else if (_type == XMatrixTypes.Unknown)
            {
                _offsetX += offsetX;
                _offsetY += offsetY;
            }
            else
            {
                _offsetX += offsetX;
                _offsetY += offsetY;
                _type |= XMatrixTypes.Translation;
            }
        }

        /// <summary>
        /// Prepends a translation of the specified offsets to this matrix.
        /// </summary>
        public void TranslatePrepend(double offsetX, double offsetY)
        {
            this = CreateTranslation(offsetX, offsetY) * this;
        }

        /// <summary>
        /// Translates the matrix with the specified offsets.
        /// </summary>
        public void Translate(double offsetX, double offsetY, XMatrixOrder order)
        {
            if (_type == XMatrixTypes.Identity)
                this = CreateIdentity();

            if (order == XMatrixOrder.Append)
            {
                _offsetX += offsetX;
                _offsetY += offsetY;
            }
            else
            {
                _offsetX += offsetX * _m11 + offsetY * _m21;
                _offsetY += offsetX * _m12 + offsetY * _m22;
            }
            DeriveMatrixType();
        }

        /// <summary>
        /// Appends the specified scale vector to this matrix.
        /// </summary>
        [Obsolete("Use ScaleAppend or ScalePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Scale(double scaleX, double scaleY)
        {
            this = CreateScaling(scaleX, scaleY) * this;
        }

        /// <summary>
        /// Appends the specified scale vector to this matrix.
        /// </summary>
        public void ScaleAppend(double scaleX, double scaleY)  // TODO: will become default
        {
            this *= CreateScaling(scaleX, scaleY);
        }

        /// <summary>
        /// Prepends the specified scale vector to this matrix.
        /// </summary>
        public void ScalePrepend(double scaleX, double scaleY)
        {
            this = CreateScaling(scaleX, scaleY) * this;
        }

        /// <summary>
        /// Scales the matrix with the specified scalars.
        /// </summary>
        public void Scale(double scaleX, double scaleY, XMatrixOrder order)
        {
            if (_type == XMatrixTypes.Identity)
                this = CreateIdentity();

            if (order == XMatrixOrder.Append)
            {
                _m11 *= scaleX;
                _m12 *= scaleY;
                _m21 *= scaleX;
                _m22 *= scaleY;
                _offsetX *= scaleX;
                _offsetY *= scaleY;
            }
            else
            {
                _m11 *= scaleX;
                _m12 *= scaleX;
                _m21 *= scaleY;
                _m22 *= scaleY;
            }
            DeriveMatrixType();
        }

        /// <summary>
        /// Scales the matrix with the specified scalar.
        /// </summary>
        [Obsolete("Use ScaleAppend or ScalePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        // ReSharper disable InconsistentNaming
        public void Scale(double scaleXY)
        // ReSharper restore InconsistentNaming
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //Scale(scaleXY, scaleXY, XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Appends the specified scale vector to this matrix.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public void ScaleAppend(double scaleXY)
        // ReSharper restore InconsistentNaming
        {
            Scale(scaleXY, scaleXY, XMatrixOrder.Append);
        }

        /// <summary>
        /// Prepends the specified scale vector to this matrix.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public void ScalePrepend(double scaleXY)
        // ReSharper restore InconsistentNaming
        {
            Scale(scaleXY, scaleXY, XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Scales the matrix with the specified scalar.
        /// </summary>
        // ReSharper disable InconsistentNaming
        public void Scale(double scaleXY, XMatrixOrder order)
        // ReSharper restore InconsistentNaming
        {
            Scale(scaleXY, scaleXY, order);
        }

        /// <summary>
        /// Function is obsolete.
        /// </summary>
        [Obsolete("Use ScaleAtAppend or ScaleAtPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //this *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }

        /// <summary>
        /// Apppends the specified scale about the specified point of this matrix.
        /// </summary>
        public void ScaleAtAppend(double scaleX, double scaleY, double centerX, double centerY) // TODO: will become default
        {
            this *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }

        /// <summary>
        /// Prepends the specified scale about the specified point of this matrix.
        /// </summary>
        public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
        {
            this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;
        }

        /// <summary>
        /// Function is obsolete.
        /// </summary>
        [Obsolete("Use RotateAppend or RotatePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Rotate(double angle)
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //angle = angle % 360.0;
            //this *= CreateRotationRadians(angle * Const.Deg2Rad);
        }

        /// <summary>
        /// Appends a rotation of the specified angle to this matrix.
        /// </summary>
        public void RotateAppend(double angle) // TODO: will become default Rotate
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * Const.Deg2Rad);
        }

        /// <summary>
        /// Prepends a rotation of the specified angle to this matrix.
        /// </summary>
        public void RotatePrepend(double angle)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * Const.Deg2Rad) * this;
        }

        /// <summary>
        /// Rotates the matrix with the specified angle.
        /// </summary>
        public void Rotate(double angle, XMatrixOrder order)
        {
            if (_type == XMatrixTypes.Identity)
                this = CreateIdentity();

            angle = angle * Const.Deg2Rad;
            double cos = Math.Cos(angle);
            double sin = Math.Sin(angle);
            if (order == XMatrixOrder.Append)
            {
                double t11 = _m11;
                double t12 = _m12;
                double t21 = _m21;
                double t22 = _m22;
                double tdx = _offsetX;
                double tdy = _offsetY;
                _m11 = t11 * cos - t12 * sin;
                _m12 = t11 * sin + t12 * cos;
                _m21 = t21 * cos - t22 * sin;
                _m22 = t21 * sin + t22 * cos;
                _offsetX = tdx * cos - tdy * sin;
                _offsetY = tdx * sin + tdy * cos;
            }
            else
            {
                double t11 = _m11;
                double t12 = _m12;
                double t21 = _m21;
                double t22 = _m22;
                _m11 = t11 * cos + t21 * sin;
                _m12 = t12 * cos + t22 * sin;
                _m21 = -t11 * sin + t21 * cos;
                _m22 = -t12 * sin + t22 * cos;
            }
            DeriveMatrixType();
        }

        /// <summary>
        /// Function is obsolete.
        /// </summary>
        [Obsolete("Use RotateAtAppend or RotateAtPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void RotateAt(double angle, double centerX, double centerY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //angle = angle % 360.0;
            //this *= CreateRotationRadians(angle * Const.Deg2Rad, centerX, centerY);
        }

        /// <summary>
        /// Appends a rotation of the specified angle at the specified point to this matrix.
        /// </summary>
        public void RotateAtAppend(double angle, double centerX, double centerY)  // TODO: will become default
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * Const.Deg2Rad, centerX, centerY);
        }

        /// <summary>
        /// Prepends a rotation of the specified angle at the specified point to this matrix.
        /// </summary>
        public void RotateAtPrepend(double angle, double centerX, double centerY)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * Const.Deg2Rad, centerX, centerY) * this;
        }

        /// <summary>
        /// Rotates the matrix with the specified angle at the specified point.
        /// </summary>
        [Obsolete("Use RotateAtAppend or RotateAtPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void RotateAt(double angle, XPoint point)
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //RotateAt(angle, point, XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Appends a rotation of the specified angle at the specified point to this matrix.
        /// </summary>
        public void RotateAtAppend(double angle, XPoint point)
        {
            RotateAt(angle, point, XMatrixOrder.Append);
        }

        /// <summary>
        /// Prepends a rotation of the specified angle at the specified point to this matrix.
        /// </summary>
        public void RotateAtPrepend(double angle, XPoint point)
        {
            RotateAt(angle, point, XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Rotates the matrix with the specified angle at the specified point.
        /// </summary>
        public void RotateAt(double angle, XPoint point, XMatrixOrder order)
        {
            if (order == XMatrixOrder.Append)
            {
                angle = angle % 360.0;
                this *= CreateRotationRadians(angle * Const.Deg2Rad, point.X, point.Y);

                //Translate(point.X, point.Y, order);
                //Rotate(angle, order);
                //Translate(-point.X, -point.Y, order);
            }
            else
            {
                angle = angle % 360.0;
                this = CreateRotationRadians(angle * Const.Deg2Rad, point.X, point.Y) * this;
            }
            DeriveMatrixType();
        }

        /// <summary>
        /// Function is obsolete.
        /// </summary>
        [Obsolete("Use ShearAppend or ShearPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Shear(double shearX, double shearY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //Shear(shearX, shearY, XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Appends a skew of the specified degrees in the x and y dimensions to this matrix.
        /// </summary>
        public void ShearAppend(double shearX, double shearY) // TODO: will become default
        {
            Shear(shearX, shearY, XMatrixOrder.Append);
        }

        /// <summary>
        /// Prepends a skew of the specified degrees in the x and y dimensions to this matrix.
        /// </summary>
        public void ShearPrepend(double shearX, double shearY)
        {
            Shear(shearX, shearY, XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Shears the matrix with the specified scalars.
        /// </summary>
        public void Shear(double shearX, double shearY, XMatrixOrder order)
        {
            if (_type == XMatrixTypes.Identity)
                this = CreateIdentity();

            double t11 = _m11;
            double t12 = _m12;
            double t21 = _m21;
            double t22 = _m22;
            double tdx = _offsetX;
            double tdy = _offsetY;
            if (order == XMatrixOrder.Append)
            {
                _m11 += shearX * t12;
                _m12 += shearY * t11;
                _m21 += shearX * t22;
                _m22 += shearY * t21;
                _offsetX += shearX * tdy;
                _offsetY += shearY * tdx;
            }
            else
            {
                _m11 += shearY * t21;
                _m12 += shearY * t22;
                _m21 += shearX * t11;
                _m22 += shearX * t12;
            }
            DeriveMatrixType();
        }

        /// <summary>
        /// Function is obsolete.
        /// </summary>
        [Obsolete("Use SkewAppend or SkewPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Skew(double skewX, double skewY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
            //skewX = skewX % 360.0;
            //skewY = skewY % 360.0;
            //this *= CreateSkewRadians(skewX * Const.Deg2Rad, skewY * Const.Deg2Rad);
        }

        /// <summary>
        /// Appends a skew of the specified degrees in the x and y dimensions to this matrix.
        /// </summary>
        public void SkewAppend(double skewX, double skewY)
        {
            skewX = skewX % 360.0;
            skewY = skewY % 360.0;
            this *= CreateSkewRadians(skewX * Const.Deg2Rad, skewY * Const.Deg2Rad);
        }

        /// <summary>
        /// Prepends a skew of the specified degrees in the x and y dimensions to this matrix.
        /// </summary>
        public void SkewPrepend(double skewX, double skewY)
        {
            skewX = skewX % 360.0;
            skewY = skewY % 360.0;
            this = CreateSkewRadians(skewX * Const.Deg2Rad, skewY * Const.Deg2Rad) * this;
        }

        /// <summary>
        /// Transforms the specified point by this matrix and returns the result.
        /// </summary>
        public XPoint Transform(XPoint point)
        {
            double x = point.X;
            double y = point.Y;
            MultiplyPoint(ref x, ref y);
            return new XPoint(x, y);
        }

        /// <summary>
        /// Transforms the specified points by this matrix. 
        /// </summary>
        public void Transform(XPoint[] points)
        {
            if (points != null)
            {
                int count = points.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    double x = points[idx].X;
                    double y = points[idx].Y;
                    MultiplyPoint(ref x, ref y);
                    points[idx].X = x;
                    points[idx].Y = y;
                }
            }
        }

        /// <summary>
        /// Multiplies all points of the specified array with the this matrix.
        /// </summary>
        public void TransformPoints(XPoint[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (IsIdentity)
                return;

            int count = points.Length;
            for (int idx = 0; idx < count; idx++)
            {
                double x = points[idx].X;
                double y = points[idx].Y;
                points[idx].X = x * _m11 + y * _m21 + _offsetX;
                points[idx].Y = x * _m12 + y * _m22 + _offsetY;
            }
        }

#if GDI
        /// <summary>
        /// Multiplies all points of the specified array with the this matrix.
        /// </summary>
        public void TransformPoints(System.Drawing.Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (IsIdentity)
                return;

            int count = points.Length;
            for (int idx = 0; idx < count; idx++)
            {
                double x = points[idx].X;
                double y = points[idx].Y;
                points[idx].X = (int)(x * _m11 + y * _m21 + _offsetX);
                points[idx].Y = (int)(x * _m12 + y * _m22 + _offsetY);
            }
        }
#endif

#if WPF
        /// <summary>
        /// Transforms an array of points.
        /// </summary>
        public void TransformPoints(System.Windows.Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (IsIdentity)
                return;

            int count = points.Length;
            for (int idx = 0; idx < count; idx++)
            {
                double x = points[idx].X;
                double y = points[idx].Y;
                points[idx].X = (int)(x * _m11 + y * _m21 + _offsetX);
                points[idx].Y = (int)(x * _m12 + y * _m22 + _offsetY);
            }
        }
#endif

        /// <summary>
        /// Transforms the specified vector by this Matrix and returns the result.
        /// </summary>
        public XVector Transform(XVector vector)
        {
            double x = vector.X;
            double y = vector.Y;
            MultiplyVector(ref x, ref y);
            return new XVector(x, y);
        }

        /// <summary>
        /// Transforms the specified vectors by this matrix.
        /// </summary>
        public void Transform(XVector[] vectors)
        {
            if (vectors != null)
            {
                int count = vectors.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    double x = vectors[idx].X;
                    double y = vectors[idx].Y;
                    MultiplyVector(ref x, ref y);
                    vectors[idx].X = x;
                    vectors[idx].Y = y;
                }
            }
        }

#if GDI
        /// <summary>
        /// Multiplies all vectors of the specified array with the this matrix. The translation elements 
        /// of this matrix (third row) are ignored.
        /// </summary>
        public void TransformVectors(PointF[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (IsIdentity)
                return;

            int count = points.Length;
            for (int idx = 0; idx < count; idx++)
            {
                double x = points[idx].X;
                double y = points[idx].Y;
                points[idx].X = (float)(x * _m11 + y * _m21 + _offsetX);
                points[idx].Y = (float)(x * _m12 + y * _m22 + _offsetY);
            }
        }
#endif

        /// <summary>
        /// Gets the determinant of this matrix.
        /// </summary>
        public double Determinant
        {
            get
            {
                switch (_type)
                {
                    case XMatrixTypes.Identity:
                    case XMatrixTypes.Translation:
                        return 1.0;

                    case XMatrixTypes.Scaling:
                    case XMatrixTypes.Scaling | XMatrixTypes.Translation:
                        return _m11 * _m22;
                }
                return (_m11 * _m22) - (_m12 * _m21);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this matrix is invertible.
        /// </summary>
        public bool HasInverse
        {
            get { return !DoubleUtil.IsZero(Determinant); }
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        public void Invert()
        {
            double determinant = Determinant;
            if (DoubleUtil.IsZero(determinant))
                throw new InvalidOperationException("NotInvertible"); //SR.Get(SRID.Transform_NotInvertible, new object[0]));

            switch (_type)
            {
                case XMatrixTypes.Identity:
                    break;

                case XMatrixTypes.Translation:
                    _offsetX = -_offsetX;
                    _offsetY = -_offsetY;
                    return;

                case XMatrixTypes.Scaling:
                    _m11 = 1.0 / _m11;
                    _m22 = 1.0 / _m22;
                    return;

                case XMatrixTypes.Scaling | XMatrixTypes.Translation:
                    _m11 = 1.0 / _m11;
                    _m22 = 1.0 / _m22;
                    _offsetX = -_offsetX * _m11;
                    _offsetY = -_offsetY * _m22;
                    return;

                default:
                    {
                        double detInvers = 1.0 / determinant;
                        SetMatrix(_m22 * detInvers, -_m12 * detInvers, -_m21 * detInvers, _m11 * detInvers, (_m21 * _offsetY - _offsetX * _m22) * detInvers, (_offsetX * _m12 - _m11 * _offsetY) * detInvers, XMatrixTypes.Unknown);
                        break;
                    }
            }
        }

        /// <summary>
        /// Gets or sets the value of the first row and first column of this matrix.
        /// </summary>
        public double M11
        {
            get
            {
                if (_type == XMatrixTypes.Identity)
                    return 1.0;
                return _m11;
            }
            set
            {
                if (_type == XMatrixTypes.Identity)
                    SetMatrix(value, 0, 0, 1, 0, 0, XMatrixTypes.Scaling);
                else
                {
                    _m11 = value;
                    if (_type != XMatrixTypes.Unknown)
                        _type |= XMatrixTypes.Scaling;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the first row and second column of this matrix.
        /// </summary>
        public double M12
        {
            get
            {
                if (_type == XMatrixTypes.Identity)
                    return 0;
                return _m12;
            }
            set
            {
                if (_type == XMatrixTypes.Identity)
                    SetMatrix(1, value, 0, 1, 0, 0, XMatrixTypes.Unknown);
                else
                {
                    _m12 = value;
                    _type = XMatrixTypes.Unknown;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the second row and first column of this matrix.
        /// </summary>
        public double M21
        {
            get
            {
                if (_type == XMatrixTypes.Identity)
                    return 0;
                return _m21;
            }
            set
            {
                if (_type == XMatrixTypes.Identity)
                    SetMatrix(1, 0, value, 1, 0, 0, XMatrixTypes.Unknown);
                else
                {
                    _m21 = value;
                    _type = XMatrixTypes.Unknown;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the second row and second column of this matrix.
        /// </summary>
        public double M22
        {
            get
            {
                if (_type == XMatrixTypes.Identity)
                    return 1.0;
                return _m22;
            }
            set
            {
                if (_type == XMatrixTypes.Identity)
                    SetMatrix(1, 0, 0, value, 0, 0, XMatrixTypes.Scaling);
                else
                {
                    _m22 = value;
                    if (_type != XMatrixTypes.Unknown)
                        _type |= XMatrixTypes.Scaling;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the third row and first column of this matrix.
        /// </summary>
        public double OffsetX
        {
            get
            {
                if (_type == XMatrixTypes.Identity)
                    return 0;
                return _offsetX;
            }
            set
            {
                if (_type == XMatrixTypes.Identity)
                    SetMatrix(1, 0, 0, 1, value, 0, XMatrixTypes.Translation);
                else
                {
                    _offsetX = value;
                    if (_type != XMatrixTypes.Unknown)
                        _type |= XMatrixTypes.Translation;
                }
            }
        }

        /// <summary>
        /// Gets or sets the value of the third row and second  column of this matrix.
        /// </summary>
        public double OffsetY
        {
            get
            {
                if (_type == XMatrixTypes.Identity)
                    return 0;
                return _offsetY;
            }
            set
            {
                if (_type == XMatrixTypes.Identity)
                    SetMatrix(1, 0, 0, 1, 0, value, XMatrixTypes.Translation);
                else
                {
                    _offsetY = value;
                    if (_type != XMatrixTypes.Unknown)
                        _type |= XMatrixTypes.Translation;
                }
            }
        }

#if GDI
//#if UseGdiObjects
        /// <summary>
        /// Converts this matrix to a System.Drawing.Drawing2D.Matrix object.
        /// </summary>
        public System.Drawing.Drawing2D.Matrix ToGdiMatrix()
        {
            if (IsIdentity)
                return new System.Drawing.Drawing2D.Matrix();

            return new System.Drawing.Drawing2D.Matrix((float)_m11, (float)_m12, (float)_m21, (float)_m22,
              (float)_offsetX, (float)_offsetY);
        }
//#endif
#endif

#if WPF
        /// Converts this matrix to a System.Windows.Media.Matrix object.
        /// <summary>
        /// </summary>
        public System.Windows.Media.Matrix ToWpfMatrix()
        {
            return (System.Windows.Media.Matrix)this;
            //return new System.Windows.Media.Matrix(_m11, _m12, _m21, _m22, _offsetX, _offsetY);
        }
#endif

#if GDI
        /// <summary>
        /// Explicitly converts a XMatrix to a Matrix.
        /// </summary>
        public static explicit operator System.Drawing.Drawing2D.Matrix(XMatrix matrix)
        {
            if (matrix.IsIdentity)
                return new System.Drawing.Drawing2D.Matrix();

            return new System.Drawing.Drawing2D.Matrix(
              (float)matrix._m11, (float)matrix._m12,
              (float)matrix._m21, (float)matrix._m22,
              (float)matrix._offsetX, (float)matrix._offsetY);
        }
#endif

#if WPF
        /// <summary>
        /// Explicitly converts an XMatrix to a Matrix.
        /// </summary>
        public static explicit operator System.Windows.Media.Matrix(XMatrix matrix)
        {
            if (matrix.IsIdentity)
                return new System.Windows.Media.Matrix();

            return new System.Windows.Media.Matrix(
              matrix._m11, matrix._m12,
              matrix._m21, matrix._m22,
              matrix._offsetX, matrix._offsetY);
        }
#endif

#if GDI
        /// <summary>
        /// Implicitly converts a Matrix to an XMatrix.
        /// </summary>
        public static implicit operator XMatrix(System.Drawing.Drawing2D.Matrix matrix)
        {
            float[] elements = matrix.Elements;
            return new XMatrix(elements[0], elements[1], elements[2], elements[3], elements[4], elements[5]);
        }
#endif

#if WPF
        /// <summary>
        /// Implicitly converts a Matrix to an XMatrix.
        /// </summary>
        public static implicit operator XMatrix(System.Windows.Media.Matrix matrix)
        {
            return new XMatrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
        }
#endif

        /// <summary>
        /// Determines whether the two matrices are equal.
        /// </summary>
        public static bool operator ==(XMatrix matrix1, XMatrix matrix2)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
                return (matrix1.IsIdentity == matrix2.IsIdentity);

            return matrix1.M11 == matrix2.M11 && matrix1.M12 == matrix2.M12 && matrix1.M21 == matrix2.M21 && matrix1.M22 == matrix2.M22 &&
              matrix1.OffsetX == matrix2.OffsetX && matrix1.OffsetY == matrix2.OffsetY;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <summary>
        /// Determines whether the two matrices are not equal.
        /// </summary>
        public static bool operator !=(XMatrix matrix1, XMatrix matrix2)
        {
            return !(matrix1 == matrix2);
        }

        /// <summary>
        /// Determines whether the two matrices are equal.
        /// </summary>
        public static bool Equals(XMatrix matrix1, XMatrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
                return matrix1.IsIdentity == matrix2.IsIdentity;

            return matrix1.M11.Equals(matrix2.M11) && matrix1.M12.Equals(matrix2.M12) &&
              matrix1.M21.Equals(matrix2.M21) && matrix1.M22.Equals(matrix2.M22) &&
              matrix1.OffsetX.Equals(matrix2.OffsetX) && matrix1.OffsetY.Equals(matrix2.OffsetY);
        }

        /// <summary>
        /// Determines whether this matrix is equal to the specified object.
        /// </summary>
        public override bool Equals(object o)
        {
            if (!(o is XMatrix))
                return false;
            return Equals(this, (XMatrix)o);
        }

        /// <summary>
        /// Determines whether this matrix is equal to the specified matrix.
        /// </summary>
        public bool Equals(XMatrix value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            if (IsDistinguishedIdentity)
                return 0;
            return M11.GetHashCode() ^ M12.GetHashCode() ^ M21.GetHashCode() ^ M22.GetHashCode() ^ OffsetX.GetHashCode() ^ OffsetY.GetHashCode();
        }

        /// <summary>
        /// Parses a matrix from a string.
        /// </summary>
        public static XMatrix Parse(string source)
        {
            IFormatProvider cultureInfo = CultureInfo.InvariantCulture; //.GetCultureInfo("en-us");
            TokenizerHelper helper = new TokenizerHelper(source, cultureInfo);
            string str = helper.NextTokenRequired();
            XMatrix identity = str == "Identity" ? Identity : new XMatrix(
                Convert.ToDouble(str, cultureInfo),
                Convert.ToDouble(helper.NextTokenRequired(), cultureInfo),
                Convert.ToDouble(helper.NextTokenRequired(), cultureInfo),
                Convert.ToDouble(helper.NextTokenRequired(), cultureInfo),
                Convert.ToDouble(helper.NextTokenRequired(), cultureInfo),
                Convert.ToDouble(helper.NextTokenRequired(), cultureInfo));
            helper.LastTokenRequired();
            return identity;
        }

        /// <summary>
        /// Converts this XMatrix to a human readable string.
        /// </summary>
        public override string ToString()
        {
            return ConvertToString(null, null);
        }

        /// <summary>
        /// Converts this XMatrix to a human readable string.
        /// </summary>
        public string ToString(IFormatProvider provider)
        {
            return ConvertToString(null, provider);
        }

        /// <summary>
        /// Converts this XMatrix to a human readable string.
        /// </summary>
        string IFormattable.ToString(string format, IFormatProvider provider)
        {
            return ConvertToString(format, provider);
        }

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            if (IsIdentity)
                return "Identity";

            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            provider = provider ?? CultureInfo.InvariantCulture;
            // ReSharper disable FormatStringProblem
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}",
                new object[] { numericListSeparator, _m11, _m12, _m21, _m22, _offsetX, _offsetY });
            // ReSharper restore FormatStringProblem
        }

        internal void MultiplyVector(ref double x, ref double y)
        {
            switch (_type)
            {
                case XMatrixTypes.Identity:
                case XMatrixTypes.Translation:
                    return;

                case XMatrixTypes.Scaling:
                case XMatrixTypes.Scaling | XMatrixTypes.Translation:
                    x *= _m11;
                    y *= _m22;
                    return;
            }
            double d1 = y * _m21;
            double d2 = x * _m12;
            x *= _m11;
            x += d1;
            y *= _m22;
            y += d2;
        }

        internal void MultiplyPoint(ref double x, ref double y)
        {
            switch (_type)
            {
                case XMatrixTypes.Identity:
                    return;

                case XMatrixTypes.Translation:
                    x += _offsetX;
                    y += _offsetY;
                    return;

                case XMatrixTypes.Scaling:
                    x *= _m11;
                    y *= _m22;
                    return;

                case (XMatrixTypes.Scaling | XMatrixTypes.Translation):
                    x *= _m11;
                    x += _offsetX;
                    y *= _m22;
                    y += _offsetY;
                    return;
            }
            double d1 = (y * _m21) + _offsetX;
            double d2 = (x * _m12) + _offsetY;
            x *= _m11;
            x += d1;
            y *= _m22;
            y += d2;
        }

        internal static XMatrix CreateTranslation(double offsetX, double offsetY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(1, 0, 0, 1, offsetX, offsetY, XMatrixTypes.Translation);
            return matrix;
        }

        internal static XMatrix CreateRotationRadians(double angle)
        {
            return CreateRotationRadians(angle, 0, 0);
        }

        internal static XMatrix CreateRotationRadians(double angle, double centerX, double centerY)
        {
            XMatrix matrix = new XMatrix();
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            double offsetX = (centerX * (1.0 - cos)) + (centerY * sin);
            double offsetY = (centerY * (1.0 - cos)) - (centerX * sin);
            matrix.SetMatrix(cos, sin, -sin, cos, offsetX, offsetY, XMatrixTypes.Unknown);
            return matrix;
        }

        internal static XMatrix CreateScaling(double scaleX, double scaleY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(scaleX, 0, 0, scaleY, 0, 0, XMatrixTypes.Scaling);
            return matrix;
        }

        internal static XMatrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(scaleX, 0, 0, scaleY, centerX - scaleX * centerX, centerY - scaleY * centerY, XMatrixTypes.Scaling | XMatrixTypes.Translation);
            return matrix;
        }

        internal static XMatrix CreateSkewRadians(double skewX, double skewY, double centerX, double centerY)
        {
            XMatrix matrix = new XMatrix();
            matrix.Append(CreateTranslation(-centerX, -centerY));
            matrix.Append(new XMatrix(1, Math.Tan(skewY), Math.Tan(skewX), 1, 0, 0));
            matrix.Append(CreateTranslation(centerX, centerY));
            return matrix;
        }

        internal static XMatrix CreateSkewRadians(double skewX, double skewY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(1, Math.Tan(skewY), Math.Tan(skewX), 1, 0, 0, XMatrixTypes.Unknown);
            return matrix;
        }

        static XMatrix CreateIdentity()
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(1, 0, 0, 1, 0, 0, XMatrixTypes.Identity);
            return matrix;
        }

        /// <summary>
        /// Sets the matrix.
        /// </summary>
        void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY, XMatrixTypes type)
        {
            _m11 = m11;
            _m12 = m12;
            _m21 = m21;
            _m22 = m22;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _type = type;
        }

        void DeriveMatrixType()
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            _type = XMatrixTypes.Identity;
            if (_m12 != 0 || _m21 != 0)
            {
                _type = XMatrixTypes.Unknown;
            }
            else
            {
                if (_m11 != 1 || _m22 != 1)
                    _type = XMatrixTypes.Scaling;

                if (_offsetX != 0 || _offsetY != 0)
                    _type |= XMatrixTypes.Translation;

                if ((_type & (XMatrixTypes.Scaling | XMatrixTypes.Translation)) == XMatrixTypes.Identity)
                    _type = XMatrixTypes.Identity;
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        bool IsDistinguishedIdentity
        {
            get { return (_type == XMatrixTypes.Identity); }
        }

        // Keep the fields private and force using the properties.
        // This prevents using m11 and m22 by mistake when the matrix is identity.
        double _m11;
        double _m12;
        double _m21;
        double _m22;
        double _offsetX;
        double _offsetY;
        XMatrixTypes _type;
        static readonly XMatrix s_identity = CreateIdentity();

        /// <summary>
        /// Internal matrix helper.
        /// </summary>
        internal static class MatrixHelper
        {
            // Fast mutiplication taking matrix type into account. Reflectored from WPF.
            internal static void MultiplyMatrix(ref XMatrix matrix1, ref XMatrix matrix2)
            {
                XMatrixTypes type1 = matrix1._type;
                XMatrixTypes type2 = matrix2._type;
                if (type2 != XMatrixTypes.Identity)
                {
                    if (type1 == XMatrixTypes.Identity)
                        matrix1 = matrix2;
                    else if (type2 == XMatrixTypes.Translation)
                    {
                        matrix1._offsetX += matrix2._offsetX;
                        matrix1._offsetY += matrix2._offsetY;
                        if (type1 != XMatrixTypes.Unknown)
                            matrix1._type |= XMatrixTypes.Translation;
                    }
                    else if (type1 == XMatrixTypes.Translation)
                    {
                        double num = matrix1._offsetX;
                        double num2 = matrix1._offsetY;
                        matrix1 = matrix2;
                        matrix1._offsetX = num * matrix2._m11 + num2 * matrix2._m21 + matrix2._offsetX;
                        matrix1._offsetY = num * matrix2._m12 + num2 * matrix2._m22 + matrix2._offsetY;
                        if (type2 == XMatrixTypes.Unknown)
                            matrix1._type = XMatrixTypes.Unknown;
                        else
                            matrix1._type = XMatrixTypes.Scaling | XMatrixTypes.Translation;
                    }
                    else
                    {
                        switch ((((int)type1) << 4) | (int)type2)
                        {
                            case 0x22:
                                matrix1._m11 *= matrix2._m11;
                                matrix1._m22 *= matrix2._m22;
                                return;

                            case 0x23:
                                matrix1._m11 *= matrix2._m11;
                                matrix1._m22 *= matrix2._m22;
                                matrix1._offsetX = matrix2._offsetX;
                                matrix1._offsetY = matrix2._offsetY;
                                matrix1._type = XMatrixTypes.Scaling | XMatrixTypes.Translation;
                                return;

                            case 0x24:
                            case 0x34:
                            case 0x42:
                            case 0x43:
                            case 0x44:
                                matrix1 = new XMatrix(
                                  matrix1._m11 * matrix2._m11 + matrix1._m12 * matrix2._m21,
                                  matrix1._m11 * matrix2._m12 + matrix1._m12 * matrix2._m22,
                                  matrix1._m21 * matrix2._m11 + matrix1._m22 * matrix2._m21,
                                  matrix1._m21 * matrix2._m12 + matrix1._m22 * matrix2._m22,
                                  matrix1._offsetX * matrix2._m11 + matrix1._offsetY * matrix2._m21 + matrix2._offsetX,
                                  matrix1._offsetX * matrix2._m12 + matrix1._offsetY * matrix2._m22 + matrix2._offsetY);
                                return;

                            case 50:
                                matrix1._m11 *= matrix2._m11;
                                matrix1._m22 *= matrix2._m22;
                                matrix1._offsetX *= matrix2._m11;
                                matrix1._offsetY *= matrix2._m22;
                                return;

                            case 0x33:
                                matrix1._m11 *= matrix2._m11;
                                matrix1._m22 *= matrix2._m22;
                                matrix1._offsetX = matrix2._m11 * matrix1._offsetX + matrix2._offsetX;
                                matrix1._offsetY = matrix2._m22 * matrix1._offsetY + matrix2._offsetY;
                                return;
                        }
                    }
                }
            }

            internal static void PrependOffset(ref XMatrix matrix, double offsetX, double offsetY)
            {
                if (matrix._type == XMatrixTypes.Identity)
                {
                    matrix = new XMatrix(1, 0, 0, 1, offsetX, offsetY);
                    matrix._type = XMatrixTypes.Translation;
                }
                else
                {
                    matrix._offsetX += (matrix._m11 * offsetX) + (matrix._m21 * offsetY);
                    matrix._offsetY += (matrix._m12 * offsetX) + (matrix._m22 * offsetY);
                    if (matrix._type != XMatrixTypes.Unknown)
                        matrix._type |= XMatrixTypes.Translation;
                }
            }

            internal static void TransformRect(ref XRect rect, ref XMatrix matrix)
            {
                if (!rect.IsEmpty)
                {
                    XMatrixTypes type = matrix._type;
                    if (type != XMatrixTypes.Identity)
                    {
                        if ((type & XMatrixTypes.Scaling) != XMatrixTypes.Identity)
                        {
                            rect.X *= matrix._m11;
                            rect.Y *= matrix._m22;
                            rect.Width *= matrix._m11;
                            rect.Height *= matrix._m22;
                            if (rect.Width < 0)
                            {
                                rect.X += rect.Width;
                                rect.Width = -rect.Width;
                            }
                            if (rect.Height < 0)
                            {
                                rect.Y += rect.Height;
                                rect.Height = -rect.Height;
                            }
                        }
                        if ((type & XMatrixTypes.Translation) != XMatrixTypes.Identity)
                        {
                            rect.X += matrix._offsetX;
                            rect.Y += matrix._offsetY;
                        }
                        if (type == XMatrixTypes.Unknown)
                        {
                            XPoint point1 = matrix.Transform(rect.TopLeft);
                            XPoint point2 = matrix.Transform(rect.TopRight);
                            XPoint point3 = matrix.Transform(rect.BottomRight);
                            XPoint point4 = matrix.Transform(rect.BottomLeft);
                            rect.X = Math.Min(Math.Min(point1.X, point2.X), Math.Min(point3.X, point4.X));
                            rect.Y = Math.Min(Math.Min(point1.Y, point2.Y), Math.Min(point3.Y, point4.Y));
                            rect.Width = Math.Max(Math.Max(point1.X, point2.X), Math.Max(point3.X, point4.X)) - rect.X;
                            rect.Height = Math.Max(Math.Max(point1.Y, point2.Y), Math.Max(point3.Y, point4.Y)) - rect.Y;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        /// <value>The debugger display.</value>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get
            {
                if (IsIdentity)
                    return "matrix=(Identity)";

                const string format = Config.SignificantFigures7;

                // Calculate the angle in degrees.
                XPoint point = new XMatrix(_m11, _m12, _m21, _m22, 0, 0).Transform(new XPoint(1, 0));
                double φ = Math.Atan2(point.Y, point.X) / Const.Deg2Rad;
                return String.Format(CultureInfo.InvariantCulture,
                    "matrix=({0:" + format + "}, {1:" + format + "}, {2:" + format + "}, {3:" + format + "}, {4:" + format + "}, {5:" + format + "}), φ={6:0.0#########}°",
                    _m11, _m12, _m21, _m22, _offsetX, _offsetY, φ);
            }
        }
    }
}
