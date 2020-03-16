using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    /// <summary>
    /// Interface for poositionable elements
    /// </summary>
    interface IPositionable
    {
        Vector2? Position { get; set; }
        int ZIndex { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        bool IsPositioned { get; }
        bool IsFixed { get; set; }
    }
}
