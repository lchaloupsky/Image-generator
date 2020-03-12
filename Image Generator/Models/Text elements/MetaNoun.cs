using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class MetaNoun : IDrawableGroup
    {
        public Vector2? Position { get; set; }

        public int ZIndex { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool IsPositioned => true;

        public bool IsFixed { get; set; }

        public bool IsValid { get; set; } = true;

        public Image Image { get; set; }

        public IDrawableGroup Group { get; set; }

        public void Draw(Renderer renderer, ImageManager manager)
        {
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, this.Width, this.Height);
        }

        public MetaNoun(IDrawable drawable1, IDrawable drawable2)
        {
            var rect = this.GetSmallestCoveringRectangle(drawable1, drawable2);
            this.SetNewCoordinatesAndDimensions(rect.Item1, rect.Item2, rect.Item3, Math.Min(drawable1.ZIndex, drawable2.ZIndex));
            this.Image =  this.CombineImages(drawable1, drawable2, this.Position.Value, this.Width, this.Height);
            this.IsFixed = drawable1.IsFixed || drawable2.IsFixed;
        }

        public void CombineIntoGroup(IDrawable drawable)
        {
            var rect = this.GetSmallestCoveringRectangle(this, drawable);
            this.Image = this.CombineImages(this, drawable, rect.Item1, rect.Item2, rect.Item3);
            this.SetNewCoordinatesAndDimensions(rect.Item1, rect.Item2, rect.Item3, Math.Min(this.ZIndex, drawable.ZIndex));

            if (drawable.Group != null)
                drawable.Group.IsValid = false;
            else if (drawable is IDrawableGroup)
                ((IDrawableGroup)drawable).IsValid = false;
            else
                drawable.Group = this;

            drawable.IsFixed = this.IsFixed || drawable.IsFixed;
        }

        private Tuple<Vector2, int, int> GetSmallestCoveringRectangle(IDrawable drawable1, IDrawable drawable2) {
            // Finding smallest possible rectangle to cover both objects
            int x = (int)Math.Min(drawable1.Position.Value.X, drawable2.Position.Value.X);
            int y = (int)Math.Min(drawable1.Position.Value.Y, drawable2.Position.Value.Y);

            int width = Math.Max((int)drawable1.Position.Value.X + drawable1.Width - x, (int)drawable2.Position.Value.X + drawable2.Width - x);
            int height = Math.Max((int)drawable1.Position.Value.Y + drawable1.Height - y, (int)drawable2.Position.Value.Y + drawable2.Height - y);

            return Tuple.Create(new Vector2(x,y), width, height);
        }

        private void SetNewCoordinatesAndDimensions(Vector2 position, int width, int height, int zIndex)
        {
            this.Position = position;
            this.Width = width;
            this.Height = height;
            this.ZIndex = zIndex;
        }

        private Image CombineImages(IDrawable drawable1, IDrawable drawable2, Vector2 newPosition, int newWidth, int newHeight)
        {
            Bitmap bitmap = new Bitmap(newWidth, newHeight);
            Graphics graphics = Graphics.FromImage(bitmap);

            float x1 = drawable1.Position.Value.X - newPosition.X;
            float x2 = drawable2.Position.Value.X - newPosition.X;

            float y1 = drawable1.Position.Value.Y - newPosition.Y;
            float y2 = drawable2.Position.Value.Y - newPosition.Y;

            if (drawable1.ZIndex <= drawable2.ZIndex)
            {
                graphics.DrawImage(drawable1.Image, x1, y1, drawable1.Width, drawable1.Height);
                graphics.DrawImage(drawable2.Image, x2, y2, drawable2.Width, drawable2.Height);
                return bitmap;
            }

            graphics.DrawImage(drawable2.Image, x2, y2, drawable2.Width, drawable2.Height);
            graphics.DrawImage(drawable1.Image, x1, y1, drawable1.Width, drawable1.Height);
            return bitmap;
        }
    }
}
