namespace ImageGeneratorInterfaces.Edges
{
    /// <summary>
    /// Interface describing absolute positioning edge
    /// </summary>
    public interface IAbsolutePositionateEdge : IPositionateEdge
    {
        /// <summary>
        /// Absolute type of place, where edge is positioning 
        /// </summary>
        PlaceType Type { get; }

        /// <summary>
        /// Method for resolving conflict between two same absolute edges
        /// </summary>
        /// <param name="edge">Edge to be resolved</param>
        /// <returns>New relative edge connecting these two edges together</returns>
        IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge);
    }

    /// <summary>
    /// Enum describing absolute place type of an edge
    /// </summary>
    public enum PlaceType
    {
        VERTICAL, HORIZONTAL, CORNER, MIDDLE
    }
}
