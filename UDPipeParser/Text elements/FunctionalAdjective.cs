using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using UDPipeParsing.Text_elements;

namespace UDPipeParsing.Text_elements
{
    public class FunctionalAdjective : Adjective
    {
        public float Scale { get; set; }

        private HashSet<string> ScaleModifiers { get; } = new HashSet<string>() { "very", "extremely", "quite", "really", "terribly", "too" };

        public FunctionalAdjective(int Id, string Lemma, string Dependency, float scale) : base(Id, Lemma, Dependency)
        {
            this.Scale = scale;
        }

        protected sealed override IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            if (adj is FunctionalAdjective)
                return this;

            return base.ProcessElement(adj, graph);
        }

        protected sealed override IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            adv.ExtendingAdverbs.SelectMany(advs => advs.ExtendingAdverbs).Select(ad => 
            {
                if(this.ChangeScale(ad))
                    this.Extensions.Add(ad);

                return ad;
            }).ToList();

            if (this.ChangeScale(adv))
                this.Extensions.Add(adv);

            return this;
        }

        private bool ChangeScale(Adverb adv)
        {
            if (this.ScaleModifiers.Contains(adv.Lemma))
            {
                this.Scale = this.Scale > 1 ? this.Scale * 1.75f : this.Scale / 1.75f;
                return true;
            }

            return false;
        }
    }
}
