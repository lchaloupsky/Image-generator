using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Class representing one word(element) in the sentence
    /// </summary>
    public abstract class Element : IProcessable
    {
        // IProcessable properties 
        public int Id { get; }
        public string Lemma { get; protected set; }
        public string DependencyType { get; set; }

        // Actual coordination type between elements
        public CoordinationType CoordinationType { get; set; } = CoordinationType.AND;

        // Flag indicating if element is negated
        public bool IsNegated { get; protected set; }

        // Helper for resolving dependency types
        protected DependencyTypeHelper DependencyHelper { get; }

        // Helper for resolving coordination types
        protected CoordinationTypeHelper CoordinationTypeHelper { get; }

        public Element(int Id, string Lemma, string DependencyType)
        {
            this.Id = Id;
            this.Lemma = Lemma;
            this.DependencyType = DependencyType;
            this.DependencyHelper = new DependencyTypeHelper();
            this.CoordinationTypeHelper = new CoordinationTypeHelper();
        }

        public override string ToString()
        {
            return this.Lemma;
        }

        /// <summary>
        /// Interface overriden method for finalizing element processing
        /// </summary>
        /// <param name="graph">Sentence graph</param>
        /// <returns>Finalized element</returns>
        public virtual IProcessable FinalizeProcessing(ISentenceGraph graph)
        {
            return this;
        }

        /// <summary>
        /// Interface overriden method
        /// </summary>
        /// <param name="element">Element to process</param>
        /// <param name="graph">Sentence graph</param>
        /// <returns>Processed element</returns>
        public IProcessable Process(IProcessable element, ISentenceGraph graph)
        {
            if (element is Negation)
                return this.ProcessNegation((Negation)element);

            if (element.IsNegated && !(this is Noun) && !(element is Verb))
                return this;

            if (this.IsNegated && this.DependencyHelper.IsSubject(element.DependencyType))
                return element;

            if (!this.CoordinationTypeHelper.IsAllowedCoordination(this.CoordinationType) && this.DependencyHelper.IsConjuction(element.DependencyType))
            {
                this.CoordinationType = CoordinationType.AND;
                return this;
            }

            var returnElement = this.ProcessElement(element, graph);
            return returnElement.IsNegated && returnElement != this ? this : returnElement;
        }

        /// <summary>
        /// Each child of this class processes element by themself.
        /// </summary>
        /// <param name="element">Element to process</param>
        /// <param name="graph">Sentence graph</param>
        /// <returns>Processed element</returns>
        protected abstract IProcessable ProcessElement(IProcessable element, ISentenceGraph graph);

        /// <summary>
        /// Processing coordination
        /// </summary>
        /// <param name="coordination">Coordination element</param>
        /// <returns>Processed element</returns>
        protected IProcessable ProcessCoordination(Coordination coordination)
        {
            this.CoordinationType = coordination.CoordinationType;
            return this;
        }

        /// <summary>
        /// Processing negation
        /// </summary>
        /// <param name="negation">Negation element</param>
        /// <returns>Processed element</returns>
        private IProcessable ProcessNegation(Negation negation)
        {
            this.IsNegated = true;
            return this;
        }
    }
}
