using Image_Generator.Models.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    interface IPositionateEdge
    {
        IDrawable Left { get; set; }
        IDrawable Right { get; set; }

        void Positionate(int maxWidth, int maxHeight);
        bool CheckPosition();
    }
}
