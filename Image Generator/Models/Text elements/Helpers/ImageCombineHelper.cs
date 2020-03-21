using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements.Helpers
{
    class ImageCombineHelper
    {
        public Tuple<int, int, int, Image> PlaceInRow(List<Noun> Nouns)
        {
            double scale = 1;
            int width = 0,
                height = 0, 
                zIndex = 0;

            foreach (var noun in Nouns)
            {
                width += noun.Group?.Width ?? noun.Width;
                height = Math.Max(height, noun.Group?.Height ?? noun.Height);
                zIndex = Math.Min(zIndex, noun.ZIndex);
                scale *= 0.75;

                if (noun.Group != null)
                    noun.Group.IsValid = false;
            }

            height = (int)(height * scale);
            width = (int)(width * scale);
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);

            int LastX = 0;
            IDrawable finalElement;
            foreach (IDrawable noun in Nouns)
            {
                finalElement = noun.Group ?? noun;
                lock (finalElement.Image)
                    graphics.DrawImage(finalElement.Image, LastX, (int)(height - finalElement.Height * scale), (int)(scale * finalElement.Width), (int)(scale * finalElement.Height));

                LastX += (int)(finalElement.Width * scale);
            }

            return Tuple.Create(width, height, zIndex, image);
        }

        public Tuple<int, int, int, Image> PlaceInCircle(List<Noun> Nouns)
        {
            int MaxWidth = 0;
            int MaxHeight = 0;
            int width = 0,
                height = 0,
                zIndex = 0;

            foreach (var noun in Nouns)
            {
                MaxWidth = Math.Max(MaxWidth, noun.Group?.Width ?? noun.Width);
                MaxHeight = Math.Max(MaxHeight, noun.Group?.Height ?? noun.Height);
                zIndex = Math.Min(zIndex, noun.ZIndex);

                if (noun.Group != null)
                    noun.Group.IsValid = false;
            }

            // REDO Scaling
            //double scale = Math.Max(Math.Pow(0.8, this.Nouns.Count), 0.05);
            double scale = Math.Pow(0.9, Nouns.Count);
            //var finalDim = (int)(Math.Max(MaxWidth, MaxHeight) * Math.Pow(this.Nouns.Count, scale));
            var finalDim = (int)Math.Log(Math.Max(MaxWidth, MaxHeight) * Nouns.Count) / 3 * 100;

            height = finalDim;
            width = finalDim;
            Image image = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(image);

            int newX = 0;
            int newY = 0;
            Random rand = new Random();
            IDrawable finalElement;
            foreach (IDrawable noun in Nouns)
            {
                finalElement = noun.Group ?? noun;
                int finalWidth = Math.Max(34, (int)(finalElement.Width * scale));
                int finalHeight = Math.Max(20, (int)(finalElement.Height * scale));

                int distance = rand.Next(finalDim / 2);
                double angleInRadians = rand.Next(360) / (2 * Math.PI);

                newX = (int)(distance * Math.Cos(angleInRadians)) + finalDim / 2;
                newY = (int)(distance * Math.Sin(angleInRadians)) + finalDim / 2;

                newX = Math.Min(newX, finalDim - finalWidth);
                newY = Math.Min(newY, finalDim - finalHeight);

                lock (finalElement.Image)
                    graphics.DrawImage(finalElement.Image, newX, newY, finalWidth, finalHeight);
            }

            return Tuple.Create(width, height, zIndex, image);
        }
    }
}
