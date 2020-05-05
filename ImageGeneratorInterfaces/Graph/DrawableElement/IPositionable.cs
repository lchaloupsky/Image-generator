using System.Numerics;

namespace ImageGeneratorInterfaces.Graph.DrawableElement
{
    /// <summary>
    /// Interface for positionable elements
    /// </summary>
    public interface IPositionable
    {
        /// <summary>
        /// Position of the element
        /// </summary>
        Vector2? Position { get; set; }

        /// <summary>
        /// Z index of the element
        /// </summary>
        int ZIndex { get; set; }

        /// <summary>
        /// Width of the element
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Height of the element
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Flag if element is already positioned
        /// </summary>
        bool IsPositioned { get; }

        /// <summary>
        /// Flag if element is absolute positioned
        /// </summary>
        bool IsFixed { get; set; }
    }
}
