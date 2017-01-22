#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Thomas Hövel
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
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;

namespace PdfSharp.Drawing.Internal
{
    // $THHO THHO4THHO add support for PdfDocument.Options.
    internal class ImageImporterBmp : ImageImporterRoot, IImageImporter
    {
        public ImportedImage ImportImage(StreamReaderHelper stream, PdfDocument document)
        {
            try
            {
                stream.CurrentOffset = 0;
                int offsetImageData;
                if (TestBitmapFileHeader(stream, out offsetImageData))
                {
                    // Magic: TestBitmapFileHeader updates stream.CurrentOffset on success.

                    ImagePrivateDataBitmap ipd = new ImagePrivateDataBitmap(stream.Data, stream.Length);
                    ImportedImage ii = new ImportedImageBitmap(this, ipd, document);
                    if (TestBitmapInfoHeader(stream, ii, offsetImageData))
                    {
                        //stream.CurrentOffset = offsetImageData;
                        return ii;
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
            return null;
        }

        private bool TestBitmapFileHeader(StreamReaderHelper stream, out int offset)
        {
            offset = 0;
            // File must start with "BM".
            if (stream.GetWord(0, true) == 0x424d)
            {
                int filesize = (int)stream.GetDWord(2, false);
                // Integrity check: filesize set in BM header should match size of the stream.
                // We test "<" instead of "!=" to allow extra bytes at the end of the stream.
                if (filesize < stream.Length)
                    return false;

                offset = (int)stream.GetDWord(10, false);
                stream.CurrentOffset += 14;
                return true;
            }
            return false;
        }

        private bool TestBitmapInfoHeader(StreamReaderHelper stream, ImportedImage ii, int offset)
        {
            int size = (int)stream.GetDWord(0, false);
            if (size == 40 || size == 108 || size == 124) // sizeof BITMAPINFOHEADER == 40, sizeof BITMAPV4HEADER == 108, sizeof BITMAPV5HEADER == 124
            {
                uint width = stream.GetDWord(4, false);
                int height = (int)stream.GetDWord(8, false);
                int planes = stream.GetWord(12, false);
                int bitcount = stream.GetWord(14, false);
                int compression = (int)stream.GetDWord(16, false);
                int sizeImage = (int)stream.GetDWord(20, false);
                int xPelsPerMeter = (int)stream.GetDWord(24, false);
                int yPelsPerMeter = (int)stream.GetDWord(28, false);
                uint colorsUsed = stream.GetDWord(32, false);
                uint colorsImportant = stream.GetDWord(36, false);
                // TODO Integrity and plausibility checks.
                if (sizeImage != 0 && sizeImage + offset > stream.Length)
                    return false;

                ImagePrivateDataBitmap privateData = (ImagePrivateDataBitmap)ii.Data;

                // Return true only for supported formats.
                if (compression == 0 || compression == 3) // BI_RGB == 0, BI_BITFIELDS == 3
                {
                    ((ImagePrivateDataBitmap)ii.Data).Offset = offset;
                    ((ImagePrivateDataBitmap)ii.Data).ColorPaletteOffset = stream.CurrentOffset + size;
                    ii.Information.Width = width;
                    ii.Information.Height = (uint)Math.Abs(height);
                    ii.Information.HorizontalDPM = xPelsPerMeter;
                    ii.Information.VerticalDPM = yPelsPerMeter;
                    privateData.FlippedImage = height < 0;
                    if (planes == 1 && bitcount == 24)
                    {
                        // RGB24
                        ii.Information.ImageFormat = ImageInformation.ImageFormats.RGB24;

                        // TODO: Verify Mask if size >= 108 && compression == 3.
                        return true;
                    }
                    if (planes == 1 && bitcount == 32)
                    {
                        // ARGB32
                        //ii.Information.ImageFormat = ImageInformation.ImageFormats.ARGB32;
                        ii.Information.ImageFormat = compression == 0 ?
                            ImageInformation.ImageFormats.RGB24 :
                            ImageInformation.ImageFormats.ARGB32;

                        // TODO: tell RGB from ARGB. Idea: assume RGB if alpha is always 0.

                        // TODO: Verify Mask if size >= 108 && compression == 3.
                        return true;
                    }
                    if (planes == 1 && bitcount == 8)
                    {
                        // Palette8
                        ii.Information.ImageFormat = ImageInformation.ImageFormats.Palette8;
                        ii.Information.ColorsUsed = colorsUsed;

                        return true;
                    }
                    if (planes == 1 && bitcount == 4)
                    {
                        // Palette8
                        ii.Information.ImageFormat = ImageInformation.ImageFormats.Palette4;
                        ii.Information.ColorsUsed = colorsUsed;

                        return true;
                    }
                    if (planes == 1 && bitcount == 1)
                    {
                        // Palette8
                        ii.Information.ImageFormat = ImageInformation.ImageFormats.Palette1;
                        ii.Information.ColorsUsed = colorsUsed;

                        return true;
                    }
                    // TODO Implement more formats!
                }
            }
            return false;
        }


        public ImageData PrepareImage(ImagePrivateData data)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Bitmap refers to the format used in PDF. Will be used for BMP, PNG, TIFF, GIF and others.
    /// </summary>
    internal class ImportedImageBitmap : ImportedImage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportedImageBitmap"/> class.
        /// </summary>
        public ImportedImageBitmap(IImageImporter importer, ImagePrivateDataBitmap data, PdfDocument document)
            : base(importer, data, document)
        { }

        internal override ImageData PrepareImageData()
        {
            ImagePrivateDataBitmap data = (ImagePrivateDataBitmap)Data;
            ImageDataBitmap imageData = new ImageDataBitmap(_document);
            //imageData.Data = data.Data;
            //imageData.Length = data.Length;

            data.CopyBitmap(imageData);

            return imageData;
        }
    }

    // THHO4THHO Maybe there will be derived classes for direct bitmaps vs. palettized bitmaps or so. Time will tell.

    /// <summary>
    /// Contains data needed for PDF. Will be prepared when needed.
    /// Bitmap refers to the format used in PDF. Will be used for BMP, PNG, TIFF, GIF and others.
    /// </summary>
    internal class ImageDataBitmap : ImageData
    {
        private ImageDataBitmap()
        {
        }

        internal ImageDataBitmap(PdfDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            internal set { _data = value; }
        }
        private byte[] _data;

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get { return _length; }
            internal set { _length = value; }
        }
        private int _length;

        /// <summary>
        /// Gets the data.
        /// </summary>
        public byte[] DataFax
        {
            get { return _dataFax; }
            internal set { _dataFax = value; }
        }
        private byte[] _dataFax;

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int LengthFax
        {
            get { return _lengthFax; }
            internal set { _lengthFax = value; }
        }
        private int _lengthFax;

        public byte[] AlphaMask
        {
            get { return _alphaMask; }
            internal set { _alphaMask = value; }
        }
        private byte[] _alphaMask;

        public int AlphaMaskLength
        {
            get { return _alphaMaskLength; }
            internal set { _alphaMaskLength = value; }
        }
        private int _alphaMaskLength;

        public byte[] BitmapMask
        {
            get { return _bitmapMask; }
            internal set { _bitmapMask = value; }
        }
        private byte[] _bitmapMask;

        public int BitmapMaskLength
        {
            get { return _bitmapMaskLength; }
            internal set { _bitmapMaskLength = value; }
        }
        private int _bitmapMaskLength;

        public byte[] PaletteData
        {
            get { return _paletteData; }
            set { _paletteData = value; }
        }
        private byte[] _paletteData;

        public int PaletteDataLength
        {
            get { return _paletteDataLength; }
            set { _paletteDataLength = value; }
        }
        private int _paletteDataLength;

        public bool SegmentedColorMask;

        public int IsBitonal;

        public int K;

        public bool IsGray;

        internal readonly PdfDocument _document;
    }

    /// <summary>
    /// Image data needed for PDF bitmap images.
    /// </summary>
    internal class ImagePrivateDataBitmap : ImagePrivateData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagePrivateDataBitmap"/> class.
        /// </summary>
        public ImagePrivateDataBitmap(byte[] data, int length)
        {
            _data = data;
            _length = length;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public byte[] Data
        {
            get { return _data; }
            //internal set { _data = value; }
        }
        private readonly byte[] _data;

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length
        {
            get { return _length; }
            //internal set { _length = value; }
        }
        private readonly int _length;

        /// <summary>
        /// True if first line is the top line, false if first line is the bottom line of the image. When needed, lines will be reversed while converting data into PDF format.
        /// </summary>
        internal bool FlippedImage;

        /// <summary>
        /// The offset of the image data in Data.
        /// </summary>
        internal int Offset;

        /// <summary>
        /// The offset of the color palette in Data.
        /// </summary>
        internal int ColorPaletteOffset;

        internal void CopyBitmap(ImageDataBitmap dest)
        {
            switch (Image.Information.ImageFormat)
            {
                case ImageInformation.ImageFormats.ARGB32:
                    CopyTrueColorMemoryBitmap(3, 8, true, dest);
                    break;

                case ImageInformation.ImageFormats.RGB24:
                    CopyTrueColorMemoryBitmap(4, 8, false, dest);
                    break;

                case ImageInformation.ImageFormats.Palette8:
                    CopyIndexedMemoryBitmap(8, dest);
                    break;

                case ImageInformation.ImageFormats.Palette4:
                    CopyIndexedMemoryBitmap(4, dest);
                    break;

                case ImageInformation.ImageFormats.Palette1:
                    CopyIndexedMemoryBitmap(1, dest);
                    break;



                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Copies images without color palette.
        /// </summary>
        /// <param name="components">4 (32bpp RGB), 3 (24bpp RGB, 32bpp ARGB)</param>
        /// <param name="bits">8</param>
        /// <param name="hasAlpha">true (ARGB), false (RGB)</param>
        /// <param name="dest">Destination </param>
        private void CopyTrueColorMemoryBitmap(int components, int bits, bool hasAlpha, ImageDataBitmap dest)
        {
            int width = (int)Image.Information.Width;
            int height = (int)Image.Information.Height;

            int logicalComponents = components;
            if (components == 4)
                logicalComponents = 3;

            byte[] imageData = new byte[components * width * height];

            bool hasMask = false;
            bool hasAlphaMask = false;
            byte[] alphaMask = hasAlpha ? new byte[width * height] : null;
            MonochromeMask mask = hasAlpha ?
              new MonochromeMask(width, height) : null;

            int nFileOffset = Offset;
            int nOffsetRead = 0;
            if (logicalComponents == 3)
            {
                for (int y = 0; y < height; ++y)
                {
                    // TODO Handle Flipped.
                    int nOffsetWrite = 3 * (height - 1 - y) * width;
                    int nOffsetWriteAlpha = 0;
                    if (hasAlpha)
                    {
                        mask.StartLine(y);
                        nOffsetWriteAlpha = (height - 1 - y) * width;
                    }

                    for (int x = 0; x < width; ++x)
                    {
                        imageData[nOffsetWrite] = Data[nFileOffset + nOffsetRead + 2];
                        imageData[nOffsetWrite + 1] = Data[nFileOffset + nOffsetRead + 1];
                        imageData[nOffsetWrite + 2] = Data[nFileOffset + nOffsetRead];
                        if (hasAlpha)
                        {
                            mask.AddPel(Data[nFileOffset + nOffsetRead + 3]);
                            alphaMask[nOffsetWriteAlpha] = Data[nFileOffset + nOffsetRead + 3];
                            if (!hasMask || !hasAlphaMask)
                            {
                                if (Data[nFileOffset + nOffsetRead + 3] != 255)
                                {
                                    hasMask = true;
                                    if (Data[nFileOffset + nOffsetRead + 3] != 0)
                                        hasAlphaMask = true;
                                }
                            }
                            ++nOffsetWriteAlpha;
                        }
                        nOffsetRead += hasAlpha ? 4 : components;
                        nOffsetWrite += 3;
                    }
                    nOffsetRead = 4 * ((nOffsetRead + 3) / 4); // Align to 32 bit boundary
                }
            }
            else if (components == 1)
            {
                // Grayscale
                throw new NotImplementedException("Image format not supported (grayscales).");
            }

            dest.Data = imageData;
            dest.Length = imageData.Length;

            if (alphaMask != null)
            {
                dest.AlphaMask = alphaMask;
                dest.AlphaMaskLength = alphaMask.Length;
            }

            if (mask != null)
            {
                dest.BitmapMask = mask.MaskData;
                dest.BitmapMaskLength = mask.MaskData.Length;
            }
        }

        private void CopyIndexedMemoryBitmap(int bits/*, ref bool hasAlpha*/, ImageDataBitmap dest)
        {
            int firstMaskColor = -1, lastMaskColor = -1;
            bool segmentedColorMask = false;

            int bytesColorPaletteOffset = ((ImagePrivateDataBitmap)Image.Data).ColorPaletteOffset; // GDI+ always returns Windows bitmaps: sizeof BITMAPFILEHEADER + sizeof BITMAPINFOHEADER

            int bytesFileOffset = ((ImagePrivateDataBitmap)Image.Data).Offset;
            uint paletteColors = Image.Information.ColorsUsed;
            int width = (int)Image.Information.Width;
            int height = (int)Image.Information.Height;

            MonochromeMask mask = new MonochromeMask(width, height);

            bool isGray = bits == 8 && (paletteColors == 256 || paletteColors == 0);
            int isBitonal = 0; // 0: false; >0: true; <0: true (inverted)
            byte[] paletteData = new byte[3 * paletteColors];
            for (int color = 0; color < paletteColors; ++color)
            {
                paletteData[3 * color] = Data[bytesColorPaletteOffset + 4 * color + 2];
                paletteData[3 * color + 1] = Data[bytesColorPaletteOffset + 4 * color + 1];
                paletteData[3 * color + 2] = Data[bytesColorPaletteOffset + 4 * color + 0];
                if (isGray)
                    isGray = paletteData[3 * color] == paletteData[3 * color + 1] &&
                      paletteData[3 * color] == paletteData[3 * color + 2];

                if (Data[bytesColorPaletteOffset + 4 * color + 3] < 128)
                {
                    // We treat this as transparency:
                    if (firstMaskColor == -1)
                        firstMaskColor = color;
                    if (lastMaskColor == -1 || lastMaskColor == color - 1)
                        lastMaskColor = color;
                    if (lastMaskColor != color)
                        segmentedColorMask = true;
                }
                //else
                //{
                //  // We treat this as opacity:
                //}
            }

            if (bits == 1)
            {
                if (paletteColors == 0)
                    isBitonal = 1;
                if (paletteColors == 2)
                {
                    if (paletteData[0] == 0 &&
                      paletteData[1] == 0 &&
                      paletteData[2] == 0 &&
                      paletteData[3] == 255 &&
                      paletteData[4] == 255 &&
                      paletteData[5] == 255)
                        isBitonal = 1; // Black on white
                    if (paletteData[5] == 0 &&
                      paletteData[4] == 0 &&
                      paletteData[3] == 0 &&
                      paletteData[2] == 255 &&
                      paletteData[1] == 255 &&
                      paletteData[0] == 255)
                        isBitonal = -1; // White on black
                }
            }

            // NYI: (no sample found where this was required) 
            // if (segmentedColorMask = true)
            // { ... }

            bool isFaxEncoding = false;
            byte[] imageData = new byte[((width * bits + 7) / 8) * height];
            byte[] imageDataFax = null;
            int k = 0;


            if (bits == 1 && dest._document.Options.EnableCcittCompressionForBilevelImages)
            {
                // TODO: flag/option?
                // We try Group 3 1D and Group 4 (2D) encoding here and keep the smaller byte array.
                //byte[] temp = new byte[imageData.Length];
                //int ccittSize = DoFaxEncoding(ref temp, imageBits, (uint)bytesFileOffset, (uint)width, (uint)height);

                // It seems that Group 3 2D encoding never beats both other encodings, therefore we don't call it here.
                //byte[] temp2D = new byte[imageData.Length];
                //uint dpiY = (uint)image.VerticalResolution;
                //uint kTmp = 0;
                //int ccittSize2D = DoFaxEncoding2D((uint)bytesFileOffset, ref temp2D, imageBits, (uint)width, (uint)height, dpiY, out kTmp);
                //k = (int) kTmp;

                byte[] tempG4 = new byte[imageData.Length];
                int ccittSizeG4 = PdfImage.DoFaxEncodingGroup4(ref tempG4, Data, (uint)bytesFileOffset, (uint)width, (uint)height);

                isFaxEncoding = /*ccittSize > 0 ||*/ ccittSizeG4 > 0;
                if (isFaxEncoding)
                {
                    //if (ccittSize == 0)
                    //  ccittSize = 0x7fffffff;
                    if (ccittSizeG4 == 0)
                        ccittSizeG4 = 0x7fffffff;
                    //if (ccittSize <= ccittSizeG4)
                    //{
                    //  Array.Resize(ref temp, ccittSize);
                    //  imageDataFax = temp;
                    //  k = 0;
                    //}
                    //else
                    {
                        Array.Resize(ref tempG4, ccittSizeG4);
                        imageDataFax = tempG4;
                        k = -1;
                    }
                }
            }

            //if (!isFaxEncoding)
            {
                int bytesOffsetRead = 0;
                if (bits == 8 || bits == 4 || bits == 1)
                {
                    int bytesPerLine = (width * bits + 7) / 8;
                    for (int y = 0; y < height; ++y)
                    {
                        mask.StartLine(y);
                        int bytesOffsetWrite = (height - 1 - y) * ((width * bits + 7) / 8);
                        for (int x = 0; x < bytesPerLine; ++x)
                        {
                            if (isGray)
                            {
                                // Lookup the gray value from the palette:
                                imageData[bytesOffsetWrite] = paletteData[3 * Data[bytesFileOffset + bytesOffsetRead]];
                            }
                            else
                            {
                                // Store the palette index.
                                imageData[bytesOffsetWrite] = Data[bytesFileOffset + bytesOffsetRead];
                            }
                            if (firstMaskColor != -1)
                            {
                                int n = Data[bytesFileOffset + bytesOffsetRead];
                                if (bits == 8)
                                {
                                    // TODO???: segmentedColorMask == true => bad mask NYI
                                    mask.AddPel((n >= firstMaskColor) && (n <= lastMaskColor));
                                }
                                else if (bits == 4)
                                {
                                    // TODO???: segmentedColorMask == true => bad mask NYI
                                    int n1 = (n & 0xf0) / 16;
                                    int n2 = (n & 0x0f);
                                    mask.AddPel((n1 >= firstMaskColor) && (n1 <= lastMaskColor));
                                    mask.AddPel((n2 >= firstMaskColor) && (n2 <= lastMaskColor));
                                }
                                else if (bits == 1)
                                {
                                    // TODO???: segmentedColorMask == true => bad mask NYI
                                    for (int bit = 1; bit <= 8; ++bit)
                                    {
                                        int n1 = (n & 0x80) / 128;
                                        mask.AddPel((n1 >= firstMaskColor) && (n1 <= lastMaskColor));
                                        n *= 2;
                                    }
                                }
                            }
                            bytesOffsetRead += 1;
                            bytesOffsetWrite += 1;
                        }
                        bytesOffsetRead = 4 * ((bytesOffsetRead + 3) / 4); // Align to 32 bit boundary
                    }
                }
                else
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format #3");
                }
            }

            dest.Data = imageData;
            dest.Length = imageData.Length;

            if (imageDataFax != null)
            {
                dest.DataFax = imageDataFax;
                dest.LengthFax = imageDataFax.Length;
            }

            dest.IsGray = isGray;
            dest.K = k;
            dest.IsBitonal = isBitonal;

            dest.PaletteData = paletteData;
            dest.PaletteDataLength = paletteData.Length;
            dest.SegmentedColorMask = segmentedColorMask;

            //if (alphaMask != null)
            //{
            //    dest.AlphaMask = alphaMask;
            //    dest.AlphaMaskLength = alphaMask.Length;
            //}

            if (mask != null && firstMaskColor != -1)
            {
                dest.BitmapMask = mask.MaskData;
                dest.BitmapMaskLength = mask.MaskData.Length;
            }

        }
    }
}
