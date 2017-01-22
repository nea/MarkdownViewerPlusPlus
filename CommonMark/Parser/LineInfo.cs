using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal sealed class LineInfo
    {
        public LineInfo(bool trackPositions)
        {
            this.IsTrackingPositions = trackPositions;
        }

        public readonly bool IsTrackingPositions;

        public string Line;

        /// <summary>
        /// Gets or sets the offset in the source data at which the current line starts.
        /// </summary>
        public int LineOffset;

        public int LineNumber;

        public PositionOffset[] Offsets = new PositionOffset[20];

        public int OffsetCount;

        public void AddOffset(int position, int offset)
        {
            if (offset == 0)
                return;

            if (this.OffsetCount == this.Offsets.Length)
                Array.Resize(ref this.Offsets, this.OffsetCount + 20);

            this.Offsets[this.OffsetCount++] = new PositionOffset(position, offset);
        }

        public override string ToString()
        {
            string ln;
            if (this.Line == null)
                ln = string.Empty;
            else if (this.Line.Length < 50)
                ln = this.Line;
            else
                ln = this.Line.Substring(0, 49) + "…";

            return this.LineNumber.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + ": " + ln;
        }

        public int CalculateOrigin(int position, bool isStartPosition)
        {
            return PositionTracker.CalculateOrigin(this.Offsets, this.OffsetCount, this.LineOffset + position, true, isStartPosition);
        }
    }
}
