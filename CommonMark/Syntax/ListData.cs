using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for list block elements. Used in <see cref="Block.ListData"/> property.
    /// </summary>
    public sealed class ListData
    {
        /// <summary>
        /// Gets or sets the number of spaces the list markers are indented.
        /// </summary>
        public int MarkerOffset { get; set; }

        /// <summary>
        /// Gets or sets the position of the list item contents in the source text line.
        /// </summary>
        public int Padding { get; set; }

        /// <summary>
        /// Gets or sets the number for the first list item if <see cref="ListData.ListType"/> is set to
        /// <see cref="F:ListType.Ordered"/>.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the character used for unordered lists. Used if <see cref="ListData.ListType"/> is set to
        /// <see cref="F:ListType.Bullet"/>.
        /// </summary>
        public char BulletChar { get; set; }

        /// <summary>
        /// Gets or sets the type (ordered or unordered) of this list.
        /// </summary>
        public ListType ListType { get; set; }

        /// <summary>
        /// Gets or sets the character that follows the number if <see cref="ListData.ListType"/> is set to
        /// <see cref="F:ListType.Ordered"/>.
        /// </summary>
        public ListDelimiter Delimiter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the list is tight (such list will not render additional explicit
        /// paragraph elements).
        /// </summary>
        public bool IsTight { get; set; }
    }
}
