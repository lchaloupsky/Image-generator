using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Rendering;
using System;
using System.Drawing;
using System.Numerics;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Class representing group of drawables
    /// </summary>
    public class MetaNoun : IDrawableGroup
    {
        // Group position
        public Vector2? Position { get; set; }

        // Group z-index
        public int ZIndex { get; set; }

        // Group width
        public int Width { get; set; }

        // Group height
        public int Height { get; set; }

        // Indicator if group is positionated
        public bool IsPositioned => true;

        // Indicator if group is fixed
        public bool IsFixed { get; set; }

        // Indicator if group is still valid
        public bool IsValid { get; set; } = true;

        // Group image
        public Image Image { get; set; }

        public IDrawableGroup Group { get; set; }

        // String representation of this group
        private string StringRepresentation { get; set; }

        public void Draw(IRenderer renderer, IImageManager manager)
        {
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, this.Width, this.Height);
        }

        public MetaNoun(IDrawable drawable1, IDrawable drawable2)
        {
            var rect = this.GetSmallestCoveringRectangle(drawable1, drawable2);
            this.SetNewCoordinatesAndDimensions(rect.Item1, rect.Item2, rect.Item3, Math.Min(drawable1.ZIndex, drawable2.ZIndex));
            this.Image = this.CombineImages(drawable1, drawable2, this.Position.Value, this.Width, this.Height);
            this.IsFixed = drawable1.IsFixed || drawable2.IsFixed;
            this.StringRepresentation = $"{drawable1}, {drawable2}";
        }

        /// <summary>
        /// Method that combines new drawable into this group
        /// </summary>
        /// <param name="drawable"></param>
        public void CombineIntoGroup(IDrawable drawable)
        {
            // get smallest needed rectangle
            var rect = this.GetSmallestCoveringRectangle(this, drawable);

            // get combined images
            this.Image = this.CombineImages(this, drawable, rect.Item1, rect.Item2, rect.Item3);

            // set new properties
            this.SetNewCoordinatesAndDimensions(rect.Item1, rect.Item2, rect.Item3, Math.Min(this.ZIndex, drawable.ZIndex));

            // Invalidating groups
            if (drawable.Group != null)
                drawable.Group.IsValid = false;
            else if (drawable is IDrawableGroup)
                ((IDrawableGroup)drawable).IsValid = false;
            else
                drawable.Group = this;

            // check if is fixed
            drawable.IsFixed = this.IsFixed || drawable.IsFixed;

            // Update string representation
            this.StringRepresentation = $"{this}, {drawable}";
        }

        /// <summary>
        /// Method that finds smallest needed rectangle
        /// </summary>
        /// <param name="drawable1">First drawable</param>
        /// <param name="drawable2">Second drawable</param>
        /// <returns>Smallest needed rectangle(position, width, height)</returns>
        private Tuple<Vector2, int, int> GetSmallestCoveringRectangle(IDrawable drawable1, IDrawable drawable2)
        {
            // Finding smallest possible rectangle position
            int x = (int)Math.Min(drawable1.Position.Value.X, drawable2.Position.Value.X);
            int y = (int)Math.Min(drawable1.Position.Value.Y, drawable2.Position.Value.Y);

            // Smallest possible dimensions to cover both objects
            int width = Math.Max((int)drawable1.Position.Value.X + drawable1.Width - x, (int)drawable2.Position.Value.X + drawable2.Width - x);
            int height = Math.Max((int)drawable1.Position.Value.Y + drawable1.Height - y, (int)drawable2.Position.Value.Y + drawable2.Height - y);

            return Tuple.Create(new Vector2(x, y), width, height);
        }

        /// <summary>
        /// Sets new properties
        /// </summary>
        /// <param name="position">New position</param>
        /// <param name="width">New width</param>
        /// <param name="height">New height</param>
        /// <param name="zIndex">New z-index</param>
        private void SetNewCoordinatesAndDimensions(Vector2 position, int width, int height, int zIndex)
        {
            this.Position = position;
            this.Width = width;
            this.Height = height;
            this.ZIndex = zIndex;
        }

        /// <summary>
        /// Method that combines two drawables into one
        /// </summary>
        /// <param name="drawable1">First drawable</param>
        /// <param name="drawable2">Second drawable</param>
        /// <param name="newPosition">New osition</param>
        /// <param name="newWidth">New width</param>
        /// <param name="newHeight">New height</param>
        /// <returns>Combined image</returns>
        private Image CombineImages(IDrawable drawable1, IDrawable drawable2, Vector2 newPosition, int newWidth, int newHeight)
        {
            // initialize needed variables
            Bitmap bitmap = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                // New positions in image
                float x1 = drawable1.Position.Value.X - newPosition.X;
                float x2 = drawable2.Position.Value.X - newPosition.X;

                float y1 = drawable1.Position.Value.Y - newPosition.Y;
                float y2 = drawable2.Position.Value.Y - newPosition.Y;

                // Locking ONLY ONE BY ONE --> to avoid deadlocks
                // Deciding which drawable draw first
                if (drawable1.ZIndex <= drawable2.ZIndex)
                {
                    lock (drawable1.Image)
                        graphics.DrawImage(drawable1.Image, x1, y1, drawable1.Width, drawable1.Height);

                    lock (drawable2.Image)
                        graphics.DrawImage(drawable2.Image, x2, y2, drawable2.Width, drawable2.Height);
                }
                else
                {
                    lock (drawable2.Image)
                        graphics.DrawImage(drawable2.Image, x2, y2, drawable2.Width, drawable2.Height);

                    lock (drawable1.Image)
                        graphics.DrawImage(drawable1.Image, x1, y1, drawable1.Width, drawable1.Height);
                }
            }

            return bitmap;
        }

        public override string ToString()
        {
            return this.StringRepresentation;
        }

        public void Dispose()
        {
            this.Image?.Dispose();
            this.Image = null;
        }
    }
}
