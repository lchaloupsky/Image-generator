using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UDPipeParsing.Interfaces;

namespace UDPipeParsing.Preprocessors
{
    /// <summary>
    /// Preprocesses number in text format into corresponding number format
    /// </summary>
    public class TextToNumberPreprocessor : IPreprocessor
    {
        private const int MaxAllowedNumber = 1_000;
        private const int MaxLengthOfCollection = 4;

        private Dictionary<string, long> Units { get; } = new Dictionary<string, long>
        {{"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
        {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
        {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
        {"fourteen",14},{"fifteen",15},{"sixteen",16},
        {"seventeen",17},{"eighteen",18},{"nineteen",19}};

        private Dictionary<string, long> Tens { get; } = new Dictionary<string, long>
        {{"twenty", 20},{"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
        {"seventy",70},{"eighty",80},{"ninety",90}};

        private Dictionary<string, long> Multiplies { get; } = new Dictionary<string, long>
        {{ "hundred",100}, {"thousand",1000},{"million",1000000},{"billion",1000000000},
        {"trillion",1000000000000},{"quadrillion",1000000000000000},
        {"quintillion",1000000000000000000}};

        private Dictionary<string, long> SmallCollections { get; } = new Dictionary<string, long>
        {{"several",2}, {"few",2},{"many",5},{"a few", 2},{"very few", 1},{"a bit",2},{"some",2}};

        private Dictionary<string, long> BigCollections { get; } = new Dictionary<string, long>
        {{ "hundreds",100}, {"thousands",1000},{"millions",1000000},{"billions",1000000000},
        {"trillions",1000000000000},{"quadrillions",1000000000000000},
        {"quintillions",1000000000000000000},{"plenty",10},{"a lot", 10},{"a lot of",10},{"lots of",10},
        { "a large number of", 20},{"a great number of", 20},{"a large quantity of", 20},{"a large amount of", 20},
        {"very many",7},{"a number of",5},{"plenty of",10}};

        private Random Random { get; } = new Random();

        public string Preprocess(string text)
        {
            long scale = 0;
            long result = 0;
            bool wasNumber = false;
            bool wasNegation = false;
            StringBuilder builder = new StringBuilder();

            var splittedText = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splittedText.Length; i++)
            {
                // Words can be also connected with '-' character
                var wordParts = splittedText[i].Split('-');
                if (i == 0)
                    wordParts[0] = wordParts[0].ToLower();

                // Check negation
                if (wordParts[0] == "not")
                    wasNegation = true;

                // Try to get collection(number phrase)
                if (wordParts.Count() == 1 && TryGetCollection(wordParts[0], splittedText, ref i, ref result, wasNumber, wasNegation, scale))
                {
                    scale = 0;
                    if (wasNegation)
                        builder.Length -= 4;
                }
                // Try to get multiplies
                else if (this.Multiplies.ContainsKey(wordParts[0]))
                {
                    result += wasNumber ? scale * Multiplies[wordParts[0]] : Multiplies[wordParts[0]];
                    scale = 0;
                }
                // Try to get tens
                else if (this.Tens.ContainsKey(wordParts[0]))
                {
                    scale += wordParts.Length == 1 ? Tens[wordParts[0]] : Tens[wordParts[0]] + Units[wordParts[1]];
                    wasNumber = true;
                }
                // Try to get units
                else if (wordParts.Length == 1 && this.Units.ContainsKey(wordParts[0]))
                {
                    scale += Units[wordParts[0]];
                    wasNumber = true;
                }
                // If number is already in numerical shape
                else if (!wasNumber && wordParts.Length == 1 && long.TryParse(wordParts[0], out scale))
                    wasNumber = true;
                // Number still continues
                else if ((wordParts[0] == "and" && wasNumber) || wordParts[0] == "minus")
                    continue;
                // Final transform to number representation
                else
                {
                    result += scale;
                    if (wasNumber || result != 0)
                        builder.Append(Clamp(result)).Append(' ');

                    scale = 0;
                    result = 0;
                    wasNumber = false;
                    builder.Append(splittedText[i]).Append(' ');

                    if (wasNegation)
                        continue;
                }

                wasNegation = false;
            }

            if (wasNumber || result != 0)
            {
                result += scale;
                builder.Append(Clamp(result));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Tries to get collection phrase from 
        /// </summary>
        /// <param name="word">Base</param>
        /// <param name="splittedText">Splitted input text</param>
        /// <param name="index">Index where to find</param>
        /// <param name="result">Result number</param>
        /// <param name="wasNumber">Flag if was number before collection</param>
        /// <param name="wasNegation">Flag if it was negated</param>
        /// <param name="scale">Collection scale</param>
        /// <returns>True if collection were found</returns>
        private bool TryGetCollection(string word, string[] splittedText, ref int index, ref long result, bool wasNumber, bool wasNegation, long scale)
        {
            // Negation scale of number
            int shrinkageScale = 1;
            if (wasNegation)
                shrinkageScale = this.Random.Next(2, 5);

            // Increase small collections number by shrinkage scale
            if (this.SmallCollections.ContainsKey(word))
            {
                result = this.GetNumberFromCollection(word, wasNumber, scale, SmallCollections) * shrinkageScale;
                return true;
            }

            // Decrease big collections number by shrinkage scale
            if (this.BigCollections.ContainsKey(word))
            {
                result = this.GetNumberFromCollection(word, wasNumber, scale, BigCollections) / shrinkageScale;
                return true;
            }

            // Finding to the maximal length of collection phrase
            string textToFind = word;
            int indexIncrement = 0;
            for (int i = 1; i <= MaxLengthOfCollection; i++)
            {
                if (index + i >= splittedText.Length)
                    break;

                textToFind += $" {splittedText[index + i]}";
                if (this.SmallCollections.ContainsKey(textToFind))
                {
                    indexIncrement = i;
                    result = this.GetNumberFromCollection(textToFind, wasNumber, scale, SmallCollections) * shrinkageScale;
                }

                if (this.BigCollections.ContainsKey(textToFind))
                {
                    indexIncrement = i;
                    result = this.GetNumberFromCollection(textToFind, wasNumber, scale, BigCollections) / shrinkageScale;
                }
            }

            // Increase by one to be still collection
            index += indexIncrement;
            if (indexIncrement > 0)
            {
                result++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets number representation of collection
        /// </summary>
        /// <param name="collection">Collection to use</param>
        /// <param name="wasNumber">Flag if already was a number</param>
        /// <param name="scale">Scale to collection</param>
        /// <param name="collections">Collection set</param>
        /// <returns>Number representation</returns>
        private long GetNumberFromCollection(string collection, bool wasNumber, long scale, Dictionary<string, long> collections)
        {
            return wasNumber ? scale * collections[collection] : this.Random.Next(1, 5) * collections[collection];
        }

        /// <summary>
        /// Clamps value between borders
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <returns>clamped value</returns>
        private long Clamp(long value)
        {
            return (value < 0) ? 0 : (value > MaxAllowedNumber) ? MaxAllowedNumber : value;
        }
    }
}
