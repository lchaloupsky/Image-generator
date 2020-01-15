using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Edges
{
    class AtEdge : Edge
    {
        public override void Positionate(int maxWidth, int maxHeight)
        {
            Console.WriteLine("At edge --> positioning: \n     Left: {0} \n    Right: {1}", this.Left, this.Right);
        }
    }
}
