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
using System.Diagnostics;
using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace PdfSharp.Internal
{
    /// <summary>
    /// A bunch of internal helper functions.
    /// </summary>
    internal static class DiagnosticsHelper
    {
        public static void HandleNotImplemented(string message)
        {
            string text = "Not implemented: " + message;
            switch (Diagnostics.NotImplementedBehaviour)
            {
                case NotImplementedBehaviour.DoNothing:
                    break;

                case NotImplementedBehaviour.Log:
                    Logger.Log(text);
                    break;

                case NotImplementedBehaviour.Throw:
                    ThrowNotImplementedException(text);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Indirectly throws NotImplementedException.
        /// Required because PDFsharp Release builds tread warnings as errors and
        /// throwing NotImplementedException may lead to unreachable code which
        /// crashes the build.
        /// </summary>
        public static void ThrowNotImplementedException(string message)
        {
            throw new NotImplementedException(message);
        }
    }

    internal static class Logger
    {
        public static void Log(string format, params object[] args)
        {
            Debug.WriteLine("Log...");
        }
    }

    class Logging
    {

    }

    class Tracing
    {
        [Conditional("DEBUG")]
        public void Foo()
        { }
    }

    /// <summary>
    /// Helper class around the Debugger class.
    /// </summary>
    public static class DebugBreak
    {
        /// <summary>
        /// Call Debugger.Break() if a debugger is attached.
        /// </summary>
        public static void Break()
        {
            Break(false);
        }

        /// <summary>
        /// Call Debugger.Break() if a debugger is attached or when always is set to true.
        /// </summary>
        public static void Break(bool always)
        {
            if (always || Debugger.IsAttached)
                Debugger.Break();
        }
    }

    /// <summary>
    /// Internal stuff for development of PDFsharp.
    /// </summary>
    public static class FontsDevHelper
    {
        /// <summary>
        /// Creates font and enforces bold/italic simulation.
        /// </summary>
        public static XFont CreateSpecialFont(string familyName, double emSize, XFontStyle style,
            XPdfFontOptions pdfOptions, XStyleSimulations styleSimulations)
        {
            return new XFont(familyName, emSize, style, pdfOptions, styleSimulations);
        }

        /// <summary>
        /// Dumps the font caches to a string.
        /// </summary>
        public static string GetFontCachesState()
        {
            return FontFactory.GetFontCachesState();
        }
    }
}
