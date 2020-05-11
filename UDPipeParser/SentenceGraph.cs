using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System.Collections.Generic;
using System.Linq;
using UDPipeParsing.Text_elements;

namespace UDPipeParsing
{
    /// <summary>
    /// Class representing sentence graph
    /// Implementation of ISentenceGraph interface
    /// </summary>
    public class SentenceGraph : ISentenceGraph
    {
        // Graph is represented by dictionary
        private Dictionary<IDrawable, List<IPositionateEdge>> Graph { get; }

        // List of absolute edges
        // We keep this for resolving(reconnecting) same absolute conflicts in the graphs.
        private List<IAbsolutePositionateEdge> AbsoluteEdges { get; } = new List<IAbsolutePositionateEdge>();

        public IEnumerable<IDrawable> Groups { get; set; } = new List<IDrawable>();
        public IEnumerable<IDrawable> Vertices => this.Graph.Keys;
        public IEnumerable<IPositionateEdge> Edges => this.Graph.Values.SelectMany(edge => edge);
        public IEnumerable<IPositionateEdge> this[IDrawable vertex] => vertex != null && this.Graph.ContainsKey(vertex) ? this.Graph[vertex] : null;

        public SentenceGraph()
        {
            this.Graph = new Dictionary<IDrawable, List<IPositionateEdge>>();
        }

        /// <summary>
        /// Adds new edge into a graph
        /// </summary>
        /// <param name="edge">Edge to add</param>
        public void AddEdge(IPositionateEdge edge)
        {
            if (!this.Graph.ContainsKey(edge.Left))
                this.Graph.Add(edge.Left, new List<IPositionateEdge>());

            if (edge.Right != null && !this.Graph.ContainsKey(edge.Right))
                this.Graph.Add(edge.Right, new List<IPositionateEdge>());

            if (edge is IAbsolutePositionateEdge && edge.Right == null)
                edge = AddAbsoluteEdge((IAbsolutePositionateEdge)edge);

            this.Graph[edge.Left].Add(edge);
        }

        /// <summary>
        /// Adds new vertex into the graph
        /// </summary>
        /// <param name="vertex">vertex to add</param>
        public void AddVertex(IDrawable vertex)
        {
            if (this.Graph.ContainsKey(vertex))
                return;

            this.Graph.Add(vertex, new List<IPositionateEdge>());
        }

        /// <summary>
        /// Removes vertex and all its edges from graph
        /// </summary>
        /// <param name="vertex">Vertex to remove</param>
        public void RemoveVertex(IDrawable vertex, bool removeRecursive = false)
        {
            if (!this.Graph.ContainsKey(vertex))
                return;

            if (vertex is NounSet ns)
                ns.Nouns.ForEach(n => this.RemoveVertex(n, removeRecursive));

            if (removeRecursive)
            {
                var vertices = this.Graph[vertex].Select(edge => edge.Right).ToList();
                for (int i = 0; i < vertices.Count(); i++)
                    this.RemoveVertex(vertices[i], true);             
            }

            this.Graph.Remove(vertex);
            foreach (var edges in this.Graph.Values)
                edges.RemoveAll(edge => edge.Right == vertex);

            vertex.Dispose();
        }

        /// <summary>
        /// Replaces vertex with given replace vertex. 
        /// Also reconnects all edges with this vertex to the new vertex
        /// </summary>
        /// <param name="vertex">Vertex</param>
        /// <param name="vertexToReplace">Vertex to replace</param>
        public void ReplaceVertex(IDrawable vertex, IDrawable vertexToReplace)
        {
            if (!this.Graph.ContainsKey(vertexToReplace))
                return;

            // Get all edges belonging to the vertex
            var newEdges = this.Edges
                .Where(e => e.Left.Equals(vertexToReplace))
                .Select(e => { e.Left = vertex; return e; })
                .ToList();

            // Reconnect edges with given vertex
            this.Edges.Where(e => e.Right.Equals(vertexToReplace))
                       .Select(e => { e.Right = vertex; return e; })
                       .ToList();

            // Take its edges as edges from this vertex
            this.RemoveVertex(vertexToReplace);
            newEdges.ForEach(e => this.AddEdge(e));
        }

        /// <summary>
        /// Disposing graph
        /// </summary>
        public void Dispose()
        {
            foreach (var vertex in this.Vertices)
            {
                if (vertex.Image != null)
                    vertex.Dispose();
            }

            foreach (var group in this.Groups)
            {
                if (group.Image != null)
                    group.Dispose();
            }
        }

        /// <summary>
        /// Clears whole graph
        /// </summary>
        public void Clear()
        {
            Dispose();
            this.Graph.Clear();
            this.AbsoluteEdges.Clear();
        }

        /// <summary>
        /// Method for adding absolute edges
        /// </summary>
        /// <param name="edge"></param>
        /// <returns>Edge to add into a graph</returns>
        private IPositionateEdge AddAbsoluteEdge(IAbsolutePositionateEdge edge)
        {
            foreach (var absEdge in this.AbsoluteEdges)
            {
                if (absEdge.GetType() == edge.GetType())
                {
                    // Edge will return us which edge we should add into graph
                    var newEdge = absEdge.ResolveConflict(edge);
                    if (newEdge == null)
                        continue;

                    return newEdge;
                }
            }

            this.AbsoluteEdges.Add(edge);
            return edge;
        }
    }
}
