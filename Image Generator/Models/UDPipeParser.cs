using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private ImageManager Manager { get; }
        private ElementFactory Factory { get; }

        public UDPipeParser(string model, ImageManager manager)
        {
            this.Model = model;
            this.Factory = new ElementFactory();
        }

        /// <summary>
        /// Parses sentence given by user
        /// Creates for each word in sentence class that correspondonds to its part of speech
        /// </summary>
        /// <param name="text">Sentence given by user</param>
        public List<IProcessable> ParseText(string text)
        {
            var parts = new List<IProcessable>();
            foreach (var line in text.Split(new char[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries))
            {
                parts.Add(ParseSentence(line));
            }

            return parts;
        }

        /// <summary>
        /// Method to parsing one sentence of given text
        /// </summary>
        /// <param name="sentence">Sentence to parse</param>
        /// <returns>List of parsed elements</returns>
        public IProcessable ParseSentence(string sentence)
        {
            string json;
            Dictionary<int, List<IProcessable>> dependencyTree; // Field of fields --> REDO

            using (WebClient client = new WebClient())
                json = client.DownloadString(ConstructURL(sentence));

            var JsonObject = JObject.Parse(json);
            var parts = new List<TextElement>();
            var validLines = JsonObject["result"].ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(line => !line[0].Equals('#'));
            dependencyTree = GetDependencyTree(validLines);

            return CompressDependencyTree(dependencyTree, new Root());
        }

        public Dictionary<int, List<IProcessable>> GetDependencyTree(IEnumerable<string> validLines)
        {
            int index;
            string[] parts;
            Element element;
            Dictionary<int, List<IProcessable>> tree = new Dictionary<int, List<IProcessable>>();

            foreach (string line in validLines)
            {
                parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                index = int.Parse(parts[6]);
                element = this.Factory.CreateElement(parts);

                if (element == null)
                    continue;

                AddDependencyIntoTree(tree, element, index);
            }             

            return tree;
        }

        public void AddDependencyIntoTree(Dictionary<int, List<IProcessable>> tree, IProcessable element, int index)
        {
            if (!tree.ContainsKey(index))
                tree.Add(index, new List<IProcessable>());

            tree[index].Add(element);
        }

        public IProcessable CompressDependencyTree(Dictionary<int, List<IProcessable>> tree, IProcessable element)
        {
            if (!tree.ContainsKey(element.Id))
                return element;

            foreach(var vertex in tree[element.Id])
            {
                element = element.Process(CompressDependencyTree(tree, vertex));
            }

            return element;
        }

        /// <summary>
        /// Parses line from REST API JSON response.
        /// Creates corresponding class to part of speech of the word.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private void ParseJSONResponseLine(string line)
        {
            // FUTURE --> there create classes by a factory ?
            var parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            if (!int.TryParse(parts[0], out int wordID))
                return;

            // In the future make elements here via Factory?
            IProcessable part = null;
            int pendingID = int.Parse(parts[6]);
            switch (parts[3])
            {
                case "NOUN":
                    part = new Noun(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                case "ADJ":
                    part = new Adjective(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                default:
                    return;
            }
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
    }
}
