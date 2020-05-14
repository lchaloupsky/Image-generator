using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents auxiliary element in the sentence (is, are..)
    /// </summary>
    public class Auxiliary : Element
    {
        public Auxiliary(int id, string lemma, string dependency) : base(id, lemma, dependency) { }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            return element;
        }
    }
}
