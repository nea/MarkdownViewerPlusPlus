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
using System.Diagnostics;
using System.IO;
#if GDI
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using GdiPoint = System.Drawing.Point;
using GdiSize = System.Drawing.Size;
using GdiRect = System.Drawing.Rectangle;
using GdiPointF = System.Drawing.PointF;
using GdiSizeF = System.Drawing.SizeF;
using GdiRectF = System.Drawing.RectangleF;
using GdiMatrix = System.Drawing.Drawing2D.Matrix;
#endif
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PdfSharp.Windows;
using SysPoint = System.Windows.Point;
using SysSize = System.Windows.Size;
using SysRect = System.Windows.Rect;
using SysMatrix = System.Windows.Media.Matrix;
using WpfBrush = System.Windows.Media.Brush;
using WpfPen = System.Windows.Media.Pen;
#if !SILVERLIGHT
using WpfBrushes = System.Windows.Media.Brushes;
#endif
#endif
#if NETFX_CORE
using Windows.UI.Xaml.Media;
using SysPoint = Windows.Foundation.Point;
using SysSize = Windows.Foundation.Size;
using SysRect = Windows.Foundation.Rect;
#endif
#if UWP
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using SysPoint = Windows.Foundation.Point;
using SysSize = Windows.Foundation.Size;
using SysRect = Windows.Foundation.Rect;
#endif
using PdfSharp.Pdf;
using PdfSharp.Drawing.Pdf;
using PdfSharp.Internal;
using PdfSharp.Pdf.Advanced;

#pragma warning disable 1587
// ReSharper disable UseNullPropagation
// ReSharper disable RedundantNameQualifier
// ReSharper disable UseNameofExpression

namespace PdfSharp.Drawing  // #??? aufräumen
{
    /// <summary>
    /// Holds information about the current state of the XGraphics object.
    /// </summary>
    [Flags]
    enum InternalGraphicsMode
    {
        DrawingGdiGraphics,
        DrawingPdfContent,
        DrawingBitmap,
    }

    /// <summary>
    /// Represents a drawing surface for a fixed size page.
    /// </summary>
    public sealed class XGraphics : IDisposable
    {
#if CORE
        // TODO: Implement better concept of a measure context.
#endif

#if GDI
        /// <summary>
        /// Initializes a new instance of the XGraphics class.
        /// </summary>
        /// <param name="gfx">The gfx.</param>
        /// <param name="size">The size.</param>
        /// <param name="pageUnit">The page unit.</param>
        /// <param name="pageDirection">The page direction.</param>
        XGraphics(Graphics gfx, XSize size, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
            if (gfx == null)
            {
                // MigraDoc comes here when creating a MeasureContext.
                try
                {
                    Lock.EnterGdiPlus();
                    gfx = Graphics.FromHwnd(IntPtr.Zero);  // BUG: Use measure image
                }
                finally { Lock.ExitGdiPlus(); }
            }

            _gsStack = new GraphicsStateStack(this);
            TargetContext = XGraphicTargetContext.GDI;
            _gfx = gfx;
            _drawGraphics = true;
            _pageSize = new XSize(size.Width, size.Height);
            _pageUnit = pageUnit;
            switch (pageUnit)
            {
                case XGraphicsUnit.Point:
                    _pageSizePoints = new XSize(size.Width, size.Height);
                    break;

                case XGraphicsUnit.Inch:
                    _pageSizePoints = new XSize(XUnit.FromInch(size.Width), XUnit.FromInch(size.Height));
                    break;

                case XGraphicsUnit.Millimeter:
                    _pageSizePoints = new XSize(XUnit.FromMillimeter(size.Width), XUnit.FromMillimeter(size.Height));
                    break;

                case XGraphicsUnit.Centimeter:
                    _pageSizePoints = new XSize(XUnit.FromCentimeter(size.Width), XUnit.FromCentimeter(size.Height));
                    break;

                case XGraphicsUnit.Presentation:
                    _pageSizePoints = new XSize(XUnit.FromPresentation(size.Width), XUnit.FromPresentation(size.Height));
                    break;

                default:
                    throw new NotImplementedException("unit");
            }

            _pageDirection = pageDirection;
            Initialize();
        }
#endif

#if WPF && !SILVERLIGHT
        /// <summary>
        /// Initializes a new instance of the XGraphics class.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="size">The size.</param>
        /// <param name="pageUnit">The page unit.</param>
        /// <param name="pageDirection">The page direction.</param>
        XGraphics(DrawingContext dc, XSize size, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
            if (dc == null)
            {
                //throw new ArgumentNullException("dc");
                _dv = new DrawingVisual();
                dc = _dv.RenderOpen();
            }

            _gsStack = new GraphicsStateStack(this);
            TargetContext = XGraphicTargetContext.WPF;
            _dc = dc;
            _drawGraphics = true;
            _pageSize = new XSize(size.Width, size.Height);
            _pageUnit = pageUnit;
            switch (pageUnit)
            {
                case XGraphicsUnit.Point:
                    _pageSizePoints = new XSize(size.Width, size.Height);
                    break;

                case XGraphicsUnit.Inch:
                    _pageSizePoints = new XSize(XUnit.FromInch(size.Width), XUnit.FromInch(size.Height));
                    break;

                case XGraphicsUnit.Millimeter:
                    _pageSizePoints = new XSize(XUnit.FromMillimeter(size.Width), XUnit.FromMillimeter(size.Height));
                    break;

                case XGraphicsUnit.Centimeter:
                    _pageSizePoints = new XSize(XUnit.FromCentimeter(size.Width), XUnit.FromCentimeter(size.Height));
                    break;

                case XGraphicsUnit.Presentation:
                    _pageSizePoints = new XSize(XUnit.FromPresentation(size.Width), XUnit.FromPresentation(size.Height));
                    break;

                default:
                    throw new NotImplementedException("unit");
            }

            _pageDirection = pageDirection;
            Initialize();
        }
#endif

#if WPF
        /// <summary>
        /// Initializes a new instance of the XGraphics class.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="size">The size.</param>
        /// <param name="pageUnit">The page unit.</param>
        /// <param name="pageDirection">The page direction.</param>
        XGraphics(Canvas canvas, XSize size, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
            //throw new ArgumentNullException("canvas");
            if (canvas == null)
                canvas = new Canvas();

#if !SILVERLIGHT
            // Create DrawingVisual as container for the content of the page.
            _dv = new DrawingVisual();
            // Create a host that shows the visual.
            VisualPresenter vp = new VisualPresenter();
            vp.Children.Add(_dv);
            // The canvan only contains the host of the DrawingVisual.
            canvas.Children.Add(vp);
            _dc = _dv.RenderOpen();
            TargetContext = XGraphicTargetContext.WPF;
            //////VisualBrush brush = new VisualBrush(_dv);
            ////////brush.ViewboxUnits = BrushMappingMode.
            //////brush.Viewport=new Rect(new Point(), size.ToSize());
            //////brush.Viewbox=new Rect(new Point(), size.ToSize());
            ////////brush.Viewport=new Rect(new Point(), (Size)size);
            //////brush.AutoLayoutContent = true;
            //////canvas.Background = brush;
#else
            _dc = new AgDrawingContext(canvas);
#endif

            _gsStack = new GraphicsStateStack(this);
            TargetContext = XGraphicTargetContext.WPF;

            _drawGraphics = true;
            _pageSize = new XSize(size.Width, size.Height);
            _pageUnit = pageUnit;
            switch (pageUnit)
            {
                case XGraphicsUnit.Point:
                    _pageSizePoints = new XSize(size.Width, size.Height);
                    break;

                case XGraphicsUnit.Inch:
                    _pageSizePoints = new XSize(XUnit.FromInch(size.Width), XUnit.FromInch(size.Height));
                    break;

                case XGraphicsUnit.Millimeter:
                    _pageSizePoints = new XSize(XUnit.FromMillimeter(size.Width), XUnit.FromMillimeter(size.Height));
                    break;

                case XGraphicsUnit.Centimeter:
                    _pageSizePoints = new XSize(XUnit.FromCentimeter(size.Width), XUnit.FromCentimeter(size.Height));
                    break;

                case XGraphicsUnit.Presentation:
                    _pageSizePoints = new XSize(XUnit.FromPresentation(size.Width), XUnit.FromPresentation(size.Height));
                    break;

                default:
                    throw new NotImplementedException("unit");
            }

            _pageDirection = pageDirection;
            Initialize();
        }
#endif

#if UWP
        /// <summary>
        /// Initializes a new instance of the XGraphics class.
        /// </summary>
        /// <param name="canvasDrawingSession">The canvas.</param>
        /// <param name="size">The size.</param>
        /// <param name="pageUnit">The page unit.</param>
        /// <param name="pageDirection">The page direction.</param>
        XGraphics(CanvasDrawingSession canvasDrawingSession, XSize size, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
            if (canvasDrawingSession == null)
                throw new ArgumentNullException("canvasDrawingSession");

            _cds = canvasDrawingSession;

            _gsStack = new GraphicsStateStack(this);
            TargetContext = XGraphicTargetContext.WPF;

            _drawGraphics = true;
            _pageSize = new XSize(size.Width, size.Height);
            _pageUnit = pageUnit;
            switch (pageUnit)
            {
                case XGraphicsUnit.Point:
                    _pageSizePoints = new XSize(size.Width, size.Height);
                    break;

                case XGraphicsUnit.Inch:
                    _pageSizePoints = new XSize(XUnit.FromInch(size.Width), XUnit.FromInch(size.Height));
                    break;

                case XGraphicsUnit.Millimeter:
                    _pageSizePoints = new XSize(XUnit.FromMillimeter(size.Width), XUnit.FromMillimeter(size.Height));
                    break;

                case XGraphicsUnit.Centimeter:
                    _pageSizePoints = new XSize(XUnit.FromCentimeter(size.Width), XUnit.FromCentimeter(size.Height));
                    break;

                case XGraphicsUnit.Presentation:
                    _pageSizePoints = new XSize(XUnit.FromPresentation(size.Width), XUnit.FromPresentation(size.Height));
                    break;

                default:
                    throw new NotImplementedException("unit");
            }

            _pageDirection = pageDirection;
            Initialize();
        }
#endif

        /// <summary>
        /// Initializes a new instance of the XGraphics class for drawing on a PDF page.
        /// </summary>
        XGraphics(PdfPage page, XGraphicsPdfPageOptions options, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            if (page.Owner == null)
                throw new ArgumentException("You cannot draw on a page that is not owned by a PdfDocument object.", "page");

            if (page.RenderContent != null)
                throw new InvalidOperationException("An XGraphics object already exists for this page and must be disposed before a new one can be created.");

            if (page.Owner.IsReadOnly)
                throw new InvalidOperationException("Cannot create XGraphics for a page of a document that cannot be modified. Use PdfDocumentOpenMode.Modify.");

            _gsStack = new GraphicsStateStack(this);
            PdfContent content = null;
            switch (options)
            {
                case XGraphicsPdfPageOptions.Replace:
                    page.Contents.Elements.Clear();
                    goto case XGraphicsPdfPageOptions.Append;

                case XGraphicsPdfPageOptions.Prepend:
                    content = page.Contents.PrependContent();
                    break;

                case XGraphicsPdfPageOptions.Append:
                    content = page.Contents.AppendContent();
                    break;
            }
            page.RenderContent = content;

#if CORE
            TargetContext = XGraphicTargetContext.CORE;
#endif
#if GDI
            // HACK: This does not work with #MediumTrust
            //_gfx = Graphics.FromHwnd(IntPtr.Zero);  // _gfx should not be necessary anymore.
            _gfx = null;
            TargetContext = XGraphicTargetContext.GDI;
#endif
#if WPF && !SILVERLIGHT
            _dv = new DrawingVisual();
            _dc = _dv.RenderOpen();
            TargetContext = XGraphicTargetContext.WPF;
#endif
#if SILVERLIGHT
            _dc = new AgDrawingContext(new Canvas());
            TargetContext = XGraphicTargetContext.WPF;
#endif
#if GDI && WPF
            TargetContext = PdfSharp.Internal.TargetContextHelper.TargetContext;
#endif
            _renderer = new PdfSharp.Drawing.Pdf.XGraphicsPdfRenderer(page, this, options);
            _pageSizePoints = new XSize(page.Width, page.Height);
            switch (pageUnit)
            {
                case XGraphicsUnit.Point:
                    _pageSize = new XSize(page.Width, page.Height);
                    break;

                case XGraphicsUnit.Inch:
                    _pageSize = new XSize(XUnit.FromPoint(page.Width).Inch, XUnit.FromPoint(page.Height).Inch);
                    break;

                case XGraphicsUnit.Millimeter:
                    _pageSize = new XSize(XUnit.FromPoint(page.Width).Millimeter, XUnit.FromPoint(page.Height).Millimeter);
                    break;

                case XGraphicsUnit.Centimeter:
                    _pageSize = new XSize(XUnit.FromPoint(page.Width).Centimeter, XUnit.FromPoint(page.Height).Centimeter);
                    break;

                case XGraphicsUnit.Presentation:
                    _pageSize = new XSize(XUnit.FromPoint(page.Width).Presentation, XUnit.FromPoint(page.Height).Presentation);
                    break;

                default:
                    throw new NotImplementedException("unit");
            }
            _pageUnit = pageUnit;
            _pageDirection = pageDirection;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the XGraphics class used for drawing on a form.
        /// </summary>
        XGraphics(XForm form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            _form = form;
            form.AssociateGraphics(this);

            _gsStack = new GraphicsStateStack(this);
#if CORE
            TargetContext = XGraphicTargetContext.CORE;
            _drawGraphics = false;
            if (form.Owner != null)
                _renderer = new XGraphicsPdfRenderer(form, this);
            _pageSize = form.Size;
            Initialize();
#endif
#if GDI && !WPF
            try
            {
                Lock.EnterGdiPlus();
                TargetContext = XGraphicTargetContext.GDI;
                // If form.Owner is null create a meta file.
                if (form.Owner == null)
                {
                    MemoryStream stream = new MemoryStream();
                    // BUG: This Windows 1.0 technique issued an exception under Microsoft Azure.  // #???
                    using (Graphics refgfx = Graphics.FromHwnd(IntPtr.Zero))
                    {
                        IntPtr hdc = refgfx.GetHdc();
#if true_
                        // This code comes from my C++ RenderContext and checks some confusing details in connection 
                        // with metafiles.
                        //                                                                                    Display                 | LaserJet
                        //                                                                               DPI   96 : 120               | 300
                        // physical device size in MM                                                    ---------------------------------------------
                        int horzSizeMM = NativeMethods.GetDeviceCaps(hdc, NativeMethods.HORZSIZE);    // = 360 : 360               | 198 (not 210)
                        int vertSizeMM = NativeMethods.GetDeviceCaps(hdc, NativeMethods.VERTSIZE);    // = 290 : 290               | 288 (hot 297)
                                                                                                      // Cool:
                                                                                                      // My monitor is a Sony SDM-N80 and its size is EXACTLY 360mm x 290mm!!
                                                                                                      // It is an 18.1" Flat Panel LCD from 2002 and these are the values
                                                                                                      // an older display drivers reports in about 2003:
                                                                                                      //        Display  
                                                                                                      //  DPI   96 : 120
                                                                                                      //  --------------
                                                                                                      //       330 : 254
                                                                                                      //       254 : 203
                                                                                                      // Obviously my ATI driver reports the exact size of the monitor.


                        // device size in pixel
                        int horzSizePixel = NativeMethods.GetDeviceCaps(hdc, NativeMethods.HORZRES);     // = 1280 : 1280             | 4676
                        int vertSizePixel = NativeMethods.GetDeviceCaps(hdc, NativeMethods.VERTRES);     // = 1024 : 1024             | 6814

                        // 'logical' device resolution in DPI
                        int logResX = NativeMethods.GetDeviceCaps(hdc, NativeMethods.LOGPIXELSX);  // = 96 : 120                | 600
                        int logResY = NativeMethods.GetDeviceCaps(hdc, NativeMethods.LOGPIXELSY);  // = 96 : 120                | 600

                        // now we can get the 'physical' device resolution...
                        float phyResX = horzSizePixel / (horzSizeMM / 25.4f);  // = 98.521210 : 128.00000   | 599.85052
                        float phyResY = vertSizePixel / (vertSizeMM / 25.4f);  // = 102.40000 : 128.12611   | 600.95691

                        // ...and rescale the size of the meta rectangle.
                        float magicX = logResX / phyResX;                      // = 0.97440946 : 0.93750000 | 1.0002491
                        float magicY = logResY / phyResY;                      // = 0.93750000 : 0.93657720 | 0.99840766

                        // use A4 page in point
                        // adjust size of A4 page so that meta file fits with DrawImage...
                        //GdiRectF rcMagic = new GdiRectF(0, 0, magicX * form.Width, magicY * form.Height);
                        //m_PreviewMetafile = new Metafile(hdc, rcMagic, MetafileFrameUnitPoint,
                        //  EmfTypeEmfPlusOnly, L"some description");
#endif
                        GdiRectF rect = new GdiRectF(0, 0, form.PixelWidth, form.PixelHeight);
                        Metafile = new Metafile(stream, hdc, rect, MetafileFrameUnit.Pixel); //, EmfType.EmfPlusOnly);

                        // Petzold disposes the refgfx object, although the hdc is in use of the metafile
                        refgfx.ReleaseHdc(hdc);
                    } // refgfx.Dispose();

                    _gfx = Graphics.FromImage(Metafile);
                    _gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    _drawGraphics = true;
                }
                else
                {
                    Metafile = null;
                    _gfx = Graphics.FromHwnd(IntPtr.Zero);
                }
                if (form.Owner != null)
                    _renderer = new PdfSharp.Drawing.Pdf.XGraphicsPdfRenderer(form, this);
                _pageSize = form.Size;
            }
            finally { Lock.ExitGdiPlus(); }
            Initialize();
#endif
#if WPF && !GDI
            TargetContext = XGraphicTargetContext.WPF;
#if !SILVERLIGHT
            // If form.Owner is null create a meta file.
            if (form.Owner == null)
            {
                _dv = new DrawingVisual();
                _dc = _dv.RenderOpen();
                _drawGraphics = true;
            }
            else
            {
                _dv = new DrawingVisual();
                _dc = _dv.RenderOpen();
            }
            if (form.Owner != null)
                _renderer = new PdfSharp.Drawing.Pdf.XGraphicsPdfRenderer(form, this);
            _pageSize = form.Size;
            Initialize();
#else
            throw new NotImplementedException(); // AGHACK
            //Initialize();
#endif
#endif
        }

        /// <summary>
        /// Creates the measure context. This is a graphics context created only for querying measures of text.
        /// Drawing on a measure context has no effect.
        /// </summary>
        public static XGraphics CreateMeasureContext(XSize size, XGraphicsUnit pageUnit, XPageDirection pageDirection)
        {
#if CORE
            //throw new InvalidOperationException("No measure context in CORE build.");
            PdfDocument dummy = new PdfDocument();
            PdfPage page = dummy.AddPage();
            //XGraphics gfx = new XGraphics(((System.Drawing.Graphics)null, size, pageUnit, pageDirection);
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append, pageUnit, pageDirection);
            return gfx;
#endif
#if GDI && !WPF
            //XGraphics gfx = new XGraphics((System.Drawing.Graphics)null, size, pageUnit, pageDirection);
            XGraphics gfx = new XGraphics((System.Drawing.Graphics)null, size, pageUnit, pageDirection);
            return gfx;
#endif
#if WPF && !SILVERLIGHT
            XGraphics gfx = new XGraphics((System.Windows.Media.DrawingContext)null, size, pageUnit, pageDirection);
            return gfx;
#endif
#if SILVERLIGHT
            XGraphics gfx = new XGraphics(new Canvas(), size, pageUnit, pageDirection);
            return gfx;
#endif
#if NETFX_CORE || UWP // NETFX_CORE_TODO
            return null;
#endif
        }

#if GDI
        /// <summary>
        /// Creates a new instance of the XGraphics class from a System.Drawing.Graphics object.
        /// </summary>
        public static XGraphics FromGraphics(Graphics graphics, XSize size)
        {
            // Creating a new instance is by design.
            return new XGraphics(graphics, size, XGraphicsUnit.Point, XPageDirection.Downwards);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a System.Drawing.Graphics object.
        /// </summary>
        public static XGraphics FromGraphics(Graphics graphics, XSize size, XGraphicsUnit unit)
        {
            // Creating a new instance is by design.
            return new XGraphics(graphics, size, unit, XPageDirection.Downwards);
        }

        ///// <summary>
        ///// Creates a new instance of the XGraphics class from a System.Drawing.Graphics object.
        ///// </summary>
        //public static XGraphics FromGraphics(Graphics graphics, XSize size, XPageDirection pageDirection)
        //{
        //  // Creating a new instance is by design.
        //  return new XGraphics(graphics, size, XGraphicsUnit.Point, pageDirection);
        //}

        ///// <summary>
        ///// Creates a new instance of the XGraphics class from a System.Drawing.Graphics object.
        ///// </summary>
        //public static XGraphics FromGraphics(Graphics graphics, XSize size, XGraphicsUnit unit, XPageDirection pageDirection)
        //{
        //  // Creating a new instance is by design.
        //  return new XGraphics(graphics, size, XGraphicsUnit.Point, pageDirection);
        //}
#endif

#if WPF && !SILVERLIGHT
        /// <summary>
        /// Creates a new instance of the XGraphics class from a System.Windows.Media.DrawingContext object.
        /// </summary>
        public static XGraphics FromDrawingContext(DrawingContext drawingContext, XSize size, XGraphicsUnit unit)
        {
            return new XGraphics(drawingContext, size, unit, XPageDirection.Downwards);
        }
#endif

#if WPF
        /// <summary>
        /// Creates a new instance of the XGraphics class from a System.Windows.Media.DrawingContext object.
        /// </summary>
        public static XGraphics FromCanvas(Canvas canvas, XSize size, XGraphicsUnit unit)
        {
            return new XGraphics(canvas, size, unit, XPageDirection.Downwards);
        }
#endif

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Pdf.PdfPage object.
        /// </summary>
        public static XGraphics FromPdfPage(PdfPage page)
        {
            return new XGraphics(page, XGraphicsPdfPageOptions.Append, XGraphicsUnit.Point, XPageDirection.Downwards);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Pdf.PdfPage object.
        /// </summary>
        public static XGraphics FromPdfPage(PdfPage page, XGraphicsUnit unit)
        {
            return new XGraphics(page, XGraphicsPdfPageOptions.Append, unit, XPageDirection.Downwards);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Pdf.PdfPage object.
        /// </summary>
        public static XGraphics FromPdfPage(PdfPage page, XPageDirection pageDirection)
        {
            return new XGraphics(page, XGraphicsPdfPageOptions.Append, XGraphicsUnit.Point, pageDirection);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Pdf.PdfPage object.
        /// </summary>
        public static XGraphics FromPdfPage(PdfPage page, XGraphicsPdfPageOptions options)
        {
            return new XGraphics(page, options, XGraphicsUnit.Point, XPageDirection.Downwards);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Pdf.PdfPage object.
        /// </summary>
        public static XGraphics FromPdfPage(PdfPage page, XGraphicsPdfPageOptions options, XPageDirection pageDirection)
        {
            return new XGraphics(page, options, XGraphicsUnit.Point, pageDirection);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Pdf.PdfPage object.
        /// </summary>
        public static XGraphics FromPdfPage(PdfPage page, XGraphicsPdfPageOptions options, XGraphicsUnit unit)
        {
            return new XGraphics(page, options, unit, XPageDirection.Downwards);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Pdf.PdfPage object.
        /// </summary>
        public static XGraphics FromPdfPage(PdfPage page, XGraphicsPdfPageOptions options, XGraphicsUnit unit, XPageDirection pageDirection)
        {
            return new XGraphics(page, options, unit, pageDirection);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Drawing.XPdfForm object.
        /// </summary>
        public static XGraphics FromPdfForm(XPdfForm form)
        {
            if (form.Gfx != null)
                return form.Gfx;

            return new XGraphics(form);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Drawing.XForm object.
        /// </summary>
        public static XGraphics FromForm(XForm form)
        {
            if (form.Gfx != null)
                return form.Gfx;

            return new XGraphics(form);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Drawing.XForm object.
        /// </summary>
        public static XGraphics FromImage(XImage image)
        {
            return FromImage(image, XGraphicsUnit.Point);
        }

        /// <summary>
        /// Creates a new instance of the XGraphics class from a PdfSharp.Drawing.XImage object.
        /// </summary>
        public static XGraphics FromImage(XImage image, XGraphicsUnit unit)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            XBitmapImage bmImage = image as XBitmapImage;
            if (bmImage != null)
            {
#if CORE
                return null;
#endif
#if GDI && !WPF
                Graphics gfx = Graphics.FromImage(image._gdiImage);
                image.XImageState = image.XImageState | XImageState.UsedInDrawingContext;
                return new XGraphics(gfx, new XSize(image.PixelWidth, image.PixelHeight), unit, XPageDirection.Downwards);
#endif
#if WPF && !GDI
                DiagnosticsHelper.ThrowNotImplementedException("WPF image");
                return null;
#endif
#if NETFX_CORE
                DiagnosticsHelper.ThrowNotImplementedException("NETFX_CORE image");
                return null;
#endif
            }
            return null;
        }

        /// <summary>
        /// Internal setup.
        /// </summary>
        void Initialize()
        {
            _pageOrigin = new XPoint();

            double pageHeight = _pageSize.Height;
            PdfPage targetPage = PdfPage;
            XPoint trimOffset = new XPoint();
            if (targetPage != null && targetPage.TrimMargins.AreSet)
            {
                pageHeight += targetPage.TrimMargins.Top.Point + targetPage.TrimMargins.Bottom.Point;
                trimOffset = new XPoint(targetPage.TrimMargins.Left.Point, targetPage.TrimMargins.Top.Point);
            }

            XMatrix matrix = new XMatrix();
#if CORE
            // Nothing to do here.
            Debug.Assert(TargetContext == XGraphicTargetContext.CORE);
#endif
#if GDI
            if (TargetContext == XGraphicTargetContext.GDI)
            {
                try
                {
                    Lock.EnterFontFactory();
                    if (_gfx != null)
                        matrix = _gfx.Transform;

                    if (_pageUnit != XGraphicsUnit.Point)
                    {
                        switch (_pageUnit)
                        {
                            case XGraphicsUnit.Inch:
                                matrix.ScalePrepend(XUnit.InchFactor);
                                break;

                            case XGraphicsUnit.Millimeter:
                                matrix.ScalePrepend(XUnit.MillimeterFactor);
                                break;

                            case XGraphicsUnit.Centimeter:
                                matrix.ScalePrepend(XUnit.CentimeterFactor);
                                break;

                            case XGraphicsUnit.Presentation:
                                matrix.ScalePrepend(XUnit.PresentationFactor);
                                break;
                        }
                        if (_gfx != null)
                            _gfx.Transform = (GdiMatrix)matrix;
                    }
                }
                finally { Lock.ExitFontFactory(); }
            }
#endif
#if WPF
            if (TargetContext == XGraphicTargetContext.WPF)
            {
                if (_pageUnit != XGraphicsUnit.Presentation)
                {
                    switch (_pageUnit)
                    {
                        case XGraphicsUnit.Point:
                            matrix.ScalePrepend(XUnit.PointFactorWpf);
                            break;

                        case XGraphicsUnit.Inch:
                            matrix.ScalePrepend(XUnit.InchFactorWpf);
                            break;

                        case XGraphicsUnit.Millimeter:
                            matrix.ScalePrepend(XUnit.MillimeterFactorWpf);
                            break;

                        case XGraphicsUnit.Centimeter:
                            matrix.ScalePrepend(XUnit.CentimeterFactorWpf);
                            break;
                    }
                    if (!matrix.IsIdentity)
                    {
#if !SILVERLIGHT
                        MatrixTransform transform = new MatrixTransform((SysMatrix)matrix);
                        _dc.PushTransform(transform);
#else
                        MatrixTransform transform2 = new MatrixTransform();
                        transform2.Matrix = (SysMatrix)matrix;
                        _dc.PushTransform(transform2);
#endif
                    }
                }
            }
#endif
            if (_pageDirection != XPageDirection.Downwards)
                matrix.Prepend(new XMatrix(1, 0, 0, -1, 0, pageHeight));

            if (trimOffset != new XPoint())
                matrix.TranslatePrepend(trimOffset.X, -trimOffset.Y);

            DefaultViewMatrix = matrix;
            _transform = new XMatrix();
        }

        /// <summary>
        /// Releases all resources used by this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_associatedImage != null)
                    {
                        _associatedImage.DisassociateWithGraphics(this);
                        _associatedImage = null;
                    }
                }

                if (_form != null)
                    _form.Finish();
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        // GDI+ requires this to disassociate it from metafiles.
                        if (_gfx != null)
                            _gfx.Dispose();
                        _gfx = null;
                        Metafile = null;
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (_dc != null)
                {
                    _dc.Close();
#if !SILVERLIGHT
                    _dv = null;
#endif
                }
#endif
                _drawGraphics = false;

                if (_renderer != null)
                {
                    _renderer.Close();
                    _renderer = null;
                }
            }
        }
        bool _disposed;

        /// <summary>
        /// Internal hack for MigraDoc. Will be removed in further releases.
        /// Unicode support requires a global refactoring of MigraDoc and will be done in further releases.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once ConvertToAutoProperty
        public PdfFontEncoding MUH  // MigraDoc Unicode Hack...
        {
            get { return _muh; }
            set { _muh = value; }
        }
        PdfFontEncoding _muh;

        /// <summary>
        /// A value indicating whether GDI+ or WPF is used as context.
        /// </summary>
        internal XGraphicTargetContext TargetContext;

        /// <summary>
        /// Gets or sets the unit of measure used for page coordinates.
        /// CURRENTLY ONLY POINT IS IMPLEMENTED.
        /// </summary>
        public XGraphicsUnit PageUnit
        {
            get { return _pageUnit; }
            //set
            //{
            //  // TODO: other page units
            //  if (value != XGraphicsUnit.Point)
            //    throw new NotImplementedException("PageUnit must be XGraphicsUnit.Point in current implementation.");
            //}
        }
        readonly XGraphicsUnit _pageUnit;

        /// <summary>
        /// Gets or sets the a value indicating in which direction y-value grow.
        /// </summary>
        public XPageDirection PageDirection
        {
            get { return _pageDirection; }
            set
            {
                // Is there really anybody who needes the concept of XPageDirection.Upwards?
                if (value != XPageDirection.Downwards)
                    throw new NotImplementedException("PageDirection must be XPageDirection.Downwards in current implementation.");
            }
        }
        readonly XPageDirection _pageDirection;

        /// <summary>
        /// Gets the current page origin. Setting the origin is not yet implemented.
        /// </summary>
        public XPoint PageOrigin
        {
            get { return _pageOrigin; }
            set
            {
                // Is there really anybody who needes to set the page origin?
                if (value != new XPoint())
                    throw new NotImplementedException("PageOrigin cannot be modified in current implementation.");
            }
        }
        XPoint _pageOrigin;

        /// <summary>
        /// Gets the current size of the page.
        /// </summary>
        public XSize PageSize
        {
            get { return _pageSize; }
            //set
            //{
            //  //TODO
            //  throw new NotImplementedException("PageSize cannot be modified in current implementation.");
            //}
        }
        XSize _pageSize;
        XSize _pageSizePoints;

        #region Drawing

        // ----- DrawLine -----------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws a line connecting two Point structures.
        /// </summary>
        public void DrawLine(XPen pen, GdiPoint pt1, GdiPoint pt2)
        {
            // Because of overloading the cast is NOT redundant.
            DrawLine(pen, (double)pt1.X, (double)pt1.Y, (double)pt2.X, (double)pt2.Y);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a line connecting two Point structures.
        /// </summary>
        public void DrawLine(XPen pen, SysPoint pt1, SysPoint pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a line connecting two GdiPointF structures.
        /// </summary>
        public void DrawLine(XPen pen, GdiPointF pt1, GdiPointF pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }
#endif

        /// <summary>
        /// Draws a line connecting two XPoint structures.
        /// </summary>
        public void DrawLine(XPen pen, XPoint pt1, XPoint pt2)
        {
            DrawLine(pen, pt1.X, pt1.Y, pt2.X, pt2.Y);
        }

        /// <summary>
        /// Draws a line connecting the two points specified by coordinate pairs.
        /// </summary>
        public void DrawLine(XPen pen, double x1, double y1, double x2, double y2)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawLine(pen.RealizeGdiPen(), (float)x1, (float)y1, (float)x2, (float)y2);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    _dc.DrawLine(pen.RealizeWpfPen(), new SysPoint(x1, y1), new SysPoint(x2, y2));
#endif
            }

            if (_renderer != null)
                _renderer.DrawLines(pen, new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2) });
        }

        // ----- DrawLines ----------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws a series of line segments that connect an array of points.
        /// </summary>
        public void DrawLines(XPen pen, GdiPoint[] points)
        {
            DrawLines(pen, MakePointFArray(points, 0, points.Length));
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Draws a series of line segments that connect an array of points.
        /// </summary>
        public void DrawLines(XPen pen, SysPoint[] points)
        {
            DrawLines(pen, XGraphics.MakeXPointArray(points, 0, points.Length));
        }
#endif

#if GDI
        /// <summary>
        /// Draws a series of line segments that connect an array of points.
        /// </summary>
        public void DrawLines(XPen pen, GdiPointF[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < 2)
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));

            if (_drawGraphics)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    _gfx.DrawLines(pen.RealizeGdiPen(), points);
                }
                finally { Lock.ExitGdiPlus(); }
            }

            if (_renderer != null)
                _renderer.DrawLines(pen, MakeXPointArray(points, 0, points.Length));
        }
#endif

        /// <summary>
        /// Draws a series of line segments that connect an array of points.
        /// </summary>
        public void DrawLines(XPen pen, XPoint[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < 2)
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawLines(pen.RealizeGdiPen(), XGraphics.MakePointFArray(points));
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
#if !SILVERLIGHT
                    PolyLineSegment seg = new PolyLineSegment(XGraphics.MakePointArray(points), true);
#else
                    Point[] pts = XGraphics.MakePointArray(points);
                    PointCollection collection = new PointCollection();
                    foreach (Point point in pts)
                        collection.Add(point);
                    PolyLineSegment seg = new PolyLineSegment();
                    seg.Points = collection;
#endif
                    PathFigure figure = new PathFigure();
                    figure.IsFilled = false;
                    figure.StartPoint = new SysPoint(points[0].X, points[0].Y);
                    figure.Segments.Add(seg);
                    PathGeometry geo = new PathGeometry();
                    geo.Figures.Add(figure);
                    _dc.DrawGeometry(null, pen.RealizeWpfPen(), geo);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawLines(pen, points);
        }

        /// <summary>
        /// Draws a series of line segments that connect an array of x and y pairs.
        /// </summary>
        public void DrawLines(XPen pen, double x, double y, params double[] value)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (value == null)
                throw new ArgumentNullException("value");

            int length = value.Length;
            XPoint[] points = new XPoint[length / 2 + 1];
            points[0].X = x;
            points[0].Y = y;
            for (int idx = 0; idx < length / 2; idx++)
            {
                points[idx + 1].X = value[2 * idx];
                points[idx + 1].Y = value[2 * idx + 1];
            }
            DrawLines(pen, points);
        }

        // ----- DrawBezier ---------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws a Bézier spline defined by four points.
        /// </summary>
        public void DrawBezier(XPen pen, GdiPoint pt1, GdiPoint pt2, GdiPoint pt3, GdiPoint pt4)
        {
            // ReSharper disable RedundantCast because it is required
            DrawBezier(pen, (double)pt1.X, (double)pt1.Y, (double)pt2.X, (double)pt2.Y,
              (double)pt3.X, (double)pt3.Y, (double)pt4.X, (double)pt4.Y);
            // ReSharper restore RedundantCast
        }
#endif

#if WPF
        /// <summary>
        /// Draws a Bézier spline defined by four points.
        /// </summary>
        public void DrawBezier(XPen pen, SysPoint pt1, SysPoint pt2, SysPoint pt3, SysPoint pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a Bézier spline defined by four points.
        /// </summary>
        public void DrawBezier(XPen pen, GdiPointF pt1, GdiPointF pt2, GdiPointF pt3, GdiPointF pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }
#endif

        /// <summary>
        /// Draws a Bézier spline defined by four points.
        /// </summary>
        public void DrawBezier(XPen pen, XPoint pt1, XPoint pt2, XPoint pt3, XPoint pt4)
        {
            DrawBezier(pen, pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);
        }

        /// <summary>
        /// Draws a Bézier spline defined by four points.
        /// </summary>
        public void DrawBezier(XPen pen, double x1, double y1, double x2, double y2,
          double x3, double y3, double x4, double y4)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawBezier(pen.RealizeGdiPen(), (float)x1, (float)y1, (float)x2, (float)y2, (float)x3, (float)y3, (float)x4, (float)y4);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
#if !SILVERLIGHT
                    BezierSegment seg = new BezierSegment(new SysPoint(x2, y2), new SysPoint(x3, y3), new SysPoint(x4, y4), true);
#else
                    BezierSegment seg = new BezierSegment();
                    seg.Point1 = new SysPoint(x2, y2);
                    seg.Point2 = new SysPoint(x3, y3);
                    seg.Point3 = new SysPoint(x4, y4);
#endif
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = new SysPoint(x1, y1);
                    figure.Segments.Add(seg);
                    PathGeometry geo = new PathGeometry();
                    geo.Figures.Add(figure);
                    _dc.DrawGeometry(null, pen.RealizeWpfPen(), geo);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawBeziers(pen,
                  new XPoint[] { new XPoint(x1, y1), new XPoint(x2, y2), new XPoint(x3, y3), new XPoint(x4, y4) });
        }

        // ----- DrawBeziers --------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws a series of Bézier splines from an array of points.
        /// </summary>
        public void DrawBeziers(XPen pen, GdiPoint[] points)
        {
            DrawBeziers(pen, MakeXPointArray(points, 0, points.Length));
        }
#endif

#if WPF
        /// <summary>
        /// Draws a series of Bézier splines from an array of points.
        /// </summary>
        public void DrawBeziers(XPen pen, SysPoint[] points)
        {
            DrawBeziers(pen, MakeXPointArray(points, 0, points.Length));
        }
#endif

#if GDI
        /// <summary>
        /// Draws a series of Bézier splines from an array of points.
        /// </summary>
        public void DrawBeziers(XPen pen, GdiPointF[] points)
        {
            DrawBeziers(pen, MakeXPointArray(points, 0, points.Length));
        }
#endif

        /// <summary>
        /// Draws a series of Bézier splines from an array of points.
        /// </summary>
        public void DrawBeziers(XPen pen, XPoint[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            int count = points.Length;
            if (count == 0)
                return;

            if ((count - 1) % 3 != 0)
                throw new ArgumentException("Invalid number of points for bezier curves. Number must fulfil 4+3n.", "points");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawBeziers(pen.RealizeGdiPen(), MakePointFArray(points));
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = new SysPoint(points[0].X, points[0].Y);
                    for (int idx = 1; idx < count; idx += 3)
                    {
#if !SILVERLIGHT
                        BezierSegment seg = new BezierSegment(
                            new SysPoint(points[idx].X, points[idx].Y),
                            new SysPoint(points[idx + 1].X, points[idx + 1].Y),
                            new SysPoint(points[idx + 2].X, points[idx + 2].Y), true);
#else
                        BezierSegment seg = new BezierSegment();
                        seg.Point1 = new SysPoint(points[idx].X, points[idx].Y);
                        seg.Point2 = new SysPoint(points[idx + 1].X, points[idx + 1].Y);
                        seg.Point3 = new SysPoint(points[idx + 2].X, points[idx + 2].Y);
#endif
                        figure.Segments.Add(seg);
                    }
                    PathGeometry geo = new PathGeometry();
                    geo.Figures.Add(figure);
                    _dc.DrawGeometry(null, pen.RealizeWpfPen(), geo);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawBeziers(pen, points);
        }

        // ----- DrawCurve ----------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws a cardinal spline through a specified array of points.
        /// </summary>
        public void DrawCurve(XPen pen, GdiPoint[] points)
        {
            DrawCurve(pen, MakePointFArray(points, 0, points.Length), 0.5);
        }

        /// <summary>
        /// Draws a cardinal spline through a specified array of point using a specified tension.
        /// The drawing begins offset from the beginning of the array.
        /// </summary>
        public void DrawCurve(XPen pen, GdiPoint[] points, int offset, int numberOfSegments, double tension)
        {
            DrawCurve(pen, MakePointFArray(points, offset, numberOfSegments), tension);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a cardinal spline through a specified array of points.
        /// </summary>
        public void DrawCurve(XPen pen, SysPoint[] points)
        {
            DrawCurve(pen, MakeXPointArray(points, 0, points.Length), 0.5);
        }

        /// <summary>
        /// Draws a cardinal spline through a specified array of point. The drawing begins offset from the beginning of the array.
        /// </summary>
        public void DrawCurve(XPen pen, SysPoint[] points, int offset, int numberOfSegments)
        {
            DrawCurve(pen, MakeXPointArray(points, offset, numberOfSegments), 0.5);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a cardinal spline through a specified array of points.
        /// </summary>
        public void DrawCurve(XPen pen, GdiPointF[] points)
        {
            DrawCurve(pen, MakeXPointArray(points, 0, points.Length), 0.5);
        }
#endif

        /// <summary>
        /// Draws a cardinal spline through a specified array of points.
        /// </summary>
        public void DrawCurve(XPen pen, XPoint[] points)
        {
            DrawCurve(pen, points, 0.5);
        }

#if GDI
        /// <summary>
        /// Draws a cardinal spline through a specified array of points using a specified tension. 
        /// </summary>
        public void DrawCurve(XPen pen, GdiPoint[] points, double tension)
        {
            DrawCurve(pen, MakeXPointArray(points, 0, points.Length), tension);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a cardinal spline through a specified array of points using a specified tension. 
        /// </summary>
        public void DrawCurve(XPen pen, SysPoint[] points, double tension)
        {
            DrawCurve(pen, MakeXPointArray(points, 0, points.Length), tension);
        }

        /// <summary>
        /// Draws a cardinal spline through a specified array of point using a specified tension.
        /// The drawing begins offset from the beginning of the array.
        /// </summary>
        public void DrawCurve(XPen pen, SysPoint[] points, int offset, int numberOfSegments, double tension)
        {
            DrawCurve(pen, MakeXPointArray(points, offset, numberOfSegments), tension);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a cardinal spline through a specified array of points using a specified tension. 
        /// </summary>
        public void DrawCurve(XPen pen, GdiPointF[] points, double tension)
        {
            DrawCurve(pen, MakeXPointArray(points, 0, points.Length), tension);
        }

        /// <summary>
        /// Draws a cardinal spline through a specified array of point. The drawing begins offset from the beginning of the array.
        /// </summary>
        public void DrawCurve(XPen pen, GdiPointF[] points, int offset, int numberOfSegments)
        {
            DrawCurve(pen, MakeXPointArray(points, offset, numberOfSegments), 0.5);
        }

        /// <summary>
        /// Draws a cardinal spline through a specified array of point using a specified tension.
        /// The drawing begins offset from the beginning of the array.
        /// </summary>
        public void DrawCurve(XPen pen, GdiPointF[] points, int offset, int numberOfSegments, double tension)
        {
            DrawCurve(pen, MakeXPointArray(points, offset, numberOfSegments), tension);
        }
#endif

        /// <summary>
        /// Draws a cardinal spline through a specified array of point using a specified tension.
        /// The drawing begins offset from the beginning of the array.
        /// </summary>
        public void DrawCurve(XPen pen, XPoint[] points, int offset, int numberOfSegments, double tension)
        {
            XPoint[] points2 = new XPoint[numberOfSegments];
            Array.Copy(points, offset, points2, 0, numberOfSegments);
            DrawCurve(pen, points2, tension);
        }

        /// <summary>
        /// Draws a cardinal spline through a specified array of points using a specified tension. 
        /// </summary>
        public void DrawCurve(XPen pen, XPoint[] points, double tension)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");

            int count = points.Length;
            if (count < 2)
                throw new ArgumentException("DrawCurve requires two or more points.", "points");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawCurve(pen.RealizeGdiPen(), MakePointFArray(points), (float)tension);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    tension /= 3;

                    PathFigure figure = new PathFigure();
                    figure.StartPoint = new SysPoint(points[0].X, points[0].Y);
                    if (count == 2)
                    {
                        figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[0], points[0], points[1], points[1], tension));
                    }
                    else
                    {
                        figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[0], points[0], points[1], points[2], tension));
                        for (int idx = 1; idx < count - 2; idx++)
                            figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[idx - 1], points[idx], points[idx + 1], points[idx + 2], tension));
                        figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[count - 3], points[count - 2], points[count - 1], points[count - 1], tension));
                    }
                    PathGeometry geo = new PathGeometry();
                    geo.Figures.Add(figure);
                    _dc.DrawGeometry(null, pen.RealizeWpfPen(), geo);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawCurve(pen, points, tension);
        }

        // ----- DrawArc ------------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws an arc representing a portion of an ellipse.
        /// </summary>
        public void DrawArc(XPen pen, Rectangle rect, double startAngle, double sweepAngle)
        {
            // Because of overloading the cast is NOT redundant.
            DrawArc(pen, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height, startAngle, sweepAngle);
        }
#endif

#if GDI
        /// <summary>
        /// Draws an arc representing a portion of an ellipse.
        /// </summary>
        public void DrawArc(XPen pen, GdiRectF rect, double startAngle, double sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
#endif

        /// <summary>
        /// Draws an arc representing a portion of an ellipse.
        /// </summary>
        public void DrawArc(XPen pen, XRect rect, double startAngle, double sweepAngle)
        {
            DrawArc(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws an arc representing a portion of an ellipse.
        /// </summary>
        public void DrawArc(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (Math.Abs(sweepAngle) >= 360)
            {
                DrawEllipse(pen, x, y, width, height);
            }
            else
            {
                if (_drawGraphics)
                {
#if GDI
                    if (TargetContext == XGraphicTargetContext.GDI)
                    {
                        try
                        {
                            Lock.EnterGdiPlus();
                            _gfx.DrawArc(pen.RealizeGdiPen(), (float)x, (float)y, (float)width, (float)height, (float)startAngle, (float)sweepAngle);
                        }
                        finally { Lock.ExitGdiPlus(); }
                    }
#endif
#if WPF
                    if (TargetContext == XGraphicTargetContext.WPF)
                    {
                        SysPoint startPoint;
                        ArcSegment seg = GeometryHelper.CreateArcSegment(x, y, width, height, startAngle, sweepAngle, out startPoint);
                        PathFigure figure = new PathFigure();
                        figure.StartPoint = startPoint;
                        figure.Segments.Add(seg);
                        PathGeometry geo = new PathGeometry();
                        geo.Figures.Add(figure);
                        _dc.DrawGeometry(null, pen.RealizeWpfPen(), geo);
                    }
#endif
                }

                if (_renderer != null)
                    _renderer.DrawArc(pen, x, y, width, height, startAngle, sweepAngle);
            }
        }

        // ----- DrawRectangle ------------------------------------------------------------------------

        // ----- stroke -----

#if GDI
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, Rectangle rect)
        {
            // Because of overloading the cast is NOT redundant.
            DrawRectangle(pen, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, GdiRectF rect)
        {
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, XRect rect)
        {
            DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, double x, double y, double width, double height)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawRectangle(pen.RealizeGdiPen(), (float)x, (float)y, (float)width, (float)height);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    _dc.DrawRectangle(null, pen.RealizeWpfPen(), new Rect(x, y, width, height));
                }
#endif
#if UWP
                if (TargetContext == XGraphicTargetContext.UWP)
                {
                    _cds.DrawRectangle((float)x, (float)y, (float)width, (float)height, pen.Color.ToUwpColor());
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawRectangle(pen, null, x, y, width, height);
        }

        // ----- fill -----

#if GDI
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XBrush brush, Rectangle rect)
        {
            // Because of overloading the cast is NOT redundant.
            DrawRectangle(brush, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XBrush brush, GdiRectF rect)
        {
            DrawRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XBrush brush, XRect rect)
        {
            DrawRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XBrush brush, double x, double y, double width, double height)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.FillRectangle(brush.RealizeGdiBrush(), (float)x, (float)y, (float)width, (float)height);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    _dc.DrawRectangle(brush.RealizeWpfBrush(), null, new Rect(x, y, width, height));
#endif
#if UWP
                if (TargetContext == XGraphicTargetContext.UWP)
                {
                    _cds.DrawRectangle((float)x, (float)y, (float)width, (float)height, brush.RealizeCanvasBrush());
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawRectangle(null, brush, x, y, width, height);
        }

        // ----- stroke and fill -----

#if GDI
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, XBrush brush, Rectangle rect)
        {
            // Because of overloading the cast is NOT redundant.
            DrawRectangle(pen, brush, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, XBrush brush, GdiRectF rect)
        {
            DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, XBrush brush, XRect rect)
        {
            DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        public void DrawRectangle(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (brush != null)
                            _gfx.FillRectangle(brush.RealizeGdiBrush(), (float)x, (float)y, (float)width, (float)height);
                        if (pen != null)
                            _gfx.DrawRectangle(pen.RealizeGdiPen(), (float)x, (float)y, (float)width, (float)height);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    _dc.DrawRectangle(
                      brush != null ? brush.RealizeWpfBrush() : null,
                      pen != null ? pen.RealizeWpfPen() : null,
                      new Rect(x, y, width, height));
#endif
            }

            if (_renderer != null)
                _renderer.DrawRectangle(pen, brush, x, y, width, height);
        }

        // ----- DrawRectangles -----------------------------------------------------------------------

        // ----- stroke -----

#if GDI
        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XPen pen, GdiRect[] rectangles)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            DrawRectangles(pen, null, rectangles);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XPen pen, GdiRectF[] rectangles)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            DrawRectangles(pen, null, rectangles);
        }
#endif

        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XPen pen, XRect[] rectangles)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            DrawRectangles(pen, null, rectangles);
        }

        // ----- fill -----

#if GDI
        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XBrush brush, GdiRect[] rectangles)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            DrawRectangles(null, brush, rectangles);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XBrush brush, GdiRectF[] rectangles)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            DrawRectangles(null, brush, rectangles);
        }
#endif

        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XBrush brush, XRect[] rectangles)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            DrawRectangles(null, brush, rectangles);
        }

        // ----- stroke and fill -----

#if GDI
        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XPen pen, XBrush brush, Rectangle[] rectangles)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            if (_drawGraphics)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    if (brush != null)
                        _gfx.FillRectangles(brush.RealizeGdiBrush(), rectangles);
                    if (pen != null)
                        _gfx.DrawRectangles(pen.RealizeGdiPen(), rectangles);
                }
                finally { Lock.ExitGdiPlus(); }
            }
            if (_renderer != null)
            {
                int count = rectangles.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    Rectangle rect = rectangles[idx];
                    _renderer.DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }
#endif

#if GDI
        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XPen pen, XBrush brush, GdiRectF[] rectangles)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            if (_drawGraphics)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    if (brush != null)
                        _gfx.FillRectangles(brush.RealizeGdiBrush(), rectangles);
                    if (pen != null)
                        _gfx.DrawRectangles(pen.RealizeGdiPen(), rectangles);
                }
                finally { Lock.ExitGdiPlus(); }
            }
            if (_renderer != null)
            {
                int count = rectangles.Length;
                for (int idx = 0; idx < count; idx++)
                {
                    GdiRectF rect = rectangles[idx];
                    _renderer.DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }
#endif

        /// <summary>
        /// Draws a series of rectangles.
        /// </summary>
        public void DrawRectangles(XPen pen, XBrush brush, XRect[] rectangles)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            if (rectangles == null)
                throw new ArgumentNullException("rectangles");

            int count = rectangles.Length;
            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    GdiRectF[] rects = MakeRectangleFArray(rectangles, 0, rectangles.Length);
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (brush != null)
                            _gfx.FillRectangles(brush.RealizeGdiBrush(), rects);
                        if (pen != null)
                            _gfx.DrawRectangles(pen.RealizeGdiPen(), rects);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    WpfBrush wpfBrush = brush != null ? brush.RealizeWpfBrush() : null;
                    WpfPen wpfPen = pen != null ? pen.RealizeWpfPen() : null;
                    for (int idx = 0; idx < count; idx++)
                    {
                        XRect rect = rectangles[idx];
                        _dc.DrawRectangle(wpfBrush, wpfPen, new SysRect(new SysPoint(rect.X, rect.Y), new SysSize(rect.Width, rect.Height)));
                    }
                }
#endif
            }

            if (_renderer != null)
            {
                for (int idx = 0; idx < count; idx++)
                {
                    XRect rect = rectangles[idx];
                    _renderer.DrawRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
                }
            }
        }

        // ----- DrawRoundedRectangle -----------------------------------------------------------------

        // ----- stroke -----

#if GDI
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, Rectangle rect, GdiSize ellipseSize)
        {
            DrawRoundedRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, Rect rect, SysSize ellipseSize)
        {
            DrawRoundedRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, GdiRectF rect, SizeF ellipseSize)
        {
            DrawRoundedRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, XRect rect, XSize ellipseSize)
        {
            DrawRoundedRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }

        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            DrawRoundedRectangle(pen, null, x, y, width, height, ellipseWidth, ellipseHeight);
        }

        // ----- fill -----

#if GDI
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XBrush brush, Rectangle rect, GdiSize ellipseSize)
        {
            DrawRoundedRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XBrush brush, Rect rect, SysSize ellipseSize)
        {
            DrawRoundedRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XBrush brush, GdiRectF rect, SizeF ellipseSize)
        {
            DrawRoundedRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XBrush brush, XRect rect, XSize ellipseSize)
        {
            DrawRoundedRectangle(brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }

        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            DrawRoundedRectangle(null, brush, x, y, width, height, ellipseWidth, ellipseHeight);
        }

        // ----- stroke and fill -----

#if GDI
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, XBrush brush, Rectangle rect, GdiSize ellipseSize)
        {
            // ReSharper disable RedundantCast because it is required
            DrawRoundedRectangle(pen, brush, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height,
                (double)ellipseSize.Width, (double)ellipseSize.Height);
            // ReSharper restore RedundantCast
        }
#endif

#if WPF
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, XBrush brush, Rect rect, SysSize ellipseSize)
        {
            DrawRoundedRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, XBrush brush, GdiRectF rect, SizeF ellipseSize)
        {
            DrawRoundedRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }
#endif

        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, XBrush brush, XRect rect, XSize ellipseSize)
        {
            DrawRoundedRectangle(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, ellipseSize.Width, ellipseSize.Height);
        }

        /// <summary>
        /// Draws a rectangles with round corners.
        /// </summary>
        public void DrawRoundedRectangle(XPen pen, XBrush brush, double x, double y, double width, double height,
            double ellipseWidth, double ellipseHeight)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        XGraphicsPath path = new XGraphicsPath();
                        path.AddRoundedRectangle(x, y, width, height, ellipseWidth, ellipseHeight);
                        DrawPath(pen, brush, path);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    _dc.DrawRoundedRectangle(
                      brush != null ? brush.RealizeWpfBrush() : null,
                      pen != null ? pen.RealizeWpfPen() : null,
                      new Rect(x, y, width, height), ellipseWidth / 2, ellipseHeight / 2);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawRoundedRectangle(pen, brush, x, y, width, height, ellipseWidth, ellipseHeight);
        }

        // ----- DrawEllipse --------------------------------------------------------------------------

        // ----- stroke -----

#if GDI
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, Rectangle rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, GdiRectF rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, XRect rect)
        {
            DrawEllipse(pen, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, double x, double y, double width, double height)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");

            // No DrawArc defined?
            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawArc(pen.RealizeGdiPen(), (float)x, (float)y, (float)width, (float)height, 0, 360);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    double radiusX = width / 2;
                    double radiusY = height / 2;
                    _dc.DrawEllipse(null, pen.RealizeWpfPen(), new SysPoint(x + radiusX, y + radiusY), radiusX, radiusY);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawEllipse(pen, null, x, y, width, height);
        }

        // ----- fill -----

#if GDI
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XBrush brush, Rectangle rect)
        {
            DrawEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XBrush brush, GdiRectF rect)
        {
            DrawEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XBrush brush, XRect rect)
        {
            DrawEllipse(brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XBrush brush, double x, double y, double width, double height)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.FillEllipse(brush.RealizeGdiBrush(), (float)x, (float)y, (float)width, (float)height);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    double radiusX = width / 2;
                    double radiusY = height / 2;
                    _dc.DrawEllipse(brush.RealizeWpfBrush(), null, new SysPoint(x + radiusX, y + radiusY), radiusX, radiusY);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawEllipse(null, brush, x, y, width, height);
        }

        // ----- stroke and fill -----

#if GDI
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, XBrush brush, Rectangle rect)
        {
            DrawEllipse(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, XBrush brush, GdiRectF rect)
        {
            DrawEllipse(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, XBrush brush, XRect rect)
        {
            DrawEllipse(pen, brush, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws an ellipse defined by a bounding rectangle.
        /// </summary>
        public void DrawEllipse(XPen pen, XBrush brush, double x, double y, double width, double height)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (brush != null)
                            _gfx.FillEllipse(brush.RealizeGdiBrush(), (float)x, (float)y, (float)width, (float)height);
                        if (pen != null)
                            _gfx.DrawArc(pen.RealizeGdiPen(), (float)x, (float)y, (float)width, (float)height, 0, 360);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    double radiusX = width / 2;
                    double radiusY = height / 2;
                    _dc.DrawEllipse(
                        brush != null ? brush.RealizeWpfBrush() : null,
                        pen != null ? pen.RealizeWpfPen() : null,
                        new SysPoint(x + radiusX, y + radiusY), radiusX, radiusY);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawEllipse(pen, brush, x, y, width, height);
        }

        // ----- DrawPolygon --------------------------------------------------------------------------

        // ----- stroke -----

#if GDI
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, GdiPoint[] points)
        {
            DrawPolygon(pen, MakeXPointArray(points, 0, points.Length));
        }
#endif

#if WPF
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, SysPoint[] points)
        {
            DrawPolygon(pen, MakeXPointArray(points, 0, points.Length));
        }
#endif

#if GDI
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, GdiPointF[] points)
        {
            DrawPolygon(pen, MakeXPointArray(points, 0, points.Length));
        }
#endif

        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, XPoint[] points)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < 2)
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawPolygon(pen.RealizeGdiPen(), MakePointFArray(points));
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    _dc.DrawGeometry(null, pen.RealizeWpfPen(), GeometryHelper.CreatePolygonGeometry(MakePointArray(points), XFillMode.Alternate, true));
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawPolygon(pen, null, points, XFillMode.Alternate);  // XFillMode is ignored
        }

        // ----- fill -----

#if GDI
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XBrush brush, GdiPoint[] points, XFillMode fillmode)
        {
            DrawPolygon(brush, MakeXPointArray(points, 0, points.Length), fillmode);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XBrush brush, SysPoint[] points, XFillMode fillmode)
        {
            DrawPolygon(brush, MakeXPointArray(points, 0, points.Length), fillmode);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XBrush brush, GdiPointF[] points, XFillMode fillmode)
        {
            DrawPolygon(brush, MakeXPointArray(points, 0, points.Length), fillmode);
        }
#endif

        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < 2)
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.FillPolygon(brush.RealizeGdiBrush(), MakePointFArray(points), (FillMode)fillmode);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    _dc.DrawGeometry(brush.RealizeWpfBrush(), null, GeometryHelper.CreatePolygonGeometry(MakePointArray(points), fillmode, true));
#endif
            }

            if (_renderer != null)
                _renderer.DrawPolygon(null, brush, points, fillmode);
        }

        // ----- stroke and fill -----

#if GDI
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, XBrush brush, GdiPoint[] points, XFillMode fillmode)
        {
            DrawPolygon(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, XBrush brush, SysPoint[] points, XFillMode fillmode)
        {
            DrawPolygon(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, XBrush brush, GdiPointF[] points, XFillMode fillmode)
        {
            DrawPolygon(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode);
        }
#endif

        /// <summary>
        /// Draws a polygon defined by an array of points.
        /// </summary>
        public void DrawPolygon(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            if (points == null)
                throw new ArgumentNullException("points");
            if (points.Length < 2)
                throw new ArgumentException("points", PSSR.PointArrayAtLeast(2));

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    GdiPointF[] pts = MakePointFArray(points);
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (brush != null)
                            _gfx.FillPolygon(brush.RealizeGdiBrush(), pts, (FillMode)fillmode);
                        if (pen != null)
                            _gfx.DrawPolygon(pen.RealizeGdiPen(), pts);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    WpfBrush wpfBrush = brush != null ? brush.RealizeWpfBrush() : null;
                    WpfPen wpfPen = brush != null ? pen.RealizeWpfPen() : null;
                    _dc.DrawGeometry(wpfBrush, wpfPen, GeometryHelper.CreatePolygonGeometry(MakePointArray(points), fillmode, true));
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawPolygon(pen, brush, points, fillmode);
        }

        // ----- DrawPie ------------------------------------------------------------------------------

        // ----- stroke -----

#if GDI
        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, Rectangle rect, double startAngle, double sweepAngle)
        {
            // ReSharper disable RedundantCast because it is required
            DrawPie(pen, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height, startAngle, sweepAngle);
            // ReSharper restore RedundantCast
        }
#endif

#if GDI
        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, GdiRectF rect, double startAngle, double sweepAngle)
        {
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
#endif

        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, XRect rect, double startAngle, double sweepAngle)
        {
            DrawPie(pen, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (pen == null)
                throw new ArgumentNullException("pen", PSSR.NeedPenOrBrush);

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawPie(pen.RealizeGdiPen(), (float)x, (float)y, (float)width, (float)height, (float)startAngle, (float)sweepAngle);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    DrawPie(pen, null, x, y, width, height, startAngle, sweepAngle);
#endif
            }

            if (_renderer != null)
                _renderer.DrawPie(pen, null, x, y, width, height, startAngle, sweepAngle);
        }

        // ----- fill -----

#if GDI
        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XBrush brush, Rectangle rect, double startAngle, double sweepAngle)
        {
            // Because of overloading the cast is NOT redundant.
            DrawPie(brush, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height, startAngle, sweepAngle);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XBrush brush, GdiRectF rect, double startAngle, double sweepAngle)
        {
            DrawPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
#endif

        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XBrush brush, XRect rect, double startAngle, double sweepAngle)
        {
            DrawPie(brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (brush == null)
                throw new ArgumentNullException("brush", PSSR.NeedPenOrBrush);

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.FillPie(brush.RealizeGdiBrush(), (float)x, (float)y, (float)width, (float)height, (float)startAngle, (float)sweepAngle);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    DrawPie(null, brush, x, y, width, height, startAngle, sweepAngle);
#endif
            }

            if (_renderer != null)
                _renderer.DrawPie(null, brush, x, y, width, height, startAngle, sweepAngle);
        }

        // ----- stroke and fill -----

#if GDI
        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, XBrush brush, Rectangle rect, double startAngle, double sweepAngle)
        {
            DrawPie(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, XBrush brush, GdiRectF rect, double startAngle, double sweepAngle)
        {
            DrawPie(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }
#endif

        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, XBrush brush, XRect rect, double startAngle, double sweepAngle)
        {
            DrawPie(pen, brush, rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle);
        }

        /// <summary>
        /// Draws a pie defined by an ellipse.
        /// </summary>
        public void DrawPie(XPen pen, XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle)
        {
            if (pen == null && brush == null)
                throw new ArgumentNullException("pen", PSSR.NeedPenOrBrush);

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (brush != null)
                            _gfx.FillPie(brush.RealizeGdiBrush(), (float)x, (float)y, (float)width, (float)height, (float)startAngle, (float)sweepAngle);
                        if (pen != null)
                            _gfx.DrawPie(pen.RealizeGdiPen(), (float)x, (float)y, (float)width, (float)height, (float)startAngle, (float)sweepAngle);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    WpfBrush wpfBrush = brush != null ? brush.RealizeWpfBrush() : null;
                    WpfPen wpfPen = pen != null ? pen.RealizeWpfPen() : null;
                    SysPoint center = new SysPoint(x + width / 2, y + height / 2);
                    SysPoint startArc;
                    ArcSegment arc = GeometryHelper.CreateArcSegment(x, y, width, height, startAngle, sweepAngle, out startArc);
                    PathFigure figure = new PathFigure();
                    figure.StartPoint = center;
#if !SILVERLIGHT
                    LineSegment seg = new LineSegment(startArc, true);
#else
                    LineSegment seg = new LineSegment { Point = startArc };
#endif
                    figure.Segments.Add(seg);
                    figure.Segments.Add(arc);
                    figure.IsClosed = true;
                    PathGeometry geo = new PathGeometry();
                    geo.Figures.Add(figure);
                    _dc.DrawGeometry(wpfBrush, wpfPen, geo);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawPie(pen, brush, x, y, width, height, startAngle, sweepAngle);
        }

        // ----- DrawClosedCurve ----------------------------------------------------------------------

        // ----- stroke -----

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, GdiPoint[] points)
        {
            DrawClosedCurve(pen, null, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, SysPoint[] points)
        {
            DrawClosedCurve(pen, null, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, GdiPointF[] points)
        {
            DrawClosedCurve(pen, null, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XPoint[] points)
        {
            DrawClosedCurve(pen, null, points, XFillMode.Alternate, 0.5);
        }

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, GdiPoint[] points, double tension)
        {
            DrawClosedCurve(pen, null, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, tension);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, SysPoint[] points, double tension)
        {
            DrawClosedCurve(pen, null, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, tension);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, GdiPointF[] points, double tension)
        {
            DrawClosedCurve(pen, null, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, tension);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XPoint[] points, double tension)
        {
            DrawClosedCurve(pen, null, points, XFillMode.Alternate, tension);
        }

        // ----- fill -----

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, GdiPoint[] points)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, SysPoint[] points)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, GdiPointF[] points)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, XPoint[] points)
        {
            DrawClosedCurve(null, brush, points, XFillMode.Alternate, 0.5);
        }

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, GdiPoint[] points, XFillMode fillmode)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), fillmode, 0.5);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, SysPoint[] points, XFillMode fillmode)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), fillmode, 0.5);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, GdiPointF[] points, XFillMode fillmode)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), fillmode, 0.5);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            DrawClosedCurve(null, brush, points, fillmode, 0.5);
        }

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, GdiPoint[] points, XFillMode fillmode, double tension)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), fillmode, tension);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, SysPoint[] points, XFillMode fillmode, double tension)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), fillmode, tension);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, GdiPointF[] points, XFillMode fillmode, double tension)
        {
            DrawClosedCurve(null, brush, MakeXPointArray(points, 0, points.Length), fillmode, tension);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XBrush brush, XPoint[] points, XFillMode fillmode, double tension)
        {
            DrawClosedCurve(null, brush, points, fillmode, tension);
        }

        // ----- stroke and fill -----

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, GdiPoint[] points)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, SysPoint[] points)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, GdiPointF[] points)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), XFillMode.Alternate, 0.5);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points)
        {
            DrawClosedCurve(pen, brush, points, XFillMode.Alternate, 0.5);
        }

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, GdiPoint[] points, XFillMode fillmode)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode, 0.5);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, SysPoint[] points, XFillMode fillmode)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode, 0.5);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, GdiPointF[] points, XFillMode fillmode)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode, 0.5);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode)
        {
            DrawClosedCurve(pen, brush, points, fillmode, 0.5);
        }

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, GdiPoint[] points, XFillMode fillmode, double tension)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode, tension);
        }
#endif

#if WPF
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, SysPoint[] points, XFillMode fillmode, double tension)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode, tension);
        }
#endif

#if GDI
        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, GdiPointF[] points, XFillMode fillmode, double tension)
        {
            DrawClosedCurve(pen, brush, MakeXPointArray(points, 0, points.Length), fillmode, tension);
        }
#endif

        /// <summary>
        /// Draws a closed cardinal spline defined by an array of points.
        /// </summary>
        public void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode, double tension)
        {
            if (pen == null && brush == null)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }

            int count = points.Length;
            if (count == 0)
                return;
            if (count < 2)
                throw new ArgumentException("Not enough points.", "points");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (brush != null)
                            _gfx.FillClosedCurve(brush.RealizeGdiBrush(), MakePointFArray(points), (FillMode)fillmode, (float)tension);
                        if (pen != null)
                        {
                            // The fillmode is not used by DrawClosedCurve.
                            _gfx.DrawClosedCurve(pen.RealizeGdiPen(), MakePointFArray(points), (float)tension, (FillMode)fillmode);
                        }
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    tension /= 3; // Simply tried out. Not proofed why it is correct.

                    PathFigure figure = new PathFigure();
                    figure.IsClosed = true;
                    figure.StartPoint = new SysPoint(points[0].X, points[0].Y);
                    if (count == 2)
                    {
                        figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[0], points[0], points[1], points[1], tension));
                    }
                    else
                    {
                        figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[count - 1], points[0], points[1], points[2], tension));
                        for (int idx = 1; idx < count - 2; idx++)
                            figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[idx - 1], points[idx], points[idx + 1], points[idx + 2], tension));
                        figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[count - 3], points[count - 2], points[count - 1], points[0], tension));
                        figure.Segments.Add(GeometryHelper.CreateCurveSegment(points[count - 2], points[count - 1], points[0], points[1], tension));
                    }
                    PathGeometry geo = new PathGeometry();
                    geo.FillRule = fillmode == XFillMode.Alternate ? FillRule.EvenOdd : FillRule.Nonzero;
                    geo.Figures.Add(figure);
                    WpfBrush wpfBrush = brush != null ? brush.RealizeWpfBrush() : null;
                    WpfPen wpfPen = pen != null ? pen.RealizeWpfPen() : null;
                    _dc.DrawGeometry(wpfBrush, wpfPen, geo);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawClosedCurve(pen, brush, points, tension, fillmode);
        }

        // ----- DrawPath -----------------------------------------------------------------------------

        // ----- stroke -----

        /// <summary>
        /// Draws a graphical path.
        /// </summary>
        public void DrawPath(XPen pen, XGraphicsPath path)
        {
            if (pen == null)
                throw new ArgumentNullException("pen");
            if (path == null)
                throw new ArgumentNullException("path");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.DrawPath(pen.RealizeGdiPen(), path._gdipPath);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    _dc.DrawGeometry(null, pen.RealizeWpfPen(), path._pathGeometry);
#endif
            }

            if (_renderer != null)
                _renderer.DrawPath(pen, null, path);
        }

        // ----- fill -----

        /// <summary>
        /// Draws a graphical path.
        /// </summary>
        public void DrawPath(XBrush brush, XGraphicsPath path)
        {
            if (brush == null)
                throw new ArgumentNullException("brush");
            if (path == null)
                throw new ArgumentNullException("path");

            if (_drawGraphics)
            {
#if GDI
                // $TODO THHO Lock???
                if (TargetContext == XGraphicTargetContext.GDI)
                    _gfx.FillPath(brush.RealizeGdiBrush(), path._gdipPath);
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                    _dc.DrawGeometry(brush.RealizeWpfBrush(), null, path._pathGeometry);
#endif
            }

            if (_renderer != null)
                _renderer.DrawPath(null, brush, path);
        }

        // ----- stroke and fill -----

        /// <summary>
        /// Draws a graphical path.
        /// </summary>
        public void DrawPath(XPen pen, XBrush brush, XGraphicsPath path)
        {
            if (pen == null && brush == null)
            {
                // ReSharper disable once NotResolvedInText
                throw new ArgumentNullException("pen and brush", PSSR.NeedPenOrBrush);
            }
            if (path == null)
                throw new ArgumentNullException("path");

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (brush != null)
                            _gfx.FillPath(brush.RealizeGdiBrush(), path._gdipPath);
                        if (pen != null)
                            _gfx.DrawPath(pen.RealizeGdiPen(), path._gdipPath);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    WpfBrush wpfBrush = brush != null ? brush.RealizeWpfBrush() : null;
                    WpfPen wpfPen = pen != null ? pen.RealizeWpfPen() : null;
                    _dc.DrawGeometry(wpfBrush, wpfPen, path._pathGeometry);
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawPath(pen, brush, path);
        }

        // ----- DrawString ---------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, GdiPointF point)
        {
            DrawString(s, font, brush, new XRect(point.X, point.Y, 0, 0), XStringFormats.Default);
        }
#endif

        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, XPoint point)
        {
            DrawString(s, font, brush, new XRect(point.X, point.Y, 0, 0), XStringFormats.Default);
        }

#if GDI
        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, GdiPointF point, XStringFormat format)
        {
            DrawString(s, font, brush, new XRect(point.X, point.Y, 0, 0), format);
        }
#endif

        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, XPoint point, XStringFormat format)
        {
            DrawString(s, font, brush, new XRect(point.X, point.Y, 0, 0), format);
        }

        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, double x, double y)
        {
            DrawString(s, font, brush, new XRect(x, y, 0, 0), XStringFormats.Default);
        }

        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, double x, double y, XStringFormat format)
        {
            DrawString(s, font, brush, new XRect(x, y, 0, 0), format);
        }

#if GDI
        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, GdiRectF layoutRectangle)
        {
            DrawString(s, font, brush, new XRect(layoutRectangle), XStringFormats.Default);
        }
#endif

        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, XRect layoutRectangle)
        {
            DrawString(s, font, brush, layoutRectangle, XStringFormats.Default);
        }

#if GDI
        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string s, XFont font, XBrush brush, GdiRectF layoutRectangle, XStringFormat format)
        {
            DrawString(s, font, brush, new XRect(layoutRectangle), format);
        }
#endif

        /// <summary>
        /// Draws the specified text string.
        /// </summary>
        public void DrawString(string text, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");
            if (brush == null)
                throw new ArgumentNullException("brush");

            if (format != null && format.LineAlignment == XLineAlignment.BaseLine && layoutRectangle.Height != 0)
                throw new InvalidOperationException("DrawString: With XLineAlignment.BaseLine the height of the layout rectangle must be 0.");

            if (text.Length == 0)
                return;

            if (format == null)
                format = XStringFormats.Default;

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    // Was font created with font resolver?
                    if (font.GdiFont == null)
                        throw new InvalidOperationException("This font cannot be used by GDI+.");

                    try
                    {
                        Lock.EnterGdiPlus();
                        GdiRectF rect = layoutRectangle.ToRectangleF();
                        if (format.LineAlignment == XLineAlignment.BaseLine)
                        {
                            double lineSpace = font.GetHeight(); //old: font.GetHeight(this);
                            int cellSpace = font.FontFamily.GetLineSpacing(font.Style);
                            int cellAscent = font.FontFamily.GetCellAscent(font.Style);
                            int cellDescent = font.FontFamily.GetCellDescent(font.Style);
                            double cyAscent = lineSpace * cellAscent / cellSpace;
                            cyAscent = lineSpace * font.CellAscent / font.CellSpace;
                            rect.Offset(0, (float)-cyAscent);
                        }
                        //_gfx.DrawString(text, font.Realize_GdiFont(), brush.RealizeGdiBrush(), rect,
                        //    format != null ? format.RealizeGdiStringFormat() : null);
                        _gfx.DrawString(text, font.GdiFont, brush.RealizeGdiBrush(), rect,
                            format != null ? format.RealizeGdiStringFormat() : null);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
#if !SILVERLIGHT
                    double x = layoutRectangle.X;
                    double y = layoutRectangle.Y;

                    double lineSpace = font.GetHeight(); // old: font.GetHeight(this);
                    double cyAscent = lineSpace * font.CellAscent / font.CellSpace;
                    double cyDescent = lineSpace * font.CellDescent / font.CellSpace;

                    bool bold = (font.Style & XFontStyle.Bold) != 0;
                    bool italic = (font.Style & XFontStyle.Italic) != 0;
                    bool strikeout = (font.Style & XFontStyle.Strikeout) != 0;
                    bool underline = (font.Style & XFontStyle.Underline) != 0;

                    //GlyphRun glyphRun=new GlyphRun(font.GlyphTypeface , 0,);
#if DEBUG_
                    if (font.WpfTypeface.FontFamily.Source == "Segoe UI Light")
                        GetType();
#endif
                    FormattedText formattedText = FontHelper.CreateFormattedText(text, font.WpfTypeface, font.Size, brush.RealizeWpfBrush());

                    //formattedText.SetTextDecorations(TextDecorations.OverLine);
                    switch (format.Alignment)
                    {
                        case XStringAlignment.Near:
                            // nothing to do, this is the default
                            //formattedText.TextAlignment = TextAlignment.Left;
                            break;

                        case XStringAlignment.Center:
                            x += layoutRectangle.Width / 2;
                            formattedText.TextAlignment = TextAlignment.Center;
                            break;

                        case XStringAlignment.Far:
                            x += layoutRectangle.Width;
                            formattedText.TextAlignment = TextAlignment.Right;
                            break;
                    }
                    if (PageDirection == XPageDirection.Downwards)
                    {
                        switch (format.LineAlignment)
                        {
                            case XLineAlignment.Near:
                                //y += cyAscent;
                                break;

                            case XLineAlignment.Center:
                                // TODO use CapHeight. PDFlib also uses 3/4 of ascent
                                y += -formattedText.Baseline + (cyAscent * 1 / 3) + layoutRectangle.Height / 2;
                                //y += -formattedText.Baseline + (font.Size * font.Metrics.CapHeight / font.unitsPerEm / 2) + layoutRectangle.Height / 2;
                                break;

                            case XLineAlignment.Far:
                                y += -formattedText.Baseline - cyDescent + layoutRectangle.Height;
                                break;

                            case XLineAlignment.BaseLine:
                                y -= formattedText.Baseline;
                                break;
                        }
                    }
                    else
                    {
                        // TODOWPF: make unit test
                        switch (format.LineAlignment)
                        {
                            case XLineAlignment.Near:
                                //y += cyDescent;
                                break;

                            case XLineAlignment.Center:
                                // TODO use CapHeight. PDFlib also uses 3/4 of ascent
                                //y += -(cyAscent * 3 / 4) / 2 + rect.Height / 2;
                                break;

                            case XLineAlignment.Far:
                                //y += -cyAscent + rect.Height;
                                break;

                            case XLineAlignment.BaseLine:
                                // nothing to do
                                break;
                        }
                    }

                    // BoldSimulation and ItalicSimulation is done only in PDF, not in UI.

                    if (underline)
                    {
                        formattedText.SetTextDecorations(TextDecorations.Underline);
                        //double underlinePosition = lineSpace * realizedFont.FontDescriptor.descriptor.UnderlinePosition / font.cellSpace;
                        //double underlineThickness = lineSpace * realizedFont.FontDescriptor.descriptor.UnderlineThickness / font.cellSpace;
                        //DrawRectangle(null, brush, x, y - underlinePosition, width, underlineThickness);
                    }

                    if (strikeout)
                    {
                        formattedText.SetTextDecorations(TextDecorations.Strikethrough);
                        //double strikeoutPosition = lineSpace * realizedFont.FontDescriptor.descriptor.StrikeoutPosition / font.cellSpace;
                        //double strikeoutSize = lineSpace * realizedFont.FontDescriptor.descriptor.StrikeoutSize / font.cellSpace;
                        //DrawRectangle(null, brush, x, y - strikeoutPosition - strikeoutSize, width, strikeoutSize);
                    }

                    //_dc.DrawText(formattedText, layoutRectangle.Location.ToPoint());
                    _dc.DrawText(formattedText, new SysPoint(x, y));
#else
                    _dc.DrawString(this, text, font, brush, layoutRectangle, format);
#endif
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawString(text, font, brush, layoutRectangle, format);
        }

        // ----- MeasureString ------------------------------------------------------------------------

        /// <summary>
        /// Measures the specified string when drawn with the specified font.
        /// </summary>
        public XSize MeasureString(string text, XFont font, XStringFormat stringFormat)
        {
            if (text == null)
                throw new ArgumentNullException("text");
            if (font == null)
                throw new ArgumentNullException("font");
            if (stringFormat == null)
                throw new ArgumentNullException("stringFormat");

#if GDI && !WPF
            //XSize gdiSize;  // #MediumTrust
            //if (_gfx != null)
            //    gdiSize = XSize.FromSizeF(_gfx.MeasureString(text, font.Realize_GdiFont(), new GdiPointF(0, 0), stringFormat.RealizeGdiStringFormat()));
            //else
            //    gdiSize = FontHelper.MeasureString(text, font, XStringFormats.Default); // TODO 4STLA: Why is parameter stringFormat not used here?
#if DEBUG_
            XSize edfSize = FontHelper.MeasureString(text, font, XStringFormats.Default);
            //Debug.Assert(gdiSize == edfSize, "Measure string failed.");
            if (gdiSize != edfSize)
            {
                double dx = Math.Abs(gdiSize.Width - edfSize.Width);
                double dy = Math.Abs(gdiSize.Height - edfSize.Height);
                Debug.Assert(dx < .05 * gdiSize.Width, "MeasureString: width differs.");
                Debug.Assert(dy < .05 * gdiSize.Height, "MeasureString: height differs.");
            }
#endif
            return FontHelper.MeasureString(text, font, XStringFormats.Default);
#endif
#if WPF && !GDI
#if !SILVERLIGHT
#if DEBUG
            FormattedText formattedText = FontHelper.CreateFormattedText(text, font.WpfTypeface, font.Size, WpfBrushes.Black);
            XSize size1 = FontHelper.MeasureString(text, font, null);
            XSize size2 = new XSize(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
            //Debug.Assert(Math.Abs((size1.Height - size2.Height) * 10) < 1.0);
            return size1;
#else
            // Same as above, but without code needed for Debug.Assert.
            XSize size1 = FontHelper.MeasureString(text, font, null);
            return size1;
#endif
#else
            return _dc.MeasureString(this, text, font, stringFormat);
#endif

#endif
#if WPF && GDI
#if true_
            if (TargetContext == XGraphicTargetContext.GDI)
            {
                XSize gdiSize = XSize.FromSizeF(_gfx.MeasureString(text, font.Realize_GdiFont(), new GdiPointF(0, 0), stringFormat.RealizeGdiStringFormat()));
#if DEBUG
#if GDI
                {
                    //Debug.WriteLine(gdiSize);
                    XSize edfSize = FontHelper14.MeasureStringGdi(_gfx, text, font, XStringFormats.Default);
                    //Debug.WriteLine(edfSize);
                    //Debug.Assert(gdiSize == edfSize, "Measure string failed.");
                    if (gdiSize.Width != edfSize.Width)
                    {
                        Debug.WriteLine(String.Format("Width: {0}, {1} : {2}", gdiSize.Width, edfSize.Width, gdiSize.Width / edfSize.Width));
                    }
                    if (gdiSize.Height != edfSize.Height)
                    {
                        Debug.WriteLine(String.Format("Height: {0}, {1}", gdiSize.Height, edfSize.Height));
                    }

                    //double lineSpace = font.GetHeight(this);
                    //int cellSpace = font.cellSpace; // font.FontFamily.GetLineSpacing(font.Style);
                    //int cellAscent = font.cellAscent; // font.FontFamily.GetCellAscent(font.Style);
                    //int cellDescent = font.cellDescent; // font.FontFamily.GetCellDescent(font.Style);
                    //double cyAscent = lineSpace * cellAscent / cellSpace;
                    //double cyDescent = lineSpace * cellDescent / cellSpace;
                }
#endif
#if WPF
                {
                    //Debug.WriteLine(gdiSize);
                    XSize edfSize = FontHelper14.MeasureStringWpf(text, font, XStringFormats.Default);
                    //Debug.WriteLine(edfSize);
                    //Debug.Assert(gdiSize == edfSize, "Measure string failed.");
                    if (gdiSize.Width != edfSize.Width)
                    {
                        Debug.WriteLine(String.Format("Width: {0}, {1} : {2}", gdiSize.Width, edfSize.Width, gdiSize.Width / edfSize.Width));
                    }
                    if (gdiSize.Height != edfSize.Height)
                    {
                        Debug.WriteLine(String.Format("Height: {0}, {1}", gdiSize.Height, edfSize.Height));
                    }

                    //double lineSpace = font.GetHeight(this);
                    //int cellSpace = font.cellSpace; // font.FontFamily.GetLineSpacing(font.Style);
                    //int cellAscent = font.cellAscent; // font.FontFamily.GetCellAscent(font.Style);
                    //int cellDescent = font.cellDescent; // font.FontFamily.GetCellDescent(font.Style);
                    //double cyAscent = lineSpace * cellAscent / cellSpace;
                    //double cyDescent = lineSpace * cellDescent / cellSpace;
                }
#endif
#endif
                return gdiSize;
            }
            if (TargetContext == XGraphicTargetContext.WPF)
            {
                //double h = font.Height;
                //FormattedText formattedText = new FormattedText(text, new CultureInfo("en-us"),
                //  FlowDirection.LeftToRight, font.typeface, font.Size, WpfBrushes.Black);
                FormattedText formattedText = FontHelper.CreateFormattedText(text, font.Typeface, font.Size, WpfBrushes.Black);
                XSize wpfSize = new XSize(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
#if DEBUG
                Debug.WriteLine(wpfSize);
#endif
                return wpfSize;
            }
            Debug.Assert(false);
            return XSize.Empty;
#else
            XSize size23 = FontHelper.MeasureString(text, font, XStringFormats.Default);
            return size23;
#endif
#endif
#if CORE || NETFX_CORE || UWP
            XSize size = FontHelper.MeasureString(text, font, XStringFormats.Default);
            return size;
#endif
        }

        /// <summary>
        /// Measures the specified string when drawn with the specified font.
        /// </summary>
        public XSize MeasureString(string text, XFont font)
        {
            return MeasureString(text, font, XStringFormats.Default);
        }

        // ----- DrawImage ----------------------------------------------------------------------------

#if GDI
        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, GdiPoint point)
        {
            DrawImage(image, point.X, point.Y);
        }
#endif

#if WPF
        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, SysPoint point)
        {
            DrawImage(image, point.X, point.Y);
        }
#endif

#if GDI
        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, GdiPointF point)
        {
            DrawImage(image, point.X, point.Y);
        }
#endif

        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, XPoint point)
        {
            DrawImage(image, point.X, point.Y);
        }

        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, double x, double y)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            CheckXPdfFormConsistence(image);

            double width = image.PointWidth;
            double height = image.PointHeight;

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (image._gdiImage != null)
                        {
                            InterpolationMode interpolationMode = InterpolationMode.Invalid;
                            if (!image.Interpolate)
                            {
                                interpolationMode = _gfx.InterpolationMode;
                                _gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            }

                            _gfx.DrawImage(image._gdiImage, (float)x, (float)y, (float)width, (float)height);

                            if (!image.Interpolate)
                                _gfx.InterpolationMode = interpolationMode;
                        }
                        else
                        {
                            DrawMissingImageRect(new XRect(x, y, width, height));
                            //_gfx.DrawRectangle(Pens.Red, (float)x, (float)y, (float)width, (float)height);
                            //_gfx.DrawLine(Pens.Red, (float)x, (float)y, (float)(x + width), (float)(y + height));
                            //_gfx.DrawLine(Pens.Red, (float)(x + width), (float)y, (float)x, (float)(y + height));
                        }
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    if (image._wpfImage != null)
                    {
                        _dc.DrawImage(image._wpfImage, new Rect(x, y, image.PointWidth, image.PointHeight));
                    }
                    else
                    {
                        DrawMissingImageRect(new XRect(x, y, width, height));
                    }
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawImage(image, x, y, image.PointWidth, image.PointHeight);
            //image.Width * 72 / image.HorizontalResolution,
            //image.Height * 72 / image.HorizontalResolution);
        }

#if GDI
        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, Rectangle rect)
        {
            // Because of overloading the cast is NOT redundant.
            DrawImage(image, (double)rect.X, (double)rect.Y, (double)rect.Width, (double)rect.Height);
        }
#endif

#if GDI
        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, GdiRectF rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }
#endif

        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, XRect rect)
        {
            DrawImage(image, rect.X, rect.Y, rect.Width, rect.Height);
        }

        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, double x, double y, double width, double height)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            CheckXPdfFormConsistence(image);

            if (_drawGraphics)
            {
                // THHO4STLA: Platform-independent images cannot be drawn here, can they?    => They can. Lazy create platform-dependent image and draw that.
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (image._gdiImage != null)
                        {
                            InterpolationMode interpolationMode = InterpolationMode.Invalid;
                            if (!image.Interpolate)
                            {
                                interpolationMode = _gfx.InterpolationMode;
                                _gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            }

                            _gfx.DrawImage(image._gdiImage, (float)x, (float)y, (float)width, (float)height);

                            if (!image.Interpolate)
                                _gfx.InterpolationMode = interpolationMode;
                        }
                        else
                        {
                            XImage placeholder = null;
                            XPdfForm pdfForm = image as XPdfForm;
                            if (pdfForm != null)
                            {
                                //XPdfForm pf = pdfForm;
                                if (pdfForm.PlaceHolder != null)
                                    placeholder = pdfForm.PlaceHolder;
                            }
                            if (placeholder != null)
                                _gfx.DrawImage(placeholder._gdiImage, (float)x, (float)y, (float)width,
                                    (float)height);
                            else
                            {
                                DrawMissingImageRect(new XRect(x, y, width, height));
                            }
                        }
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    if (image._wpfImage != null)
                    {
                        //InterpolationMode interpolationMode = InterpolationMode.Invalid;
                        //if (!image.Interpolate)
                        //{
                        //  interpolationMode = gfx.InterpolationMode;
                        //  gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                        //}

                        _dc.DrawImage(image._wpfImage, new Rect(x, y, width, height));

                        //if (!image.Interpolate)
                        //  gfx.InterpolationMode = interpolationMode;
                    }
                    else
                    {
                        XImage placeholder = null;
                        if (image is XPdfForm)
                        {
                            XPdfForm pf = image as XPdfForm;
                            if (pf.PlaceHolder != null)
                                placeholder = pf.PlaceHolder;
                        }
                        if (placeholder != null)
                            _dc.DrawImage(placeholder._wpfImage, new Rect(x, y, width, height));
                        else
                            DrawMissingImageRect(new XRect(x, y, width, height));
                    }
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawImage(image, x, y, width, height);
        }

        // TODO: calculate destination size
        //public void DrawImage(XImage image, double x, double y, GdiRectF srcRect, XGraphicsUnit srcUnit)
        //public void DrawImage(XImage image, double x, double y, XRect srcRect, XGraphicsUnit srcUnit)

#if GDI
        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, Rectangle destRect, Rectangle srcRect, XGraphicsUnit srcUnit)
        {
            XRect destRectX = new XRect(destRect.X, destRect.Y, destRect.Width, destRect.Height);
            XRect srcRectX = new XRect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
            DrawImage(image, destRectX, srcRectX, srcUnit);
        }
#endif

#if GDI
        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, GdiRectF destRect, GdiRectF srcRect, XGraphicsUnit srcUnit)
        {
            XRect destRectX = new XRect(destRect.X, destRect.Y, destRect.Width, destRect.Height);
            XRect srcRectX = new XRect(srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height);
            DrawImage(image, destRectX, srcRectX, srcUnit);
        }
#endif

        /// <summary>
        /// Draws the specified image.
        /// </summary>
        public void DrawImage(XImage image, XRect destRect, XRect srcRect, XGraphicsUnit srcUnit)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            CheckXPdfFormConsistence(image);

            if (_drawGraphics)
            {
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        if (image._gdiImage != null)
                        {
                            InterpolationMode interpolationMode = InterpolationMode.Invalid;
                            if (!image.Interpolate)
                            {
                                interpolationMode = _gfx.InterpolationMode;
                                _gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            }

                            GdiRectF destRectF = new GdiRectF((float)destRect.X, (float)destRect.Y,
                                (float)destRect.Width, (float)destRect.Height);
                            GdiRectF srcRectF = new GdiRectF((float)srcRect.X, (float)srcRect.Y,
                                (float)srcRect.Width, (float)srcRect.Height);
                            _gfx.DrawImage(image._gdiImage, destRectF, srcRectF, GraphicsUnit.Pixel);

                            if (!image.Interpolate)
                                _gfx.InterpolationMode = interpolationMode;
                        }
                        else
                        {
                            DrawMissingImageRect(new XRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));
                        }
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                if (TargetContext == XGraphicTargetContext.WPF)
                {
                    if (image._wpfImage != null)
                    {
                        //InterpolationMode interpolationMode = InterpolationMode.Invalid;
                        //if (!image.Interpolate)
                        //{
                        //  interpolationMode = gfx.InterpolationMode;
                        //  //gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                        //}

                        // HACK: srcRect is ignored
                        //double x = destRect.X;
                        //double y = destRect.Y;
                        _dc.DrawImage(image._wpfImage, new SysRect(destRect.X, destRect.Y, destRect.Width, destRect.Height));

                        //if (!image.Interpolate)
                        //  gfx.InterpolationMode = interpolationMode;
                    }
                    else
                    {
                        DrawMissingImageRect(destRect);
                    }
                }
#endif
            }

            if (_renderer != null)
                _renderer.DrawImage(image, destRect, srcRect, srcUnit);
        }

        //TODO?
        //public void DrawImage(XImage image, Rectangle destRect, double srcX, double srcY, double srcWidth, double srcHeight, GraphicsUnit srcUnit);
        //public void DrawImage(XImage image, Rectangle destRect, double srcX, double srcY, double srcWidth, double srcHeight, GraphicsUnit srcUnit);

        void DrawMissingImageRect(XRect rect)
        {
#if GDI
            if (TargetContext == XGraphicTargetContext.GDI)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    float x = (float)rect.X;
                    float y = (float)rect.Y;
                    float width = (float)rect.Width;
                    float height = (float)rect.Height;
                    _gfx.DrawRectangle(Pens.Red, x, y, width, height);
                    _gfx.DrawLine(Pens.Red, x, y, x + width, y + height);
                    _gfx.DrawLine(Pens.Red, x + width, y, x, y + height);
                }
                finally { Lock.ExitGdiPlus(); }
            }
#endif
#if WPF
            if (TargetContext == XGraphicTargetContext.WPF)
            {
                double x = rect.X;
                double y = rect.Y;
                double width = rect.Width;
                double height = rect.Height;
#if !SILVERLIGHT
                WpfPen pen = new WpfPen(WpfBrushes.Red, 1);
#else
                WpfPen pen = new WpfPen();
                pen.Brush = new SolidColorBrush(Colors.Red);
                pen.Thickness = 1;
#endif
                _dc.DrawRectangle(null, pen, new Rect(x, y, width, height));
                _dc.DrawLine(pen, new SysPoint(x, y), new SysPoint(x + width, y + height));
                _dc.DrawLine(pen, new SysPoint(x + width, y), new SysPoint(x, y + height));
            }
#endif
        }

        /// <summary>
        /// Checks whether drawing is allowed and disposes the XGraphics object, if necessary.
        /// </summary>
        void CheckXPdfFormConsistence(XImage image)
        {
            XForm xForm = image as XForm;
            if (xForm != null)
            {
                // Force disposing of XGraphics that draws the content
                xForm.Finish();

                // ReSharper disable once MergeSequentialChecks
                if (_renderer != null && (_renderer as XGraphicsPdfRenderer) != null)
                {
                    if (xForm.Owner != null && xForm.Owner != ((XGraphicsPdfRenderer)_renderer).Owner)
                        throw new InvalidOperationException(
                            "A XPdfForm object is always bound to the document it was created for and cannot be drawn in the context of another document.");

                    if (xForm == ((XGraphicsPdfRenderer)_renderer)._form)
                        throw new InvalidOperationException(
                            "A XPdfForm cannot be drawn on itself.");
                }
            }
        }

        // ----- DrawBarCode --------------------------------------------------------------------------

        /// <summary>
        /// Draws the specified bar code.
        /// </summary>
        public void DrawBarCode(BarCodes.BarCode barcode, XPoint position)
        {
            barcode.Render(this, XBrushes.Black, null, position);
        }

        /// <summary>
        /// Draws the specified bar code.
        /// </summary>
        public void DrawBarCode(BarCodes.BarCode barcode, XBrush brush, XPoint position)
        {
            barcode.Render(this, brush, null, position);
        }

        /// <summary>
        /// Draws the specified bar code.
        /// </summary>
        public void DrawBarCode(BarCodes.BarCode barcode, XBrush brush, XFont font, XPoint position)
        {
            barcode.Render(this, brush, font, position);
        }

        // ----- DrawMatrixCode -----------------------------------------------------------------------

        /// <summary>
        /// Draws the specified data matrix code.
        /// </summary>
        public void DrawMatrixCode(BarCodes.MatrixCode matrixcode, XPoint position)
        {
            matrixcode.Render(this, XBrushes.Black, position);
        }

        /// <summary>
        /// Draws the specified data matrix code.
        /// </summary>
        public void DrawMatrixCode(BarCodes.MatrixCode matrixcode, XBrush brush, XPoint position)
        {
            matrixcode.Render(this, brush, position);
        }

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Save and Restore

        /// <summary>
        /// Saves the current state of this XGraphics object and identifies the saved state with the
        /// returned XGraphicsState object.
        /// </summary>
        public XGraphicsState Save()
        {
            XGraphicsState xState = null;
#if CORE || NETFX_CORE
            if (TargetContext == XGraphicTargetContext.CORE || TargetContext == XGraphicTargetContext.NONE)
            {
                xState = new XGraphicsState();
                InternalGraphicsState iState = new InternalGraphicsState(this, xState);
                iState.Transform = _transform;
                _gsStack.Push(iState);
            }
            else
            {
                Debug.Assert(false, "XGraphicTargetContext must be XGraphicTargetContext.CORE.");
            }
#endif
#if GDI
            if (TargetContext == XGraphicTargetContext.GDI)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    xState = new XGraphicsState(_gfx != null ? _gfx.Save() : null);
                    InternalGraphicsState iState = new InternalGraphicsState(this, xState);
                    iState.Transform = _transform;
                    _gsStack.Push(iState);
                }
                finally { Lock.ExitGdiPlus(); }
            }
#endif
#if WPF
            if (TargetContext == XGraphicTargetContext.WPF)
            {
                xState = new XGraphicsState();
                InternalGraphicsState iState = new InternalGraphicsState(this, xState);
                iState.Transform = _transform;
                _gsStack.Push(iState);
            }
#endif

            if (_renderer != null)
                _renderer.Save(xState);

            return xState;
        }

        /// <summary>
        /// Restores the state of this XGraphics object to the state represented by the specified 
        /// XGraphicsState object.
        /// </summary>
        public void Restore(XGraphicsState state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

#if CORE
            if (TargetContext == XGraphicTargetContext.CORE)
            {
                _gsStack.Restore(state.InternalState);
                _transform = state.InternalState.Transform;
            }
#endif
#if GDI
            if (TargetContext == XGraphicTargetContext.GDI)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    _gsStack.Restore(state.InternalState);
                    if (_gfx != null)
                        _gfx.Restore(state.GdiState);
                    _transform = state.InternalState.Transform;
                }
                finally { Lock.ExitGdiPlus(); }
            }
#endif
#if WPF
            if (TargetContext == XGraphicTargetContext.WPF)
            {
                _gsStack.Restore(state.InternalState);
                _transform = state.InternalState.Transform;
            }
#endif

            if (_renderer != null)
                _renderer.Restore(state);
        }

        /// <summary>
        /// Restores the state of this XGraphics object to the state before the most recently call of Save.
        /// </summary>
        public void Restore()
        {
            if (_gsStack.Count == 0)
                throw new InvalidOperationException("Cannot restore without preceding save operation.");
            Restore(_gsStack.Current.State);
        }

        /// <summary>
        /// Saves a graphics container with the current state of this XGraphics and 
        /// opens and uses a new graphics container.
        /// </summary>
        public XGraphicsContainer BeginContainer()
        {
            return BeginContainer(new XRect(0, 0, 1, 1), new XRect(0, 0, 1, 1), XGraphicsUnit.Point);
        }

#if GDI
        /// <summary>
        /// Saves a graphics container with the current state of this XGraphics and 
        /// opens and uses a new graphics container.
        /// </summary>
        public XGraphicsContainer BeginContainer(Rectangle dstrect, Rectangle srcrect, XGraphicsUnit unit)
        {
            return BeginContainer(new XRect(dstrect), new XRect(dstrect), unit);
        }
#endif

#if GDI
        /// <summary>
        /// Saves a graphics container with the current state of this XGraphics and 
        /// opens and uses a new graphics container.
        /// </summary>
        public XGraphicsContainer BeginContainer(GdiRectF dstrect, GdiRectF srcrect, XGraphicsUnit unit)
        {
            return BeginContainer(new XRect(dstrect), new XRect(dstrect), unit);
        }
#endif

#if WPF
        /// <summary>
        /// Saves a graphics container with the current state of this XGraphics and 
        /// opens and uses a new graphics container.
        /// </summary>
        public XGraphicsContainer BeginContainer(Rect dstrect, Rect srcrect, XGraphicsUnit unit)
        {
            return BeginContainer(new XRect(dstrect), new XRect(dstrect), unit);
        }
#endif

        /// <summary>
        /// Saves a graphics container with the current state of this XGraphics and 
        /// opens and uses a new graphics container.
        /// </summary>
        public XGraphicsContainer BeginContainer(XRect dstrect, XRect srcrect, XGraphicsUnit unit)
        {
            // TODO: unit
            if (unit != XGraphicsUnit.Point)
                throw new ArgumentException("The current implementation supports XGraphicsUnit.Point only.", "unit");

            XGraphicsContainer xContainer = null;
#if CORE
            if (TargetContext == XGraphicTargetContext.CORE)
                xContainer = new XGraphicsContainer();
#endif
#if GDI
            // _gfx can be null if drawing applies to PDF page only.
            if (TargetContext == XGraphicTargetContext.GDI)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    xContainer = new XGraphicsContainer(_gfx != null ? _gfx.Save() : null);
                }
                finally { Lock.ExitGdiPlus(); }
            }
#endif
#if WPF
            if (TargetContext == XGraphicTargetContext.WPF)
                xContainer = new XGraphicsContainer();
#endif
            InternalGraphicsState iState = new InternalGraphicsState(this, xContainer);
            iState.Transform = _transform;

            _gsStack.Push(iState);

            if (_renderer != null)
                _renderer.BeginContainer(xContainer, dstrect, srcrect, unit);

            XMatrix matrix = new XMatrix();
            double scaleX = dstrect.Width / srcrect.Width;
            double scaleY = dstrect.Height / srcrect.Height;
            matrix.TranslatePrepend(-srcrect.X, -srcrect.Y);
            matrix.ScalePrepend(scaleX, scaleY);
            matrix.TranslatePrepend(dstrect.X / scaleX, dstrect.Y / scaleY);
            AddTransform(matrix, XMatrixOrder.Prepend);

            return xContainer;
        }

        /// <summary>
        /// Closes the current graphics container and restores the state of this XGraphics 
        /// to the state saved by a call to the BeginContainer method.
        /// </summary>
        public void EndContainer(XGraphicsContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            _gsStack.Restore(container.InternalState);
#if CORE
            // nothing to do
#endif
#if GDI
            if (TargetContext == XGraphicTargetContext.GDI && _gfx != null)
            {
                try
                {
                    Lock.EnterGdiPlus();
                    _gfx.Restore(container.GdiState);
                }
                finally { Lock.ExitGdiPlus(); }
            }
#endif
#if WPF
            // nothing to do
#endif
            _transform = container.InternalState.Transform;

            if (_renderer != null)
                _renderer.EndContainer(container);
        }

        /// <summary>
        /// Gets the current graphics state level. The default value is 0. Each call of Save or BeginContainer
        /// increased and each call of Restore or EndContainer decreased the value by 1.
        /// </summary>
        public int GraphicsStateLevel
        {
            get { return _gsStack.Count; }
        }

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Properties

        /// <summary>
        /// Gets or sets the smoothing mode.
        /// </summary>
        /// <value>The smoothing mode.</value>
        public XSmoothingMode SmoothingMode
        {
            get
            {
#if CORE
                // nothing to do
#endif
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        return (XSmoothingMode)_gfx.SmoothingMode;
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                // nothing to do
#endif
                return _smoothingMode;
            }
            set
            {
                _smoothingMode = value;
#if CORE
                // nothing to do
#endif
#if GDI
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.SmoothingMode = (SmoothingMode)value;
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF
                // nothing to do
#endif
            }
        }
        XSmoothingMode _smoothingMode;

        //public Region Clip { get; set; }
        //public GdiRectF ClipBounds { get; }
        //public CompositingMode CompositingMode { get; set; }
        //public CompositingQuality CompositingQuality { get; set; }
        //public float DpiX { get; }
        //public float DpiY { get; }
        //public InterpolationMode InterpolationMode { get; set; }
        //public bool IsClipEmpty { get; }
        //public bool IsVisibleClipEmpty { get; }
        //public float PageScale { get; set; }
        //public GraphicsUnit PageUnit { get; set; }
        //public PixelOffsetMode PixelOffsetMode { get; set; }
        //public Point RenderingOrigin { get; set; }
        //public SmoothingMode SmoothingMode { get; set; }
        //public int TextContrast { get; set; }
        //public TextRenderingHint TextRenderingHint { get; set; }
        //public Matrix Transform { get; set; }
        //public GdiRectF VisibleClipBounds { get; }

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Transformation

        /// <summary>
        /// Applies the specified translation operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        public void TranslateTransform(double dx, double dy)
        {
            AddTransform(XMatrix.CreateTranslation(dx, dy), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified translation operation to the transformation matrix of this object
        /// in the specified order.
        /// </summary>
        public void TranslateTransform(double dx, double dy, XMatrixOrder order)
        {
            XMatrix matrix = new XMatrix();
            matrix.TranslatePrepend(dx, dy);
            AddTransform(matrix, order);
        }

        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        public void ScaleTransform(double scaleX, double scaleY)
        {
            AddTransform(XMatrix.CreateScaling(scaleX, scaleY), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this object
        /// in the specified order.
        /// </summary>
        public void ScaleTransform(double scaleX, double scaleY, XMatrixOrder order)
        {
            XMatrix matrix = new XMatrix();
            matrix.ScalePrepend(scaleX, scaleY);
            AddTransform(matrix, order);
        }

        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public void ScaleTransform(double scaleXY)
        {
            ScaleTransform(scaleXY, scaleXY);
        }

        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this object
        /// in the specified order.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public void ScaleTransform(double scaleXY, XMatrixOrder order)
        {
            ScaleTransform(scaleXY, scaleXY, order);
        }

        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        public void ScaleAtTransform(double scaleX, double scaleY, double centerX, double centerY)
        {
            AddTransform(XMatrix.CreateScaling(scaleX, scaleY, centerX, centerY), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified scaling operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        public void ScaleAtTransform(double scaleX, double scaleY, XPoint center)
        {
            AddTransform(XMatrix.CreateScaling(scaleX, scaleY, center.X, center.Y), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified rotation operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        public void RotateTransform(double angle)
        {
            AddTransform(XMatrix.CreateRotationRadians(angle * Const.Deg2Rad), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified rotation operation to the transformation matrix of this object
        /// in the specified order. The angle unit of measure is degree.
        /// </summary>
        public void RotateTransform(double angle, XMatrixOrder order)
        {
            XMatrix matrix = new XMatrix();
            matrix.RotatePrepend(angle);
            AddTransform(matrix, order);
        }

        /// <summary>
        /// Applies the specified rotation operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        public void RotateAtTransform(double angle, XPoint point)
        {
            AddTransform(XMatrix.CreateRotationRadians(angle * Const.Deg2Rad, point.X, point.Y), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified rotation operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// </summary>
        public void RotateAtTransform(double angle, XPoint point, XMatrixOrder order)
        {
            AddTransform(XMatrix.CreateRotationRadians(angle * Const.Deg2Rad, point.X, point.Y), order);
        }

        /// <summary>
        /// Applies the specified shearing operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// ShearTransform is a synonym for SkewAtTransform.
        /// Parameter shearX specifies the horizontal skew which is measured in degrees counterclockwise from the y-axis.
        /// Parameter shearY specifies the vertical skew which is measured in degrees counterclockwise from the x-axis.
        /// </summary>
        public void ShearTransform(double shearX, double shearY)
        {
            AddTransform(XMatrix.CreateSkewRadians(shearX * Const.Deg2Rad, shearY * Const.Deg2Rad), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified shearing operation to the transformation matrix of this object
        /// in the specified order.
        /// ShearTransform is a synonym for SkewAtTransform.
        /// Parameter shearX specifies the horizontal skew which is measured in degrees counterclockwise from the y-axis.
        /// Parameter shearY specifies the vertical skew which is measured in degrees counterclockwise from the x-axis.
        /// </summary>
        public void ShearTransform(double shearX, double shearY, XMatrixOrder order)
        {
            AddTransform(XMatrix.CreateSkewRadians(shearX * Const.Deg2Rad, shearY * Const.Deg2Rad), order);
        }

        /// <summary>
        /// Applies the specified shearing operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// ShearTransform is a synonym for SkewAtTransform.
        /// Parameter shearX specifies the horizontal skew which is measured in degrees counterclockwise from the y-axis.
        /// Parameter shearY specifies the vertical skew which is measured in degrees counterclockwise from the x-axis.
        /// </summary>
        public void SkewAtTransform(double shearX, double shearY, double centerX, double centerY)
        {
            AddTransform(XMatrix.CreateSkewRadians(shearX * Const.Deg2Rad, shearY * Const.Deg2Rad, centerX, centerY), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Applies the specified shearing operation to the transformation matrix of this object by 
        /// prepending it to the object's transformation matrix.
        /// ShearTransform is a synonym for SkewAtTransform.
        /// Parameter shearX specifies the horizontal skew which is measured in degrees counterclockwise from the y-axis.
        /// Parameter shearY specifies the vertical skew which is measured in degrees counterclockwise from the x-axis.
        /// </summary>
        public void SkewAtTransform(double shearX, double shearY, XPoint center)
        {
            AddTransform(XMatrix.CreateSkewRadians(shearX * Const.Deg2Rad, shearY * Const.Deg2Rad, center.X, center.Y), XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Multiplies the transformation matrix of this object and specified matrix.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix)
        {
            AddTransform(matrix, XMatrixOrder.Prepend);
        }

        /// <summary>
        /// Multiplies the transformation matrix of this object and specified matrix in the specified order.
        /// </summary>
        public void MultiplyTransform(XMatrix matrix, XMatrixOrder order)
        {
            AddTransform(matrix, order);
        }

        /// <summary>
        /// Gets the current transformation matrix.
        /// The transformation matrix canot be set. Insted use Save/Restore or BeginContainer/EndContainer to
        /// save the state before Transform is called and later restore to the previous transform.
        /// </summary>
        public XMatrix Transform
        {
            get { return _transform; }
        }

        /// <summary>
        /// Applies a new transformation to the current transformation matrix.
        /// </summary>
        void AddTransform(XMatrix transform, XMatrixOrder order)
        {
            XMatrix matrix = _transform;
            matrix.Multiply(transform, order);
            _transform = matrix;
            matrix = DefaultViewMatrix;
            matrix.Multiply(_transform, XMatrixOrder.Prepend);
#if CORE
            if (TargetContext == XGraphicTargetContext.CORE)
            {
                GetType();
                // TODO: _gsStack...
            }
#endif
#if GDI
            if (TargetContext == XGraphicTargetContext.GDI)
            {
                if (_gfx != null)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.Transform = (GdiMatrix)matrix;
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
            }
#endif
#if WPF
            if (TargetContext == XGraphicTargetContext.WPF)
            {
#if !SILVERLIGHT
                MatrixTransform mt = new MatrixTransform(transform.ToWpfMatrix());
#else
                MatrixTransform mt = new MatrixTransform();
                mt.Matrix = transform.ToWpfMatrix();
#endif
                if (order == XMatrixOrder.Append)
                    mt = (MatrixTransform)mt.Inverse;
                _gsStack.Current.PushTransform(mt);
            }
#endif
            if (_renderer != null)
                _renderer.AddTransform(transform, XMatrixOrder.Prepend);
        }

        //public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, Point[] points)
        //{
        //}
        //
        //public void TransformPoints(CoordinateSpace destSpace, CoordinateSpace srcSpace, GdiPointF[] points)
        //{
        //}

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Clipping

#if GDI
        /// <summary>
        /// Updates the clip region of this XGraphics to the intersection of the 
        /// current clip region and the specified rectangle.
        /// </summary>
        public void IntersectClip(Rectangle rect)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRectangle(rect);
            IntersectClip(path);
        }
#endif

#if GDI
        /// <summary>
        /// Updates the clip region of this XGraphics to the intersection of the 
        /// current clip region and the specified rectangle.
        /// </summary>
        public void IntersectClip(GdiRectF rect)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRectangle(rect);
            IntersectClip(path);
        }
#endif

        /// <summary>
        /// Updates the clip region of this XGraphics to the intersection of the 
        /// current clip region and the specified rectangle.
        /// </summary>
        public void IntersectClip(XRect rect)
        {
            XGraphicsPath path = new XGraphicsPath();
            path.AddRectangle(rect);
            IntersectClip(path);
        }

        /// <summary>
        /// Updates the clip region of this XGraphics to the intersection of the 
        /// current clip region and the specified graphical path.
        /// </summary>
        public void IntersectClip(XGraphicsPath path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (_drawGraphics)
            {
#if GDI && !WPF
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.SetClip(path._gdipPath, CombineMode.Intersect);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
#endif
#if WPF && !GDI
                if (TargetContext == XGraphicTargetContext.WPF)
                    _gsStack.Current.PushClip(path._pathGeometry);
#endif
#if GDI && WPF
                if (TargetContext == XGraphicTargetContext.GDI)
                {
                    try
                    {
                        Lock.EnterGdiPlus();
                        _gfx.SetClip(path._gdipPath, CombineMode.Intersect);
                    }
                    finally { Lock.ExitGdiPlus(); }
                }
                else
                {
                    _gsStack.Current.PushClip(path._pathGeometry);
                }
#endif
            }

            if (_renderer != null)
                _renderer.SetClip(path, XCombineMode.Intersect);
        }

        //public void SetClip(Graphics g);
        //public void SetClip(Graphics g, CombineMode combineMode);
        //public void SetClip(GraphicsPath path, CombineMode combineMode);
        //public void SetClip(Rectangle rect, CombineMode combineMode);
        //public void SetClip(GdiRectF rect, CombineMode combineMode);
        //public void SetClip(Region region, CombineMode combineMode);
        //public void IntersectClip(Region region);
        //public void ExcludeClip(Region region);

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Miscellaneous

        /// <summary>
        /// Writes a comment to the output stream. Comments have no effect on the rendering of the output.
        /// They may be useful to mark a position in a content stream of a PDF document.
        /// </summary>
        public void WriteComment(string comment)
        {
            if (comment == null)
                throw new ArgumentNullException("comment");

            if (_drawGraphics)
            {
                // TODO: Do something if metafile?
            }

            if (_renderer != null)
                _renderer.WriteComment(comment);
        }

        /// <summary>
        /// Permits access to internal data.
        /// </summary>
        public XGraphicsInternals Internals
        {
            get { return _internals ?? (_internals = new XGraphicsInternals(this)); }
        }
        XGraphicsInternals _internals;

        /// <summary>
        /// (Under construction. May change in future versions.)
        /// </summary>
        public SpaceTransformer Transformer
        {
            get { return _transformer ?? (_transformer = new SpaceTransformer(this)); }
        }
        SpaceTransformer _transformer;

        #endregion

        // --------------------------------------------------------------------------------------------

        #region Internal Helper Functions

#if GDI
        /// <summary>
        /// Converts a GdiPoint[] into a GdiPointF[].
        /// </summary>
        internal static GdiPointF[] MakePointFArray(GdiPoint[] points, int offset, int count)
        {
            if (points == null)
                return null;

            //int length = points.Length;
            GdiPointF[] result = new GdiPointF[count];
            for (int idx = 0, srcIdx = offset; idx < count; idx++, srcIdx++)
            {
                result[idx].X = points[srcIdx].X;
                result[idx].Y = points[srcIdx].Y;
            }
            return result;
        }
#endif

#if GDI
        /// <summary>
        /// Converts a XPoint[] into a GdiPointF[].
        /// </summary>
        internal static GdiPointF[] MakePointFArray(XPoint[] points)
        {
            if (points == null)
                return null;

            int count = points.Length;
            GdiPointF[] result = new GdiPointF[count];
            for (int idx = 0; idx < count; idx++)
            {
                result[idx].X = (float)points[idx].X;
                result[idx].Y = (float)points[idx].Y;
            }
            return result;
        }
#endif

#if GDI
        /// <summary>
        /// Converts a Point[] into a XPoint[].
        /// </summary>
        internal static XPoint[] MakeXPointArray(GdiPoint[] points, int offset, int count)
        {
            if (points == null)
                return null;

            //int lengh = points.Length;
            XPoint[] result = new XPoint[count];
            for (int idx = 0, srcIdx = offset; idx < count; idx++, srcIdx++)
            {
                result[idx].X = points[srcIdx].X;
                result[idx].Y = points[srcIdx].Y;
            }
            return result;
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Converts a Point[] into a XPoint[].
        /// </summary>
        internal static XPoint[] MakeXPointArray(SysPoint[] points, int offset, int count)
        {
            if (points == null)
                return null;

            //int length = points.Length;
            XPoint[] result = new XPoint[count];
            for (int idx = 0, srcIdx = offset; idx < count; idx++, srcIdx++)
            {
                result[idx].X = points[srcIdx].X;
                result[idx].Y = points[srcIdx].Y;
            }
            return result;
        }
#endif

#if GDI
        /// <summary>
        /// Converts a GdiPointF[] into a XPoint[].
        /// </summary>
        internal static XPoint[] MakeXPointArray(GdiPointF[] points, int offset, int count)
        {
            if (points == null)
                return null;

            //int length = points.Length;
            XPoint[] result = new XPoint[count];
            for (int idx = 0, srcIdx = offset; idx < count; idx++, srcIdx++)
            {
                result[idx].X = points[srcIdx].X;
                result[idx].Y = points[srcIdx].Y;
            }
            return result;
        }
#endif

#if GDI
        /// <summary>
        /// Converts a XRect[] into a GdiRectF[].
        /// </summary>
        internal static GdiRectF[] MakeRectangleFArray(XRect[] rects, int offset, int count)
        {
            if (rects == null)
                return null;

            //int length = rects.Length;
            GdiRectF[] result = new GdiRectF[count];
            for (int idx = 0, srcIdx = offset; idx < count; idx++, srcIdx++)
            {
                XRect rect = rects[srcIdx];
                result[idx] = new GdiRectF((float)rect.X, (float)rect.Y, (float)rect.Width, (float)rect.Height);
            }
            return result;
        }
#endif

#if WPF || NETFX_CORE
        /// <summary>
        /// Converts an XPoint[] into a Point[].
        /// </summary>
        internal static SysPoint[] MakePointArray(XPoint[] points)
        {
            if (points == null)
                return null;

            int count = points.Length;
            SysPoint[] result = new SysPoint[count];
            for (int idx = 0; idx < count; idx++)
            {
                result[idx].X = points[idx].X;
                result[idx].Y = points[idx].Y;
            }
            return result;
        }
#endif

        #endregion

        ///// <summary>
        ///// Testcode
        ///// </summary>
        //public void TestXObject(PdfDocument thisDoc, PdfPage thisPage, int page,
        //      PdfDocument externalDoc, ImportedObjectTable impDoc)
        //{
        //    PdfPage impPage = externalDoc.Pages[page];
        //    //      impDoc.ImportPage(impPage);
        //    PdfFormXObject form = new PdfFormXObject(thisDoc, impDoc, impPage);
        //    thisDoc.xrefTable.Add(form);

        //    PdfDictionary xobjects = new PdfDictionary();
        //    xobjects.Elements["/X42"] = form.XRef;
        //    thisPage.Resources.Elements[PdfResources.Keys.XObject] = xobjects;
        //    ((XGraphicsPdfRenderer)renderer).DrawXObject("/X42");
        //}

        internal void DisassociateImage()
        {
            if (_associatedImage == null)
                throw new InvalidOperationException("No image associated.");

            Dispose();
        }

        internal InternalGraphicsMode InternalGraphicsMode
        {
            get { return _internalGraphicsMode; }
            set { _internalGraphicsMode = value; }
        }
        InternalGraphicsMode _internalGraphicsMode;

        internal XImage AssociatedImage
        {
            get { return _associatedImage; }
            set { _associatedImage = value; }
        }
        XImage _associatedImage;

#if GDI
        /// <summary>
        /// Always defined System.Drawing.Graphics object. Used as 'query context' for PDF pages.
        /// </summary>
        internal Graphics _gfx;
#endif

#if WPF
        /// <summary>
        /// Always defined System.Drawing.Graphics object. Used as 'query context' for PDF pages.
        /// </summary>
#if !SILVERLIGHT
        DrawingVisual _dv;
        internal DrawingContext _dc;
#else
        internal AgDrawingContext _dc;
#endif
#endif

#if UWP
        CanvasDrawingSession _cds;
#endif

        /// <summary>
        /// The transformation matrix from the XGraphics page space to the Graphics world space.
        /// (The name 'default view matrix' comes from Microsoft OS/2 Presentation Manager. I choose
        /// this name because I have no better one.)
        /// </summary>
        internal XMatrix DefaultViewMatrix;

        /// <summary>
        /// Indicates whether to send drawing operations to _gfx or _dc.
        /// </summary>
        bool _drawGraphics;

        readonly XForm _form;

#if GDI
        internal Metafile Metafile;
#endif

        /// <summary>
        /// Interface to an (optional) renderer. Currently it is the XGraphicsPdfRenderer, if defined.
        /// </summary>
        IXGraphicsRenderer _renderer;

        /// <summary>
        /// The transformation matrix from XGraphics world space to page unit space.
        /// </summary>
        XMatrix _transform;

        /// <summary>
        /// The graphics state stack.
        /// </summary>
        readonly GraphicsStateStack _gsStack;

        /// <summary>
        /// Gets the PDF page that serves as drawing surface if PDF is rendered,
        /// or null, if no such object exists.
        /// </summary>
        public PdfPage PdfPage
        {
            get
            {
                XGraphicsPdfRenderer renderer = _renderer as PdfSharp.Drawing.Pdf.XGraphicsPdfRenderer;
                return renderer != null ? renderer._page : null;
            }
        }

#if GDI
        /// <summary>
        /// Gets the System.Drawing.Graphics objects that serves as drawing surface if no PDF is rendered,
        /// or null, if no such object exists.
        /// </summary>
        public Graphics Graphics
        {
            get { return _gfx; }
        }
#endif

        //#if CORE || GDI
        //        /// <summary>
        //        /// Critical section used to serialize access to GDI+.
        //        /// This may be necessary to use PDFsharp savely in a Web application.
        //        /// </summary>
        //        internal static readonly object GdiPlus = new object();
        //#endif

        /// <summary>
        /// Provides access to internal data structures of the XGraphics class.
        /// </summary>
        public class XGraphicsInternals
        {
            internal XGraphicsInternals(XGraphics gfx)
            {
                _gfx = gfx;
            }
            readonly XGraphics _gfx;

#if GDI
            /// <summary>
            /// Gets the underlying Graphics object.
            /// </summary>
            public Graphics Graphics
            {
                get { return _gfx._gfx; }
            }
#endif
        }

        /// <summary>
        /// (This class is under construction.)
        /// Currently used in MigraDoc
        /// </summary>
        public class SpaceTransformer
        {
            internal SpaceTransformer(XGraphics gfx)
            {
                _gfx = gfx;
            }
            readonly XGraphics _gfx;

            /// <summary>
            /// Gets the smallest rectangle in default page space units that completely encloses the specified rect
            /// in world space units.
            /// </summary>
            public XRect WorldToDefaultPage(XRect rect)
            {
                XPoint[] points = new XPoint[4];
                points[0] = new XPoint(rect.X, rect.Y);
                points[1] = new XPoint(rect.X + rect.Width, rect.Y);
                points[2] = new XPoint(rect.X, rect.Y + rect.Height);
                points[3] = new XPoint(rect.X + rect.Width, rect.Y + rect.Height);

                XMatrix matrix = _gfx.Transform;
                matrix.TransformPoints(points);

                double height = _gfx.PageSize.Height;
                points[0].Y = height - points[0].Y;
                points[1].Y = height - points[1].Y;
                points[2].Y = height - points[2].Y;
                points[3].Y = height - points[3].Y;

                double xmin = Math.Min(Math.Min(points[0].X, points[1].X), Math.Min(points[2].X, points[3].X));
                double xmax = Math.Max(Math.Max(points[0].X, points[1].X), Math.Max(points[2].X, points[3].X));
                double ymin = Math.Min(Math.Min(points[0].Y, points[1].Y), Math.Min(points[2].Y, points[3].Y));
                double ymax = Math.Max(Math.Max(points[0].Y, points[1].Y), Math.Max(points[2].Y, points[3].Y));

                return new XRect(xmin, ymin, xmax - xmin, ymax - ymin);
            }
        }
    }
}
