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
    class ToLeftEdge : AbsoluteEdge
    {
        private IDrawable LastElement { get; set; } = null;

        public ToLeftEdge() : base(ImageGeneratorInterfaces.Edges.PlaceType.VERTICAL) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(0, this.GetShift(maxHeight ,this.Left.Height));
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position + new Vector2(this.Left.Width, this.GetShift(this.Left.Height, this.Right.Height));
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position - new Vector2(this.Left.Width, this.GetShift(this.Left.Height, this.Right.Height));
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            this.LastElement = this.LastElement ?? this.Left;
            var newEdge = new UnderEdge();
            newEdge.Add(this.LastElement, edge.Left);
            this.LastElement = edge.Left;

            return newEdge;
        }
    }
}
