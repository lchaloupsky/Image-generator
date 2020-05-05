using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParserTests.Mocks
{
    class IAbsoluteEdgeMock : IAbsolutePositionateEdge
    {
        public PlaceType Type => PlaceType.MIDDLE;
        public IDrawable Left { get; set; }
        public IDrawable Right { get; set; }

        public void Positionate(int maxWidth, int maxHeight) { }
        public IPositionateEdge ResolveConflict(IAbsolutePositionateEdge edge)
        {
            return new IPositionateEdgeMock() { Left = this.Left, Right = this.Right };
        }
    }
}
