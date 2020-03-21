using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParserLibrary.Text_elements
{
    class Root : Element, IDrawable //TODO pass whole sentence
    {
        public Vector2? Position { get; set; }
        public int ZIndex { get; set; } = 0;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned { get; } = true;
        public bool IsFixed { get; set; } = true;

        public IDrawableGroup Group { get; set; }
        public Image Image { get; set; }

        public Root() : base(0)
        {
            this.Position = new Vector2(0, 0);
        }

        public void SetSizes(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Draw(Renderer renderer, ImageManager manager) { }

        public void CombineIntoGroup(IDrawable drawable)
        {
            return;
        }

        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            element = element.FinalizeProcessing(graph);
            return element is IDrawable ? element : this;
        }
    }
}
