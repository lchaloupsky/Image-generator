using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Parsing;
using System;
using System.Collections.Generic;
using UDPipeParsing.Factories;
using UDPipeParsing.Interfaces;
using UDPipeParsing.Preprocessors;
using UDPipeParsing.Text_elements;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing
{
    /// <summary>
    /// Class to parsing response from UDPipe REST API calling 
    /// </summary>
    public class UDPipeParser
    {
        private static readonly char[] UNSUPPORTED_CHARS = new char[] { '\'', '\"', '`','\\', '/', '>', '<', '|' };
        private static readonly int MAX_SENTENCE_LENGTH = 1500;

        private ElementFactory ElementFactory { get; }
        private ElementComparer Comparer { get; }
        private List<IPreprocessor> Preprocessors { get; }
        private UDPipeClient Client { get; }

        public UDPipeParser(string model, IImageManager manager, IEdgeFactory edgeFactory)
        {
            this.ElementFactory = new ElementFactory(manager, edgeFactory);
            this.Comparer = new ElementComparer();
            this.Client = new UDPipeClient(model);
            this.Preprocessors = this.GetPreprocessors();
        }

        /// <summary>
        /// Parses sentence given by user
        /// Creates for each word in sentence class that correspondonds to its part of speech
        /// </summary>
        /// <param name="text">Sentence given by user</param>
        public List<ISentenceGraph> ParseText(string text, int width, int height)
        {
            var parts = new List<ISentenceGraph>();
            text = this.RemoveUnsupportedCharsFromSentence(text);
            foreach (var line in text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Trim() == string.Empty)
                    continue;

                if (line.Length > MAX_SENTENCE_LENGTH)
                    throw new ArgumentException($"Sentence is too long. Maximal length of sentence is {MAX_SENTENCE_LENGTH} characters.");

                parts.Add(ParseSentence(PreprocessText($"{line}.")));
            }

            return parts;
        }

        /// <summary>
        /// Method to parsing one sentence of given text
        /// </summary>
        /// <param name="sentence">Sentence to parse</param>
        /// <returns>List of parsed elements</returns>
        public SentenceGraph ParseSentence(string sentence)
        {
            Dictionary<int, List<IProcessable>> dependencyTree;
            SentenceGraph graph = new SentenceGraph();

            // recreating dependency tree given as RESTAPI reponse from UDPipe
            var validLines = this.Client.GetResponse(sentence);
            dependencyTree = GetDependencyTree(validLines);

            // new root element
            var root = this.ElementFactory.CreateRoot(sentence);

            // compressing dependency tree into graph
            this.Comparer.Tree = dependencyTree;
            IProcessable element = CompressDependencyTree(dependencyTree, graph, root).FinalizeProcessing(graph);

            // Clear grapg if nothing were found for safety
            if (element is Root)
                graph.Clear();

            // Adding last processed vertex (is added only if its only vertex in sentence)
            if (element is IDrawable)
                graph.AddVertex((IDrawable)element);

            return graph;
        }

        /// <summary>
        /// Preprocesses input text by preprocessors
        /// </summary>
        /// <param name="text">description</param>
        /// <returns>preprocessed description</returns>
        private string PreprocessText(string text)
        {
            this.Preprocessors.ForEach(p => text = p.Preprocess(text));
            return text;
        }

        /// <summary>
        /// Get all current preprocessors
        /// </summary>
        /// <returns>list of preprocessors</returns>
        private List<IPreprocessor> GetPreprocessors()
        {
            return new List<IPreprocessor> {
                new CaptitalLetterPreprocessor(),
                new TextToNumberPreprocessor(),
                new MissingArticlePreprocessor(this.Client)
            };
        }

        /// <summary>
        /// Function for creating dependency tree
        /// </summary>
        /// <param name="validLines"></param>
        /// <returns>Dependency tree</returns>
        private Dictionary<int, List<IProcessable>> GetDependencyTree(IEnumerable<string> validLines)
        {
            int index;
            string[] parts;
            IProcessable element;
            Dictionary<int, List<IProcessable>> tree = new Dictionary<int, List<IProcessable>>();

            // Each line defines one sentence element
            foreach (string line in validLines)
            {
                parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                index = int.Parse(parts[6]);
                element = this.ElementFactory.Create(parts);

                // skip unsupported elements
                if (element == null)
                    continue;

                AddDependencyIntoTree(tree, element, index);
            }

            return tree;
        }

        /// <summary>
        /// Help method for adding dependency into dependency tree
        /// </summary>
        /// <param name="tree">tree to add</param>
        /// <param name="element">element to be added</param>
        /// <param name="index">index where element should be added</param>
        private void AddDependencyIntoTree(Dictionary<int, List<IProcessable>> tree, IProcessable element, int index)
        {
            if (!tree.ContainsKey(index))
                tree.Add(index, new List<IProcessable>());

            tree[index].Add(element);
        }

        /// <summary>
        /// Method for compressing dependency tree edges into other representation
        /// </summary>
        /// <param name="tree">tree to be compressed</param>
        /// <param name="graph">new graph to be created</param>
        /// <param name="element">actual processed element</param>
        /// <returns>Element with processed its dependencies</returns>
        private IProcessable CompressDependencyTree(Dictionary<int, List<IProcessable>> tree, SentenceGraph graph, IProcessable element)
        {
            if (!tree.ContainsKey(element.Id))
                return element;

            // Adpositions have different priorities
            if (element is Adposition)
                this.Comparer.IsAdposition = true;
            else
                this.Comparer.IsAdposition = false;

            // sorting dependencies before they are processed
            tree[element.Id].Sort(this.Comparer);

            // processing each dependency
            foreach (var vertex in tree[element.Id])
                element = element.Process(CompressDependencyTree(tree, graph, vertex), graph);

            return element;
        }

        /// <summary>
        /// Removes unwanted characters from sentence.
        /// </summary>
        /// <param name="sentence">Sentence to be processed</param>
        /// <returns>Modified string</returns>
        private string RemoveUnsupportedCharsFromSentence(string sentence)
        {
            return string.Join(string.Empty, sentence.Split(UNSUPPORTED_CHARS, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Comparer class for sorting dependencies before they are processed
        /// </summary>
        private class ElementComparer : IComparer<IProcessable>
        {
            public Dictionary<int, List<IProcessable>> Tree { get; set; }

            public bool IsAdposition { get; set; }

            private DependencyTypeHelper DependencyTypeHelper { get; } = new DependencyTypeHelper();

            /// <summary>
            /// Overriden method for comparing elements
            /// </summary>
            /// <param name="x">First element</param>
            /// <param name="y">Second element</param>
            /// <returns>Order of given elements</returns>
            public int Compare(IProcessable x, IProcessable y)
            {
                // Numerals modificating number of elements are a priority 
                if (x is Numeral && this.DependencyTypeHelper.IsNumeralModifier(x.DependencyType))
                    return -1;

                if (y is Numeral && this.DependencyTypeHelper.IsNumeralModifier(y.DependencyType))
                    return 1;

                // Direct object nouns of verbs has to be processed first
                if ((x is Noun || x is NounSet) && this.DependencyTypeHelper.IsObject(x.DependencyType)
                    && (y is Noun || y is NounSet) && this.DependencyTypeHelper.IsObject(y.DependencyType))
                    return x.Id < y.Id ? -1 : 1;

                if ((x is Noun || x is NounSet) && this.DependencyTypeHelper.IsObject(x.DependencyType))
                    return -1;

                if ((y is Noun || y is NounSet) && this.DependencyTypeHelper.IsObject(y.DependencyType))
                    return 1;

                // Check objects priorities
                bool isFirstPrior = this.IsPriority(x) && !this.Tree.ContainsKey(x.Id);
                bool isSecondPrior = this.IsPriority(y) && !this.Tree.ContainsKey(y.Id);

                // Both are prior -> return by original sentence order
                if (isFirstPrior && isSecondPrior)
                    return x.Id < y.Id ? -1 : 1;

                // Sort by priority or by original sentence order
                return isFirstPrior ? -1 : (isSecondPrior ? 1 : (x.Id < y.Id ? -1 : 1));
            }

            /// <summary>
            /// Checks if element has priority
            /// </summary>
            /// <param name="x">Elements to check</param>
            /// <returns>True if it is prior</returns>
            private bool IsPriority(IProcessable x)
            {
                if (this.IsAdposition)
                    return (x is Adposition || x is Adjective || x is Adverb || x is Negation);
                else
                    return (x is Adposition || x is Adverb || x is Negation);
            }
        }
    }
}
