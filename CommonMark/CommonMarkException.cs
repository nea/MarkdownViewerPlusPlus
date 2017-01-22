using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark
{
    /// <summary>
    /// An exception that is caught during CommonMark parsing or formatting.
    /// </summary>
#if v2_0 || v3_5 || v4_0 || v4_5
    [Serializable]
#endif
    public class CommonMarkException : Exception
    {
        /// <summary>
        /// Gets the block that caused the exception. Can be <see langword="null"/>.
        /// </summary>
        public Block BlockElement { get; private set; }

        /// <summary>
        /// Gets the inline element that caused the exception. Can be <see langword="null"/>.
        /// </summary>
        public Inline InlineElement { get; private set; }

        /// <summary>Initializes a new instance of the <see cref="CommonMarkException" /> class.</summary>
        public CommonMarkException() { }

        /// <summary>Initializes a new instance of the <see cref="CommonMarkException" /> class with a specified error message.</summary>
        /// <param name="message">The message that describes the error.</param>
        public CommonMarkException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="CommonMarkException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
        public CommonMarkException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="CommonMarkException" /> class with a specified error message, a reference to the element that caused it and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="inline">The inline element that is related to the exception cause.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
        public CommonMarkException(string message, Inline inline, Exception innerException = null)
            : base(message, innerException)
        {
            this.InlineElement = inline;
        }

        /// <summary>Initializes a new instance of the <see cref="CommonMarkException" /> class with a specified error message, a reference to the element that caused it and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="block">The block element that is related to the exception cause.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or <see langword="null"/> if no inner exception is specified.</param>
        public CommonMarkException(string message, Block block, Exception innerException = null)
            : base(message, innerException) 
        {
            this.BlockElement = block;
        }

#if v2_0 || v3_5 || v4_0 || v4_5
        /// <summary>Initializes a new instance of the <see cref="CommonMarkException" /> class from the specified instances of the <see cref="System.Runtime.Serialization.SerializationInfo" /> and <see cref="System.Runtime.Serialization.StreamingContext" /> classes.</summary>
        /// <param name="serializationInfo">A <see cref="System.Runtime.Serialization.SerializationInfo" /> instance that contains the information required to deserialize the new <see cref="T:System.Security.Authentication.InvalidCredentialException" /> instance. </param>
        /// <param name="streamingContext">A <see cref="System.Runtime.Serialization.StreamingContext" /> instance. </param>
        [System.Security.SecuritySafeCritical]
        protected CommonMarkException(
          System.Runtime.Serialization.SerializationInfo serializationInfo,
          System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext) 
        {
            // Block and Inline classes are not marked [Serializable] and thus cannot be used here.
            // Currently there also aren't any good use cases where this would provide added value.
            ////this.BlockElement = (Block)info.GetValue("BlockElement", typeof(Block));
            ////this.InlineElement = (Inline)info.GetValue("InlineElement", typeof(Inline));
        }

        /// <summary>Sets the <see cref="System.Runtime.Serialization.SerializationInfo" /> with information about the exception.</summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="info" /> parameter is <see langword="null"/>.</exception>
        [System.Security.SecurityCritical]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            ////info.AddValue("BlockElement", this.BlockElement);
            ////info.AddValue("InlineElement", this.InlineElement);
        }
#endif
    }
}
