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

namespace PdfSharp.Pdf.Actions
{
    /// <summary>
    /// Represents the base class for all PDF actions.
    /// </summary>
    public abstract class PdfAction : PdfDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAction"/> class.
        /// </summary>
        protected PdfAction()
        {
            Elements.SetName(Keys.Type, "/Action");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfAction"/> class.
        /// </summary>
        /// <param name="document">The document that owns this object.</param>
        protected PdfAction(PdfDocument document)
            : base(document)
        {
            Elements.SetName(Keys.Type, "/Action");
        }

        /// <summary>
        /// Predefined keys of this dictionary.
        /// </summary>
        internal class Keys : KeysBase
        {
            /// <summary>
            /// (Optional) The type of PDF object that this dictionary describes;
            /// if present, must be Action for an action dictionary.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Optional, FixedValue = "Action")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The type of action that this dictionary describes.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required)]
            public const string S = "/S";

            /// <summary>
            /// (Optional; PDF 1.2) The next action or sequence of actions to be performed
            /// after the action represented by this dictionary. The value is either a
            /// single action dictionary or an array of action dictionaries to be performed
            /// in order; see below for further discussion.
            /// </summary>
            [KeyInfo(KeyType.ArrayOrDictionary | KeyType.Optional)]
            public const string Next = "/Next";
        }
    }
}
