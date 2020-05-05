using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "in front", "in front of" relation
    /// </summary>
    public class InFrontEdge : Edge
    {
        // Flag if it is reversed relation
        public bool IsReversed { get; private set; }

        public InFrontEdge()
        {
            this.IsReversed = false;
        }

        public InFrontEdge(bool reversed)
        {
            this.IsReversed = reversed;
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            if (this.IsReversed)
            {
                this.SwitchVertices();
                this.CopyPosition(this.Right, this.Left);
            }
            else
                this.CopyPosition(this.Left, this.Right);

            this.PositionateLeft(maxWidth, maxHeight);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            if (this.IsReversed && this.Left.Position == null)
            {
                this.CopyPosition(this.Right, this.Left);
                this.SwitchVertices();
            }

            this.Left.ZIndex++;
            this.Left.Position = this.Right.Position + new Vector2(-this.Left.Width / 2, this.Right.Height - this.Left.Height / 2);
        }
    }
}
