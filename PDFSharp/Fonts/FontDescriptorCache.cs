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
using PdfSharp.Drawing;
using PdfSharp.Fonts.OpenType;
using PdfSharp.Internal;

namespace PdfSharp.Fonts
{
    /// <summary>
    /// Global table of OpenType font descriptor objects.
    /// </summary>
    internal sealed class FontDescriptorCache
    {
        FontDescriptorCache()
        {
            _cache = new Dictionary<string, FontDescriptor>();
        }

        ///// <summary>
        ///// Gets the FontDescriptor identified by the specified FontSelector. If no such object 
        ///// exists, a new FontDescriptor is created and added to the stock.
        ///// </summary>
        //public static FontDescriptor GetOrCreateDescriptor_DEL-ETE(string familyName, XFontStyle stlye, OpenTypeFontface fontface)
        //{
        //    //FontSelector1 selector = new FontSelector1(familyName, stlye);
        //    string fontDescriptorKey = null; // FontDescriptor.ComputeKey(familyName, stlye);
        //    try
        //    {
        //        Lock.EnterFontFactory();
        //        FontDescriptor descriptor;
        //        if (!Singleton._cache.TryGetValue(fontDescriptorKey, out descriptor))
        //        {
        //            descriptor = new OpenTypeDescriptor(fontDescriptorKey, familyName, stlye, fontface, null);
        //            Singleton._cache.Add(fontDescriptorKey, descriptor);
        //        }
        //        return descriptor;
        //    }
        //    finally { Lock.ExitFontFactory(); }
        //}

        /// <summary>
        /// Gets the FontDescriptor identified by the specified XFont. If no such object 
        /// exists, a new FontDescriptor is created and added to the cache.
        /// </summary>
        public static FontDescriptor GetOrCreateDescriptorFor(XFont font)
        {
            if (font == null)
                throw new ArgumentNullException("font");

            //FontSelector1 selector = new FontSelector1(font);
            string fontDescriptorKey = FontDescriptor.ComputeKey(font);
            try
            {
                Lock.EnterFontFactory();
                FontDescriptor descriptor;
                if (!Singleton._cache.TryGetValue(fontDescriptorKey, out descriptor))
                {
                    descriptor = new OpenTypeDescriptor(fontDescriptorKey, font);
                    Singleton._cache.Add(fontDescriptorKey, descriptor);
                }
                return descriptor;
            }
            finally { Lock.ExitFontFactory(); }
        }

        /// <summary>
        /// Gets the FontDescriptor identified by the specified FontSelector. If no such object 
        /// exists, a new FontDescriptor is created and added to the stock.
        /// </summary>
        public static FontDescriptor GetOrCreateDescriptor(string fontFamilyName, XFontStyle style)
        {
            if (string.IsNullOrEmpty(fontFamilyName))
                throw new ArgumentNullException("fontFamilyName");

            //FontSelector1 selector = new FontSelector1(fontFamilyName, style);
            string fontDescriptorKey = FontDescriptor.ComputeKey(fontFamilyName, style);
            try
            {
                Lock.EnterFontFactory();
                FontDescriptor descriptor;
                if (!Singleton._cache.TryGetValue(fontDescriptorKey, out descriptor))
                {
                    XFont font = new XFont(fontFamilyName, 10, style);
                    descriptor = GetOrCreateDescriptorFor(font);
                    if (Singleton._cache.ContainsKey(fontDescriptorKey))
                        Singleton.GetType();
                    else
                        Singleton._cache.Add(fontDescriptorKey, descriptor);
                }
                return descriptor;
            }
            finally { Lock.ExitFontFactory(); }
        }

        public static FontDescriptor GetOrCreateDescriptor(string idName, byte[] fontData)
        {
            //FontSelector1 selector = new FontSelector1(idName);
            string fontDescriptorKey = FontDescriptor.ComputeKey(idName);
            try
            {
                Lock.EnterFontFactory();
                FontDescriptor descriptor;
                if (!Singleton._cache.TryGetValue(fontDescriptorKey, out descriptor))
                {
                    descriptor = GetOrCreateOpenTypeDescriptor(fontDescriptorKey, idName, fontData);
                    Singleton._cache.Add(fontDescriptorKey, descriptor);
                }
                return descriptor;
            }
            finally { Lock.ExitFontFactory(); }
        }

        static OpenTypeDescriptor GetOrCreateOpenTypeDescriptor(string fontDescriptorKey, string idName, byte[] fontData)
        {
            return new OpenTypeDescriptor(fontDescriptorKey, idName, fontData);
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        static FontDescriptorCache Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    try
                    {
                        Lock.EnterFontFactory();
                        if (_singleton == null)
                            _singleton = new FontDescriptorCache();
                    }
                    finally { Lock.ExitFontFactory(); }
                }
                return _singleton;
            }
        }
        static volatile FontDescriptorCache _singleton;

        /// <summary>
        /// Maps font font descriptor key to font descriptor.
        /// </summary>
        readonly Dictionary<string, FontDescriptor> _cache;
    }
}
