using Image_Generator.Models.Parts_of_speech;
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

        public UDPipeParser(string model, ImageManager manager)
        {
            this.Model = model;
        }

        /// <summary>
        /// Parses sentence given by user
        /// Creates for each word in sentence class that correspondonds to its part of speech
        /// </summary>
        /// <param name="text">Sentence given by user</param>
        public List<List<PartOfSpeech>> ParseText(string text)
        {
            var parts = new List<List<PartOfSpeech>>();
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
        public List<PartOfSpeech> ParseSentence(string sentence)
        {
            string json;
            using (WebClient client = new WebClient())
                json = client.DownloadString(ConstructURL(sentence));

            var JsonObject = JObject.Parse(json);
            var parts = new List<PartOfSpeech>();
            foreach (string line in JsonObject["result"].ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line[0].Equals('#'))
                    continue;

                var result = ParseJSONResponseLine(line);
                if (result is null)
                    continue;

                parts.Add(result);
            }

            return parts;
        }

        /// <summary>
        /// Parses line from REST API JSON response.
        /// Creates corresponding class to part of speech of the word.
        /// </summary>
        /// <param name="line">Line to parse</param>
        private PartOfSpeech ParseJSONResponseLine(string line)
        {
            // FUTURE --> there create classes by a factory ?
            var parts = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            if (!int.TryParse(parts[0], out int wordID))
                return null;

            // In the future make elements here via Factory?
            PartOfSpeech part = null;
            int pendingID = int.Parse(parts[6]);
            switch (parts[3])
            {
                case "NOUN":
                case "PROPN":
                    part = new Noun(parts[1], parts[2], int.Parse(parts[0]));
                    break;
                default:
                    break;
            }

            return part;
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
