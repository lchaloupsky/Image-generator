using ImageGeneratorInterfaces.Edges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagePositioner.Edges
{
    abstract class AbsoluteEdge : Edge, IAbsolutePositionateEdge
    {
        public PlaceType Type { get; }

        public AbsoluteEdge(ImageGeneratorInterfaces.Edges.PlaceType type)
        {
            this.Type = type;
        }

        public abstract IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge);

        protected override abstract void PositionateLeft(int maxWidth, int maxHeight);

        protected override abstract void PositionateRight(int maxWidth, int maxHeight);
    }
}
