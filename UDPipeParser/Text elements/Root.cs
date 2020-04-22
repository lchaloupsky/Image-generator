using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Parsing;
using ImageGeneratorInterfaces.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements
{
    public class Root : Element, IDrawable
    {
        public Vector2? Position { get; set; }
        public int ZIndex { get; set; } = 0;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned { get; } = true;
        public bool IsFixed { get; set; } = true;

        public IDrawableGroup Group { get; set; }
        public Image Image { get; set; }

        private IImageManager Manager { get; }

        public Root(string text, IImageManager manager) : base(0, text, "root")
        {
            this.Position = new Vector2(0, 0);
            this.Manager = manager;
        }

        public void SetSizes(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Draw(IRenderer renderer, IImageManager manager)
        {
            Width = renderer.Width;
            Height = renderer.Height;

            this.ResizeToImage();

            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, renderer.Width, renderer.Height);
        }

        public void CombineIntoGroup(IDrawable drawable)
        {
            return;
        }

        public override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            element = element.FinalizeProcessing(graph);
            return element is IDrawable ? element : this;
        }

        public override IProcessable FinalizeProcessing(ISentenceGraph graph)
        {
            this.Image = this.Manager.GetImage(this.Lemma, null);

            return this;
        }

        public void Dispose()
        {
            this.Image = null;
        }

        private void ResizeToImage()
        {
            //TODO helper
            var ratio = this.Image.Width * 1f / Image.Height;
            this.Width = (int)(this.Height * ratio);
        }
    }
}
