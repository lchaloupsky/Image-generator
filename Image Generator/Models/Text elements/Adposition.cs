﻿using Image_Generator.Models.Interfaces;
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
        private List<Adjective> Extensions { get; }

        public Adposition(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<Adjective>();
        }

        public IEnumerable<Adposition> GetAdpositions()
        {
            return this.DependingAdposition == null ?
                    new List<Adposition>() { this } :
                    new List<Adposition>() { DependingAdposition, this };
        }

        public override IProcessable ProcessElement(IProcessable element, SentenceGraph graph)
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

        private IProcessable ProcessElement(Adposition adp, SentenceGraph graph)
        {
            this.DependingAdposition = adp;

            return this;
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            if (noun.DependencyType == "compound" || noun.DependencyType == "nmod:poss")
                noun.DependencyType = this.DependencyType;

            return noun.Process(this, graph);
        }

        private IProcessable ProcessElement(Verb verb, SentenceGraph graph)
        {
            return verb.Process(this, graph);
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            return nounSet.Process(this, graph);
        }

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
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