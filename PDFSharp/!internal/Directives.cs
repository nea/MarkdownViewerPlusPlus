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

//
// Documentation of conditional compilation symbols used in PDFsharp.
// Checks correct settings and obsolete conditional compilation symbols.
//

#if NewViewMatrix // obsolete
#error NewViewMatrix must not be defined anmore.
#endif

#if MIGRADOC  // obsolete
// empira internal only: Some hacks that make PDFsharp behave like PDFlib when used with Asc.RenderContext.
// Applies to MigraDoc 1.2 only. The Open Source MigraDoc lite does not need this define.
#error MIGRADOC must not be defined anmore.
#endif

#if NET_ZIP  // obsolete
// In .NET 2.0 GZipStream is used instead of SharpZipLib
// This does not work as anticipated.
#error Undefine 'NET_ZIP' because it has no effect anymore.
#endif

#if NET_2_0  // obsolete
#error Undefine 'NET_2_0' because earlier versions are not supported anymore.
#endif

#if Gdip  // obsolete
#error Conditional compilation symbol 'Gdip' was renamed to 'GDI'.
#endif

#if GdipUseGdiObjects  // obsolete
#error Conditional compilation symbol 'GdipUseGdiObjects' was renamed to 'UseGdiObjects'.
#endif

// Fragmentation of large object heap is a serious issue that must be tackled in the future.
// Update: .NET 4.51 can ultimately defragment LOH. So maybe we can wait and see.
#if UseMemoryStreams
// Use MemoryStream instead of byte[] to avoid large heap problems.
#error Undefine 'UseMemoryStreams' because it has no effect anymore.
#else
// Use byte[] (instead of MemoryStream) to analyse the symptoms of large heap issues.
#endif

#if GDI && WPF
// PDFsharp based on both System.Drawing and System.Windows classes
// This is for developing and cross testing only
#elif GDI
// PDFsharp based on System.Drawing classes
#if GdipUseGdiObjects
#error Conditional compilation symbol 'GdipUseGdiObjects' was renamed to 'UseGdiObjects'.
#endif

#if UseGdiObjects
// PDFsharp X graphics classes have implicit cast operators for GDI+ objects.
// Define this to make it easier to use older code with PDFsharp.
// Undefine this to prevent dependencies to GDI+
#endif

#elif WPF
// PDFsharp based on Windows Presentation Foundation.
#elif SILVERLIGHT
// PDFsharp based on 'Silverlight'.
#if !WPF
#error 'SILVERLIGHT' must be defined together with 'WPF'
#endif

#elif WINDOWS_PHONE
// PDFsharp based on 'Windows Phone'.
#if !WPF
#error 'WINDOWS_PHONE' must be defined together with 'WPF'.
#endif
#if !SILVERLIGHT
#error 'WINDOWS_PHONE' must be defined together with 'SILVERLIGHT'.
#endif

#elif CORE
// PDFsharp independent of any particular .NET library.
#elif NETFX_CORE
// PDFsharp based on 'WinRT'.
#elif UWP
// PDFsharp based on 'Windows Universal Platform'.

#else
#error Either 'CORE', 'GDI', 'WPF', 'SILVERLIGHT', 'WINDOWS_PHONE', or 'NETFX_CORE' must be defined. Or UWP.
#endif
