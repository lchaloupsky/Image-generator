using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Preprocessors
{
    class CaptitalLetterPreprocessor : IPreprocessor
    {
        public string Preprocess(string sentence)
        {
            if (char.IsDigit(sentence[0]))
                return sentence;

            return sentence.First().ToString().ToUpper() + sentence.Substring(1);
        }
    }
}
