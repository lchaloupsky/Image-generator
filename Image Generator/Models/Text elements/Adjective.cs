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
        public List<Element> ExtendingElements { get; }

        public Adjective(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.ExtendingElements = new List<Element>();
        }

        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
                case Adverb adv: return this.ProcessElement(adv, graph);
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

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            this.ExtendingElements.Add(adj);

            return this;
        }

        private IProcessable ProcessElement(Adverb adv, SentenceGraph graph)
        {
            this.ExtendingElements.Add(adv);

            return this;
        }

        public override string ToString()
        {
            return this.ExtendingElements.Count == 0 ? base.ToString() : $"{string.Join(" ", this.ExtendingElements)} {this.Lemma}";
        }
    }
}
