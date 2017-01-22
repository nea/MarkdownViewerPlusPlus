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
using System.Globalization;
using System.ComponentModel;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Annotations;

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents a page in a PDF document.
    /// </summary>
    public sealed class PdfPage : PdfDictionary, IContentStream
    {
        /// <summary>
        /// Initializes a new page. The page must be added to a document before it can be used.
        /// Depending of the IsMetric property of the current region the page size is set to 
        /// A4 or Letter respectively. If this size is not appropriate it should be changed before
        /// any drawing operations are performed on the page.
        /// </summary>
        public PdfPage()
        {
            Elements.SetName(Keys.Type, "/Page");
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfPage"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public PdfPage(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/Page");
            Elements[Keys.Parent] = document.Pages.Reference;
            Initialize();
        }

        internal PdfPage(PdfDictionary dict)
            : base(dict)
        {
            // Set Orientation depending on /Rotate.
            int rotate = Elements.GetInteger(InheritablePageKeys.Rotate);
            if (Math.Abs((rotate / 90)) % 2 == 1)
                _orientation = PageOrientation.Landscape;
        }

        void Initialize()
        {
            Size = RegionInfo.CurrentRegion.IsMetric ? PageSize.A4 : PageSize.Letter;

#pragma warning disable 168
            // Force creation of MediaBox object by invoking property
            PdfRectangle rect = MediaBox;
#pragma warning restore 168
        }

        /// <summary>
        /// Gets or sets a user defined object that contains arbitrary information associated with this PDF page.
        /// The tag is not used by PDFsharp.
        /// </summary>
        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }
        object _tag;

        /// <summary>
        /// Closes the page. A closes page cannot be modified anymore and it is not possible to
        /// get an XGraphics object for a closed page. Closing a page is not required, but may saves
        /// resources if the document has many pages. 
        /// </summary>
        public void Close()
        {
            //// Close renderer, if any
            //if (_content.pdfRenderer != null)
            //  _content.pdfRenderer.endp.Close();
            _closed = true;
        }
        bool _closed;

        /// <summary>
        /// Gets a value indicating whether the page is closed.
        /// </summary>
        internal bool IsClosed
        {
            get { return _closed; }
        }

        /// <summary>
        /// Gets or sets the PdfDocument this page belongs to.
        /// </summary>
        internal override PdfDocument Document
        {
            set
            {
                if (!ReferenceEquals(_document, value))
                {
                    if (_document != null)
                        throw new InvalidOperationException("Cannot change document.");
                    _document = value;
                    if (Reference != null)
                        Reference.Document = value;
                    Elements[Keys.Parent] = _document.Pages.Reference;
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the page. The default value PageOrientation.Portrait.
        /// If an imported page has a /Rotate value that matches the formula 90 + n * 180 the 
        /// orientation is set to PageOrientation.Landscape.
        /// </summary>
        public PageOrientation Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }
        PageOrientation _orientation;

        /// <summary>
        /// Gets or sets one of the predefined standard sizes like.
        /// </summary>
        public PageSize Size
        {
            get { return _pageSize; }
            set
            {
                if (!Enum.IsDefined(typeof(PageSize), value))
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(PageSize));

                XSize size = PageSizeConverter.ToSize(value);
                // MediaBox is always in Portrait mode (see Height, Width).
                // So take Orientation NOT into account.
                MediaBox = new PdfRectangle(0, 0, size.Width, size.Height);
                _pageSize = value;
            }
        }
        PageSize _pageSize;

        /// <summary>
        /// Gets or sets the trim margins.
        /// </summary>
        public TrimMargins TrimMargins
        {
            get
            {
                if (_trimMargins == null)
                    _trimMargins = new TrimMargins();
                return _trimMargins;
            }
            set
            {
                if (_trimMargins == null)
                    _trimMargins = new TrimMargins();
                if (value != null)
                {
                    _trimMargins.Left = value.Left;
                    _trimMargins.Right = value.Right;
                    _trimMargins.Top = value.Top;
                    _trimMargins.Bottom = value.Bottom;
                }
                else
                    _trimMargins.All = 0;
            }
        }
        TrimMargins _trimMargins = new TrimMargins();

        /// <summary>
        /// Gets or sets the media box directly. XGrahics is not prepared to work with a media box
        /// with an origin other than (0,0).
        /// </summary>
        public PdfRectangle MediaBox
        {
            get { return Elements.GetRectangle(Keys.MediaBox, true); }
            set { Elements.SetRectangle(Keys.MediaBox, value); }
        }

        /// <summary>
        /// Gets or sets the crop box.
        /// </summary>
        public PdfRectangle CropBox
        {
            get { return Elements.GetRectangle(Keys.CropBox, true); }
            set { Elements.SetRectangle(Keys.CropBox, value); }
        }

        /// <summary>
        /// Gets or sets the bleed box.
        /// </summary>
        public PdfRectangle BleedBox
        {
            get { return Elements.GetRectangle(Keys.BleedBox, true); }
            set { Elements.SetRectangle(Keys.BleedBox, value); }
        }

        /// <summary>
        /// Gets or sets the art box.
        /// </summary>
        public PdfRectangle ArtBox
        {
            get { return Elements.GetRectangle(Keys.ArtBox, true); }
            set { Elements.SetRectangle(Keys.ArtBox, value); }
        }

        /// <summary>
        /// Gets or sets the trim box.
        /// </summary>
        public PdfRectangle TrimBox
        {
            get { return Elements.GetRectangle(Keys.TrimBox, true); }
            set { Elements.SetRectangle(Keys.TrimBox, value); }
        }

        /// <summary>
        /// Gets or sets the height of the page. If orientation is Landscape, this function applies to
        /// the width.
        /// </summary>
        public XUnit Height
        {
            get
            {
                PdfRectangle rect = MediaBox;
                return _orientation == PageOrientation.Portrait ? rect.Height : rect.Width;
            }
            set
            {
                PdfRectangle rect = MediaBox;
                if (_orientation == PageOrientation.Portrait)
                    MediaBox = new PdfRectangle(rect.X1, 0, rect.X2, value);
                else
                    MediaBox = new PdfRectangle(0, rect.Y1, value, rect.Y2);
                _pageSize = PageSize.Undefined;
            }
        }

        /// <summary>
        /// Gets or sets the width of the page. If orientation is Landscape, this function applies to
        /// the height.
        /// </summary>
        public XUnit Width
        {
            get
            {
                PdfRectangle rect = MediaBox;
                return _orientation == PageOrientation.Portrait ? rect.Width : rect.Height;
            }
            set
            {
                PdfRectangle rect = MediaBox;
                if (_orientation == PageOrientation.Portrait)
                    MediaBox = new PdfRectangle(0, rect.Y1, value, rect.Y2);
                else
                    MediaBox = new PdfRectangle(rect.X1, 0, rect.X2, value);
                _pageSize = PageSize.Undefined;
            }
        }

        /// <summary>
        /// Gets or sets the /Rotate entry of the PDF page. The value is the number of degrees by which the page 
        /// should be rotated clockwise when displayed or printed. The value must be a multiple of 90.
        /// TODO: Next statement is not correct: 
        /// PDFsharp does not set this value, but for imported pages this value can be set and must be taken
        /// into account when adding graphic to such a page.
        /// </summary>
        public int Rotate
        {
            get { return _elements.GetInteger(InheritablePageKeys.Rotate); }
            set
            {
                if (value / 90 * 90 != value)
                    throw new ArgumentException("Value must be a multiple of 90.");
                _elements.SetInteger(InheritablePageKeys.Rotate, value);
            }
        }

        // TODO: PdfAnnotations
        // TODO: PdfActions
        // TODO: PdfPageTransition

        /// <summary>
        /// The content stream currently used by an XGraphics object for rendering.
        /// </summary>
        internal PdfContent RenderContent;

        /// <summary>
        /// Gets the array of content streams of the page.
        /// </summary>
        public PdfContents Contents
        {
            get
            {
                if (_contents == null)
                {
                    if (true) // || Document.IsImported)
                    {
                        PdfItem item = Elements[Keys.Contents];
                        if (item == null)
                        {
                            _contents = new PdfContents(Owner);
                            //Owner.irefTable.Add(_contents);
                        }
                        else
                        {
                            if (item is PdfReference)
                                item = ((PdfReference)item).Value;

                            PdfArray array = item as PdfArray;
                            if (array != null)
                            {
                                // It is already an array of content streams.
                                if (array.IsIndirect)
                                {
                                    // Make it a direct array
                                    array = array.Clone();
                                    array.Document = Owner;
                                }
                                // TODO 4STLA: Causes Exception "Object type transformation must not be done with direct objects" in "protected PdfObject(PdfObject obj)"
                                _contents = new PdfContents(array);
                            }
                            else
                            {
                                // Only one content stream -> create array
                                _contents = new PdfContents(Owner);
                                //Owner.irefTable.Add(_contents);
                                PdfContent content = new PdfContent((PdfDictionary)item);
                                _contents.Elements.Add(content.Reference);
                            }
                        }
                    }
                    //else
                    //{
                    //  _content = new PdfContent(Document);
                    //  Document.xrefTable.Add(_content);
                    //}
                    Debug.Assert(_contents.Reference == null);
                    Elements[Keys.Contents] = _contents;
                }
                return _contents;
            }
        }
        PdfContents _contents;

        #region Annotations

        /// <summary>
        /// Gets the annotations array of this page.
        /// </summary>
        public bool HasAnnotations
        {
            get
            {
                if (_annotations == null)
                {
                    // Get annotations array if exists.
                    _annotations = (PdfAnnotations)Elements.GetValue(Keys.Annots);
                    _annotations.Page = this;
                }
                return _annotations != null;
            }
        }

        /// <summary>
        /// Gets the annotations array of this page.
        /// </summary>
        public PdfAnnotations Annotations
        {
            get
            {
                if (_annotations == null)
                {
                    // Get or create annotations array.
                    _annotations = (PdfAnnotations)Elements.GetValue(Keys.Annots, VCF.Create);
                    _annotations.Page = this;
                }
                return _annotations;
            }
        }
        PdfAnnotations _annotations;

        /// <summary>
        /// Adds an intra document link.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="destinationPage">The destination page.</param>
        public PdfLinkAnnotation AddDocumentLink(PdfRectangle rect, int destinationPage)
        {
            PdfLinkAnnotation annotation = PdfLinkAnnotation.CreateDocumentLink(rect, destinationPage);
            Annotations.Add(annotation);
            return annotation;
        }

        /// <summary>
        /// Adds a link to the Web.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="url">The URL.</param>
        public PdfLinkAnnotation AddWebLink(PdfRectangle rect, string url)
        {
            PdfLinkAnnotation annotation = PdfLinkAnnotation.CreateWebLink(rect, url);
            Annotations.Add(annotation);
            return annotation;
        }

        /// <summary>
        /// Adds a link to a file.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="fileName">Name of the file.</param>
        public PdfLinkAnnotation AddFileLink(PdfRectangle rect, string fileName)
        {
            PdfLinkAnnotation annotation = PdfLinkAnnotation.CreateFileLink(rect, fileName);
            Annotations.Add(annotation);
            return annotation;
        }

        #endregion

        /// <summary>
        /// Gets or sets the custom values.
        /// </summary>
        public PdfCustomValues CustomValues
        {
            get
            {
                if (_customValues == null)
                    _customValues = PdfCustomValues.Get(Elements);
                return _customValues;
            }
            set
            {
                if (value != null)
                    throw new ArgumentException("Only null is allowed to clear all custom values.");
                PdfCustomValues.Remove(Elements);
                _customValues = null;
            }
        }
        PdfCustomValues _customValues;

        /// <summary>
        /// Gets the PdfResources object of this page.
        /// </summary>
        public PdfResources Resources
        {
            get
            {
                if (_resources == null)
                    _resources = (PdfResources)Elements.GetValue(Keys.Resources, VCF.Create); //VCF.CreateIndirect
                return _resources;
            }
        }
        PdfResources _resources;

        /// <summary>
        /// Implements the interface because the primary function is internal.
        /// </summary>
        PdfResources IContentStream.Resources
        {
            get { return Resources; }
        }

        /// <summary>
        /// Gets the resource name of the specified font within this page.
        /// </summary>
        internal string GetFontName(XFont font, out PdfFont pdfFont)
        {
            pdfFont = _document.FontTable.GetFont(font);
            Debug.Assert(pdfFont != null);
            string name = Resources.AddFont(pdfFont);
            return name;
        }

        string IContentStream.GetFontName(XFont font, out PdfFont pdfFont)
        {
            return GetFontName(font, out pdfFont);
        }

        /// <summary>
        /// Tries to get the resource name of the specified font data within this page.
        /// Returns null if no such font exists.
        /// </summary>
        internal string TryGetFontName(string idName, out PdfFont pdfFont)
        {
            pdfFont = _document.FontTable.TryGetFont(idName);
            string name = null;
            if (pdfFont != null)
                name = Resources.AddFont(pdfFont);
            return name;
        }

        /// <summary>
        /// Gets the resource name of the specified font data within this page.
        /// </summary>
        internal string GetFontName(string idName, byte[] fontData, out PdfFont pdfFont)
        {
            pdfFont = _document.FontTable.GetFont(idName, fontData);
            //pdfFont = new PdfType0Font(Owner, idName, fontData);
            //pdfFont.Document = _document;
            Debug.Assert(pdfFont != null);
            string name = Resources.AddFont(pdfFont);
            return name;
        }

        string IContentStream.GetFontName(string idName, byte[] fontData, out PdfFont pdfFont)
        {
            return GetFontName(idName, fontData, out pdfFont);
        }

        /// <summary>
        /// Gets the resource name of the specified image within this page.
        /// </summary>
        internal string GetImageName(XImage image)
        {
            PdfImage pdfImage = _document.ImageTable.GetImage(image);
            Debug.Assert(pdfImage != null);
            string name = Resources.AddImage(pdfImage);
            return name;
        }

        /// <summary>
        /// Implements the interface because the primary function is internal.
        /// </summary>
        string IContentStream.GetImageName(XImage image)
        {
            return GetImageName(image);
        }

        /// <summary>
        /// Gets the resource name of the specified form within this page.
        /// </summary>
        internal string GetFormName(XForm form)
        {
            PdfFormXObject pdfForm = _document.FormTable.GetForm(form);
            Debug.Assert(pdfForm != null);
            string name = Resources.AddForm(pdfForm);
            return name;
        }

        /// <summary>
        /// Implements the interface because the primary function is internal.
        /// </summary>
        string IContentStream.GetFormName(XForm form)
        {
            return GetFormName(form);
        }

        internal override void WriteObject(PdfWriter writer)
        {
            // HACK: temporarily flip media box if Landscape
            PdfRectangle mediaBox = MediaBox;
            // TODO: Take /Rotate into account
            if (_orientation == PageOrientation.Landscape)
                MediaBox = new PdfRectangle(mediaBox.X1, mediaBox.Y1, mediaBox.Y2, mediaBox.X2);

#if true
            // Add transparency group to prevent rendering problems of Adobe viewer.
            // Update (PDFsharp 1.50 beta 3): Add transparency group only of ColorMode is defined.
            // Rgb is the default for the ColorMode, but if user sets it to Undefined then
            // we respect this and skip the transparency group.
            TransparencyUsed = true; // TODO: check XObjects
            if (TransparencyUsed && !Elements.ContainsKey(Keys.Group) &&
                _document.Options.ColorMode != PdfColorMode.Undefined)
            {
                PdfDictionary group = new PdfDictionary();
                _elements["/Group"] = group;
                if (_document.Options.ColorMode != PdfColorMode.Cmyk)
                    group.Elements.SetName("/CS", "/DeviceRGB");
                else
                    group.Elements.SetName("/CS", "/DeviceCMYK");
                group.Elements.SetName("/S", "/Transparency");
                //False is default: group.Elements["/I"] = new PdfBoolean(false);
                //False is default: group.Elements["/K"] = new PdfBoolean(false);
            }
#endif

#if DEBUG_
            PdfItem item = Elements["/MediaBox"];
            if (item != null)
                item.GetType();
#endif
            base.WriteObject(writer);

            if (_orientation == PageOrientation.Landscape)
                MediaBox = mediaBox;
        }

        /// <summary>
        /// Hack to indicate that a page-level transparency group must be created.
        /// </summary>
        internal bool TransparencyUsed;

        /// <summary>
        /// Inherit values from parent node.
        /// </summary>
        internal static void InheritValues(PdfDictionary page, InheritedValues values)
        {
            // HACK: I'M ABSOLUTELY NOT SURE WHETHER THIS CODE COVERS ALL CASES.
            if (values.Resources != null)
            {
                PdfDictionary resources;
                PdfItem res = page.Elements[InheritablePageKeys.Resources];
                if (res is PdfReference)
                {
                    resources = (PdfDictionary)((PdfReference)res).Value.Clone();
                    resources.Document = page.Owner;
                }
                else
                    resources = (PdfDictionary)res;

                if (resources == null)
                {
                    resources = values.Resources.Clone();
                    resources.Document = page.Owner;
                    page.Elements.Add(InheritablePageKeys.Resources, resources);
                }
                else
                {
                    foreach (PdfName name in values.Resources.Elements.KeyNames)
                    {
                        if (!resources.Elements.ContainsKey(name.Value))
                        {
                            PdfItem item = values.Resources.Elements[name];
                            if (item is PdfObject)
                                item = item.Clone();
                            resources.Elements.Add(name.ToString(), item);
                        }
                    }
                }
            }

            if (values.MediaBox != null && page.Elements[InheritablePageKeys.MediaBox] == null)
                page.Elements[InheritablePageKeys.MediaBox] = values.MediaBox;

            if (values.CropBox != null && page.Elements[InheritablePageKeys.CropBox] == null)
                page.Elements[InheritablePageKeys.CropBox] = values.CropBox;

            if (values.Rotate != null && page.Elements[InheritablePageKeys.Rotate] == null)
                page.Elements[InheritablePageKeys.Rotate] = values.Rotate;
        }

        /// <summary>
        /// Add all inheritable values from the specified page to the specified values structure.
        /// </summary>
        internal static void InheritValues(PdfDictionary page, ref InheritedValues values)
        {
            PdfItem item = page.Elements[InheritablePageKeys.Resources];
            if (item != null)
            {
                PdfReference reference = item as PdfReference;
                if (reference != null)
                    values.Resources = (PdfDictionary)(reference.Value);
                else
                    values.Resources = (PdfDictionary)item;
            }

            item = page.Elements[InheritablePageKeys.MediaBox];
            if (item != null)
                values.MediaBox = new PdfRectangle(item);

            item = page.Elements[InheritablePageKeys.CropBox];
            if (item != null)
                values.CropBox = new PdfRectangle(item);

            item = page.Elements[InheritablePageKeys.Rotate];
            if (item != null)
            {
                if (item is PdfReference)
                    item = ((PdfReference)item).Value;
                values.Rotate = (PdfInteger)item;
            }
        }

        internal override void PrepareForSave()
        {
            if (_trimMargins.AreSet)
            {
                // These are the values InDesign set for an A4 page with 3mm crop margin at each edge.
                // (recall that PDF rect are two points and NOT a point and a width)
                // /MediaBox[0.0 0.0 612.283 858.898]  216 302.7
                // /CropBox[0.0 0.0 612.283 858.898]
                // /BleedBox[0.0 0.0 612.283 858.898]
                // /ArtBox[8.50394 8.50394 603.78 850.394] 3 3 213 300
                // /TrimBox[8.50394 8.50394 603.78 850.394]

                double width = _trimMargins.Left.Point + Width.Point + _trimMargins.Right.Point;
                double height = _trimMargins.Top.Point + Height.Point + _trimMargins.Bottom.Point;

                MediaBox = new PdfRectangle(0, 0, width, height);
                CropBox = new PdfRectangle(0, 0, width, height);
                BleedBox = new PdfRectangle(0, 0, width, height);

                PdfRectangle rect = new PdfRectangle(_trimMargins.Left.Point, _trimMargins.Top.Point,
                  width - _trimMargins.Right.Point, height - _trimMargins.Bottom.Point);
                TrimBox = rect;
                ArtBox = rect.Clone();
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : InheritablePageKeys
        {
            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes;
            /// must be Page for a page object.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "Page")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required; must be an indirect reference)
            /// The page tree node that is the immediate parent of this page object.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required | KeyType.MustBeIndirect)]
            public const string Parent = "/Parent";

            /// <summary>
            /// (Required if PieceInfo is present; optional otherwise; PDF 1.3) The date and time
            /// when the page’s contents were most recently modified. If a page-piece dictionary
            /// (PieceInfo) is present, the modification date is used to ascertain which of the 
            /// application data dictionaries that it contains correspond to the current content
            /// of the page.
            /// </summary>
            [KeyInfo(KeyType.Date)]
            public const string LastModified = "/LastModified";

            /// <summary>
            /// (Optional; PDF 1.3) A rectangle, expressed in default user space units, defining the 
            /// region to which the contents of the page should be clipped when output in a production
            /// environment. Default value: the value of CropBox.
            /// </summary>
            [KeyInfo("1.3", KeyType.Rectangle | KeyType.Optional)]
            public const string BleedBox = "/BleedBox";

            /// <summary>
            /// (Optional; PDF 1.3) A rectangle, expressed in default user space units, defining the
            /// intended dimensions of the finished page after trimming. Default value: the value of 
            /// CropBox.
            /// </summary>
            [KeyInfo("1.3", KeyType.Rectangle | KeyType.Optional)]
            public const string TrimBox = "/TrimBox";

            /// <summary>
            /// (Optional; PDF 1.3) A rectangle, expressed in default user space units, defining the
            /// extent of the page’s meaningful content (including potential white space) as intended
            /// by the page’s creator. Default value: the value of CropBox.
            /// </summary>
            [KeyInfo("1.3", KeyType.Rectangle | KeyType.Optional)]
            public const string ArtBox = "/ArtBox";

            /// <summary>
            /// (Optional; PDF 1.4) A box color information dictionary specifying the colors and other 
            /// visual characteristics to be used in displaying guidelines on the screen for the various
            /// page boundaries. If this entry is absent, the application should use its own current 
            /// default settings.
            /// </summary>
            [KeyInfo("1.4", KeyType.Dictionary | KeyType.Optional)]
            public const string BoxColorInfo = "/BoxColorInfo";

            /// <summary>
            /// (Optional) A content stream describing the contents of this page. If this entry is absent, 
            /// the page is empty. The value may be either a single stream or an array of streams. If the 
            /// value is an array, the effect is as if all of the streams in the array were concatenated,
            /// in order, to form a single stream. This allows PDF producers to create image objects and
            /// other resources as they occur, even though they interrupt the content stream. The division
            /// between streams may occur only at the boundaries between lexical tokens but is unrelated
            /// to the page’s logical content or organization. Applications that consume or produce PDF 
            /// files are not required to preserve the existing structure of the Contents array.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Stream | KeyType.Optional)]
            public const string Contents = "/Contents";

            /// <summary>
            /// (Optional; PDF 1.4) A group attributes dictionary specifying the attributes of the page’s 
            /// page group for use in the transparent imaging model.
            /// </summary>
            [KeyInfo("1.4", KeyType.Dictionary | KeyType.Optional)]
            public const string Group = "/Group";

            /// <summary>
            /// (Optional) A stream object defining the page’s thumbnail image.
            /// </summary>
            [KeyInfo(KeyType.Stream | KeyType.Optional)]
            public const string Thumb = "/Thumb";

            /// <summary>
            /// (Optional; PDF 1.1; recommended if the page contains article beads) An array of indirect
            /// references to article beads appearing on the page. The beads are listed in the array in 
            /// natural reading order.
            /// </summary>
            [KeyInfo("1.1", KeyType.Array | KeyType.Optional)]
            public const string B = "/B";

            /// <summary>
            /// (Optional; PDF 1.1) The page’s display duration (also called its advance timing): the 
            /// maximum length of time, in seconds, that the page is displayed during presentations before
            /// the viewer application automatically advances to the next page. By default, the viewer does 
            /// not advance automatically.
            /// </summary>
            [KeyInfo("1.1", KeyType.Real | KeyType.Optional)]
            public const string Dur = "/Dur";

            /// <summary>
            /// (Optional; PDF 1.1) A transition dictionary describing the transition effect to be used 
            /// when displaying the page during presentations.
            /// </summary>
            [KeyInfo("1.1", KeyType.Dictionary | KeyType.Optional)]
            public const string Trans = "/Trans";

            /// <summary>
            /// (Optional) An array of annotation dictionaries representing annotations associated with 
            /// the page.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional, typeof(PdfAnnotations))]
            public const string Annots = "/Annots";

            /// <summary>
            /// (Optional; PDF 1.2) An additional-actions dictionary defining actions to be performed 
            /// when the page is opened or closed.
            /// </summary>
            [KeyInfo("1.2", KeyType.Dictionary | KeyType.Optional)]
            public const string AA = "/AA";

            /// <summary>
            /// (Optional; PDF 1.4) A metadata stream containing metadata for the page.
            /// </summary>
            [KeyInfo("1.4", KeyType.Stream | KeyType.Optional)]
            public const string Metadata = "/Metadata";

            /// <summary>
            /// (Optional; PDF 1.3) A page-piece dictionary associated with the page.
            /// </summary>
            [KeyInfo("1.3", KeyType.Dictionary | KeyType.Optional)]
            public const string PieceInfo = "/PieceInfo";

            /// <summary>
            /// (Required if the page contains structural content items; PDF 1.3)
            /// The integer key of the page’s entry in the structural parent tree.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string StructParents = "/StructParents";

            /// <summary>
            /// (Optional; PDF 1.3; indirect reference preferred) The digital identifier of
            /// the page’s parent Web Capture content set.
            /// </summary>
            [KeyInfo("1.3", KeyType.String | KeyType.Optional)]
            public const string ID = "/ID";

            /// <summary>
            /// (Optional; PDF 1.3) The page’s preferred zoom (magnification) factor: the factor 
            /// by which it should be scaled to achieve the natural display magnification.
            /// </summary>
            [KeyInfo("1.3", KeyType.Real | KeyType.Optional)]
            public const string PZ = "/PZ";

            /// <summary>
            /// (Optional; PDF 1.3) A separation dictionary containing information needed
            /// to generate color separations for the page.
            /// </summary>
            [KeyInfo("1.3", KeyType.Dictionary | KeyType.Optional)]
            public const string SeparationInfo = "/SeparationInfo";

            /// <summary>
            /// (Optional; PDF 1.5) A name specifying the tab order to be used for annotations
            /// on the page. The possible values are R (row order), C (column order),
            /// and S (structure order).
            /// </summary>
            [KeyInfo("1.5", KeyType.Name | KeyType.Optional)]
            public const string Tabs = "/Tabs";

            /// <summary>
            /// (Required if this page was created from a named page object; PDF 1.5)
            /// The name of the originating page object.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string TemplateInstantiated = "/TemplateInstantiated";

            /// <summary>
            /// (Optional; PDF 1.5) A navigation node dictionary representing the first node
            /// on the page.
            /// </summary>
            [KeyInfo("1.5", KeyType.Dictionary | KeyType.Optional)]
            public const string PresSteps = "/PresSteps";

            /// <summary>
            /// (Optional; PDF 1.6) A positive number giving the size of default user space units,
            /// in multiples of 1/72 inch. The range of supported values is implementation-dependent.
            /// </summary>
            [KeyInfo("1.6", KeyType.Real | KeyType.Optional)]
            public const string UserUnit = "/UserUnit";

            /// <summary>
            /// (Optional; PDF 1.6) An array of viewport dictionaries specifying rectangular regions 
            /// of the page.
            /// </summary>
            [KeyInfo("1.6", KeyType.Dictionary | KeyType.Optional)]
            public const string VP = "/VP";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            internal static DictionaryMeta Meta
            {
                get { return _meta ?? (_meta = CreateMeta(typeof(Keys))); }
            }
            static DictionaryMeta _meta;
        }

        /// <summary>
        /// Gets the KeysMeta of this dictionary type.
        /// </summary>
        internal override DictionaryMeta Meta
        {
            get { return Keys.Meta; }
        }

        /// <summary>
        /// Predefined keys common to PdfPage and PdfPages.
        /// </summary>
        internal class InheritablePageKeys : KeysBase
        {
            /// <summary>
            /// (Required; inheritable) A dictionary containing any resources required by the page. 
            /// If the page requires no resources, the value of this entry should be an empty dictionary.
            /// Omitting the entry entirely indicates that the resources are to be inherited from an 
            /// ancestor node in the page tree.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Required | KeyType.Inheritable, typeof(PdfResources))]
            public const string Resources = "/Resources";

            /// <summary>
            /// (Required; inheritable) A rectangle, expressed in default user space units, defining the 
            /// boundaries of the physical medium on which the page is intended to be displayed or printed.
            /// </summary>
            [KeyInfo(KeyType.Rectangle | KeyType.Required | KeyType.Inheritable)]
            public const string MediaBox = "/MediaBox";

            /// <summary>
            /// (Optional; inheritable) A rectangle, expressed in default user space units, defining the 
            /// visible region of default user space. When the page is displayed or printed, its contents 
            /// are to be clipped (cropped) to this rectangle and then imposed on the output medium in some
            /// implementation defined manner. Default value: the value of MediaBox.
            /// </summary>
            [KeyInfo(KeyType.Rectangle | KeyType.Optional | KeyType.Inheritable)]
            public const string CropBox = "/CropBox";

            /// <summary>
            /// (Optional; inheritable) The number of degrees by which the page should be rotated clockwise 
            /// when displayed or printed. The value must be a multiple of 90. Default value: 0.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string Rotate = "/Rotate";
        }

        /// <summary>
        /// Values inherited from a parent in the parent chain of a page tree.
        /// </summary>
        internal struct InheritedValues
        {
            public PdfDictionary Resources;
            public PdfRectangle MediaBox;
            public PdfRectangle CropBox;
            public PdfInteger Rotate;
        }
    }
}
