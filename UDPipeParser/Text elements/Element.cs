using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
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

        // Helper for processable elements
        protected ProcessableHelper ProcessableHelper { get; }

        protected Element(int id, string lemma, string dependencyType)
        {
            this.Id = id;
            this.Lemma = lemma;
            this.DependencyType = dependencyType;
            this.DependencyHelper = new DependencyTypeHelper();
            this.CoordinationTypeHelper = new CoordinationTypeHelper();
            this.ProcessableHelper = new ProcessableHelper();
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
            if (element == null)
                return this;

            if (element is Negation)
                return this.ProcessNegation((Negation)element);

            if (element.IsNegated && !(this is Noun) && !(element is Verb))
            {
                if (element is IDrawable drawable)
                    graph.RemoveVertex(drawable, true);

                return this;
            }

            if (this.IsNegated && this.DependencyHelper.IsSubject(element.DependencyType))
            {
                if(this is Verb verb)
                    this.ProcessableHelper.RemoveVerbFromGraph(verb, graph);

                return element;
            }


            if (!this.CoordinationTypeHelper.IsAllowedCoordination(this.CoordinationType) && this.DependencyHelper.IsConjuction(element.DependencyType))
            {
                this.CoordinationType = CoordinationType.AND;
                if (element is IDrawable drawable)
                    graph.RemoveVertex(drawable, true);

                if (element is Verb verb)
                    this.ProcessableHelper.RemoveVerbFromGraph(verb, graph);

                return this;
            }

            var returnElement = this.ProcessElement(element, graph);
            return returnElement.IsNegated && returnElement != this ? this : returnElement;
        }

        /// <summary>
        /// Each child of this class processes element by itself.
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
