using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositionerTests.Mocks
{
    class ISentenceGraphMock : ISentenceGraph
    {
        public Dictionary<IDrawable, List<IPositionateEdge>> Graph { get; set; }
        public IEnumerable<IDrawable> Groups { get; set; }
        public IEnumerable<IDrawable> Vertices => this.Graph.Keys;
        public IEnumerable<IPositionateEdge> Edges => this.Graph.Values.SelectMany(edge => edge);
        public IEnumerable<IPositionateEdge> this[IDrawable vertex] => this.Graph.ContainsKey(vertex) ? this.Graph[vertex] : null;

        public void AddEdge(IPositionateEdge edge) {}
        public void AddVertex(IDrawable vertex) {}
        public void Dispose() {}
        public void RemoveVertex(IDrawable vertex) {}
        public void ReplaceVertex(IDrawable vertex, IDrawable vertexToReplace) {}
    }
}
