using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Interfaces
{
    /// <summary>
    /// Interface for Drawable objects
    /// </summary>
    interface IDrawable
    {
        int X { get; set; }
        int Y { get; set; }
        int Width { get; set; }
        int Height { get; set; }

        void Draw(Renderer renderer, ImageManager manager);
    }
}
