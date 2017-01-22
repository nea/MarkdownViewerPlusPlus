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
#if GDI
using System.Drawing;
#endif
#if WPF
using System.Windows;
#endif

#pragma warning disable 649

namespace PdfSharp.Internal
{
    struct SColor
    {
        public byte a;
        public byte r;
        public byte g;
        public byte b;
    }

    struct SCColor
    {
        public float a;
        public float r;
        public float g;
        public float b;
    }

    static class ColorHelper
    {
        public static float sRgbToScRgb(byte bval)
        {
            float num = ((float)bval) / 255f;
            if (num <= 0.0)
                return 0f;
            if (num <= 0.04045)
                return (num / 12.92f);
            if (num < 1f)
                return (float)Math.Pow((num + 0.055) / 1.055, 2.4);
            return 1f;
        }

        public static byte ScRgbTosRgb(float val)
        {
            if (val <= 0.0)
                return 0;
            if (val <= 0.0031308)
                return (byte)(((255f * val) * 12.92f) + 0.5f);
            if (val < 1.0)
                return (byte)((255f * ((1.055f * ((float)Math.Pow((double)val, 0.41666666666666669))) - 0.055f)) + 0.5f);
            return 0xff;
        }
    }
}
