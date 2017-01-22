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
    /// Encapsulates access to the security settings of a PDF document.
    /// </summary>
    public sealed class PdfSecuritySettings
    {
        internal PdfSecuritySettings(PdfDocument document)
        {
            _document = document;
        }
        readonly PdfDocument _document;

        /// <summary>
        /// Indicates whether the granted access to the document is 'owner permission'. Returns true if the document 
        /// is unprotected or was opened with the owner password. Returns false if the document was opened with the
        /// user password.
        /// </summary>
        public bool HasOwnerPermissions
        {
            get { return _hasOwnerPermissions; }
        }
        internal bool _hasOwnerPermissions = true;

        /// <summary>
        /// Gets or sets the document security level. If you set the security level to anything but PdfDocumentSecurityLevel.None
        /// you must also set a user and/or an owner password. Otherwise saving the document will fail.
        /// </summary>
        public PdfDocumentSecurityLevel DocumentSecurityLevel
        {
            get { return _documentSecurityLevel; }
            set { _documentSecurityLevel = value; }
        }
        PdfDocumentSecurityLevel _documentSecurityLevel;

        /// <summary>
        /// Sets the user password of the document. Setting a password automatically sets the
        /// PdfDocumentSecurityLevel to PdfDocumentSecurityLevel.Encrypted128Bit if its current
        /// value is PdfDocumentSecurityLevel.None.
        /// </summary>
        public string UserPassword
        {
            set { SecurityHandler.UserPassword = value; }
        }

        /// <summary>
        /// Sets the owner password of the document. Setting a password automatically sets the
        /// PdfDocumentSecurityLevel to PdfDocumentSecurityLevel.Encrypted128Bit if its current
        /// value is PdfDocumentSecurityLevel.None.
        /// </summary>
        public string OwnerPassword
        {
            set { SecurityHandler.OwnerPassword = value; }
        }

        /// <summary>
        /// Determines whether the document can be saved.
        /// </summary>
        internal bool CanSave(ref string message)
        {
            if (_documentSecurityLevel != PdfDocumentSecurityLevel.None)
            {
                if (String.IsNullOrEmpty(SecurityHandler._userPassword) && String.IsNullOrEmpty(SecurityHandler._ownerPassword))
                {
                    message = PSSR.UserOrOwnerPasswordRequired;
                    return false;
                }
            }
            return true;
        }

        #region Permissions
        //TODO: Use documentation from our English Acrobat 6.0 version.

        /// <summary>
        /// Permits printing the document. Should be used in conjunction with PermitFullQualityPrint.
        /// </summary>
        public bool PermitPrint
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitPrint) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitPrint;
                else
                    permission &= ~PdfUserAccessPermission.PermitPrint;
                SecurityHandler.Permission = permission;
            }
        }

        /// <summary>
        /// Permits modifying the document.
        /// </summary>
        public bool PermitModifyDocument
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitModifyDocument) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitModifyDocument;
                else
                    permission &= ~PdfUserAccessPermission.PermitModifyDocument;
                SecurityHandler.Permission = permission;
            }
        }

        /// <summary>
        /// Permits content copying or extraction.
        /// </summary>
        public bool PermitExtractContent
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitExtractContent) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitExtractContent;
                else
                    permission &= ~PdfUserAccessPermission.PermitExtractContent;
                SecurityHandler.Permission = permission;
            }
        }

        /// <summary>
        /// Permits commenting the document.
        /// </summary>
        public bool PermitAnnotations
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitAnnotations) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitAnnotations;
                else
                    permission &= ~PdfUserAccessPermission.PermitAnnotations;
                SecurityHandler.Permission = permission;
            }
        }

        /// <summary>
        /// Permits filling of form fields.
        /// </summary>
        public bool PermitFormsFill
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitFormsFill) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitFormsFill;
                else
                    permission &= ~PdfUserAccessPermission.PermitFormsFill;
                SecurityHandler.Permission = permission;
            }
        }

        /// <summary>
        /// Permits content extraction for accessibility.
        /// </summary>
        public bool PermitAccessibilityExtractContent
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitAccessibilityExtractContent) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitAccessibilityExtractContent;
                else
                    permission &= ~PdfUserAccessPermission.PermitAccessibilityExtractContent;
                SecurityHandler.Permission = permission;
            }
        }

        /// <summary>
        /// Permits to insert, rotate, or delete pages and create bookmarks or thumbnail images even if
        /// PermitModifyDocument is not set.
        /// </summary>
        public bool PermitAssembleDocument
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitAssembleDocument) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitAssembleDocument;
                else
                    permission &= ~PdfUserAccessPermission.PermitAssembleDocument;
                SecurityHandler.Permission = permission;
            }
        }

        /// <summary>
        /// Permits to print in high quality. insert, rotate, or delete pages and create bookmarks or thumbnail images
        /// even if PermitModifyDocument is not set.
        /// </summary>
        public bool PermitFullQualityPrint
        {
            get { return (SecurityHandler.Permission & PdfUserAccessPermission.PermitFullQualityPrint) != 0; }
            set
            {
                PdfUserAccessPermission permission = SecurityHandler.Permission;
                if (value)
                    permission |= PdfUserAccessPermission.PermitFullQualityPrint;
                else
                    permission &= ~PdfUserAccessPermission.PermitFullQualityPrint;
                SecurityHandler.Permission = permission;
            }
        }
        #endregion

        /// <summary>
        /// PdfStandardSecurityHandler is the only implemented handler.
        /// </summary>
        internal PdfStandardSecurityHandler SecurityHandler
        {
            get { return _document._trailer.SecurityHandler; }
        }
    }
}
