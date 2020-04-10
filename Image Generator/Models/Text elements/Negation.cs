using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Negation : Element
    {
        public Negation(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency){}

        public override IProcessable ProcessElement(IProcessable element, SentenceGraph graph)
        {
            return this;
        }

        public override string ToString()
        {
            return "NOT";
        }
    }
}
