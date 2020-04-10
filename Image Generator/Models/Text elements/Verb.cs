using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    /// <summary>
    /// Class representing VERB in sentence
    /// </summary>
    class Verb : Element
    {
        // --------Public properties--------
        // Related actions of this verb
        public List<Verb> RelatedActions { get; }
        
        // Original form of verb
        public string Word { get; }

        // Object of verb
        public IProcessable Object { get; set; }

        // List of drawables, that has not been processed yet
        public List<IProcessable> DependingDrawables { get; set; }

        // Adposition of drawable, that has not been processed yet
        public Adposition DrawableAdposition { get; set; }

        // Form of verb
        public VerbForm Form { get; }

        // -------Private properties--------
        // Extensions before verb
        private List<Element> ExtensionsBefore { get; }

        // Extensions after verb
        private List<Element> ExtensionsAfter { get; }

        // Phrase part after verb
        private Element PhrasePart { get; set; }    

        // Helper for constructing final verb form
        private VerbFormHelper VerbFormHelper { get; }

        public Verb(int Id, string Lemma, string Dependency, VerbForm form, string word) : base(Id, Lemma, Dependency)
        {
            this.VerbFormHelper = new VerbFormHelper();
            this.RelatedActions = new List<Verb>();
            this.ExtensionsAfter = new List<Element>();
            this.ExtensionsBefore = new List<Element>();
            this.DependingDrawables = new List<IProcessable>();
            this.Form = form;
            this.Word = word;
        }

        public override IProcessable ProcessElement(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Verb verb: return this.ProcessElement(verb, graph);
                case Adverb adv: return this.ProcessElement(adv, graph);
                case Adposition adp: return this.ProcessElement(adp, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
                case Coordination cor: return this.ProcessCoordination(cor);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Adverb adv, SentenceGraph graph)
        {
            // check if it is part of this verb
            if (adv.DependencyType.Contains("compound"))
                this.PhrasePart = adv;
            else if (adv.Id > this.Id)
                this.ExtensionsAfter.Add(adv);
            else 
                this.ExtensionsBefore.Add(adv);

            return this;
        }

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            if (adj.Id > this.Id)
                this.ExtensionsAfter.Add(adj);
            else
                this.ExtensionsBefore.Add(adj);

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, SentenceGraph graph)
        {
            if (adp.DependencyType.Contains("compound"))
                this.PhrasePart = adp;
            else
                this.DrawableAdposition = adp;

            return this;
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            // if noun is subject, process this
            if (noun.DependencyType == "nsubj" || noun.DependencyType == "nsubjpass")
            {
                if (this.DrawableAdposition != null)
                    noun.Process(this.DrawableAdposition, graph);

                return noun.Process(this, graph);
            }                

            // if noun is object, save it as part of this verb, else save it for future
            if (noun.DependencyType == "dobj")
                this.Object = noun;
            else
                this.DependingDrawables.Add(noun);

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            if (nounSet.DependencyType == "nsubj" || nounSet.DependencyType == "nsubjpass")
            {
                if (this.DrawableAdposition != null)
                    nounSet.Process(this.DrawableAdposition, graph);

                return nounSet.Process(this, graph);
            }

            if (nounSet.DependencyType == "dobj")
                this.Object = nounSet;
            else
                this.DependingDrawables.Add(nounSet);

            return this;
        }

        private IProcessable ProcessElement(Verb verb, SentenceGraph graph)
        {
            this.RelatedActions.Add(verb);
            if (verb.DependingDrawables != null)
                this.DependingDrawables = verb.DependingDrawables;

            return this;
        }

        public override IProcessable FinalizeProcessing(SentenceGraph graph)
        {
            // EXPLORE THIS PROBLEM #281 from dataset -- dobj of verb is supposed to be a subject
            if (this.Object == null && this.DependingDrawables.Count == 0)
                return this;

            // TEMPORARY
            if (this.DependingDrawables.Count != 0)
            {
                var first = this.DependingDrawables.First();
                this.DependingDrawables.Remove(first);

                return first.Process(this, graph);
            }

            var obj = this.Object;
            this.Object = null;

            return obj.Process(this, graph);
        }

        /// <summary>
        /// Method that return final string form of this verb
        /// </summary>
        /// <returns></returns>
        private string GetFinalWordSequence()
        {
            string final = "";
            if (this.ExtensionsBefore.Count != 0)
                final = $"{string.Join(" ", this.ExtensionsBefore)} ";

            final += this.Form != VerbForm.NORMAL ? this.Word : this.VerbFormHelper.CreatePastParticipleTense(this.Lemma);
            final += this.PhrasePart != null ? $" {this.PhrasePart}" : "";

            if (this.Object != null)
                final += $" {this.Object}";

            if (this.ExtensionsAfter.Count != 0)
                final += $" {string.Join(" ", this.ExtensionsAfter)}";

            if (this.RelatedActions.Count != 0)
                final += $" {string.Join(" and ", this.RelatedActions)}";

            return final;
        }

        public override string ToString()
        {
            return this.GetFinalWordSequence();
        }
    }

    /// <summary>
    /// Enum representing verb form
    /// </summary>
    public enum VerbForm
    {
        NORMAL, GERUND, PARTICIPLE
    }
}
