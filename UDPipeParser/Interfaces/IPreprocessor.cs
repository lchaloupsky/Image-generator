using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPipeParsing.Interfaces
{
    /// <summary>
    /// Interface for text preprocessors
    /// </summary>
    public interface IPreprocessor
    {
        /// <summary>
        /// Preprocesses given text
        /// </summary>
        /// <param name="sentence">Sentence to preprocess</param>
        /// <returns>Preprocessed text</returns>
        string Preprocess(string sentence);
    }
}
