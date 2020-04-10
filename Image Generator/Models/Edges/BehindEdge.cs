using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Edges
{
    class BehindEdge : Edge
    {
        private int MaxWidth { get => this.Right.Width / 2; }
        private int MaxHeight { get => this.Right.Height / 2; }
        private bool IsReversed { get; }

        public BehindEdge()
        {
            this.IsReversed = false;
        }

        public BehindEdge(bool reversed)
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

            this.PositionateLeft(MaxWidth, maxHeight);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            if (this.IsReversed && this.Left.Position == null)
            {
                this.CopyPosition(this.Right, this.Left);
                this.SwitchVertices();
            }

            this.Right.ZIndex++;
            this.Left.Position = this.Right.Position + new Vector2(this.MaxWidth, - this.MaxHeight);
        }
    }
}
