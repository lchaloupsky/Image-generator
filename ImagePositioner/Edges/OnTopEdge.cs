using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;

namespace ImagePositioner.Edges
{
    class OnTopEdge : AbsoluteEdge
    {
        private IDrawable LastElement { get; set; } = null;

        public OnTopEdge() : base(ImageGeneratorInterfaces.Edges.PlaceType.HORIZONTAL) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(this.GetShift(maxWidth, this.Left.Width), 0);
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position + new Vector2(this.GetShift(this.Left.Width, this.Right.Width), this.Left.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position - new Vector2(this.GetShift(this.Left.Width, this.Right.Width), this.Left.Height);
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            this.LastElement = this.LastElement ?? this.Left;
            var newEdge = new ToLeftEdge();
            newEdge.Add(this.LastElement, edge.Left);
            this.LastElement = edge.Left;

            return newEdge;
        }
    }
}
