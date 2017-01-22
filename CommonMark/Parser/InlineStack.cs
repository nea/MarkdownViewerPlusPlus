using System;
using CommonMark.Syntax;

namespace CommonMark.Parser
{
    /// <summary>
    /// Describes an element in a stack of possible inline openers.
    /// </summary>
    internal sealed class InlineStack
    {
        /// <summary>
        /// The parser priority if this stack entry.
        /// </summary>
        public InlineStackPriority Priority;

        /// <summary>
        /// Previous entry in the stack; <see langword="null"/> if this is the last one.
        /// </summary>
        public InlineStack Previous;

        /// <summary>
        /// Next entry in the stack; <see langword="null"/> if this is the last one.
        /// </summary>
        public InlineStack Next;

        /// <summary>
        /// The at-the-moment text inline that could be transformed into the opener.
        /// </summary>
        public Inline StartingInline;

        /// <summary>
        /// The number of delimiter characters found for this opener.
        /// </summary>
        public int DelimiterCount;

        /// <summary>
        /// The character that was used in the opener.
        /// </summary>
        public char Delimiter;

        /// <summary>
        /// The position in the <see cref="Subject.Buffer"/> where this inline element was found.
        /// Used only if the specific parser requires this information.
        /// </summary>
        public int StartPosition;

        /// <summary>
        /// The flags set for this stack entry.
        /// </summary>
        public InlineStackFlags Flags;

        [Flags]
        public enum InlineStackFlags : byte
        {
            None = 0,
            Opener = 1,
            Closer = 2,
            ImageLink = 4,
            /// <summary>
            /// The <c>Closer</c> flag will be removed during incremental processing. However some parts of the parser
            /// requires knowledge if the delimiter was ever considered to be a closer so this flag would then be used.
            /// </summary>
            CloserOriginally = 8
        }

        public enum InlineStackPriority : byte
        {
            Emphasis = 0,
            Links = 1,
            Maximum = Links
        }

        public static InlineStack FindMatchingOpener(InlineStack searchBackwardsFrom, InlineStackPriority priority, 
            char delimiter, int closerDelimiterCount, bool closerCanOpen, out bool canClose)
        {
            canClose = true;
            var istack = searchBackwardsFrom;
            while (true)
            {
                if (istack == null)
                {
                    // this cannot be a closer since there is no opener available.
                    canClose = false;
                    return null;
                }

                if (istack.Priority > priority ||
                    (istack.Delimiter == delimiter && 0 != (istack.Flags & InlineStackFlags.Closer)))
                {
                    // there might be a closer further back but we cannot go there yet because a higher priority element is blocking
                    // the other option is that the stack entry could be a closer for the same char - this means
                    // that any opener we might find would first have to be matched against this closer.
                    return null;
                }

                if (istack.Delimiter == delimiter) {

                    // interior closer of size 2 does not match opener of size 1 and vice versa.
                    // for more details, see https://github.com/jgm/cmark/commit/c50197bab81d7105c9c790548821b61bcb97a62a
                    var oddMatch = (closerCanOpen || (istack.Flags & InlineStackFlags.CloserOriginally) > 0)
                        && istack.DelimiterCount != closerDelimiterCount
                        && ((istack.DelimiterCount + closerDelimiterCount) % 3 == 0);

                    if (!oddMatch)
                        return istack;
                }

                istack = istack.Previous;
            }
        }

        public static void AppendStackEntry(InlineStack entry, Subject subj)
        {
            if (subj.LastPendingInline != null)
            {
                entry.Previous = subj.LastPendingInline;
                subj.LastPendingInline.Next = entry;
            }

            if (subj.FirstPendingInline == null)
                subj.FirstPendingInline = entry;

            subj.LastPendingInline = entry;
        }

        /// <summary>
        /// Removes a subset of the stack.
        /// </summary>
        /// <param name="first">The first entry to be removed.</param>
        /// <param name="subj">The subject associated with this stack. Can be <see langword="null"/> if the pointers in the subject should not be updated.</param>
        /// <param name="last">The last entry to be removed. Can be <see langword="null"/> if everything starting from <paramref name="first"/> has to be removed.</param>
        public static void RemoveStackEntry(InlineStack first, Subject subj, InlineStack last)
        {
            var curPriority = first.Priority;

            if (last == null)
            {
                if (first.Previous != null)
                    first.Previous.Next = null;
                else if (subj != null)
                    subj.FirstPendingInline = null;

                if (subj != null)
                {
                    last = subj.LastPendingInline;
                    subj.LastPendingInline = first.Previous;
                }

                first = first.Next;
            }
            else
            {
                if (first.Previous != null)
                    first.Previous.Next = last.Next;
                else if (subj != null)
                    subj.FirstPendingInline = last.Next;

                if (last.Next != null)
                    last.Next.Previous = first.Previous;
                else if (subj != null)
                    subj.LastPendingInline = first.Previous;

                if (first == last)
                    return;

                first = first.Next;
                last = last.Previous;
            }

            if (last == null || first == null)
                return;

            first.Previous = null;
            last.Next = null;

            // handle case like [*b*] (the whole [..] is being removed but the inner *..* must still be matched).
            // this is not done automatically because the initial * is recognized as a potential closer (assuming
            // potential scenario '*[*' ).
            if (curPriority > 0)
                PostProcessInlineStack(null, first, last, curPriority);
        }

        public static void PostProcessInlineStack(Subject subj, InlineStack first, InlineStack last, InlineStackPriority ignorePriority)
        {
            while (ignorePriority > 0)
            {
                var istack = first;
                while (istack != null)
                {
                    if (istack.Priority >= ignorePriority)
                    {
                        RemoveStackEntry(istack, subj, istack);
                    }
                    else if (0 != (istack.Flags & InlineStackFlags.Closer))
                    {
                        bool canClose;
                        var iopener = FindMatchingOpener(istack.Previous, istack.Priority, istack.Delimiter, istack.DelimiterCount, (istack.Flags & InlineStackFlags.Opener) > 0, out canClose);
                        if (iopener != null)
                        {
                            bool retry = false;
                            if (iopener.Delimiter == '~')
                            {
                                InlineMethods.MatchInlineStack(iopener, subj, istack.DelimiterCount, istack, (InlineTag)0, InlineTag.Strikethrough);
                                if (istack.DelimiterCount > 1)
                                    retry = true;
                            }
                            else
                            {
                                var useDelims = InlineMethods.MatchInlineStack(iopener, subj, istack.DelimiterCount, istack, InlineTag.Emphasis, InlineTag.Strong);
                                if (istack.DelimiterCount > 0)
                                    retry = true;
                            }

                            if (retry)
                            {
                                // remove everything between opened and closer (not inclusive).
                                if (istack.Previous != null && iopener.Next != istack.Previous)
                                    RemoveStackEntry(iopener.Next, subj, istack.Previous);

                                continue;
                            }
                            else
                            {
                                // remove opener, everything in between, and the closer
                                RemoveStackEntry(iopener, subj, istack);
                            }
                        }
                        else if (!canClose)
                        {
                            // this case means that a matching opener does not exist
                            // remove the Closer flag so that a future Opener can be matched against it.
                            istack.Flags &= ~InlineStackFlags.Closer;
                        }
                    }

                    if (istack == last)
                        break;

                    istack = istack.Next;
                }

                ignorePriority--;
            }
        }


    }
}
