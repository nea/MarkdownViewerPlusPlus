using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Represents a parsed reference link definition.
    /// </summary>
    public sealed class Reference
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        public Reference()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class.
        /// </summary>
        public Reference(string label, string url, string title)
        {
            this.Label = label;
            this.Url = url;
            this.Title = title;
        }

        /// <summary>
        /// Represents the maximum allowed length of a reference definition (<c>foo</c> in <c>[foo]: /url</c>).
        /// </summary>
        public const int MaximumReferenceLabelLength = 1000;

        /// <summary>
        /// A special constant reference that represents an collapsed reference link: [foo][]
        /// </summary>
        internal static readonly Reference SelfReference = new Reference();

        /// <summary>
        /// A special constant reference that signifies that the reference label was not found: [foo][bar]
        /// </summary>
        internal static readonly Reference InvalidReference = new Reference();

        /// <summary>
        /// Gets or sets the label (the key by which it is referenced in the mapping) of the reference.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the URL of the reference.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the title of the reference (used in <c>&lt;a title="..."&gt;</c>).
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the current reference is a placeholder: [foo]
        /// </summary>
        public bool IsPlaceholder { get; set; }
    }
}
