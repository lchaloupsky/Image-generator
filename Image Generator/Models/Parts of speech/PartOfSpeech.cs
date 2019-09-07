using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Parts_of_speech
{
    /// <summary>
    /// Abstract class for parts of speech
    /// </summary>
    abstract class PartOfSpeech
    {
        // Word form in text
        public string Word { get; protected set; }

        // Lemma of the word
        public string Lemma { get; protected set; }

        // Word ID in the text
        public int ID { get; }

        // TODO some needed properties?
        // TODO in FUTURE some links to other parts?

        public PartOfSpeech(string word, string lemma, int id)
        {
            this.Word = word;
            this.Lemma = lemma;
        }

        /// <summary>
        /// Method for debug listing  
        /// </summary>
        public void Print()
        {
            Console.WriteLine("Word: {0}\n"
                            + "Lemma: {1}\n"
                            , this.Word, this.Lemma);
        }

        // Preliminarily methods that will be maybe helpful

        abstract public void Merge(List<PartOfSpeech> parts);

        protected void AddWordToBeginning(string word)
        {
            this.Word = word + " " + this.Word;
        }

        protected void AddWordToEnd(string word)
        {
            this.Word += " " + word;
        }

        /// <summary>
        /// Overriden method ToString
        /// </summary>
        /// <returns>String representation of part of speech</returns>
        public override string ToString()
        {
            return $"Word: {this.Word}, Lemma: {this.Lemma}";
        }

        // TODO in FUTURE necessary abstract methods to process parts?
    }
}
