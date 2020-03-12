using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Edges
{
    /// <summary>
    /// Abstract edge as base class for all edges
    /// </summary>
    abstract class Edge : IPositionateEdge
    {
        public IDrawable Left { get; set; }
        public IDrawable Right { get; set; }

        public Edge Add(IDrawable left, IDrawable right)
        {
            this.Left = left;
            this.Right = right;

            return this;
        }

        public virtual void Positionate(int maxWidth, int maxHeight)
        {
            if (this.Left.Group != null)
                this.Left = this.Left.Group;

            if (this.Right == null)
            {
                this.PositionateAgainstRoot(maxWidth, maxHeight);
                this.Left.IsFixed = true;
                return;
            }

            if (this.Right.Group != null)
                this.Right = this.Right.Group;

            if (!this.Left.IsPositioned)
                this.PositionateLeft(maxWidth, maxHeight);
            else if (!this.Right.IsPositioned)
                this.PositionateRight(maxWidth, maxHeight);
            else
                this.PositionateLeft(maxWidth, maxHeight);
                //Console.WriteLine("COLLISION"); //something like check some condition. then propagate change?

            // Fix both vertices into one group.
            this.Left.CombineIntoGroup(this.Right);

            Console.WriteLine(this); //debug log.
        }

        protected virtual void PositionateAgainstRoot(int maxWidth, int maxHeight) { }

        protected abstract void PositionateRight(int maxWidth, int maxHeight);

        protected abstract void PositionateLeft(int maxWidth, int maxHeight);

        protected int GetShift(int dim1, int dim2)
        {
            return (dim1 - dim2) / 2;
        }

        protected float GetScaleFactor(int max, int dim)
        {
            return max * 1f / dim;
        }

        protected void ScaleByFactor(float factor, IDrawable element)
        {
            element.Width = (int)(element.Width * factor);
            element.Height = (int)(element.Height * factor);
        }

        protected void RescaleWithMax(int max, int value, IDrawable element)
        {
            var rescale = this.GetScaleFactor(max, value);
            if (rescale < 1)
                this.ScaleByFactor(rescale, element);
        }

        protected void CopyPosition(IDrawable source, IDrawable destination)
        {
            destination.Position = source.Position;
            destination.ZIndex = source.ZIndex;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} --> positioning: \n     Left: {this.Left} \n    Right: {this.Right}";
        }
    }

    public enum HorizontalPlace { LEFT, RIGHT }
    public enum VerticalPlace { TOP, BOTTOM }
}
