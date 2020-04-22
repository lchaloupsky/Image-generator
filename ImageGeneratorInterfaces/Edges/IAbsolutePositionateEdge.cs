using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Edges
{
    public interface IAbsolutePositionateEdge : IPositionateEdge
    {
        PlaceType Type { get; }

        IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge);
    }

    public enum PlaceType
    {
        VERTICAL, HORIZONTAL, CORNER, MIDDLE
    }
}
