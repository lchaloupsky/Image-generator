using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImagePositioner.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace ImagePositioner
{
    /// <summary>
    /// Class to positioning elements in graph
    /// </summary>
    public class Positioner
    {
        // Position helper
        private PositionHelper PositionHelper { get; }

        // Helper for resolving conflicts
        private ConflictHelper ConflictHelper { get; }

        public Positioner()
        {
            this.PositionHelper = new PositionHelper();
            this.ConflictHelper = new ConflictHelper();
        }

        /// <summary>
        /// Method that positionates graph
        /// </summary>
        /// <param name="graph">graph to positionate</param>
        /// <param name="width">screen with</param>
        /// <param name="height">screen height</param>
        public void Positionate(ISentenceGraph graph, int width, int height)
        {
            // Setting helpers properties
            this.PositionHelper.SetProperties(width, height);

            // Positionate all Edges first
            this.PositionateAllEdges(graph, width, height);

            // Resolving final conflicts
            this.ResolveConflicts(graph, width, height);

            // Final not absolute positioned vertices centering
            this.PositionHelper.CenterVertices(graph.Groups.Where(g => !g.IsFixed), width, height);

            // OPT 1. ?
            // Center and fit absolute vertices
            this.PositionHelper.CenterVertices(graph.Groups.Where(g => g.IsFixed), width, height, true);

            // OPT 2. ?
            // Rescale absolute vertices
            //this.PositionHelper.RescaleAndFitAbsoluteVertices(graph.Groups.Where(g => g.IsFixed), width, height);
        }

        /// <summary>
        /// Method for resolving conflicts between vetices
        /// </summary>
        /// <param name="graph">Graph to resolve</param>
        /// <param name="width">screen width</param>
        /// <param name="height">screen height</param>
        private void ResolveConflicts(ISentenceGraph graph, int width, int height)
        {
            // positionate absolute edges and get absolute groups
            var absoluteGroups = this.PositionateAbsoluteEdges(graph, width, height);
            bool isPositionedCorrectly = false;

            // While there is some conflict, check positions
            while (!isPositionedCorrectly)
            {
                isPositionedCorrectly = true;
                foreach (var vertex in graph.Groups)
                {
                    // skip unpositioned vertices
                    if (!vertex.IsPositioned)
                        continue;

                    // check conflicts ---> repositionate(move)
                    var conflicts = this.ConflictHelper.GetConflictingVertices(vertex, graph.Groups);

                    // continue if no conflicts were found
                    if (conflicts.Count == 0)
                        continue;

                    // -------resolve conflicts--------
                    isPositionedCorrectly = false;
                    foreach (var conflictVertex in conflicts)
                    {
                        // resolve absolute conflicts
                        if (vertex.IsFixed)
                            this.ConflictHelper.ResolveAbsoluteConflict(vertex, conflictVertex, absoluteGroups[vertex], absoluteGroups[conflictVertex], width, height);
                        // move conflicts to the right
                        else
                            conflictVertex.Position += this.ConflictHelper.GetShift(vertex, conflictVertex);
                    }
                }
            }
        }

        /// <summary>
        /// Postionates all absolute edges
        /// </summary>
        /// <param name="graph">Sentence graph</param>
        /// <param name="width">Max width</param>
        /// <param name="height">Max height</param>
        /// <returns>Absolute groups of vertices</returns>
        private Dictionary<IDrawable, PlaceType> PositionateAbsoluteEdges(ISentenceGraph graph, int width, int height)
        {
            var dict = new Dictionary<IDrawable, PlaceType>();

            // positionate all absolute relations
            foreach (IAbsolutePositionateEdge absEdge in graph.Edges.Where(edge => edge is IAbsolutePositionateEdge && edge.Right == null))
            {
                absEdge.Positionate(width, height);
                if (!dict.ContainsKey(absEdge.Left))
                    dict.Add(absEdge.Left, absEdge.Type);
            }

            return dict;
        }

        /// <summary>
        /// Method that positionates all edges of graph
        /// </summary>
        /// <param name="graph">Graph to positionate</param>
        /// <param name="width">screen width</param>
        /// <param name="height">screen height</param>
        private void PositionateAllEdges(ISentenceGraph graph, int width, int height)
        {
            // positionate all vertices
            foreach (var vertex in graph.Vertices)
            {
                // skip if already positioned
                if (vertex.IsPositioned)
                    continue;

                // positionate via DFS
                this.PositionateDFS(graph, vertex, width, height);

                // assign random new position if vertex is independent
                if (vertex.Position == null)
                    vertex.Position = this.PositionHelper.GetEmptyPosition(vertex);
            }

            // Getting final vertex "groups" after positioning
            HashSet<IDrawable> groups = new HashSet<IDrawable>();
            foreach (var vertex in graph.Vertices)
            {
                // If vertex is group as itself, insert it into groups.
                if (vertex.Group == null)
                {
                    groups.Add(vertex);
                    continue;
                }

                // add only unique and valid groups.
                if (vertex.Group.IsValid && !groups.Contains(vertex.Group))
                    groups.Add(vertex.Group);
            }

            // Set groups of graph
            graph.Groups = groups;
        }

        /// <summary>
        /// Method for positionate graph via DFS pass
        /// </summary>
        /// <param name="graph">Graph to positionate</param>
        /// <param name="vertex">Vertex to positionate</param>
        /// <param name="width">sreen width</param>
        /// <param name="height">screen height</param>
        private void PositionateDFS(ISentenceGraph graph, IDrawable vertex, int width, int height)
        {
            // positionate all edges of vertex
            foreach (var edge in graph[vertex])
            {
                // skip absolute edges for now
                if (edge is IAbsolutePositionateEdge && edge.Right == null)
                    continue;

                // Positionate dfs
                if (edge.Right != null && !edge.Right.IsPositioned)
                    this.PositionateDFS(graph, edge.Right, width, height);

                // Assign new free position to the right vertex
                if (edge.Right != null && !edge.Right.IsPositioned && !edge.Left.IsPositioned)
                    edge.Right.Position = this.PositionHelper.GetEmptyPosition(edge.Right);

                // Edge positioning itself
                edge.Positionate(width, height);
            }
        }
    }
}
