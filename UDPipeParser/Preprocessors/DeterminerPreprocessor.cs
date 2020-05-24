using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDPipeParsing.Interfaces;

namespace UDPipeParsing.Preprocessors
{
    /// <summary>
    /// Checks all determiners in sentence are lower case, they are doing mess if they are not
    /// </summary>
    public class DeterminerPreprocessor : IPreprocessor
    {
        // Set of english determiners
        private static readonly HashSet<string> Determiners = new HashSet<string>() { "a", "an", "the" };

        public string Preprocess(string sentence)
        {
            var parts = sentence.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 1; i < parts.Length; i++)
            {
                if (Determiners.Contains(parts[i].ToLower()))
                    parts[i] = parts[i].ToLower();
            }

            return string.Join(" ",parts);
        }
    }
}
