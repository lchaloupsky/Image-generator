using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Edges.Factory
{
    public interface IEdgeFactory
    {
        IPositionateEdge Create<T1>(T1 left, List<string> adpositions) 
            where T1 : IDrawable, IProcessable;

        IPositionateEdge Create<T1, T2>(T1 left, T2 right, List<string> leftAdpositions, List<string> rightAdpositions) 
            where T1 : IDrawable, IProcessable
            where T2 : IDrawable, IProcessable;
    }
}
