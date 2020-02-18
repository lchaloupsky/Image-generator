using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class Positioner
    {
        private PositionHelper Helper { get; }

        public Positioner()
        {
            this.Helper = new PositionHelper();
        }

        public void Positionate(SentenceGraph graph, int width, int height)
        {
            this.Helper.SetProperties(width, height);
            bool isPositioned = false;
            while (!isPositioned)
            {
                isPositioned = true;
                foreach (var vertex in graph.Vertices)
                {
                    // check conflicts --->
                    // --->   repositionate
                    if (vertex.IsPositioned)
                    {
                        var conflicts = this.Helper.GetConflictingVertices(vertex, graph.Vertices);
                        if (conflicts.Count != 0)
                        {
                            isPositioned = false;
                            foreach (var conflictVertex in conflicts) //move conflicts to the right
                                conflictVertex.Position += new Vector2(vertex.Width, 0);
                        }
                    }

                    foreach (var edge in graph[vertex])
                    {
                        // in future remove this --> now only because theese edges are not supported in positioning
                        if (edge.Right == null)
                            continue;

                        // Assign new free position to the right vertex -- maybe in the edges?
                        if (!edge.Right.IsPositioned && !edge.Left.IsPositioned)
                            edge.Right.Position = this.Helper.GetEmptyPosition();

                        if (!edge.CheckPosition())
                            edge.Positionate(width, height);
                    }
                }
            }

            this.CenterVertices(graph.Vertices, width, height);
        }

        /// <summary>
        /// Help function for centering vertices in image
        /// </summary>
        /// <param name="vertices">Vertices to be centered</param>
        /// <param name="width">Maximal width</param>
        /// <param name="height">Maximal height</param>
        private void CenterVertices(IEnumerable<IDrawable> vertices, int width, int height)
        {
            IDrawable leftMost = vertices.First(),
                      rightMost = leftMost,
                      topMost = leftMost,
                      bottomMost = leftMost;

            // get most edgy vertices
            foreach (var vertex in vertices)
            {
                int vX = (int)vertex.Position.Value.X;
                int vY = (int)vertex.Position.Value.Y;

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
            Vector2 finalShift = new Vector2(this.GetPaddingX(leftMost, rightMost, width), this.GetPaddingY(topMost, bottomMost, height));
            foreach (var vertex in vertices)
                vertex.Position += finalShift;
        }

        /// <summary>
        /// Help function to get shift to center image in X dimension
        /// </summary>
        /// <param name="left">Most left element</param>
        /// <param name="right">Most right element</param>
        /// <param name="width">Maximal image width</param>
        /// <returns>Shift value</returns>
        private float GetPaddingX(IDrawable left, IDrawable right, int width)
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
        private float GetPaddingY(IDrawable top, IDrawable bottom, int height)
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
            float firstDist = Math.Abs(firstPos);
            float secondDist = Math.Abs(max - (secondPos + secondDim));
            float padding = (firstPos >= 0 && secondPos + secondDim <= max) ? (firstDist - secondDist) / 2 : (firstDist + secondDist) / 2;

            return firstDist > secondDist ? (firstPos < 0 ? padding : -padding) : (secondPos + secondDim > max ? -padding : padding);
        }

        /// <summary>
        /// Help function for rescaling vertices
        /// </summary>
        /// <param name="vertices">Verctices to be scaled</param>
        /// <param name="total">Total value</param>
        /// <param name="limit">MAximal allowed value</param>
        private void RescaleVertices(IEnumerable<IDrawable> vertices, int total, int limit)
        {
            float factor = (1f * limit) / total;
            foreach (var vertex in vertices)
            {
                vertex.Position *= factor;
                vertex.Position = new Vector2((float)Math.Floor(vertex.Position.Value.X), (float)Math.Floor(vertex.Position.Value.Y));
                vertex.Width = (int)(vertex.Width * factor);
                vertex.Height = (int)(vertex.Height * factor);
            }
        }
    }

    class PositionHelper
    {
        public int Width { get; set; }
        public int Height { get; set; }

        private const int defaultWidth = 240;
        private const int defaultHeight = 120;

        private Vector2? NewEmptyPosition { get; set; } = null;


        public Vector2? GetEmptyPosition()
        {
            if (this.NewEmptyPosition == null)
                return this.NewEmptyPosition = new Vector2(this.Width / 2 - (defaultWidth / 2), 2 * this.Height / 3);

            //Stupid move to the right
            this.NewEmptyPosition += new Vector2(defaultWidth, 0);
            return this.NewEmptyPosition;
        }

        public List<IDrawable> GetConflictingVertices(IDrawable vertex, IEnumerable<IDrawable> vertices)
        {
            List<IDrawable> conflicts = new List<IDrawable>();
            foreach (var vert in vertices)
            {
                if (vert == vertex)
                    continue;

                if (this.CheckConflict(vert, vertex))
                    conflicts.Add(vert);
            }

            return conflicts;
        }

        private bool CheckConflict(IDrawable vertex1, IDrawable vertex2)
        {
            return vertex1.Position == vertex2.Position;
        }

        public void SetProperties(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.NewEmptyPosition = null;
        }
    }
}
