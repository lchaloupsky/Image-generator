using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "under", etc. relations
    /// </summary>
    class UnderEdge : Edge
    {
        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position - new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width), this.Right.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position + new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width), this.Right.Height);
        }
    }
}
