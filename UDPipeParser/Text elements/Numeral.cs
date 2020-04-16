using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements
{
    public class Numeral : Element
    {
        public IProcessable DependingDrawable { get; set; }

        public Numeral(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        public override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Coordination cor: return this.ProcessCoordination(cor);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            if (this.DependencyType == "appos")
            {
                this.DependingDrawable = noun;
                return this;
            }

            this.DependencyType = "nummod";
            return noun.Process(this, graph);
        }

        private IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            if (this.DependencyType == "appos")
            {
                this.DependingDrawable = nounSet;
                return this;
            }

            this.DependencyType = "nummod";
            return nounSet.Process(this, graph);
        }

        public int GetValue()
        {
            return int.Parse(this.Lemma);
        }
    }
}
