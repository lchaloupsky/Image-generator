using System.Numerics;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Represents "under", etc. relations
    /// </summary>
    public class UnderEdge : Edge
    {
        // Flag if the relation is reversed
        public bool IsReversed { get; private set; }

        public UnderEdge()
        {
            this.IsReversed = false;
        }

        public UnderEdge(bool reversed)
        {
            this.IsReversed = true;
        }

        protected override void PositionateRight(int maxWidth, int maxHeight)
        {
            if (this.IsReversed)
            {
                this.IsReversed = false;
                this.SwitchVertices();
                this.PositionateLeft(maxWidth, maxHeight);
                return;
            }

            this.Right.Position = this.Left.Position - new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width), this.Right.Height);
        }

        protected override void PositionateLeft(int maxWidth, int maxHeight)
        {
            if (this.IsReversed)
            {
                this.IsReversed = false;
                this.SwitchVertices();
                this.PositionateRight(maxWidth, maxHeight);
                return;
            }

            this.Left.Position = this.Right.Position + new Vector2(this.PositionHelper.GetShiftToCenterVertex(this.Right.Width, this.Left.Width), this.Right.Height);
        }
    }
}
