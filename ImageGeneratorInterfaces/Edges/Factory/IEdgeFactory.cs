using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Edges.Factory
{
    /// <summary>
    /// Interface for edge factory.
    /// </summary>
    public interface IEdgeFactory
    {
        /// <summary>
        /// Method for creating abosolute positioning edges from adpositions.
        /// </summary>
        /// <typeparam name="T1">Type of object</typeparam>
        /// <param name="left">Object to be positioned</param>
        /// <param name="adpositions">List of adpositions</param>
        /// <returns>Absolute edge</returns>
        IAbsolutePositionateEdge Create<T1>(T1 left, List<string> adpositions) 
            where T1 : IDrawable, IProcessable;

        /// <summary>
        /// Method for creating relative positioning edges from adpositions.
        /// </summary>
        /// <typeparam name="T1">First object type</typeparam>
        /// <typeparam name="T2">Second object type</typeparam>
        /// <param name="left">Left vertex of edge</param>
        /// <param name="right">Right vertex of edge</param>
        /// <param name="leftAdpositions">List of left adpositions</param>
        /// <param name="rightAdpositions">List of right adpositions</param>
        /// <returns>Relative edge</returns>
        IPositionateEdge Create<T1, T2>(T1 left, T2 right, List<string> leftAdpositions, List<string> rightAdpositions) 
            where T1 : IDrawable, IProcessable
            where T2 : IDrawable, IProcessable;
    }
}
