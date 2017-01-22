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

// ReSharper disable InconsistentNaming

namespace PdfSharp.Pdf.Content
{
    /// <summary>
    /// Character table by name. Same as PdfSharp.Pdf.IO.Chars. Not yet clear if necessary.
    /// </summary>
    internal static class Chars
    {
        public const char EOF = (char)65535; //unchecked((char)(-1));
        public const char NUL = '\0';   // EOF
        public const char CR = '\x0D'; // ignored by lexer
        public const char LF = '\x0A'; // Line feed
        public const char BEL = '\a';   // Bell
        public const char BS = '\b';   // Backspace
        public const char FF = '\f';   // Form feed
        public const char HT = '\t';   // Horizontal tab
        public const char VT = '\v';   // Vertical tab
        public const char NonBreakableSpace = (char)160;  // char(160)

        // The following names come from "PDF Reference Third Edition"
        // Appendix D.1, Latin Character Set and Encoding
        public const char SP = ' ';
        public const char QuoteDbl = '"';
        public const char QuoteSingle = '\'';
        public const char ParenLeft = '(';
        public const char ParenRight = ')';
        public const char BraceLeft = '{';
        public const char BraceRight = '}';
        public const char BracketLeft = '[';
        public const char BracketRight = ']';
        public const char Less = '<';
        public const char Greater = '>';
        public const char Equal = '=';
        public const char Period = '.';
        public const char Semicolon = ';';
        public const char Colon = ':';
        public const char Slash = '/';
        public const char Bar = '|';
        public const char BackSlash = '\\';
        public const char Percent = '%';
        public const char Dollar = '$';
        public const char At = '@';
        public const char NumberSign = '#';
        public const char Asterisk = '*';
        public const char Question = '?';
        public const char Hyphen = '-';  // char(45)
        public const char SoftHyphen = '­';  // char(173)
        public const char Currency = '¤';
    }
}
