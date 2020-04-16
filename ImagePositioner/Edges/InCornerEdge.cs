using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    class InCornerEdge : Edge
    {
        private int MaxWidth { get => this.Right.Width / 2; }
        private int MaxHeight { get => this.Right.Height / 2; }

        private VerticalPlace Vertical { get; }
        private HorizontalPlace Horizontal { get; }

        public InCornerEdge()
        {
            this.Horizontal = this.GetRandomEnum<HorizontalPlace>();
            this.Vertical = this.GetRandomEnum<VerticalPlace>();
        }

        public InCornerEdge(VerticalPlace vertical)
        {
            this.Vertical = vertical;
            this.Horizontal = this.GetRandomEnum<HorizontalPlace>();
        }

        public InCornerEdge(HorizontalPlace horizontal)
        {
            this.Horizontal = horizontal;
            this.Vertical = this.GetRandomEnum<VerticalPlace>();
        }

        public InCornerEdge(HorizontalPlace horizontal, VerticalPlace vertical)
        {
            this.Vertical = vertical;
            this.Horizontal = horizontal;
        }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.PlaceLeft(maxWidth, maxHeight);
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
    }
}
