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
    /// <summary>
    /// Represents "to left", etc. relations
    /// </summary>
    public class ToLeftEdge : AbsoluteEdge
    {
        public ToLeftEdge() : base(PlaceType.VERTICAL) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(0, this.PositionHelper.GetShiftToCenterVertex(maxHeight ,this.Left.Height));
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            this.Right.Position = this.Left.Position + new Vector2(this.Left.Width, this.PositionHelper.GetShiftToCenterVertex(this.Left.Height, this.Right.Height));
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            this.Left.Position = this.Right.Position - new Vector2(this.Left.Width, this.PositionHelper.GetShiftToCenterVertex(this.Left.Height, this.Right.Height));
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            return this.ResolveConflictWithGivenEdge(edge.Left, new UnderEdge());
        }
    }
}
