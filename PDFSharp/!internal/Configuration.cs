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

namespace PdfSharp
{
    /// <summary>
    /// Floating point formatting.
    /// </summary>
    static class Config
    {
        public const string SignificantFigures2 = "0.##";
        public const string SignificantFigures3 = "0.###";
        public const string SignificantFigures4 = "0.####";
        public const string SignificantFigures7 = "0.#######";
        public const string SignificantFigures10 = "0.##########";
        public const string SignificantFigures1Plus9 = "0.0#########";
    }

    static class Const
    {
        /// <summary>
        /// Factor to convert from degree to radian measure.
        /// </summary>
        public const double Deg2Rad = Math.PI / 180;  // = 0.017453292519943295

        /// <summary>
        /// Sinus of the angle to turn a regular font to look oblique. Used for italic simulation.
        /// </summary>
        public const double ItalicSkewAngleSinus = 0.34202014332566873304409961468226;  // = sin(20°)

        /// <summary>
        /// Factor of the em size of a regular font to look bold. Used for bold simulation.
        /// Value of 2% found in original XPS 1.0 documentation.
        /// </summary>
        public const double BoldEmphasis = 0.02;

        // The κ (kappa) for drawing a circle or an ellipse with four Bézier splines, specifying the distance of the influence point from the starting or end point of a spline.
        // Petzold: 4/3 * tan(α / 4)
        public const double κ = 0.5522847498307933984022516322796;  // := 4/3 * (1 - cos(-π/4)) / sin(π/4)) <=> 4/3 * (sqrt(2) - 1) <=> 4/3 * tan(π/8)
    }
}
