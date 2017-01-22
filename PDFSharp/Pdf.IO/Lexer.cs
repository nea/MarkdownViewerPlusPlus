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
using System.Diagnostics;
using System.Text;
using System.IO;
using PdfSharp.Internal;
using PdfSharp.Pdf.Internal;

#pragma warning disable 1591

namespace PdfSharp.Pdf.IO
{
    /// <summary>
    /// Lexical analyzer for PDF files. Technically a PDF file is a stream of bytes. Some chunks
    /// of bytes represent strings in several encodings. The actual encoding depends on the
    /// context where the string is used. Therefore the bytes are 'raw encoded' into characters,
    /// i.e. a character or token read by the lexer has always character values in the range from
    /// 0 to 255.
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// Initializes a new instance of the Lexer class.
        /// </summary>
        public Lexer(Stream pdfInputStream)
        {
            _pdfSteam = pdfInputStream;
            _pdfLength = (int)_pdfSteam.Length;
            _idxChar = 0;
            Position = 0;
        }

        /// <summary>
        /// Gets or sets the position within the PDF stream.
        /// </summary>
        public int Position
        {
            get { return _idxChar; }
            set
            {
                _idxChar = value;
                _pdfSteam.Position = value;
                // ReadByte return -1 (eof) at the end of the stream.
                _currChar = (char)_pdfSteam.ReadByte();
                _nextChar = (char)_pdfSteam.ReadByte();
                _token = new StringBuilder();
            }
        }

        /// <summary>
        /// Reads the next token and returns its type. If the token starts with a digit, the parameter
        /// testReference specifies how to treat it. If it is false, the lexer scans for a single integer.
        /// If it is true, the lexer checks if the digit is the prefix of a reference. If it is a reference,
        /// the token is set to the object ID followed by the generation number separated by a blank
        /// (the 'R' is omitted from the token).
        /// </summary>
        // /// <param name="testReference">Indicates whether to test the next token if it is a reference.</param>
        public Symbol ScanNextToken()
        {
        Again:
            _token = new StringBuilder();

            char ch = MoveToNonWhiteSpace();
            switch (ch)
            {
                case '%':
                    // Eat comments, the parser doesn't handle them
                    //return symbol = ScanComment();
                    ScanComment();
                    goto Again;

                case '/':
                    return _symbol = ScanName();

                //case 'R':
                //  if (Lexer.IsWhiteSpace(nextChar))
                //  {
                //    ScanNextChar();
                //    return Symbol.R;
                //  }
                //  break;

                case '+': //TODO is it so easy?
                case '-':
                    return _symbol = ScanNumber();

                case '(':
                    return _symbol = ScanLiteralString();

                case '[':
                    ScanNextChar(true);
                    return _symbol = Symbol.BeginArray;

                case ']':
                    ScanNextChar(true);
                    return _symbol = Symbol.EndArray;

                case '<':
                    if (_nextChar == '<')
                    {
                        ScanNextChar(true);
                        ScanNextChar(true);
                        return _symbol = Symbol.BeginDictionary;
                    }
                    return _symbol = ScanHexadecimalString();

                case '>':
                    if (_nextChar == '>')
                    {
                        ScanNextChar(true);
                        ScanNextChar(true);
                        return _symbol = Symbol.EndDictionary;
                    }
                    ParserDiagnostics.HandleUnexpectedCharacter(_nextChar);
                    break;

                case '.':
                    return _symbol = ScanNumber();
            }
            if (char.IsDigit(ch))
#if true_
                return ScanNumberOrReference();
#else
                if (PeekReference())
                    return _symbol = ScanNumber();
                else
                    return _symbol = ScanNumber();
#endif

            if (char.IsLetter(ch))
                return _symbol = ScanKeyword();

            if (ch == Chars.EOF)
                return _symbol = Symbol.Eof;

            // #???

            ParserDiagnostics.HandleUnexpectedCharacter(ch);
            return _symbol = Symbol.None;
        }

        /// <summary>
        /// Reads the raw content of a stream.
        /// </summary>
        public byte[] ReadStream(int length)
        {
            int pos;

            // Skip illegal blanks behind «stream».
            while (_currChar == Chars.SP)
                ScanNextChar(true);

            // Skip new line behind «stream».
            if (_currChar == Chars.CR)
            {
                if (_nextChar == Chars.LF)
                    pos = _idxChar + 2;
                else
                    pos = _idxChar + 1;
            }
            else
                pos = _idxChar + 1;

            _pdfSteam.Position = pos;
            byte[] bytes = new byte[length];
            int read = _pdfSteam.Read(bytes, 0, length);
            Debug.Assert(read == length);

            // Synchronize idxChar etc.
            Position = pos + length;
            return bytes;
        }

        /// <summary>
        /// Reads a string in raw encoding.
        /// </summary>
        public String ReadRawString(int position, int length)
        {
            _pdfSteam.Position = position;
            byte[] bytes = new byte[length];
            _pdfSteam.Read(bytes, 0, length);
            return PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Scans a comment line.
        /// </summary>
        public Symbol ScanComment()
        {
            Debug.Assert(_currChar == Chars.Percent);

            _token = new StringBuilder();
            while (true)
            {
                char ch = AppendAndScanNextChar();
                if (ch == Chars.LF || ch == Chars.EOF)
                    break;
            }
            // TODO: not correct
            if (_token.ToString().StartsWith("%%EOF"))
                return Symbol.Eof;
            return _symbol = Symbol.Comment;
        }

        /// <summary>
        /// Scans a name.
        /// </summary>
        public Symbol ScanName()
        {
            Debug.Assert(_currChar == Chars.Slash);

            _token = new StringBuilder();
            while (true)
            {
                char ch = AppendAndScanNextChar();
                if (IsWhiteSpace(ch) || IsDelimiter(ch) || ch == Chars.EOF)
                    return _symbol = Symbol.Name;

                if (ch == '#')
                {
                    ScanNextChar(true);
                    char[] hex = new char[2];
                    hex[0] = _currChar;
                    hex[1] = _nextChar;
                    ScanNextChar(true);
                    // TODO Check syntax
                    ch = (char)(ushort)int.Parse(new string(hex), NumberStyles.AllowHexSpecifier);
                    _currChar = ch;
                }
            }
        }

        /// <summary>
        /// Scans a number.
        /// </summary>
        public Symbol ScanNumber()
        {
            // I found a PDF file created with Acrobat 7 with this entry 
            //   /Checksum 2996984786
            // What is this? It is neither an integer nor a real.
            // I introduced an UInteger...
            bool period = false;
            //bool sign;

            _token = new StringBuilder();
            char ch = _currChar;
            if (ch == '+' || ch == '-')
            {
                //sign = true;
                _token.Append(ch);
                ch = ScanNextChar(true);
            }
            while (true)
            {
                if (char.IsDigit(ch))
                {
                    _token.Append(ch);
                }
                else if (ch == '.')
                {
                    if (period)
                        ParserDiagnostics.ThrowParserException("More than one period in number.");

                    period = true;
                    _token.Append(ch);
                }
                else
                    break;
                ch = ScanNextChar(true);
            }

            if (period)
                return Symbol.Real;
            long l = Int64.Parse(_token.ToString(), CultureInfo.InvariantCulture);
            if (l >= Int32.MinValue && l <= Int32.MaxValue)
                return Symbol.Integer;
            if (l > 0 && l <= UInt32.MaxValue)
                return Symbol.UInteger;

            // Got an AutoCAD PDF file that contains this: /C 264584027963392
            // Best we can do is to convert it to real value.
            return Symbol.Real;
            //thr ow new PdfReaderException("Number exceeds integer range.");
        }

        public Symbol ScanNumberOrReference()
        {
            Symbol result = ScanNumber();
            if (result == Symbol.Integer)
            {
                int pos = Position;
                string objectNumber = Token;
            }
            return result;
        }

        /// <summary>
        /// Scans a keyword.
        /// </summary>
        public Symbol ScanKeyword()
        {
            _token = new StringBuilder();
            char ch = _currChar;
            // Scan token
            while (true)
            {
                if (char.IsLetter(ch))
                    _token.Append(ch);
                else
                    break;
                ch = ScanNextChar(false);
            }

            // Check known tokens.
            switch (_token.ToString())
            {
                case "obj":
                    return _symbol = Symbol.Obj;

                case "endobj":
                    return _symbol = Symbol.EndObj;

                case "null":
                    return _symbol = Symbol.Null;

                case "true":
                case "false":
                    return _symbol = Symbol.Boolean;

                case "R":
                    return _symbol = Symbol.R;

                case "stream":
                    return _symbol = Symbol.BeginStream;

                case "endstream":
                    return _symbol = Symbol.EndStream;

                case "xref":
                    return _symbol = Symbol.XRef;

                case "trailer":
                    return _symbol = Symbol.Trailer;

                case "startxref":
                    return _symbol = Symbol.StartXRef;
            }

            // Anything else is treated as a keyword. Samples are f or n in iref.
            return _symbol = Symbol.Keyword;
        }

        /// <summary>
        /// Scans a literal string, contained between "(" and ")".
        /// </summary>
        public Symbol ScanLiteralString()
        {
            // Reference: 3.2.3  String Objects / Page 53
            // Reference: TABLE 3.32  String Types / Page 157

            Debug.Assert(_currChar == Chars.ParenLeft);
            _token = new StringBuilder();
            int parenLevel = 0;
            char ch = ScanNextChar(false);

            // Phase 1: deal with escape characters.
            while (ch != Chars.EOF)
            {
                switch (ch)
                {
                    case '(':
                        parenLevel++;
                        break;

                    case ')':
                        if (parenLevel == 0)
                        {
                            ScanNextChar(false);
                            // Is goto evil? We could move Phase 2 code here or create a subroutine for Phase 1.
                            goto Phase2;
                        }
                        parenLevel--;
                        break;

                    case '\\':
                        {
                            ch = ScanNextChar(false);
                            switch (ch)
                            {
                                case 'n':
                                    ch = Chars.LF;
                                    break;

                                case 'r':
                                    ch = Chars.CR;
                                    break;

                                case 't':
                                    ch = Chars.HT;
                                    break;

                                case 'b':
                                    ch = Chars.BS;
                                    break;

                                case 'f':
                                    ch = Chars.FF;
                                    break;

                                case '(':
                                    ch = Chars.ParenLeft;
                                    break;

                                case ')':
                                    ch = Chars.ParenRight;
                                    break;

                                case '\\':
                                    ch = Chars.BackSlash;
                                    break;

                                // AutoCAD PDFs my contain such strings: (\ ) 
                                case ' ':
                                    ch = ' ';
                                    break;

                                case Chars.CR:
                                case Chars.LF:
                                    ch = ScanNextChar(false);
                                    continue;

                                default:
                                    if (char.IsDigit(ch))  // First octal character.
                                    {
                                        // Octal character code.
                                        if (ch >= '8')
                                            ParserDiagnostics.HandleUnexpectedCharacter(ch);

                                        int n = ch - '0';
                                        if (char.IsDigit(_nextChar))  // Second octal character.
                                        {
                                            ch = ScanNextChar(false);
                                            if (ch >= '8')
                                                ParserDiagnostics.HandleUnexpectedCharacter(ch);

                                            n = n * 8 + ch - '0';
                                            if (char.IsDigit(_nextChar))  // Third octal character.
                                            {
                                                ch = ScanNextChar(false);
                                                if (ch >= '8')
                                                    ParserDiagnostics.HandleUnexpectedCharacter(ch);

                                                n = n * 8 + ch - '0';
                                            }
                                        }
                                        ch = (char)n;
                                    }
                                    else
                                    {
                                        //TODO
                                        // Debug.As sert(false, "Not implemented; unknown escape character.");
                                        ParserDiagnostics.HandleUnexpectedCharacter(ch);
                                    }
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }

                _token.Append(ch);
                ch = ScanNextChar(false);
            }

            // Phase 2: deal with UTF-16BE if necessary.
            // UTF-16BE Unicode strings start with U+FEFF ("þÿ"). There can be empty strings with UTF-16BE prefix.
            Phase2:
            if (_token.Length >= 2 && _token[0] == '\xFE' && _token[1] == '\xFF')
            {
                // Combine two ANSI characters to get one Unicode character.
                StringBuilder temp = _token;
                int length = temp.Length;
                if ((length & 1) == 1)
                {
                    // TODO What does the PDF Reference say about this case? Assume (char)0 or treat the file as corrupted?
                    temp.Append(0);
                    ++length;
                    DebugBreak.Break();
                }
                _token = new StringBuilder();
                for (int i = 2; i < length; i += 2)
                {
                    _token.Append((char)(256 * temp[i] + temp[i + 1]));
                }
                return _symbol = Symbol.UnicodeString;
            }
            // Adobe Reader also supports UTF-16LE.
            if (_token.Length >= 2 && _token[0] == '\xFF' && _token[1] == '\xFE')
            {
                // Combine two ANSI characters to get one Unicode character.
                StringBuilder temp = _token;
                int length = temp.Length;
                if ((length & 1) == 1)
                {
                    // TODO What does the PDF Reference say about this case? Assume (char)0 or treat the file as corrupted?
                    temp.Append(0);
                    ++length;
                    DebugBreak.Break();
                }
                _token = new StringBuilder();
                for (int i = 2; i < length; i += 2)
                {
                    _token.Append((char)(256 * temp[i + 1] + temp[i]));
                }
                return _symbol = Symbol.UnicodeString;
            }
            return _symbol = Symbol.String;
        }

        public Symbol ScanHexadecimalString()
        {
            Debug.Assert(_currChar == Chars.Less);

            _token = new StringBuilder();
            char[] hex = new char[2];
            ScanNextChar(true);
            while (true)
            {
                MoveToNonWhiteSpace();
                if (_currChar == '>')
                {
                    ScanNextChar(true);
                    break;
                }
                if (char.IsLetterOrDigit(_currChar))
                {
                    hex[0] = char.ToUpper(_currChar);
                    hex[1] = char.ToUpper(_nextChar);
                    int ch = int.Parse(new string(hex), NumberStyles.AllowHexSpecifier);
                    _token.Append(Convert.ToChar(ch));
                    ScanNextChar(true);
                    ScanNextChar(true);
                }
            }
            string chars = _token.ToString();
            int count = chars.Length;
            if (count > 2 && chars[0] == (char)0xFE && chars[1] == (char)0xFF)
            {
                Debug.Assert(count % 2 == 0);
                _token.Length = 0;
                for (int idx = 2; idx < count; idx += 2)
                    _token.Append((char)(chars[idx] * 256 + chars[idx + 1]));
                return _symbol = Symbol.UnicodeHexString;
            }
            return _symbol = Symbol.HexString;
        }

        /// <summary>
        /// Move current position one character further in PDF stream.
        /// </summary>
        internal char ScanNextChar(bool handleCRLF)
        {
            if (_pdfLength <= _idxChar)
            {
                _currChar = Chars.EOF;
                _nextChar = Chars.EOF;
            }
            else
            {
                _currChar = _nextChar;
                _nextChar = (char)_pdfSteam.ReadByte();
                _idxChar++;
                if (handleCRLF && _currChar == Chars.CR)
                {
                    if (_nextChar == Chars.LF)
                    {
                        // Treat CR LF as LF.
                        _currChar = _nextChar;
                        _nextChar = (char)_pdfSteam.ReadByte();
                        _idxChar++;
                    }
                    else
                    {
                        // Treat single CR as LF.
                        _currChar = Chars.LF;
                    }
                }
            }
            return _currChar;
        }

        ///// <summary>
        ///// Resets the current token to the empty string.
        ///// </summary>
        //void ClearToken()
        //{
        //    _token.Length = 0;
        //}

        bool PeekReference()
        {
            // A Reference has the form "nnn mmm R". The implementation of the the parser used a
            // reduce/shift algorithm in the first place. But this case is the only one we need to
            // look ahead 3 tokens. 
            int positon = Position;

            // Skip digits.
            while (char.IsDigit(_currChar))
                ScanNextChar(true);

            // Space expected.
            if (_currChar != Chars.SP)
                goto False;

            // Skip spaces.
            while (_currChar == Chars.SP)
                ScanNextChar(true);

            // Digit expected.
            if (!char.IsDigit(_currChar))
                goto False;

            // Skip digits.
            while (char.IsDigit(_currChar))
                ScanNextChar(true);

            // Space expected.
            if (_currChar != Chars.SP)
                goto False;

            // Skip spaces.
            while (_currChar == Chars.SP)
                ScanNextChar(true);

            // "R" expected.
            // We can ignore _nextChar because there is no other valid token that starts with an 'R'.
            if (_currChar != 'R')
                goto False;

            Position = positon;
            return true;

        False:
            Position = positon;
            return false;
        }

        /// <summary>
        /// Appends current character to the token and reads next one.
        /// </summary>
        internal char AppendAndScanNextChar()
        {
            if (_currChar == Chars.EOF)
                ParserDiagnostics.ThrowParserException("Undetected EOF reached.");

            _token.Append(_currChar);
            return ScanNextChar(true);
        }

        /// <summary>
        /// If the current character is not a white space, the function immediately returns it.
        /// Otherwise the PDF cursor is moved forward to the first non-white space or EOF.
        /// White spaces are NUL, HT, LF, FF, CR, and SP.
        /// </summary>
        public char MoveToNonWhiteSpace()
        {
            while (_currChar != Chars.EOF)
            {
                switch (_currChar)
                {
                    case Chars.NUL:
                    case Chars.HT:
                    case Chars.LF:
                    case Chars.FF:
                    case Chars.CR:
                    case Chars.SP:
                        ScanNextChar(true);
                        break;

                    case (char)11:
                    case (char)173:
                        ScanNextChar(true);
                        break;


                    default:
                        return _currChar;
                }
            }
            return _currChar;
        }

#if DEBUG
        public string SurroundingsOfCurrentPosition(bool hex)
        {
            const int range = 20;
            int start = Math.Max(Position - range, 0);
            int length = Math.Min(2 * range, PdfLength - start);
            long posOld = _pdfSteam.Position;
            _pdfSteam.Position = start;
            byte[] bytes = new byte[length];
            _pdfSteam.Read(bytes, 0, length);
            _pdfSteam.Position = posOld;
            string result = "";
            if (hex)
            {
                for (int idx = 0; idx < length; idx++)
                    result += ((int)bytes[idx]).ToString("x2");
                //result += string.Format("{0:", (int) bytes[idx]);
            }
            else
            {
                for (int idx = 0; idx < length; idx++)
                    result += (char)bytes[idx];
            }
            return result;
        }
#endif

        /// <summary>
        /// Gets the current symbol.
        /// </summary>
        public Symbol Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        /// <summary>
        /// Gets the current token.
        /// </summary>
        public string Token
        {
            get { return _token.ToString(); }
        }

        /// <summary>
        /// Interprets current token as boolean literal.
        /// </summary>
        public bool TokenToBoolean
        {
            get
            {
                Debug.Assert(_token.ToString() == "true" || _token.ToString() == "false");
                return _token.ToString()[0] == 't';
            }
        }

        /// <summary>
        /// Interprets current token as integer literal.
        /// </summary>
        public int TokenToInteger
        {
            get
            {
                //Debug.As sert(_token.ToString().IndexOf('.') == -1);
                return int.Parse(_token.ToString(), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Interprets current token as unsigned integer literal.
        /// </summary>
        public uint TokenToUInteger
        {
            get
            {
                //Debug.As sert(_token.ToString().IndexOf('.') == -1);
                return uint.Parse(_token.ToString(), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Interprets current token as real or integer literal.
        /// </summary>
        public double TokenToReal
        {
            get { return double.Parse(_token.ToString(), CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Interprets current token as object ID.
        /// </summary>
        public PdfObjectID TokenToObjectID
        {
            get
            {
                string[] numbers = Token.Split('|');
                int objectNumber = Int32.Parse(numbers[0]);
                int generationNumber = Int32.Parse(numbers[1]);
                return new PdfObjectID(objectNumber, generationNumber);
            }
        }

        /// <summary>
        /// Indicates whether the specified character is a PDF white-space character.
        /// </summary>
        internal static bool IsWhiteSpace(char ch)
        {
            switch (ch)
            {
                case Chars.NUL:  // 0 Null
                case Chars.HT:   // 9 Horizontal Tab
                case Chars.LF:   // 10 Line Feed
                case Chars.FF:   // 12 Form Feed
                case Chars.CR:   // 13 Carriage Return
                case Chars.SP:   // 32 Space
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Indicates whether the specified character is a PDF delimiter character.
        /// </summary>
        internal static bool IsDelimiter(char ch)
        {
            switch (ch)
            {
                case '(':
                case ')':
                case '<':
                case '>':
                case '[':
                case ']':
                case '{':
                case '}':
                case '/':
                case '%':
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the length of the PDF output.
        /// </summary>
        public int PdfLength
        {
            get { return _pdfLength; }
        }

        readonly int _pdfLength;
        int _idxChar;
        char _currChar;
        char _nextChar;
        StringBuilder _token;
        Symbol _symbol = Symbol.None;

        readonly Stream _pdfSteam;
    }
}
