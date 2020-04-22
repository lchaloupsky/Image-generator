using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements
{
    public class Adposition : Element
    {
        private List<Adposition> DependingAdpositions { get; set; }
        private List<Adjective> Extensions { get; }

        public Adposition(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<Adjective>();
            this.DependingAdpositions = new List<Adposition>();
        }

        public IEnumerable<Adposition> GetAdpositions()
        {
            return this.DependingAdpositions.Count == 0 ?
                    new List<Adposition>() { this } :
                    new List<Adposition>(DependingAdpositions) { this };
        }

        public override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Adposition adp: return this.ProcessElement(adp, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
                case Verb verb: return this.ProcessElement(verb, graph);
                case Coordination cor: return this.ProcessCoordination(cor);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, ISentenceGraph graph)
        {
            this.DependingAdpositions.Add(adp);

            return this;
        }

        private IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            if (noun.DependencyType == "compound" || noun.DependencyType == "nmod:poss")
                noun.DependencyType = this.DependencyType;

            return noun.Process(this, graph);
        }

        private IProcessable ProcessElement(Verb verb, ISentenceGraph graph)
        {
            return verb.Process(this, graph);
        }

        private IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            return nounSet.Process(this, graph);
        }

        private IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            this.Extensions.Add(adj);

            return this;
        }

        public override string ToString()
        {
            return this.Extensions.Count == 0 ? base.ToString() : $"{string.Join(" ", this.Extensions)} {this.Lemma}";
        }
    }
}