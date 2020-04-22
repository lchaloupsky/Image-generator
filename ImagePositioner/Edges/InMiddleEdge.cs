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
    class InMiddleEdge : AbsoluteEdge
    {
        private int MaxWidth { get => this.Right.Width / 2; }
        private int MaxHeight { get => this.Right.Height / 2; }
        private const int MaxInRow = 3;

        private IDrawable LastElement { get; set; } = null;

        public InMiddleEdge() : base(ImageGeneratorInterfaces.Edges.PlaceType.MIDDLE) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(this.GetShift(maxWidth, this.Left.Width), this.GetShift(maxHeight, this.Left.Height));
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.CopyPosition(this.Left, this.Right);
            this.PositionateLeft(MaxWidth, maxHeight);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.RescaleWithMax(this.MaxWidth, this.Left.Width, this.Left);
            this.RescaleWithMax(this.MaxHeight, this.Left.Height, this.Left);
            this.Left.ZIndex++;
            this.Left.Position = this.Right.Position
                               + new Vector2(this.GetShift(this.Right.Width, this.Left.Width), this.GetShift(this.Right.Height, this.Left.Height));
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
