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
    interface IDrawable : IPositionable
    {
        Image Image { get; }
        IDrawable Group { get; set; }

        void CombineIntoGroup(IDrawable drawable);
        void Draw(Renderer renderer, ImageManager manager);
    }
}
