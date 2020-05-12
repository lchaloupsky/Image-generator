using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Parsing;
using ImageGeneratorInterfaces.Rendering;
using System;
using System.Drawing;
using System.Numerics;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents safe root element.
    /// That is whole sentence. If none drawable is found, then is used this element for the final image.
    /// </summary>
    public class Root : Element, IDrawable
    {
        //----IDrawable properties----
        public Vector2? Position { get; set; }
        public int ZIndex { get; set; } = 0;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned { get; } = true;
        public bool IsFixed { get; set; } = true;
        public IDrawableGroup Group { get; set; }
        public Image Image { get; set; }

        //----Private props----
        private IImageManager Manager { get; }
        private DrawableHelper DrawableHelper { get; }

        public Root(string text, IImageManager manager) : base(0, text, "root")
        {
            this.Position = new Vector2(0, 0);
            this.Manager = manager;
            this.DrawableHelper = new DrawableHelper();
        }

        public void Draw(IRenderer renderer, IImageManager manager)
        {
            // Set sizes
            Width = renderer.Width;
            Height = renderer.Height;

            // Resize to fit preserve width/height ratio
            this.DrawableHelper.ResizeToImage(this);

            // Draw image across whole canvas
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, renderer.Width, renderer.Height);
        }

        public void CombineIntoGroup(IDrawable drawable)
        {
            return;
        }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            // If last element finalized element is not drawable return this 
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
            this.Image?.Dispose();
            this.Image = null;
            GC.SuppressFinalize(this);
        }
    }
}
