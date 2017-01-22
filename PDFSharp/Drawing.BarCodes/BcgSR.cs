#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Klaus Potzesny
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

namespace PdfSharp.Drawing.BarCodes
{
    // TODO: Mere with PDFsharp strings table
    /// <summary>
    /// String resources for the empira barcode renderer.
    /// </summary>
    internal class BcgSR
    {
        internal static string Invalid2Of5Code(string code)
        {
            return string.Format("'{0}' is not a valid code for an interleave 2 of 5 bar code. It can only represent an even number of digits.", code);
        }

        internal static string Invalid3Of9Code(string code)
        {
            return string.Format("'{0}' is not a valid code for a 3 of 9 standard bar code.", code);
        }

        internal static string BarCodeNotSet
        {
            get { return "A text must be set before rendering the bar code."; }
        }

        internal static string EmptyBarCodeSize
        {
            get { return "A non-empty size must be set before rendering the bar code."; }
        }

        internal static string Invalid2of5Relation
        {
            get { return "Value of relation between thick and thin lines on the interleaved 2 of 5 code must be between 2 and 3."; }
        }

        internal static string InvalidMarkName(string name)
        {
            return string.Format("'{0}' is not a valid mark name for this OMR representation.", name);
        }

        internal static string OmrAlreadyInitialized
        {
            get { return "Mark descriptions cannot be set when marks have already been set on OMR."; }
        }

        internal static string DataMatrixTooBig
        {
            get { return "The given data and encoding combination is too big for the matrix size."; }
        }

        internal static string DataMatrixNotSupported
        {
            get { return "Zero sizes, odd sizes and other than ecc200 coded DataMatrix is not supported."; }
        }

        internal static string DataMatrixNull
        {
            get { return "No DataMatrix code is produced."; }
        }

        internal static string DataMatrixInvalid(int columns, int rows)
        {
            return string.Format("'{1}'x'{0}' is an invalid ecc200 DataMatrix size.", columns, rows);
        }
    }
}
