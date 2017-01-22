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

using PdfSharp.Drawing;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf.AcroForms
{
    /// <summary>
    /// Represents the text field.
    /// </summary>
    public sealed class PdfTextField : PdfAcroField
    {
        /// <summary>
        /// Initializes a new instance of PdfTextField.
        /// </summary>
        internal PdfTextField(PdfDocument document)
            : base(document)
        { }

        internal PdfTextField(PdfDictionary dict)
            : base(dict)
        { }

        /// <summary>
        /// Gets or sets the text value of the text field.
        /// </summary>
        public string Text
        {
            get { return Elements.GetString(Keys.V); }
            set { Elements.SetString(Keys.V, value); RenderAppearance(); } //HACK in PdfTextField
        }

        /// <summary>
        /// Gets or sets the font used to draw the text of the field.
        /// </summary>
        public XFont Font
        {
            get { return _font; }
            set { _font = value; }
        }
        XFont _font = new XFont("Courier New", 10);

        /// <summary>
        /// Gets or sets the foreground color of the field.
        /// </summary>
        public XColor ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; }
        }
        XColor _foreColor = XColors.Black;

        /// <summary>
        /// Gets or sets the background color of the field.
        /// </summary>
        public XColor BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }
        XColor _backColor = XColor.Empty;

        /// <summary>
        /// Gets or sets the maximum length of the field.
        /// </summary>
        /// <value>The length of the max.</value>
        public int MaxLength
        {
            get { return Elements.GetInteger(Keys.MaxLen); }
            set { Elements.SetInteger(Keys.MaxLen, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the field has multiple lines.
        /// </summary>
        public bool MultiLine
        {
            get { return (Flags & PdfAcroFieldFlags.Multiline) != 0; }
            set
            {
                if (value)
                    SetFlags |= PdfAcroFieldFlags.Multiline;
                else
                    SetFlags &= ~PdfAcroFieldFlags.Multiline;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this field is used for passwords.
        /// </summary>
        public bool Password
        {
            get { return (Flags & PdfAcroFieldFlags.Password) != 0; }
            set
            {
                if (value)
                    SetFlags |= PdfAcroFieldFlags.Password;
                else
                    SetFlags &= ~PdfAcroFieldFlags.Password;
            }
        }

        /// <summary>
        /// Creates the normal appearance form X object for the annotation that represents
        /// this acro form text field.
        /// </summary>
        void RenderAppearance()
        {
#if true_
            PdfFormXObject xobj = new PdfFormXObject(Owner);
            Owner.Internals.AddObject(xobj);
            xobj.Elements["/BBox"] = new PdfLiteral("[0 0 122.653 12.707]");
            xobj.Elements["/FormType"] = new PdfLiteral("1");
            xobj.Elements["/Matrix"] = new PdfLiteral("[1 0 0 1 0 0]");
            PdfDictionary res = new PdfDictionary(Owner);
            xobj.Elements["/Resources"] = res;
            res.Elements["/Font"] = new PdfLiteral("<< /Helv 28 0 R >> /ProcSet [/PDF /Text]");
            xobj.Elements["/Subtype"] = new PdfLiteral("/Form");
            xobj.Elements["/Type"] = new PdfLiteral("/XObject");

            string s =
              "/Tx BMC " + '\n' +
              "q" + '\n' +
              "1 1 120.653 10.707 re" + '\n' +
              "W" + '\n' +
              "n" + '\n' +
              "BT" + '\n' +
              "/Helv 7.93 Tf" + '\n' +
              "0 g" + '\n' +
              "2 3.412 Td" + '\n' +
              "(Hello ) Tj" + '\n' +
              "20.256 0 Td" + '\n' +
              "(XXX) Tj" + '\n' +
              "ET" + '\n' +
              "Q" + '\n' +
              "";//"EMC";
            int length = s.Length;
            byte[] stream = new byte[length];
            for (int idx = 0; idx < length; idx++)
                stream[idx] = (byte)s[idx];
            xobj.CreateStream(stream);

            // Get existing or create new appearance dictionary
            PdfDictionary ap = Elements[PdfAnnotation.Keys.AP] as PdfDictionary;
            if (ap == null)
            {
                ap = new PdfDictionary(_document);
                Elements[PdfAnnotation.Keys.AP] = ap;
            }

            // Set XRef to normal state
            ap.Elements["/N"] = xobj.Reference;




            //// HACK
            //string m =
            //"<?xpacket begin=\"ï»¿\" id=\"W5M0MpCehiHzreSzNTczkc9d\"?>" + '\n' +
            //"<x:xmpmeta xmlns:x=\"adobe:ns:meta/\" x:xmptk=\"Adobe XMP Core 4.0-c321 44.398116, Tue Aug 04 2009 14:24:39\"> " + '\n' +
            //"   <rdf:RDF xmlns:rdf=\"http://www.w3.org/1999/02/22-rdf-syntax-ns#\"> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:pdf=\"http://ns.adobe.com/pdf/1.3/\"> " + '\n' +
            //"         <pdf:Producer>PDFsharp 1.40.2150-g (www.pdfsharp.com) (Original: Powered By Crystal)</pdf:Producer> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:xap=\"http://ns.adobe.com/xap/1.0/\"> " + '\n' +
            //"         <xap:ModifyDate>2011-07-11T23:15:09+02:00</xap:ModifyDate> " + '\n' +
            //"         <xap:CreateDate>2011-05-19T16:26:51+03:00</xap:CreateDate> " + '\n' +
            //"         <xap:MetadataDate>2011-07-11T23:15:09+02:00</xap:MetadataDate> " + '\n' +
            //"         <xap:CreatorTool>Crystal Reports</xap:CreatorTool> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:dc=\"http://purl.org/dc/elements/1.1/\"> " + '\n' +
            //"         <dc:format>application/pdf</dc:format> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"      <rdf:Description rdf:about=\"\" " + '\n' +
            //"            xmlns:xapMM=\"http://ns.adobe.com/xap/1.0/mm/\"> " + '\n' +
            //"         <xapMM:DocumentID>uuid:68249d89-baed-4384-9a2d-fbf8ace75c45</xapMM:DocumentID> " + '\n' +
            //"         <xapMM:InstanceID>uuid:3d5f2f46-c140-416f-baf2-7f9c970cef1d</xapMM:InstanceID> " + '\n' +
            //"      </rdf:Description> " + '\n' +
            //"   </rdf:RDF> " + '\n' +
            //"</x:xmpmeta> " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"                                                                          " + '\n' +
            //"<?xpacket end=\"w\"?>";

            //PdfDictionary mdict = (PdfDictionary)_document.Internals.GetObject(new PdfObjectID(32));

            //length = m.Length;
            //stream = new byte[length];
            //for (int idx = 0; idx < length; idx++)
            //  stream[idx] = (byte)m[idx];

            //mdict.Stream.Value = stream;




#else
            PdfRectangle rect = Elements.GetRectangle(PdfAnnotation.Keys.Rect);
            XForm form = new XForm(_document, rect.Size);
            XGraphics gfx = XGraphics.FromForm(form);

            if (_backColor != XColor.Empty)
                gfx.DrawRectangle(new XSolidBrush(BackColor), rect.ToXRect() - rect.Location);

            string text = Text;
            if (text.Length > 0)
                gfx.DrawString(Text, Font, new XSolidBrush(ForeColor),
                  rect.ToXRect() - rect.Location + new XPoint(2, 0), XStringFormats.TopLeft);

            form.DrawingFinished();
            form.PdfForm.Elements.Add("/FormType", new PdfLiteral("1"));

            // Get existing or create new appearance dictionary.
            PdfDictionary ap = Elements[PdfAnnotation.Keys.AP] as PdfDictionary;
            if (ap == null)
            {
                ap = new PdfDictionary(_document);
                Elements[PdfAnnotation.Keys.AP] = ap;
            }

            // Set XRef to normal state
            ap.Elements["/N"] = form.PdfForm.Reference;

            PdfFormXObject xobj = form.PdfForm;
            string s = xobj.Stream.ToString();
            // Thank you Adobe: Without putting the content in 'EMC brackets'
            // the text is not rendered by PDF Reader 9 or higher.
            s = "/Tx BMC\n" + s + "\nEMC";
            xobj.Stream.Value = new RawEncoding().GetBytes(s);
#endif
        }

        internal override void PrepareForSave()
        {
            base.PrepareForSave();
            RenderAppearance();
        }

        /// <summary>
        /// Predefined keys of this dictionary. 
        /// The description comes from PDF 1.4 Reference.
        /// </summary>
        public new class Keys : PdfAcroField.Keys
        {
            /// <summary>
            /// (Optional; inheritable) The maximum length of the field’s text, in characters.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public const string MaxLen = "/MaxLen";

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
    }
}
