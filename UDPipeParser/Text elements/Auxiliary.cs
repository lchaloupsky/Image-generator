using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents auxiliary element in the sentence (is, are..)
    /// </summary>
    public class Auxiliary : Element
    {
        public Auxiliary(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            return element;
        }
    }
}
