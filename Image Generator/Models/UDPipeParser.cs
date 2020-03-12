using Image_Generator.Models.Factories;
using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Preprocessors;
using Image_Generator.Models.Text_elements;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class to parsing response from UDPipe REST API calling 
    /// </summary>
    class UDPipeParser
    {
        private const string BASE_URL = "http://lindat.mff.cuni.cz/services/udpipe/api/process?";
        private const string CONST_PARAMS = "&tokenizer&tagger&parser&output=conllu&";
        private const string MODEL_PARAM = "model=";
        private const string DATA_PARAM = "data=";

        private string Model { get; }
        private ElementFactory ElementFactory { get; }
        private ElementComparer Comparer { get; }
        private List<IPreprocessor> Preprocessors { get; }

        public UDPipeParser(string model, ImageManager manager)
        {
            this.Model = model;
            this.ElementFactory = new ElementFactory(manager);
            this.Comparer = new ElementComparer();
            this.Preprocessors = this.GetPreprocessors();
        }

        /// <summary>
        /// Parses sentence given by user
        /// Creates for each word in sentence class that correspondonds to its part of speech
        /// </summary>
        /// <param name="text">Sentence given by user</param>
        public List<SentenceGraph> ParseText(string text, int width, int height)
        {
            var parts = new List<SentenceGraph>();
            foreach (var line in text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
                parts.Add(ParseSentence(PreprocessText($"{line}.")));

            return parts;
        }

        /// <summary>
        /// Method to parsing one sentence of given text
        /// </summary>
        /// <param name="sentence">Sentence to parse</param>
        /// <returns>List of parsed elements</returns>
        public SentenceGraph ParseSentence(string sentence)
        {
            string json;
            Dictionary<int, List<IProcessable>> dependencyTree; // Field of fields --> REDO
            SentenceGraph graph = new SentenceGraph();

            // REST API call with given text
            using (WebClient client = new WebClient())
                json = client.DownloadString(ConstructURL(sentence));

            // recreating dependency tree given as RESTAPI reponse from UDPipe
            var JsonObject = JObject.Parse(json);
            var validLines = JsonObject["result"].ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(line => !line[0].Equals('#'));
            dependencyTree = GetDependencyTree(validLines);

            // compressing dependency tree into graph
            this.Comparer.Tree = dependencyTree;
            IProcessable element = CompressDependencyTree(dependencyTree, graph, new Root()).FinalizeProcessing(graph);
            if (element is IDrawable)
                graph.AddVertex((IDrawable)element); // Adding last processed vertex (is added only if its only vertex in sentence)

            return graph;
        }

        private string PreprocessText(string text)
        {
            this.Preprocessors.ForEach(p => text = p.Preprocess(text));
            return text;
        }

        private List<IPreprocessor> GetPreprocessors()
        {
            return new List<IPreprocessor> {
                new TextToNumberPreprocessor()
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

            foreach (string line in validLines)
            {
                parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                index = int.Parse(parts[6]);
                element = this.ElementFactory.Create(parts);

                if (element == null)
                    continue;

                AddDependencyIntoTree(tree, element, index);
            }

            return tree;
        }

        /// <summary>
        /// Helping function for adding dependency into dependency tree
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
        /// Function for compressing dependency tree edges into other representation
        /// </summary>
        /// <param name="tree">tree to be compressed</param>
        /// <param name="graph">new graph to be created</param>
        /// <param name="element">actual processed element</param>
        /// <returns>Element with processed its dependencies</returns>
        private IProcessable CompressDependencyTree(Dictionary<int, List<IProcessable>> tree, SentenceGraph graph, IProcessable element)
        {
            if (!tree.ContainsKey(element.Id))
                return element;

            // sorting dependencies before they are processed
            tree[element.Id].Sort(this.Comparer);

            // processing each dependency
            foreach (var vertex in tree[element.Id])
                element = element.Process(CompressDependencyTree(tree, graph, vertex), graph);

            return element;
        }

        /// <summary>
        /// Construcs adress for calling
        /// </summary>
        /// <param name="sentence">Sentence data param</param>
        /// <returns>Constructed URL for call</returns>
        private string ConstructURL(string sentence)
        {
            return BASE_URL +
                   MODEL_PARAM + Model +
                   CONST_PARAMS +
                   DATA_PARAM + sentence;
        }

        /// <summary>
        /// Help comparer class for sorting dependencies before they are processed
        /// </summary>
        private class ElementComparer : IComparer<IProcessable>
        {
            public Dictionary<int, List<IProcessable>> Tree { get; set; }

            public int Compare(IProcessable x, IProcessable y)
            {
                // Numerals modificating number of elements are a priority 
                if (x is Numeral && x.DependencyType == "nummod")
                    return -1;

                if (y is Numeral && y.DependencyType == "nummod")
                    return 1;

                if ((x is Noun || x is NounSet) && x.DependencyType == "dobj")
                    return -1;

                if ((y is Noun || y is NounSet) && y.DependencyType == "dobj")
                    return 1;

                // Little hack in comparing adpositions, putting forward only "simple" ones like (on, under, etc.)
                return x is Adposition || x is Adjective ? (!this.Tree.ContainsKey(x.Id) ? -1 : 1) :
                      (y is Adposition || y is Adjective ? (!this.Tree.ContainsKey(y.Id) ? 1 : -1) :
                      (x.Id < y.Id ? -1 : 1));
            }
        }
    }
}
