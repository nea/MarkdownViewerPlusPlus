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

// ReSharper disable ConvertToAutoProperty

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Holds information how to handle the document when it is saved as PDF stream.
    /// </summary>
    public sealed class PdfDocumentOptions
    {
        internal PdfDocumentOptions(PdfDocument document)
        {
            //_deflateContents = true;
            //_writeProcedureSets = true;
        }

        /// <summary>
        /// Gets or sets the color mode.
        /// </summary>
        public PdfColorMode ColorMode
        {
            get { return _colorMode; }
            set { _colorMode = value; }
        }
        PdfColorMode _colorMode = PdfColorMode.Rgb;

        /// <summary>
        /// Gets or sets a value indicating whether to compress content streams of PDF pages.
        /// </summary>
        public bool CompressContentStreams
        {
            get { return _compressContentStreams; }
            set { _compressContentStreams = value; }
        }
#if DEBUG
        bool _compressContentStreams = false;
#else
        bool _compressContentStreams = true;
#endif

        /// <summary>
        /// Gets or sets a value indicating that all objects are not compressed.
        /// </summary>
        public bool NoCompression
        {
            get { return _noCompression; }
            set { _noCompression = value; }
        }
        bool _noCompression;

        /// <summary>
        /// Gets or sets the flate encode mode. Besides the balanced default mode you can set modes for best compression (slower) or best speed (larger files).
        /// </summary>
        public PdfFlateEncodeMode FlateEncodeMode
        {
            get { return _flateEncodeMode; }
            set { _flateEncodeMode = value; }
        }
        PdfFlateEncodeMode _flateEncodeMode = PdfFlateEncodeMode.Default;

        /// <summary>
        /// Gets or sets a value indicating whether to compress bilevel images using CCITT compression.
        /// With true, PDFsharp will try FlateDecode CCITT and will use the smallest one or a combination of both.
        /// With false, PDFsharp will always use FlateDecode only - files may be a few bytes larger, but file creation is faster.
        /// </summary>
        public bool EnableCcittCompressionForBilevelImages
        {
            get { return _enableCcittCompressionForBilevelImages; }
            set { _enableCcittCompressionForBilevelImages = value; }
        }
        bool _enableCcittCompressionForBilevelImages = false;

        /// <summary>
        /// Gets or sets a value indicating whether to compress JPEG images with the FlateDecode filter.
        /// </summary>
        public PdfUseFlateDecoderForJpegImages UseFlateDecoderForJpegImages
        {
            get { return _useFlateDecoderForJpegImages; }
            set { _useFlateDecoderForJpegImages = value; }
        }
        PdfUseFlateDecoderForJpegImages _useFlateDecoderForJpegImages = PdfUseFlateDecoderForJpegImages.Never;
    }
}