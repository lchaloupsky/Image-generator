using Image_Generator.Models.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    interface IPositionate
    {
        IDrawable Left { get; set; }
        IDrawable Right { get; set; }

        // Add somehow list of all of vertices to check collisions, also add width and height.
        void Positionate(int maxWidth, int maxHeight);
    }
}
