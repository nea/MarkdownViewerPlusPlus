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

using System.Collections.Generic;
using System.Diagnostics;

namespace PdfSharp.Pdf.Advanced
{
    /// <summary>
    /// Represents a PDF cross-reference stream.
    /// </summary>
    internal sealed class PdfCrossReferenceStream : PdfTrailer  // Reference: 3.4.7  Cross-Reference Streams / Page 106
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfObjectStream"/> class.
        /// </summary>
        public PdfCrossReferenceStream(PdfDocument document)
            : base(document)
        {
#if DEBUG && CORE
            if (Internal.PdfDiagnostics.TraceXrefStreams)
            {
                Debug.WriteLine("PdfCrossReferenceStream created.");
            }
#endif
        }

        public readonly List<CrossReferenceStreamEntry> Entries = new List<CrossReferenceStreamEntry>();

        public struct CrossReferenceStreamEntry
        {
            // Reference: TABLE 3.16  Entries in a cross-refernece stream / Page 109

            public uint Type;  // 0, 1, or 2.

            public uint Field2;

            public uint Field3;
        }

        /// <summary>
        /// Predefined keys for cross-reference dictionaries.
        /// </summary>
        public new class Keys : PdfTrailer.Keys  // Reference: TABLE 3.15  Additional entries specific to a cross-refernece stream dictionary / Page 107
        {
            /// <summary>
            /// (Required) The type of PDF object that this dictionary describes;
            /// must be XRef for a cross-reference stream.
            /// </summary>
            [KeyInfo(KeyType.Name | KeyType.Required, FixedValue = "XRef")]
            public const string Type = "/Type";

            /// <summary>
            /// (Required) The number one greater than the highest object number
            /// used in this section or in any section for which this is an update.
            /// It is equivalent to the Size entry in a trailer dictionary.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Required)]
            public new const string Size = "/Size";

            /// <summary>
            /// (Optional) An array containing a pair of integers for each subsection in this section.
            /// The first integer is the first object number in the subsection; the second integer
            /// is the number of entries in the subsection.
            /// The array is sorted in ascending order by object number. Subsections cannot overlap;
            /// an object number may have at most one entry in a section.
            /// Default value: [0 Size].
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Optional)]
            public const string Index = "/Index";

            /// <summary>
            /// (Present only if the file has more than one cross-reference stream; not meaningful in 
            /// hybrid-reference files) The byte offset from the beginning of the file to the beginning
            /// of the previous cross-reference stream. This entry has the same function as the Prev 
            /// entry in the trailer dictionary.
            /// </summary>
            [KeyInfo(KeyType.Integer | KeyType.Optional)]
            public new const string Prev = "/Prev";

            /// <summary>
            /// (Required) An array of integers representing the size of the fields in a single 
            /// cross-reference entry. The table describes the types of entries and their fields.
            /// For PDF 1.5, W always contains three integers; the value of each integer is the
            /// number of bytes (in the decoded stream) of the corresponding field. For example,
            /// [1 2 1] means that the fields are one byte, two bytes, and one byte, respectively.
            /// 
            /// A value of zero for an element in the W array indicates that the corresponding field
            /// is not present in the stream, and the default value is used, if there is one. If the
            /// first element is zero, the type field is not present, and it defaults to type 1.
            /// 
            /// The sum of the items is the total length of each entry; it can be used with the
            /// Indexarray to determine the starting position of each subsection.
            /// 
            /// Note: Different cross-reference streams in a PDF file may use different values for W.
            /// 
            /// Entries in a cross-reference stream.
            /// 
            /// TYPE FIELD DESCRIPTION
            ///   0  1  The type of this entry, which must be 0. Type 0 entries define the linked list of free objects (corresponding to f entries in a cross-reference table).
            ///      2  The object number of the next free object.
            ///      3  The generation number to use if this object number is used again.
            ///   1  1  The type of this entry, which must be 1. Type 1 entries define objects that are in use but are not compressed (corresponding to n entries in a cross-reference table).
            ///      2  The byte offset of the object, starting from the beginning of the file.
            ///      3  The generation number of the object. Default value: 0.
            ///   2  1  The type of this entry, which must be 2. Type 2 entries define compressed objects.
            ///      2  The object number of the object stream in which this object is stored. (The generation number of the object stream is implicitly 0.)
            ///      3  The index of this object within the object stream.
            /// </summary>
            [KeyInfo(KeyType.Array | KeyType.Required)]
            public const string W = "/W";

            /// <summary>
            /// Gets the KeysMeta for these keys.
            /// </summary>
            public static new DictionaryMeta Meta
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
