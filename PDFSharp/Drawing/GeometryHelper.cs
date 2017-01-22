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
using System.Collections.Generic;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows.Media;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
#endif
#if NETFX_CORE ||  UWP
using Windows.UI.Xaml.Media;
using SysPoint = Windows.Foundation.Point;
using SysSize = Windows.Foundation.Size;
#endif
using PdfSharp.Internal;

// ReSharper disable RedundantNameQualifier
// ReSharper disable CompareOfFloatsByEqualityOperator

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Helper class for Geometry paths.
    /// </summary>
    static class GeometryHelper
    {
#if WPF || NETFX_CORE
        /// <summary>
        /// Appends a Bézier segment from a curve.
        /// </summary>
        public static BezierSegment CreateCurveSegment(XPoint pt0, XPoint pt1, XPoint pt2, XPoint pt3, double tension3)
        {
#if !SILVERLIGHT && !NETFX_CORE
            return new BezierSegment(
                new SysPoint(pt1.X + tension3 * (pt2.X - pt0.X), pt1.Y + tension3 * (pt2.Y - pt0.Y)),
                new SysPoint(pt2.X - tension3 * (pt3.X - pt1.X), pt2.Y - tension3 * (pt3.Y - pt1.Y)),
                new SysPoint(pt2.X, pt2.Y), true);
#else
            BezierSegment bezierSegment = new BezierSegment();
            bezierSegment.Point1 = new SysPoint(pt1.X + tension3 * (pt2.X - pt0.X), pt1.Y + tension3 * (pt2.Y - pt0.Y));
            bezierSegment.Point2 = new SysPoint(pt2.X - tension3 * (pt3.X - pt1.X), pt2.Y - tension3 * (pt3.Y - pt1.Y));
            bezierSegment.Point3 = new SysPoint(pt2.X, pt2.Y);
            return bezierSegment;
#endif
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Creates a path geometry from a polygon.
        /// </summary>
        public static PathGeometry CreatePolygonGeometry(SysPoint[] points, XFillMode fillMode, bool closed)
        {
            PolyLineSegment seg = new PolyLineSegment();
            int count = points.Length;
            // For correct drawing the start point of the segment must not be the same as the first point.
            for (int idx = 1; idx < count; idx++)
                seg.Points.Add(new SysPoint(points[idx].X, points[idx].Y));
#if !SILVERLIGHT &&  !NETFX_CORE
            seg.IsStroked = true;
#endif
            PathFigure fig = new PathFigure();
            fig.StartPoint = new SysPoint(points[0].X, points[0].Y);
            fig.Segments.Add(seg);
            fig.IsClosed = closed;
            PathGeometry geo = new PathGeometry();
            geo.FillRule = fillMode == XFillMode.Winding ? FillRule.Nonzero : FillRule.EvenOdd;
            geo.Figures.Add(fig);
            return geo;
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Creates a path geometry from a polygon.
        /// </summary>
        public static PolyLineSegment CreatePolyLineSegment(SysPoint[] points, XFillMode fillMode, bool closed)
        {
            PolyLineSegment seg = new PolyLineSegment();
            int count = points.Length;
            // For correct drawing the start point of the segment must not be the same as the first point.
            for (int idx = 1; idx < count; idx++)
                seg.Points.Add(new SysPoint(points[idx].X, points[idx].Y));
#if !SILVERLIGHT && !NETFX_CORE
            seg.IsStroked = true;
#endif
            return seg;
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Creates the arc segment from parameters of the GDI+ DrawArc function.
        /// </summary>
        public static ArcSegment CreateArcSegment(double x, double y, double width, double height, double startAngle,
          double sweepAngle, out SysPoint startPoint)
        {
            // Normalize the angles.
            double α = startAngle;
            if (α < 0)
                α = α + (1 + Math.Floor((Math.Abs(α) / 360))) * 360;
            else if (α > 360)
                α = α - Math.Floor(α / 360) * 360;
            Debug.Assert(α >= 0 && α <= 360);

            if (Math.Abs(sweepAngle) >= 360)
                sweepAngle = Math.Sign(sweepAngle) * 360;
            double β = startAngle + sweepAngle;
            if (β < 0)
                β = β + (1 + Math.Floor((Math.Abs(β) / 360))) * 360;
            else if (β > 360)
                β = β - Math.Floor(β / 360) * 360;

            if (α == 0 && β < 0)
                α = 360;
            else if (α == 360 && β > 0)
                α = 0;

            // Scanling factor.
            double δx = width / 2;
            double δy = height / 2;

            // Center of ellipse.
            double x0 = x + δx;
            double y0 = y + δy;

            double cosα, cosβ, sinα, sinβ;
            if (width == height)
            {
                // Circular arc needs no correction.
                α = α * Calc.Deg2Rad;
                β = β * Calc.Deg2Rad;
            }
            else
            {
                // Elliptic arc needs the angles to be adjusted such that the scaling transformation is compensated.
                α = α * Calc.Deg2Rad;
                sinα = Math.Sin(α);
                if (Math.Abs(sinα) > 1E-10)
                {
                    if (α < Math.PI)
                        α = Math.PI / 2 - Math.Atan(δy * Math.Cos(α) / (δx * sinα));
                    else
                        α = 3 * Math.PI / 2 - Math.Atan(δy * Math.Cos(α) / (δx * sinα));
                }
                //α = Calc.πHalf - Math.Atan(δy * Math.Cos(α) / (δx * sinα));
                β = β * Calc.Deg2Rad;
                sinβ = Math.Sin(β);
                if (Math.Abs(sinβ) > 1E-10)
                {
                    if (β < Math.PI)
                        β = Math.PI / 2 - Math.Atan(δy * Math.Cos(β) / (δx * sinβ));
                    else
                        β = 3 * Math.PI / 2 - Math.Atan(δy * Math.Cos(β) / (δx * sinβ));
                }
                //β = Calc.πHalf - Math.Atan(δy * Math.Cos(β) / (δx * sinβ));
            }

            sinα = Math.Sin(α);
            cosα = Math.Cos(α);
            sinβ = Math.Sin(β);
            cosβ = Math.Cos(β);

            startPoint = new SysPoint(x0 + δx * cosα, y0 + δy * sinα);
            SysPoint destPoint = new SysPoint(x0 + δx * cosβ, y0 + δy * sinβ);
            SysSize size = new SysSize(δx, δy);
            bool isLargeArc = Math.Abs(sweepAngle) >= 180;
            SweepDirection sweepDirection = sweepAngle > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise;
#if !SILVERLIGHT && !NETFX_CORE
            bool isStroked = true;
            ArcSegment seg = new ArcSegment(destPoint, size, 0, isLargeArc, sweepDirection, isStroked);
#else
            ArcSegment seg = new ArcSegment();
            seg.Point = destPoint;
            seg.Size = size;
            seg.RotationAngle = 0;
            seg.IsLargeArc = isLargeArc;
            seg.SweepDirection = sweepDirection;
            // isStroked does not exist in Silverlight 3
#endif
            return seg;
        }
#endif

        /// <summary>
        /// Creates between 1 and 5 Béziers curves from parameters specified like in GDI+.
        /// </summary>
        public static List<XPoint> BezierCurveFromArc(double x, double y, double width, double height, double startAngle, double sweepAngle,
            PathStart pathStart, ref XMatrix matrix)
        {
            List<XPoint> points = new List<XPoint>();

            // Normalize the angles.
            double α = startAngle;
            if (α < 0)
                α = α + (1 + Math.Floor((Math.Abs(α) / 360))) * 360;
            else if (α > 360)
                α = α - Math.Floor(α / 360) * 360;
            Debug.Assert(α >= 0 && α <= 360);

            double β = sweepAngle;
            if (β < -360)
                β = -360;
            else if (β > 360)
                β = 360;

            if (α == 0 && β < 0)
                α = 360;
            else if (α == 360 && β > 0)
                α = 0;

            // Is it possible that the arc is small starts and ends in same quadrant?
            bool smallAngle = Math.Abs(β) <= 90;

            β = α + β;
            if (β < 0)
                β = β + (1 + Math.Floor((Math.Abs(β) / 360))) * 360;

            bool clockwise = sweepAngle > 0;
            int startQuadrant = Quadrant(α, true, clockwise);
            int endQuadrant = Quadrant(β, false, clockwise);

            if (startQuadrant == endQuadrant && smallAngle)
                AppendPartialArcQuadrant(points, x, y, width, height, α, β, pathStart, matrix);
            else
            {
                int currentQuadrant = startQuadrant;
                bool firstLoop = true;
                do
                {
                    if (currentQuadrant == startQuadrant && firstLoop)
                    {
                        double ξ = currentQuadrant * 90 + (clockwise ? 90 : 0);
                        AppendPartialArcQuadrant(points, x, y, width, height, α, ξ, pathStart, matrix);
                    }
                    else if (currentQuadrant == endQuadrant)
                    {
                        double ξ = currentQuadrant * 90 + (clockwise ? 0 : 90);
                        AppendPartialArcQuadrant(points, x, y, width, height, ξ, β, PathStart.Ignore1st, matrix);
                    }
                    else
                    {
                        double ξ1 = currentQuadrant * 90 + (clockwise ? 0 : 90);
                        double ξ2 = currentQuadrant * 90 + (clockwise ? 90 : 0);
                        AppendPartialArcQuadrant(points, x, y, width, height, ξ1, ξ2, PathStart.Ignore1st, matrix);
                    }

                    // Don't stop immediately if arc is greater than 270 degrees.
                    if (currentQuadrant == endQuadrant && smallAngle)
                        break;
                    smallAngle = true;

                    if (clockwise)
                        currentQuadrant = currentQuadrant == 3 ? 0 : currentQuadrant + 1;
                    else
                        currentQuadrant = currentQuadrant == 0 ? 3 : currentQuadrant - 1;

                    firstLoop = false;
                } while (true);
            }
            return points;
        }

        /// <summary>
        /// Calculates the quadrant (0 through 3) of the specified angle. If the angle lies on an edge
        /// (0, 90, 180, etc.) the result depends on the details how the angle is used.
        /// </summary>
        static int Quadrant(double φ, bool start, bool clockwise)
        {
            Debug.Assert(φ >= 0);
            if (φ > 360)
                φ = φ - Math.Floor(φ / 360) * 360;

            int quadrant = (int)(φ / 90);
            if (quadrant * 90 == φ)
            {
                if ((start && !clockwise) || (!start && clockwise))
                    quadrant = quadrant == 0 ? 3 : quadrant - 1;
            }
            else
                quadrant = clockwise ? ((int)Math.Floor(φ / 90)) % 4 : (int)Math.Floor(φ / 90);
            return quadrant;
        }

        /// <summary>
        /// Appends a Bézier curve for an arc within a full quadrant.
        /// </summary>
        static void AppendPartialArcQuadrant(List<XPoint> points, double x, double y, double width, double height, double α, double β, PathStart pathStart, XMatrix matrix)
        {
            Debug.Assert(α >= 0 && α <= 360);
            Debug.Assert(β >= 0);
            if (β > 360)
                β = β - Math.Floor(β / 360) * 360;
            Debug.Assert(Math.Abs(α - β) <= 90);

            // Scanling factor.
            double δx = width / 2;
            double δy = height / 2;

            // Center of ellipse.
            double x0 = x + δx;
            double y0 = y + δy;

            // We have the following quarters:
            //     |
            //   2 | 3
            // ----+-----
            //   1 | 0
            //     |
            // If the angles lie in quarter 2 or 3, their values are subtracted by 180 and the
            // resulting curve is reflected at the center. This algorithm works as expected (simply tried out).
            // There may be a mathematically more elegant solution...
            bool reflect = false;
            if (α >= 180 && β >= 180)
            {
                α -= 180;
                β -= 180;
                reflect = true;
            }

            double cosα, cosβ, sinα, sinβ;
            if (width == height)
            {
                // Circular arc needs no correction.
                α = α * Calc.Deg2Rad;
                β = β * Calc.Deg2Rad;
            }
            else
            {
                // Elliptic arc needs the angles to be adjusted such that the scaling transformation is compensated.
                α = α * Calc.Deg2Rad;
                sinα = Math.Sin(α);
                if (Math.Abs(sinα) > 1E-10)
                    α = Math.PI / 2 - Math.Atan(δy * Math.Cos(α) / (δx * sinα));
                β = β * Calc.Deg2Rad;
                sinβ = Math.Sin(β);
                if (Math.Abs(sinβ) > 1E-10)
                    β = Math.PI / 2 - Math.Atan(δy * Math.Cos(β) / (δx * sinβ));
            }

            double κ = 4 * (1 - Math.Cos((α - β) / 2)) / (3 * Math.Sin((β - α) / 2));
            sinα = Math.Sin(α);
            cosα = Math.Cos(α);
            sinβ = Math.Sin(β);
            cosβ = Math.Cos(β);

            //XPoint pt1, pt2, pt3;
            if (!reflect)
            {
                // Calculation for quarter 0 and 1.
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        points.Add(matrix.Transform(new XPoint(x0 + δx * cosα, y0 + δy * sinα)));
                        break;

                    case PathStart.LineTo1st:
                        points.Add(matrix.Transform(new XPoint(x0 + δx * cosα, y0 + δy * sinα)));
                        break;

                    case PathStart.Ignore1st:
                        break;
                }
                points.Add(matrix.Transform(new XPoint(x0 + δx * (cosα - κ * sinα), y0 + δy * (sinα + κ * cosα))));
                points.Add(matrix.Transform(new XPoint(x0 + δx * (cosβ + κ * sinβ), y0 + δy * (sinβ - κ * cosβ))));
                points.Add(matrix.Transform(new XPoint(x0 + δx * cosβ, y0 + δy * sinβ)));
            }
            else
            {
                // Calculation for quarter 2 and 3.
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        points.Add(matrix.Transform(new XPoint(x0 - δx * cosα, y0 - δy * sinα)));
                        break;

                    case PathStart.LineTo1st:
                        points.Add(matrix.Transform(new XPoint(x0 - δx * cosα, y0 - δy * sinα)));
                        break;

                    case PathStart.Ignore1st:
                        break;
                }
                points.Add(matrix.Transform(new XPoint(x0 - δx * (cosα - κ * sinα), y0 - δy * (sinα + κ * cosα))));
                points.Add(matrix.Transform(new XPoint(x0 - δx * (cosβ + κ * sinβ), y0 - δy * (sinβ - κ * cosβ))));
                points.Add(matrix.Transform(new XPoint(x0 - δx * cosβ, y0 - δy * sinβ)));
            }
        }

        /// <summary>
        /// Creates between 1 and 5 Béziers curves from parameters specified like in WPF.
        /// </summary>
        public static List<XPoint> BezierCurveFromArc(XPoint point1, XPoint point2, XSize size,
            double rotationAngle, bool isLargeArc, bool clockwise, PathStart pathStart)
        {
            // See also http://www.charlespetzold.com/blog/blog.xml from January 2, 2008: 
            // http://www.charlespetzold.com/blog/2008/01/Mathematics-of-ArcSegment.html
            double δx = size.Width;
            double δy = size.Height;
            Debug.Assert(δx * δy > 0);
            double factor = δy / δx;
            bool isCounterclockwise = !clockwise;

            // Adjust for different radii and rotation angle.
            XMatrix matrix = new XMatrix();
            matrix.RotateAppend(-rotationAngle);
            matrix.ScaleAppend(δy / δx, 1);
            XPoint pt1 = matrix.Transform(point1);
            XPoint pt2 = matrix.Transform(point2);

            // Get info about chord that connects both points.
            XPoint midPoint = new XPoint((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2);
            XVector vect = pt2 - pt1;
            double halfChord = vect.Length / 2;

            // Get vector from chord to center.
            XVector vectRotated;

            // (comparing two Booleans here!)
            if (isLargeArc == isCounterclockwise)
                vectRotated = new XVector(-vect.Y, vect.X);
            else
                vectRotated = new XVector(vect.Y, -vect.X);

            vectRotated.Normalize();

            // Distance from chord to center.
            double centerDistance = Math.Sqrt(δy * δy - halfChord * halfChord);
            if (double.IsNaN(centerDistance))
                centerDistance = 0;

            // Calculate center point.
            XPoint center = midPoint + centerDistance * vectRotated;

            // Get angles from center to the two points.
            double α = Math.Atan2(pt1.Y - center.Y, pt1.X - center.X);
            double β = Math.Atan2(pt2.Y - center.Y, pt2.X - center.X);

            // (another comparison of two Booleans!)
            if (isLargeArc == (Math.Abs(β - α) < Math.PI))
            {
                if (α < β)
                    α += 2 * Math.PI;
                else
                    β += 2 * Math.PI;
            }

            // Invert matrix for final point calculation.
            matrix.Invert();
            double sweepAngle = β - α;

            // Let the algorithm of GDI+ DrawArc to Bézier curves do the rest of the job
            return BezierCurveFromArc(center.X - δx * factor, center.Y - δy, 2 * δx * factor, 2 * δy,
              α / Calc.Deg2Rad, sweepAngle / Calc.Deg2Rad, pathStart, ref matrix);
        }







        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // The code below comes from WPF source code, because I was not able to convert an arc
        // to a series of Bezier curves exactly the way WPF renders the arc. I tested my own code
        // with the MinBar Test Suite from QualityLogic and could not find out why it does not match.
        // My Bezier curves came very close to the arc, but in some cases they do simply not match.
        // So I gave up and use the WPF code.
#if WPF || NETFX_CORE

        // ReSharper disable InconsistentNaming
        const double FUZZ = 1e-6;           // Relative 0
        // ReSharper restore InconsistentNaming

        //+-------------------------------------------------------------------------------------------------

        // 
        //  Function: GetArcAngle
        // 
        //  Synopsis: Get the number of Bezier arcs, and sine & cosine of each 
        //
        //  Notes:    This is a private utility used by ArcToBezier 
        //            We break the arc into pieces so that no piece will span more than 90 degrees.
        //            The input points are on the unit circle
        //
        //------------------------------------------------------------------------------------------------- 
        public static void
        GetArcAngle(
            XPoint startPoint,      // Start point 
            XPoint endPoint,        // End point
            bool isLargeArc,     // Choose the larger of the 2 possible arcs if TRUE 
            //SweepDirection sweepDirection,      // Direction n which to sweep the arc.
            bool isClockwise,
            out double cosArcAngle, // Cosine of a the sweep angle of one arc piece
            out double sinArcAngle, // Sine of a the sweep angle of one arc piece
            out int pieces)      // Out: The number of pieces 
        {
            double angle;

            // The points are on the unit circle, so:
            cosArcAngle = startPoint.X * endPoint.X + startPoint.Y * endPoint.Y;
            sinArcAngle = startPoint.X * endPoint.Y - startPoint.Y * endPoint.X;

            if (cosArcAngle >= 0)
            {
                if (isLargeArc)
                {
                    // The angle is between 270 and 360 degrees, so 
                    pieces = 4;
                }
                else
                {
                    // The angle is between 0 and 90 degrees, so
                    pieces = 1;
                    return; // We already have the cosine and sine of the angle
                }
            }
            else
            {
                if (isLargeArc)
                {
                    // The angle is between 180 and 270 degrees, so
                    pieces = 3;
                }
                else
                {
                    // The angle is between 90 and 180 degrees, so
                    pieces = 2;
                }
            }

            // We have to chop the arc into the computed number of pieces.  For cPieces=2 and 4 we could 
            // have uses the half-angle trig formulas, but for pieces=3 it requires solving a cubic
            // equation; the performance difference is not worth the extra code, so we'll get the angle, 
            // divide it, and get its sine and cosine. 

            Debug.Assert(pieces > 0);
            angle = Math.Atan2(sinArcAngle, cosArcAngle);

            if (isClockwise)
            {
                if (angle < 0)
                    angle += Math.PI * 2;
            }
            else
            {
                if (angle > 0)
                    angle -= Math.PI * 2;
            }

            angle /= pieces;
            cosArcAngle = Math.Cos(angle);
            sinArcAngle = Math.Sin(angle);
        }

        /******************************************************************************\
        * 
        * Function Description: 
        *
        * Get the distance from a circular arc's endpoints to the control points of the 
        * Bezier arc that approximates it, as a fraction of the arc's radius.
        *
        * Since the result is relative to the arc's radius, it depends strictly on the
        * arc's angle. The arc is assumed to be of 90 degrees of less, so the angle is 
        * determined by the cosine of that angle, which is derived from rDot = the dot
        * product of two radius vectors.  We need the Bezier curve that agrees with 
        * the arc's points and tangents at the ends and midpoint.  Here we compute the 
        * distance from the curve's endpoints to its control points.
        * 
        * Since we are looking for the relative distance, we can work on the unit
        * circle. Place the center of the circle at the origin, and put the X axis as
        * the bisector between the 2 vectors.  Let a be the angle between the vectors.
        * Then the X coordinates of the 1st & last points are cos(a/2).  Let x be the X 
        * coordinate of the 2nd & 3rd points.  At t=1/2 we have a point at (1,0).
        * But the terms of the polynomial there are all equal: 
        * 
        *           (1-t)^3 = t*(1-t)^2 = 2^2*(1-t) = t^3 = 1/8,
        * 
        * so from the Bezier formula there we have:
        *
        *           1 = (1/8) * (cos(a/2) + 3x + 3x + cos(a/2)),
        * hence 
        *           x = (1 - cos(a/2)) / 3
        * 
        * The X difference between that and the 1st point is: 
        *
        *           DX = x - cos(a/2) = 4(1 - cos(a/2)) / 3. 
        *
        * But DX = distance / sin(a/2), hence the distance is
        *
        *           dist = (4/3)*(1 - cos(a/2)) / sin(a/2). 
        *
        * Created:  5/29/2001 [....] 
        * 
        /*****************************************************************************/
        public static double
        GetBezierDistance(  // Return the distance as a fraction of the radius
            double dot,    // In: The dot product of the two radius vectors
            double radius) // In: The radius of the arc's circle (optional=1)
        {
            double radSquared = radius * radius;  // Squared radius

            Debug.Assert(dot >= -radSquared * .1);  // angle < 90 degrees 
            Debug.Assert(dot <= radSquared * 1.1);  // as dot product of 2 radius vectors

            double dist = 0;   // Acceptable fallback value

            /* Rather than the angle a, we are given rDot = R^2 * cos(a), so we
                multiply top and bottom by R: 

                                dist = (4/3)*(R - Rcos(a/2)) / Rsin(a/2) 
 
                and use some trig:
                                    __________ 
                        cos(a/2) = \/1 + cos(a) / 2
                                        ________________         __________
                        R*cos(a/2) = \/R^2 + R^2 cos(a) / 2 = \/R^2 + rDot / 2 */

            double cos = (radSquared + dot) / 2;   // =(R*cos(a))^2
            if (cos < 0)
                return dist;
            //                 __________________
            //  R*sin(a/2) = \/R^2 - R^2 cos(a/2)

            double sin = radSquared - cos;         // =(R*sin(a))^2 
            if (sin <= 0)
                return dist;

            sin = Math.Sqrt(sin); //   = R*cos(a) 
            cos = Math.Sqrt(cos); //   = R*sin(a)

            dist = 4 * (radius - cos) / 3;
            if (dist <= sin * FUZZ)
                dist = 0;
            else
                dist = 4 * (radius - cos) / sin / 3;

            return dist;
        }

        //+------------------------------------------------------------------------------------------------- 
        //
        //  Function: ArcToBezier 
        //
        //  Synopsis: Compute the Bezier approximation of an arc
        //
        //  Notes:    This utilitycomputes the Bezier approximation for an elliptical arc as it is defined 
        //            in the SVG arc spec. The ellipse from which the arc is carved is axis-aligned in its
        //            own coordinates, and defined there by its x and y radii. The rotation angle defines 
        //            how the ellipse's axes are rotated relative to our x axis. The start and end points 
        //            define one of 4 possible arcs; the sweep and large-arc flags determine which one of
        //            these arcs will be chosen. See SVG spec for details. 
        //
        //            Returning pieces = 0 indicates a line instead of an arc
        //                      pieces = -1 indicates that the arc degenerates to a point
        // 
        //--------------------------------------------------------------------------------------------------
        public static PointCollection ArcToBezier(double xStart, double yStart, double xRadius, double yRadius, double rotationAngle,
            bool isLargeArc, bool isClockwise, double xEnd, double yEnd, out int pieces)
        {
            double cosArcAngle, sinArcAngle, xCenter, yCenter, r, bezDist;
            XVector vecToBez1, vecToBez2;
            XMatrix matToEllipse;

            double fuzz2 = FUZZ * FUZZ;
            bool isZeroCenter = false;

            pieces = -1;

            // In the following, the line segment between between the arc's start and
            // end points is referred to as "the chord".

            // Transform 1: Shift the origin to the chord's midpoint 
            double x = (xEnd - xStart) / 2;
            double y = (yEnd - yStart) / 2;

            double halfChord2 = x * x + y * y;     // (half chord length)^2

            // Degenerate case: single point
            if (halfChord2 < fuzz2)
            {
                // The chord degeneartes to a point, the arc will be ignored 
                return null;
            }

            // Degenerate case: straight line
            if (!AcceptRadius(halfChord2, fuzz2, ref xRadius) || !AcceptRadius(halfChord2, fuzz2, ref yRadius))
            {
                // We have a zero radius, add a straight line segment instead of an arc
                pieces = 0;
                return null;
            }

            if (xRadius == 0 || yRadius == 0)
            {
                // We have a zero radius, add a straight line segment instead of an arc
                pieces = 0;
                return null;
            }

            // Transform 2: Rotate to the ellipse's coordinate system
            rotationAngle = -rotationAngle * Calc.Deg2Rad;

            double cos = Math.Cos(rotationAngle);
            double sin = Math.Sin(rotationAngle);

            r = x * cos - y * sin;
            y = x * sin + y * cos;
            x = r;

            // Transform 3: Scale so that the ellipse will become a unit circle 
            x /= xRadius;
            y /= yRadius;

            // We get to the center of that circle along a verctor perpendicular to the chord 
            // from the origin, which is the chord's midpoint. By Pythagoras, the length of that
            // vector is sqrt(1 - (half chord)^2). 

            halfChord2 = x * x + y * y;   // now in the circle coordinates

            if (halfChord2 > 1)
            {
                // The chord is longer than the circle's diameter; we scale the radii uniformly so
                // that the chord will be a diameter. The center will then be the chord's midpoint, 
                // which is now the origin.
                r = Math.Sqrt(halfChord2);
                xRadius *= r;
                yRadius *= r;
                xCenter = yCenter = 0;
                isZeroCenter = true;

                // Adjust the unit-circle coordinates x and y
                x /= r;
                y /= r;
            }
            else
            {
                // The length of (-y,x) or (x,-y) is sqrt(rHalfChord2), and we want a vector 
                // of length sqrt(1 - rHalfChord2), so we'll multiply it by:
                r = Math.Sqrt((1 - halfChord2) / halfChord2);
                //if (isLargeArc != (eSweepDirection == SweepDirection.Clockwise))
                if (isLargeArc != isClockwise)
                // Going to the center from the origin=chord-midpoint 
                {
                    // in the direction of (-y, x) 
                    xCenter = -r * y;
                    yCenter = r * x;
                }
                else
                {
                    // in the direction of (y, -x)
                    xCenter = r * y;
                    yCenter = -r * x;
                }
            }

            // Transformation 4: shift the origin to the center of the circle, which then becomes 
            // the unit circle. Since the chord's midpoint is the origin, the start point is (-x, -y)
            // and the endpoint is (x, y).
            XPoint ptStart = new XPoint(-x - xCenter, -y - yCenter);
            XPoint ptEnd = new XPoint(x - xCenter, y - yCenter);

            // Set up the matrix that will take us back to our coordinate system.  This matrix is 
            // the inverse of the combination of transformation 1 thru 4. 
            matToEllipse = new XMatrix(cos * xRadius, -sin * xRadius,
                                      sin * yRadius, cos * yRadius,
                                      (xEnd + xStart) / 2, (yEnd + yStart) / 2);

            if (!isZeroCenter)
            {
                // Prepend the translation that will take the origin to the circle's center
                matToEllipse.OffsetX += (matToEllipse.M11 * xCenter + matToEllipse.M21 * yCenter);
                matToEllipse.OffsetY += (matToEllipse.M12 * xCenter + matToEllipse.M22 * yCenter);
            }

            // Get the sine & cosine of the angle that will generate the arc pieces
            GetArcAngle(ptStart, ptEnd, isLargeArc, isClockwise, out cosArcAngle, out sinArcAngle, out pieces);

            // Get the vector to the first Bezier control point 
            bezDist = GetBezierDistance(cosArcAngle, 1);

            //if (eSweepDirection == SweepDirection.Counterclockwise)
            if (!isClockwise)
                bezDist = -bezDist;

            vecToBez1 = new XVector(-bezDist * ptStart.Y, bezDist * ptStart.X);

            PointCollection result = new PointCollection();

            // Add the arc pieces, except for the last 
            for (int idx = 1; idx < pieces; idx++)
            {
                // Get the arc piece's endpoint
                XPoint ptPieceEnd = new XPoint(ptStart.X * cosArcAngle - ptStart.Y * sinArcAngle, ptStart.X * sinArcAngle + ptStart.Y * cosArcAngle);
                vecToBez2 = new XVector(-bezDist * ptPieceEnd.Y, bezDist * ptPieceEnd.X);

                result.Add(matToEllipse.Transform(ptStart + vecToBez1));
                result.Add(matToEllipse.Transform(ptPieceEnd - vecToBez2));
                result.Add(matToEllipse.Transform(ptPieceEnd));

                // Move on to the next arc
                ptStart = ptPieceEnd;
                vecToBez1 = vecToBez2;
            }

            // Last arc - we know the endpoint 
            vecToBez2 = new XVector(-bezDist * ptEnd.Y, bezDist * ptEnd.X);

            result.Add(matToEllipse.Transform(ptStart + vecToBez1));
            result.Add(matToEllipse.Transform(ptEnd - vecToBez2));
            result.Add(new XPoint(xEnd, yEnd));

            return result;
        }

        /// <summary>
        /// Gets a value indicating whether radius large enough compared to the chord length.
        /// </summary>
        /// <param name="halfChord2">(1/2 chord length)squared </param>
        /// <param name="fuzz2">Squared fuzz.</param>
        /// <param name="radius">The radius to accept (or not).</param>
        static bool AcceptRadius(double halfChord2, double fuzz2, ref double radius)
        {
            Debug.Assert(halfChord2 >= fuzz2);   // Otherewise we have no guarantee that the radius is not 0, and we need to divide by the radius
            bool accept = radius * radius > halfChord2 * fuzz2;
            if (accept)
            {
                if (radius < 0)
                    radius = 0;
            }
            return accept;
        }
#endif
    }
}