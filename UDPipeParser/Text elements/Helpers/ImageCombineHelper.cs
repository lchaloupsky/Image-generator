using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace UDPipeParsing.Text_elements.Helpers
{
    /// <summary>
    /// Class helping with combining multiple images into one image
    /// </summary>
    public class ImageCombineHelper
    {
        // Default scale when placing in row
        private const double ROW_SCALE = 0.75;
        private const double CIRCLE_SCALE = 0.95;

        // Minimal object dimension
        private const int MIN_OBJECT_WIDTH = 50;
        private const int MIN_OBJECT_HEIGHT = 25;

        private DrawableHelper DrawableHelper { get; } = new DrawableHelper();

        /// <summary>
        /// Places images into one row
        /// </summary>
        /// <param name="Nouns">Nouns to place</param>
        /// <returns>Width, Height, Z-index and image tuple</returns>
        public Tuple<int, int, int, Image> PlaceInRow(List<Noun> Nouns)
        {
            double scale = 1;
            int width = 0,
                height = 0,
                zIndex = 0;

            // Get max values of dimensions
            foreach (var noun in Nouns)
            {
                width += noun.Group?.Width ?? noun.Width;
                height = Math.Max(height, noun.Group?.Height ?? noun.Height);
                zIndex = Math.Min(zIndex, noun.ZIndex);
                scale *= ROW_SCALE;

                if (noun.Group != null)
                    noun.Group.IsValid = false;
            }

            // Scale final dimensions
            height = (int)(height * scale);
            width = (int)(width * scale);

            // Creates new image with dimensions
            Image image = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // Draw all images
                int LastX = 0;
                IDrawable finalElement;
                foreach (IDrawable noun in Nouns)
                {
                    // If noun has group, draw it with scale
                    finalElement = noun.Group ?? noun;
                    lock (finalElement.Image)
                        graphics.DrawImage(finalElement.Image, LastX, (int)(height - finalElement.Height * scale), (int)(scale * finalElement.Width), (int)(scale * finalElement.Height));

                    LastX += (int)(finalElement.Width * scale);
                }
            }

            return Tuple.Create(width, height, zIndex, image);
        }

        /// <summary>
        /// Places images into a circle shape
        /// </summary>
        /// <param name="Nouns">Nouns to place</param>
        /// <returns>Width, Height, Z-index and image tuple</returns>
        public Tuple<int, int, int, Image> PlaceInCircle(List<Noun> Nouns)
        {
            int MaxWidth = 0;
            int MaxHeight = 0;
            int width = 0,
                height = 0,
                zIndex = 0;

            // Get max dimensions values
            foreach (var noun in Nouns)
            {
                MaxWidth = Math.Max(MaxWidth, noun.Group?.Width ?? noun.Width);
                MaxHeight = Math.Max(MaxHeight, noun.Group?.Height ?? noun.Height);
                zIndex = Math.Min(zIndex, noun.ZIndex);

                if (noun.Group != null)
                    noun.Group.IsValid = false;
            }

            // Calc scaling and final dim value
            double scale = Math.Pow(CIRCLE_SCALE, Nouns.Count);
            var finalDim = (int)Math.Log(Math.Max(MaxWidth, MaxHeight) * Nouns.Count) / 3 * 100;

            // Creates new image with dimensions
            height = finalDim;
            width = finalDim;
            Image image = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // Draw final image
                int newX = 0;
                int newY = 0;
                Random rand = new Random();
                IDrawable finalElement;
                foreach (IDrawable noun in Nouns)
                {
                    // If element has a group, use the group
                    finalElement = noun.Group ?? noun;

                    // element ratio
                    float ratio = this.DrawableHelper.GetDimensionsRatio(finalElement.Width, finalElement.Height);

                    // Do scaling of dimensions
                    int finalWidth = (int)(finalElement.Width * scale);
                    int finalHeight = (int)(finalElement.Height * scale);

                    // Preserve ratio
                    if (finalHeight < MIN_OBJECT_HEIGHT)
                    {
                        finalHeight = MIN_OBJECT_HEIGHT;
                        finalWidth = (int)(finalHeight * ratio);
                    }

                    // Calc new positions -> random positions in circle
                    int distance = rand.Next(finalDim / 2);
                    double angleInRadians = rand.Next(360) / (2 * Math.PI);

                    newX = (int)(distance * Math.Cos(angleInRadians)) + finalDim / 2;
                    newY = (int)(distance * Math.Sin(angleInRadians)) + finalDim / 2;

                    newX = Math.Min(newX, finalDim - finalWidth);
                    newY = Math.Min(newY, finalDim - finalHeight);

                    // Draw image
                    lock (finalElement.Image)
                        graphics.DrawImage(finalElement.Image, newX, newY, finalWidth, finalHeight);
                }
            }

            return Tuple.Create(width, height, zIndex, image);
        }
    }
}
