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
using System.Globalization;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.IO;

#if GDI
using System.Drawing;
#endif
#if WPF
using System.Windows;
#endif

namespace PdfSharp.Internal
{
    enum NotImplementedBehaviour
    {
        DoNothing, Log, Throw
    }

    /// <summary>
    /// A bunch of internal helper functions.
    /// </summary>
    internal static class Diagnostics
    {
        public static NotImplementedBehaviour NotImplementedBehaviour
        {
            get { return _notImplementedBehaviour; }
            set { _notImplementedBehaviour = value; }
        }
        static NotImplementedBehaviour _notImplementedBehaviour;
    }

    internal static class ParserDiagnostics
    {
        public static void ThrowParserException(string message)
        {
            throw new PdfReaderException(message);
        }

        public static void ThrowParserException(string message, Exception innerException)
        {
            throw new PdfReaderException(message, innerException);
        }

        public static void HandleUnexpectedCharacter(char ch)
        {
            // Hex formatting does not work with type char. It must be casted to integer.
            string message = string.Format(CultureInfo.InvariantCulture,
                "Unexpected character '0x{0:x4}' in PDF stream. The file may be corrupted. " +
                "If you think this is a bug in PDFsharp, please send us your PDF file.", (int)ch);
            ThrowParserException(message);
        }
        public static void HandleUnexpectedToken(string token)
        {
            string message = string.Format(CultureInfo.InvariantCulture,
                "Unexpected token '{0}' in PDF stream. The file may be corrupted. " +
                "If you think this is a bug in PDFsharp, please send us your PDF file.", token);
            ThrowParserException(message);
        }
    }

    internal static class ContentReaderDiagnostics
    {
        public static void ThrowContentReaderException(string message)
        {
            throw new ContentReaderException(message);
        }

        public static void ThrowContentReaderException(string message, Exception innerException)
        {
            throw new ContentReaderException(message, innerException);
        }

        public static void ThrowNumberOutOfIntegerRange(long value)
        {
            string message = string.Format(CultureInfo.InvariantCulture, "Number '{0}' out of integer range.", value);
            ThrowContentReaderException(message);
        }

        public static void HandleUnexpectedCharacter(char ch)
        {
            string message = string.Format(CultureInfo.InvariantCulture,
                "Unexpected character '0x{0:x4}' in content stream. The stream may be corrupted or the feature is not implemented. " +
                "If you think this is a bug in PDFsharp, please send us your PDF file.", ch);
            ThrowContentReaderException(message);
        }
    }
}
