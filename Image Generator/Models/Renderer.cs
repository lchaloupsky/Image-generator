using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class for rendering new image
    /// </summary>
    class Renderer
    {
        // Max image width and height for drawing
        private const int MAX_IMAGE_WIDTH = 240;
        private const int MAX_IMAGE_HEIGHT = 120;

        // Field for drawing
        private Bitmap DrawField { get; set; }
        private Graphics MyGraphics { get; set; }

        // Coordinates for new image draw
        private int LastX { get; set; } = 0;
        private int LastY { get; set; } = 0;

        /// <summary>
        /// Constructor for Renderer
        /// </summary>
        /// <param name="width">New draw field width</param>
        /// <param name="height">New draw field height</param>
        public Renderer(int width, int height)
        {
            this.DrawField = new Bitmap(width, height);
            this.MyGraphics = Graphics.FromImage(DrawField);
        }

        /// <summary>
        /// Function for checking size of current draw field and form field sizes
        /// </summary>
        /// <param name="width">Current width of form draw field</param>
        /// <param name="height">Current height of form draw field</param>
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

        /// <summary>
        /// Return actual draw field
        /// </summary>
        /// <returns>Drawn image</returns>
        public Image GetImage()
        {
            return this.DrawField;
        }

        /// <summary>
        /// Draws whole new picture from images
        /// </summary>
        /// <param name="images">List of images to draw</param>
        /// <param name="width">Width of form draw field</param>
        /// <param name="height">Height of form draw field</param>
        public void DrawPicture(IEnumerable<Image> images, int width, int height)
        {
            CheckSizes(width, height);
            DrawImages(images);
        }

        /// <summary>
        /// Function for drawing multiple images to current draw field
        /// </summary>
        /// <param name="images">List of images to draw</param>
        public void DrawImages(IEnumerable<Image> images)
        {
            foreach (var image in images)
            {
                DrawImage(image);
            }
        }

        /// <summary>
        /// Function to draw single image to current draw field
        /// </summary>
        /// <param name="image">Image to draw</param>
        public void DrawImage(Image image)
        {
            int finalW = MAX_IMAGE_WIDTH,
                finalH = MAX_IMAGE_HEIGHT;

            // Resizing image dimension if image is larger than max width/height
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

            // Shift to next row of images if current row is fully drawn
            if (LastX + finalW > this.DrawField.Width)
            {
                LastX = 0;
                LastY += MAX_IMAGE_HEIGHT;
            }

            // Finally drawing of the current image
            this.MyGraphics.DrawImage(image, LastX, LastY, finalW, finalH);
            LastX += finalW;
        }

        /// <summary>
        /// Resets current draw field properties
        /// </summary>
        public void ResetImage()
        {
            this.LastX = 0;
            this.LastY = 0;
            this.MyGraphics.Clear(Color.White);
        }
    }
}
