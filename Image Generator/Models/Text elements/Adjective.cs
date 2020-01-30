using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image_Generator.Models.Interfaces;

namespace Image_Generator.Models.Text_elements
{
    class Adjective : Element
    {
        public Adjective(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

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
            return noun.Process(this, graph);
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            return nounSet.Process(this, graph);
        }

        //Maybe redo with list?
        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            this.Lemma += $" {adj}";
            return this;
        }
    }
}
