using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    class ToRightEdge : Edge
    {
        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(maxWidth - this.Left.Width, this.GetShift(maxHeight, this.Left.Height));
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position - new Vector2(this.Right.Width, this.GetShift(this.Right.Height, this.Left.Height));
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position + new Vector2(this.Right.Width, this.GetShift(this.Right.Height, this.Left.Height));
        }
    }
}
