using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class Renderer
    {
        private const int MAX_IMAGE_WIDTH = 240;
        private const int MAX_IMAGE_HEIGHT = 120;

        private Bitmap DrawField { get; set; }
        private Graphics MyGraphics { get; set; }

        private int LastX { get; set; } = 0;
        private int LastY { get; set; } = 0;

        public Renderer(int width, int height)
        {
            this.DrawField = new Bitmap(width, height);
            this.MyGraphics = Graphics.FromImage(DrawField);
        }

        public void CheckSizes(int width, int height)
        {
            if (this.DrawField.Width != width || this.DrawField.Height != height)
            {
                this.DrawField = new Bitmap(width, height);
                this.MyGraphics = Graphics.FromImage(DrawField);
            }
            else
                ResetImage();
        }

        public Image GetImage()
        {
            return this.DrawField;
        }

        public void DrawPicture(IEnumerable<Image> images, int width, int height)
        {
            CheckSizes(width, height);
            DrawImages(images);
        }

        public void DrawImages(IEnumerable<Image> images)
        {
            foreach (var image in images)
            {
                DrawImage(image);
            }
        }

        public void DrawImage(Image image)
        {
            int finalW = MAX_IMAGE_WIDTH,
                finalH = MAX_IMAGE_HEIGHT;

            if (image.Width > MAX_IMAGE_WIDTH)
            {
                double ratio = MAX_IMAGE_WIDTH * 1d / image.Width;
                finalW = MAX_IMAGE_WIDTH;
                finalH = (int)(image.Height * ratio);
            }
            if (finalH > MAX_IMAGE_HEIGHT)
            {
                double ratio = MAX_IMAGE_HEIGHT * 1d / finalH;
                finalH = MAX_IMAGE_HEIGHT;
                finalW = (int)(finalW * ratio);
            }

            if (LastX + finalW > this.DrawField.Width)
            {
                LastX = 0;
                LastY += MAX_IMAGE_HEIGHT;
            }

            this.MyGraphics.DrawImage(image, LastX, LastY, finalW, finalH);
            LastX += finalW;
        }

        public void ResetImage()
        {
            this.LastX = 0;
            this.LastY = 0;
            this.MyGraphics.Clear(Color.White);
        }
    }
}
