using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Text_elements
{
    public abstract class Element : IProcessable
    {
        public int Id { get; }

        public string Lemma { get; protected set; }

        public string DependencyType { get; set; }

        public CoordinationType CoordinationType { get; set; } = CoordinationType.AND;

        public bool IsNegated { get; protected set; }

        public Element(int Id, string Lemma, string DependencyType)
        {
            this.Id = Id;
            this.Lemma = Lemma;
            this.DependencyType = DependencyType;
        }

        public Element(int Id) : this(Id, "", "") { }

        public override string ToString()
        {
            return this.Lemma;
        }

        public virtual IProcessable FinalizeProcessing(ISentenceGraph graph)
        {
            return this;
        }

        public IProcessable Process(IProcessable element, ISentenceGraph graph)
        {
            if (element is Negation)
                return this.ProcessNegation((Negation)element);

            if (element.IsNegated && !(this is Noun) && !(element is Verb))
                return this;

            if (this.IsNegated && element.DependencyType == "nsubj")
                return element;

            if (!this.IsAllowedCoordination() && element.DependencyType == "conj")
            {
                this.CoordinationType = CoordinationType.AND;
                return this;
                //Remove vertex?
            }

            var returnElement = this.ProcessElement(element, graph);
            return returnElement.IsNegated && returnElement != this ? this : returnElement;
        }

        public abstract IProcessable ProcessElement(IProcessable element, ISentenceGraph graph);

        protected IProcessable ProcessCoordination(Coordination coordination)
        {
            this.CoordinationType = coordination.CoordinationType;
            return this;
        }

        private IProcessable ProcessNegation(Negation negation)
        {
            this.IsNegated = true;
            return this;
        }

        protected bool IsAllowedCoordination()
        {
            return this.CoordinationType != CoordinationType.OR && this.CoordinationType != CoordinationType.BUT;
        }
    }
}
