using System.Collections.Generic;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for document elements. Used in the <see cref="Block.Document"/> property.
    /// </summary>
    public class DocumentData
    {
        /// <summary>
        /// Gets or sets the dictionary containing resolved link references. Only set on the document node, <see langword="null"/>
        /// and not used for all other elements.
        /// </summary>
        public Dictionary<string, Reference> ReferenceMap { get; set; }
    }
}
