using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class UDPipeClient
    {
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

        public List<string> GetResponse(string sentence)
        {
            SemaphoreSlim.Wait();

            string json = null;

            // REST API call with given text
            using (WebClient client = new WebClient())
            {
                while (json == null)
                {
                    try
                    {
                        json = client.DownloadString(ConstructURL(sentence));
                    }
                    catch (WebException we)
                    {
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
