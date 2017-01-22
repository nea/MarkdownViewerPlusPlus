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

using System.IO;

namespace PdfSharp.Fonts
{
    /// <summary>
    /// Represents a writer for generation of font file streams. 
    /// </summary>
    internal class FontWriter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontWriter"/> class.
        /// Data is written in Motorola format (big-endian).
        /// </summary>
        public FontWriter(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Closes the writer and, if specified, the underlying stream.
        /// </summary>
        public void Close(bool closeUnderlyingStream)
        {
            if (_stream != null && closeUnderlyingStream)
            {
#if !UWP
                _stream.Close();
#endif
                _stream.Dispose();
            }
            _stream = null;
        }

        /// <summary>
        /// Closes the writer and the underlying stream.
        /// </summary>
        public void Close()
        {
            Close(true);
        }

        /// <summary>
        /// Gets or sets the position within the stream.
        /// </summary>
        public int Position
        {
            get { return (int)_stream.Position; }
            set { _stream.Position = value; }
        }

        /// <summary>
        /// Writes the specified value to the font stream.
        /// </summary>
        public void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        /// Writes the specified value to the font stream.
        /// </summary>
        public void WriteByte(int value)
        {
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes the specified value to the font stream using big-endian.
        /// </summary>
        public void WriteShort(short value)
        {
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes the specified value to the font stream using big-endian.
        /// </summary>
        public void WriteShort(int value)
        {
            WriteShort((short)value);
        }

        /// <summary>
        /// Writes the specified value to the font stream using big-endian.
        /// </summary>
        public void WriteUShort(ushort value)
        {
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes the specified value to the font stream using big-endian.
        /// </summary>
        public void WriteUShort(int value)
        {
            WriteUShort((ushort)value);
        }

        /// <summary>
        /// Writes the specified value to the font stream using big-endian.
        /// </summary>
        public void WriteInt(int value)
        {
            _stream.WriteByte((byte)(value >> 24));
            _stream.WriteByte((byte)(value >> 16));
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes the specified value to the font stream using big-endian.
        /// </summary>
        public void WriteUInt(uint value)
        {
            _stream.WriteByte((byte)(value >> 24));
            _stream.WriteByte((byte)(value >> 16));
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)value);
        }

        //public short ReadFWord()
        //public ushort ReadUFWord()
        //public long ReadLongDate()
        //public string ReadString(int size)

        public void Write(byte[] buffer)
        {
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Gets the underlying stream.
        /// </summary>
        internal Stream Stream
        {
            get { return _stream; }
        }
        Stream _stream;
    }
}
