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
    /// Represents numeral element in the sentence
    /// </summary>
    public class Numeral : Element
    {
        // Dependant drawable in the tree
        public IProcessable DependingDrawable { get; set; }

        public Numeral(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Coordination cor: return this.ProcessCoordination(cor);
                default: break;
            }

            return this;
        }

        #region Processing concrete elements

        private IProcessable ProcessElement<T>(T drawable, ISentenceGraph graph) where T : IProcessable
        {
            if (this.DependencyHelper.IsAppositional(this.DependencyType))
            {
                this.DependingDrawable = drawable;
                return this;
            }

            this.DependencyType = "nummod";
            return drawable.Process(this, graph);
        }

        #endregion

        /// <summary>
        /// Gets numerical value of numeral
        /// </summary>
        /// <returns>Numerical value of this numeral</returns>
        public int GetValue()
        {
            return int.Parse(this.Lemma);
        }
    }
}
