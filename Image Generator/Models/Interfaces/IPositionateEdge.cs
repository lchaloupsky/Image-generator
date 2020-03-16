using Image_Generator.Models.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    /// <summary>
    /// Interface for positioning edges
    /// </summary>
    interface IPositionateEdge
    {
        IDrawable Left { get; set; }
        IDrawable Right { get; set; }

        void Positionate(int maxWidth, int maxHeight);
    }
}
