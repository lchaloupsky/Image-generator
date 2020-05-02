using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Graph
{
    /// <summary>
    /// Inteface describing the sentence graph
    /// </summary>
    public interface ISentenceGraph : IDisposable
    {
        /// <summary>
        /// Graph drawable groups.
        /// Groups are combining multiple vertices together.
        /// </summary>
        IEnumerable<IDrawable> Groups { get; set; }

        /// <summary>
        /// Graph vertices
        /// </summary>
        IEnumerable<IDrawable> Vertices { get; }

        /// <summary>
        /// Graph edges
        /// </summary>
        IEnumerable<IPositionateEdge> Edges { get; }

        /// <summary>
        /// Indexer for getting edge for concrete vertex
        /// </summary>
        /// <param name="vertex">Vertex to get the edges</param>
        /// <returns>Edges for the given vertex</returns>
        IEnumerable<IPositionateEdge> this[IDrawable vertex] { get; }

        /// <summary>
        /// Method for adding new edge into graph
        /// </summary>
        /// <param name="edge">Edge to add</param>
        void AddEdge(IPositionateEdge edge);

        /// <summary>
        /// Method for adding new vertex into graph
        /// </summary>
        /// <param name="vertex">Vertex to add</param>
        void AddVertex(IDrawable vertex);

        /// <summary>
        /// Method for removing vertex from the graph
        /// </summary>
        /// <param name="vertex">Vertex to be removed</param>
        void RemoveVertex(IDrawable vertex);

        /// <summary>
        /// Method for replacing vertex in the graph with another vertex.
        /// Edges are reconnected to the new vertex
        /// </summary>
        /// <param name="vertex">New vertex</param>
        /// <param name="vertexToReplace">Vertex to be replaced</param>
        void ReplaceVertex(IDrawable vertex, IDrawable vertexToReplace);
    }
}
