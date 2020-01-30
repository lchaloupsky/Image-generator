using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Root : Element, IDrawable //In future throw away IDrawable
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned { get; set; } = false;

        public Root() : base(0) { }

        public void SetSizes(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Draw(Renderer renderer, ImageManager manager) { }

        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            return element is IDrawable ? element : this;
        }
    }
}
