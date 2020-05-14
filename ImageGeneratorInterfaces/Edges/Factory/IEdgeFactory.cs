using ImageGeneratorInterfaces.Graph.DrawableElement;
using System.Collections.Generic;

namespace ImageGeneratorInterfaces.Edges.Factory
{
    /// <summary>
    /// Interface for edge factory.
    /// </summary>
    public interface IEdgeFactory
    {
        /// <summary>
        /// Method for creating absolute positioning edges from adpositions.
        /// </summary>
        /// <param name="left">Vertex to be positioned</param>
        /// <param name="adpositions">List of adpositions</param>
        /// <returns>Absolute edge</returns>
        IAbsolutePositionateEdge Create(IDrawable left, List<string> adpositions);

        /// <summary>
        /// Method for creating relative positioning edges from adpositions.
        /// </summary>
        /// <param name="left">Left vertex of edge</param>
        /// <param name="right">Right vertex of edge</param>
        /// <param name="leftAdpositions">List of left adpositions</param>
        /// <param name="rightAdpositions">List of right adpositions</param>
        /// <param name="isRightSubject">Flag if right vertex is subject</param>
        /// <returns>Relative edge</returns>
        IPositionateEdge Create(IDrawable left, IDrawable right, List<string> leftAdpositions, List<string> rightAdpositions, bool isRightSubject);
    }
}
