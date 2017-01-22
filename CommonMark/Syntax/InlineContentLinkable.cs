using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// An obsolete class. Used to contain properties specific to link inline elements.
    /// </summary>
    [Obsolete("These properties have been moved directly into the Inline element.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class InlineContentLinkable
    {
        /// <summary>
        /// Gets or sets the URL of a link. Moved to <see cref="Inline.TargetUrl"/>.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title of a link. Moved to <see cref="Inline.LiteralContent"/>.
        /// </summary>
        public string Title { get; set; }
    }
}
