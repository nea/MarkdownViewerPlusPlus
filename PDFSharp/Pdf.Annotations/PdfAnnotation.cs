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
using PdfSharp.Drawing;

namespace PdfSharp.Pdf.Annotations
{
    /// <summary>
    /// Represents the base class of all annotations.
    /// </summary>
    public abstract class PdfAnnotation : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotation"/> class.
        /// </summary>
        protected PdfAnnotation()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotation"/> class.
        /// </summary>
        protected PdfAnnotation(PdfDocument document)
            : base(document)
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAnnotation"/> class.
        /// </summary>
        internal PdfAnnotation(PdfDictionary dict)
            : base(dict)
        { }

        void Initialize()
        {
            Elements.SetName(Keys.Type, "/Annot");
            Elements.SetString(Keys.NM, Guid.NewGuid().ToString("D"));
            Elements.SetDateTime(Keys.M, DateTime.Now);
        }

        /// <summary>
        /// Removes an annotation from the document
        /// <seealso cref="PdfAnnotations.Remove(PdfAnnotation)"/>
        /// </summary>
        [Obsolete("Use 'Parent.Remove(this)'")]
        public void Delete()
        {
            Parent.Remove(this);
        }

        /// <summary>
        /// Gets or sets the annotation flags of this instance.
        /// </summary>
        public PdfAnnotationFlags Flags
        {
            get { return (PdfAnnotationFlags)Elements.GetInteger(Keys.F); }
            set
            {
                Elements.SetInteger(Keys.F, (int)value);
                Elements.SetDateTime(Keys.M, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets or sets the PdfAnnotations object that this annotation belongs to.
        /// </summary>
        public PdfAnnotations Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }
        PdfAnnotations _parent;

        /// <summary>
        /// Gets or sets the annotation rectangle, defining the location of the annotation
        /// on the page in default user space units.
        /// </summary>
        public PdfRectangle Rectangle
        {
            get { return Elements.GetRectangle(Keys.Rect, true); }
            set
            {
                Elements.SetRectangle(Keys.Rect, value);
                Elements.SetDateTime(Keys.M, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets or sets the text label to be displayed in the title bar of the annotation’s
        /// pop-up window when open and active. By convention, this entry identifies
        /// the user who added the annotation.
        /// </summary>
        public string Title
        {
            get { return Elements.GetString(Keys.T, true); }
            set
            {
                Elements.SetString(Keys.T, value);
                Elements.SetDateTime(Keys.M, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets or sets text representing a short description of the subject being
        /// addressed by the annotation.
        /// </summary>
        public string Subject
        {
            get { return Elements.GetString(Keys.Subj, true); }
            set
            {
                Elements.SetString(Keys.Subj, value);
                Elements.SetDateTime(Keys.M, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets or sets the text to be displayed for the annotation or, if this type of
        /// annotation does not display text, an alternate description of the annotation’s
        /// contents in human-readable form.
        /// </summary>
        public string Contents
        {
            get { return Elements.GetString(Keys.Contents, true); }
            set
            {
                Elements.SetString(Keys.Contents, value);
                Elements.SetDateTime(Keys.M, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets or sets the color representing the components of the annotation. If the color
        /// has an alpha value other than 1, it is ignored. Use property Opacity to get or set the
        /// opacity of an annotation.
        /// </summary>
        public XColor Color
        {
            get
            {
                PdfItem item = Elements[Keys.C];
                PdfArray array = item as PdfArray;
                if (array != null)  // TODO: check for iref?
                {
                    if (array.Elements.Count == 3)
                    {
                        // TODO: an array.GetColor() function may be useful here
                        return XColor.FromArgb(
                            (int)(array.Elements.GetReal(0) * 255),
                            (int)(array.Elements.GetReal(1) * 255),
                            (int)(array.Elements.GetReal(2) * 255));
                    }
                }
                return XColors.Black;
            }
            set
            {
                // TODO: an array.SetColor(clr) function may be useful here
                PdfArray array = new PdfArray(Owner, new PdfReal[] { new PdfReal(value.R / 255.0), new PdfReal(value.G / 255.0), new PdfReal(value.B / 255.0) });
                Elements[Keys.C] = array;
                Elements.SetDateTime(Keys.M, DateTime.Now);
            }
        }

        /// <summary>
        /// Gets or sets the constant opacity value to be used in painting the annotation.
        /// This value applies to all visible elements of the annotation in its closed state
        /// (including its background and border) but not to the popup window that appears when
        /// the annotation is opened.
        /// </summary>
        public double Opacity
        {
            get
            {
                if (!Elements.ContainsKey(Keys.CA))
                    return 1;
                return Elements.GetReal(Keys.CA, true);
            }
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("value", value, "Opacity must be a value in the range from 0 to 1.");
                Elements.SetReal(Keys.CA, value);
                Elements.SetDateTime(Keys.M, DateTime.Now);
            }
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        public class Keys : KeysBase
        {
            // ReSharper disable InconsistentNaming

            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes; if present,
            /// must be Annot for an annotation dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "Annot")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The type of annotation that this dictionary describes.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string Subtype = "/Subtype";

            /// <summary>
            /// (Required) The annotation rectangle, defining the location of the annotation
            /// on the page in default user space units.
            /// </summary>
            [KeyInfo(KeyType.Rectangle | KeyType.Required)]
            public const string Rect = "/Rect";

            /// <summary>
            /// (Optional) Text to be displayed for the annotation or, if this type of annotation
            /// does not display text, an alternate description of the annotation’s contents
            /// in human-readable form. In either case, this text is useful when
            /// extracting the document’s contents in support of accessibility to users with
            /// disabilities or for other purposes.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string Contents = "/Contents";

            // P

            /// <summary>
            /// (Optional; PDF 1.4) The annotation name, a text string uniquely identifying it
            /// among all the annotations on its page.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string NM = "/NM";

            /// <summary>
            /// (Optional; PDF 1.1) The date and time when the annotation was most recently
            /// modified. The preferred format is a date string, but viewer applications should be 
            /// prepared to accept and display a string in any format.
            /// </summary>
            [KeyInfo(KeyType.Date | KeyType.Optional)]
            public const string M = "/M";

            /// <summary>
            /// (Optional; PDF 1.1) A set of flags specifying various characteristics of the annotation.
            /// Default value: 0.
            /// </summary>
            [KeyInfo("1.1", KeyType.Integer | KeyType.Optional)]
            public const string F = "/F";

            /// <summary>
            /// (Optional; PDF 1.2) A border style dictionary specifying the characteristics of
            /// the annotation’s border.
            /// </summary>
            [KeyInfo("1.2", KeyType.Dictionary | KeyType.Optional)]
            public const string BS = "/BS";

            /// <summary>
            /// (Optional; PDF 1.2) An appearance dictionary specifying how the annotation
            /// is presented visually on the page. Individual annotation handlers may ignore
            /// this entry and provide their own appearances.
            /// </summary>
            [KeyInfo("1.2", KeyType.Dictionary | KeyType.Optional)]
            public const string AP = "/AP";

            /// <summary>
            /// (Required if the appearance dictionary AP contains one or more subdictionaries; PDF 1.2)
            /// The annotation’s appearance state, which selects the applicable appearance stream from 
            /// an appearance subdictionary.
            /// </summary>
            [KeyInfo("1.2", KeyType.Dictionary | KeyType.Optional)]
            public const string AS = "/AS";

            /// <summary>
            /// (Optional) An array specifying the characteristics of the annotation’s border.
            /// The border is specified as a rounded rectangle.
            /// In PDF 1.0, the array consists of three numbers defining the horizontal corner 
            /// radius, vertical corner radius, and border width, all in default user space units.
            /// If the corner radii are 0, the border has square (not rounded) corners; if the border 
            /// width is 0, no border is drawn.
            /// In PDF 1.1, the array may have a fourth element, an optional dash array defining a 
            /// pattern of dashes and gaps to be used in drawing the border. The dash array is 
            /// specified in the same format as in the line dash pattern parameter of the graphics state.
            /// For example, a Border value of [0 0 1 [3 2]] specifies a border 1 unit wide, with
            /// square corners, drawn with 3-unit dashes alternating with 2-unit gaps. Note that no
            /// dash phase is specified; the phase is assumed to be 0.
            /// Note: In PDF 1.2 or later, this entry may be ignored in favor of the BS entry.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Border = "/Border";

            /// <summary>
            /// (Optional; PDF 1.1) An array of three numbers in the range 0.0 to 1.0, representing
            /// the components of a color in the DeviceRGB color space. This color is used for the
            /// following purposes:
            /// • The background of the annotation’s icon when closed
            /// • The title bar of the annotation’s pop-up window
            /// • The border of a link annotation
            /// </summary>
            [KeyInfo("1.1", KeyType.Array | KeyType.Optional)]
            public const string C = "/C";

            /// <summary>
            /// (Optional; PDF 1.1) An action to be performed when the annotation is activated.
            /// Note: This entry is not permitted in link annotations if a Dest entry is present.
            /// Also note that the A entry in movie annotations has a different meaning.
            /// </summary>
            [KeyInfo("1.1", KeyType.Dictionary | KeyType.Optional)]
            public const string A = "/A";

            // AA
            // StructParent
            // OC

            // ----- Excerpt of entries specific to markup annotations ----------------------------------

            /// <summary>
            /// (Optional; PDF 1.1) The text label to be displayed in the title bar of the annotation’s
            /// pop-up window when open and active. By convention, this entry identifies
            /// the user who added the annotation.
            /// </summary>
            [KeyInfo(KeyType.TextString | KeyType.Optional)]
            public const string T = "/T";

            /// <summary>
            /// (Optional; PDF 1.3) An indirect reference to a pop-up annotation for entering or
            /// editing the text associated with this annotation.
            /// </summary>
            [KeyInfo(KeyType.Dictionary | KeyType.Optional)]
            public const string Popup = "/Popup";

            /// <summary>
            /// (Optional; PDF 1.4) The constant opacity value to be used in painting the annotation.
            /// This value applies to all visible elements of the annotation in its closed state
            /// (including its background and border) but not to the popup window that appears when
            /// the annotation is opened.
            /// The specified value is not used if the annotation has an appearance stream; in that
            /// case, the appearance stream must specify any transparency. (However, if the viewer
            /// regenerates the annotation’s appearance stream, it may incorporate the CA value
            /// into the stream’s content.)
            /// The implicit blend mode is Normal.
            /// Default value: 1.0.
            /// </summary>
            [KeyInfo(KeyType.Real | KeyType.Optional)]
            public const string CA = "/CA";

            //RC
            //CreationDate
            //IRT

            /// <summary>
            /// (Optional; PDF 1.5) Text representing a short description of the subject being
            /// addressed by the annotation.
            /// </summary>
            [KeyInfo("1.5", KeyType.TextString | KeyType.Optional)]
            public const string Subj = "/Subj";

            //RT
            //IT
            // ReSharper restore InconsistentNaming
        }
    }
}
