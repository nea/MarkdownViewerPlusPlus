using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CommonMark.Syntax;

namespace CommonMark.Formatters
{
    /// <summary>
    /// An extendable implementation for writing CommonMark data as HTML.
    /// </summary>
    public class HtmlFormatter
    {
        private readonly HtmlTextWriter _target;
        private readonly CommonMarkSettings _settings;
        private readonly Stack<bool> _renderTightParagraphs = new Stack<bool>(new[] { false });
        private readonly Stack<bool> _renderPlainTextInlines = new Stack<bool>(new[] { false });
        private readonly Stack<char> _endPlaceholders = new Stack<char>();

        /// <summary>
        /// Gets a stack of values indicating whether the paragraph tags should be ommitted.
        /// Every element that impacts this setting has to push a value when opening and pop it when closing.
        /// The most recent value is used to determine the current state.
        /// </summary>
        protected Stack<bool> RenderTightParagraphs { get { return _renderTightParagraphs; } }

        /// <summary>
        /// Gets a stack of values indicating whether the inline elements should be rendered as plain text
        /// (without formatting). This usually is done within image description attributes that do not support
        /// HTML tags.
        /// Every element that impacts this setting has to push a value when opening and pop it when closing.
        /// The most recent value is used to determine the current state.
        /// </summary>
        protected Stack<bool> RenderPlainTextInlines { get { return _renderPlainTextInlines; } }

        /// <summary>Initializes a new instance of the <see cref="HtmlFormatter" /> class.</summary>
        /// <param name="target">The target text writer.</param>
        /// <param name="settings">The settings used when formatting the data.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="target"/> is <see langword="null"/></exception>
        public HtmlFormatter(TextWriter target, CommonMarkSettings settings)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (settings == null)
                settings = CommonMarkSettings.Default;

            _target = new HtmlTextWriter(target);
            _settings = settings;
        }

        /// <summary>
        /// Gets the settings used for formatting data.
        /// </summary>
        protected CommonMarkSettings Settings { get { return _settings; } }

        /// <summary>
        /// Writes the given CommonMark document to the output stream as HTML.
        /// </summary>
        public void WriteDocument(Block document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            bool ignoreChildNodes;
            Block ignoreUntilBlockCloses = null;
            Inline ignoreUntilInlineCloses = null;

            foreach (var node in document.AsEnumerable())
            {
                if (node.Block != null)
                {
                    if (ignoreUntilBlockCloses != null)
                    {
                        if (ignoreUntilBlockCloses != node.Block)
                            continue;

                        ignoreUntilBlockCloses = null;
                    }

                    WriteBlock(node.Block, node.IsOpening, node.IsClosing, out ignoreChildNodes);
                    if (ignoreChildNodes && !node.IsClosing)
                        ignoreUntilBlockCloses = node.Block;
                }
                else if (ignoreUntilBlockCloses == null && node.Inline != null)
                {
                    if (ignoreUntilInlineCloses != null)
                    {
                        if (ignoreUntilInlineCloses != node.Inline)
                            continue;

                        ignoreUntilInlineCloses = null;
                    }

                    WriteInline(node.Inline, node.IsOpening, node.IsClosing, out ignoreChildNodes);
                    if (ignoreChildNodes && !node.IsClosing)
                        ignoreUntilInlineCloses = node.Inline;
                }
            }
        }

        /// <summary>
        /// Writes the specified block element to the output stream. Does not write the child nodes, instead
        /// the <paramref name="ignoreChildNodes"/> is used to notify the caller whether it should recurse
        /// into the child nodes.
        /// </summary>
        /// <param name="block">The block element to be written to the output stream.</param>
        /// <param name="isOpening">Specifies whether the block element is being opened (or started).</param>
        /// <param name="isClosing">Specifies whether the block element is being closed. If the block does not
        /// have child nodes, then both <paramref name="isClosing"/> and <paramref name="isOpening"/> can be
        /// <see langword="true"/> at the same time.</param>
        /// <param name="ignoreChildNodes">Instructs the caller whether to skip processing of child nodes or not.</param>
        protected virtual void WriteBlock(Block block, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            ignoreChildNodes = false;
            int x;

            switch (block.Tag)
            {
                case BlockTag.Document:
                    break;

                case BlockTag.Paragraph:
                    if (RenderTightParagraphs.Peek())
                        break;

                    if (isOpening)
                    {
                        EnsureNewLine();
                        Write("<p");
                        if (Settings.TrackSourcePosition) WritePositionAttribute(block);
                        Write('>');
                    }

                    if (isClosing)
                        WriteLine("</p>");

                    break;

                case BlockTag.BlockQuote:
                    if (isOpening)
                    {
                        EnsureNewLine();
                        Write("<blockquote");
                        if (Settings.TrackSourcePosition) WritePositionAttribute(block);
                        WriteLine(">");

                        RenderTightParagraphs.Push(false);
                    }

                    if (isClosing)
                    {
                        RenderTightParagraphs.Pop();
                        WriteLine("</blockquote>");
                    }

                    break;

                case BlockTag.ListItem:
                    if (isOpening)
                    {
                        EnsureNewLine();
                        Write("<li");
                        if (Settings.TrackSourcePosition) WritePositionAttribute(block);
                        Write('>');
                    }

                    if (isClosing)
                        WriteLine("</li>");

                    break;

                case BlockTag.List:
                    var data = block.ListData;

                    if (isOpening)
                    {
                        EnsureNewLine();
                        Write(data.ListType == ListType.Bullet ? "<ul" : "<ol");
                        if (data.Start != 1)
                        {
                            Write(" start=\"");
                            Write(data.Start.ToString(CultureInfo.InvariantCulture));
                            Write('\"');
                        }
                        if (Settings.TrackSourcePosition) WritePositionAttribute(block);
                        WriteLine(">");

                        RenderTightParagraphs.Push(data.IsTight);
                    }

                    if (isClosing)
                    {
                        WriteLine(data.ListType == ListType.Bullet ? "</ul>" : "</ol>");
                        RenderTightParagraphs.Pop();
                    }

                    break;

                case BlockTag.AtxHeading:
                case BlockTag.SetextHeading:

                    x = block.Heading.Level;
                    if (isOpening)
                    {
                        EnsureNewLine();

                        Write("<h" + x.ToString(CultureInfo.InvariantCulture));
                        if (Settings.TrackSourcePosition)
                            WritePositionAttribute(block);

                        Write('>');
                    }

                    if (isClosing)
                        WriteLine("</h" + x.ToString(CultureInfo.InvariantCulture) + ">");

                    break;

                case BlockTag.IndentedCode:
                case BlockTag.FencedCode:

                    ignoreChildNodes = true;

                    EnsureNewLine();
                    Write("<pre><code");
                    if (Settings.TrackSourcePosition) WritePositionAttribute(block);

                    var info = block.FencedCodeData == null ? null : block.FencedCodeData.Info;
                    if (info != null && info.Length > 0)
                    {
                        x = info.IndexOf(' ');
                        if (x == -1)
                            x = info.Length;

                        Write(" class=\"language-");
                        WriteEncodedHtml(new StringPart(info, 0, x));
                        Write('\"');
                    }
                    Write('>');
                    WriteEncodedHtml(block.StringContent);
                    WriteLine("</code></pre>");
                    break;

                case BlockTag.HtmlBlock:
                    ignoreChildNodes = true;
                    // cannot output source position for HTML blocks
                    Write(block.StringContent);

                    break;

                case BlockTag.ThematicBreak:
                    ignoreChildNodes = true;
                    if (Settings.TrackSourcePosition)
                    {
                        Write("<hr");
                        WritePositionAttribute(block);
                        WriteLine();
                    }
                    else
                    {
                        WriteLine("<hr />");
                    }

                    break;

                case BlockTag.ReferenceDefinition:
                    break;

                default:
                    throw new CommonMarkException("Block type " + block.Tag + " is not supported.", block);
            }

            if (ignoreChildNodes && !isClosing)
                throw new InvalidOperationException("Block of type " + block.Tag + " cannot contain child nodes.");
        }

        /// <summary>
        /// Writes the specified inline element to the output stream. Does not write the child nodes, instead
        /// the <paramref name="ignoreChildNodes"/> is used to notify the caller whether it should recurse
        /// into the child nodes.
        /// </summary>
        /// <param name="inline">The inline element to be written to the output stream.</param>
        /// <param name="isOpening">Specifies whether the inline element is being opened (or started).</param>
        /// <param name="isClosing">Specifies whether the inline element is being closed. If the inline does not
        /// have child nodes, then both <paramref name="isClosing"/> and <paramref name="isOpening"/> can be
        /// <see langword="true"/> at the same time.</param>
        /// <param name="ignoreChildNodes">Instructs the caller whether to skip processing of child nodes or not.</param>
        protected virtual void WriteInline(Inline inline, bool isOpening, bool isClosing, out bool ignoreChildNodes)
        {
            if (RenderPlainTextInlines.Peek())
            {
                switch (inline.Tag)
                {
                    case InlineTag.String:
                    case InlineTag.Code:
                    case InlineTag.RawHtml:
                        WriteEncodedHtml(inline.LiteralContentValue);
                        break;

                    case InlineTag.LineBreak:
                    case InlineTag.SoftBreak:
                        WriteLine();
                        break;

                    case InlineTag.Image:
                        if (isOpening)
                            RenderPlainTextInlines.Push(true);

                        if (isClosing)
                        {
                            RenderPlainTextInlines.Pop();

                            if (!RenderPlainTextInlines.Peek())
                                goto useFullRendering;
                        }

                        break;

                    case InlineTag.Link:
                    case InlineTag.Strong:
                    case InlineTag.Emphasis:
                    case InlineTag.Strikethrough:
                    case InlineTag.Placeholder:
                        break;

                    default:
                        throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
                }

                ignoreChildNodes = false;
                return;
            }

            useFullRendering:

            switch (inline.Tag)
            {
                case InlineTag.String:
                    ignoreChildNodes = true;
                    if (Settings.TrackSourcePosition)
                    {
                        Write("<span");
                        WritePositionAttribute(inline);
                        Write('>');
                        WriteEncodedHtml(inline.LiteralContentValue);
                        Write("</span>");
                    }
                    else
                    {
                        WriteEncodedHtml(inline.LiteralContentValue);
                    }

                    break;

                case InlineTag.LineBreak:
                    ignoreChildNodes = true;
                    WriteLine("<br />");
                    break;

                case InlineTag.SoftBreak:
                    ignoreChildNodes = true;
                    if (Settings.RenderSoftLineBreaksAsLineBreaks)
                        WriteLine("<br />");
                    else
                        WriteLine();
                    break;

                case InlineTag.Code:
                    ignoreChildNodes = true;
                    Write("<code");
                    if (Settings.TrackSourcePosition) WritePositionAttribute(inline);
                    Write('>');
                    WriteEncodedHtml(inline.LiteralContentValue);
                    Write("</code>");
                    break;

                case InlineTag.RawHtml:
                    ignoreChildNodes = true;
                    // cannot output source position for HTML blocks
                    Write(inline.LiteralContentValue);
                    break;

                case InlineTag.Link:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        Write("<a href=\"");
                        var uriResolver = Settings.UriResolver;
                        if (uriResolver != null)
                            WriteEncodedUrl(uriResolver(inline.TargetUrl));
                        else
                            WriteEncodedUrl(inline.TargetUrl);

                        Write('\"');
                        if (inline.LiteralContentValue.Length > 0)
                        {
                            Write(" title=\"");
                            WriteEncodedHtml(inline.LiteralContentValue);
                            Write('\"');
                        }

                        if (Settings.TrackSourcePosition) WritePositionAttribute(inline);

                        Write('>');
                    }

                    if (isClosing)
                    {
                        Write("</a>");
                    }

                    break;

                case InlineTag.Image:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        Write("<img src=\"");
                        var uriResolver = Settings.UriResolver;
                        if (uriResolver != null)
                            WriteEncodedUrl(uriResolver(inline.TargetUrl));
                        else
                            WriteEncodedUrl(inline.TargetUrl);

                        Write("\" alt=\"");

                        if (!isClosing)
                            RenderPlainTextInlines.Push(true);
                    }

                    if (isClosing)
                    {
                        // this.RenderPlainTextInlines.Pop() is done by the plain text renderer above.

                        Write('\"');
                        if (inline.LiteralContentValue.Length > 0)
                        {
                            Write(" title=\"");
                            WriteEncodedHtml(inline.LiteralContentValue);
                            Write('\"');
                        }

                        if (Settings.TrackSourcePosition) WritePositionAttribute(inline);
                        Write(" />");
                    }

                    break;

                case InlineTag.Strong:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        Write("<strong");
                        if (Settings.TrackSourcePosition) WritePositionAttribute(inline);
                        Write('>');
                    }

                    if (isClosing)
                    {
                        Write("</strong>");
                    }
                    break;

                case InlineTag.Emphasis:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        Write("<em");
                        if (Settings.TrackSourcePosition) WritePositionAttribute(inline);
                        Write('>');
                    }

                    if (isClosing)
                    {
                        Write("</em>");
                    }
                    break;

                case InlineTag.Strikethrough:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        Write("<del");
                        if (Settings.TrackSourcePosition) WritePositionAttribute(inline);
                        Write('>');
                    }

                    if (isClosing)
                    {
                        Write("</del>");
                    }
                    break;

                case InlineTag.Placeholder:
                    ignoreChildNodes = false;

                    if (isOpening)
                    {
                        string placeholderSubstitute = null;

                        try
                        {
                            placeholderSubstitute = (_placeholderResolver != null) ? _placeholderResolver(inline.TargetUrl) : null;
                        }
                        catch (Exception ex)
                        {
                            throw new CommonMarkException("An error occurred while resolving a placeholder.", ex);
                        }

                        if (placeholderSubstitute != null)
                        {
                            ignoreChildNodes = true;

                            if (Settings.TrackSourcePosition) WritePositionAttribute(inline);
                            Write(placeholderSubstitute);
                            _endPlaceholders.Push('\0');
                        }
                        else
                        {
                            ignoreChildNodes = false;
                            Write("[");
                            _endPlaceholders.Push(']');
                        }
                    }
                    if (isClosing)
                    {
                        var closingChar = _endPlaceholders.Pop();

                        if (closingChar != '\0')
                        {
                            Write(closingChar);
                        }
                    }
                    break;

                default:
                    throw new CommonMarkException("Inline type " + inline.Tag + " is not supported.", inline);
            }
        }

        /// <summary>
        /// Writes the specified text to the target writer.
        /// </summary>
        protected void Write(string text)
        {
            if (text == null)
                return;
            _target.Write(new StringPart(text, 0, text.Length));
        }

        private void Write(StringPart text)
        {
            _target.Write(text);
        }

        /// <summary>
        /// Writes the specified text to the target writer.
        /// </summary>
        protected void Write(StringContent text)
        {
            if (text == null)
                return;

            text.WriteTo(_target);
        }

        /// <summary>
        /// Writes the specified character to the target writer.
        /// </summary>
        protected void Write(char c)
        {
            _target.Write(c);
        }

        /// <summary>
        /// Ensures that the output ends with a newline. This means that newline character will be written
        /// only if the writer does not currently end with a newline.
        /// </summary>
        protected void EnsureNewLine()
        {
            _target.EnsureLine();
        }

        /// <summary>
        /// Writes a newline to the target writer.
        /// </summary>
        protected void WriteLine()
        {
            _target.WriteLine();
        }

        /// <summary>
        /// Writes the specified text and a newline to the target writer.
        /// </summary>
        protected void WriteLine(string text)
        {
            _target.Write(new StringPart(text, 0, text.Length));
            _target.WriteLine();
        }

        /// <summary>
        /// Encodes the given text with HTML encoding (ampersand-encoding) and writes the result to the target writer.
        /// </summary>
        protected void WriteEncodedHtml(StringContent text)
        {
            if (text == null)
                return;

            HtmlFormatterSlim.EscapeHtml(text, _target);
        }

        /// <summary>
        /// Encodes the given text with HTML encoding (ampersand-encoding) and writes the result to the target writer.
        /// </summary>
        protected void WriteEncodedHtml(string text)
        {
            if (text == null)
                return;

            HtmlFormatterSlim.EscapeHtml(new StringPart(text, 0, text.Length), _target);
        }

        private void WriteEncodedHtml(StringPart text)
        {
            HtmlFormatterSlim.EscapeHtml(text, _target);
        }

        /// <summary>
        /// Encodes the given text with URL encoding (percent-encoding) and writes the result to the target writer.
        /// Note that the result is intended to be written to HTML attribute so this also encodes <c>&amp;</c> character
        /// as <c>&amp;amp;</c>.
        /// </summary>
        protected void WriteEncodedUrl(string url)
        {
            HtmlFormatterSlim.EscapeUrl(url, _target);
        }

        /// <summary>
        /// Writes a <c>data-sourcepos="start-end"</c> attribute to the target writer. 
        /// This method should only be called if <see cref="CommonMarkSettings.TrackSourcePosition"/> is set to <see langword="true"/>.
        /// Note that the attribute is preceded (but not succeeded) by a single space.
        /// </summary>
        protected void WritePositionAttribute(Block block)
        {
            HtmlFormatterSlim.PrintPosition(_target, block);
        }

        /// <summary>
        /// Writes a <c>data-sourcepos="start-end"</c> attribute to the target writer. 
        /// This method should only be called if <see cref="CommonMarkSettings.TrackSourcePosition"/> is set to <see langword="true"/>.
        /// Note that the attribute is preceded (but not succeeded) by a single space.
        /// </summary>
        protected void WritePositionAttribute(Inline inline)
        {
            HtmlFormatterSlim.PrintPosition(_target, inline);
        }

        private Func<string, string> _placeholderResolver;

        /// <summary>
        /// Provides an optional function that can provide substitute strings for placeholders.
        /// The argument contains the placeholder text. If the function returns <see langword="null"/>,
        /// the placeholder was not resolved and will be rendered as a literal, otherwise, the
        /// returned string will be output instead of the placeholder.
        /// </summary>
        public Func<string, string> PlaceholderResolver
        {
            get
            {
                return _placeholderResolver;
            }
            set
            {
                _placeholderResolver = value;
            }
        }
    }
}
