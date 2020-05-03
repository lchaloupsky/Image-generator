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
        // Flag if the relation is reversed
        private bool Reversed { get; set; }

        public UnderEdge()
        {
            this.Reversed = false;
        }

        public UnderEdge(bool reversed)
        {
            this.Reversed = true;
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            if (this.Reversed)
            {
                this.Reversed = false;
                this.SwitchVertices();
                this.PositionateLeft(maxWidth, maxHeight);
                return;
            }

            this.Right.Position = this.Left.Position - new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width), this.Right.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            if (this.Reversed)
            {
                this.Reversed = false;
                this.SwitchVertices();
                this.PositionateRight(maxWidth, maxHeight);
                return;
            }

            this.Left.Position = this.Right.Position + new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width), this.Right.Height);
        }
    }
}
