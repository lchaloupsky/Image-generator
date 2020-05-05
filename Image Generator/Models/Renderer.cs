using ImageGeneratorInterfaces.Rendering;
using System.Drawing;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class for rendering new image
    /// </summary>
    class Renderer : IRenderer
    {
        // Field for drawing
        private Bitmap DrawField { get; set; }
        private Graphics MyGraphics { get; set; }

        // Dimensions
        public int Width => this.DrawField.Width;
        public int Height => this.DrawField.Height;

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
            // Finally drawing of the current image
            lock (image)
                this.MyGraphics.DrawImage(image, x, y, width, height);
        }

        /// <summary>
        /// Method for setting resolution of drawn image
        /// </summary>
        /// <param name="width">width of image</param>
        /// <param name="height">height of image</param>
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
            this.MyGraphics.Clear(Color.White);
        }
    }
}
