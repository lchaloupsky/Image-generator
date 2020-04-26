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
    /// Represents negation in the sentence
    /// </summary>
    public class Negation : Element
    {
        public Negation(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency){}

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            // Depending elements are ommited because they are negated
            return this;
        }

        public override string ToString()
        {
            return "NOT";
        }
    }
}
