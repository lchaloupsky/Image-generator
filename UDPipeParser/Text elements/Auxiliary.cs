using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements
{
    public class Auxiliary : Element
    {
        public Auxiliary(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        public override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            return element;
        }
    }
}
