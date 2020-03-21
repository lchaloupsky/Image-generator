using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements.Helpers
{
    class VerbFormHelper
    {
        private HashSet<char> Vowels { get; }

        public VerbFormHelper()
        {
            this.Vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };
        }

        public string CreatePastParticipleTense(string lemma)
        {
            var suffix = lemma.Substring(lemma.Length - 2);
            var lemmaWithoutSuffix = lemma.Substring(0, lemma.Length - 2);
            var numberofVowels = this.GetNumberOfVowels(lemma);
            var ingSuffix = "ing";

            if (suffix == "ie")
                return lemmaWithoutSuffix + "y" + ingSuffix;

            if (suffix[1] == 'y')
                return lemma + ingSuffix;

            if (suffix == "ee" || suffix == "ye")
                return lemma + ingSuffix;

            if (suffix[1] == 'e')
                return lemmaWithoutSuffix + suffix[0] + ingSuffix;

            if (suffix[0] == suffix[1])
                return lemma + ingSuffix;

            if (suffix[1] == 'w' || suffix[1] == 'x')
                return lemma + ingSuffix;

            if (lemma.Length <= 4 && numberofVowels == 1 && this.Vowels.Contains(suffix[0]))
                return lemma + suffix[1] + ingSuffix;

            return lemma + ingSuffix;
        }

        private int GetNumberOfVowels(string text)
        {
            return text.Count(c => this.Vowels.Contains(c));
        }
    }
}
