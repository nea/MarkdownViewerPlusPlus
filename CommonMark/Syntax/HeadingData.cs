namespace CommonMark.Syntax
{
    /// <summary>
    /// Contains additional data for heading elements. Used in the <see cref="Block.Heading"/> property.
    /// </summary>
    public struct HeadingData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeadingData"/> structure.
        /// </summary>
        /// <param name="level">Heading level.</param>
        public HeadingData(int level)
        {
            Level = level <= byte.MaxValue ? (byte)level : byte.MaxValue;
        }

        /// <summary>
        /// Gets or sets the heading level.
        /// </summary>
        public byte Level { get; set; }
    }
}
