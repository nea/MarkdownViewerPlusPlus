using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark
{
    /// <summary>
    /// Specifies different formatters supported by the converter.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// The output is standard HTML format according to the CommonMark specification.
        /// </summary>
        Html,

        /// <summary>
        /// The output is a debug view of the syntax tree. Usable for debugging.
        /// </summary>
        SyntaxTree,

        /// <summary>
        /// The output is written using a delegate function specified in <see cref="CommonMarkSettings.OutputDelegate"/>.
        /// </summary>
        CustomDelegate
    }
}
