﻿using ImageGeneratorInterfaces.Edges;
using System.Numerics;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Class representing "at bottom", "at bottom of" relation
    /// </summary>
    public class AtBottomEdge : AbsoluteEdge
    {
        // Max width of left vertex
        private int MaxWidth => this.Right.Width / 2;

        // Max height of left vertex
        private int MaxHeight => this.Right.Height / 2;

        public AtBottomEdge() : base(PlaceType.HORIZONTAL) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(this.PositionHelper.GetShiftToCenterVertex(maxWidth, this.Left.Width), maxHeight - this.Left.Height);
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.CopyPosition(this.Left, this.Right);
            this.PositionateLeft(MaxWidth, maxHeight);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.PositionHelper.RescaleWithMax(this.MaxWidth, this.Left.Width, this.Left);
            this.PositionHelper.RescaleWithMax(this.MaxHeight, this.Left.Height, this.Left);
            this.Left.ZIndex++;
            this.Left.Position = this.Right.Position + new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width), this.Right.Height - this.Left.Height);
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            return this.ResolveConflictWithGivenEdge(edge.Left, new ToLeftEdge());
        }
    }
}
