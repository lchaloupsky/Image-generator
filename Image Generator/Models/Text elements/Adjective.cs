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
        public Adjective(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        public override IProcessable Process(IProcessable element)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun);
                case NounSet nounSet: return this.ProcessElement(nounSet);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Noun noun)
        {
            return noun.Process(this);
        }

        private IProcessable ProcessElement(NounSet nounSet)
        {
            return nounSet.Process(this);
        }

        //Maybe redo with list?
        private IProcessable ProcessElement(Adjective adj)
        {
            this.Lemma += $" {adj}";
            return this;
        }
    }
}
