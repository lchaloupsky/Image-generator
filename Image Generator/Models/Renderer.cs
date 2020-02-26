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
        // Field for drawing
        private Bitmap DrawField { get; set; }
        private Graphics MyGraphics { get; set; }

        // Coordinates for new image to draw
        public int LastX { get; set; } = 0;
        public int LastY { get; set; } = 0;

        /// <summary>
        /// Constructor for Renderer
        /// </summary>
        /// <param name="width">New draw field width</param>
        /// <param name="height">New draw field height</param>
        public Renderer(int width, int height)
        {
            this.SetResolution(width, height);
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
        /// Function to draw single image to current draw field
        /// </summary>
        /// <param name="image">Image to draw</param>
        public void DrawImage(Image image, int x, int y, int width, int height)
        {
            var dimensions = this.GetProportionalDimensions(image, width, height);

            // Shift to next row of images if current row is fully drawn
            if (x + dimensions.Item1 > this.DrawField.Width)
            {
                x = 0;
                y += height;
            }

            // Finally drawing of the current image
            this.MyGraphics.DrawImage(image, x, y, dimensions.Item1, dimensions.Item2);
            LastX = x + dimensions.Item1;
            LastY = y;
        }

        /// <summary>
        /// Resizes given image dimensions to be correctly proportional 
        /// </summary>
        /// <param name="image">image to be resized</param>
        /// <param name="width">width of an object</param>
        /// <param name="height">height of an object</param>
        /// <returns>Resized dimensions as Tuple</returns>
        public Tuple<int, int> GetProportionalDimensions(Image image, int width, int height)
        {
            int finalW = width,
                finalH = height;
            
            // Resizing image dimension if image is larger than max width/height
            if (image.Width > width)
            {
                double ratio = width * 1d / image.Width;
                finalW = width;
                finalH = (int)(image.Height * ratio);
            }
            if (finalH > height)
            {
                double ratio = height * 1d / finalH;
                finalH = height;
                finalW = (int)(finalW * ratio);
            }

            return Tuple.Create(finalW, finalH);
        }

        public void SetResolution(int width, int height)
        {
            this.DrawField = new Bitmap(width, height);
            this.MyGraphics = Graphics.FromImage(DrawField);
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
