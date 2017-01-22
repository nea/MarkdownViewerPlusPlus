using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Defines the type of a list block element.
    /// </summary>
    public enum ListType
    {
        /// <summary>
        /// The list is unordered and its items are represented with bullets.
        /// </summary>
        Bullet = 0,

        /// <summary>
        /// The list is ordered and its items are numbered.
        /// </summary>
        Ordered
    }
}
