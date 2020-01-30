using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Adposition : Element
    {
        private Adposition DependingAdposition { get; set; }

        public Adposition(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        public IEnumerable<Adposition> GetAdpositions()
        {
            return this.DependingAdposition == null ? new List<Adposition>() { this } : new List<Adposition>() { DependingAdposition, this };
        }

        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Adposition adp: return this.ProcessElement(adp, graph);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, SentenceGraph graph)
        {
            this.DependingAdposition = adp;
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
    }
}