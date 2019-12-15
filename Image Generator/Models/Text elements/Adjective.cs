using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Image_Generator.Models.Interfaces;

namespace Image_Generator.Models.Text_elements
{
    class Adjective : Element
    {
        public Adjective(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency){}

        public override IProcessable Process(IProcessable element)
        {
            switch (element)
            {
                case Noun noun:
                    return noun.Process(this);
                case NounSet nounSet:
                    return nounSet.Process(this);
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
