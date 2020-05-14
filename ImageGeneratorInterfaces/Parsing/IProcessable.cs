using ImageGeneratorInterfaces.Graph;

namespace ImageGeneratorInterfaces.Parsing
{
    /// <summary>
    /// Interface for processable elements
    /// </summary>
    public interface IProcessable
    {
        /// <summary>
        /// Element ID in the sentence
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Element dependency type to the parent
        /// </summary>
        string DependencyType { get; set; }

        /// <summary>
        /// Flag indicating if element is negated
        /// </summary>
        bool IsNegated { get; }

        /// <summary>
        /// Method for processing depending element in the sentence
        /// </summary>
        /// <param name="element">Element to be processed</param>
        /// <param name="graph">Sentence graph</param>
        /// <returns>Processed element. It could be same element, new element or depending element.</returns>
        IProcessable Process(IProcessable element, ISentenceGraph graph);

        /// <summary>
        /// Method for finalizing processing of the element
        /// </summary>
        /// <param name="graph">Sentence graph</param>
        /// <returns>Element given after finalizing</returns>
        IProcessable FinalizeProcessing(ISentenceGraph graph);
    }
}
