using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "on" relation
    /// </summary>
    public class OnEdge : Edge
    {
        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position + new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Left.Width, this.Right.Width), this.Left.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position - new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Left.Width, this.Right.Width), this.Left.Height);
        }
    }
}
