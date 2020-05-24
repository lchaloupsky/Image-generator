using System;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Rendering;
using System.Drawing;
using System.Numerics;

namespace ImagePositionerTests.Mocks
{
    public class IDrawableMock : IDrawable
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
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Draw(IRenderer renderer, IImageManager manager) { }

        private void Dispose(bool disposing)
        {
            this.Image?.Dispose();
        }
    }
}
