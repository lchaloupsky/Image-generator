using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImagePositioner.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Abstract edge as base class for all edges
    /// </summary>
    abstract class Edge : IPositionateEdge
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // left vertex
        public IDrawable Left { get; set; }

        // right vertex
        public IDrawable Right { get; set; }

        // position and dimension helper
        protected PositionHelper PositionHelper { get; } = new PositionHelper();

        /// <summary>
        /// Add vertices to the edge
        /// </summary>
        /// <param name="left">Left vertex</param>
        /// <param name="right">Right vertex</param>
        public void Add(IDrawable left, IDrawable right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Interface method for positioning
        /// </summary>
        /// <param name="maxWidth">Max image width</param>
        /// <param name="maxHeight">Max image height</param>
        public virtual void Positionate(int maxWidth, int maxHeight)
        {
            // If left has already group, use it 
            if (this.Left.Group != null)
                this.Left = this.Left.Group;

            // Positionate absolute edge
            if (this is AbsoluteEdge && this.Right == null)
            {
                this.PositionateAgainstRoot(maxWidth, maxHeight);
                this.Left.IsFixed = true;
                return;
            }

            // If right has already grou, use it
            if (this.Right.Group != null)
                this.Right = this.Right.Group;

            // Positionate non positioned vertex
            if (!this.Left.IsPositioned)
                this.PositionateLeft(maxWidth, maxHeight);
            else if (!this.Right.IsPositioned)
                this.PositionateRight(maxWidth, maxHeight);
            else
                this.PositionateLeft(maxWidth, maxHeight);

            // Fix both vertices into one group.
            this.Left.CombineIntoGroup(this.Right);

            // Log this positioning
            Logger.Info(this);
        }

        /// <summary>
        /// Method for positioning absolute edges
        /// </summary>
        /// <param name="maxWidth">Max width</param>
        /// <param name="maxHeight">Max height</param>
        protected virtual void PositionateAgainstRoot(int maxWidth, int maxHeight) { }

        /// <summary>
        /// Method for positioning right vertex
        /// </summary>
        /// <param name="maxWidth">Max width</param>
        /// <param name="maxHeight">Max height</param>
        protected abstract void PositionateRight(int maxWidth, int maxHeight);

        /// <summary>
        /// Method for positioning left vertex
        /// </summary>
        /// <param name="maxWidth">Max width</param>
        /// <param name="maxHeight">Max height</param>
        protected abstract void PositionateLeft(int maxWidth, int maxHeight);

        /// <summary>
        /// Method for copying position between vertices
        /// </summary>
        /// <param name="source">Source vector</param>
        /// <param name="destination">Vector to copy position</param>
        protected void CopyPosition(IDrawable source, IDrawable destination)
        {
            destination.Position = source.Position;
            destination.ZIndex = source.ZIndex;
        }

        /// <summary>
        /// Switches edge vertices
        /// </summary>
        protected void SwitchVertices()
        {
            var temp = this.Right;
            this.Right = this.Left;
            this.Left = temp;
        }

        /// <summary>
        /// Overrides standard ToString method
        /// </summary>
        /// <returns>String represenatation</returns>
        public override string ToString()
        {
            return $"{this.GetType().Name} --> positioning: \n     Left: {this.Left} \n    Right: {this.Right}";
        }
    }

    /// <summary>
    /// Enum representing Horizontal placing 
    /// </summary>
    public enum HorizontalPlace
    {
        LEFT, RIGHT
    }

    /// <summary>
    /// Enum representing Vertical placing
    /// </summary>
    public enum VerticalPlace
    {
        TOP, BOTTOM
    }
}
