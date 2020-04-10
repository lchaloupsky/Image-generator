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
        public List<Element> Extensions { get; }

        public Adjective(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<Element>();
        }

        public override IProcessable ProcessElement(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
                case Adverb adv: return this.ProcessElement(adv, graph);
                case Verb verb: return this.ProcessElement(verb, graph);
                case Coordination cor: return this.ProcessCoordination(cor);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Verb verb, SentenceGraph graph)
        {
            return verb.Process(this, graph);
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
            if (adj.CoordinationType != CoordinationType.AND)
                return this;

            this.Extensions.Add(adj);
            return this;
        }

        private IProcessable ProcessElement(Adverb adv, SentenceGraph graph)
        {
            this.Extensions.Add(adv);

            return this;
        }

        public override string ToString()
        {
            return this.Extensions.Count == 0 ? base.ToString() : $"{string.Join(" ", this.Extensions)} {this.Lemma}";
        }
    }
}
