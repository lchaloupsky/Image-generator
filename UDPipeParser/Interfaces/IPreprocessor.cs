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
