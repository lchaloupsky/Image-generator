using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents negation in the sentence
    /// </summary>
    public class Negation : Element
    {
        public Negation(int id, string lemma, string dependency) : base(id, lemma, dependency) { }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            // Depending elements are omitted because they are negated
            return this;
        }

        public override string ToString()
        {
            return "NOT";
        }
    }
}
