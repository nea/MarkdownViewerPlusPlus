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

namespace PdfSharp.Pdf.Annotations
{
    /// <summary>
    /// Specifies the annotation flags.
    /// </summary>
    public enum PdfAnnotationFlags
    {
        /// <summary>
        /// If set, do not display the annotation if it does not belong to one of the standard
        /// annotation types and no annotation handler is available. If clear, display such an
        /// unknown annotation using an appearance stream specified by its appearancedictionary,
        /// if any.
        /// </summary>
        Invisible = 1 << (1 - 1),

        /// <summary>
        /// (PDF 1.2) If set, do not display or print the annotation or allow it to interact
        /// with the user, regardless of its annotation type or whether an annotation
        /// handler is available. In cases where screen space is limited, the ability to hide
        /// and show annotations selectively can be used in combination with appearance
        /// streams to display auxiliary pop-up information similar in function to online
        /// help systems.
        /// </summary>
        Hidden = 1 << (2 - 1),

        /// <summary>
        /// (PDF 1.2) If set, print the annotation when the page is printed. If clear, never
        /// print the annotation, regardless of whether it is displayed on the screen. This
        /// can be useful, for example, for annotations representing interactive pushbuttons,
        /// which would serve no meaningful purpose on the printed page.
        /// </summary>
        Print = 1 << (3 - 1),

        /// <summary>
        /// (PDF 1.3) If set, do not scale the annotation’s appearance to match the magnification
        /// of the page. The location of the annotation on the page (defined by the
        /// upper-left corner of its annotation rectangle) remains fixed, regardless of the
        /// page magnification. See below for further discussion.
        /// </summary>
        NoZoom = 1 << (4 - 1),

        /// <summary>
        /// (PDF 1.3) If set, do not rotate the annotation’s appearance to match the rotation
        /// of the page. The upper-left corner of the annotation rectangle remains in a fixed
        /// location on the page, regardless of the page rotation. See below for further discussion.
        /// </summary>
        NoRotate = 1 << (5 - 1),

        /// <summary>
        /// (PDF 1.3) If set, do not display the annotation on the screen or allow it to
        /// interact with the user. The annotation may be printed (depending on the setting
        /// of the Print flag) but should be considered hidden for purposes of on-screen
        /// display and user interaction.
        /// </summary>
        NoView = 1 << (6 - 1),

        /// <summary>
        /// (PDF 1.3) If set, do not allow the annotation to interact with the user. The
        /// annotation may be displayed or printed (depending on the settings of the
        /// NoView and Print flags) but should not respond to mouse clicks or change its
        /// appearance in response to mouse motions.
        /// Note: This flag is ignored for widget annotations; its function is subsumed by
        /// the ReadOnly flag of the associated form field.
        /// </summary>
        ReadOnly = 1 << (7 - 1),

        /// <summary>
        /// (PDF 1.4) If set, do not allow the annotation to be deleted or its properties
        /// (including position and size) to be modified by the user. However, this flag does
        /// not restrict changes to the annotation’s contents, such as the value of a form
        /// field.
        /// </summary>
        Locked = 1 << (8 - 1),

        /// <summary>
        /// (PDF 1.5) If set, invert the interpretation of the NoView flag for certain events.
        /// A typical use is to have an annotation that appears only when a mouse cursor is
        /// held over it.
        /// </summary>
        ToggleNoView = 1 << (9 - 1),
    }
}
