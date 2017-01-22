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

namespace PdfSharp.Pdf.Annotations
{
    /// <summary>
    /// Represents a text annotation.
    /// </summary>
    public sealed class PdfTextAnnotation : PdfAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextAnnotation"/> class.
        /// </summary>
        public PdfTextAnnotation()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfTextAnnotation"/> class.
        /// </summary>
        public PdfTextAnnotation(PdfDocument document)
            : base(document)
        {
            Initialize();
        }

        void Initialize()
        {
            Elements.SetName(Keys.Subtype, "/Text");
            // By default make a yellow comment.
            Icon = PdfTextAnnotationIcon.Comment;
            //Color = XColors.Yellow;
        }

        //    public static PdfTextAnnotation CreateDocumentLink(PdfRectangle rect, int destinatinPage)
        //    {
        //      PdfTextAnnotation link = new PdfTextAnnotation();
        //      //link.linkType = PdfTextAnnotation.LinkType.Document;
        //      //link.Rectangle = rect;
        //      //link.destPage = destinatinPage;
        //      return link;
        //    }

        /// <summary>
        /// Gets or sets a flag indicating whether the annotation should initially be displayed open.
        /// </summary>
        public bool Open
        {
            get { return Elements.GetBoolean(Keys.Open); }
            set { Elements.SetBoolean(Keys.Open, value); }
        }

        /// <summary>
        /// Gets or sets an icon to be used in displaying the annotation.
        /// </summary>
        public PdfTextAnnotationIcon Icon
        {
            get
            {
                string value = Elements.GetName(Keys.Name);
                if (value == "")
                    return PdfTextAnnotationIcon.NoIcon;
                value = value.Substring(1);
                if (!Enum.IsDefined(typeof(PdfTextAnnotationIcon), value))
                    return PdfTextAnnotationIcon.NoIcon;
                return (PdfTextAnnotationIcon)Enum.Parse(typeof(PdfTextAnnotationIcon), value, false);
            }
            set
            {
                if (Enum.IsDefined(typeof(PdfTextAnnotationIcon), value) &&
                  PdfTextAnnotationIcon.NoIcon != value)
                {
                    Elements.SetName(Keys.Name, "/" + value.ToString());
                }
                else
                    Elements.Remove(Keys.Name);
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal new class Keys : PdfAnnotation.Keys
        {
            /// <summary>
            /// (Optional) A flag specifying whether the annotation should initially be displayed open.
            /// Default value: false (closed).
            /// </summary>
            [KeyInfo(KeyType.Boolean | KeyType.Optional)]
            public const string Open = "/Open";

            /// <summary>
            /// (Optional) The name of an icon to be used in displaying the annotation. Viewer
            /// applications should provide predefined icon appearances for at least the following
            /// standard names:
            ///   Comment 
            ///   Help 
            ///   Insert
            ///   Key 
            ///   NewParagraph 
            ///   Note
            ///   Paragraph
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional)]
            public const string Name = "/Name";

            //State
            //StateModel

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
