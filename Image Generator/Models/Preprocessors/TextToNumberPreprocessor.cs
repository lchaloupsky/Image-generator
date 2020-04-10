﻿using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Preprocessors
{
    class TextToNumberPreprocessor : IPreprocessor
    {
        private const int MAX_ALLOWED_NUMBER = 1_000_000;
        private const int MAX_LENGTH_OF_COLLECTION = 4;

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
        {{"several",2}, {"few",2},{"many",5},{"a few", 20},{"very few", 1},{"a bit",2},{"some",2}};

        private Dictionary<string, long> BigCollections { get; } = new Dictionary<string, long>
        {{ "hundreds",100}, {"thousands",1000},{"millions",1000000},{"billions",1000000000},
        {"trillions",1000000000000},{"quadrillions",1000000000000000},
        {"quintillions",1000000000000000000},{"plenty",10},{"a lot", 10},{"a lot of",10},{"lots of",10},
        { "a large number of", 20},{"a great number of", 20},{"a large quatity of", 20},{"a large amount of", 20},
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
                var wordParts = splittedText[i].Split('-');
                if (i == 0)
                    wordParts[0] = wordParts[0].ToLower();

                if (wordParts[0] == "not")
                    wasNegation = true;

                if (wordParts.Count() == 1 && TryGetCollection(wordParts[0], splittedText, ref i, ref result, wasNumber, wasNegation, scale))
                {
                    scale = 0;
                    if (wasNegation)
                        builder.Length -= 4;
                }
                else if (this.Multiplies.ContainsKey(wordParts[0]))
                {
                    result += wasNumber ? scale * Multiplies[wordParts[0]] : Multiplies[wordParts[0]];
                    scale = 0;
                }
                else if (this.Tens.ContainsKey(wordParts[0]))
                {
                    scale += wordParts.Length == 1 ? Tens[wordParts[0]] : Tens[wordParts[0]] + Units[wordParts[1]];
                    wasNumber = true;
                }
                else if (wordParts.Length == 1 && this.Units.ContainsKey(wordParts[0]))
                {
                    scale += Units[wordParts[0]];
                    wasNumber = true;
                }
                else if (!wasNumber && wordParts.Length == 1 && long.TryParse(wordParts[0], out scale))
                    wasNumber = true;
                else if ((wordParts[0] == "and" && wasNumber) || wordParts[0] == "minus")
                    continue;
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

            return builder.ToString();
        }

        private bool TryGetCollection(string word, string[] splittedText, ref int index, ref long result, bool wasNumber, bool wasNegation, long scale)
        {
            int shrinkage = 1;
            if (wasNegation)
                shrinkage = this.Random.Next(2, 5);

            if (this.SmallCollections.ContainsKey(word))
            {
                result = this.GetNumberFromCollection(word, wasNumber, scale, SmallCollections) * shrinkage;
                return true;
            }

            if (this.BigCollections.ContainsKey(word))
            {
                result = this.GetNumberFromCollection(word, wasNumber, scale, BigCollections) / shrinkage;
                return true;
            }

            string textToFind = word;
            int indexIncrement = 0;
            for (int i = 1; i <= MAX_LENGTH_OF_COLLECTION; i++)
            {
                if (index + i >= splittedText.Length)
                    break;

                textToFind += $" {splittedText[index + i]}";
                if (this.SmallCollections.ContainsKey(textToFind))
                {
                    indexIncrement = i;
                    result = this.GetNumberFromCollection(textToFind, wasNumber, scale, SmallCollections) * shrinkage;
                }

                if (this.BigCollections.ContainsKey(textToFind))
                {
                    indexIncrement = i;
                    result = this.GetNumberFromCollection(textToFind, wasNumber, scale, BigCollections) / shrinkage;
                }
            }

            index += indexIncrement;
            if (indexIncrement > 0)
            {
                result++;
                return true;
            }

            return false;
        }

        private long GetNumberFromCollection(string collection, bool wasNumber, long scale, Dictionary<string, long> Collections)
        {
            return wasNumber ? scale * Collections[collection] : this.Random.Next(1, 5) * Collections[collection];
        }

        private long Clamp(long value)
        {
            return (value < 0) ? 0 : (value > MAX_ALLOWED_NUMBER) ? MAX_ALLOWED_NUMBER : value;
        }
    }
}
