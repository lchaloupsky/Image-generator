using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImagePositioner.Helpers;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "in corner", "in corner of", etc. relation
    /// </summary>
    public class InCornerEdge : AbsoluteEdge
    {
        // Max left width
        private int MaxWidth { get => this.Right.Width / 2; }

        // Max left height
        private int MaxHeight { get => this.Right.Height / 2; }

        // Enum helper
        private EnumHelper EnumHelper { get; } = new EnumHelper();

        // Vertical position tyoe
        public VerticalPlace Vertical { get; }

        // Horizontal position type
        public HorizontalPlace Horizontal { get; }

        public InCornerEdge() : base(PlaceType.CORNER)
        {
            this.Horizontal = this.EnumHelper.GetRandomEnum<HorizontalPlace>();
            this.Vertical = this.EnumHelper.GetRandomEnum<VerticalPlace>();
        }

        public InCornerEdge(VerticalPlace vertical) : base(PlaceType.CORNER)
        {
            this.Vertical = vertical;
            this.Horizontal = this.EnumHelper.GetRandomEnum<HorizontalPlace>();
        }

        public InCornerEdge(HorizontalPlace horizontal) : base(PlaceType.CORNER)
        {
            this.Horizontal = horizontal;
            this.Vertical = this.EnumHelper.GetRandomEnum<VerticalPlace>();
        }

        public InCornerEdge(HorizontalPlace horizontal, VerticalPlace vertical) : base(PlaceType.CORNER)
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
            this.PositionHelper.RescaleWithMax(this.MaxWidth, this.Left.Width, this.Left);
            this.PositionHelper.RescaleWithMax(this.MaxHeight, this.Left.Height, this.Left);
            this.Left.ZIndex++;
            this.PlaceLeft(this.Right.Width, this.Right.Height);
        }

        private void PlaceLeft(int RightWidth, int RightHeight)
        {
            // Place it in the middle
            var rightPos = this.Right?.Position ?? new Vector2(0, 0);
            this.Left.Position = rightPos + new Vector2(this.PositionHelper.GetShiftToCenterVertex(RightWidth, this.Left.Width),
                                                        this.PositionHelper.GetShiftToCenterVertex(RightHeight, this.Left.Height));

            // Do shift to the right side
            Vector2 shift = (Vector2)(this.Left.Position - rightPos);
            if (this.Vertical == VerticalPlace.TOP)
                shift.Y *= -1;
            if (this.Horizontal == HorizontalPlace.LEFT)
                shift.X *= -1;

            this.Left.Position += shift;
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            var cornerEdge = (InCornerEdge)edge;
            if (cornerEdge.Horizontal != this.Horizontal || cornerEdge.Vertical != this.Vertical)
                return null;

            return this.ResolveConflictWithGivenEdge(edge.Left, new ToLeftEdge());
        }
    }
}
