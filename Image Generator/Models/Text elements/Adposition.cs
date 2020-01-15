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
        private Adposition DependingAdposition { get; set; }

        public Adposition(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency) { }

        public IEnumerable<Adposition> GetAdpositions()
        {
            return this.DependingAdposition == null ? new List<Adposition>() { this } : new List<Adposition>() { DependingAdposition, this };
        }

        public override IProcessable Process(IProcessable element)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun);
                case NounSet nounSet: return this.ProcessElement(nounSet);
                case Adposition adp: return this.ProcessElement(adp);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Adposition adp)
        {
            this.DependingAdposition = adp;
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
    }
}