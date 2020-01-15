using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Edges
{
    /// <summary>
    /// Abstract edge as base class for all edges
    /// </summary>
    abstract class Edge : IPositionate
    {
        public IDrawable Left { get; set; }
        public IDrawable Right { get; set; }

        public Edge Add(IDrawable left, IDrawable right)
        {
            this.Left = left;
            this.Right = right;

            return this;
        }

        public virtual void Positionate(int maxWidth, int maxHeight)
        {
            Console.WriteLine("Positioning: \n     Left: {0} \n    Right: {1}", this.Left, this.Right);
        }
    }
}
