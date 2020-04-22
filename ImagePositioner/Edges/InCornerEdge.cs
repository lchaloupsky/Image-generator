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
    class InCornerEdge : AbsoluteEdge
    {
        private int MaxWidth { get => this.Right.Width / 2; }
        private int MaxHeight { get => this.Right.Height / 2; }

        private VerticalPlace Vertical { get; }
        private HorizontalPlace Horizontal { get; }

        private IDrawable LastElement { get; set; }

        public InCornerEdge() : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER)
        {
            this.Horizontal = this.GetRandomEnum<HorizontalPlace>();
            this.Vertical = this.GetRandomEnum<VerticalPlace>();
        }

        public InCornerEdge(VerticalPlace vertical) : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER)
        {
            this.Vertical = vertical;
            this.Horizontal = this.GetRandomEnum<HorizontalPlace>();
        }

        public InCornerEdge(HorizontalPlace horizontal) : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER)
        {
            this.Horizontal = horizontal;
            this.Vertical = this.GetRandomEnum<VerticalPlace>();
        }

        public InCornerEdge(HorizontalPlace horizontal, VerticalPlace vertical) : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER)
        {
            this.Vertical = vertical;
            this.Horizontal = horizontal;
        }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.PlaceLeft(maxWidth, maxHeight);
            this.Left.Position = new Vector2(Math.Max(0, this.Left.Position.Value.X), Math.Max(0, this.Left.Position.Value.Y));
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.CopyPosition(this.Left, this.Right);
            this.PositionateLeft(maxWidth, maxHeight);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.RescaleWithMax(this.MaxWidth, this.Left.Width, this.Left);
            this.RescaleWithMax(this.MaxHeight, this.Left.Height, this.Left);
            this.Left.ZIndex++;
            this.PlaceLeft(this.Right.Width, this.Right.Height);
        }

        private void PlaceLeft(int RightWidth, int RightHeight)
        {
            // Place it in the middle
            var rightPos = this.Right?.Position ?? new Vector2(0, 0);
            this.Left.Position = rightPos + new Vector2(this.GetShift(RightWidth, this.Left.Width),
                                                                   this.GetShift(RightHeight, this.Left.Height));

            // Do shift to the right side
            Vector2 shift = (Vector2)(this.Left.Position - rightPos);
            if (this.Vertical == VerticalPlace.TOP)
                shift.Y *= -1;
            if (this.Horizontal == HorizontalPlace.LEFT)
                shift.X *= -1;

            this.Left.Position += shift;
        }

        private T GetRandomEnum<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(new Random().Next(values.Length));
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            var cornerEdge = (InCornerEdge)edge;
            if (cornerEdge.Horizontal != this.Horizontal || cornerEdge.Vertical != this.Vertical)
                return null;

            this.LastElement = this.LastElement ?? this.Left;
            var newEdge = new ToLeftEdge();
            newEdge.Add(this.LastElement, edge.Left);
            this.LastElement = edge.Left;

            return newEdge;
        }
    }
}
