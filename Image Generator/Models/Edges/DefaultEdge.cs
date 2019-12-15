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
    /// Default edge for connecting Drawable elements in a sentence
    /// </summary>
    class DefaultEdge
    {
        public IProcessable Left { get; set; }
        public IProcessable Right { get; set; }

        public DefaultEdge(Root root)
        {
            this.Left = root;
            this.Right = root;
        }

        public DefaultEdge(IProcessable left, IProcessable right)
        {
            this.Left = left;
            this.Right = right;
        }

        public DefaultEdge(IProcessable element, Root root)
        {
            this.Left = element;
            this.Right = root;
        }

        public void Positionate()
        {
            Console.WriteLine("Positioning: \n Left: {0} \n Right: {1}", this.Left, this.Right);
            Console.WriteLine();
        }
    }
}
