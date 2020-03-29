using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Preprocessors
{
    class MissingArticlePreprocessor : IPreprocessor
    {
        private const string ARTICLE = "the";
        private UDPipeClient Client { get; }

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

        private void PreprocessAdpositionsArticles(List<string[]> responseLines)
        {
            bool wasAdp = false;
            int lastDetIndex = 0;
            for (int i = 0; i < responseLines.Count; i++)
            {
                var parts = responseLines[i];

                if (wasAdp && this.IsDeterminer(parts[3], parts[7]))
                {
                    wasAdp = false;
                    continue;
                }

                if (this.IsAdposition(parts[3]))
                {
                    wasAdp = true;
                    lastDetIndex = i + 1;
                    continue;
                }

                if (wasAdp && this.IsSubject(parts[3]) && !this.IsNounExtension(parts[7]))
                {
                    if (this.IsSingular(parts[5]))
                        responseLines.Insert(lastDetIndex, this.GetNewDeterminerLine(lastDetIndex, int.Parse(parts[0])));

                    wasAdp = false;
                }
            }
        }
        private string PreprocessMissingArticles(List<string[]> taggedSentence)
        {
            Dictionary<int, bool> nounsWithArticle = new Dictionary<int, bool>();
            Dictionary<int, int> finalIndex = new Dictionary<int, int>();
            List<string> finalPartsToJoin = new List<string>();
            int i = taggedSentence.Count;
            foreach (var parts in Enumerable.Reverse(taggedSentence))
            {
                var id = int.Parse(parts[0]);
                var index = int.Parse(parts[6]);
                i--;

                finalPartsToJoin.Add(parts[1]);

                if (this.IsSubject(parts[3]))
                {
                    if (!this.IsSingular(parts[5]) || this.IsObject(parts[7]))
                        continue;

                    if (this.IsNounExtension(parts[7]))
                    {
                        finalIndex[index] = i;
                        continue;
                    }

                    finalIndex[id] = i;
                    if(!nounsWithArticle.ContainsKey(id))
                        nounsWithArticle.Add(id, false);
                }

                if (this.IsExtension(parts[3]))
                    if (nounsWithArticle.ContainsKey(index) || nounsWithArticle.ContainsKey(int.Parse(taggedSentence[Math.Max(index - 1, 0)][6])))
                        finalIndex[index] = i;

                if (this.IsVerbExtension(parts[3], parts[7]))
                    if (finalIndex.ContainsKey(index))
                        finalIndex[index] = i;

                if (this.IsDeterminer(parts[3], parts[7]))
                    nounsWithArticle[index] = true;
            }

            finalPartsToJoin.Reverse();
            foreach (var noun in nounsWithArticle)
            {
                if (!noun.Value)
                {
                    finalPartsToJoin.Insert(finalIndex[noun.Key], ARTICLE);

                    if (finalIndex[noun.Key] == 0)
                    {
                        finalPartsToJoin[0] = finalPartsToJoin[0].First().ToString().ToUpper() + finalPartsToJoin[0].Substring(1);
                        finalPartsToJoin[1] = finalPartsToJoin[1].First().ToString().ToLower() + finalPartsToJoin[1].Substring(1);
                    }
                }
            }

            return string.Join(" ", finalPartsToJoin);
        }

        private string[] GetNewDeterminerLine(int id, int index)
        {
            return new string[] { id.ToString(), ARTICLE, ARTICLE, "DET", "DT", "_", index.ToString(), "det", "_", "_" };
        }

        private List<string[]> GetSplittedLines(List<string> lines)
        {
            return lines
                .Select(line => line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries))
                .ToList();
        }

        private bool IsObject(string relation)
        {
            return relation == "dobj";
        }

        private bool IsExtension(string partOfSpeech)
        {
            return partOfSpeech == "ADJ" || partOfSpeech == "ADV";
        }

        private bool IsVerbExtension(string partOfSpeech, string relation)
        {
            return partOfSpeech == "VERB" && relation == "amod";
        }

        private bool IsAdposition(string partOfSpeech)
        {
            return partOfSpeech == "ADP" || partOfSpeech == "PART";
        }

        private bool IsDeterminer(string partOfSpeech, string relation)
        {
            return partOfSpeech == "DET" || (partOfSpeech == "PRON" && relation == "nmod:poss");
        }

        private bool IsSingular(string otherParams)
        {
            return otherParams.Contains("Number=Sing");
        }

        private bool IsNounExtension(string relationType)
        {
            return relationType == "compound" || relationType == "nmod:poss";
        }

        private bool IsSubject(string partOfSpeech)
        {
            return partOfSpeech == "NOUN" || partOfSpeech == "PROPN";
        }
    }
}
