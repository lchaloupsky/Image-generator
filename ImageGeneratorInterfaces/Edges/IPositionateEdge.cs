using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Edges
{
    /// <summary>
    /// Interface for positioning edges
    /// </summary>
    public interface IPositionateEdge
    {
        /// <summary>
        /// Left vertex of the edge
        /// </summary>
        IDrawable Left { get; set; }

        /// <summary>
        /// Right vertex of the edge
        /// </summary>
        IDrawable Right { get; set; }

        /// <summary>
        /// Method for positioning edge
        /// </summary>
        /// <param name="maxWidth">Maximal width of an image</param>
        /// <param name="maxHeight">Maximal height of an image</param>
        void Positionate(int maxWidth, int maxHeight);
    }
}
