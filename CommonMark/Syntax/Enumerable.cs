using System;
using System.Collections.Generic;

namespace CommonMark.Syntax
{
    internal class Enumerable : IEnumerable<EnumeratorEntry>
    {
        private Block _root;

        public Enumerable(Block root)
        {
            if (root == null)
                throw new ArgumentNullException(nameof(root));

            this._root = root;
        }

        public IEnumerator<EnumeratorEntry> GetEnumerator()
        {
            return new Enumerator(this._root);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private sealed class Enumerator : IEnumerator<EnumeratorEntry>
        {
            private Block _root;
            private EnumeratorEntry _current;
            private Stack<BlockStackEntry> _blockStack = new Stack<BlockStackEntry>();
            private Stack<InlineStackEntry> _inlineStack = new Stack<InlineStackEntry>();

            public Enumerator(Block root)
            {
                this._root = root;
                this._blockStack.Push(new BlockStackEntry(root, null));
            }

            public EnumeratorEntry Current
            {
                get { return this._current; }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this._current; }
            }

#if OptimizeFor45
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
#endif
            private bool ShouldSkip(Inline inline)
            {
                if (inline.Tag == InlineTag.String
                    && inline.FirstChild == null
                    && inline.LiteralContentValue.Length == 0)
                    return true;

                return false;
            }

            public bool MoveNext()
            {
            repeatMoveNext:

                Inline inline;
                if (this._inlineStack.Count > 0)
                {
                    var entry = this._inlineStack.Pop();
                    if (entry.NeedsClose != null)
                    {
                        inline = entry.NeedsClose;
                        this._current = new EnumeratorEntry(false, true, inline);

                        if (entry.Target != null)
                        {
                            entry.NeedsClose = null;
                            this._inlineStack.Push(entry);
                        }

                        return true;
                    }

                    if (entry.Target != null)
                    {
                        inline = entry.Target;
                        this._current = new EnumeratorEntry(true, inline.FirstChild == null, inline);

                        if (inline.FirstChild != null)
                        {
                            this._inlineStack.Push(new InlineStackEntry(inline.NextSibling, inline));
                            this._inlineStack.Push(new InlineStackEntry(inline.FirstChild, null));
                        }
                        else if (inline.NextSibling != null)
                        {
                            this._inlineStack.Push(new InlineStackEntry(inline.NextSibling, null));
                        }

                        if (this.ShouldSkip(this._current.Inline))
                        {
                            goto repeatMoveNext;
                        }
                    }

                    return true;
                }

                Block block;
                if (this._blockStack.Count > 0)
                {
                    var entry = this._blockStack.Pop();
                    if (entry.NeedsClose != null)
                    {
                        block = entry.NeedsClose;
                        this._current = new EnumeratorEntry(false, true, block);

                        if (entry.Target != null)
                        {
                            entry.NeedsClose = null;
                            this._blockStack.Push(entry);
                        }

                        return true;
                    }

                    if (entry.Target != null)
                    {
                        block = entry.Target;
                        this._current = new EnumeratorEntry(true, block.FirstChild == null && block.InlineContent == null, block);

                        if (block.FirstChild != null)
                        {
                            this._blockStack.Push(new BlockStackEntry(block.NextSibling, block));
                            this._blockStack.Push(new BlockStackEntry(block.FirstChild, null));
                        }
                        else if (block.NextSibling != null && block != this._root)
                        {
                            this._blockStack.Push(new BlockStackEntry(block.NextSibling, block.InlineContent == null ? null : block));
                        }
                        else if (block.InlineContent != null)
                        {
                            this._blockStack.Push(new BlockStackEntry(null, block));
                        }

                        if (block.InlineContent != null)
                        {
                            this._inlineStack.Push(new InlineStackEntry(block.InlineContent, null));
                        }
                    }

                    return true;
                }

                return false;
            }

            public void Reset()
            {
                this._current = null;
                this._blockStack.Clear();
                this._inlineStack.Clear();
            }

            void IDisposable.Dispose()
            {
            }

            private struct BlockStackEntry
            {
                public readonly Block Target;
                public Block NeedsClose;
                public BlockStackEntry(Block target, Block needsClose)
                {
                    this.Target = target;
                    this.NeedsClose = needsClose;
                }
            }
            private struct InlineStackEntry
            {
                public readonly Inline Target;
                public Inline NeedsClose;
                public InlineStackEntry(Inline target, Inline needsClose)
                {
                    this.Target = target;
                    this.NeedsClose = needsClose;
                }
            }
        }
    }
}
