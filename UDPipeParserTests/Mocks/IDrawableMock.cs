using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Rendering;
using System.Drawing;
using System.Numerics;

namespace UDPipeParserTests.Mocks
{
    class IDrawableMock : IDrawable
    {
        public Image Image { get; set; } = new Bitmap(1280, 720);
        public IDrawableGroup Group { get; set; }
        public Vector2? Position { get; set; }
        public int ZIndex { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned => this.Position != null;
        public bool IsFixed { get; set; }
        public void CombineIntoGroup(IDrawable drawable) { }
        public void Dispose() { }
        public void Draw(IRenderer renderer, IImageManager manager) { }
    }
}
