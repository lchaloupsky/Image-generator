using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "in" relation
    /// </summary>
    public class InEdge : Edge
    {
        // Max left width
        private int MaxWidth { get => this.Right.Width / 2; }

        // Max left height
        private int MaxHeight { get => this.Right.Height / 2; }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.CopyPosition(this.Left, this.Right);
            this.PositionateLeft(MaxWidth, maxHeight);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.PositionHelper.RescaleWithMax(this.MaxWidth, this.Left.Width, this.Left);
            this.PositionHelper.RescaleWithMax(this.MaxHeight, this.Left.Height, this.Left);
            this.Left.ZIndex++;
            this.Left.Position = this.Right.Position + new Vector2(this.MaxWidth, this.MaxHeight);
        }
    }
}
