using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    /// <summary>
    /// Abstract class for all possibly absolute edges.
    /// </summary>
    abstract class AbsoluteEdge : Edge, IAbsolutePositionateEdge
    {
        // Absolute edge place type
        public PlaceType Type { get; }

        // Last connected element
        protected IDrawable LastElement { get; set; }

        public AbsoluteEdge(PlaceType type)
        {
            this.Type = type;
        }

        /// <summary>
        /// Interface method for resolving conflicts of same type.
        /// </summary>
        /// <param name="edge">Edge to resolve</param>
        /// <returns>New edge connecting elements</returns>
        public abstract IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge);

        protected override abstract void PositionateLeft(int maxWidth, int maxHeight);

        protected override abstract void PositionateRight(int maxWidth, int maxHeight);

        /// <summary>
        /// Connects element to the given edge
        /// </summary>
        /// <param name="ElementToConnect">Element to connect</param>
        /// <param name="newEdge">Edge to fill</param>
        /// <returns>Filled edge</returns>
        protected IPositionateEdge ResolveConflictWithGivenEdge(IDrawable ElementToConnect, Edge newEdge)
        {
            this.LastElement = this.LastElement ?? this.Left;
            newEdge.Add(this.LastElement, ElementToConnect);
            this.LastElement = ElementToConnect;

            return newEdge;
        }
    }
}
