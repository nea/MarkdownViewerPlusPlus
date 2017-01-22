using System;

namespace CommonMark.Syntax
{
    /// <summary>
    /// Represents a parsed inline element in the document.
    /// </summary>
    public sealed class Inline
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class.
        /// </summary>
        public Inline()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class.
        /// </summary>
        /// <param name="tag">The type of inline element.</param>
        public Inline(InlineTag tag)
        {
            this.Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class.
        /// </summary>
        /// <param name="tag">The type of inline element. Should be one of the types that require literal content, for example, <see cref="InlineTag.Code"/>.</param>
        /// <param name="content">The literal contents of the inline element.</param>
        public Inline(InlineTag tag, string content)
        {
            this.Tag = tag;
            this.LiteralContent = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class.
        /// </summary>
        internal Inline(InlineTag tag, string content, int startIndex, int length)
        {
            this.Tag = tag;
            this.LiteralContentValue.Source = content;
            this.LiteralContentValue.StartIndex = startIndex;
            this.LiteralContentValue.Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class. The element type is set to <see cref="InlineTag.String"/>
        /// </summary>
        /// <param name="content">The literal string contents of the inline element.</param>
        public Inline(string content)
        {
            // this is not assigned because it is the default value.
            ////this.Tag = InlineTag.String;

            this.LiteralContent = content;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class. The element type is set to <see cref="InlineTag.String"/>
        /// </summary>
        internal Inline(string content, int sourcePosition, int sourceLastPosition)
        {
            this.LiteralContent = content;
            this.SourcePosition = sourcePosition;
            this.SourceLastPosition = sourceLastPosition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class. The element type is set to <see cref="InlineTag.String"/>
        /// </summary>
        internal Inline(string content, int startIndex, int length, int sourcePosition, int sourceLastPosition, char delimiterCharacter)
        {
            this.LiteralContentValue.Source = content;
            this.LiteralContentValue.StartIndex = startIndex;
            this.LiteralContentValue.Length = length; 
            this.SourcePosition = sourcePosition;
            this.SourceLastPosition = sourceLastPosition;
            this.Emphasis = new EmphasisData(delimiterCharacter);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Inline"/> class.
        /// </summary>
        /// <param name="tag">The type of inline element. Should be one of the types that contain child elements, for example, <see cref="InlineTag.Emphasis"/>.</param>
        /// <param name="content">The first descendant element of the inline that is being created.</param>
        public Inline(InlineTag tag, Inline content)
        {
            this.Tag = tag;
            this.FirstChild = content;
        }

        internal static Inline CreateLink(Inline label, string url, string title)
        {
            return new Inline()
            {
                Tag = InlineTag.Link,
                FirstChild = label,
                TargetUrl = url,
                LiteralContent = title
            };
        }

        /// <summary>
        /// Gets of sets the type of the inline element this instance represents.
        /// </summary>
        public InlineTag Tag { get; set; }

        /// <summary>
        /// Gets or sets the literal content of this element. This is only used if the <see cref="Tag"/> property specifies
        /// a type that can have literal content.
        /// 
        /// Note that for <see cref="InlineTag.Link"/> this property contains the title of the link.
        /// </summary>
        public string LiteralContent 
        {
            get 
            {
                // since the .ToString() has been called once, cache the value.
                return this.LiteralContent = this.LiteralContentValue.ToString(); 
            }

            set 
            { 
                this.LiteralContentValue.Source = value; 
                this.LiteralContentValue.StartIndex = 0; 
                this.LiteralContentValue.Length = value == null ? 0 : value.Length;
            }
        }

        internal StringPart LiteralContentValue;

        /// <summary>
        /// Gets or sets the target URL for this element. Only used for <see cref="InlineTag.Link"/> and 
        /// <see cref="InlineTag.Image"/>.
        /// </summary>
        public string TargetUrl { get; set; }

        /// <summary>
        /// Gets or sets the first descendant of this element. This is only used if the <see cref="Tag"/> property specifies
        /// a type that can have nested elements. 
        /// </summary>
        public Inline FirstChild { get; set; }

        /// <summary>
        /// Gets or sets the position of the element within the source data.
        /// Note that if <see cref="CommonMarkSettings.TrackSourcePosition"/> is not enabled, this property will contain
        /// the position relative to the containing block and not the whole document (not accounting for processing done
        /// in earlier parser stage, such as converting tabs to spaces).
        /// </summary>
        /// <seealso cref="SourceLength"/>
        public int SourcePosition { get; set; }

        internal int SourceLastPosition { get; set; }

        /// <summary>
        /// Gets or sets the length of the element within the source data.
        /// Note that if <see cref="CommonMarkSettings.TrackSourcePosition"/> is not enabled, this property will contain
        /// the length within the containing block (not accounting for processing done in earlier parser stage, such as
        /// converting tabs to spaces).
        /// </summary>
        /// <seealso cref="SourcePosition"/>
        public int SourceLength 
        { 
            get { return this.SourceLastPosition - this.SourcePosition; }
            set { this.SourceLastPosition = this.SourcePosition + value; }
        }

        /// <summary>
        /// Gets the link details. This is now obsolete in favor of <see cref="TargetUrl"/> and <see cref="LiteralContent"/>
        /// properties and this property will be removed in future.
        /// </summary>
        [Obsolete("The link properties have been moved to TargetUrl and LiteralContent (previously Title) properties to reduce number of objects created. This property will be removed in future versions.", false)]
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public InlineContentLinkable Linkable { get { return new InlineContentLinkable() { Url = this.TargetUrl, Title = this.LiteralContent }; } }

        private Inline _next;

        /// <summary>
        /// Gets the next sibling inline element. Returns <see langword="null"/> if this is the last element.
        /// </summary>
        public Inline NextSibling
        {
            get { return this._next; }
            set { this._next = value; }
        }

        /// <summary>
        /// Gets the last sibling of this inline. If no siblings are defined, returns self.
        /// </summary>
        public Inline LastSibling
        {
            get
            {
                var x = this._next;
                if (x == null)
                    return this;

                while (x._next != null)
                    x = x._next;

                return x;
            }
        }

        /// <summary>
        /// Gets the additional properties that apply to emphasis elements.
        /// </summary>
        public EmphasisData Emphasis { get; }
    }
}
