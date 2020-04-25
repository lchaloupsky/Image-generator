using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Rendering
{
    /// <summary>
    /// Interface describing Renderer object.
    /// This is used for drawing IDrawable elements in the final image.
    /// </summary>
    public interface IRenderer
    {
        /// <summary>
        /// Image width
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Image height
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Method for getting currently drawn image
        /// </summary>
        /// <returns>Current image</returns>
        Image GetImage();

        /// <summary>
        /// Method for drawing image in the final image
        /// </summary>
        /// <param name="image">Image to be drawn</param>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        void DrawImage(Image image, int x, int y, int width, int height);

        /// <summary>
        /// Sets resolution of the final image in the renderer
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        void SetResolution(int width, int height);

        /// <summary>
        /// Resets current image
        /// </summary>
        void ResetImage();
    }
}
