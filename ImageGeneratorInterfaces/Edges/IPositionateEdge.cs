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
        IDrawable Left { get; set; }
        IDrawable Right { get; set; }

        void Positionate(int maxWidth, int maxHeight);
    }
}
