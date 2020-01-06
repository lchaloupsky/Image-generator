using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Adposition : Element
    {
        public Adposition(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        public override IProcessable Process(IProcessable element)
        {
            switch (element)
            {
                case Noun noun:
                    return noun.Process(this);
                default:
                    break;
            }

            return this;
        }

        public override string ToString()
        {
            return this.Lemma;
        }
    }
}