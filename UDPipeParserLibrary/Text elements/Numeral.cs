using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParserLibrary.Text_elements
{
    class Numeral : Element
    {
        public IProcessable DependingDrawable { get; set; }

        public Numeral(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            if (this.DependencyType == "appos")
            {
                this.DependingDrawable = noun;
                return this;
            }

            this.DependencyType = "nummod";
            return noun.Process(this, graph);
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
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
