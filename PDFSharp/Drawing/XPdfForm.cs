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
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
#endif
#if WPF
using System.Windows.Media;
#endif
using PdfSharp.Internal;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfSharp.Drawing
{
    /// <summary>
    /// Represents a so called 'PDF form external object', which is typically an imported page of an external
    /// PDF document. XPdfForm objects are used like images to draw an existing PDF page of an external
    /// document in the current document. XPdfForm objects can only be placed in PDF documents. If you try
    /// to draw them using a XGraphics based on an GDI+ context no action is taken if no placeholder image
    /// is specified. Otherwise the place holder is drawn.
    /// </summary>
    public class XPdfForm : XForm
    {
        /// <summary>
        /// Initializes a new instance of the XPdfForm class from the specified path to an external PDF document.
        /// Although PDFsharp internally caches XPdfForm objects it is recommended to reuse XPdfForm objects
        /// in your code and change the PageNumber property if more than one page is needed form the external
        /// document. Furthermore, because XPdfForm can occupy very much memory, it is recommended to
        /// dispose XPdfForm objects if not needed anymore.
        /// </summary>
        internal XPdfForm(string path)
        {
            int pageNumber;
            path = ExtractPageNumber(path, out pageNumber);

#if !NETFX_CORE
            path = Path.GetFullPath(path);
            if (!File.Exists(path))
                throw new FileNotFoundException(PSSR.FileNotFound(path));
#endif

            if (PdfReader.TestPdfFile(path) == 0)
                throw new ArgumentException("The specified file has no valid PDF file header.", "path");

            _path = path;
            if (pageNumber != 0)
                PageNumber = pageNumber;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XPdfForm"/> class from a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        internal XPdfForm(Stream stream)
        {
            // Create a dummy unique path
            _path = "*" + Guid.NewGuid().ToString("B");

            if (PdfReader.TestPdfFile(stream) == 0)
                throw new ArgumentException("The specified stream has no valid PDF file header.", "stream");

            _externalDocument = PdfReader.Open(stream);
        }

        /// <summary>
        /// Creates an XPdfForm from a file.
        /// </summary>
        public static new XPdfForm FromFile(string path)
        {
            // TODO: Same file should return same object (that's why the function is static).
            return new XPdfForm(path);
        }

        /// <summary>
        /// Creates an XPdfForm from a stream.
        /// </summary>
        public static new XPdfForm FromStream(Stream stream)
        {
            return new XPdfForm(stream);
        }

        /*
            void Initialize()
            {
              // ImageFormat has no overridden Equals...
            }
        */

        /// <summary>
        /// Sets the form in the state FormState.Finished.
        /// </summary>
        internal override void Finish()
        {
            if (_formState == FormState.NotATemplate || _formState == FormState.Finished)
                return;

            base.Finish();

            //if (Gfx.metafile != null)
            //  image = Gfx.metafile;

            //Debug.Assert(_fromState == FormState.Created || _fromState == FormState.UnderConstruction);
            //_fromState = FormState.Finished;
            //Gfx.Dispose();
            //Gfx = null;

            //if (_pdfRenderer != null)
            //{
            //  _pdfForm.Stream = new PdfDictionary.PdfStream(PdfEncoders.RawEncoding.GetBytes(pdfRenderer.GetContent()), this.pdfForm);

            //  if (_document.Options.CompressContentStreams)
            //  {
            //    _pdfForm.Stream.Value = Filtering.FlateDecode.Encode(pdfForm.Stream.Value);
            //    _pdfForm.Elements["/Filter"] = new PdfName("/FlateDecode");
            //  }
            //  int length = _pdfForm.Stream.Length;
            //  _pdfForm.Elements.SetInteger("/Length", length);
            //}
        }

        /// <summary>
        /// Frees the memory occupied by the underlying imported PDF document, even if other XPdfForm objects
        /// refer to this document. A reuse of this object doesn't fail, because the underlying PDF document
        /// is re-imported if necessary.
        /// </summary>
        // TODO: NYI: Dispose
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                try
                {
                    if (disposing)
                    {
                        //...
                    }
                    if (_externalDocument != null)
                        PdfDocument.Tls.DetachDocument(_externalDocument.Handle);
                    //...
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }
        }
        bool _disposed;

        /// <summary>
        /// Gets or sets an image that is used for drawing if the current XGraphics object cannot handle
        /// PDF forms. A place holder is useful for showing a preview of a page on the display, because
        /// PDFsharp cannot render native PDF objects.
        /// </summary>
        public XImage PlaceHolder
        {
            get { return _placeHolder; }
            set { _placeHolder = value; }
        }
        XImage _placeHolder;

        /// <summary>
        /// Gets the underlying PdfPage (if one exists).
        /// </summary>
        public PdfPage Page
        {
            get
            {
                if (IsTemplate)
                    return null;
                PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                return page;
            }
        }

        /// <summary>
        /// Gets the number of pages in the PDF form.
        /// </summary>
        public int PageCount
        {
            get
            {
                if (IsTemplate)
                    return 1;
                if (_pageCount == -1)
                    _pageCount = ExternalDocument.Pages.Count;
                return _pageCount;
            }
        }
        int _pageCount = -1;

        /// <summary>
        /// Gets the width in point of the page identified by the property PageNumber.
        /// </summary>
        [Obsolete("Use either PixelWidth or PointWidth. Temporarily obsolete because of rearrangements for WPF.")]
        public override double Width
        {
            get
            {
                PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                return page.Width;
            }
        }

        /// <summary>
        /// Gets the height in point of the page identified by the property PageNumber.
        /// </summary>
        [Obsolete("Use either PixelHeight or PointHeight. Temporarily obsolete because of rearrangements for WPF.")]
        public override double Height
        {
            get
            {
                PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                return page.Height;
            }
        }

        /// <summary>
        /// Gets the width in point of the page identified by the property PageNumber.
        /// </summary>
        public override double PointWidth
        {
            get
            {
                PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                return page.Width;
            }
        }

        /// <summary>
        /// Gets the height in point of the page identified by the property PageNumber.
        /// </summary>
        public override double PointHeight
        {
            get
            {
                PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                return page.Height;
            }
        }

        /// <summary>
        /// Gets the width in point of the page identified by the property PageNumber.
        /// </summary>
        public override int PixelWidth
        {
            get
            {
                //PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                //return (int)page.Width;
                return DoubleUtil.DoubleToInt(PointWidth);
            }
        }

        /// <summary>
        /// Gets the height in point of the page identified by the property PageNumber.
        /// </summary>
        public override int PixelHeight
        {
            get
            {
                //PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                //return (int)page.Height;
                return DoubleUtil.DoubleToInt(PointHeight);
            }
        }

        /// <summary>
        /// Get the size of the page identified by the property PageNumber.
        /// </summary>
        public override XSize Size
        {
            get
            {
                PdfPage page = ExternalDocument.Pages[_pageNumber - 1];
                return new XSize(page.Width, page.Height);
            }
        }

        /// <summary>
        /// Gets or sets the transformation matrix.
        /// </summary>
        public override XMatrix Transform
        {
            get { return _transform; }
            set
            {
                if (_transform != value)
                {
                    // discard PdfFromXObject when Transform changed
                    _pdfForm = null;
                    _transform = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the page number in the external PDF document this object refers to. The page number
        /// is one-based, i.e. it is in the range from 1 to PageCount. The default value is 1.
        /// </summary>
        public int PageNumber
        {
            get { return _pageNumber; }
            set
            {
                if (IsTemplate)
                    throw new InvalidOperationException("The page number of an XPdfForm template cannot be modified.");

                if (_pageNumber != value)
                {
                    _pageNumber = value;
                    // dispose PdfFromXObject when number has changed
                    _pdfForm = null;
                }
            }
        }
        int _pageNumber = 1;

        /// <summary>
        /// Gets or sets the page index in the external PDF document this object refers to. The page index
        /// is zero-based, i.e. it is in the range from 0 to PageCount - 1. The default value is 0.
        /// </summary>
        public int PageIndex
        {
            get { return PageNumber - 1; }
            set { PageNumber = value + 1; }
        }

        /// <summary>
        /// Gets the underlying document from which pages are imported.
        /// </summary>
        internal PdfDocument ExternalDocument
        {
            // The problem is that you can ask an XPdfForm about the number of its pages before it was
            // drawn the first time. At this moment the XPdfForm doesn't know the document where it will
            // be later draw on one of its pages. To prevent the import of the same document more than
            // once, all imported documents of a thread are cached. The cache is local to the current 
            // thread and not to the appdomain, because I won't get problems in a multi-thread environment
            // that I don't understand.
            get
            {
                if (IsTemplate)
                    throw new InvalidOperationException("This XPdfForm is a template and not an imported PDF page; therefore it has no external document.");

                if (_externalDocument == null)
                    _externalDocument = PdfDocument.Tls.GetDocument(_path);
                return _externalDocument;
            }
        }
        internal PdfDocument _externalDocument;

        /// <summary>
        /// Extracts the page number if the path has the form 'MyFile.pdf#123' and returns
        /// the actual path without the number sign and the following digits.
        /// </summary>
        public static string ExtractPageNumber(string path, out int pageNumber)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            pageNumber = 0;
            int length = path.Length;
            if (length != 0)
            {
                length--;
                if (char.IsDigit(path, length))
                {
                    while (char.IsDigit(path, length) && length >= 0)
                        length--;
                    if (length > 0 && path[length] == '#')
                    {
                        // Must have at least one dot left of colon to distinguish from e.g. '#123'
                        if (path.IndexOf('.') != -1)
                        {
                            pageNumber = int.Parse(path.Substring(length + 1));
                            path = path.Substring(0, length);
                        }
                    }
                }
            }
            return path;
        }
    }
}
