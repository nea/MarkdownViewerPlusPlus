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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PdfSharp.Pdf.Advanced;
using PdfSharp.Pdf.Security;
using PdfSharp.Pdf.Internal;

namespace PdfSharp.Pdf.IO
{
    /// <summary>
    /// Encapsulates the arguments of the PdfPasswordProvider delegate.
    /// </summary>
    public class PdfPasswordProviderArgs
    {
        /// <summary>
        /// Sets the password to open the document with.
        /// </summary>
        public string Password;

        /// <summary>
        /// When set to true the PdfReader.Open function returns null indicating that no PdfDocument was created.
        /// </summary>
        public bool Abort;
    }

    /// <summary>
    /// A delegated used by the PdfReader.Open function to retrieve a password if the document is protected.
    /// </summary>
    public delegate void PdfPasswordProvider(PdfPasswordProviderArgs args);

    /// <summary>
    /// Represents the functionality for reading PDF documents.
    /// </summary>
    public static class PdfReader
    {
        /// <summary>
        /// Determines whether the file specified by its path is a PDF file by inspecting the first eight
        /// bytes of the data. If the file header has the form «%PDF-x.y» the function returns the version
        /// number as integer (e.g. 14 for PDF 1.4). If the file header is invalid or inaccessible
        /// for any reason, 0 is returned. The function never throws an exception. 
        /// </summary>
        public static int TestPdfFile(string path)
        {
#if !NETFX_CORE
            FileStream stream = null;
            try
            {
                int pageNumber;
                string realPath = Drawing.XPdfForm.ExtractPageNumber(path, out pageNumber);
                if (File.Exists(realPath)) // prevent unwanted exceptions during debugging
                {
                    stream = new FileStream(realPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    byte[] bytes = new byte[1024];
                    stream.Read(bytes, 0, 1024);
                    return GetPdfFileVersion(bytes);
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
            finally
            {
                try
                {
                    if (stream != null)
                    {
#if UWP
                        stream.Dispose();
#else
                        stream.Close();
#endif
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
            }
#endif
            return 0;
        }

        /// <summary>
        /// Determines whether the specified stream is a PDF file by inspecting the first eight
        /// bytes of the data. If the data begins with «%PDF-x.y» the function returns the version
        /// number as integer (e.g. 14 for PDF 1.4). If the data is invalid or inaccessible
        /// for any reason, 0 is returned. The function never throws an exception. 
        /// </summary>
        public static int TestPdfFile(Stream stream)
        {
            long pos = -1;
            try
            {
                pos = stream.Position;
                byte[] bytes = new byte[1024];
                stream.Read(bytes, 0, 1024);
                return GetPdfFileVersion(bytes);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
            finally
            {
                try
                {
                    if (pos != -1)
                        stream.Position = pos;
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch { }
            }
            return 0;
        }

        /// <summary>
        /// Determines whether the specified data is a PDF file by inspecting the first eight
        /// bytes of the data. If the data begins with «%PDF-x.y» the function returns the version
        /// number as integer (e.g. 14 for PDF 1.4). If the data is invalid or inaccessible
        /// for any reason, 0 is returned. The function never throws an exception. 
        /// </summary>
        public static int TestPdfFile(byte[] data)
        {
            return GetPdfFileVersion(data);
        }

        /// <summary>
        /// Implements scanning the PDF file version.
        /// </summary>
        internal static int GetPdfFileVersion(byte[] bytes)
        {
            try
            {
                // Acrobat accepts headers like «%!PS-Adobe-N.n PDF-M.m»...
                string header = PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);  // Encoding.ASCII.GetString(bytes);
                if (header[0] == '%' || header.IndexOf("%PDF", StringComparison.Ordinal) >= 0)
                {
                    int ich = header.IndexOf("PDF-", StringComparison.Ordinal);
                    if (ich > 0 && header[ich + 5] == '.')
                    {
                        char major = header[ich + 4];
                        char minor = header[ich + 6];
                        if (major >= '1' && major < '2' && minor >= '0' && minor <= '9')
                            return (major - '0') * 10 + (minor - '0');
                    }
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch { }
            return 0;
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(string path, PdfDocumentOpenMode openmode)
        {
            return Open(path, null, openmode, null);
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(string path, PdfDocumentOpenMode openmode, PdfPasswordProvider provider)
        {
            return Open(path, null, openmode, provider);
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(string path, string password, PdfDocumentOpenMode openmode)
        {
            return Open(path, password, openmode, null);
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(string path, string password, PdfDocumentOpenMode openmode, PdfPasswordProvider provider)
        {
#if !NETFX_CORE
            PdfDocument document;
            Stream stream = null;
            try
            {
                stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                document = Open(stream, password, openmode, provider);
                if (document != null)
                {
                    document._fullPath = Path.GetFullPath(path);
                }
            }
            finally
            {
                if (stream != null)
#if !UWP
                    stream.Close();
#else
                    stream.Dispose();
#endif
            }
            return document;
#else
                    return null;
#endif
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(string path)
        {
            return Open(path, null, PdfDocumentOpenMode.Modify, null);
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(string path, string password)
        {
            return Open(path, password, PdfDocumentOpenMode.Modify, null);
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(Stream stream, PdfDocumentOpenMode openmode)
        {
            return Open(stream, null, openmode);
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(Stream stream, PdfDocumentOpenMode openmode, PdfPasswordProvider passwordProvider)
        {
            return Open(stream, null, openmode);
        }
        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(Stream stream, string password, PdfDocumentOpenMode openmode)
        {
            return Open(stream, password, openmode, null);
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(Stream stream, string password, PdfDocumentOpenMode openmode, PdfPasswordProvider passwordProvider)
        {
            PdfDocument document;
            try
            {
                Lexer lexer = new Lexer(stream);
                document = new PdfDocument(lexer);
                document._state |= DocumentState.Imported;
                document._openMode = openmode;
                document._fileSize = stream.Length;

                // Get file version.
                byte[] header = new byte[1024];
                stream.Position = 0;
                stream.Read(header, 0, 1024);
                document._version = GetPdfFileVersion(header);
                if (document._version == 0)
                    throw new InvalidOperationException(PSSR.InvalidPdf);

                document._irefTable.IsUnderConstruction = true;
                Parser parser = new Parser(document);
                // Read all trailers or cross-reference streams, but no objects.
                document._trailer = parser.ReadTrailer();

                Debug.Assert(document._irefTable.IsUnderConstruction);
                document._irefTable.IsUnderConstruction = false;

                // Is document encrypted?
                PdfReference xrefEncrypt = document._trailer.Elements[PdfTrailer.Keys.Encrypt] as PdfReference;
                if (xrefEncrypt != null)
                {
                    //xrefEncrypt.Value = parser.ReadObject(null, xrefEncrypt.ObjectID, false);
                    PdfObject encrypt = parser.ReadObject(null, xrefEncrypt.ObjectID, false, false);

                    encrypt.Reference = xrefEncrypt;
                    xrefEncrypt.Value = encrypt;
                    PdfStandardSecurityHandler securityHandler = document.SecurityHandler;
                    TryAgain:
                    PasswordValidity validity = securityHandler.ValidatePassword(password);
                    if (validity == PasswordValidity.Invalid)
                    {
                        if (passwordProvider != null)
                        {
                            PdfPasswordProviderArgs args = new PdfPasswordProviderArgs();
                            passwordProvider(args);
                            if (args.Abort)
                                return null;
                            password = args.Password;
                            goto TryAgain;
                        }
                        else
                        {
                            if (password == null)
                                throw new PdfReaderException(PSSR.PasswordRequired);
                            else
                                throw new PdfReaderException(PSSR.InvalidPassword);
                        }
                    }
                    else if (validity == PasswordValidity.UserPassword && openmode == PdfDocumentOpenMode.Modify)
                    {
                        if (passwordProvider != null)
                        {
                            PdfPasswordProviderArgs args = new PdfPasswordProviderArgs();
                            passwordProvider(args);
                            if (args.Abort)
                                return null;
                            password = args.Password;
                            goto TryAgain;
                        }
                        else
                            throw new PdfReaderException(PSSR.OwnerPasswordRequired);
                    }
                }
                else
                {
                    if (password != null)
                    {
                        // Password specified but document is not encrypted.
                        // ignore
                    }
                }

                PdfReference[] irefs2 = document._irefTable.AllReferences;
                int count2 = irefs2.Length;

                // 3rd: Create iRefs for all compressed objects.
                Dictionary<int, object> objectStreams = new Dictionary<int, object>();
                for (int idx = 0; idx < count2; idx++)
                {
                    PdfReference iref = irefs2[idx];
                    PdfCrossReferenceStream xrefStream = iref.Value as PdfCrossReferenceStream;
                    if (xrefStream != null)
                    {
                        for (int idx2 = 0; idx2 < xrefStream.Entries.Count; idx2++)
                        {
                            PdfCrossReferenceStream.CrossReferenceStreamEntry item = xrefStream.Entries[idx2];
                            // Is type xref to compressed object?
                            if (item.Type == 2)
                            {
                                //PdfReference irefNew = parser.ReadCompressedObject(new PdfObjectID((int)item.Field2), (int)item.Field3);
                                //document._irefTable.Add(irefNew);
                                int objectNumber = (int)item.Field2;
                                if (!objectStreams.ContainsKey(objectNumber))
                                {
                                    objectStreams.Add(objectNumber, null);
                                    PdfObjectID objectID = new PdfObjectID((int)item.Field2);
                                    parser.ReadIRefsFromCompressedObject(objectID);
                                }
                            }
                        }
                    }
                }

                // 4th: Read compressed objects.
                for (int idx = 0; idx < count2; idx++)
                {
                    PdfReference iref = irefs2[idx];
                    PdfCrossReferenceStream xrefStream = iref.Value as PdfCrossReferenceStream;
                    if (xrefStream != null)
                    {
                        for (int idx2 = 0; idx2 < xrefStream.Entries.Count; idx2++)
                        {
                            PdfCrossReferenceStream.CrossReferenceStreamEntry item = xrefStream.Entries[idx2];
                            // Is type xref to compressed object?
                            if (item.Type == 2)
                            {
                                PdfReference irefNew = parser.ReadCompressedObject(new PdfObjectID((int)item.Field2),
                                    (int)item.Field3);
                                Debug.Assert(document._irefTable.Contains(iref.ObjectID));
                                //document._irefTable.Add(irefNew);
                            }
                        }
                    }
                }


                PdfReference[] irefs = document._irefTable.AllReferences;
                int count = irefs.Length;

                // Read all indirect objects.
                for (int idx = 0; idx < count; idx++)
                {
                    PdfReference iref = irefs[idx];
                    if (iref.Value == null)
                    {
#if DEBUG_
                        if (iref.ObjectNumber == 1074)
                            iref.GetType();
#endif
                        try
                        {
                            Debug.Assert(document._irefTable.Contains(iref.ObjectID));
                            PdfObject pdfObject = parser.ReadObject(null, iref.ObjectID, false, false);
                            Debug.Assert(pdfObject.Reference == iref);
                            pdfObject.Reference = iref;
                            Debug.Assert(pdfObject.Reference.Value != null, "Something went wrong.");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            // 4STLA rethrow exception to notify caller.
                            throw;
                        }
                    }
                    else
                    {
                        Debug.Assert(document._irefTable.Contains(iref.ObjectID));
                        //iref.GetType();
                    }
                    // Set maximum object number.
                    document._irefTable._maxObjectNumber = Math.Max(document._irefTable._maxObjectNumber,
                        iref.ObjectNumber);
                }

                // Encrypt all objects.
                if (xrefEncrypt != null)
                {
                    document.SecurityHandler.EncryptDocument();
                }

                // Fix references of trailer values and then objects and irefs are consistent.
                document._trailer.Finish();

#if DEBUG_
    // Some tests...
                PdfReference[] reachables = document.xrefTable.TransitiveClosure(document.trailer);
                reachables.GetType();
                reachables = document.xrefTable.AllXRefs;
                document.xrefTable.CheckConsistence();
#endif

                if (openmode == PdfDocumentOpenMode.Modify)
                {
                    // Create new or change existing document IDs.
                    if (document.Internals.SecondDocumentID == "")
                        document._trailer.CreateNewDocumentIDs();
                    else
                    {
                        byte[] agTemp = Guid.NewGuid().ToByteArray();
                        document.Internals.SecondDocumentID = PdfEncoders.RawEncoding.GetString(agTemp, 0, agTemp.Length);
                    }

                    // Change modification date
                    document.Info.ModificationDate = DateTime.Now;

                    // Remove all unreachable objects
                    int removed = document._irefTable.Compact();
                    if (removed != 0)
                        Debug.WriteLine("Number of deleted unreachable objects: " + removed);

                    // Force flattening of page tree
                    PdfPages pages = document.Pages;
                    Debug.Assert(pages != null);

                    //bool b = document.irefTable.Contains(new PdfObjectID(1108));
                    //b.GetType();

                    document._irefTable.CheckConsistence();
                    document._irefTable.Renumber();
                    document._irefTable.CheckConsistence();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            return document;
        }

        /// <summary>
        /// Opens an existing PDF document.
        /// </summary>
        public static PdfDocument Open(Stream stream)
        {
            return Open(stream, PdfDocumentOpenMode.Modify);
        }
    }
}
