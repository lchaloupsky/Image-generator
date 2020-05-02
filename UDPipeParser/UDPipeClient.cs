using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UDPipeParsing
{
    /// <summary>
    /// Class for REST API calls to UDPIPE service
    /// </summary>
    public class UDPipeClient
    {
        // Service constants
        private const string BASE_URL = "http://lindat.mff.cuni.cz/services/udpipe/api/process?";
        private const string CONST_PARAMS = "&tokenizer&tagger&parser&output=conllu&";
        private const string MODEL_PARAM = "model=";
        private const string DATA_PARAM = "data=";

        private string Model { get; }
        private SemaphoreSlim SemaphoreSlim { get; }

        public UDPipeClient(string model)
        {
            this.Model = model;
            this.SemaphoreSlim = new SemaphoreSlim(4, 4);
        }

        /// <summary>
        /// Method for getting response from UDPipe
        /// </summary>
        /// <param name="sentence">Sentence to request</param>
        /// <returns>List of reponse lines without comments</returns>
        public List<string> GetResponse(string sentence)
        {
            // wait until resource is free to use
            SemaphoreSlim.Wait();

            // REST API call with given text
            string json = null;
            using (WebClient client = new WebClient())
            {
                while (json == null)
                {
                    try
                    {
                        // Do the REST API call
                        json = client.DownloadString(ConstructURL(sentence));
                    }
                    catch (WebException we)
                    {
                        // Too much requests -> try again
                        var code = (we.Response as HttpWebResponse)?.StatusCode;
                        if (code != null && (int)code == 429)
                        {
                            Thread.Sleep(200);
                            continue;
                        }                           

                        throw;
                    }
                }
            }

            // This wait is because we do not want to overload the service
            Thread.Sleep(200);
            SemaphoreSlim.Release();

            // getting response tagged lines from json
            var JsonObject = JObject.Parse(json);
            return JsonObject["result"].ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Where(line => !line[0].Equals('#')).ToList();
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
