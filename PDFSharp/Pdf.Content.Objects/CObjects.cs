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
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

namespace PdfSharp.Pdf.Content.Objects  // TODO: split into single files
{
    /// <summary>
    /// Base class for all PDF content stream objects.
    /// </summary>
    public abstract class CObject : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CObject"/> class.
        /// </summary>
        protected CObject()
        { }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        object ICloneable.Clone()
        {
            return Copy();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public CObject Clone()
        {
            return Copy();
        }

        /// <summary>
        /// Implements the copy mechanism. Must be overridden in derived classes.
        /// </summary>
        protected virtual CObject Copy()
        {
            return (CObject)MemberwiseClone();
        }

        /// <summary>
        /// 
        /// </summary>
        internal abstract void WriteObject(ContentWriter writer);
    }

    /// <summary>
    /// Represents a comment in a PDF content stream.
    /// </summary>
    [DebuggerDisplay("({Text})")]
    public class CComment : CObject
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CComment Clone()
        {
            return (CComment)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        /// <summary>
        /// Gets or sets the comment text.
        /// </summary>
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }
        string _text;

        /// <summary>
        /// Returns a string that represents the current comment.
        /// </summary>
        public override string ToString()
        {
            return "% " + _text;
        }

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteLineRaw(ToString());
        }
    }

    /// <summary>
    /// Represents a sequence of objects in a PDF content stream.
    /// </summary>
    [DebuggerDisplay("(count={Count})")]
    public class CSequence : CObject, IList<CObject>  // , ICollection<CObject>, IEnumerable<CObject>
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CSequence Clone()
        {
            return (CSequence)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            _items = new List<CObject>(_items);
            for (int idx = 0; idx < _items.Count; idx++)
                _items[idx] = _items[idx].Clone();
            return obj;
        }

        /// <summary>
        /// Adds the specified sequence.
        /// </summary>
        /// <param name="sequence">The sequence.</param>
        public void Add(CSequence sequence)
        {
            int count = sequence.Count;
            for (int idx = 0; idx < count; idx++)
                _items.Add(sequence[idx]);
        }

        #region IList Members

        /// <summary>
        /// Adds the specified value add the end of the sequence.
        /// </summary>
        public void Add(CObject value)
        {
            _items.Add(value);
        }

        /// <summary>
        /// Removes all elements from the sequence.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        //bool IList.Contains(object value)
        //{
        //  return items.Contains(value);
        //}

        /// <summary>
        /// Determines whether the specified value is in the sequence.
        /// </summary>
        public bool Contains(CObject value)
        {
            return _items.Contains(value);
        }

        /// <summary>
        /// Returns the index of the specified value in the sequence or -1, if no such value is in the sequence.
        /// </summary>
        public int IndexOf(CObject value)
        {
            return _items.IndexOf(value);
        }

        /// <summary>
        /// Inserts the specified value in the sequence.
        /// </summary>
        public void Insert(int index, CObject value)
        {
            _items.Insert(index, value);
        }

        /////// <summary>
        /////// Gets a value indicating whether the sequence has a fixed size.
        /////// </summary>
        ////public bool IsFixedSize
        ////{
        ////  get { return items.IsFixedSize; }
        ////}

        /////// <summary>
        /////// Gets a value indicating whether the sequence is read-only.
        /////// </summary>
        ////public bool IsReadOnly
        ////{
        ////  get { return items.IsReadOnly; }
        ////}

        /// <summary>
        /// Removes the specified value from the sequence.
        /// </summary>
        public bool Remove(CObject value)
        {
            return _items.Remove(value);
        }

        /// <summary>
        /// Removes the value at the specified index from the sequence.
        /// </summary>
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        /// <summary>
        /// Gets or sets a CObject at the specified index.
        /// </summary>
        /// <value></value>
        public CObject this[int index]
        {
            get { return (CObject)_items[index]; }
            set { _items[index] = value; }
        }
        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies the elements of the sequence to the specified array.
        /// </summary>
        public void CopyTo(CObject[] array, int index)
        {
            _items.CopyTo(array, index);
        }


        /// <summary>
        /// Gets the number of elements contained in the sequence.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        ///// <summary>
        ///// Gets a value indicating whether access to the sequence is synchronized (thread safe).
        ///// </summary>
        //public bool IsSynchronized
        //{
        //  get { return items.IsSynchronized; }
        //}

        ///// <summary>
        ///// Gets an object that can be used to synchronize access to the sequence.
        ///// </summary>
        //public object SyncRoot
        //{
        //  get { return items.SyncRoot; }
        //}

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the sequence.
        /// </summary>
        public IEnumerator<CObject> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Converts the sequence to a PDF content stream.
        /// </summary>
        public byte[] ToContent()
        {
            Stream stream = new MemoryStream();
            ContentWriter writer = new ContentWriter(stream);
            WriteObject(writer);
            writer.Close(false);

            stream.Position = 0;
            int count = (int)stream.Length;
            byte[] bytes = new byte[count];
            stream.Read(bytes, 0, count);
#if !UWP
            stream.Close();
#else
            stream.Dispose();
#endif
            return bytes;
        }

        /// <summary>
        /// Returns a string containing all elements of the sequence.
        /// </summary>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            for (int idx = 0; idx < _items.Count; idx++)
                s.Append(_items[idx]);

            return s.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal override void WriteObject(ContentWriter writer)
        {
            for (int idx = 0; idx < _items.Count; idx++)
                _items[idx].WriteObject(writer);
        }

        #region IList<CObject> Members

        int IList<CObject>.IndexOf(CObject item)
        {
            throw new NotImplementedException();
        }

        void IList<CObject>.Insert(int index, CObject item)
        {
            throw new NotImplementedException();
        }

        void IList<CObject>.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        CObject IList<CObject>.this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection<CObject> Members

        void ICollection<CObject>.Add(CObject item)
        {
            throw new NotImplementedException();
        }

        void ICollection<CObject>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<CObject>.Contains(CObject item)
        {
            throw new NotImplementedException();
        }

        void ICollection<CObject>.CopyTo(CObject[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        int ICollection<CObject>.Count
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<CObject>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        bool ICollection<CObject>.Remove(CObject item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<CObject> Members

        IEnumerator<CObject> IEnumerable<CObject>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        List<CObject> _items = new List<CObject>();
    }

    /// <summary>
    /// Represents the base class for numerical objects in a PDF content stream.
    /// </summary>
    public abstract class CNumber : CObject
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CNumber Clone()
        {
            return (CNumber)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        //internal override void WriteObject(ContentWriter writer)
        //{
        //  throw new Exception("Must not come here.");
        //}
    }

    /// <summary>
    /// Represents an integer value in a PDF content stream.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public class CInteger : CNumber
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CInteger Clone()
        {
            return (CInteger)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }
        int _value;

        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        public override string ToString()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(ToString() + " ");
        }
    }

    /// <summary>
    /// Represents a real value in a PDF content stream.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public class CReal : CNumber
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CReal Clone()
        {
            return (CReal)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }
        double _value;

        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        public override string ToString()
        {
            const string format = Config.SignificantFigures1Plus9;
            return _value.ToString(format, CultureInfo.InvariantCulture);
        }

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(ToString() + " ");
        }
    }

    /// <summary>
    /// Type of the parsed string.
    /// </summary>
    public enum CStringType
    {
        /// <summary>
        /// The string has the format "(...)".
        /// </summary>
        String,

        /// <summary>
        /// The string has the format "&lt;...&gt;".
        /// </summary>
        HexString,

        /// <summary>
        /// The string... TODO.
        /// </summary>
        UnicodeString,

        /// <summary>
        /// The string... TODO.
        /// </summary>
        UnicodeHexString,

        /// <summary>
        /// HACK: The string is the content of a dictionary.
        /// Currently there is no parser for dictionaries in Content Streams.
        /// </summary>
        Dictionary,
    }

    /// <summary>
    /// Represents a string value in a PDF content stream.
    /// </summary>
    [DebuggerDisplay("({Value})")]
    public class CString : CObject
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CString Clone()
        {
            return (CString)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        string _value;

        /// <summary>
        /// Gets or sets the type of the content string.
        /// </summary>
        public CStringType CStringType
        {
            get { return _cStringType; }
            set { _cStringType = value; }
        }
        CStringType _cStringType;

        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            switch (CStringType)
            {
                case CStringType.String:
                    s.Append("(");
                    int length = _value.Length;
                    for (int ich = 0; ich < length; ich++)
                    {
                        char ch = _value[ich];
                        switch (ch)
                        {
                            case Chars.LF:
                                s.Append("\\n");
                                break;

                            case Chars.CR:
                                s.Append("\\r");
                                break;

                            case Chars.HT:
                                s.Append("\\t");
                                break;

                            case Chars.BS:
                                s.Append("\\b");
                                break;

                            case Chars.FF:
                                s.Append("\\f");
                                break;

                            case Chars.ParenLeft:
                                s.Append("\\(");
                                break;

                            case Chars.ParenRight:
                                s.Append("\\)");
                                break;

                            case Chars.BackSlash:
                                s.Append("\\\\");
                                break;

                            default:
#if true_
                                // not absolut necessary to use octal encoding for characters less than blank
                                if (ch < ' ')
                                {
                                    s.Append("\\");
                                    s.Append((char)(((ch >> 6) & 7) + '0'));
                                    s.Append((char)(((ch >> 3) & 7) + '0'));
                                    s.Append((char)((ch & 7) + '0'));
                                }
                                else
#endif
                                s.Append(ch);
                                break;
                        }
                    }
                    s.Append(')');
                    break;


                case CStringType.HexString:
                    throw new NotImplementedException();
                //break;

                case CStringType.UnicodeString:
                    throw new NotImplementedException();
                //break;

                case CStringType.UnicodeHexString:
                    throw new NotImplementedException();
                //break;

                case CStringType.Dictionary:
                    s.Append(_value);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return s.ToString();
        }

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(ToString());
        }
    }

    /// <summary>
    /// Represents a name in a PDF content stream.
    /// </summary>
    [DebuggerDisplay("({Name})")]
    public class CName : CObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CName"/> class.
        /// </summary>
        public CName()
        {
            _name = "/";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CName"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CName(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CName Clone()
        {
            return (CName)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        /// <summary>
        /// Gets or sets the name. Names must start with a slash.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                if (String.IsNullOrEmpty(_name))
                    throw new ArgumentNullException("name");
                if (_name[0] != '/')
                    throw new ArgumentException(PSSR.NameMustStartWithSlash);
                _name = value;
            }
        }
        string _name;

        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        public override string ToString()
        {
            return _name;
        }

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(ToString() + " ");
        }
    }

    /// <summary>
    /// Represents an array of objects in a PDF content stream.
    /// </summary>
    [DebuggerDisplay("(count={Count})")]
    public class CArray : CSequence
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new CArray Clone()
        {
            return (CArray)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        /// <summary>
        /// Returns a string that represents the current value.
        /// </summary>
        public override string ToString()
        {
            return "[" + base.ToString() + "]";
        }

        internal override void WriteObject(ContentWriter writer)
        {
            writer.WriteRaw(ToString());
        }
    }

    /// <summary>
    /// Represents an operator a PDF content stream.
    /// </summary>
    [DebuggerDisplay("({Name}, operands={Operands.Count})")]
    public class COperator : CObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="COperator"/> class.
        /// </summary>
        protected COperator()
        { }

        internal COperator(OpCode opcode)
        {
            _opcode = opcode;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        public new COperator Clone()
        {
            return (COperator)Copy();
        }

        /// <summary>
        /// Implements the copy mechanism of this class.
        /// </summary>
        protected override CObject Copy()
        {
            CObject obj = base.Copy();
            return obj;
        }

        /// <summary>
        /// Gets or sets the name of the operator
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get { return _opcode.Name; }
        }

        /// <summary>
        /// Gets or sets the operands.
        /// </summary>
        /// <value>The operands.</value>
        public CSequence Operands
        {
            get { return _seqence ?? (_seqence = new CSequence()); }
        }
        CSequence _seqence;

        /// <summary>
        /// Gets the operator description for this instance.
        /// </summary>
        public OpCode OpCode
        {
            get { return _opcode; }
        }
        readonly OpCode _opcode;


        /// <summary>
        /// Returns a string that represents the current operator.
        /// </summary>
        public override string ToString()
        {
            if (_opcode.OpCodeName == OpCodeName.Dictionary)
                return " ";

            return Name;
        }

        internal override void WriteObject(ContentWriter writer)
        {
            int count = _seqence != null ? _seqence.Count : 0;
            for (int idx = 0; idx < count; idx++)
            {
                // ReSharper disable once PossibleNullReferenceException because the loop is not entered if _sequence is null
                _seqence[idx].WriteObject(writer);
            }
            writer.WriteLineRaw(ToString());
        }
    }
}
