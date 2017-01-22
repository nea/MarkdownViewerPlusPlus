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
using System.Resources;
using System.Reflection;
using PdfSharp.Drawing;
using PdfSharp.Internal;
using PdfSharp.Pdf;

#pragma warning disable 1591

namespace PdfSharp
{
    /// <summary>
    /// The Pdf-Sharp-String-Resources.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    static class PSSR
    {
        // How to use:
        // Create a function or property for each message text, depending on how many parameters are
        // part of the message. For the beginning, type plain English text in the function or property. 
        // The use of functions is safe when a parameter must be changed. The compiler tells you all
        // places in your code that must be modified.
        // For localization, create an enum value for each function or property with the same name. Then
        // create localized message files with the enum values as messages identifiers. In the properties
        // and functions all text is replaced by Format or GetString functions with the corresponding enum value
        // as first parameter. The use of enums ensures that typing errors in message resource names are 
        // simply impossible. Use the TestResourceMessages function to ensure that each enum value has an 
        // appropriate message text.

        #region Helper functions
        /// <summary>
        /// Loads the message from the resource associated with the enum type and formats it
        /// using 'String.Format'. Because this function is intended to be used during error
        /// handling it never raises an exception.
        /// </summary>
        /// <param name="id">The type of the parameter identifies the resource
        /// and the name of the enum identifies the message in the resource.</param>
        /// <param name="args">Parameters passed through 'String.Format'.</param>
        /// <returns>The formatted message.</returns>
        public static string Format(PSMsgID id, params object[] args)
        {
            string message;
            try
            {
                message = GetString(id);
                message = message != null ? Format(message, args) : "INTERNAL ERROR: Message not found in resources.";
                return message;
            }
            catch (Exception ex)
            {
                message = String.Format("UNEXPECTED ERROR while formatting message with ID {0}: {1}", id.ToString(), ex.ToString());
            }
            return message;
        }

        public static string Format(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            string message;
            try
            {
                message = String.Format(format, args);
            }
            catch (Exception ex)
            {
                message = String.Format("UNEXPECTED ERROR while formatting message '{0}': {1}", format, ex);
            }
            return message;
        }

        /// <summary>
        /// Gets the localized message identified by the specified DomMsgID.
        /// </summary>
        public static string GetString(PSMsgID id)
        {
            return ResMngr.GetString(id.ToString());
        }

        #endregion

        #region General messages

        public static string IndexOutOfRange
        {
            get { return "The index is out of range."; }
        }

        public static string ListEnumCurrentOutOfRange
        {
            get { return "Enumeration out of range."; }
        }

        public static string PageIndexOutOfRange
        {
            get { return "The index of a page is out of range."; }
        }

        public static string OutlineIndexOutOfRange
        {
            get { return "The index of an outline is out of range."; }
        }

        public static string SetValueMustNotBeNull
        {
            get { return "The set value property must not be null."; }
        }

        public static string InvalidValue(int val, string name, int min, int max)
        {
            return Format("{0} is not a valid value for {1}. {1} should be greater than or equal to {2} and less than or equal to {3}.",
              val, name, min, max);
        }

        public static string ObsoleteFunktionCalled
        {
            get { return "The function is obsolete and must not be called."; }
        }

        public static string OwningDocumentRequired
        {
            get { return "The PDF object must belong to a PdfDocument, but property Document is null."; }
        }

        public static string FileNotFound(string path)
        {
            return Format("The file '{0}' does not exist.", path);
        }

        public static string FontDataReadOnly
        {
            get { return "Font data is read-only."; }
        }

        public static string ErrorReadingFontData
        {
            get { return "Error while parsing an OpenType font."; }
        }

        #endregion

        #region XGraphics specific messages

        // ----- XGraphics ----------------------------------------------------------------------------

        public static string PointArrayEmpty
        {
            get { return "The PointF array must not be empty."; }
        }

        public static string PointArrayAtLeast(int count)
        {
            return Format("The point array must contain {0} or more points.", count);
        }

        public static string NeedPenOrBrush
        {
            get { return "XPen or XBrush or both must not be null."; }
        }

        public static string CannotChangeImmutableObject(string typename)
        {
            return String.Format("You cannot change this immutable {0} object.", typename);
        }

        public static string FontAlreadyAdded(string fontname)
        {
            return String.Format("Fontface with the name '{0}' already added to font collection.", fontname);
        }

        public static string NotImplementedForFontsRetrievedWithFontResolver(string name)
        {
            return String.Format("Not implemented for font '{0}', because it was retrieved with font resolver.", name);
        }

        #endregion

        #region PDF specific messages

        // ----- PDF ----------------------------------------------------------------------------------

        public static string InvalidPdf
        {
            get { return "The file is not a valid PDF document."; }
        }

        public static string InvalidVersionNumber
        {
            get { return "Invalid version number. Valid values are 12, 13, and 14."; }
        }

        public static string CannotHandleXRefStreams
        {
            get { return "Cannot handle cross-reference streams. The current implementation of PDFsharp cannot handle this PDF feature introduced with Acrobat 6."; }
        }

        public static string PasswordRequired
        {
            get { return "A password is required to open the PDF document."; }
        }

        public static string InvalidPassword
        {
            get { return "The specified password is invalid."; }
        }

        public static string OwnerPasswordRequired
        {
            get { return "To modify the document the owner password is required"; }
        }

        public static string UserOrOwnerPasswordRequired
        {
            get { return GetString(PSMsgID.UserOrOwnerPasswordRequired); }
            //get { return "At least a user or an owner password is required to encrypt the document."; }
        }

        public static string CannotModify
        {
            get { return "The document cannot be modified."; }
        }

        public static string NameMustStartWithSlash
        {
            //get { return GetString(PSMsgID.NameMustStartWithSlash); }
            get { return "A PDF name must start with a slash (/)."; }
        }

        public static string ImportPageNumberOutOfRange(int pageNumber, int maxPage, string path)
        {
            return String.Format("The page cannot be imported from document '{2}', because the page number is out of range. " +
              "The specified page number is {0}, but it must be in the range from 1 to {1}.", pageNumber, maxPage, path);
        }

        public static string MultiplePageInsert
        {
            get { return "The page cannot be added to this document because the document already owned this page."; }
        }

        public static string UnexpectedTokenInPdfFile
        {
            get { return "Unexpected token in PDF file. The PDF file may be corrupt. If it is not, please send us the file for service."; }
        }

        public static string InappropriateColorSpace(PdfColorMode colorMode, XColorSpace colorSpace)
        {
            string mode;
            switch (colorMode)
            {
                case PdfColorMode.Rgb:
                    mode = "RGB";
                    break;

                case PdfColorMode.Cmyk:
                    mode = "CMYK";
                    break;

                default:
                    mode = "(undefined)";
                    break;
            }

            string space;
            switch (colorSpace)
            {
                case XColorSpace.Rgb:
                    space = "RGB";
                    break;

                case XColorSpace.Cmyk:
                    space = "CMYK";
                    break;

                case XColorSpace.GrayScale:
                    space = "grayscale";
                    break;

                default:
                    space = "(undefined)";
                    break;
            }
            return String.Format("The document requires color mode {0}, but a color is defined using {1}. " +
              "Use only colors that match the color mode of the PDF document", mode, space);
        }

        public static string CannotGetGlyphTypeface(string fontName)
        {
            return Format("Cannot get a matching glyph typeface for font '{0}'.", fontName);
        }


        // ----- PdfParser ----------------------------------------------------------------------------

        public static string UnexpectedToken(string token)
        {
            return Format(PSMsgID.UnexpectedToken, token);
            //return Format("Token '{0}' was not expected.", token);
        }

        public static string UnknownEncryption
        {
            get { return GetString(PSMsgID.UnknownEncryption); }
            //get { return "The PDF document is protected with an encryption not supported by PDFsharp."; }
        }

        #endregion

        #region Resource manager

        /// <summary>
        /// Gets the resource manager for this module.
        /// </summary>
        public static ResourceManager ResMngr
        {
            get
            {
                if (_resmngr == null)
                {
                    try
                    {
                        Lock.EnterFontFactory();
                        if (_resmngr == null)
                        {
#if true_
                            // Force the English language.
                            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
#endif
#if !NETFX_CORE && !UWP
                            _resmngr = new ResourceManager("PdfSharp.Resources.Messages",
                                Assembly.GetExecutingAssembly());
#else
                            _resmngr = new ResourceManager("PdfSharp.Resources.Messages",
                                typeof(PSSR).GetTypeInfo().Assembly);
#endif
                        }
                    }
                    finally { Lock.ExitFontFactory(); }
                }
                return _resmngr;
            }
        }
        static ResourceManager _resmngr;

        /// <summary>
        /// Writes all messages defined by PSMsgID.
        /// </summary>
        [Conditional("DEBUG")]
        public static void TestResourceMessages()
        {
#if !SILVERLIGHT
            string[] names = Enum.GetNames(typeof(PSMsgID));
            foreach (string name in names)
            {
                string message = String.Format("{0}: '{1}'", name, ResMngr.GetString(name));
                Debug.Assert(message != null);
                Debug.WriteLine(message);
            }
#else
#endif
        }

        static PSSR()
        {
            TestResourceMessages();
        }

        #endregion
    }
}