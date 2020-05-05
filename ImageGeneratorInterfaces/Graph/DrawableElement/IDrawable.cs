using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Rendering;
using System;
using System.Drawing;

namespace ImageGeneratorInterfaces.Graph.DrawableElement
{
    /// <summary>
    /// Interface for Drawable objects
    /// </summary>
    public interface IDrawable : IPositionable, IDisposable
    {
        /// <summary>
        /// Elements object
        /// </summary>
        Image Image { get; }

        /// <summary>
        /// Group where element belongs to
        /// </summary>
        IDrawableGroup Group { get; set; }

        /// <summary>
        /// Method for combining two elements into same group
        /// </summary>
        /// <param name="drawable">Drawable to combine</param>
        void CombineIntoGroup(IDrawable drawable);

        /// <summary>
        /// Method for drawing this drawable element
        /// </summary>
        /// <param name="renderer">Renderer to draw an image</param>
        /// <param name="manager">Manager for getting images</param>
        void Draw(IRenderer renderer, IImageManager manager);
    }
}
