using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Verb : Element
    {
        private VerbFormHelper VerbFormHelper { get; }

        private List<Verb> RelatedActions { get; }

        private List<Element> ExtendingElementsBefore { get; }

        private List<Element> ExtendingElementsAfter { get; }

        private Element PhrasePart { get; set; }

        private bool IsParticiple { get; }

        public string Word { get; }

        public IProcessable Object { get; set; }

        public IProcessable DependingDrawable { get; set; }

        public Verb(int Id, string Lemma, string Dependency, bool isParticiple, string word) : base(Id, Lemma, Dependency)
        {
            this.VerbFormHelper = new VerbFormHelper();
            this.RelatedActions = new List<Verb>();
            this.ExtendingElementsAfter = new List<Element>();
            this.ExtendingElementsBefore = new List<Element>();
            this.IsParticiple = isParticiple;
            this.Word = word;
        }

        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Verb verb: return this.ProcessElement(verb, graph);
                case Adverb adv: return this.ProcessElement(adv, graph);
                case Adposition adp: return this.ProcessElement(adp, graph);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Adverb adv, SentenceGraph graph)
        {
            if (adv.DependencyType.Contains("compound"))
                this.PhrasePart = adv;
            else if (adv.Id > this.Id)
                this.ExtendingElementsAfter.Add(adv);
            else 
                this.ExtendingElementsBefore.Add(adv);

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, SentenceGraph graph)
        {
            if (adp.DependencyType.Contains("compound"))
                this.PhrasePart = adp;

            return this;
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            if (noun.DependencyType == "nsubj")
                return noun.Process(this, graph);

            if (noun.DependencyType == "dobj")
                this.Object = noun;
            else
                this.DependingDrawable = noun;

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            if (nounSet.DependencyType == "nsubj")
                return nounSet.Process(this, graph);

            if (nounSet.DependencyType == "dobj")
                this.Object = nounSet;
            else
                this.DependingDrawable = nounSet;

            return this;
        }

        private IProcessable ProcessElement(Verb verb, SentenceGraph graph)
        {
            this.RelatedActions.Add(verb);
            return this;
        }

        private string GetFinalWordSequence()
        {
            string final = "";
            if (this.ExtendingElementsBefore.Count != 0)
                final = $"{string.Join(" ", this.ExtendingElementsBefore)} ";

            final += IsParticiple ? this.Word : this.VerbFormHelper.CreatePastParticipleTense(this.Lemma);
            final += this.PhrasePart != null ? $" {this.PhrasePart}" : "";

            if (this.Object != null)
                final += $" {this.Object}";

            if (this.ExtendingElementsAfter.Count != 0)
                final += $" {string.Join(" ", this.ExtendingElementsAfter)}";

            if (this.RelatedActions.Count != 0)
                final += $" {string.Join(" and ", this.RelatedActions)}";

            return final;
        }

        public override string ToString()
        {
            return this.GetFinalWordSequence();
        }
    }
}
