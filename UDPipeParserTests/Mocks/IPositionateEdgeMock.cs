using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;

namespace UDPipeParserTests.Mocks
{
    class IPositionateEdgeMock : IPositionateEdge
    {
        public IDrawable Left { get; set; }
        public IDrawable Right { get; set; }

        public void Positionate(int maxWidth, int maxHeight) { }
    }
}
