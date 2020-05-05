using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System.Collections.Generic;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents adjective part of speech
    /// </summary>
    public class Adjective : Element
    {
        // List of extensions to this element
        public List<Element> Extensions { get; }

        public Adjective(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<Element>();
        }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
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

        #region Processing concrete elements

        protected virtual IProcessable ProcessElement(Verb verb, ISentenceGraph graph)
        {
            return verb.Process(this, graph);
        }

        protected virtual IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            return noun.Process(this, graph);
        }

        protected virtual IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            return nounSet.Process(this, graph);
        }

        protected virtual IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            if (adj.CoordinationType != CoordinationType.AND)
                return this;

            this.Extensions.Add(adj);
            return this;
        }

        protected virtual IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            this.Extensions.Add(adv);
            return this;
        }

        #endregion

        public override string ToString()
        {
            return this.Extensions.Count == 0 ? base.ToString() : $"{string.Join(" ", this.Extensions)} {this.Lemma}";
        }
    }
}
