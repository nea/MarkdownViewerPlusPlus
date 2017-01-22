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

namespace PdfSharp.Pdf
{
    /// <summary>
    /// Represents the PDF document viewer preferences dictionary.
    /// </summary>
    public sealed class PdfViewerPreferences : PdfDictionary
    {
        internal PdfViewerPreferences(PdfDocument document)
            : base(document)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfViewerPreferences"/> class.
        /// </summary>
        PdfViewerPreferences(PdfDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets or sets a value indicating whether to hide the viewer application’s tool
        /// bars when the document is active.
        /// </summary>
        public bool HideToolbar
        {
            get { return Elements.GetBoolean(Keys.HideToolbar); }
            set { Elements.SetBoolean(Keys.HideToolbar, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide the viewer application’s
        /// menu bar when the document is active.
        /// </summary>
        public bool HideMenubar
        {
            get { return Elements.GetBoolean(Keys.HideMenubar); }
            set { Elements.SetBoolean(Keys.HideMenubar, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to hide user interface elements in
        /// the document’s window (such as scroll bars and navigation controls),
        /// leaving only the document’s contents displayed.
        /// </summary>
        public bool HideWindowUI
        {
            get { return Elements.GetBoolean(Keys.HideWindowUI); }
            set { Elements.SetBoolean(Keys.HideWindowUI, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to resize the document’s window to
        /// fit the size of the first displayed page.
        /// </summary>
        public bool FitWindow
        {
            get { return Elements.GetBoolean(Keys.FitWindow); }
            set { Elements.SetBoolean(Keys.FitWindow, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to position the document’s window
        /// in the center of the screen.
        /// </summary>
        public bool CenterWindow
        {
            get { return Elements.GetBoolean(Keys.CenterWindow); }
            set { Elements.SetBoolean(Keys.CenterWindow, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the window’s title bar
        /// should display the document title taken from the Title entry of the document
        /// information dictionary. If false, the title bar should instead display the name
        /// of the PDF file containing the document.
        /// </summary>
        public bool DisplayDocTitle
        {
            get { return Elements.GetBoolean(Keys.DisplayDocTitle); }
            set { Elements.SetBoolean(Keys.DisplayDocTitle, value); }
        }

        /// <summary>
        /// The predominant reading order for text: LeftToRight or RightToLeft 
        /// (including vertical writing systems, such as Chinese, Japanese, and Korean).
        /// This entry has no direct effect on the document’s contents or page numbering
        /// but can be used to determine the relative positioning of pages when displayed 
        /// side by side or printed n-up. Default value: LeftToRight.
        /// </summary>
        public PdfReadingDirection? Direction
        {
            get
            {
                switch (Elements.GetName(Keys.Direction))
                {
                    case "L2R":
                        return PdfReadingDirection.LeftToRight;

                    case "R2L":
                        return PdfReadingDirection.RightToLeft;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    switch (value.Value)
                    {
                        case PdfReadingDirection.RightToLeft:
                            Elements.SetName(Keys.Direction, "R2L");
                            break;

                        default:
                            Elements.SetName(Keys.Direction, "L2R");
                            break;
                    }
                }
                else
                    Elements.Remove(Keys.Direction);
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal sealed class Keys : KeysBase
        {
            /// <summary>
            /// (Optional) A flag specifying whether to hide the viewer application’s tool
            ///  bars when the document is active. Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string HideToolbar = "/HideToolbar";

            /// <summary>
            /// (Optional) A flag specifying whether to hide the viewer application’s
            /// menu bar when the document is active. Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string HideMenubar = "/HideMenubar";

            /// <summary>
            /// (Optional) A flag specifying whether to hide user interface elements in
            ///  the document’s window (such as scroll bars and navigation controls),
            ///  leaving only the document’s contents displayed. Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string HideWindowUI = "/HideWindowUI";

            /// <summary>
            /// (Optional) A flag specifying whether to resize the document’s window to
            /// fit the size of the first displayed page. Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string FitWindow = "/FitWindow";

            /// <summary>
            /// (Optional) A flag specifying whether to position the document’s window
            /// in the center of the screen. Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string CenterWindow = "/CenterWindow";

            /// <summary>
            /// (Optional; PDF 1.4) A flag specifying whether the window’s title bar
            /// should display the document title taken from the Title entry of the document
            /// information dictionary. If false, the title bar should instead display the name
            /// of the PDF file containing the document. Default value: false.
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string DisplayDocTitle = "/DisplayDocTitle";

            /// <summary>
            /// (Optional) The document’s page mode, specifying how to display the document on 
            /// exiting full-screen mode:
            ///   UseNone     Neither document outline nor thumbnail images visible
            ///   UseOutlines Document outline visible
            ///   UseThumbs   Thumbnail images visible
            ///   UseOC       Optional content group panel visible
            /// This entry is meaningful only if the value of the PageMode entry in the catalog 
            /// dictionary is FullScreen; it is ignored otherwise. Default value: UseNone.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string NonFullScreenPageMode = "/NonFullScreenPageMode";

            /// <summary>
            /// (Optional; PDF 1.3) The predominant reading order for text:
            ///   L2R  Left to right
            ///   R2L  Right to left (including vertical writing systems, such as Chinese, Japanese, and Korean)
            /// This entry has no direct effect on the document’s contents or page numbering
            /// but can be used to determine the relative positioning of pages when displayed 
            /// side by side or printed n-up. Default value: L2R.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Direction = "/Direction";

            /// <summary>
            /// (Optional; PDF 1.4) The name of the page boundary representing the area of a page
            /// to be displayed when viewing the document on the screen. The value is the key 
            /// designating the relevant page boundary in the page object. If the specified page
            /// boundary is not defined in the page object, its default value is used.
            /// Default value: CropBox.
            /// Note: This entry is intended primarily for use by prepress applications that
            /// interpret or manipulate the page boundaries as described in Section 10.10.1, “Page Boundaries.”
            /// Most PDF consumer applications disregard it.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string ViewArea = "/ViewArea";

            /// <summary>
            /// (Optional; PDF 1.4) The name of the page boundary to which the contents of a page 
            /// are to be clipped when viewing the document on the screen. The value is the key 
            /// designating the relevant page boundary in the page object. If the specified page 
            /// boundary is not defined in the page object, its default value is used.
            /// Default value: CropBox.
            /// Note: This entry is intended primarily for use by prepress applications that
            /// interpret or manipulate the page boundaries as described in Section 10.10.1, “Page Boundaries.”
            /// Most PDF consumer applications disregard it.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string ViewClip = "/ViewClip";

            /// <summary>
            /// (Optional; PDF 1.4) The name of the page boundary representing the area of a page
            /// to be rendered when printing the document. The value is the key designating the 
            /// relevant page boundary in the page object. If the specified page boundary is not 
            /// defined in the page object, its default value is used.
            /// Default value: CropBox.
            /// Note: This entry is intended primarily for use by prepress applications that
            /// interpret or manipulate the page boundaries as described in Section 10.10.1, “Page Boundaries.”
            /// Most PDF consumer applications disregard it.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string PrintArea = "/PrintArea";

            /// <summary>
            /// (Optional; PDF 1.4) The name of the page boundary to which the contents of a page
            /// are to be clipped when printing the document. The value is the key designating the 
            /// relevant page boundary in the page object. If the specified page boundary is not
            /// defined in the page object, its default value is used.
            /// Default value: CropBox.
            /// Note: This entry is intended primarily for use by prepress applications that interpret
            /// or manipulate the page boundaries. Most PDF consumer applications disregard it.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string PrintClip = "/PrintClip";

            /// <summary>
            /// (Optional; PDF 1.6) The page scaling option to be selected when a print dialog is
            /// displayed for this document. Valid values are None, which indicates that the print
            /// dialog should reflect no page scaling, and AppDefault, which indicates that 
            /// applications should use the current print scaling. If this entry has an unrecognized
            /// value, applications should use the current print scaling.
            /// Default value: AppDefault.
            /// Note: If the print dialog is suppressed and its parameters are provided directly
            /// by the application, the value of this entry should still be used.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string PrintScaling = "/PrintScaling";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            public static DictionaryMeta Meta
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
    }
}