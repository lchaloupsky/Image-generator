using ImageGeneratorInterfaces.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Parsing
{
    /// <summary>
    /// Interface for processable elements
    /// </summary>
    public interface IProcessable
    {
        int Id { get; }
        string DependencyType { get; set; }
        bool IsNegated { get; }

        IProcessable Process(IProcessable element, ISentenceGraph graph);
        IProcessable FinalizeProcessing(ISentenceGraph graph);
    }
}
