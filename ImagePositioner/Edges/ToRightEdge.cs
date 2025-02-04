﻿using ImageGeneratorInterfaces.Edges;
using System.Numerics;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "to right", etc. relations
    /// </summary>
    public class ToRightEdge : AbsoluteEdge
    {
        public ToRightEdge() : base(PlaceType.VERTICAL) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(maxWidth - this.Left.Width, this.PositionHelper.GetShiftToCenterVertex(maxHeight, this.Left.Height));
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position - new Vector2(this.Right.Width, this.PositionHelper.GetShiftToCenterVertex(this.Right.Height, this.Left.Height));
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position + new Vector2(this.Right.Width, this.PositionHelper.GetShiftToCenterVertex(this.Right.Height, this.Left.Height));
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            return this.ResolveConflictWithGivenEdge(edge.Left, new UnderEdge());
        }
    }
}
