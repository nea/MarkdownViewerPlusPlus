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

namespace PdfSharp
{
    /// <summary>
    /// Version info base for all PDFsharp related assemblies.
    /// </summary>
    public static class ProductVersionInfo
    {
        /// <summary>
        /// The title of the product.
        /// </summary>
        public const string Title = "PDFsharp";

        /// <summary>
        /// A characteristic description of the product.
        /// </summary>
        public const string Description = "A .NET library for processing PDF.";

        /// <summary>
        /// The PDF producer information string.
        /// TODO: Called Creator in MigraDoc???
        /// </summary>
        public const string Producer = Title + " " + VersionMajor + "." + VersionMinor + "." + VersionBuild + Technology + " (" + Url + ")";

        /// <summary>
        /// The PDF producer information string including VersionPatch.
        /// </summary>
        public const string Producer2 = Title + " " + VersionMajor + "." + VersionMinor + "." + VersionBuild + "." + VersionPatch + Technology + " (" + Url + ")";

        /// <summary>
        /// The full version number.
        /// </summary>
        public const string Version = VersionMajor + "." + VersionMinor + "." + VersionBuild + "." + VersionPatch;

        /// <summary>
        /// The full version string.
        /// </summary>
        public const string Version2 = VersionMajor + "." + VersionMinor + "." + VersionBuild + "." + VersionPatch + Technology;

        /// <summary>
        /// The home page of this product.
        /// </summary>
        public const string Url = "www.pdfsharp.com";

        /// <summary>
        /// Unused.
        /// </summary>
        public const string Configuration = "";

        /// <summary>
        /// The company that created/owned the product.
        /// </summary>
        public const string Company = "empira Software GmbH, Cologne Area (Germany)";

        /// <summary>
        /// The name the product.
        /// </summary>
        public const string Product = "PDFsharp";

        /// <summary>
        /// The copyright information.
        /// </summary>
        public const string Copyright = "Copyright © 2005-2016 empira Software GmbH.";

        /// <summary>
        /// The trademark the product.
        /// </summary>
        public const string Trademark = "PDFsharp";

        /// <summary>
        /// Unused.
        /// </summary>
        public const string Culture = "";

        /// <summary>
        /// The major version number of the product.
        /// </summary>
        public const string VersionMajor = "1";

        /// <summary>
        /// The minor version number of the product.
        /// </summary>
        public const string VersionMinor = "50";

        /// <summary>
        /// The build number of the product.
        /// </summary>
        public const string VersionBuild = "4000";  // V16G // Build = days since 2005-01-01  -  change this values ONLY HERE

        /// <summary>
        /// The patch number of the product.
        /// </summary>
        public const string VersionPatch = "0";

        /// <summary>
        /// The Version Prerelease String for NuGet.
        /// </summary>
        public const string VersionPrerelease = "beta3b"; // "" for stable Release, e.g. "beta" or "rc.1.2" for Prerelease. // Also used for NuGet Version.

#if DEBUG
        /// <summary>
        /// The calculated build number.
        /// </summary>
// ReSharper disable RedundantNameQualifier
        public static int BuildNumber = (System.DateTime.Now - new System.DateTime(2005, 1, 1)).Days;
// ReSharper restore RedundantNameQualifier
#endif

        /// <summary>
        /// E.g. "2005-01-01", for use in NuGet Script.
        /// </summary>
        public const string VersionReferenceDate = "2005-01-01";

        /// <summary>
        /// Use _ instead of blanks and special characters. Can be complemented with a suffix in the NuGet Script.
        /// Nuspec Doc: The unique identifier for the package. This is the package name that is shown when packages
        /// are listed using the Package Manager Console. These are also used when installing a package using the
        /// Install-Package command within the Package Manager Console. Package IDs may not contain any spaces
        /// or characters that are invalid in an URL. In general, they follow the same rules as .NET namespaces do.
        /// So Foo.Bar is a valid ID, Foo! and Foo Bar are not. 
        /// </summary>
        public const string NuGetID = "PDFsharp";

        /// <summary>
        /// Nuspec Doc: The human-friendly title of the package displayed in the Manage NuGet Packages dialog.
        /// If none is specified, the ID is used instead. 
        /// </summary>
        public const string NuGetTitle = "PDFsharp";

        /// <summary>
        /// Nuspec Doc: A comma-separated list of authors of the package code.
        /// </summary>
        public const string NuGetAuthors = "empira Software GmbH";

        /// <summary>
        /// Nuspec Doc: A comma-separated list of the package creators. This is often the same list as in authors.
        /// This is ignored when uploading the package to the NuGet.org Gallery. 
        /// </summary>
        public const string NuGetOwners = "empira Software GmbH";

        /// <summary>
        /// Nuspec Doc: A long description of the package. This shows up in the right pane of the Add Package Dialog
        /// as well as in the Package Manager Console when listing packages using the Get-Package command. 
        /// </summary>
        // This assignment must be written in one line because it will be parsed from a PS1 file.
        public const string NuGetDescription = "PDFsharp is the Open Source .NET library that easily creates and processes PDF documents on the fly from any .NET language. The same drawing routines can be used to create PDF documents, draw on the screen, or send output to any printer.";

        /// <summary>
        /// Nuspec Doc: A description of the changes made in each release of the package. This field only shows up
        /// when the _Updates_ tab is selected and the package is an update to a previously installed package.
        /// It is displayed where the Description would normally be displayed. 
        /// </summary>                  
        public const string NuGetReleaseNotes = "";

        /// <summary>
        /// Nuspec Doc: A short description of the package. If specified, this shows up in the middle pane of the
        /// Add Package Dialog. If not specified, a truncated version of the description is used instead.
        /// </summary>                  
        public const string NuGetSummary = "A .NET library for processing PDF.";

        /// <summary>
        /// Nuspec Doc: The locale ID for the package, such as en-us.
        /// </summary>                  
        public const string NuGetLanguage = "";

        /// <summary>
        /// Nuspec Doc: A URL for the home page of the package.
        /// </summary>
        /// <remarks>
        /// http://www.pdfsharp.net/NuGetPackage_PDFsharp-GDI.ashx
        /// http://www.pdfsharp.net/NuGetPackage_PDFsharp-WPF.ashx
        /// </remarks>
        public const string NuGetProjectUrl = "www.pdfsharp.net";

        /// <summary>
        /// Nuspec Doc: A URL for the image to use as the icon for the package in the Manage NuGet Packages
        /// dialog box. This should be a 32x32-pixel .png file that has a transparent background.
        /// </summary>
        public const string NuGetIconUrl = "http://www.pdfsharp.net/resources/PDFsharp-Logo-32x32.png";

        /// <summary>
        /// Nuspec Doc: A link to the license that the package is under.
        /// </summary>                  
        public const string NuGetLicenseUrl = "http://www.pdfsharp.net/PDFsharp_License.ashx";

        /// <summary>
        /// Nuspec Doc: A Boolean value that specifies whether the client needs to ensure that the package license (described by licenseUrl) is accepted before the package is installed.
        /// </summary>                  
        public const bool NuGetRequireLicenseAcceptance = false;

        /// <summary>
        /// Nuspec Doc: A space-delimited list of tags and keywords that describe the package. This information is used to help make sure users can find the package using
        /// searches in the Add Package Reference dialog box or filtering in the Package Manager Console window.
        /// </summary>                  
        public const string NuGetTags = "PDFsharp PDF creation";

        /// <summary>
        /// The technology tag of the product:
        /// (none) Pure .NET
        /// -gdi : GDI+,
        /// -wpf : WPF,
        /// -hybrid : Both GDI+ and WPF (hybrid).
        /// -sl : Silverlight
        /// -wp : Windows Phone
        /// -wrt : Windows RunTime
        /// </summary>
#if GDI && !WPF
        // GDI+ (System.Drawing)
        public const string Technology = "-gdi";
#endif
#if WPF && !GDI && !SILVERLIGHT
        // Windows Presentation Foundation
        public const string Technology = "-wpf";
#endif
#if WPF && GDI
        // Hybrid - for testing only
        public const string Technology = "-h";
#endif
#if SILVERLIGHT && !WINDOWS_PHONE
        // Silverlight 5
        public const string Technology = "-sl";
#endif
#if WINDOWS_PHONE
        // Windows Phone
        public const string Technology = "-wp";
#endif
#if NETFX_CORE
        // WinRT
        public const string Technology = "-wrt";
#endif
#if UWP
        // Windows Universal App
        public const string Technology = "-uwp";
#endif
#if CORE
        // .net without GDI+ and WPF
        public const string Technology = "";  // no extension
#endif
    }
}
