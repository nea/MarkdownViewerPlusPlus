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
#if GDI
using System.Drawing;
#endif
#if WPF
using System.Windows;
#endif
using PdfSharp.Drawing;

namespace PdfSharp.Internal
{
    /// <summary>
    /// Some static helper functions for calculations.
    /// </summary>
    internal static class Calc
    {
        /// <summary>
        /// Degree to radiant factor.
        /// </summary>
        public const double Deg2Rad = Math.PI / 180;

        ///// <summary>
        ///// Half of pi.
        ///// </summary>
        //public const double πHalf = Math.PI / 2;
        //// α - β κ

        /// <summary>
        /// Get page size in point from specified PageSize.
        /// </summary>
        public static XSize PageSizeToSize(PageSize value)
        {
            switch (value)
            {
                case PageSize.A0:
                    return new XSize(2380, 3368);

                case PageSize.A1:
                    return new XSize(1684, 2380);

                case PageSize.A2:
                    return new XSize(1190, 1684);

                case PageSize.A3:
                    return new XSize(842, 1190);

                case PageSize.A4:
                    return new XSize(595, 842);

                case PageSize.A5:
                    return new XSize(420, 595);

                case PageSize.B4:
                    return new XSize(729, 1032);

                case PageSize.B5:
                    return new XSize(516, 729);

                // The strange sizes from overseas...

                case PageSize.Letter:
                    return new XSize(612, 792);

                case PageSize.Legal:
                    return new XSize(612, 1008);

                case PageSize.Tabloid:
                    return new XSize(792, 1224);

                case PageSize.Ledger:
                    return new XSize(1224, 792);

                case PageSize.Statement:
                    return new XSize(396, 612);

                case PageSize.Executive:
                    return new XSize(540, 720);

                case PageSize.Folio:
                    return new XSize(612, 936);

                case PageSize.Quarto:
                    return new XSize(610, 780);

                case PageSize.Size10x14:
                    return new XSize(720, 1008);
            }
            throw new ArgumentException("Invalid PageSize.");
        }
    }
}
