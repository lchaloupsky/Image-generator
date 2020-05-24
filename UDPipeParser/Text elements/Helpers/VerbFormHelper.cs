using System.Collections.Generic;
using System.Linq;

namespace UDPipeParsing.Text_elements.Helpers
{
    /// <summary>
    /// Class helping with verb forms
    /// </summary>
    public class VerbFormHelper
    {
        private const string IngSuffix = "ing";

        private HashSet<char> Vowels { get; }

        public VerbFormHelper()
        {
            this.Vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };
        }

        /// <summary>
        /// Creates past participle tense of given verb lemma
        /// </summary>
        /// <param name="verbLemma">Verb</param>
        /// <returns>Past participle form of verb</returns>
        public string CreatePastParticipleTense(string verbLemma)
        {
            if (verbLemma.Length < 3)
                return $"{verbLemma}ing";

            var suffix = verbLemma.Substring(verbLemma.Length - 2);
            var lemmaWithoutSuffix = verbLemma.Substring(0, verbLemma.Length - 2);
            var numberOfVowels = this.GetNumberOfVowels(verbLemma);

            // Rules for creating past participle tense
            if (suffix == "ie")
                return lemmaWithoutSuffix + "y" + IngSuffix;

            if (suffix[1] == 'y')
                return verbLemma + IngSuffix;

            if (suffix == "ee" || suffix == "ye")
                return verbLemma + IngSuffix;

            if (suffix[1] == 'e')
                return lemmaWithoutSuffix + suffix[0] + IngSuffix;

            if (suffix[0] == suffix[1])
                return verbLemma + IngSuffix;

            if (suffix[1] == 'w' || suffix[1] == 'x')
                return verbLemma + IngSuffix;

            if (verbLemma.Length <= 4 && numberOfVowels == 1 && this.Vowels.Contains(suffix[0]))
                return verbLemma + suffix[1] + IngSuffix;

            return verbLemma + IngSuffix;
        }

        /// <summary>
        /// Auxiliary method for getting number of vowels in verb
        /// </summary>
        /// <param name="verb">Verb</param>
        /// <returns>Number of vowels</returns>
        private int GetNumberOfVowels(string verb)
        {
            return verb.Count(c => this.Vowels.Contains(c));
        }
    }
}
