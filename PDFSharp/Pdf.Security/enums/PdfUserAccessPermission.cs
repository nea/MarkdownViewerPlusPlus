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

namespace PdfSharp.Pdf.Security
{
    /// <summary>
    /// Specifies which operations are permitted when the document is opened with user access.
    /// </summary>
    [Flags]
    internal enum PdfUserAccessPermission
    {
        /// <summary>
        /// Permits everything. This is the default value.
        /// </summary>
        PermitAll = -3, // = 0xFFFFFFFC,

        // Bit 1–2 Reserved; must be 0.

        // Bit 3 (Revision 2) Print the document.
        // (Revision 3 or greater) Print the document (possibly not at the highest
        // quality level, depending on whether bit 12 is also set).
        PermitPrint = 0x00000004,  //1 << (3 - 1),

        // Bit 4 Modify the contents of the document by operations other than
        // those controlled by bits 6, 9, and 11.
        PermitModifyDocument = 0x00000008,  //1 << (4 - 1),

        // Bit 5 (Revision 2) Copy or otherwise extract text and graphics from the
        // document, including extracting text and graphics (in support of accessibility
        // to users with disabilities or for other purposes).
        // (Revision 3 or greater) Copy or otherwise extract text and graphics
        // from the document by operations other than that controlled by bit 10.
        PermitExtractContent = 0x00000010,  //1 << (5 - 1),

        // Bit 6 Add or modify text annotations, fill in interactive form fields, and,
        // if bit 4 is also set, create or modify interactive form fields (including
        // signature fields).
        PermitAnnotations = 0x00000020,  //1 << (6 - 1),

        // Bit 7–8 Reserved; must be 1.

        // 9 (Revision 3 or greater) Fill in existing interactive form fields (including
        // signature fields), even if bit 6 is clear.
        PermitFormsFill = 0x00000100,  //1 << (9 - 1),

        // Bit 10 (Revision 3 or greater) Extract text and graphics (in support of accessibility
        // to users with disabilities or for other purposes).
        PermitAccessibilityExtractContent = 0x00000200,  //1 << (10 - 1),

        // Bit 11 (Revision 3 or greater) Assemble the document (insert, rotate, or delete
        // pages and create bookmarks or thumbnail images), even if bit 4
        // is clear.
        PermitAssembleDocument = 0x00000400,  //1 << (11 - 1),

        // Bit 12 (Revision 3 or greater) Print the document to a representation from
        // which a faithful digital copy of the PDF content could be generated.
        // When this bit is clear (and bit 3 is set), printing is limited to a lowlevel
        // representation of the appearance, possibly of degraded quality.
        // (See implementation note 24 in Appendix H.)
        PermitFullQualityPrint = 0x00000800,  //1 << (12 - 1),

        //Bit 13–32 (Revision 3 or greater) Reserved; must be 1.
    }
}
