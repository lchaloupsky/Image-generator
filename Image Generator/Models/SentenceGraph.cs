using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class SentenceGraph
    {
        private Dictionary<IDrawable, List<IPositionateEdge>> Graph { get; }

        public IEnumerable<IDrawable> Vertices => this.Graph.Keys;
        public IEnumerable<IPositionateEdge> Edges => this.Graph.Values.SelectMany(edge => edge);
        public IEnumerable<IPositionateEdge> this[IDrawable vertex] => this.Graph.ContainsKey(vertex) ? this.Graph[vertex] : null;

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

            this.Graph[edge.Left].Add(edge);
        }

        public void AddVertex(IDrawable vertex)
        {
            if (this.Graph.ContainsKey(vertex))
                return;

            this.Graph.Add(vertex, new List<IPositionateEdge>());
        }
    }
}
