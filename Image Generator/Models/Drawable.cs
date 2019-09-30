using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image_Generator.Models.Interfaces;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class that represents drawable element in sentence
    /// </summary>
    class Drawable : Interfaces.IDrawable
    {
        // Image to draw
        public Image MyImage { get; }

        // X coordinate
        public int X { get; set; }

        // Y coordinate
        public int Y { get; set; }

        public Drawable(Image image)
        {
            this.MyImage = image;
        }

        public Drawable(Image image, int x, int y) : this (image)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Function to draw
        /// </summary>
        /// <returns>Image to be drawn</returns>
        public void Draw(Renderer renderer)
        {
            renderer.DrawImage(this.MyImage);
        }
    }
}
