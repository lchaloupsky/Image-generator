using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing
{
    public class SentenceGraph : ISentenceGraph
    {
        private Dictionary<IDrawable, List<IPositionateEdge>> Graph { get; }

        public IEnumerable<IDrawable> Groups { get; set; }
        public IEnumerable<IDrawable> Vertices => this.Graph.Keys;
        public IEnumerable<IPositionateEdge> Edges => this.Graph.Values.SelectMany(edge => edge);
        public IEnumerable<IPositionateEdge> this[IDrawable vertex] => this.Graph.ContainsKey(vertex) ? this.Graph[vertex] : null;

        private List<IAbsolutePositionateEdge> AbsoluteEdges { get; } = new List<IAbsolutePositionateEdge>();

        public SentenceGraph()
        {
            this.Graph = new Dictionary<IDrawable, List<IPositionateEdge>>();
        }

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

        public void AddVertex(IDrawable vertex)
        {
            if (this.Graph.ContainsKey(vertex))
                return;

            this.Graph.Add(vertex, new List<IPositionateEdge>());
        }

        public void RemoveVertex(IDrawable vertex)
        {
            this.Graph.Remove(vertex);
        }

        public void ReplaceVertex(IDrawable vertex, IDrawable vertexToReplace)
        {
            // Get all edges belonging to the vertex
            var newEdges = this.Edges
                .Where(e => e.Left.Equals(vertexToReplace))
                .Select(e => { e.Left = vertex; return e; })
                .ToList();

            this.Edges.Where(e => e.Right.Equals(vertexToReplace))
                       .Select(e => { e.Right = vertex; return e; })
                       .ToList();

            // Take its edges as edges from this vertex
            this.RemoveVertex(vertexToReplace);
            newEdges.ForEach(e => this.AddEdge(e));
        }

        public void Dispose()
        {
            foreach (var vertex in this.Vertices)
            {
                if(vertex.Image != null)
                    vertex.Dispose();
            }

            foreach (var group in this.Groups)
            {
                if (group.Image != null)
                    group.Dispose();
            }
        }

        private IPositionateEdge AddAbsoluteEdge(IAbsolutePositionateEdge edge)
        {
            foreach (var absEdge in this.AbsoluteEdges)
            {
                if (absEdge.GetType() == edge.GetType())
                {
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
