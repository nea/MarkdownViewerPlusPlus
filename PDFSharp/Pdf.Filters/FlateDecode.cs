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
using System.IO;
using PdfSharp.Internal;
#if NET_ZIP
using System.IO.Compression;
#else
using PdfSharp.SharpZipLib.Zip.Compression;
using PdfSharp.SharpZipLib.Zip.Compression.Streams;
#endif

namespace PdfSharp.Pdf.Filters
{
    /// <summary>
    /// Implements the FlateDecode filter by wrapping SharpZipLib.
    /// </summary>
    public class FlateDecode : Filter
    {
        // Reference: 3.3.3  LZWDecode and FlateDecode Filters / Page 71

        /// <summary>
        /// Encodes the specified data.
        /// </summary>
        public override byte[] Encode(byte[] data)
        {
            return Encode(data, PdfFlateEncodeMode.Default);
        }

        /// <summary>
        /// Encodes the specified data.
        /// </summary>
        public byte[] Encode(byte[] data, PdfFlateEncodeMode mode)
        {
            MemoryStream ms = new MemoryStream();

            // DeflateStream/GZipStream does not work immediately and I have not the leisure to work it out.
            // So I keep on using SharpZipLib even with .NET 2.0.
#if NET_ZIP
            // See http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=97064
            // 
            // Excerpt from the RFC 1950 specs for first byte:
            //
            // CMF (Compression Method and flags)
            //    This byte is divided into a 4-bit compression method and a 4-
            //    bit information field depending on the compression method.
            //
            //      bits 0 to 3  CM     Compression method
            //      bits 4 to 7  CINFO  Compression info
            //
            // CM (Compression method)
            //    This identifies the compression method used in the file. CM = 8
            //    denotes the "deflate" compression method with a window size up
            //    to 32K.  This is the method used by gzip and PNG (see
            //    references [1] and [2] in Chapter 3, below, for the reference
            //    documents).  CM = 15 is reserved.  It might be used in a future
            //    version of this specification to indicate the presence of an
            //    extra field before the compressed data.
            //
            // CINFO (Compression info)
            //    For CM = 8, CINFO is the base-2 logarithm of the LZ77 window
            //    size, minus eight (CINFO=7 indicates a 32K window size). Values
            //    of CINFO above 7 are not allowed in this version of the
            //    specification.  CINFO is not defined in this specification for
            //    CM not equal to 8.
            ms.WriteByte(0x78);

            // Excerpt from the RFC 1950 specs for second byte:
            //
            // FLG (FLaGs)
            //    This flag byte is divided as follows:
            //
            //       bits 0 to 4  FCHECK  (check bits for CMF and FLG)
            //       bit  5       FDICT   (preset dictionary)
            //       bits 6 to 7  FLEVEL  (compression level)
            //
            //    The FCHECK value must be such that CMF and FLG, when viewed as
            //    a 16-bit unsigned integer stored in MSB order (CMF*256 + FLG),
            //    is a multiple of 31.
            //
            // FDICT (Preset dictionary)
            //    If FDICT is set, a DICT dictionary identifier is present
            //    immediately after the FLG byte. The dictionary is a sequence of
            //    bytes which are initially fed to the compressor without
            //    producing any compressed output. DICT is the Adler-32 checksum
            //    of this sequence of bytes (see the definition of ADLER32
            //    below).  The decompressor can use this identifier to determine
            //    which dictionary has been used by the compressor.
            //
            // FLEVEL (Compression level)
            //    These flags are available for use by specific compression
            //    methods.  The "deflate" method (CM = 8) sets these flags as
            //    follows:
            //
            //       0 - compressor used fastest algorithm
            //       1 - compressor used fast algorithm
            //       2 - compressor used default algorithm
            //       3 - compressor used maximum compression, slowest algorithm
            //
            //    The information in FLEVEL is not needed for decompression; it
            //    is there to indicate if recompression might be worthwhile.
            ms.WriteByte(0x49);

            DeflateStream zip = new DeflateStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
#else
            int level = Deflater.DEFAULT_COMPRESSION;
            switch (mode)
            {
                case PdfFlateEncodeMode.BestCompression:
                    level = Deflater.BEST_COMPRESSION;
                    break;
                case PdfFlateEncodeMode.BestSpeed:
                    level = Deflater.BEST_SPEED;
                    break;
            }
            DeflaterOutputStream zip = new DeflaterOutputStream(ms, new Deflater(level, false));
            zip.Write(data, 0, data.Length);
            zip.Finish();
#endif
#if !NETFX_CORE && !UWP
            ms.Capacity = (int)ms.Length;
            return ms.GetBuffer();
#else
            return ms.ToArray();
#endif
        }

        /// <summary>
        /// Decodes the specified data.
        /// </summary>
        public override byte[] Decode(byte[] data, FilterParms parms)
        {
            MemoryStream msInput = new MemoryStream(data);
            MemoryStream msOutput = new MemoryStream();
#if NET_ZIP
            // See http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=97064
            // It seems to work when skipping the first two bytes.
            byte header;   // 0x30 0x59
            header = (byte)msInput.ReadByte();
            //Debug.Assert(header == 48);
            header = (byte)msInput.ReadByte();
            //Debug.Assert(header == 89);
            DeflateStream zip = new DeflateStream(msInput, CompressionMode.Decompress, true);
            int cbRead;
            byte[] abResult = new byte[1024];
            do
            {
                cbRead = zip.Read(abResult, 0, abResult.Length);
                if (cbRead > 0)
                    msOutput.Write(abResult, 0, cbRead);
            }
            while (cbRead > 0);
            zip.Close();
            msOutput.Flush();
            if (msOutput.Length >= 0)
            {
                msOutput.Capacity = (int)msOutput.Length;
                return msOutput.GetBuffer();
            }
            return null;
#else
            InflaterInputStream iis = new InflaterInputStream(msInput, new Inflater(false));
            int cbRead;
            byte[] abResult = new byte[32768];
            do
            {
                cbRead = iis.Read(abResult, 0, abResult.Length);
                if (cbRead > 0)
                    msOutput.Write(abResult, 0, cbRead);
            }
            while (cbRead > 0);
#if UWP
            iis.Dispose();
#else
            iis.Close();
#endif
            msOutput.Flush();
            if (msOutput.Length >= 0)
            {
#if NETFX_CORE || UWP
                return msOutput.ToArray();
#else
                msOutput.Capacity = (int)msOutput.Length;
                return msOutput.GetBuffer();
#endif
            }
            return null;
#endif
        }
    }
}
