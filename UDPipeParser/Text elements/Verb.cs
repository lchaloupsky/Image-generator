using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Class representing VERB in sentence
    /// </summary>
    public class Verb : Element
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

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
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

        #region Processing concrete elements

        private IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            // check if it is part of this verb
            if (this.DependencyHelper.IsCompound(adv.DependencyType))
                this.PhrasePart = adv;
            else if (adv.Id > this.Id)
                this.ExtensionsAfter.Add(adv);
            else 
                this.ExtensionsBefore.Add(adv);

            return this;
        }

        private IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            if (adj.Id > this.Id)
                this.ExtensionsAfter.Add(adj);
            else
                this.ExtensionsBefore.Add(adj);

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, ISentenceGraph graph)
        {
            // Check if adposition is part of this verb
            if (this.DependencyHelper.IsCompound(adp.DependencyType))
                this.PhrasePart = adp;
            else
                this.DrawableAdposition = adp;

            return this;
        }

        private IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            // if noun is subject, process this
            if (this.DependencyHelper.IsSubject(noun.DependencyType))
            {
                if (this.DrawableAdposition != null)
                {
                    noun.Process(this.DrawableAdposition, graph);
                    this.DrawableAdposition = null;
                }

                return noun.Process(this, graph);
            }                

            // if noun is object, save it as part of this verb, else save it for future
            if (this.DependencyHelper.IsObject(noun.DependencyType))
                this.Object = noun;
            else
                this.DependingDrawables.Add(noun);

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            // Check if it is subject
            if (this.DependencyHelper.IsSubject(nounSet.DependencyType))
            {
                if (this.DrawableAdposition != null)
                {
                    nounSet.Process(this.DrawableAdposition, graph);
                    this.DrawableAdposition = null;
                }

                return nounSet.Process(this, graph);
            }

            // Check if it is object
            if (this.DependencyHelper.IsObject(nounSet.DependencyType))
                this.Object = nounSet;
            else
                this.DependingDrawables.Add(nounSet);

            return this;
        }

        private IProcessable ProcessElement(Verb verb, ISentenceGraph graph)
        {
            this.RelatedActions.Add(verb);

            // Add depending drawables from dependency tree to future processing
            if (verb.DependingDrawables.Count != 0)
                this.DependingDrawables = verb.DependingDrawables;

            return this;
        }

        #endregion

        public override IProcessable FinalizeProcessing(ISentenceGraph graph)
        {
            // Nothing is depending -> return this
            if (this.Object == null && this.DependingDrawables.Count == 0)
                return this;

            // Let depending drawable process this
            if (this.DependingDrawables.Count != 0)
            {
                var first = this.DependingDrawables.First();
                this.DependingDrawables.Remove(first);

                return first.Process(this, graph);
            }

            // Let object process this
            var obj = this.Object;
            this.Object = null;

            return obj.Process(this, graph);
        }      

        /// <summary>
        /// Method that return final string form of this verb
        /// </summary>
        /// <returns>Constructed final string representation</returns>
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
