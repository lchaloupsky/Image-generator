using ImageGeneratorInterfaces.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    class OnCornerEdge : AbsoluteEdge
    {
        private int MaxWidth { get => (int)(this.Right.Width * 0.75); }
        private int MaxHeight { get => (int)(this.Right.Height * 0.75); }

        private VerticalPlace Vertical { get; }
        private HorizontalPlace Horizontal { get; }

        public OnCornerEdge() : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER) 
        {
            this.Horizontal = this.GetRandomEnum<HorizontalPlace>();
            this.Vertical = this.GetRandomEnum<VerticalPlace>();
        }

        public OnCornerEdge(VerticalPlace vertical) : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER)
        {
            this.Vertical = vertical;
            this.Horizontal = this.GetRandomEnum<HorizontalPlace>();
        }

        public OnCornerEdge(HorizontalPlace horizontal) : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER)
        {
            this.Horizontal = horizontal;
            this.Vertical = this.GetRandomEnum<VerticalPlace>();
        }

        public OnCornerEdge(HorizontalPlace horizontal, VerticalPlace vertical) : base(ImageGeneratorInterfaces.Edges.PlaceType.CORNER)
        {
            this.Vertical = vertical;
            this.Horizontal = horizontal;
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
            Vector2 shift = (Vector2)(this.Left.Position - rightPos  + new Vector2(this.Left.Width, this.Left.Height) / 2 );
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
            throw new NotImplementedException();
        }
    }
}
