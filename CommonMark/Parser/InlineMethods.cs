using CommonMark.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal static class InlineMethods
    {
        private static readonly char[] WhiteSpaceCharacters = new[] { '\n', ' ' };
        private static readonly char[] BracketSpecialCharacters = new[] { '\\', ']', '[' };

        /// <summary>
        /// Initializes the array of delegates for inline parsing.
        /// </summary>
        /// <returns></returns>
        internal static Func<Subject, Inline>[] InitializeParsers(CommonMarkSettings settings)
        {
            var strikethroughTilde = 0 != (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.StrikethroughTilde);
            var placeholderBracket = 0 != (settings.AdditionalFeatures & CommonMarkAdditionalFeatures.PlaceholderBracket);

            var p = new Func<Subject, Inline>[strikethroughTilde ? 127 : 97];
            p['\n'] = handle_newline;
            p['`'] = handle_backticks;
            p['\\'] = handle_backslash;
            p['&'] = HandleEntity;
            p['<'] = handle_pointy_brace;
            p['_'] = HandleEmphasis;
            p['*'] = HandleEmphasis;
            p['['] = HandleLeftSquareBracket;

            if (placeholderBracket)
                p[']'] = subj => HandleRightSquareBracket(subj, true);
            else
                p[']'] = subj => HandleRightSquareBracket(subj, false);
            
            p['!'] = HandleExclamation;

            if (strikethroughTilde)
                p['~'] = HandleTilde;

            return p;
        }


        /// <summary>
        /// Collapses internal whitespace to single space, removes leading/trailing whitespace, folds case.
        /// </summary>
        private static string NormalizeReference(StringPart s)
        {
            if (s.Length == 0)
                return string.Empty;

            return NormalizeWhitespace(s.Source, s.StartIndex, s.Length).ToUpperInvariant();
        }

        /// <summary>
        /// Checks if the reference dictionary contains a reference with the given label and returns it,
        /// otherwise returns <see langword="null"/>.
        /// Returns <see cref="Reference.InvalidReference"/> if the reference label is not valid.
        /// </summary>
        private static Reference LookupReference(DocumentData data, StringPart lab)
        {
            if (data?.ReferenceMap == null)
                return null;

            if (lab.Length > Reference.MaximumReferenceLabelLength)
                return Reference.InvalidReference;

            string label = NormalizeReference(lab);

            Reference r;
            if (data.ReferenceMap.TryGetValue(label, out r))
                return r;

            return null;
        }

        /// <summary>
        /// Adds a new reference to the dictionary, if the label does not already exist there.
        /// Assumes that the length of the label does not exceed <see cref="Reference.MaximumReferenceLabelLength"/>.
        /// </summary>
        private static void AddReference(Dictionary<string, Reference> refmap, StringPart label, string url, string title)
        {
            var normalizedLabel = NormalizeReference(label);
            if (refmap.ContainsKey(normalizedLabel))
                return;

            refmap.Add(normalizedLabel, new Reference(normalizedLabel, url, title));
        }

        // Return the next character in the subject, without advancing.
        // Return 0 if at the end of the subject.
#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
        private static char peek_char(Subject subj)
        {
            return subj.Length <= subj.Position ? '\0' : subj.Buffer[subj.Position];
        }

        /// <summary>
        /// Searches the subject for a span of backticks that matches the given length.
        /// Returns <c>0</c> if the closing backticks cannot be found, otherwise returns
        /// the position in the subject after the closing backticks.
        /// Also updates the position on the subject itself.
        /// </summary>
        private static int ScanToClosingBackticks(Subject subj, int openticklength)
        {
            // note - attempt to optimize by using string.IndexOf("````",...) proved to
            // be ~2x times slower than the current implementation.
            // but - buf.IndexOf('`') gives ~1.5x better performance than iterating over
            // every char in the loop.

            var buf = subj.Buffer;
            var len = buf.Length;
            var cc = 0;

            for (var i = subj.Position; i < len; i++)
            {
                if (buf[i] == '`')
                {
                    cc++;
                }
                else
                {
                    if (cc == openticklength)
                        return subj.Position = i;

                    i = buf.IndexOf('`', i, len - i) - 1;
                    if (i == -2)
                        return 0;

                    cc = 0;
                }
            }

            if (cc == openticklength)
                return subj.Position = len;

            return 0;
        }

        /// <summary>
        /// Collapses consecutive space and newline characters into a single space.
        /// Additionaly removes leading and trailing spaces.
        /// </summary>
        internal static string NormalizeWhitespace(string s, int startIndex, int count)
        {
            char c;

            // count will actually be the lastIndex. The method argument is count only because other similar methods have startIndex/count
            count = startIndex + count - 1;

            // trim leading and trailing spaces.
            while (startIndex < count)
            {
                c = s[startIndex];
                if (c != ' ' && c != '\n') break;
                startIndex++;
            }

            while (count >= startIndex)
            {
                c = s[count];
                if (c != ' ' && c != '\n') break;
                count--;
            }

            if (count < startIndex)
                return string.Empty;

            // collapse inner whitespace
            // the complexity of this method is mainly so that the use of StringBuilder could be avoided if it is not needed
            StringBuilder sb = null;
            int pos = startIndex;
            int lastPos = startIndex;
            while (-1 != (pos = s.IndexOfAny(WhiteSpaceCharacters, pos, count - pos)))
            {
                if (s[pos] == '\n')
                {
                    if (sb == null)
                        sb = new StringBuilder(s.Length);

                    // newline has to be replaced with ' '
                    sb.Append(s, lastPos, pos - lastPos);
                    sb.Append(' ');

                    // move past consecutive spaces
                    do
                    {
                        c = s[++pos];
                        if (c != ' ' && c != '\n')
                            break;
                    } while (pos < count);

                    lastPos = pos;
                }
                else
                {
                    c = s[++pos];

                    if (c == ' ' || c == '\n')
                    {
                        // multiple consecutive whitespaces
                        if (sb == null)
                            sb = new StringBuilder(s.Length);

                        sb.Append(s, lastPos, pos - lastPos);

                        // move past consecutive spaces
                        do
                        {
                            c = s[++pos];
                            if (c != ' ' && c != '\n')
                                break;
                        } while (pos < count);

                        lastPos = pos;
                    }
                }
            }

            if (sb == null)
                return s.Substring(startIndex, count - startIndex + 1);

            sb.Append(s, lastPos, count - lastPos + 1);
            return sb.ToString();
        }

        // Parse backtick code section or raw backticks, return an inline.
        // Assumes that the subject has a backtick at the current position.
        static Inline handle_backticks(Subject subj)
        {
            int ticklength = 0;
            var bl = subj.Length;
            while (subj.Position < bl && (subj.Buffer[subj.Position] == '`'))
            {
                ticklength++;
                subj.Position++;
            }

            int startpos = subj.Position;
            int endpos = ScanToClosingBackticks(subj, ticklength);
            if (endpos == 0)
            {
                // closing not found
                subj.Position = startpos; // rewind to right after the opening ticks
                return new Inline(new string('`', ticklength), startpos - ticklength, startpos);
            }
            else
            {
                return new Inline(InlineTag.Code, NormalizeWhitespace(subj.Buffer, startpos, endpos - startpos - ticklength))
                    {
                        SourcePosition = startpos - ticklength,
                        SourceLastPosition = endpos
                    };
            }
        }

        /// <summary>
        /// Scans the subject for a series of the given emphasis character, testing if they could open and/or close
        /// an emphasis element.
        /// </summary>
        private static int ScanEmphasisDelimiters(Subject subj, char delimiter, out bool canOpen, out bool canClose)
        {
            int numdelims = 0;
            int startpos = subj.Position;
            int len = subj.Length;

            while (startpos + numdelims < len && subj.Buffer[startpos + numdelims] == delimiter)
                numdelims++;

            if (numdelims == 0)
            {
                canOpen = false;
                canClose = false;
                return numdelims;
            }

            char charBefore, charAfter;
            bool beforeIsSpace, beforeIsPunctuation, afterIsSpace, afterIsPunctuation;

            charBefore = startpos == 0 ? '\n' : subj.Buffer[startpos - 1];
            subj.Position = (startpos += numdelims);
            charAfter = len == startpos ? '\n' : subj.Buffer[startpos];
            
            Utilities.CheckUnicodeCategory(charBefore, out beforeIsSpace, out beforeIsPunctuation);
            Utilities.CheckUnicodeCategory(charAfter, out afterIsSpace, out afterIsPunctuation);

            canOpen = !afterIsSpace && !(afterIsPunctuation && !beforeIsSpace && !beforeIsPunctuation);
            canClose = !beforeIsSpace && !(beforeIsPunctuation && !afterIsSpace && !afterIsPunctuation);

            if (delimiter == '_')
            {
                var temp = canOpen;
                canOpen &= (!canClose || beforeIsPunctuation);
                canClose &= (!temp || afterIsPunctuation);
            }

            return numdelims;
        }

        internal static int MatchInlineStack(InlineStack opener, Subject subj, int closingDelimiterCount, InlineStack closer, InlineTag singleCharTag, InlineTag doubleCharTag)
        {
            // calculate the actual number of delimiters used from this closer
            int useDelims;
            var openerDelims = opener.DelimiterCount;

            if (closingDelimiterCount < 3 || openerDelims < 3)
            {
                useDelims = closingDelimiterCount <= openerDelims ? closingDelimiterCount : openerDelims;
                if (useDelims == 1 && singleCharTag == 0)
                    return 0;
            }
            else if (singleCharTag == 0)
                useDelims = 2;
            else if (doubleCharTag == 0)
                useDelims = 1;
            else
                useDelims = closingDelimiterCount % 2 == 0 ? 2 : 1;

            Inline inl = opener.StartingInline;
            InlineTag tag = useDelims == 1 ? singleCharTag : doubleCharTag;
            if (openerDelims == useDelims)
            {
                // the opener is completely used up - remove the stack entry and reuse the inline element
                inl.Tag = tag;
                inl.LiteralContent = null;
                inl.FirstChild = inl.NextSibling;
                inl.NextSibling = null;

                InlineStack.RemoveStackEntry(opener, subj, closer?.Previous);
            }
            else
            {
                // the opener will only partially be used - stack entry remains (truncated) and a new inline is added.
                opener.DelimiterCount -= useDelims;
                inl.LiteralContent = inl.LiteralContent.Substring(0, opener.DelimiterCount);
                inl.SourceLastPosition -= useDelims;

                inl.NextSibling = new Inline(tag, inl.NextSibling);
                inl = inl.NextSibling;

                inl.SourcePosition = opener.StartingInline.SourcePosition + opener.DelimiterCount;
            }

            // there are two callers for this method, distinguished by the `closer` argument.
            // if closer == null it means the method is called during the initial subject parsing and the closer
            //   characters are at the current position in the subject. The main benefit is that there is nothing
            //   parsed that is located after the matched inline element.
            // if closer != null it means the method is called when the second pass for previously unmatched
            //   stack elements is done. The drawback is that there can be other elements after the closer.
            if (closer != null)
            {
                var clInl = closer.StartingInline;
                if ((closer.DelimiterCount -= useDelims) > 0)
                {
                    // a new inline element must be created because the old one has to be the one that
                    // finalizes the children of the emphasis
                    var newCloserInline = new Inline(clInl.LiteralContent.Substring(useDelims));
                    newCloserInline.SourcePosition = inl.SourceLastPosition = clInl.SourcePosition + useDelims;
                    newCloserInline.SourceLength = closer.DelimiterCount;
                    newCloserInline.NextSibling = clInl.NextSibling;

                    clInl.LiteralContent = null;
                    clInl.NextSibling = null;
                    inl.NextSibling = closer.StartingInline = newCloserInline;
                }
                else
                {
                    inl.SourceLastPosition = clInl.SourceLastPosition;

                    clInl.LiteralContent = null;
                    inl.NextSibling = clInl.NextSibling;
                    clInl.NextSibling = null;
                }
            }
            else if (subj != null)
            {
                inl.SourceLastPosition = subj.Position - closingDelimiterCount + useDelims;
                subj.LastInline = inl;
            }

            return useDelims;
        }

        private static Inline HandleEmphasis(Subject subj)
        {
            return HandleOpenerCloser(subj, InlineTag.Emphasis, InlineTag.Strong);
        }

        private static Inline HandleTilde(Subject subj)
        {
            return HandleOpenerCloser(subj, (InlineTag)0, InlineTag.Strikethrough);
        }

        private static Inline HandleOpenerCloser(Subject subj, InlineTag singleCharTag, InlineTag doubleCharTag)
        {
            bool canOpen, canClose;
            var c = subj.Buffer[subj.Position];
            var numdelims = ScanEmphasisDelimiters(subj, c, out canOpen, out canClose);

            if (canClose)
            {
                // walk the stack and find a matching opener, if there is one
                var istack = InlineStack.FindMatchingOpener(subj.LastPendingInline, InlineStack.InlineStackPriority.Emphasis, c, numdelims, canOpen, out canClose);
                if (istack != null)
                {
                    var useDelims = MatchInlineStack(istack, subj, numdelims, null, singleCharTag, doubleCharTag);

                    if (useDelims > 0)
                    {
                        // if the closer was not fully used, move back a char or two and try again.
                        if (useDelims < numdelims)
                        {
                            subj.Position = subj.Position - numdelims + useDelims;

                            // use recursion only if it will not be very deep.
                            // however it cannot be used if the single char will not be parsed.
                            if (numdelims < 10)
                                return HandleOpenerCloser(subj, singleCharTag, doubleCharTag);
                        }

                        return null;
                    }
                }
            }

            var inlText = new Inline(subj.Buffer, subj.Position - numdelims, numdelims, subj.Position - numdelims, subj.Position, c);

            if (canOpen || canClose)
            {
                var istack = new InlineStack();
                istack.DelimiterCount = numdelims;
                istack.Delimiter = c;
                istack.StartingInline = inlText;
                istack.Priority = InlineStack.InlineStackPriority.Emphasis;
                istack.Flags = (canOpen ? InlineStack.InlineStackFlags.Opener : 0)
                             | (canClose ? (InlineStack.InlineStackFlags.Closer | InlineStack.InlineStackFlags.CloserOriginally) : 0);

                InlineStack.AppendStackEntry(istack, subj);
            }

            return inlText;
        }

        private static Inline HandleExclamation(Subject subj)
        {
            subj.Position++;
            if (peek_char(subj) == '[')
                return HandleLeftSquareBracket(subj, true);
            else
                return new Inline("!", subj.Position - 1, subj.Position);
        }

        private static Inline HandleLeftSquareBracket(Subject subj)
        {
            return HandleLeftSquareBracket(subj, false);
        }

        private static Inline HandleLeftSquareBracket(Subject subj, bool isImage)
        {
            Inline inlText;
            
            if (isImage)
            {
                inlText = new Inline("![", subj.Position - 1, subj.Position + 1);
            }
            else
            {
                inlText = new Inline("[", subj.Position, subj.Position + 1);
            }

            // move past the '['
            subj.Position++;

            var istack = new InlineStack();
            istack.Delimiter = '[';
            istack.StartingInline = inlText;
            istack.StartPosition = subj.Position;
            istack.Priority = InlineStack.InlineStackPriority.Links;
            istack.Flags = InlineStack.InlineStackFlags.Opener | (isImage ? InlineStack.InlineStackFlags.ImageLink : InlineStack.InlineStackFlags.None);

            InlineStack.AppendStackEntry(istack, subj);

            return inlText;
        }

        internal static void MatchSquareBracketStack(InlineStack opener, Subject subj, Reference details)
        {
            if (details != null)
            {
                var inl = opener.StartingInline;
                var isImage = 0 != (opener.Flags & InlineStack.InlineStackFlags.ImageLink);
                inl.Tag = isImage ? InlineTag.Image : (details.IsPlaceholder ? InlineTag.Placeholder : InlineTag.Link);
                inl.FirstChild = inl.NextSibling;
                inl.NextSibling = null;
                inl.SourceLastPosition = subj.Position;

                inl.TargetUrl = details.Url;
                inl.LiteralContent = details.Title;

                if (!isImage && !details.IsPlaceholder)
                {
                    // since there cannot be nested links, remove any other link openers before this
                    var temp = opener.Previous;
                    while (temp != null && temp.Priority <= InlineStack.InlineStackPriority.Links)
                    {
                        if (temp.Delimiter == '[' && temp.Flags == opener.Flags)
                        {
                            // mark the previous entries as "inactive"
                            if (temp.DelimiterCount == -1)
                                break;

                            temp.DelimiterCount = -1;
                        }

                        temp = temp.Previous;
                    }
                }

                InlineStack.RemoveStackEntry(opener, subj, null);

                subj.LastInline = inl;
            }
            else
            {
                // this looked like a link, but was not.
                // remove the opener stack entry but leave the inbetween intact
                InlineStack.RemoveStackEntry(opener, subj, opener);

                var inl = new Inline("]", subj.Position - 1, subj.Position);
                subj.LastInline.LastSibling.NextSibling = inl;
                subj.LastInline = inl;
            }
        }

        private static Inline HandleRightSquareBracket(Subject subj, bool supportPlaceholderBrackets)
        {
            // move past ']'
            subj.Position++;

            bool canClose;
            var istack = InlineStack.FindMatchingOpener(subj.LastPendingInline, InlineStack.InlineStackPriority.Links, '[', 1, false, out canClose);

            if (istack != null)
            {
                // if the opener is "inactive" then it means that there was a nested link
                if (istack.DelimiterCount == -1)
                {
                    InlineStack.RemoveStackEntry(istack, subj, istack);
                    return new Inline("]", subj.Position - 1, subj.Position);
                }

                var endpos = subj.Position;

                // try parsing details for '[foo](/url "title")' or '[foo][bar]'
                //var details = ParseLinkDetails(subj);
                var details = ParseLinkDetails(subj, supportPlaceholderBrackets);

                // try lookup of the brackets themselves
                if (details == null || details == Reference.SelfReference)
                {
                    var startpos = istack.StartPosition;
                    var label = new StringPart(subj.Buffer, startpos, endpos - startpos - 1);

                    details = LookupReference(subj.DocumentData, label);
                }

                if (details == Reference.InvalidReference)
                    details = null;

                MatchSquareBracketStack(istack, subj, details);
                return null;
            }

            var inlText = new Inline("]", subj.Position - 1, subj.Position);

            if (canClose)
            {
                // note that the current implementation will not work if there are other inlines with priority
                // higher than Links.
                // to fix this the parsed link details should be added to the closer element in the stack.

                throw new NotSupportedException("It is not supported to have inline stack priority higher than Links.");

                ////istack = new InlineStack();
                ////istack.Delimiter = '[';
                ////istack.StartingInline = inlText;
                ////istack.StartPosition = subj.Position;
                ////istack.Priority = InlineStack.InlineStackPriority.Links;
                ////istack.Flags = InlineStack.InlineStackFlags.Closer;

                ////InlineStack.AppendStackEntry(istack, subj);
            }

            return inlText;
        }

        // Parse backslash-escape or just a backslash, returning an inline.
        private static Inline handle_backslash(Subject subj)
        {
            subj.Position++;

            if (subj.Position >= subj.Length)
                return new Inline("\\", subj.Position - 1, subj.Position); 

            var nextChar = subj.Buffer[subj.Position];

            if (Utilities.IsEscapableSymbol(nextChar))
            {
                // only ascii symbols and newline can be escaped
                // the exception is the unicode bullet char since it can be used for defining list items
                subj.Position++;
                return new Inline(nextChar.ToString(), subj.Position - 2, subj.Position);
            }
            else if (nextChar == '\n')
            {
                subj.Position++;
                return new Inline(InlineTag.LineBreak) 
                {
                    SourcePosition = subj.Position - 2,
                    SourceLastPosition = subj.Position
                };
            }
            else
            {
                return new Inline("\\", subj.Position - 1, subj.Position);
            }
        }

        /// <summary>
        /// Parses the entity at the current position. Returns a new string inline.
        /// Assumes that there is a <c>&amp;</c> at the current position.
        /// </summary>
        private static Inline HandleEntity(Subject subj)
        {
            var origPos = subj.Position;
            return new Inline(ParseEntity(subj), origPos, subj.Position);
        }

        /// <summary>
        /// Parses the entity at the current position.
        /// Assumes that there is a <c>&amp;</c> at the current position.
        /// </summary>
        private static string ParseEntity(Subject subj)
        {
            int match;
            string entity;
            int numericEntity;
            var origPos = subj.Position;
            match = Scanner.scan_entity(subj.Buffer, subj.Position, subj.Length - subj.Position, out entity, out numericEntity);
            if (match > 0)
            {
                subj.Position += match;

                if (entity != null)
                {
                    entity = EntityDecoder.DecodeEntity(entity);
                    if (entity != null)
                        return entity;

                    return subj.Buffer.Substring(origPos, match);
                }
                else if (numericEntity > 0)
                {
                    entity = EntityDecoder.DecodeEntity(numericEntity);
                    if (entity != null)
                        return entity;
                }

                return "\uFFFD";
            }
            else
            {
                subj.Position++;
                return "&";
            }
        }

        /// <summary>
        /// Creates a new <see cref="Inline"/> element that represents string content but the given content
        /// is processed to decode any HTML entities in it.
        /// This method is guaranteed to return just one Inline, without nested elements.
        /// </summary>
        private static Inline ParseStringEntities(string s)
        {
            string result = null;
            StringBuilder builder = null;

            int searchpos;
            char c;
            var subj = new Subject(s, null);

            while ('\0' != (c = peek_char(subj)))
            {
                if (result != null)
                {
                    if (builder == null)
                        builder = new StringBuilder(result, s.Length);
                    else
                        builder.Append(result);
                }

                if (c == '&')
                {
                    result = ParseEntity(subj);
                }
                else
                {
                    searchpos = subj.Buffer.IndexOf('&', subj.Position);
                    if (searchpos == -1)
                        searchpos = subj.Length;

                    result = subj.Buffer.Substring(subj.Position, searchpos - subj.Position);
                    subj.Position = searchpos;
                }
            }

            if (builder == null)
                return new Inline(result);

            builder.Append(result);

            return new Inline(builder.ToString());
        }

        /// <summary>
        /// Destructively unescape a string: remove backslashes before punctuation or symbol characters.
        /// </summary>
        /// <param name="url">The string data that will be changed by unescaping any punctuation or symbol characters.</param>
        public static string Unescape(string url)
        {
            // remove backslashes before punctuation chars:
            int searchPos = 0;
            int lastPos = 0;
            int match;
            char c;
            char[] search = new[] { '\\', '&' };
            StringBuilder sb = null;

            while ((searchPos = url.IndexOfAny(search, searchPos)) != -1)
            {
                c = url[searchPos];
                if (c == '\\')
                {
                    searchPos++;

                    if (url.Length == searchPos)
                        break;

                    c = url[searchPos];
                    if (Utilities.IsEscapableSymbol(c))
                    {
                        if (sb == null) sb = new StringBuilder(url.Length);
                        sb.Append(url, lastPos, searchPos - lastPos - 1);
                        lastPos = searchPos;
                    }
                }
                else if (c == '&')
                {
                    string namedEntity;
                    int numericEntity;
                    match = Scanner.scan_entity(url, searchPos, url.Length - searchPos, out namedEntity, out numericEntity);
                    if (match == 0)
                    {
                        searchPos++;
                    }
                    else
                    {
                        searchPos += match;

                        if (namedEntity != null)
                        {
                            var decoded = EntityDecoder.DecodeEntity(namedEntity);
                            if (decoded != null)
                            {
                                if (sb == null) sb = new StringBuilder(url.Length);
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                                lastPos = searchPos;
                            }
                        }
                        else if (numericEntity > 0)
                        {
                            var decoded = EntityDecoder.DecodeEntity(numericEntity);
                            if (decoded != null)
                            {
                                if (sb == null) sb = new StringBuilder(url.Length);
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append(decoded);
                            }
                            else
                            {
                                if (sb == null) sb = new StringBuilder(url.Length);
                                sb.Append(url, lastPos, searchPos - match - lastPos);
                                sb.Append('\uFFFD');
                            }

                            lastPos = searchPos;
                        }
                    }
                }
            }

            if (sb == null)
                return url;

            sb.Append(url, lastPos, url.Length - lastPos);
            return sb.ToString();
        }

        /// <summary>
        /// Clean a URL: remove surrounding whitespace and surrounding &lt; &gt; and remove \ that escape punctuation and other symbols.
        /// </summary>
        /// <remarks>Original: clean_url(ref string)</remarks>
        private static string CleanUrl(string url)
        {
            if (url.Length == 0)
                return url;

            // remove surrounding <> if any:
            url = url.Trim();

            if (url[0] == '<' && url[url.Length - 1] == '>')
                url = url.Substring(1, url.Length - 2);

            return Unescape(url);
        }

        /// <summary>
        /// Clean a title: remove surrounding quotes and remove \ that escape punctuation.
        /// </summary>
        /// <remarks>Original: clean_title(ref string)</remarks>
        private static string CleanTitle(string title)
        {
            // remove surrounding quotes if any:
            int titlelength = title.Length;
            if (titlelength == 0)
                return title;

            var a = title[0];
            var b = title[titlelength - 1];
            if ((a == '\'' && b == '\'') || (a == '(' && b == ')') || (a == '"' && b == '"'))
                title = title.Substring(1, titlelength - 2);

            return Unescape(title);
        }

        // Parse an autolink or HTML tag.
        // Assumes the subject has a '<' character at the current position.
        static Inline handle_pointy_brace(Subject subj)
        {
            int matchlen;
            string contents;

            // advance past first <
            subj.Position++;  

            // first try to match a URL autolink
            matchlen = Scanner.scan_autolink_uri(subj.Buffer, subj.Position, subj.Length);
            if (matchlen > 0)
            {
                contents = subj.Buffer.Substring(subj.Position, matchlen - 1);
                var resultContents = ParseStringEntities(contents);
                var result = Inline.CreateLink(resultContents, contents, string.Empty);

                result.SourcePosition = subj.Position - 1;
                resultContents.SourcePosition = subj.Position;
                subj.Position += matchlen;
                result.SourceLastPosition = subj.Position;
                resultContents.SourceLastPosition = subj.Position - 1;
                
                return result;
            }

            // next try to match an email autolink
            matchlen = Scanner.scan_autolink_email(subj.Buffer, subj.Position, subj.Length);
            if (matchlen > 0)
            {
                contents = subj.Buffer.Substring(subj.Position, matchlen - 1);
                var resultContents = ParseStringEntities(contents);
                var result = Inline.CreateLink(resultContents, "mailto:" + contents, string.Empty);
                
                result.SourcePosition = subj.Position - 1;
                resultContents.SourcePosition = subj.Position;
                subj.Position += matchlen;
                result.SourceLastPosition = subj.Position;
                resultContents.SourceLastPosition = subj.Position - 1;

                return result;
            }

            // finally, try to match an html tag
            matchlen = Scanner.scan_html_tag(subj.Buffer, subj.Position, subj.Length);
            if (matchlen > 0)
            {
                var result = new Inline(InlineTag.RawHtml, subj.Buffer, subj.Position - 1, matchlen + 1);
                result.SourcePosition = subj.Position - 1;
                subj.Position += matchlen;
                result.SourceLastPosition = subj.Position;
                return result;
            }
            else
            {
                // if nothing matches, just return the opening <:
                return new Inline("<", subj.Position - 1, subj.Position);
            }
        }

        // Parse a link or the link portion of an image, or return a fallback.
        static Reference ParseLinkDetails(Subject subj, bool supportPlaceholderBrackets)
        {
            int n;
            int sps;
            int endlabel, starturl, endurl, starttitle, endtitle, endall;
            string url, title;
            endlabel = subj.Position;

            var c = peek_char(subj);

            if (c == '(' &&
                    ((sps = Scanner.scan_spacechars(subj.Buffer, subj.Position + 1, subj.Length)) > -1) &&
                    ((n = Scanner.scan_link_url(subj.Buffer, subj.Position + 1 + sps, subj.Length)) > -1))
            {
                // try to parse an explicit link:
                starturl = subj.Position + 1 + sps; // after (
                endurl = starturl + n;
                starttitle = endurl + Scanner.scan_spacechars(subj.Buffer, endurl, subj.Length);
                // ensure there are spaces btw url and title
                endtitle = (starttitle == endurl) ? starttitle :
                           starttitle + Scanner.scan_link_title(subj.Buffer, starttitle, subj.Length);
                endall = endtitle + Scanner.scan_spacechars(subj.Buffer, endtitle, subj.Length);
                if (endall < subj.Length && subj.Buffer[endall] == ')')
                {
                    subj.Position = endall + 1;
                    url = subj.Buffer.Substring(starturl, endurl - starturl);
                    url = CleanUrl(url);
                    title = subj.Buffer.Substring(starttitle, endtitle - starttitle);
                    title = CleanTitle(title);

                    return new Reference() { Title = title, Url = url };
                }
            }
            else if (c == '[')
            {
                var label = ParseReferenceLabel(subj);
                if (label != null)
                {
                    if (label.Value.Length == 0)
                        return Reference.SelfReference;

                    var details = LookupReference(subj.DocumentData, label.Value);
                    if (details != null)
                        return details;

                    // rollback the subject but return InvalidReference so that the caller knows not to
                    // parse 'foo' from [foo][bar].
                    subj.Position = endlabel;
                    return Reference.InvalidReference;
                }
            }

            // rollback the subject position because didn't match anything.
            subj.Position = endlabel;
            if (supportPlaceholderBrackets)
            {
                return new Reference()
                {
                    Url = subj.Buffer.Substring(subj.LastPendingInline.StartPosition, subj.Position - subj.LastPendingInline.StartPosition - 1),
                    IsPlaceholder = true
                };
            }
            else
            {
                return null;
            }
        }

        // Parse a hard or soft linebreak, returning an inline.
        // Assumes the subject has a newline at the current position.
        static Inline handle_newline(Subject subj)
        {
            int nlpos = subj.Position;

            // skip over newline
            subj.Position++;

            // skip spaces at beginning of line
            var len = subj.Length;
            while (subj.Position < len && subj.Buffer[subj.Position] == ' ')
                subj.Position++;

            if (nlpos > 1 && subj.Buffer[nlpos - 1] == ' ' && subj.Buffer[nlpos - 2] == ' ')
                return new Inline(InlineTag.LineBreak) { SourcePosition = nlpos - 2, SourceLastPosition = nlpos + 1 };
            else
                return new Inline(InlineTag.SoftBreak) { SourcePosition = nlpos, SourceLastPosition = nlpos + 1 };
        }

        /// <summary>
        /// Parse an inline element from the subject. The subject position is updated to after the element.
        /// </summary>
        public static Inline ParseInline(Subject subj, Func<Subject, Inline>[] parsers, char[] specialCharacters)
        {
            var c = subj.Buffer[subj.Position];

            var parser = c < parsers.Length ? parsers[c] : null;

            if (parser != null)
                return parser(subj);

            var startpos = subj.Position;

            // we read until we hit a special character
            // +1 is so that any special character at the current position is ignored.
            var endpos = subj.Buffer.IndexOfAny(specialCharacters, startpos + 1, subj.Length - startpos - 1);

            if (endpos == -1)
                endpos = subj.Length;

            subj.Position = endpos;

            // if we're at a newline, strip trailing spaces.
            if (endpos < subj.Length && subj.Buffer[endpos] == '\n')
                while (endpos > startpos && subj.Buffer[endpos - 1] == ' ')
                    endpos--;

            return new Inline(subj.Buffer, startpos, endpos - startpos, startpos, endpos, c);
        }

        public static Inline parse_inlines(Subject subj, Func<Subject, Inline>[] parsers, char[] specialCharacters)
        {
            var len = subj.Length;

            if (len == 0)
                return null;

            var first = ParseInline(subj, parsers, specialCharacters);
            subj.LastInline = first.LastSibling;

            Inline cur;
            while (subj.Position < len)
            {
                cur = ParseInline(subj, parsers, specialCharacters);
                if (cur != null)
                {
                    subj.LastInline.NextSibling = cur;
                    subj.LastInline = cur.LastSibling;
                }
            }

            InlineStack.PostProcessInlineStack(subj, subj.FirstPendingInline, subj.LastPendingInline, InlineStack.InlineStackPriority.Maximum);

            return first;
        }

        // Parse zero or more space characters, including at most one newline.
        private static void spnl(Subject subj)
        {
            var seenNewline = false;
            var len = subj.Length;
            while (subj.Position < len)
            {
                var c = subj.Buffer[subj.Position];
                if (c == ' ' || (!seenNewline && (seenNewline = c == '\n')))
                    subj.Position++;
                else
                    return;
            }
        }

        /// <summary>
        /// Parses the contents of [..] for a reference label. Only used for parsing 
        /// reference definition labels for use with the reference dictionary because 
        /// it does not properly parse nested inlines.
        /// 
        /// Assumes the source starts with '[' character.
        /// Returns null and does not advance if no matching ] is found.
        /// Note the precedence:  code backticks have precedence over label bracket
        /// markers, which have precedence over *, _, and other inline formatting
        /// markers. So, 2 below contains a link while 1 does not:
        /// 1. [a link `with a ](/url)` character
        /// 2. [a link *with emphasized ](/url) text*        /// </summary>
        private static StringPart? ParseReferenceLabel(Subject subj)
        {
            var startPos = subj.Position;
            var source = subj.Buffer;
            var len = subj.Length;

            var labelStartPos = ++subj.Position;

            len = subj.Position + Reference.MaximumReferenceLabelLength;
            if (len > source.Length)
                len = source.Length;

            subj.Position = source.IndexOfAny(BracketSpecialCharacters, subj.Position, len - subj.Position);
            while (subj.Position > -1)
            {
                var c = source[subj.Position];
                if (c == '\\')
                {
                    subj.Position += 2;
                    if (subj.Position >= len)
                        break;

                    subj.Position = source.IndexOfAny(BracketSpecialCharacters, subj.Position, len - subj.Position);
                }
                else if (c == '[')
                {
                    break;
                }
                else
                {
                    var label = new StringPart(source, labelStartPos, subj.Position - labelStartPos);
                    subj.Position++;
                    return label;
                }
            }

            subj.Position = startPos;
            return null;
        }

        // Parse reference.  Assumes string begins with '[' character.
        // Modify refmap if a reference is encountered.
        // Return 0 if no reference found, otherwise position of subject
        // after reference is parsed.
        public static int ParseReference(Subject subj)
        {
            string title;
            var startPos = subj.Position;

            // parse label:
            var lab = ParseReferenceLabel(subj);
            if (lab == null || lab.Value.Length > Reference.MaximumReferenceLabelLength)
                goto INVALID;

            if (!Scanner.HasNonWhitespace(lab.Value))
                goto INVALID;

            // colon:
            if (peek_char(subj) == ':')
                subj.Position++;
            else
                goto INVALID;

            // parse link url:
            spnl(subj);
            var matchlen = Scanner.scan_link_url(subj.Buffer, subj.Position, subj.Length);
            if (matchlen == 0)
                goto INVALID;

            var url = subj.Buffer.Substring(subj.Position, matchlen);
            url = CleanUrl(url);
            subj.Position += matchlen;

            // parse optional link_title
            var beforetitle = subj.Position;
            spnl(subj);

            matchlen = Scanner.scan_link_title(subj.Buffer, subj.Position, subj.Length);
            if (matchlen > 0)
            {
                title = subj.Buffer.Substring(subj.Position, matchlen);
                title = CleanTitle(title);
                subj.Position += matchlen;
            }
            else
            {
                subj.Position = beforetitle;
                title = string.Empty;
            }

            char c;
            // parse final spaces and newline:
            while ((c = peek_char(subj)) == ' ') subj.Position++;

            if (c == '\n')
            {
                subj.Position++;
            }
            else if (c != '\0')
            {
                if (matchlen > 0)
                { // try rewinding before title
                    subj.Position = beforetitle;
                    while ((c = peek_char(subj)) == ' ') subj.Position++;
                    if (c == '\n')
                        subj.Position++;
                    else if (c != '\0')
                       goto INVALID;
                }
                else
                {
                    goto INVALID;
                }
            }

            // insert reference into refmap
            AddReference(subj.DocumentData.ReferenceMap, lab.Value, url, title);

            return subj.Position;

        INVALID:
            subj.Position = startPos;
            return 0;
        }
    }
}
