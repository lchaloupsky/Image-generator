using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Graph
{
    public interface ISentenceGraph : IDisposable
    {
        IEnumerable<IDrawable> Groups { get; set; }
        IEnumerable<IDrawable> Vertices { get; }
        IEnumerable<IPositionateEdge> Edges { get; }
        IEnumerable<IPositionateEdge> this[IDrawable vertex] { get; }

        void AddEdge(IPositionateEdge edge);
        void AddVertex(IDrawable vertex);
        void RemoveVertex(IDrawable vertex);
        void ReplaceVertex(IDrawable vertex, IDrawable vertexToReplace);
    }
}
