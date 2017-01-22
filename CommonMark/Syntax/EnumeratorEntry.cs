using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Represents a single element in the document tree when traversing through it with the enumerator.
    /// </summary>
    /// <seealso cref="Syntax.Block.AsEnumerable"/>
    public sealed class EnumeratorEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratorEntry"/> class.
        /// </summary>
        /// <param name="opening">Specifies if this instance represents the opening of the block element.</param>
        /// <param name="closing">Specifies if this instance represents the closing of the block element (returned by the
        /// enumerator after the children have been enumerated). Both <paramref name="closing"/> and <paramref name="opening"/>
        /// can be specified at the same time if there are no children for the <paramref name="block"/> element.</param>
        /// <param name="block">The block element being returned from the enumerator.</param>
        public EnumeratorEntry(bool opening, bool closing, Block block)
        {
            this.IsOpening = opening;
            this.IsClosing = closing;
            this.Block = block;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumeratorEntry"/> class.
        /// </summary>
        /// <param name="opening">Specifies if this instance represents the opening of the inline element.</param>
        /// <param name="closing">Specifies if this instance represents the closing of the inline element (returned by the
        /// enumerator after the children have been enumerated). Both <paramref name="closing"/> and <paramref name="opening"/>
        /// can be specified at the same time if there are no children for the <paramref name="inline"/> element.</param>
        /// <param name="inline">The inlien element being returned from the enumerator.</param>
        public EnumeratorEntry(bool opening, bool closing, Inline inline)
        {
            this.IsOpening = opening;
            this.IsClosing = closing;
            this.Inline = inline;
        }

        /// <summary>
        /// Gets the value indicating whether this instance represents the opening of the element (returned before enumerating
        /// over the children of the element).
        /// </summary>
        public bool IsOpening { get; private set; }

        /// <summary>
        /// Gets the value indicating whether this instance represents the closing of the element (returned by the
        /// enumerator after the children have been enumerated). Both <see name="IsOpening"/> and <see name="IsClosing"/>
        /// can be <see langword="true"/> at the same time if there are no children for the given element.
        /// </summary>
        public bool IsClosing { get; private set; }

        /// <summary>
        /// Gets the inline element. Can be <see langword="null"/> if <see cref="Block"/> is set.
        /// </summary>
        public Inline Inline { get; private set; }

        /// <summary>
        /// Gets the block element. Can be <see langword="null"/> if <see cref="Inline"/> is set.
        /// </summary>
        public Block Block { get; private set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        public override string ToString()
        {
            string r;

            if (this.IsOpening && this.IsClosing)
                r = "Complete ";
            else if (this.IsOpening)
                r = "Opening ";
            else if (this.IsClosing)
                r = "Closing ";
            else
                r = "Invalid ";

            if (this.Block != null)
                r += "block " + this.Block.Tag.ToString();
            else if (this.Inline != null)
            {
                r += "inline " + this.Inline.Tag.ToString();

                if (this.Inline.Tag == InlineTag.String)
                {
                    if (this.Inline.LiteralContent == null)
                        r += ": <null>";
                    else if (this.Inline.LiteralContent.Length < 20)
                        r += ": " + this.Inline.LiteralContent;
                    else
                        r += ": " + this.Inline.LiteralContent.Substring(0, 19) + "…";
                }
            }
            else
                r += "empty";

            return r;
        }
    }
}