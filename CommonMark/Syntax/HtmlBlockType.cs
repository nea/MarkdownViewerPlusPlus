using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Specifies the type of the HTML block a <see cref="Block"/> instance represents.
    /// See http://spec.commonmark.org/0.22/#html-block
    /// </summary>
    public enum HtmlBlockType
    {
        /// <summary>
        /// This is not a HTML block.
        /// </summary>
        None = 0,

        /// <summary>
        /// The HTML block represents <c>script</c>, <c>pre</c> or <c>style</c> element. Unline other HTML tags
        /// these are allowed to contain blank lines.
        /// </summary>
        InterruptingBlockWithEmptyLines = 1,

        /// <summary>
        /// The block represents an HTML comment.
        /// </summary>
        Comment = 2,

        /// <summary>
        /// The block represents a processing instruction <c>&lt;??&gt;</c>
        /// </summary>
        ProcessingInstruction = 3,

        /// <summary>
        /// The block represents a doctype element <c>&lt;!...&gt;</c>
        /// </summary>
        DocumentType = 4,

        /// <summary>
        /// The block represents <c>&lt;![CDATA[...]]</c> element.
        /// </summary>
        CData = 5,

        /// <summary>
        /// This HTML block can interrupt paragraphs.
        /// </summary>
        InterruptingBlock = 6,

        /// <summary>
        /// This HTML block cannot interrupt paragraphs.
        /// </summary>
        NonInterruptingBlock = 7,
    }
}
