using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDPipeParsing.Interfaces;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Preprocessors
{
    /// <summary>
    /// Find and fill missing articles in given text
    /// </summary>
    public class MissingArticlePreprocessor : IPreprocessor
    {
        // default article to fill
        private const string ARTICLE = "the";

        private UDPipeClient Client { get; }

        private DependencyTypeHelper DependencyTypeHelper { get; } = new DependencyTypeHelper();

        public MissingArticlePreprocessor(UDPipeClient client)
        {
            this.Client = client;
        }

        public string Preprocess(string sentence)
        {
            var response = this.GetSplittedLines(this.Client.GetResponse(sentence));
            this.PreprocessAdpositionsArticles(response);

            return PreprocessMissingArticles(response);
        }

        /// <summary>
        /// Fill missing articles after adpositions
        /// </summary>
        /// <param name="responseLines">UDPipe response lines</param>
        private void PreprocessAdpositionsArticles(List<string[]> responseLines)
        {
            bool wasAdp = false;
            int lastDetIndex = 0;
            for (int i = 0; i < responseLines.Count; i++)
            {
                var parts = responseLines[i];

                // If determiner is found, dont add new article
                if (wasAdp && this.IsDeterminer(parts[3], parts[7]))
                {
                    wasAdp = false;
                    continue;
                }

                // Remember last determiner index
                if (this.IsAdposition(parts[3], parts[2]))
                {
                    wasAdp = true;
                    lastDetIndex = i + 1;
                    continue;
                }

                // If adposition was found
                if (wasAdp && this.IsSubject(parts[3]) && !this.IsNounExtension(parts[7]))
                {
                    // If noun is singular add new article
                    if (this.IsSingular(parts[5]))
                        responseLines.Insert(lastDetIndex, this.GetNewDeterminerLine(lastDetIndex, int.Parse(parts[0])));

                    wasAdp = false;
                }
            }
        }

        /// <summary>
        /// Fill missing articles for nouns 
        /// </summary>
        /// <param name="responseLines">UDPipe response lines</param>
        /// <returns>Preprocessed text</returns>
        private string PreprocessMissingArticles(List<string[]> responseLines)
        {
            // Dictionary indicating if noun has already article
            Dictionary<int, bool> nounsWithArticle = new Dictionary<int, bool>();

            // Final indices, where to put new article
            Dictionary<int, int> finalIndex = new Dictionary<int, int>();

            // Final string parts
            List<string> finalPartsToJoin = new List<string>();

            // Go through response in inverse order
            int i = responseLines.Count;
            foreach (var parts in Enumerable.Reverse(responseLines))
            {
                var id = int.Parse(parts[0]);
                var index = int.Parse(parts[6]);
                i--;

                finalPartsToJoin.Add(parts[1]);
 
                if (this.IsSubject(parts[3]))
                {
                    // Not singular -> do not add article
                    if (!this.IsSingular(parts[5]))
                        continue;

                    // Noun extension (adjective, adverb, ...) 
                    if (this.IsNounExtension(parts[7]))
                    {
                        finalIndex[index] = i;
                        continue;
                    }

                    // Add new article if noun does not have already one 
                    finalIndex[id] = i;
                    if (!nounsWithArticle.ContainsKey(id))
                        nounsWithArticle.Add(id, false);
                }

                // Shift final index where to add new article for extensions
                if (this.IsExtension(parts[3]))
                    if (nounsWithArticle.ContainsKey(index) || nounsWithArticle.ContainsKey(int.Parse(responseLines[Math.Max(index - 1, 0)][6])))
                        finalIndex[index] = i;

                // Shift final index for verb extensions
                if (this.IsVerbExtension(parts[3], parts[7]))
                    if (finalIndex.ContainsKey(index))
                        finalIndex[index] = i;

                // Register that noun has already determiner
                if (this.IsDeterminer(parts[3], parts[7]))
                    nounsWithArticle[index] = true;
            }

            // Go through in original order and build final sentence string
            finalPartsToJoin.Reverse();
            foreach (var noun in nounsWithArticle)
            {
                // If noun does not have article add it
                if (!noun.Value)
                {
                    finalPartsToJoin.Insert(finalIndex[noun.Key], ARTICLE);

                    // Preserve original form
                    if (finalIndex[noun.Key] == 0)
                    {
                        finalPartsToJoin[0] = finalPartsToJoin[0].First().ToString().ToUpper() + finalPartsToJoin[0].Substring(1);
                        finalPartsToJoin[1] = finalPartsToJoin[1].First().ToString().ToLower() + finalPartsToJoin[1].Substring(1);
                    }

                }
            }

            // Return preprocessed text
            return string.Join(" ", finalPartsToJoin);
        }

        /// <summary>
        /// Returns new UDPipe like reponse line
        /// </summary>
        /// <param name="id">Id of element</param>
        /// <param name="index">Dependency index</param>
        /// <returns>response line</returns>
        private string[] GetNewDeterminerLine(int id, int index)
        {
            return new string[] { id.ToString(), ARTICLE, ARTICLE, "DET", "DT", "_", index.ToString(), "det", "_", "_" };
        }

        /// <summary>
        /// Splits response line into string array
        /// </summary>
        /// <param name="lines">Lines to split</param>
        /// <returns>List of string array splitted reponse lines</returns>
        private List<string[]> GetSplittedLines(List<string> lines)
        {
            return lines
                .Select(line => line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries))
                .ToList();
        }

        private bool IsExtension(string partOfSpeech)
        {
            return partOfSpeech == "ADJ" || partOfSpeech == "ADV";
        }

        private bool IsVerbExtension(string partOfSpeech, string relation)
        {
            return partOfSpeech == "VERB" && this.DependencyTypeHelper.IsAdjectivalModifier(relation);
        }

        private bool IsAdposition(string partOfSpeech, string lemma)
        {
            return partOfSpeech == "ADP" || (partOfSpeech == "PART" && this.IsAllowedParticle(lemma));
        }

        private bool IsAllowedParticle(string lemma)
        {
            return !lemma.Contains("'") && lemma != "not"; 
        }

        private bool IsDeterminer(string partOfSpeech, string relation)
        {
            return partOfSpeech == "DET" || ((partOfSpeech == "PRON" || partOfSpeech == "NOUN") && this.DependencyTypeHelper.IsNominalPossesive(relation));
        }

        private bool IsSingular(string otherParams)
        {
            return otherParams.Contains("Number=Sing");
        }

        private bool IsNounExtension(string relationType)
        {
            return this.DependencyTypeHelper.IsCompound(relationType) || this.DependencyTypeHelper.IsNominalPossesive(relationType);
        }

        private bool IsSubject(string partOfSpeech)
        {
            return partOfSpeech == "NOUN" || partOfSpeech == "PROPN";
        }
    }
}
