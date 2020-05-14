using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Parsing;
using System.Collections.Generic;
using System.Linq;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents functional adjective in the sentence (big, small, etc.)
    /// </summary>
    public class FunctionalAdjective : Adjective
    {
        // Scale modifiers
        private static HashSet<string> ScaleModifiers { get; } = new HashSet<string>() { "very", "extremely", "quite", "really", "terribly", "too" };

        // Change of scale, when if functional adverb is found
        private const float ScaleChange = 1.5f;

        // Size scale
        public float Scale { get; set; }

        public FunctionalAdjective(int id, string lemma, string dependency, float scale) : base(id, lemma, dependency)
        {
            this.Scale = scale;
        }

        #region Processing conrete elements

        protected sealed override IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            if (adj is FunctionalAdjective)
                return this;

            return base.ProcessElement(adj, graph);
        }

        protected sealed override IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            // Go through all adverbs and find modifier ones
            adv.ExtendingAdverbs.SelectMany(advs => advs.ExtendingAdverbs).Select(ad =>
            {
                if (this.ChangeScale(ad))
                    this.Extensions.Add(ad);

                return ad;
            }).ToList();

            if (this.ChangeScale(adv))
                this.Extensions.Add(adv);

            return this;
        }

        /// <summary>
        /// Changes scale if adverb is change scale modifier
        /// </summary>
        /// <param name="adv">Adverb</param>
        /// <returns>True if scale was changed</returns>
        private bool ChangeScale(Adverb adv)
        {
            if (ScaleModifiers.Contains(adv.Lemma))
            {
                this.Scale = this.Scale > 1 ? this.Scale * ScaleChange : this.Scale / ScaleChange;
                return true;
            }

            return false;
        }

        #endregion
    }
}
