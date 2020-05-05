using ImageGeneratorInterfaces.Edges;
using System.Numerics;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "in middle", etc. relations
    /// </summary>
    public class InMiddleEdge : AbsoluteEdge
    {
        // Max left width
        private int MaxWidth { get => this.Right.Width / 2; }

        // Max left height
        private int MaxHeight { get => this.Right.Height / 2; }

        public InMiddleEdge() : base(PlaceType.MIDDLE) { }

        protected override void PositionateAgainstRoot(int maxWidth, int maxHeight)
        {
            this.Left.Position = new Vector2(this.PositionHelper.GetShiftToCenterVertex(maxWidth, this.Left.Width),
                                             this.PositionHelper.GetShiftToCenterVertex(maxHeight, this.Left.Height));
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
            this.Left.Position = this.Right.Position
                               + new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width),
                                             this.PositionHelper.GetShiftToCenterVertex(this.Right.Height, this.Left.Height));
        }

        public override IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            return this.ResolveConflictWithGivenEdge(edge.Left, new ToLeftEdge());
        }
    }
}
