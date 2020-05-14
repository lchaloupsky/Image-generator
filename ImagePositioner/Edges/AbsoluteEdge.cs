using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Abstract class for all possibly absolute edges.
    /// </summary>
    public abstract class AbsoluteEdge : Edge, IAbsolutePositionateEdge
    {
        // Absolute edge place type
        public PlaceType Type { get; }

        // Last connected element
        protected IDrawable LastElement { get; set; }

        protected AbsoluteEdge(PlaceType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Interface method for resolving conflicts of same type.
        /// </summary>
        /// <param name="edge">Edge to resolve</param>
        /// <returns>New edge connecting elements</returns>
        public abstract IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge);

        protected abstract override void PositionateLeft(int maxWidth, int maxHeight);

        protected abstract override void PositionateRight(int maxWidth, int maxHeight);

        /// <summary>
        /// Connects element to the given edge
        /// </summary>
        /// <param name="elementToConnect">Element to connect</param>
        /// <param name="newEdge">Edge to fill</param>
        /// <returns>Filled edge</returns>
        protected IPositionateEdge ResolveConflictWithGivenEdge(IDrawable elementToConnect, Edge newEdge)
        {
            this.LastElement = this.LastElement ?? this.Left;
            newEdge.Add(this.LastElement, elementToConnect);
            this.LastElement = elementToConnect;

            return newEdge;
        }
    }
}
