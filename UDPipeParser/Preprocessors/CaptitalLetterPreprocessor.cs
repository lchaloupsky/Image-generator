using System.Linq;
using UDPipeParsing.Interfaces;

namespace UDPipeParsing.Preprocessors
{
    /// <summary>
    /// Changes the first letter of sentence to upper case
    /// </summary>
    public class CaptitalLetterPreprocessor : IPreprocessor
    {
        public string Preprocess(string sentence)
        {
            if (char.IsDigit(sentence[0]))
                return sentence;

            return sentence.First().ToString().ToUpper() + sentence.Substring(1);
        }
    }
}
