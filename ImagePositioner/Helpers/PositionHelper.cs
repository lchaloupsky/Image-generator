using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ImagePositioner.Helpers
{
    /// <summary>
    /// Helper for position and dimension operations
    /// </summary>
    public class PositionHelper
    {
        // default shift
        private const int defaultShiftPadding = 60;

        // Default object dimensions
        private int LastObjectWidth { get; set; }
        private int LastObjectHeight { get; set; }

        // screen dimensions
        public int Width { get; set; }
        public int Height { get; set; }

        // other properties
        private Random Random { get; } = new Random();
        private Vector2? NewEmptyPosition { get; set; } = null;

        /// <summary>
        /// Method that returns new empty position
        /// </summary>
        /// <returns>new empty position</returns>
        public Vector2? GetEmptyPosition(IDrawable vertex)
        {
            // If no position was given yet, return a center position
            if (this.NewEmptyPosition == null)
            {
                this.LastObjectHeight = vertex.Height;
                this.LastObjectWidth = vertex.Width;
                return this.NewEmptyPosition = new Vector2(this.Width / 2 - (vertex.Width / 2), 2 * this.Height / 3);
            }

            //Stupid move to the right
            this.NewEmptyPosition += new Vector2(this.LastObjectWidth + Random.Next(0, defaultShiftPadding) + 1,
                                                 Random.Next(-defaultShiftPadding, defaultShiftPadding));
            // Return new position
            return this.NewEmptyPosition;
        }

        /// <summary>
        /// Method that sets properties of helper
        /// </summary>
        /// <param name="width">screen width</param>
        /// <param name="height">screen height</param>
        public void SetProperties(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.NewEmptyPosition = null;
        }

        /// <summary>
        /// Help function for centering vertices in image
        /// </summary>
        /// <param name="vertices">Vertices to be centered</param>
        /// <param name="width">Maximal width</param>
        /// <param name="height">Maximal height</param>
        public void CenterVertices(IEnumerable<IDrawable> vertices, int width, int height, bool absolute = false)
        {
            if (vertices.Count() == 0)
                return;

            // set inital vertices
            IDrawable leftMost = vertices.First(),
                      rightMost = leftMost,
                      topMost = leftMost,
                      bottomMost = leftMost;

            // get most edgy vertices
            foreach (var vertex in vertices)
            {
                // skip fixed vertices
                int vX = (int)vertex.Position.Value.X;
                int vY = (int)vertex.Position.Value.Y;

                // check most edgy vertices
                leftMost = leftMost.Position.Value.X < vX ? leftMost : vertex;
                rightMost = rightMost.Position.Value.X + rightMost.Width > vX + vertex.Width ? rightMost : vertex;
                topMost = topMost.Position.Value.Y < vY ? topMost : vertex;
                bottomMost = bottomMost.Position.Value.Y + bottomMost.Height > vY + vertex.Height ? bottomMost : vertex;
            }

            // if total vertices width is > width --> rescale
            int totalW = (int)(rightMost.Position.Value.X + rightMost.Width - leftMost.Position.Value.X);
            if (totalW > width)
                this.RescaleVertices(vertices, totalW, width);

            // if total vertices height is > height --> rescale
            int totalH = (int)(bottomMost.Position.Value.Y + bottomMost.Height - topMost.Position.Value.Y);
            if (totalH > height)
                this.RescaleVertices(vertices, totalH, height);

            // adding final shift to center vertices
            Vector2 finalShift;
            if (!absolute)
                finalShift = new Vector2(this.GetPaddingX(leftMost, rightMost, width), this.GetPaddingY(topMost, bottomMost, height));
            else
                finalShift = new Vector2(Math.Abs(Math.Min(leftMost.Position.Value.X, 0)), Math.Abs(Math.Min(topMost.Position.Value.Y, 0)));

            // apply final shift
            foreach (var vertex in vertices)
            {
                // shift vertex
                vertex.Position += finalShift;
            }
        }

        /// <summary>
        /// Rescales and fits absolute vertices to the max width and height
        /// </summary>
        /// <param name="vertices">Vertices to fit</param>
        /// <param name="width">Max width</param>
        /// <param name="height">Max height</param>
        public void RescaleAndFitAbsoluteVertices(IEnumerable<IDrawable> vertices, int width, int height)
        {
            // Rescale vertices if they are bigger than borders
            foreach (var vertex in vertices)
            {
                if (vertex.Width > width)
                    RescaleVertex(vertex, width / (vertex.Width * 1f));

                if (vertex.Height > height)
                    RescaleVertex(vertex, height / (vertex.Height * 1f));

                vertex.Position = new Vector2(Math.Abs(Math.Max(vertex.Position.Value.X, 0)), Math.Abs(Math.Max(vertex.Position.Value.Y, 0)));
            }
        }

        /// <summary>
        /// Help function to get shift to center image in X dimension
        /// </summary>
        /// <param name="left">Most left element</param>
        /// <param name="right">Most right element</param>
        /// <param name="width">Maximal image width</param>
        /// <returns>Shift value</returns>
        public float GetPaddingX(IDrawable left, IDrawable right, int width)
        {
            return GetPadding(left.Position.Value.X, right.Position.Value.X, right.Width, width);
        }

        /// <summary>
        /// Help function to get shift to center image in Y dimension
        /// </summary>
        /// <param name="top">Most left element</param>
        /// <param name="bottom">Most right element</param>
        /// <param name="height">Maximal image width</param>
        /// <returns>Shift value</returns>
        public float GetPaddingY(IDrawable top, IDrawable bottom, int height)
        {
            return GetPadding(top.Position.Value.Y, bottom.Position.Value.Y, bottom.Height, height);
        }

        /// <summary>
        /// Help function for calculation shift
        /// </summary>
        /// <param name="firstPos">first position</param>
        /// <param name="secondPos">Second position</param>
        /// <param name="secondDim">Second dimension value</param>
        /// <param name="max">Maximal value</param>
        /// <returns>Calculated shift</returns>
        private float GetPadding(float firstPos, float secondPos, float secondDim, int max)
        {
            if (firstPos == secondPos)
                return firstPos < 0 ? (max - secondDim) / 2 - firstPos : (max - secondDim) / 2 - firstPos;

            float firstDist = Math.Abs(firstPos);
            float secondDist = Math.Abs(max - (secondPos + secondDim));
            float padding = (firstPos > 0 && secondPos + secondDim <= max) ? (firstDist - secondDist) / 2 : (firstDist + secondDist) / 2;

            return firstDist > secondDist ? (firstPos < 0 ? padding : -padding) : (secondPos + secondDim > max ? -padding : padding);
        }

        /// <summary>
        /// Gets shift of dimensions to center vertex
        /// </summary>
        /// <param name="dim1">First vertex dimension</param>
        /// <param name="dim2">Second vertex dimension</param>
        /// <returns>Shift</returns>
        public int GetShiftToCenterVertex(int dim1, int dim2)
        {
            return (int)Math.Round((dim1 - dim2) / 2.0f, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Returns scale of dimension to the max 
        /// </summary>
        /// <param name="max">Maximal dimension value</param>
        /// <param name="dim">Dimension</param>
        /// <returns>Scale from dim to max</returns>
        public float GetScaleFactor(int max, int dim)
        {
            return (max * 1f) / dim;
        }

        /// <summary>
        /// Help function for rescaling vertices
        /// </summary>
        /// <param name="vertices">Verctices to be scaled</param>
        /// <param name="total">Total value</param>
        /// <param name="limit">MAximal allowed value</param>
        public void RescaleVertices(IEnumerable<IDrawable> vertices, int total, int limit)
        {
            float factor = GetScaleFactor(limit, total);

            // rescale all vertices with factor
            foreach (var vertex in vertices)
                RescaleVertex(vertex, factor);
        }

        /// <summary>
        /// Rescales vertex dimensions and position by given factor
        /// </summary>
        /// <param name="vertex">Vertex</param>
        /// <param name="factor">Factor</param>
        public void RescaleVertex(IDrawable vertex, float factor)
        {
            this.RescaleVertexDimensions(vertex, factor);
            this.RescaleVertexPosition(vertex, factor);
        }

        /// <summary>
        /// Rescales vertex given max value
        /// </summary>
        /// <param name="max">Maximum</param>
        /// <param name="value">Actual value</param>
        /// <param name="vertex">Element to Rescale</param>
        public void RescaleWithMax(int max, int value, IDrawable vertex)
        {
            var rescale = this.GetScaleFactor(max, value);
            if (rescale < 1)
                this.RescaleVertexDimensions(vertex, rescale);
        }

        /// <summary>
        /// Rescales vertex dimensions by factor
        /// </summary>
        /// <param name="vertex">Vertex to rescale</param>
        /// <param name="factor">Factor</param>
        public void RescaleVertexDimensions(IDrawable vertex, float factor)
        {
            vertex.Width = (int)(vertex.Width * factor);
            vertex.Height = (int)(vertex.Height * factor);
        }

        /// <summary>
        /// Rescales vertex position by factor
        /// </summary>
        /// <param name="vertex">Vertex</param>
        /// <param name="factor">Factor</param>
        public void RescaleVertexPosition(IDrawable vertex, float factor)
        {
            vertex.Position *= factor;
            vertex.Position = new Vector2((float)Math.Floor(vertex.Position.Value.X), (float)Math.Floor(vertex.Position.Value.Y));
        }
    }
}
