using ImagePositioner.Helpers;
using System.Numerics;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "on corner", "on corner of" relations
    /// </summary>
    public class OnCornerEdge : Edge
    {
        // Max left width
        private int MaxWidth => (int)(this.Right.Width * 0.75);

        // Max left height
        private int MaxHeight => (int)(this.Right.Height * 0.75);

        // Enum helper
        private EnumHelper EnumHelper { get; } = new EnumHelper();

        // Vertical position type
        public VerticalPlace Vertical { get; }

        // Horizontal position type
        public HorizontalPlace Horizontal { get; }

        public OnCornerEdge()
        {
            this.Horizontal = this.EnumHelper.GetRandomEnum<HorizontalPlace>();
            this.Vertical = this.EnumHelper.GetRandomEnum<VerticalPlace>();
        }

        public OnCornerEdge(VerticalPlace vertical)
        {
            this.Vertical = vertical;
            this.Horizontal = this.EnumHelper.GetRandomEnum<HorizontalPlace>();
        }

        public OnCornerEdge(HorizontalPlace horizontal)
        {
            this.Horizontal = horizontal;
            this.Vertical = this.EnumHelper.GetRandomEnum<VerticalPlace>();
        }

        public OnCornerEdge(HorizontalPlace horizontal, VerticalPlace vertical)
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
            this.PositionHelper.RescaleWithMax(this.MaxWidth, this.Left.Width, this.Left);
            this.PositionHelper.RescaleWithMax(this.MaxHeight, this.Left.Height, this.Left);
            this.Left.ZIndex++;
            this.PlaceLeft(this.Right.Width, this.Right.Height);
        }

        private void PlaceLeft(int rightWidth, int rightHeight)
        {
            // Place it in the middle
            var rightPos = this.Right?.Position ?? new Vector2(0, 0);
            this.Left.Position = rightPos + new Vector2(this.PositionHelper.GetShiftToCenterVertex(rightWidth, this.Left.Width),
                                                        this.PositionHelper.GetShiftToCenterVertex(rightHeight, this.Left.Height));

            // Do shift to the right side
            Vector2 shift = (Vector2)(this.Left.Position - rightPos + new Vector2(this.Left.Width, this.Left.Height) / 2);
            if (this.Vertical == VerticalPlace.TOP)
                shift.Y *= -1;
            if (this.Horizontal == HorizontalPlace.LEFT)
                shift.X *= -1;

            this.Left.Position += shift;
        }
    }
}
