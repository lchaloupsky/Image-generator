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
        private const string FIRST_WORD_ARTICLE = "The";

        // Client to downloading UDPipe responses
        private UDPipeClient Client { get; }

        // Helper to resolving dependencies
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
                // in the second part of condition -- "to" is forbiden in other cases
                if (this.IsAdposition(parts[3], parts[2], parts[7]) || (i > 0 && parts[2] == "to" && responseLines[i-1][2] == "next"))
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
                {
                    // transitive dependency
                    var idx = int.Parse(responseLines[Math.Max(index - 1, 0)][6]);

                    // Check belonging indices ??? check this change
                    if (nounsWithArticle.ContainsKey(index))
                        finalIndex[index] = i;                
                    else if (nounsWithArticle.ContainsKey(idx))
                        finalIndex[idx] = i;
                }

                // Shift final index for verb extensions
                if (this.IsVerbExtension(parts[3], parts[7]))
                    if (finalIndex.ContainsKey(index))
                        finalIndex[index] = i;

                // Register that noun has already determiner
                if (this.IsDeterminer(parts[3], parts[7]))
                {
                    if (this.DependencyTypeHelper.IsPossesive(responseLines[index - 1][7]))
                        index = int.Parse(responseLines[index - 1][6]);

                    nounsWithArticle[index] = true;
                }
            }

            // Checking if first word is verb in the gerund form
            // If has belonging noun, check if already has article and if not, add it
            int firstIdOfTree = 1;
            var firstLine = responseLines[0];
            if (this.IsVerbExtension(firstLine[3], firstIdOfTree, firstLine[5]))
            {
                var index = int.Parse(firstLine[6]);
                if (finalIndex.ContainsKey(index))
                    finalIndex[index] = 0;

                // Check if element is root, then the dependency is upside down
                else if (this.DependencyTypeHelper.IsRoot(firstLine[7]))
                {
                    var minNounId = finalIndex.Min(fi => fi.Key);
                    if (int.Parse(responseLines[minNounId - 1][6]) == firstIdOfTree)
                        finalIndex[minNounId] = 0;
                }
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

            // Check if first word is adjective and no article is before it -- standalone adjective/abverb
            if ((this.IsExtension(firstLine[3]) || this.IsStandAloneVerb(firstLine[3], firstIdOfTree, firstLine[5])) && !finalIndex.ContainsValue(0))
            {
                var lastPart = finalPartsToJoin.First();
                finalPartsToJoin.RemoveAt(0);
                finalPartsToJoin.Insert(0, lastPart.ToLower());
                finalPartsToJoin.Insert(0, FIRST_WORD_ARTICLE);
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

        /// <summary>
        /// Checks if the given word is adjective or adverb
        /// </summary>
        /// <param name="partOfSpeech">Part of speech of the word</param>
        /// <returns>True if satisfied</returns>
        private bool IsExtension(string partOfSpeech)
        {
            return partOfSpeech == "ADJ" || partOfSpeech == "ADV";
        }

        /// <summary>
        /// Checks if the first element in the sentence is standalone verb, thus is incorrectly tagged
        /// </summary>
        /// <param name="partofSpeech">Word part of speech</param>
        /// <param name="id">Word id</param>
        /// <param name="verbParams">Other word params from UDPipe</param>
        /// <returns>True if satisfied</returns>
        private bool IsStandAloneVerb(string partofSpeech, int id, string verbParams)
        {
            return partofSpeech == "VERB" && id == 1 && verbParams.Contains("Mood=Imp");
        }

        /// <summary>
        /// Checks if word is verb extending a noun
        /// </summary>
        /// <param name="partOfSpeech">Words part of speech</param>
        /// <param name="id">Word id</param>
        /// <param name="verbParams">Other verb params</param>
        /// <returns>True if satisfied</returns>
        private bool IsVerbExtension(string partOfSpeech, int id, string verbParams)
        {
            return partOfSpeech == "VERB" && id == 1 && verbParams.Contains("VerbForm=Ger");
        }

        /// <summary>
        /// Checks if word is verb extending a noun
        /// </summary>
        /// <param name="partOfSpeech">Words part of speech</param>
        /// <param name="relation">Words dependency tree relation</param>
        /// <returns>True if satisfied</returns>
        private bool IsVerbExtension(string partOfSpeech, string relation)
        {
            return partOfSpeech == "VERB" && this.DependencyTypeHelper.IsAdjectivalModifier(relation);
        }

        /// <summary>
        /// Checks if word is adposition or adpositional particle
        /// </summary>
        /// <param name="partOfSpeech">Word part of speech</param>
        /// <param name="lemma">Word lemma</param>
        /// <param name="dependencyType">Word dependency tree relation</param>
        /// <returns>True if satisfied</returns>
        private bool IsAdposition(string partOfSpeech, string lemma, string dependencyType)
        {
            return partOfSpeech == "ADP" || (partOfSpeech == "PART" && this.IsAllowedParticle(lemma, dependencyType));
        }

        /// <summary>
        /// Checks if word is allowed particle
        /// </summary>
        /// <param name="lemma">Word lemma</param>
        /// <param name="dependencyType">Word dependency tree relation</param>
        /// <returns>True if satisfied</returns>
        private bool IsAllowedParticle(string lemma, string dependencyType)
        {
            return !lemma.Contains("'") && lemma != "not" && !this.DependencyTypeHelper.IsMark(dependencyType);
        }

        /// <summary>
        /// Checks if word is determiner(article)
        /// </summary>
        /// <param name="partOfSpeech">Words part of speech</param>
        /// <param name="relation">Word dependency tree relation</param>
        /// <returns>True if satisfied</returns>
        private bool IsDeterminer(string partOfSpeech, string relation)
        {
            return partOfSpeech == "DET"
                || ((partOfSpeech == "PRON"
                    || partOfSpeech == "NOUN") && this.DependencyTypeHelper.IsNominalPossesive(relation));
        }

        /// <summary>
        /// Checks if given noun params indicating singular form of a noun
        /// </summary>
        /// <param name="otherParams">Noun params from UDPipe</param>
        /// <returns>True if satisfied</returns>
        private bool IsSingular(string otherParams)
        {
            return otherParams.Contains("Number=Sing");
        }

        /// <summary>
        /// Checks if word describing a noun extending another noun
        /// </summary>
        /// <param name="relationType">Word dependency tree relation</param>
        /// <returns>True if satisfied</returns>
        private bool IsNounExtension(string relationType)
        {
            return this.DependencyTypeHelper.IsCompound(relationType)
                || this.DependencyTypeHelper.IsNominalPossesive(relationType)
                || this.DependencyTypeHelper.IsName(relationType);
        }

        /// <summary>
        /// Checks if word is Noun or Proper noun -> thus is "subject"
        /// </summary>
        /// <param name="partOfSpeech">Words part of speech</param>
        /// <returns>True if satisfied</returns>
        private bool IsSubject(string partOfSpeech)
        {
            return partOfSpeech == "NOUN" || partOfSpeech == "PROPN";
        }
    }
}
