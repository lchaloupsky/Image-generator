﻿using ImageGeneratorInterfaces.Edges;
using System.Numerics;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "on top", etc. relations
    /// </summary>
    public class OnTopEdge : AbsoluteEdge
    {
        public OnTopEdge() : base(PlaceType.HORIZONTAL) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(this.PositionHelper.GetShiftToCenterVertex(maxWidth, this.Left.Width), 0);
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position + new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Left.Width, this.Right.Width), this.Left.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position - new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Left.Width, this.Right.Width), this.Left.Height);
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            return this.ResolveConflictWithGivenEdge(edge.Left, new ToLeftEdge());
        }
    }
}
