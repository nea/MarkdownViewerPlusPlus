#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Stefan Lange
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
using System.Diagnostics;
using System.IO;
#if CORE
using System.Drawing.Imaging;
#endif
#if GDI
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows.Media.Imaging;
#endif
using PdfSharp.Drawing;
using PdfSharp.Drawing.Internal;
using PdfSharp.Pdf.Filters;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents an image.
    /// </summary>
    public sealed partial class PdfImage : PdfXObject
    {
        /// <summary>
        /// Initializes a new instance of PdfImage from an XImage.
        /// </summary>
        public PdfImage(PdfDocument document, XImage image)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/XObject");
            Elements.SetName(Keys.Subtype, "/Image");

            _image = image;

#if !SILVERLIGHT
            ////// TODO: identify multiple used images. If the image already exists use the same XRef.
            ////_defaultName = PdfImageTable.NextImageName;

            switch (_image.Format.Guid.ToString("B").ToUpper())
            {
                // Pdf supports Jpeg, therefore we can write what we've read:
                case "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}":  //XImageFormat.Jpeg
                    InitializeJpeg();
                    break;

                // All other image formats are converted to PDF bitmaps:
                case "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}":  //XImageFormat.Png
                case "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}":  //XImageFormat.Gif
                case "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}":  //XImageFormat.Tiff
                case "{B96B3CB5-0728-11D3-9D7B-0000F81EF32E}":  //XImageFormat.Icon
                    // TODO: possible optimization for PNG (do not decompress/recompress)???
                    // TODO: try Jpeg for size optimization???
                    InitializeNonJpeg();
                    break;

                case "{84570158-DBF0-4C6B-8368-62D6A3CA76E0}":  //XImageFormat.Pdf:
                    Debug.Assert(false, "XPdfForm not expected here.");
                    break;

                default:
                    Debug.Assert(false, "Unexpected image type.");
                    break;
            }
#else
            InitializeAg();
#endif
        }

#if SILVERLIGHT
        private void InitializeAg()
        {
            ReadTrueColorMemoryBitmapAg(3, 8, true);
        }

        private void ReadTrueColorMemoryBitmapAg(int components, int bits, bool hasAlpha)
        {
            int pdfVersion = Owner.Version;
            MemoryStream memory = new MemoryStream();

            WriteableBitmap bitmap = null;
            if (_image._wpfImage is WriteableBitmap)
                bitmap = (WriteableBitmap)_image._wpfImage;
            else if (_image._wpfImage is BitmapImage)
                bitmap = new WriteableBitmap(_image._wpfImage);

            if (bitmap != null)
            {
                int height = _image.PixelHeight;
                int width = _image.PixelWidth;

                int logicalComponents = components;
                if (components == 4)
                    logicalComponents = 3;

                byte[] imageData = new byte[components * width * height];

                bool hasMask = false;
                bool hasAlphaMask = false;
                byte[] alphaMask = hasAlpha ? new byte[width * height] : null;
                MonochromeMask mask = hasAlpha ? new MonochromeMask(width, height) : null;

                int nOffsetRead = 0;
                if (logicalComponents == 3)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        int nOffsetWrite = 3 * y * width; // 3*(height - 1 - y)*width;
                        int nOffsetWriteAlpha = 0;
                        if (hasAlpha)
                        {
                            mask.StartLine(y);
                            nOffsetWriteAlpha = y * width; // (height - 1 - y) * width;
                        }

                        for (int x = 0; x < width; ++x)
                        {
                            uint pixel = (uint)bitmap.Pixels[nOffsetRead];
                            imageData[nOffsetWrite] = (byte)(pixel >> 16);
                            imageData[nOffsetWrite + 1] = (byte)(pixel >> 8);
                            imageData[nOffsetWrite + 2] = (byte)(pixel);
                            if (hasAlpha)
                            {
                                byte pel = (byte)(pixel >> 24);
                                mask.AddPel(pel);
                                alphaMask[nOffsetWriteAlpha] = pel;
                                if (!hasMask || !hasAlphaMask)
                                {
                                    if (pel != 255)
                                    {
                                        hasMask = true;
                                        if (pel != 0)
                                            hasAlphaMask = true;
                                    }
                                }
                                ++nOffsetWriteAlpha;
                            }
                            //nOffsetRead += hasAlpha ? 4 : components;
                            ++nOffsetRead;
                            nOffsetWrite += 3;
                        }
                        //nOffsetRead = 4*((nOffsetRead + 3)/4); // Align to 32 bit boundary
                    }
                }
                else if (components == 1)
                {
                    // Grayscale
                    throw new NotImplementedException("Image format not supported (grayscales).");
                }

                FlateDecode fd = new FlateDecode();
                if (hasMask)
                {
                    // monochrome mask is either sufficient or
                    // provided for compatibility with older reader versions
                    byte[] maskDataCompressed = fd.Encode(mask.MaskData, _document.Options.FlateEncodeMode);
                    PdfDictionary pdfMask = new PdfDictionary(_document);
                    pdfMask.Elements.SetName(Keys.Type, "/XObject");
                    pdfMask.Elements.SetName(Keys.Subtype, "/Image");

                    Owner._irefTable.Add(pdfMask);
                    pdfMask.Stream = new PdfStream(maskDataCompressed, pdfMask);
                    pdfMask.Elements[Keys.Length] = new PdfInteger(maskDataCompressed.Length);
                    pdfMask.Elements[Keys.Filter] = new PdfName("/FlateDecode");
                    pdfMask.Elements[Keys.Width] = new PdfInteger(width);
                    pdfMask.Elements[Keys.Height] = new PdfInteger(height);
                    pdfMask.Elements[Keys.BitsPerComponent] = new PdfInteger(1);
                    pdfMask.Elements[Keys.ImageMask] = new PdfBoolean(true);
                    Elements[Keys.Mask] = pdfMask.Reference;
                }
                if (hasMask && hasAlphaMask && pdfVersion >= 14)
                {
                    // The image provides an alpha mask (requires Arcrobat 5.0 or higher)
                    byte[] alphaMaskCompressed = fd.Encode(alphaMask, _document.Options.FlateEncodeMode);
                    PdfDictionary smask = new PdfDictionary(_document);
                    smask.Elements.SetName(Keys.Type, "/XObject");
                    smask.Elements.SetName(Keys.Subtype, "/Image");

                    Owner._irefTable.Add(smask);
                    smask.Stream = new PdfStream(alphaMaskCompressed, smask);
                    smask.Elements[Keys.Length] = new PdfInteger(alphaMaskCompressed.Length);
                    smask.Elements[Keys.Filter] = new PdfName("/FlateDecode");
                    smask.Elements[Keys.Width] = new PdfInteger(width);
                    smask.Elements[Keys.Height] = new PdfInteger(height);
                    smask.Elements[Keys.BitsPerComponent] = new PdfInteger(8);
                    smask.Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                    Elements[Keys.SMask] = smask.Reference;
                }

                byte[] imageDataCompressed = fd.Encode(imageData, _document.Options.FlateEncodeMode);

                Stream = new PdfStream(imageDataCompressed, this);
                Elements[Keys.Length] = new PdfInteger(imageDataCompressed.Length);
                Elements[Keys.Filter] = new PdfName("/FlateDecode");
                Elements[Keys.Width] = new PdfInteger(width);
                Elements[Keys.Height] = new PdfInteger(height);
                Elements[Keys.BitsPerComponent] = new PdfInteger(8);
                // TODO: CMYK
                Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                if (_image.Interpolate)
                    Elements[Keys.Interpolate] = PdfBoolean.True;
            }
        }
#endif

        /// <summary>
        /// Gets the underlying XImage object.
        /// </summary>
        public XImage Image
        {
            get { return _image; }
        }

        readonly XImage _image;

        /// <summary>
        /// Returns 'Image'.
        /// </summary>
        public override string ToString()
        {
            return "Image";
        }

        /// <summary>
        /// Creates the keys for a JPEG image.
        /// </summary>
        void InitializeJpeg()
        {
            // PDF supports JPEG, so there's not much to be done.
            MemoryStream memory = null;
            // Close the MemoryStream if we create it.
            bool ownMemory = false;

            byte[] imageBits = null;
            int streamLength = 0;

#if CORE || GDI || WPF
            if (_image._importedImage != null)
            {
                ImageDataDct idd = (ImageDataDct)_image._importedImage.ImageData;
                imageBits = idd.Data;
                streamLength = idd.Length;
            }
#endif

#if CORE || GDI
            if (_image._importedImage == null)
            {
                if (!_image._path.StartsWith("*"))
                {
                    // Image does not come from a stream, so we have the path to the file - just use the path.
                    // If the image was modified in memory, those changes will be lost and the original image, as it was read from the file, will be added to the PDF.
                    using (FileStream sourceFile = File.OpenRead(_image._path))
                    {
                        int count;
                        byte[] buffer = new byte[8192];
                        memory = new MemoryStream((int)sourceFile.Length);
                        ownMemory = true;
                        do
                        {
                            count = sourceFile.Read(buffer, 0, buffer.Length);
                            // memory.Write(buffer, 0, buffer.Length);
                            memory.Write(buffer, 0, count);
                        } while (count > 0);
                    }
                }
                else
                {
                    memory = new MemoryStream();
                    ownMemory = true;
                    // If we have a stream, copy data from the stream.
                    if (_image._stream != null && _image._stream.CanSeek)
                    {
                        Stream stream = _image._stream;
                        stream.Seek(0, SeekOrigin.Begin);
                        byte[] buffer = new byte[32 * 1024]; // 32K buffer.
                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            memory.Write(buffer, 0, bytesRead);
                        }
                    }
                    else
                    {
#if CORE_WITH_GDI
                        // No stream, no filename, get image data.
                        // Save the image to a memory stream.
                        _image._gdiImage.Save(memory, ImageFormat.Jpeg);
#endif
                    }
                }

                if ((int)memory.Length == 0)
                {
                    Debug.Assert(false, "Internal error? JPEG image, but file not found!");
                }
            }
#endif
#if WPF
            // AGHACK
            //string filename = XImage.GetImageFilename(image._wpfImage);
            //if (XImage.ReadJpegFile(filename, -1, ref imageBits))
            //{
            //  streamLength = imageBits.Length;
            //}
            //else
            //  imageBits = null;
#if !SILVERLIGHT
            memory = _image.Memory;
#else
            memory = new MemoryStream();
            ownMemory = true;
#endif
#endif
#if NETFX_CORE
            memory = new MemoryStream();
            ownMemory = true;
#endif
            // THHO4THHO Use ImageImporterJPEG here to avoid redundant code.

            if (imageBits == null)
            {
                streamLength = (int)memory.Length;
                imageBits = new byte[streamLength];
                memory.Seek(0, SeekOrigin.Begin);
                memory.Read(imageBits, 0, streamLength);
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (ownMemory)
                {
#if UWP || true
                    memory.Dispose();
#else
                memory.Close();
#endif
                }
            }

            bool tryFlateDecode = _document.Options.UseFlateDecoderForJpegImages == PdfUseFlateDecoderForJpegImages.Automatic;
            bool useFlateDecode = _document.Options.UseFlateDecoderForJpegImages == PdfUseFlateDecoderForJpegImages.Always;

            FlateDecode fd = new FlateDecode();
            byte[] imageDataCompressed = (useFlateDecode || tryFlateDecode) ? fd.Encode(imageBits, _document.Options.FlateEncodeMode) : null;
            if (useFlateDecode || tryFlateDecode && imageDataCompressed.Length < imageBits.Length)
            {
                Stream = new PdfStream(imageDataCompressed, this);
                Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataCompressed.Length);
                PdfArray arrayFilters = new PdfArray(_document);
                arrayFilters.Elements.Add(new PdfName("/FlateDecode"));
                arrayFilters.Elements.Add(new PdfName("/DCTDecode"));
                Elements[PdfStream.Keys.Filter] = arrayFilters;
            }
            else
            {
                Stream = new PdfStream(imageBits, this);
                Elements[PdfStream.Keys.Length] = new PdfInteger(streamLength);
                Elements[PdfStream.Keys.Filter] = new PdfName("/DCTDecode");
            }
            if (_image.Interpolate)
                Elements[Keys.Interpolate] = PdfBoolean.True;
            Elements[Keys.Width] = new PdfInteger(_image.PixelWidth);
            Elements[Keys.Height] = new PdfInteger(_image.PixelHeight);
            Elements[Keys.BitsPerComponent] = new PdfInteger(8);

#if CORE || GDI || WPF
            if (_image._importedImage != null)
            {
                if (_image._importedImage.Information.ImageFormat == ImageInformation.ImageFormats.JPEGCMYK ||
                    _image._importedImage.Information.ImageFormat == ImageInformation.ImageFormats.JPEGRGBW)
                {
                    // TODO: Test with CMYK JPEG files (so far I only found ImageFlags.ColorSpaceYcck JPEG files ...)
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");
                    if (_image._importedImage.Information.ImageFormat == ImageInformation.ImageFormats.JPEGRGBW)
                        Elements["/Decode"] = new PdfLiteral("[1 0 1 0 1 0 1 0]"); // Invert colors from RGBW to CMYK.
                }
                else if (_image._importedImage.Information.ImageFormat == ImageInformation.ImageFormats.JPEGGRAY)
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                }
                else
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                }
            }
#endif
#if CORE_WITH_GDI
            if (_image._importedImage == null)
            {
                if ((_image._gdiImage.Flags & ((int)ImageFlags.ColorSpaceCmyk | (int)ImageFlags.ColorSpaceYcck)) != 0)
                {
                    // TODO: Test with CMYK JPEG files (so far I only found ImageFlags.ColorSpaceYcck JPEG files ...)
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");
                    if ((_image._gdiImage.Flags & (int)ImageFlags.ColorSpaceYcck) != 0)
                        Elements["/Decode"] = new PdfLiteral("[1 0 1 0 1 0 1 0]"); // Invert colors? Why??
                }
                else if ((_image._gdiImage.Flags & (int)ImageFlags.ColorSpaceGray) != 0)
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                }
                else
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                }
            }
#endif
#if GDI
            if (_image._importedImage == null)
            {
                if ((_image._gdiImage.Flags & ((int)ImageFlags.ColorSpaceCmyk | (int)ImageFlags.ColorSpaceYcck)) != 0)
                {
                    // TODO: Test with CMYK JPEG files (so far I only found ImageFlags.ColorSpaceYcck JPEG files ...)
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");
                    if ((_image._gdiImage.Flags & (int)ImageFlags.ColorSpaceYcck) != 0)
                        Elements["/Decode"] = new PdfLiteral("[1 0 1 0 1 0 1 0]"); // Invert colors? Why??
                }
                else if ((_image._gdiImage.Flags & (int)ImageFlags.ColorSpaceGray) != 0)
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                }
                else
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                }
            }
#endif
#if WPF
            // TODOSILVERLIGHT
#if !SILVERLIGHT
            string pixelFormat = _image._wpfImage.Format.ToString();
#else
            string pixelFormat = "xxx";
#endif
            bool isCmyk = _image.IsCmyk;
            bool isGrey = pixelFormat == "Gray8";
            if (isCmyk)
            {
                // TODO: Test with CMYK JPEG files (so far I only found ImageFlags.ColorSpaceYcck JPEG files ...)
                Elements[Keys.ColorSpace] = new PdfName("/DeviceCMYK");
                Elements["/Decode"] = new PdfLiteral("[1 0 1 0 1 0 1 0]");  // Invert colors? Why??
            }
            else if (isGrey)
            {
                Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
            }
            else
            {
                Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
            }
#endif
        }

        /// <summary>
        /// Creates the keys for a FLATE image.
        /// </summary>
        void InitializeNonJpeg()
        {
#if CORE || GDI || WPF
            if (_image._importedImage != null)
            {
                switch (_image._importedImage.Information.ImageFormat)
                {
                    case ImageInformation.ImageFormats.RGB24:
                        CreateTrueColorMemoryBitmap(3, 8, false);
                        break;

                    case ImageInformation.ImageFormats.Palette8:
                        CreateIndexedMemoryBitmap(8);
                        break;

                    case ImageInformation.ImageFormats.Palette4:
                        CreateIndexedMemoryBitmap(4);
                        break;

                    case ImageInformation.ImageFormats.Palette1:
                        CreateIndexedMemoryBitmap(1);
                        break;

                    default:
                        throw new NotImplementedException("Image format not supported.");
                }
                return;
            }
#endif

#if (CORE_WITH_GDI || GDI) && !WPF
            //bool hasMask = false;
            switch (_image._gdiImage.PixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    ReadTrueColorMemoryBitmap(3, 8, false);
                    break;

                case PixelFormat.Format32bppRgb:
                    ReadTrueColorMemoryBitmap(4, 8, false);
                    break;

                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format32bppPArgb:
                    //hasMask = true;
                    ReadTrueColorMemoryBitmap(3, 8, true);
                    break;

                case PixelFormat.Format8bppIndexed:
                    ReadIndexedMemoryBitmap(8/*, ref hasMask*/);
                    break;

                case PixelFormat.Format4bppIndexed:
                    ReadIndexedMemoryBitmap(4/*, ref hasMask*/);
                    break;

                case PixelFormat.Format1bppIndexed:
                    ReadIndexedMemoryBitmap(1/*, ref hasMask*/);
                    break;

                default:
#if DEBUGxxx
          image.image.Save("$$$.bmp", ImageFormat.Bmp);
#endif
                    throw new NotImplementedException("Image format not supported.");
            }
#endif
#if WPF // && !GDI
#if !SILVERLIGHT
            string format = _image._wpfImage.Format.ToString();
#else
            string format = "Bgr24";
#endif
            switch (format)
            {
                case "Bgr24": //Format24bppRgb:
                    ReadTrueColorMemoryBitmap(3, 8, false);
                    break;

                //case .PixelFormat.Format32bppRgb:
                //  ReadTrueColorMemoryBitmap(4, 8, false);
                //  break;

                case "Bgra32":  //PixelFormat.Format32bppArgb:
                    //case PixelFormat.Format32bppPArgb:
                    ReadTrueColorMemoryBitmap(3, 8, true);
                    break;

                case "Bgr32":
                    ReadTrueColorMemoryBitmap(4, 8, false);
                    break;

                case "Pbgra32":
                    ReadTrueColorMemoryBitmap(3, 8, true);
                    break;

                case "Indexed8":  //Format8bppIndexed:
                case "Gray8":
                    ReadIndexedMemoryBitmap(8/*, ref hasMask*/);
                    break;

                case "Indexed4":  //Format4bppIndexed:
                case "Gray4":
                    ReadIndexedMemoryBitmap(4/*, ref hasMask*/);
                    break;

                case "Indexed2":
                    ReadIndexedMemoryBitmap(2/*, ref hasMask*/);
                    break;

                case "Indexed1":  //Format1bppIndexed:
                case "BlackWhite":  //Format1bppIndexed:
                    ReadIndexedMemoryBitmap(1/*, ref hasMask*/);
                    break;

                default:
#if DEBUGxxx
                    image.image.Save("$$$.bmp", ImageFormat.Bmp);
#endif
                    throw new NotImplementedException("Image format \"" + format + "\" not supported.");
            }
#endif
        }

#if CORE || GDI || WPF
        private void CreateIndexedMemoryBitmap(int bits)
        {
            ImageDataBitmap idb = (ImageDataBitmap)_image._importedImage.ImageData;
            ImageInformation ii = _image._importedImage.Information;

            int pdfVersion = Owner.Version;
            int firstMaskColor = -1, lastMaskColor = -1;
            bool segmentedColorMask = idb.SegmentedColorMask;

            {

                FlateDecode fd = new FlateDecode();
                if (firstMaskColor != -1 &&
                  lastMaskColor != -1)
                {
                    // Color mask requires Reader 4.0 or higher:
                    //if (!segmentedColorMask && pdfVersion >= 13)
                    if (!segmentedColorMask && pdfVersion >= 13 && !idb.IsGray)
                    {
                        PdfArray array = new PdfArray(_document);
                        array.Elements.Add(new PdfInteger(firstMaskColor));
                        array.Elements.Add(new PdfInteger(lastMaskColor));
                        Elements[Keys.Mask] = array;
                    }
                    else
                    {
                        // Monochrome mask
                        byte[] maskDataCompressed = fd.Encode(idb.BitmapMask, _document.Options.FlateEncodeMode);
                        PdfDictionary pdfMask = new PdfDictionary(_document);
                        pdfMask.Elements.SetName(Keys.Type, "/XObject");
                        pdfMask.Elements.SetName(Keys.Subtype, "/Image");

                        Owner._irefTable.Add(pdfMask);
                        pdfMask.Stream = new PdfStream(maskDataCompressed, pdfMask);
                        pdfMask.Elements[PdfStream.Keys.Length] = new PdfInteger(maskDataCompressed.Length);
                        pdfMask.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                        pdfMask.Elements[Keys.Width] = new PdfInteger((int)ii.Width);
                        pdfMask.Elements[Keys.Height] = new PdfInteger((int)ii.Height);
                        pdfMask.Elements[Keys.BitsPerComponent] = new PdfInteger(1);
                        pdfMask.Elements[Keys.ImageMask] = new PdfBoolean(true);
                        Elements[Keys.Mask] = pdfMask.Reference;
                    }
                }

                byte[] imageDataCompressed = fd.Encode(idb.Data, _document.Options.FlateEncodeMode);
                byte[] imageDataFaxCompressed = idb.DataFax != null ? fd.Encode(idb.DataFax, _document.Options.FlateEncodeMode) : null;

                bool usesCcittEncoding = false;
                if (idb.DataFax != null &&
                  (idb.LengthFax < imageDataCompressed.Length ||
                  imageDataFaxCompressed.Length < imageDataCompressed.Length))
                {
                    // /CCITTFaxDecode creates the smaller file (with or without /FlateDecode):
                    usesCcittEncoding = true;

                    if (idb.LengthFax < imageDataCompressed.Length)
                    {
                        Stream = new PdfStream(idb.DataFax, this);
                        Elements[PdfStream.Keys.Length] = new PdfInteger(idb.LengthFax);
                        Elements[PdfStream.Keys.Filter] = new PdfName("/CCITTFaxDecode");
                        //PdfArray array2 = new PdfArray(_document);
                        PdfDictionary dictionary = new PdfDictionary();
                        if (idb.K != 0)
                            dictionary.Elements.Add("/K", new PdfInteger(idb.K));
                        if (idb.IsBitonal < 0)
                            dictionary.Elements.Add("/BlackIs1", new PdfBoolean(true));
                        dictionary.Elements.Add("/EndOfBlock", new PdfBoolean(false));
                        dictionary.Elements.Add("/Columns", new PdfInteger((int)ii.Width));
                        dictionary.Elements.Add("/Rows", new PdfInteger((int)ii.Height));
                        //array2.Elements.Add(dictionary);
                        Elements[PdfStream.Keys.DecodeParms] = dictionary; // array2;
                    }
                    else
                    {
                        Stream = new PdfStream(imageDataFaxCompressed, this);
                        Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataFaxCompressed.Length);
                        PdfArray arrayFilters = new PdfArray(_document);
                        arrayFilters.Elements.Add(new PdfName("/FlateDecode"));
                        arrayFilters.Elements.Add(new PdfName("/CCITTFaxDecode"));
                        Elements[PdfStream.Keys.Filter] = arrayFilters;
                        PdfArray arrayDecodeParms = new PdfArray(_document);

                        PdfDictionary dictFlateDecodeParms = new PdfDictionary();
                        //dictFlateDecodeParms.Elements.Add("/Columns", new PdfInteger(1));

                        PdfDictionary dictCcittFaxDecodeParms = new PdfDictionary();
                        if (idb.K != 0)
                            dictCcittFaxDecodeParms.Elements.Add("/K", new PdfInteger(idb.K));
                        if (idb.IsBitonal < 0)
                            dictCcittFaxDecodeParms.Elements.Add("/BlackIs1", new PdfBoolean(true));
                        dictCcittFaxDecodeParms.Elements.Add("/EndOfBlock", new PdfBoolean(false));
                        dictCcittFaxDecodeParms.Elements.Add("/Columns", new PdfInteger((int)ii.Width));
                        dictCcittFaxDecodeParms.Elements.Add("/Rows", new PdfInteger((int)ii.Height));

                        arrayDecodeParms.Elements.Add(dictFlateDecodeParms); // How to add the "null object"?
                        arrayDecodeParms.Elements.Add(dictCcittFaxDecodeParms);
                        Elements[PdfStream.Keys.DecodeParms] = arrayDecodeParms;
                    }
                }
                else
                {
                    // /FlateDecode creates the smaller file (or no monochrome bitmap):
                    Stream = new PdfStream(imageDataCompressed, this);
                    Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataCompressed.Length);
                    Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                }

                Elements[Keys.Width] = new PdfInteger((int)ii.Width);
                Elements[Keys.Height] = new PdfInteger((int)ii.Height);
                Elements[Keys.BitsPerComponent] = new PdfInteger(bits);
                // TODO: CMYK

                // CCITT encoding: we need color palette for isBitonal == 0
                // FlateDecode: we need color palette for isBitonal <= 0 unless we have grayscales
                if ((usesCcittEncoding && idb.IsBitonal == 0) ||
                  (!usesCcittEncoding && idb.IsBitonal <= 0 && !idb.IsGray))
                {
                    PdfDictionary colorPalette = null;
                    colorPalette = new PdfDictionary(_document);
                    byte[] packedPaletteData = idb.PaletteDataLength >= 48 ? fd.Encode(idb.PaletteData, _document.Options.FlateEncodeMode) : null; // don't compress small palettes
                    if (packedPaletteData != null && packedPaletteData.Length + 20 < idb.PaletteDataLength) // +20: compensate for the overhead (estimated value)
                    {
                        // Create compressed color palette:
                        colorPalette.CreateStream(packedPaletteData);
                        colorPalette.Elements[PdfStream.Keys.Length] = new PdfInteger(packedPaletteData.Length);
                        colorPalette.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                    }
                    else
                    {
                        // Create uncompressed color palette:
                        colorPalette.CreateStream(idb.PaletteData);
                        colorPalette.Elements[PdfStream.Keys.Length] = new PdfInteger(idb.PaletteDataLength);
                    }
                    Owner._irefTable.Add(colorPalette);

                    PdfArray arrayColorSpace = new PdfArray(_document);
                    arrayColorSpace.Elements.Add(new PdfName("/Indexed"));
                    arrayColorSpace.Elements.Add(new PdfName("/DeviceRGB"));
                    arrayColorSpace.Elements.Add(new PdfInteger((int)ii.ColorsUsed - 1));
                    arrayColorSpace.Elements.Add(colorPalette.Reference);
                    Elements[Keys.ColorSpace] = arrayColorSpace;
                }
                else
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                }
                if (_image.Interpolate)
                    Elements[Keys.Interpolate] = PdfBoolean.True;
            }
        }

        private void CreateTrueColorMemoryBitmap(int components, int bits, bool hasAlpha)
        {
            int pdfVersion = Owner.Version;
            FlateDecode fd = new FlateDecode();
            ImageDataBitmap idb = (ImageDataBitmap)_image._importedImage.ImageData;
            ImageInformation ii = _image._importedImage.Information;
            bool hasMask = idb.AlphaMaskLength > 0 || idb.BitmapMaskLength > 0;
            bool hasAlphaMask = idb.AlphaMaskLength > 0;

            if (hasMask)
            {
                // monochrome mask is either sufficient or
                // provided for compatibility with older reader versions
                byte[] maskDataCompressed = fd.Encode(idb.BitmapMask, _document.Options.FlateEncodeMode);
                PdfDictionary pdfMask = new PdfDictionary(_document);
                pdfMask.Elements.SetName(Keys.Type, "/XObject");
                pdfMask.Elements.SetName(Keys.Subtype, "/Image");

                Owner._irefTable.Add(pdfMask);
                pdfMask.Stream = new PdfStream(maskDataCompressed, pdfMask);
                pdfMask.Elements[PdfStream.Keys.Length] = new PdfInteger(maskDataCompressed.Length);
                pdfMask.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                pdfMask.Elements[Keys.Width] = new PdfInteger((int)ii.Width);
                pdfMask.Elements[Keys.Height] = new PdfInteger((int)ii.Height);
                pdfMask.Elements[Keys.BitsPerComponent] = new PdfInteger(1);
                pdfMask.Elements[Keys.ImageMask] = new PdfBoolean(true);
                Elements[Keys.Mask] = pdfMask.Reference;
            }
            if (hasMask && hasAlphaMask && pdfVersion >= 14)
            {
                // The image provides an alpha mask (requires Arcrobat 5.0 or higher)
                byte[] alphaMaskCompressed = fd.Encode(idb.AlphaMask, _document.Options.FlateEncodeMode);
                PdfDictionary smask = new PdfDictionary(_document);
                smask.Elements.SetName(Keys.Type, "/XObject");
                smask.Elements.SetName(Keys.Subtype, "/Image");

                Owner._irefTable.Add(smask);
                smask.Stream = new PdfStream(alphaMaskCompressed, smask);
                smask.Elements[PdfStream.Keys.Length] = new PdfInteger(alphaMaskCompressed.Length);
                smask.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                smask.Elements[Keys.Width] = new PdfInteger((int)ii.Width);
                smask.Elements[Keys.Height] = new PdfInteger((int)ii.Height);
                smask.Elements[Keys.BitsPerComponent] = new PdfInteger(8);
                smask.Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                Elements[Keys.SMask] = smask.Reference;
            }

            byte[] imageDataCompressed = fd.Encode(idb.Data, _document.Options.FlateEncodeMode);

            Stream = new PdfStream(imageDataCompressed, this);
            Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataCompressed.Length);
            Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
            Elements[Keys.Width] = new PdfInteger((int)ii.Width);
            Elements[Keys.Height] = new PdfInteger((int)ii.Height);
            Elements[Keys.BitsPerComponent] = new PdfInteger(8);
            // TODO: CMYK
            Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
            if (_image.Interpolate)
                Elements[Keys.Interpolate] = PdfBoolean.True;
        }
#endif

        private static int ReadWord(byte[] ab, int offset)
        {
            return ab[offset] + 256 * ab[offset + 1];
        }

        private static int ReadDWord(byte[] ab, int offset)
        {
            return ReadWord(ab, offset) + 0x10000 * ReadWord(ab, offset + 2);
        }

        /// <summary>
        /// Reads images that are returned from GDI+ without color palette.
        /// </summary>
        /// <param name="components">4 (32bpp RGB), 3 (24bpp RGB, 32bpp ARGB)</param>
        /// <param name="bits">8</param>
        /// <param name="hasAlpha">true (ARGB), false (RGB)</param>
        private void ReadTrueColorMemoryBitmap(int components, int bits, bool hasAlpha)
        {
#if DEBUG_
            image.image.Save("$$$.bmp", ImageFormat.Bmp);
#endif
            int pdfVersion = Owner.Version;
            MemoryStream memory = new MemoryStream();
#if CORE_WITH_GDI
            _image._gdiImage.Save(memory, ImageFormat.Bmp);
#endif
#if GDI
            _image._gdiImage.Save(memory, ImageFormat.Bmp);
#endif
#if WPF
#if !SILVERLIGHT
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(_image._wpfImage));
            encoder.Save(memory);
#else
            // AGHACK
            GetType();
#endif
#endif
            // THHO4THHO Use ImageImporterBMP here to avoid redundant code.

            int streamLength = (int)memory.Length;
            Debug.Assert(streamLength > 0, "Bitmap image encoding failed.");
            if (streamLength > 0)
            {
#if !NETFX_CORE && !UWP
                // THHO4STLA: available with wrt, but not with wrt81.
                // Note: imageBits.Length can be larger than streamLength. Do not use these extra bytes!
                byte[] imageBits = memory.GetBuffer();
#elif NETFX_CORE 
                byte[] imageBits = new byte[streamLength];
                memory.Seek(0, SeekOrigin.Begin);
                memory.Read(imageBits, 0, streamLength);
                memory.Close();
#elif UWP
                byte[] imageBits = new byte[streamLength];
                memory.Seek(0, SeekOrigin.Begin);
                memory.Read(imageBits, 0, streamLength);
                memory.Dispose();
#endif

                int height = _image.PixelHeight;
                int width = _image.PixelWidth;

                // TODO: we could define structures for
                //   BITMAPFILEHEADER
                //   { BITMAPINFO }
                //   BITMAPINFOHEADER
                // to avoid ReadWord and ReadDWord ... (but w/o pointers this doesn't help much)

                if (ReadWord(imageBits, 0) != 0x4d42 || // "BM"
                    ReadDWord(imageBits, 2) != streamLength ||
                    ReadDWord(imageBits, 14) != 40 || // sizeof BITMAPINFOHEADER
                    ReadDWord(imageBits, 18) != width ||
                    ReadDWord(imageBits, 22) != height)
                {
                    throw new NotImplementedException("ReadTrueColorMemoryBitmap: unsupported format");
                }
                if (ReadWord(imageBits, 26) != 1 ||
                  (!hasAlpha && ReadWord(imageBits, 28) != components * bits ||
                   hasAlpha && ReadWord(imageBits, 28) != (components + 1) * bits) ||
                  ReadDWord(imageBits, 30) != 0)
                {
                    throw new NotImplementedException("ReadTrueColorMemoryBitmap: unsupported format #2");
                }

                int nFileOffset = ReadDWord(imageBits, 10);
                int logicalComponents = components;
                if (components == 4)
                    logicalComponents = 3;

                byte[] imageData = new byte[components * width * height];

                bool hasMask = false;
                bool hasAlphaMask = false;
                byte[] alphaMask = hasAlpha ? new byte[width * height] : null;
                MonochromeMask mask = hasAlpha ?
                  new MonochromeMask(width, height) : null;

                int nOffsetRead = 0;
                if (logicalComponents == 3)
                {
                    for (int y = 0; y < height; ++y)
                    {
                        int nOffsetWrite = 3 * (height - 1 - y) * width;
                        int nOffsetWriteAlpha = 0;
                        if (hasAlpha)
                        {
                            mask.StartLine(y);
                            nOffsetWriteAlpha = (height - 1 - y) * width;
                        }

                        for (int x = 0; x < width; ++x)
                        {
                            imageData[nOffsetWrite] = imageBits[nFileOffset + nOffsetRead + 2];
                            imageData[nOffsetWrite + 1] = imageBits[nFileOffset + nOffsetRead + 1];
                            imageData[nOffsetWrite + 2] = imageBits[nFileOffset + nOffsetRead];
                            if (hasAlpha)
                            {
                                mask.AddPel(imageBits[nFileOffset + nOffsetRead + 3]);
                                alphaMask[nOffsetWriteAlpha] = imageBits[nFileOffset + nOffsetRead + 3];
                                if (!hasMask || !hasAlphaMask)
                                {
                                    if (imageBits[nFileOffset + nOffsetRead + 3] != 255)
                                    {
                                        hasMask = true;
                                        if (imageBits[nFileOffset + nOffsetRead + 3] != 0)
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

                FlateDecode fd = new FlateDecode();
                if (hasMask)
                {
                    // monochrome mask is either sufficient or
                    // provided for compatibility with older reader versions
                    byte[] maskDataCompressed = fd.Encode(mask.MaskData, _document.Options.FlateEncodeMode);
                    PdfDictionary pdfMask = new PdfDictionary(_document);
                    pdfMask.Elements.SetName(Keys.Type, "/XObject");
                    pdfMask.Elements.SetName(Keys.Subtype, "/Image");

                    Owner._irefTable.Add(pdfMask);
                    pdfMask.Stream = new PdfStream(maskDataCompressed, pdfMask);
                    pdfMask.Elements[PdfStream.Keys.Length] = new PdfInteger(maskDataCompressed.Length);
                    pdfMask.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                    pdfMask.Elements[Keys.Width] = new PdfInteger(width);
                    pdfMask.Elements[Keys.Height] = new PdfInteger(height);
                    pdfMask.Elements[Keys.BitsPerComponent] = new PdfInteger(1);
                    pdfMask.Elements[Keys.ImageMask] = new PdfBoolean(true);
                    Elements[Keys.Mask] = pdfMask.Reference;
                }
                if (hasMask && hasAlphaMask && pdfVersion >= 14)
                {
                    // The image provides an alpha mask (requires Arcrobat 5.0 or higher)
                    byte[] alphaMaskCompressed = fd.Encode(alphaMask, _document.Options.FlateEncodeMode);
                    PdfDictionary smask = new PdfDictionary(_document);
                    smask.Elements.SetName(Keys.Type, "/XObject");
                    smask.Elements.SetName(Keys.Subtype, "/Image");

                    Owner._irefTable.Add(smask);
                    smask.Stream = new PdfStream(alphaMaskCompressed, smask);
                    smask.Elements[PdfStream.Keys.Length] = new PdfInteger(alphaMaskCompressed.Length);
                    smask.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                    smask.Elements[Keys.Width] = new PdfInteger(width);
                    smask.Elements[Keys.Height] = new PdfInteger(height);
                    smask.Elements[Keys.BitsPerComponent] = new PdfInteger(8);
                    smask.Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                    Elements[Keys.SMask] = smask.Reference;
                }

                byte[] imageDataCompressed = fd.Encode(imageData, _document.Options.FlateEncodeMode);

                Stream = new PdfStream(imageDataCompressed, this);
                Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataCompressed.Length);
                Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                Elements[Keys.Width] = new PdfInteger(width);
                Elements[Keys.Height] = new PdfInteger(height);
                Elements[Keys.BitsPerComponent] = new PdfInteger(8);
                // TODO: CMYK
                Elements[Keys.ColorSpace] = new PdfName("/DeviceRGB");
                if (_image.Interpolate)
                    Elements[Keys.Interpolate] = PdfBoolean.True;
            }
        }

        /* BITMAPINFOHEADER struct and byte offsets:
            typedef struct tagBITMAPINFOHEADER{
              DWORD  biSize;           // 14
              LONG   biWidth;          // 18
              LONG   biHeight;         // 22
              WORD   biPlanes;         // 26
              WORD   biBitCount;       // 28
              DWORD  biCompression;    // 30
              DWORD  biSizeImage;      // 34
              LONG   biXPelsPerMeter;  // 38
              LONG   biYPelsPerMeter;  // 42
              DWORD  biClrUsed;        // 46
              DWORD  biClrImportant;   // 50
            } BITMAPINFOHEADER, *PBITMAPINFOHEADER; 
        */

        private void ReadIndexedMemoryBitmap(int bits/*, ref bool hasAlpha*/)
        {
            int pdfVersion = Owner.Version;
            int firstMaskColor = -1, lastMaskColor = -1;
            bool segmentedColorMask = false;

            MemoryStream memory = new MemoryStream();
#if CORE_WITH_GDI
            _image._gdiImage.Save(memory, ImageFormat.Bmp);
#endif
#if GDI
            _image._gdiImage.Save(memory, ImageFormat.Bmp);
#endif
#if WPF
#if !SILVERLIGHT
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            //if (!_image._path.StartsWith("*"))
            //    encoder.Frames.Add(BitmapFrame.Create(new Uri(_image._path), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad));
            //else
            encoder.Frames.Add(BitmapFrame.Create(_image._wpfImage));
            encoder.Save(memory);
#else
            // AGHACK
            GetType();
#endif
#endif
            // THHO4THHO Use ImageImporterBMP here to avoid redundant code.

            int streamLength = (int)memory.Length;
            Debug.Assert(streamLength > 0, "Bitmap image encoding failed.");
            if (streamLength > 0)
            {
                byte[] imageBits = new byte[streamLength];
                memory.Seek(0, SeekOrigin.Begin);
                memory.Read(imageBits, 0, streamLength);
#if !UWP
                memory.Close();
#else
                memory.Dispose();
#endif

                int height = _image.PixelHeight;
                int width = _image.PixelWidth;

                if (ReadWord(imageBits, 0) != 0x4d42 || // "BM"
                  ReadDWord(imageBits, 2) != streamLength ||
                  ReadDWord(imageBits, 14) != 40 || // sizeof BITMAPINFOHEADER
#if WPF
                    // TODOWPF: bug with height and width??? With which files???
                  ReadDWord(imageBits, 18) != width ||
                  ReadDWord(imageBits, 22) != height)
#else
                  ReadDWord(imageBits, 18) != width ||
                  ReadDWord(imageBits, 22) != height)
#endif
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format");
                }
#if WPF
                // TODOWPF: bug with height and width
                width = ReadDWord(imageBits, 18);
                height = ReadDWord(imageBits, 22);
#endif
                int fileBits = ReadWord(imageBits, 28);
                if (fileBits != bits)
                {
                    if (fileBits == 1 || fileBits == 4 || fileBits == 8)
                        bits = fileBits;
                }

                if (ReadWord(imageBits, 26) != 1 ||
                    ReadWord(imageBits, 28) != bits ||
                    ReadDWord(imageBits, 30) != 0)
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format #2");
                }

                int bytesFileOffset = ReadDWord(imageBits, 10);
                const int bytesColorPaletteOffset = 0x36; // GDI+ always returns Windows bitmaps: sizeof BITMAPFILEHEADER + sizeof BITMAPINFOHEADER
                int paletteColors = ReadDWord(imageBits, 46);
                if ((bytesFileOffset - bytesColorPaletteOffset) / 4 != paletteColors)
                {
                    throw new NotImplementedException("ReadIndexedMemoryBitmap: unsupported format #3");
                }

                MonochromeMask mask = new MonochromeMask(width, height);

                bool isGray = bits == 8 && (paletteColors == 256 || paletteColors == 0);
                int isBitonal = 0; // 0: false; >0: true; <0: true (inverted)
                byte[] paletteData = new byte[3 * paletteColors];
                for (int color = 0; color < paletteColors; ++color)
                {
                    paletteData[3 * color] = imageBits[bytesColorPaletteOffset + 4 * color + 2];
                    paletteData[3 * color + 1] = imageBits[bytesColorPaletteOffset + 4 * color + 1];
                    paletteData[3 * color + 2] = imageBits[bytesColorPaletteOffset + 4 * color + 0];
                    if (isGray)
                        isGray = paletteData[3 * color] == paletteData[3 * color + 1] &&
                          paletteData[3 * color] == paletteData[3 * color + 2];

                    if (imageBits[bytesColorPaletteOffset + 4 * color + 3] < 128)
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


                if (bits == 1)
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
                    int ccittSizeG4 = DoFaxEncodingGroup4(ref tempG4, imageBits, (uint)bytesFileOffset, (uint)width, (uint)height);

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
                                    imageData[bytesOffsetWrite] = paletteData[3 * imageBits[bytesFileOffset + bytesOffsetRead]];
                                }
                                else
                                {
                                    // Store the palette index.
                                    imageData[bytesOffsetWrite] = imageBits[bytesFileOffset + bytesOffsetRead];
                                }
                                if (firstMaskColor != -1)
                                {
                                    int n = imageBits[bytesFileOffset + bytesOffsetRead];
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

                FlateDecode fd = new FlateDecode();
                if (firstMaskColor != -1 &&
                  lastMaskColor != -1)
                {
                    // Color mask requires Reader 4.0 or higher:
                    //if (!segmentedColorMask && pdfVersion >= 13)
                    if (!segmentedColorMask && pdfVersion >= 13 && !isGray)
                    {
                        PdfArray array = new PdfArray(_document);
                        array.Elements.Add(new PdfInteger(firstMaskColor));
                        array.Elements.Add(new PdfInteger(lastMaskColor));
                        Elements[Keys.Mask] = array;
                    }
                    else
                    {
                        // Monochrome mask
                        byte[] maskDataCompressed = fd.Encode(mask.MaskData, _document.Options.FlateEncodeMode);
                        PdfDictionary pdfMask = new PdfDictionary(_document);
                        pdfMask.Elements.SetName(Keys.Type, "/XObject");
                        pdfMask.Elements.SetName(Keys.Subtype, "/Image");

                        Owner._irefTable.Add(pdfMask);
                        pdfMask.Stream = new PdfStream(maskDataCompressed, pdfMask);
                        pdfMask.Elements[PdfStream.Keys.Length] = new PdfInteger(maskDataCompressed.Length);
                        pdfMask.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                        pdfMask.Elements[Keys.Width] = new PdfInteger(width);
                        pdfMask.Elements[Keys.Height] = new PdfInteger(height);
                        pdfMask.Elements[Keys.BitsPerComponent] = new PdfInteger(1);
                        pdfMask.Elements[Keys.ImageMask] = new PdfBoolean(true);
                        Elements[Keys.Mask] = pdfMask.Reference;
                    }
                }

                byte[] imageDataCompressed = fd.Encode(imageData, _document.Options.FlateEncodeMode);
                byte[] imageDataFaxCompressed = isFaxEncoding ? fd.Encode(imageDataFax, _document.Options.FlateEncodeMode) : null;

                bool usesCcittEncoding = false;
                if (isFaxEncoding &&
                  (imageDataFax.Length < imageDataCompressed.Length ||
                  imageDataFaxCompressed.Length < imageDataCompressed.Length))
                {
                    // /CCITTFaxDecode creates the smaller file (with or without /FlateDecode):
                    usesCcittEncoding = true;

                    if (imageDataFax.Length < imageDataCompressed.Length)
                    {
                        Stream = new PdfStream(imageDataFax, this);
                        Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataFax.Length);
                        Elements[PdfStream.Keys.Filter] = new PdfName("/CCITTFaxDecode");
                        //PdfArray array2 = new PdfArray(_document);
                        PdfDictionary dictionary = new PdfDictionary();
                        if (k != 0)
                            dictionary.Elements.Add("/K", new PdfInteger(k));
                        if (isBitonal < 0)
                            dictionary.Elements.Add("/BlackIs1", new PdfBoolean(true));
                        dictionary.Elements.Add("/EndOfBlock", new PdfBoolean(false));
                        dictionary.Elements.Add("/Columns", new PdfInteger(width));
                        dictionary.Elements.Add("/Rows", new PdfInteger(height));
                        //array2.Elements.Add(dictionary);
                        Elements[PdfStream.Keys.DecodeParms] = dictionary; // array2;
                    }
                    else
                    {
                        Stream = new PdfStream(imageDataFaxCompressed, this);
                        Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataFaxCompressed.Length);
                        PdfArray arrayFilters = new PdfArray(_document);
                        arrayFilters.Elements.Add(new PdfName("/FlateDecode"));
                        arrayFilters.Elements.Add(new PdfName("/CCITTFaxDecode"));
                        Elements[PdfStream.Keys.Filter] = arrayFilters;
                        PdfArray arrayDecodeParms = new PdfArray(_document);

                        PdfDictionary dictFlateDecodeParms = new PdfDictionary();
                        //dictFlateDecodeParms.Elements.Add("/Columns", new PdfInteger(1));

                        PdfDictionary dictCcittFaxDecodeParms = new PdfDictionary();
                        if (k != 0)
                            dictCcittFaxDecodeParms.Elements.Add("/K", new PdfInteger(k));
                        if (isBitonal < 0)
                            dictCcittFaxDecodeParms.Elements.Add("/BlackIs1", new PdfBoolean(true));
                        dictCcittFaxDecodeParms.Elements.Add("/EndOfBlock", new PdfBoolean(false));
                        dictCcittFaxDecodeParms.Elements.Add("/Columns", new PdfInteger(width));
                        dictCcittFaxDecodeParms.Elements.Add("/Rows", new PdfInteger(height));

                        arrayDecodeParms.Elements.Add(dictFlateDecodeParms); // How to add the "null object"?
                        arrayDecodeParms.Elements.Add(dictCcittFaxDecodeParms);
                        Elements[PdfStream.Keys.DecodeParms] = arrayDecodeParms;
                    }
                }
                else
                {
                    // /FlateDecode creates the smaller file (or no monochrome bitmap):
                    Stream = new PdfStream(imageDataCompressed, this);
                    Elements[PdfStream.Keys.Length] = new PdfInteger(imageDataCompressed.Length);
                    Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                }

                Elements[Keys.Width] = new PdfInteger(width);
                Elements[Keys.Height] = new PdfInteger(height);
                Elements[Keys.BitsPerComponent] = new PdfInteger(bits);
                // TODO: CMYK

                // CCITT encoding: we need color palette for isBitonal == 0
                // FlateDecode: we need color palette for isBitonal <= 0 unless we have grayscales
                if ((usesCcittEncoding && isBitonal == 0) ||
                  (!usesCcittEncoding && isBitonal <= 0 && !isGray))
                {
                    PdfDictionary colorPalette = null;
                    colorPalette = new PdfDictionary(_document);
                    byte[] packedPaletteData = paletteData.Length >= 48 ? fd.Encode(paletteData, _document.Options.FlateEncodeMode) : null; // don't compress small palettes
                    if (packedPaletteData != null && packedPaletteData.Length + 20 < paletteData.Length) // +20: compensate for the overhead (estimated value)
                    {
                        // Create compressed color palette:
                        colorPalette.CreateStream(packedPaletteData);
                        colorPalette.Elements[PdfStream.Keys.Length] = new PdfInteger(packedPaletteData.Length);
                        colorPalette.Elements[PdfStream.Keys.Filter] = new PdfName("/FlateDecode");
                    }
                    else
                    {
                        // Create uncompressed color palette:
                        colorPalette.CreateStream(paletteData);
                        colorPalette.Elements[PdfStream.Keys.Length] = new PdfInteger(paletteData.Length);
                    }
                    Owner._irefTable.Add(colorPalette);

                    PdfArray arrayColorSpace = new PdfArray(_document);
                    arrayColorSpace.Elements.Add(new PdfName("/Indexed"));
                    arrayColorSpace.Elements.Add(new PdfName("/DeviceRGB"));
                    arrayColorSpace.Elements.Add(new PdfInteger(paletteColors - 1));
                    arrayColorSpace.Elements.Add(colorPalette.Reference);
                    Elements[Keys.ColorSpace] = arrayColorSpace;
                }
                else
                {
                    Elements[Keys.ColorSpace] = new PdfName("/DeviceGray");
                }
                if (_image.Interpolate)
                    Elements[Keys.Interpolate] = PdfBoolean.True;
            }
        }

        /// <summary>
        /// Common keys for all streams.
        /// </summary>
        public sealed new class Keys : PdfXObject.Keys
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes;
            /// if present, must be XObject for an image XObject.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The type of XObject that this dictionary describes;
            /// must be Image for an image XObject.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string Subtype = "/Subtype";

            /// <summary>
            /// (Required) The width of the image, in samples.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Width = "/Width";

            /// <summary>
            /// (Required) The height of the image, in samples.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string Height = "/Height";

            /// <summary>
            /// (Required for images, except those that use the JPXDecode filter; not allowed for image masks)
            /// The color space in which image samples are specified; it can be any type of color space except
            /// Pattern. If the image uses the JPXDecode filter, this entry is optional:
            /// • If ColorSpace is present, any color space specifications in the JPEG2000 data are ignored.
            /// • If ColorSpace is absent, the color space specifications in the JPEG2000 data are used.
            ///   The Decode array is also ignored unless ImageMask is true.
            /// </summary>
            [KeyInfo(KeyType.NameOrArray | KeyType.Required)]
            public const string ColorSpace = "/ColorSpace";

            /// <summary>
            /// (Required except for image masks and images that use the JPXDecode filter)
            /// The number of bits used to represent each color component. Only a single value may be specified;
            /// the number of bits is the same for all color components. Valid values are 1, 2, 4, 8, and 
            /// (in PDF 1.5) 16. If ImageMask is true, this entry is optional, and if specified, its value 
            /// must be 1.
            /// If the image stream uses a filter, the value of BitsPerComponent must be consistent with the 
            /// size of the data samples that the filter delivers. In particular, a CCITTFaxDecode or JBIG2Decode 
            /// filter always delivers 1-bit samples, a RunLengthDecode or DCTDecode filter delivers 8-bit samples,
            /// and an LZWDecode or FlateDecode filter delivers samples of a specified size if a predictor function
            /// is used.
            /// If the image stream uses the JPXDecode filter, this entry is optional and ignored if present.
            /// The bit depth is determined in the process of decoding the JPEG2000 image.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string BitsPerComponent = "/BitsPerComponent";

            /// <summary>
            /// (Optional; PDF 1.1) The name of a color rendering intent to be used in rendering the image.
            /// Default value: the current rendering intent in the graphics state.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Intent = "/Intent";

            /// <summary>
            /// (Optional) A flag indicating whether the image is to be treated as an image mask.
            /// If this flag is true, the value of BitsPerComponent must be 1 and Mask and ColorSpace should
            /// not be specified; unmasked areas are painted using the current nonstroking color.
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string ImageMask = "/ImageMask";

            /// <summary>
            /// (Optional except for image masks; not allowed for image masks; PDF 1.3)
            /// An image XObject defining an image mask to be applied to this image, or an array specifying 
            /// a range of colors to be applied to it as a color key mask. If ImageMask is true, this entry
            /// must not be present.
            /// </summary>
            [KeyInfo(KeyType.StreamOrArray | KeyType.Optional)]
            public const string Mask = "/Mask";

            /// <summary>
            /// (Optional) An array of numbers describing how to map image samples into the range of values
            /// appropriate for the image’s color space. If ImageMask is true, the array must be either
            /// [0 1] or [1 0]; otherwise, its length must be twice the number of color components required 
            /// by ColorSpace. If the image uses the JPXDecode filter and ImageMask is false, Decode is ignored.
            /// Default value: see “Decode Arrays”.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Decode = "/Decode";

            /// <summary>
            /// (Optional) A flag indicating whether image interpolation is to be performed. 
            /// Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string Interpolate = "/Interpolate";

            /// <summary>
            /// (Optional; PDF 1.3) An array of alternate image dictionaries for this image. The order of 
            /// elements within the array has no significance. This entry may not be present in an image 
            /// XObject that is itself an alternate image.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Alternates = "/Alternates";

            /// <summary>
            /// (Optional; PDF 1.4) A subsidiary image XObject defining a soft-mask image to be used as a 
            /// source of mask shape or mask opacity values in the transparent imaging model. The alpha 
            /// source parameter in the graphics state determines whether the mask values are interpreted as
            /// shape or opacity. If present, this entry overrides the current soft mask in the graphics state,
            /// as well as the image’s Mask entry, if any. (However, the other transparency related graphics 
            /// state parameters — blend mode and alpha constant — remain in effect.) If SMask is absent, the 
            /// image has no associated soft mask (although the current soft mask in the graphics state may
            /// still apply).
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string SMask = "/SMask";

            /// <summary>
            /// (Optional for images that use the JPXDecode filter, meaningless otherwise; PDF 1.5)
            /// A code specifying how soft-mask information encoded with image samples should be used:
            /// 0 If present, encoded soft-mask image information should be ignored.
            /// 1 The image’s data stream includes encoded soft-mask values. An application can create
            ///   a soft-mask image from the information to be used as a source of mask shape or mask 
            ///   opacity in the transparency imaging model.
            /// 2 The image’s data stream includes color channels that have been preblended with a 
            ///   background; the image data also includes an opacity channel. An application can create
            ///   a soft-mask image with a Matte entry from the opacity channel information to be used as
            ///   a source of mask shape or mask opacity in the transparency model. If this entry has a 
            ///   nonzero value, SMask should not be specified.
            /// Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string SMaskInData = "/SMaskInData";

            /// <summary>
            /// (Required in PDF 1.0; optional otherwise) The name by which this image XObject is 
            /// referenced in the XObject subdictionary of the current resource dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Name = "/Name";

            /// <summary>
            /// (Required if the image is a structural content item; PDF 1.3) The integer key of the 
            /// image’s entry in the structural parent tree.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public const string StructParent = "/StructParent";

            /// <summary>
            /// (Optional; PDF 1.3; indirect reference preferred) The digital identifier of the image’s
            /// parent Web Capture content set.
            /// </summary>
            [KeyInfo(KeyType.String | KeyType.Optional)]
            public const string ID = "/ID";

            /// <summary>
            /// (Optional; PDF 1.2) An OPI version dictionary for the image. If ImageMask is true, 
            /// this entry is ignored.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string OPI = "/OPI";

            /// <summary>
            /// (Optional; PDF 1.4) A metadata stream containing metadata for the image.
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Optional)]
            public const string Metadata = "/Metadata";

            /// <summary>
            /// (Optional; PDF 1.5) An optional content group or optional content membership dictionary,
            /// specifying the optional content properties for this image XObject. Before the image is
            /// processed, its visibility is determined based on this entry. If it is determined to be 
            /// invisible, the entire image is skipped, as if there were no Do operator to invoke it.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string OC = "/OC";

            // ReSharper restore InconsistentNaming
        }
    }

    /// <summary>
    /// Helper class for creating bitmap masks (8 pels per byte).
    /// </summary>
    class MonochromeMask
    {
        /// <summary>
        /// Returns the bitmap mask that will be written to PDF.
        /// </summary>
        public byte[] MaskData
        {
            get { return _maskData; }
        }
        private readonly byte[] _maskData;

        /// <summary>
        /// Creates a bitmap mask.
        /// </summary>
        public MonochromeMask(int sizeX, int sizeY)
        {
            _sizeX = sizeX;
            _sizeY = sizeY;
            int byteSize = ((sizeX + 7) / 8) * sizeY;
            _maskData = new byte[byteSize];
            StartLine(0);
        }

        /// <summary>
        /// Starts a new line.
        /// </summary>
        public void StartLine(int newCurrentLine)
        {
            _bitsWritten = 0;
            _byteBuffer = 0;
            _writeOffset = ((_sizeX + 7) / 8) * (_sizeY - 1 - newCurrentLine);
        }

        /// <summary>
        /// Adds a pel to the current line.
        /// </summary>
        /// <param name="isTransparent"></param>
        public void AddPel(bool isTransparent)
        {
            if (_bitsWritten < _sizeX)
            {
                // Mask: 0: opaque, 1: transparent (default mapping)
                if (isTransparent)
                    _byteBuffer = (_byteBuffer << 1) + 1;
                else
                    _byteBuffer = _byteBuffer << 1;
                ++_bitsWritten;
                if ((_bitsWritten & 7) == 0)
                {
                    _maskData[_writeOffset] = (byte)_byteBuffer;
                    ++_writeOffset;
                    _byteBuffer = 0;
                }
                else if (_bitsWritten == _sizeX)
                {
                    int n = 8 - (_bitsWritten & 7);
                    _byteBuffer = _byteBuffer << n;
                    _maskData[_writeOffset] = (byte)_byteBuffer;
                }
            }
        }

        /// <summary>
        /// Adds a pel from an alpha mask value.
        /// </summary>
        public void AddPel(int shade)
        {
            // NYI: dithering!!!
            AddPel(shade < 128);
        }

        private readonly int _sizeX;
        private readonly int _sizeY;
        private int _writeOffset;
        private int _byteBuffer;
        private int _bitsWritten;
    }
}
