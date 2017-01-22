using System;
using System.Collections.Generic;
using System.Text;

namespace CommonMark.Parser
{
    internal struct PositionOffset
    {
        public PositionOffset(int position, int offset)
        {
            this.Position = position;
            this.Offset = offset;
        }
        
        public int Position;
        public int Offset;

        public override string ToString()
        {
            if (this.Offset == 0)
                return "empty";

            return (this.Offset < 0 ? string.Empty : "+")
                + this.Offset.ToString(System.Globalization.CultureInfo.InvariantCulture)
                + " chars @ " + this.Position.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
