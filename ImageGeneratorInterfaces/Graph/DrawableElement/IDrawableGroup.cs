namespace ImageGeneratorInterfaces.Graph.DrawableElement
{
    /// <summary>
    /// Wrapper for IDrawable
    /// </summary>
    public interface IDrawableGroup : IDrawable
    {
        /// <summary>
        /// Indicator if group is still valid
        /// </summary>
        bool IsValid { get; set; }
    }
}
