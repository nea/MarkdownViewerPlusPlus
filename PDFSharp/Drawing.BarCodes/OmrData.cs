#region PDFsharp - A .NET library for processing PDF
//
// Authors:
//   Klaus Potzesny
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

namespace PdfSharp.Drawing.BarCodes
{
#if true_
    /// <summary>
    /// Represents the data coded within the OMR code.
    /// </summary>
    class OmrData
    {
        private OmrData()
        {
        }

        public static OmrData ForTesting
        {
            get
            {
                OmrData data = new OmrData();
                data.AddMarkDescription("LK");
                data.AddMarkDescription("DGR");
                data.AddMarkDescription("GM1");
                data.AddMarkDescription("GM2");
                data.AddMarkDescription("GM4");
                data.AddMarkDescription("GM8");
                data.AddMarkDescription("GM16");
                data.AddMarkDescription("GM32");
                data.AddMarkDescription("ZS1");
                data.AddMarkDescription("ZS2");
                data.AddMarkDescription("ZS3");
                data.AddMarkDescription("ZS4");
                data.AddMarkDescription("ZS5");
                data.InitMarks();
                return data;
            }
        }

        ///// <summary>
        ///// NYI: Get OMR description read from text file.
        ///// </summary>
        ///// <returns>An OmrData object.</returns>
        //public static OmrData FromDescriptionFile(string filename)
        //{
        //  throw new NotImplementedException();
        //}

        /// <summary>
        /// Adds a mark description by name.
        /// </summary>
        /// <param name="name">The name to for setting or unsetting the mark.</param>
        private void AddMarkDescription(string name)
        {
            if (_marksInitialized)
                throw new InvalidOperationException(BcgSR.OmrAlreadyInitialized);

            _nameToIndex[name] = AddedDescriptions;
            ++AddedDescriptions;
        }

        private void InitMarks()
        {
            if (AddedDescriptions == 0)
                throw new InvalidOperationException();

            _marks = new bool[AddedDescriptions];
            _marks.Initialize();
            _marksInitialized = true;
        }

        private int FindIndex(string name)
        {
            if (!_marksInitialized)
                InitMarks();

            if (!_nameToIndex.Contains(name))
                throw new ArgumentException(BcgSR.InvalidMarkName(name));

            return (int)_nameToIndex[name];
        }

        public void SetMark(string name)
        {
            int idx = FindIndex(name);
            _marks[idx] = true;
        }

        public void UnsetMark(string name)
        {
            int idx = FindIndex(name);
            _marks[idx] = false;
        }

        public bool[] Marks
        {
            get { return _marks; }
        }
        System.Collections.Hash_table nameToIndex = new Hash_table();
        bool[] marks;
        int addedDescriptions = 0;
        bool marksInitialized = false;
    }
#endif
}
