using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class Positioner
    {
        private PositionHelper Helper { get; }

        public Positioner()
        {
            this.Helper = new PositionHelper();
        }

        public void Positionate(SentenceGraph graph, int width, int height)
        {
            this.Helper.SetProperties(width, height, graph);

            foreach (var vertex in graph.Vertices)
            {
                vertex.IsPositioned = true;
                foreach (var edge in graph[vertex])
                    edge.Positionate(width, height);
            }
        }
    }

    class PositionHelper
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public IEnumerable<IDrawable> Vertices { get; set; }
    
        public void GetEmptyPosition()
        {
            //TODO
        }

        public void SetProperties(int width, int height, SentenceGraph graph)
        {
            this.Width = width;
            this.Height = height;
            this.Vertices = graph.Vertices;
        }
    }
}
