using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal sealed class PositionTracker
    {
        public PositionTracker(int blockOffset)
        {
            this._blockOffset = blockOffset;
        }

        private static readonly PositionOffset EmptyPositionOffset = default(PositionOffset);

        private int _blockOffset;

        public void AddBlockOffset(int offset)
        {
            this._blockOffset += offset;
        }

        public void AddOffset(LineInfo line, int startIndex, int length)
        {
            if (this.OffsetCount + line.OffsetCount + 2 >= this.Offsets.Length)
                Array.Resize(ref this.Offsets, this.Offsets.Length + line.OffsetCount + 20);

            PositionOffset po1, po2;

            if (startIndex > 0)
                po1 = new PositionOffset(
                    CalculateOrigin(line.Offsets, line.OffsetCount, line.LineOffset, false, true),
                    startIndex);
            else
                po1 = EmptyPositionOffset;

            if (line.Line.Length - startIndex - length > 0)
                po2 = new PositionOffset(
                    CalculateOrigin(line.Offsets, line.OffsetCount, line.LineOffset + startIndex + length, false, true),
                    line.Line.Length - startIndex - length);
            else
                po2 = EmptyPositionOffset;

            var indexAfterLastCopied = 0;

            if (po1.Offset == 0)
            {
                if (po2.Offset == 0)
                    goto FINTOTAL;

                po1 = po2;
                po2 = EmptyPositionOffset;
            }

            for (var i = 0; i < line.OffsetCount; i++)
            {
                var pc = line.Offsets[i];
                if (pc.Position > po1.Position)
                {
                    if (i > indexAfterLastCopied)
                    {
                        Array.Copy(line.Offsets, indexAfterLastCopied, this.Offsets, this.OffsetCount, i - indexAfterLastCopied);
                        this.OffsetCount += i - indexAfterLastCopied;
                        indexAfterLastCopied = i;
                    }

                    this.Offsets[this.OffsetCount++] = po1;

                    po1 = po2;

                    if (po1.Offset == 0)
                        goto FIN;

                    po2 = EmptyPositionOffset;
                }
            }

        FIN:
            if (po1.Offset != 0)
                this.Offsets[this.OffsetCount++] = po1;

            if (po2.Offset != 0)
                this.Offsets[this.OffsetCount++] = po2;

        FINTOTAL:
            Array.Copy(line.Offsets, indexAfterLastCopied, this.Offsets, this.OffsetCount, line.OffsetCount - indexAfterLastCopied);
            this.OffsetCount += line.OffsetCount - indexAfterLastCopied;
        }

        public int CalculateInlineOrigin(int position, bool isStartPosition)
        {
            return CalculateOrigin(this.Offsets, this.OffsetCount, this._blockOffset + position, true, isStartPosition);
        }

        internal static int CalculateOrigin(PositionOffset[] offsets, int offsetCount, int position, bool includeReduce, bool isStart)
        {
            if (isStart)
                position++;

            var minus = 0;
            for (var i = 0; i < offsetCount; i++)
            {
                var po = offsets[i];
                if (po.Position < position)
                {
                    if (po.Offset > 0)
                        position += po.Offset;
                    else
                        minus += po.Offset;
                }
                else
                    break;
            }

            if (includeReduce)
                position += minus;

            if (isStart)
                position--;

            return position;
        }

        private PositionOffset[] Offsets = new PositionOffset[10];
        private int OffsetCount;
    }
}
