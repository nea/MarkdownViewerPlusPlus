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
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PdfSharp.Internal;

namespace PdfSharp.Fonts.OpenType
{
    /// <summary>
    /// Global table of all OpenType fontfaces chached by their face name and check sum.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    internal class OpenTypeFontfaceCache
    {
        OpenTypeFontfaceCache()
        {
            _fontfaceCache = new Dictionary<string, OpenTypeFontface>(StringComparer.OrdinalIgnoreCase);
            _fontfacesByCheckSum = new Dictionary<ulong, OpenTypeFontface>();
        }

        /// <summary>
        /// Tries to get fontface by its key.
        /// </summary>
        public static bool TryGetFontface(string key, out OpenTypeFontface fontface)
        {
            try
            {
                Lock.EnterFontFactory();
                bool result = Singleton._fontfaceCache.TryGetValue(key, out fontface);
                return result;
            }
            finally { Lock.ExitFontFactory(); }
        }

        /// <summary>
        /// Tries to get fontface by its check sum.
        /// </summary>
        public static bool TryGetFontface(ulong checkSum, out OpenTypeFontface fontface)
        {
            try
            {
                Lock.EnterFontFactory();
                bool result = Singleton._fontfacesByCheckSum.TryGetValue(checkSum, out fontface);
                return result;
            }
            finally { Lock.ExitFontFactory(); }
        }

        public static OpenTypeFontface AddFontface(OpenTypeFontface fontface)
        {
            try
            {
                Lock.EnterFontFactory();
                OpenTypeFontface fontfaceCheck;
                if (TryGetFontface(fontface.FullFaceName, out fontfaceCheck))
                {
                    if (fontfaceCheck.CheckSum != fontface.CheckSum)
                        throw new InvalidOperationException("OpenTypeFontface with same signature but different bytes.");
                    return fontfaceCheck;
                }
                Singleton._fontfaceCache.Add(fontface.FullFaceName, fontface);
                Singleton._fontfacesByCheckSum.Add(fontface.CheckSum, fontface);
                return fontface;
            }
            finally { Lock.ExitFontFactory(); }
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>
        static OpenTypeFontfaceCache Singleton
        {
            get
            {
                // ReSharper disable once InvertIf
                if (_singleton == null)
                {
                    try
                    {
                        Lock.EnterFontFactory();
                        if (_singleton == null)
                            _singleton = new OpenTypeFontfaceCache();
                    }
                    finally { Lock.ExitFontFactory(); }
                }
                return _singleton;
            }
        }
        static volatile OpenTypeFontfaceCache _singleton;

        internal static string GetCacheState()
        {
            StringBuilder state = new StringBuilder();
            state.Append("====================\n");
            state.Append("OpenType fontfaces by name\n");
            Dictionary<string, OpenTypeFontface>.KeyCollection familyKeys = Singleton._fontfaceCache.Keys;
            int count = familyKeys.Count;
            string[] keys = new string[count];
            familyKeys.CopyTo(keys, 0);
            Array.Sort(keys, StringComparer.OrdinalIgnoreCase);
            foreach (string key in keys)
                state.AppendFormat("  {0}: {1}\n", key, Singleton._fontfaceCache[key].DebuggerDisplay);
            state.Append("\n");
            return state.ToString();
        }

        /// <summary>
        /// Maps face name to OpenType fontface.
        /// </summary>
        readonly Dictionary<string, OpenTypeFontface> _fontfaceCache;

        /// <summary>
        /// Maps font source key to OpenType fontface.
        /// </summary>
        readonly Dictionary<ulong, OpenTypeFontface> _fontfacesByCheckSum;

        /// <summary>
        /// Gets the DebuggerDisplayAttribute text.
        /// </summary>
        // ReSharper disable UnusedMember.Local
        string DebuggerDisplay
        // ReSharper restore UnusedMember.Local
        {
            get { return string.Format(CultureInfo.InvariantCulture, "Fontfaces: {0}", _fontfaceCache.Count); }
        }
    }
}
