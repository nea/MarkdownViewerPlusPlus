using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for fenced code blocks. Used in <see cref="Block.FencedCodeData"/>/
    /// </summary>
    public sealed class FencedCodeData
    {
        /// <summary>
        /// Gets or sets the number of characters that were used in the opening code fence.
        /// </summary>
        public int FenceLength { get; set; }

        /// <summary>
        /// Gets or sets the number of spaces the opening fence was indented.
        /// </summary>
        public int FenceOffset { get; set; }

        /// <summary>
        /// Gets or sets the character that is used in the fences.
        /// </summary>
        public char FenceChar { get; set; }

        /// <summary>
        /// Gets or sets the additional information that was present in the same line as the opening fence.
        /// </summary>
        public string Info { get; set; }
    }
}
