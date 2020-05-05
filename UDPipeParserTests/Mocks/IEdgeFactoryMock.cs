using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParserTests.Mocks
{
    class IEdgeFactoryMock : IEdgeFactory
    {
        public IAbsolutePositionateEdge Create(IDrawable left, List<string> adpositions)
        {
            return new IAbsoluteEdgeMock() { Left = left };
        }

        public IPositionateEdge Create(IDrawable left, IDrawable right, List<string> leftAdpositions, List<string> rightAdpositions, bool isRightSubject)
        {
            if (!isRightSubject)
                return new IPositionateEdgeMock() { Left = left, Right = right };

            return new IPositionateEdgeMock() { Left = right, Right = left };
        }
    }
}
