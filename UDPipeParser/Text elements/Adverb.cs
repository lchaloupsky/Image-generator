using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents adverb element in the sentence
    /// </summary>
    public class Adverb : Element
    {
        // Extending adverbs
        public List<Adverb> ExtendingAdverbs { get; }

        public Adverb(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.ExtendingAdverbs = new List<Adverb>();
        }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            switch (element)
            {
                case Verb verb: return this.ProcessElement(verb, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
                case Adverb adv: return this.ProcessElement(adv, graph);
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Coordination cor: return this.ProcessCoordination(cor);
                default: break;
            }

            return this;
        }

        #region Processing concrete elements

        private IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            return nounSet.Process(this, graph);
        }

        private IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            return noun.Process(this, graph);
        }

        private IProcessable ProcessElement(Verb verb, ISentenceGraph graph)
        {
            return verb.Process(this, graph);
        }

        private IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            return adj.Process(this, graph);
        }

        private IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            this.ExtendingAdverbs.Add(adv);
            return this;
        }

        #endregion

        public override string ToString()
        {
            return this.ExtendingAdverbs.Count == 0 ? base.ToString() : $"{string.Join(" ", this.ExtendingAdverbs)} {this.Lemma}";
        }
    }
}
