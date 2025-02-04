﻿using System.Collections.Generic;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents numeral element in the sentence
    /// </summary>
    public class Numeral : Element
    {
        private static readonly int DEFAULT_NUMBER = 3;

        // Dependent drawable in the tree
        public List<IProcessable> DependingDrawables { get; private set; }

        public List<IProcessable> DependingActions { get; private set; }

        public IProcessable DependingNumeral { get; private set; }

        public Numeral(int id, string lemma, string dependency) : base(id, lemma, dependency)
        {
            this.DependingDrawables = new List<IProcessable>();
            this.DependingActions = new List<IProcessable>();
        }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Coordination cor: return this.ProcessCoordination(cor);
                case Verb verb: return this.ProcessElement(verb, graph);
                case Numeral num: return this.ProcessElement(num, graph);
                default: break;
            }

            return this;
        }

        #region Processing concrete elements

        private IProcessable ProcessElement<T>(T drawable, ISentenceGraph graph) where T : IProcessable
        {
            if (this.DependencyHelper.IsAppositional(this.DependencyType))
            {
                this.DependingDrawables.Add(drawable);
                return this;
            }

            this.DependencyType = "nummod";
            return drawable.Process(this, graph);
        }

        private IProcessable ProcessElement(Verb verb, ISentenceGraph graph)
        {
            this.DependingActions.Add(verb);

            return this;
        }

        private IProcessable ProcessElement(Numeral num, ISentenceGraph graph)
        {
            num.DependencyType = this.DependencyType;
            this.DependingNumeral = num;

            return this;
        }

        #endregion

        /// <summary>
        /// Gets numerical value of numeral
        /// </summary>
        /// <returns>Numerical value of this numeral</returns>
        public int GetValue()
        {
            return int.TryParse(this.Lemma, out int outValue) ? outValue : DEFAULT_NUMBER;
        }
    }
}
