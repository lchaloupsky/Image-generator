using Image_Generator.Models.Interfaces;
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

        private Dictionary<string, long> Collections { get; } = new Dictionary<string, long>
        {{ "hundreds",100}, {"thousands",1000},{"millions",1000000},{"billions",1000000000},
        {"trillions",1000000000000},{"quadrillions",1000000000000000},
        {"quintillions",1000000000000000000},
        {"several",2}, {"few",1},{"many",5},{"plenty",10}}; // TODO: "a lot", "lots of", etc.

        private Random Random { get; } = new Random();

        public string Preprocess(string text)
        {
            long scale = 0;
            long result = 0;
            bool wasNumber = false;
            StringBuilder builder = new StringBuilder();

            // TODO: number + string combination

            var splittedText = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splittedText.Length; i++)
            {
                var wordParts = splittedText[i].Split('-');
                if (i == 0)
                    wordParts[0] = wordParts[0].ToLower();

                if (this.Collections.ContainsKey(wordParts[0]))
                {
                    result = wasNumber ? scale * Collections[wordParts[0]] : this.Random.Next(1, 9) * Collections[wordParts[0]];
                    scale = 0;
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
                else if (this.Units.ContainsKey(wordParts[0]))
                {
                    scale += Units[wordParts[0]];
                    wasNumber = true;
                }
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
                }
            }

            return builder.ToString();
        }

        private long Clamp(long value)
        {
            return (value < 0) ? 0 : (value > MAX_ALLOWED_NUMBER) ? MAX_ALLOWED_NUMBER : value;
        }
    }
}
