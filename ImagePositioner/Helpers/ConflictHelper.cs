using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Helpers
{
    /// <summary>
    /// Helper class for resolving conflicts
    /// </summary>
    class ConflictHelper
    {
        // default shift
        private const int defaultShiftPadding = 60;

        /// <summary>
        /// Return vertices of graph that are in conflict
        /// </summary>
        /// <param name="vertex">Vertex to check</param>
        /// <param name="vertices">All vertices</param>
        /// <returns>Conflicting vertices</returns>
        public List<IDrawable> GetConflictingVertices(IDrawable vertex, IEnumerable<IDrawable> vertices)
        {
            List<IDrawable> conflicts = new List<IDrawable>();
            foreach (var vert in vertices)
            {
                // skip vertex itself and fixed vertices
                if (vert == vertex || vert.IsFixed != vertex.IsFixed)
                    continue;

                // check conflict
                if (this.CheckConflict(vert, vertex))
                    conflicts.Add(vert);
            }

            return conflicts;
        }

        /// <summary>
        /// Resolves absolute conflicts
        /// </summary>
        /// <param name="left">Left vertex</param>
        /// <param name="right">Right vertex</param>
        /// <param name="leftPlace">Left place type</param>
        /// <param name="rightPlace">Right place type</param>
        /// <param name="width">Max width</param>
        /// <param name="height">Max height</param>
        public void ResolveAbsoluteConflict(IDrawable left, IDrawable right, PlaceType leftPlace, PlaceType rightPlace, int width, int height)
        {
            // Resolve conflicts by place type
            // Corner cofnlicts
            if (leftPlace == PlaceType.CORNER)
            {
                if (rightPlace == PlaceType.CORNER)
                    this.ResolveCornerConflict(left, right, width, height);

                else if (rightPlace == PlaceType.VERTICAL)
                    this.ResolveVerticalConflict(left, right);

                else
                    this.ResolveHorizontalConflict(left, right);
            }

            // Horizontal conflicts
            if (leftPlace == PlaceType.HORIZONTAL)
            {
                if (rightPlace == PlaceType.MIDDLE)
                    this.ResolveVerticalConflict(left, right);
                else
                    this.ResolveHorizontalConflict(left, right);
            }

            // Middle conflicts
            if (leftPlace == PlaceType.MIDDLE)
                this.ResolveHorizontalConflict(left, right);

            // Vertical conflicts
            if (leftPlace == PlaceType.VERTICAL)
            {
                if (rightPlace == PlaceType.MIDDLE)
                    this.ResolveHorizontalConflict(left, right);
                else
                    this.ResolveVerticalConflict(left, right);
            }
        }

        /// <summary>
        /// Method for resolving conflicts
        /// </summary>
        /// <param name="vertex">Vertex to resolve</param>
        /// <param name="conflictVertex">Conflict vertex</param>
        public void ResolveHorizontalConflict(IDrawable vertex, IDrawable conflictVertex)
        {
            // Resolving by both vertices shifting by a half
            IDrawable left = vertex.Position.Value.X <= conflictVertex.Position.Value.X ? vertex : conflictVertex;
            IDrawable right = left == vertex ? conflictVertex : vertex;

            // shift
            Vector2 overlap = GetOverlapX(left, right);
            left.Position -= overlap / 2;
            right.Position += overlap / 2;
        }

        /// <summary>
        /// Method for resolving conflicts
        /// </summary>
        /// <param name="vertex">Vertex to resolve</param>
        /// <param name="conflictVertex">Conflict vertex</param>
        public void ResolveVerticalConflict(IDrawable vertex, IDrawable conflictVertex)
        {
            // Resolving by both vertices shifting by a half
            IDrawable left = vertex.Position.Value.Y <= conflictVertex.Position.Value.Y ? vertex : conflictVertex;
            IDrawable right = left == vertex ? conflictVertex : vertex;

            // shift
            Vector2 overlap = GetOverlapY(left, right);
            left.Position -= overlap / 2;
            right.Position += overlap / 2;
        }

        /// <summary>
        /// Method for resolving conflicts
        /// </summary>
        /// <param name="vertex">Vertex to resolve</param>
        /// <param name="conflictVertex">Conflict vertex</param>
        public void ResolveCornerConflict(IDrawable vertex, IDrawable conflictVertex, int width, int height)
        {
            var vertexPosition = vertex.Position.Value;
            var conflictPosition = vertex.Position.Value;

            // Horizontal conflicts
            if ((vertexPosition.Y == 0 && conflictPosition.Y == 0)
                || (vertexPosition.Y + vertex.Height == height && conflictPosition.Y + conflictVertex.Height == height
                    && vertex.Height != height && conflictVertex.Height != height))
                ResolveHorizontalConflict(vertex, conflictVertex);

            // Vertical conflicts
            if ((vertexPosition.X == 0 && conflictPosition.X == 0)
                || (vertexPosition.X + vertex.Width == width && conflictPosition.X + conflictVertex.Width == width))
                ResolveVerticalConflict(vertex, conflictVertex);
        }

        /// <summary>
        /// Method that return needed shift
        /// </summary>
        /// <param name="vertex1">First vertex</param>
        /// <param name="vertex2">Second vertex</param>
        /// <returns>Vector that represents the shift</returns>
        public Vector2 GetShift(IDrawable vertex1, IDrawable vertex2)
        {
            return new Vector2(vertex1.Position.Value.X + vertex1.Width - vertex2.Position.Value.X + defaultShiftPadding, 0);
        }

        /// <summary>
        /// Method that return how much are vertices overlapping in X dimension
        /// </summary>
        /// <param name="vertex1">First vertex</param>
        /// <param name="vertex2">Second vertex</param>
        /// <returns>Overlap vector</returns>
        public Vector2 GetOverlapX(IDrawable vertex1, IDrawable vertex2)
        {
            return new Vector2((float)Math.Ceiling(vertex1.Position.Value.X + vertex1.Width - vertex2.Position.Value.X + defaultShiftPadding), 0);
        }

        /// <summary>
        /// Method that return how much are vertices overlapping in X dimension
        /// </summary>
        /// <param name="vertex1">First vertex</param>
        /// <param name="vertex2">Second vertex</param>
        /// <returns>Overlap vector</returns>
        public Vector2 GetOverlapY(IDrawable vertex1, IDrawable vertex2)
        {
            return new Vector2(0, (float)Math.Ceiling(vertex1.Position.Value.Y + vertex1.Height - vertex2.Position.Value.Y));
        }

        /// <summary>
        /// Method that determines if vertices are in conflict
        /// </summary>
        /// <param name="vertex1">First vertex</param>
        /// <param name="vertex2">Second vertex</param>
        /// <returns>True if they are in conflict</returns>
        public bool CheckConflict(IDrawable vertex1, IDrawable vertex2)
        {
            // initial assigns
            float x1 = vertex1.Position.Value.X;
            float y1 = vertex1.Position.Value.Y;

            float x2 = vertex2.Position.Value.X;
            float y2 = vertex2.Position.Value.Y;

            // If in z-axis are distinct
            if (vertex1.ZIndex != vertex2.ZIndex)
                return false;

            // If in x-axis are distinct
            if (x1 >= x2 + vertex2.Width || x1 + vertex1.Width <= x2)
                return false;

            // If in y-axis are distinct
            if (y1 >= y2 + vertex2.Height || y1 + vertex1.Height <= y2)
                return false;

            // they overlap
            return true;
        }
    }
}
