using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParserLibrary.Text_elements
{
    class Adverb : Element
    {
        private List<Adverb> ExtendingAdverbs { get; }

        public Adverb(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.ExtendingAdverbs = new List<Adverb>();
        }

        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Verb verb: return this.ProcessElement(verb, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
                case Adverb adv: return this.ProcessElement(adv, graph);
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            return nounSet.Process(this, graph);
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            return noun.Process(this, graph);
        }

        private IProcessable ProcessElement(Verb verb, SentenceGraph graph)
        {
            return verb.Process(this, graph);
        }

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            return adj.Process(this, graph);
        }

        private IProcessable ProcessElement(Adverb adv, SentenceGraph graph)
        {
            this.ExtendingAdverbs.Add(adv);

            return this;
        }

        public override string ToString()
        {
            return this.ExtendingAdverbs.Count == 0 ? base.ToString() : $"{string.Join(" ", this.ExtendingAdverbs)} {this.Lemma}";
        }
    }
}
