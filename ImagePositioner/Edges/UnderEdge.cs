using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    class UnderEdge : Edge
    {
        private int AllowedMargin { get => this.Right.Width / 2; }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position - new Vector2(this.GetShift(this.Right.Width, this.Left.Width), this.Right.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position + new Vector2(this.GetShift(this.Right.Width, this.Left.Width), this.Right.Height);
        }

        //protected override bool CheckConcretePosition()
        //{
        //    var diff = this.Left.Position - this.Right.Position;
        //    return (diff.Value.X >= -this.AllowedMargin && diff.Value.X <= this.AllowedMargin) && diff.Value.Y == this.Left.Height;
        //}
    }
}
