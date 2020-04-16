using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UDPipeParsing.Interfaces;

namespace UDPipeParsing.Preprocessors
{
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
