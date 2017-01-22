using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Registers blocks of string data together so that it is not needed to concatenate multiple substrings
    /// together - thus reducing memory usage and number of string instances.
    /// </summary>
    public sealed class StringContent
    {
        private int _partCounter = 0;
        private int _length = 0;
        private int _partsLength = 2;
        private StringPart[] _parts = new StringPart[2];

        internal Parser.PositionTracker PositionTracker;

        /// <summary>
        /// Gets the total length of string data.
        /// </summary>
        public int Length
        {
            get { return this._length; }
        }

        private void RecalculateLength()
        {
            this._length = 0;
            for (var i = 0; i < this._partCounter; i++)
                this._length += this._parts[i].Length;
        }

        /// <summary>
        /// Appends a part of the given string data to this instance.
        /// </summary>
        /// <param name="source">The source string.</param>
        /// <param name="startIndex">The index of the first character that will be appended.</param>
        /// <param name="length">The length of the substring that will be appended.</param>
        public void Append(string source, int startIndex, int length)
        {
            if (startIndex > source.Length || length < 1)
                return;

            if (this._partCounter == this._partsLength)
            {
                this._partsLength += 10;
                Array.Resize(ref this._parts, this._partsLength);
            }

            this._parts[_partCounter++] = new StringPart() { Source = source, StartIndex = startIndex, Length = length };
            this._length += length;
        }
        
        /// <summary>
        /// Returns all of the data as a single string.
        /// </summary>
        /// <param name="buffer">A reusable instance of <see cref="StringBuilder"/>. Any existing content will be removed from it.</param>
        public string ToString(StringBuilder buffer)
        {
            if (this._partCounter == 0)
                return string.Empty;

            if (this._partCounter == 1)
                return this._parts[0].Source.Substring(this._parts[0].StartIndex, this._parts[0].Length);

            if (this._partCounter == 2)
            {
                return this._parts[0].Source.Substring(this._parts[0].StartIndex, this._parts[0].Length)
                     + this._parts[1].Source.Substring(this._parts[1].StartIndex, this._parts[1].Length);
            }

            buffer.Length = 0;

            for (var i = 0; i < this._partCounter; i++ )
            {
                buffer.Append(this._parts[i].Source, this._parts[i].StartIndex, this._parts[i].Length);
            }

            return buffer.ToString();
        }

        /// <summary>
        /// Returns all of the data as a single string.
        /// </summary>
        public override string ToString()
        {
            return this.ToString(new StringBuilder());
        }

        /// <summary>
        /// Resets the given subject instance with data from this string content.
        /// Note that this method calls <see cref="TrimEnd"/> thus changing the source data as well.
        /// </summary>
        /// <param name="subj">The subject instance which will be reinitialized with the data from this instance.</param>
        internal void FillSubject(Parser.Subject subj)
        {
            subj.LastInline = null;
            subj.LastPendingInline = null;
            subj.FirstPendingInline = null;

#if DEBUG
            subj.DebugStartIndex = 0;
#endif

            this.TrimEnd();

            if (this._partCounter == 0)
            {
                subj.Buffer = string.Empty;
                subj.Position = 0;
                subj.Length = 0;
                return;
            }

            if (this._partCounter > 1)
            {
                subj.Buffer = this.ToString(subj.ReusableStringBuilder);
                subj.Position = 0;
                subj.Length = subj.Buffer.Length;
                return;
            }

            subj.Buffer = this._parts[0].Source;
            subj.Position = this._parts[0].StartIndex;
            subj.Length = this._parts[0].StartIndex + this._parts[0].Length;
#if DEBUG
            subj.DebugStartIndex = this._parts[0].StartIndex;
#endif
        }

        /// <summary>
        /// Writes the data contained in this instance to the given text writer.
        /// </summary>
        public void WriteTo(System.IO.TextWriter writer)
        {
            for (var i = 0; i < this._partCounter; i++)
            {
#if OptimizeFor45
                writer.Write(this._parts[i].Source.ToCharArray(this._parts[i].StartIndex, this._parts[i].Length));
#else
                writer.Write(this._parts[i].Source.ToCharArray(), this._parts[i].StartIndex, this._parts[i].Length);
#endif
            }
        }

        /// <summary>
        /// Writes the data contained in this instance to the given html text writer.
        /// </summary>
        internal void WriteTo(Formatters.HtmlTextWriter writer)
        {
            var buffer = writer.Buffer;
            for (var i = 0; i < this._partCounter; i++)
            {
                if (buffer.Length < this._parts[i].Length)
                    buffer = writer.Buffer = new char[this._parts[i].Length];

                this._parts[i].Source.CopyTo(this._parts[i].StartIndex, buffer, 0, this._parts[i].Length);
                writer.Write(buffer, 0, this._parts[i].Length);
            }
        }

        /// <summary>
        /// Checks if the first character of the string content matches the given.
        /// </summary>
        public bool StartsWith(char character)
        {
            for (var i = 0; i < this._partCounter; i++)
            {
                if (this._parts[i].Length != 0)
                    return this._parts[i].Source[this._parts[i].StartIndex] == character;
            }

            return false;
        }

        /// <summary>
        /// Replaces the whole string content with the given substring.
        /// </summary>
        public void Replace(string data, int startIndex, int length)
        {
            this._partCounter = 1;
            this._parts[0].Source = data;
            this._parts[0].StartIndex = startIndex;
            this._parts[0].Length = length;
        }

        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified character in this instance.
        /// </summary>
        /// <returns>The zero-based index position of value if that character is found, or -1 if it is not.</returns>
        public int IndexOf(char character)
        {
            int res = -1;
            var index = 0;
            for (var i = 0; i < this._partCounter; i++)
            {
                res = this._parts[i].Source.IndexOf(character, this._parts[i].StartIndex, this._parts[i].Length);
                if (res != -1)
                {
                    res = res - this._parts[i].StartIndex + index;
                    break;
                }

                index += this._parts[i].Length;
            }

            return res;
        }

        /// <summary>
        /// Returns a substring starting at the beginning this instance with the given length.
        /// Optionally the returned characters are removed from this instance.
        /// </summary>
        /// <param name="length">The number of characters to return.</param>
        /// <param name="trim">If set to <see langword="true"/>, the characters are removed from this instance.</param>
        public string TakeFromStart(int length, bool trim = false)
        {
            // does not use StringBuilder because in most cases the substring will be taken from just the first part

            if (trim)
            {
                this._length -= length;
                if (this._length < 0)
                    this._length = 0;
            }

            string result = null;
            for (var i = 0; i < this._partCounter; i++)
            {
                if (length > this._parts[i].Length)
                {
                    result += this._parts[i].Source.Substring(this._parts[i].StartIndex, this._parts[i].Length);
                    length -= this._parts[i].Length;

                    if (trim)
                    {
                        this._parts[i].Length = 0;
                        this._parts[i].StartIndex = 0;
                        this._parts[i].Source = string.Empty;
                    }
                }
                else
                {
                    result += this._parts[i].Source.Substring(this._parts[i].StartIndex, length);

                    if (trim)
                    {
                        this._parts[i].Length -= length;
                        this._parts[i].StartIndex += length;
                    }

                    return result;
                }
            }

            throw new ArgumentOutOfRangeException(nameof(length), "The length of the substring cannot be greater than the length of the string.");
        }

        /// <summary>
        /// Removes any space or newline characters from the end of the string data.
        /// </summary>
        public void TrimEnd()
        {
            int pos, si;
            char c;
            string source;
            for (var i = this._partCounter - 1; i >= 0; i--)
            {
                source = this._parts[i].Source;
                si = this._parts[i].StartIndex;
                pos = si + this._parts[i].Length - 1;

                while (pos >= si)
                {
                    c = source[pos];
                    if (c != ' ' && c != '\n')
                    {
                        this._parts[i].Length = pos - si + 1;
                        return;
                    }

                    pos--;
                }

                this._length -= this._parts[i].Length;
                this._partCounter--;
            }
        }

        /// <summary>
        /// Removes any trailing blank lines.
        /// </summary>
        public void RemoveTrailingBlankLines()
        {
            int pos, si;
            int lastNewLinePos = -1;
            int lastNewLineIndex = -1;
            char c;
            string source;
            for (var i = this._partCounter - 1; i >= 0; i--)
            {
                source = this._parts[i].Source;
                si = this._parts[i].StartIndex;
                pos = si + this._parts[i].Length - 1;

                while (pos >= si)
                {
                    c = source[pos];

                    if (c == '\n')
                    {
                        lastNewLinePos = pos;
                        lastNewLineIndex = i;
                    }
                    else if (c != ' ')
                    {
                        if (lastNewLinePos == -1)
                            return;

                        // 0123456789012345
                        //     --n---          s = 4, len = 6, pos = 6
                        //     --n             s = 4, len = 3=(pos-s+1)

                        this._partCounter = lastNewLineIndex + 1;
                        this._parts[lastNewLineIndex].Length = lastNewLinePos - this._parts[lastNewLineIndex].StartIndex + 1;

                        this.RecalculateLength();
                        return;
                    }

                    pos--;
                }
            }
        }

        internal ArraySegment<StringPart> RetrieveParts()
        {
            return new ArraySegment<StringPart>(this._parts, 0, this._partCounter);
        }
    }
}
