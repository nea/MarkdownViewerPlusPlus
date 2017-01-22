namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for emphasis elements. Used in the <see cref="Inline.Emphasis"/> property.
    /// </summary>
    public struct EmphasisData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmphasisData"/> struccture.
        /// </summary>
        /// <param name="delimiterCharacter">Delimiter character.</param>
        public EmphasisData(char delimiterCharacter)
        {
            DelimiterCharacter = delimiterCharacter;
        }

        /// <summary>
        /// Gets or sets the delimiter character.
        /// </summary>
        public char DelimiterCharacter { get; set; }
    }
}
