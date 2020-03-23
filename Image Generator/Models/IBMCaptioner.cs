using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class IBMCaptioner
    {
        // Base URL of web service
        private const string BASE_URL = @"http://max-image-caption-generator.max.us-south.containers.appdomain.cloud/model/predict";
        private HttpClient Client { get; } = new HttpClient();

        public async Task<List<ImageCaption>> GetCaptionsFromImage(Image image, string imageName)
        {
            // Get image byte data
            byte[] imageData = this.GetImageData(image);

            // Get multipart form POST request
            MultipartFormDataContent requestContent = this.CreateRequest(imageData, imageName);

            // ----Modify to final representation-----
            // send request to BASE_URL of tool to obtain captions
            var result = await Client.PostAsync(BASE_URL, requestContent).ConfigureAwait(false);
            var resultJson = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            // parse returned json
            JObject parsedJson = JObject.Parse(resultJson);

            // create final list of image captions from response
            List<ImageCaption> imageCaptions = new List<ImageCaption>();
            foreach (JToken caption in parsedJson["predictions"])
                imageCaptions.Add(new ImageCaption(caption["caption"].ToString(), float.Parse(caption["probability"].ToString())));

            return imageCaptions;
        }

        private MultipartFormDataContent CreateRequest(byte[] imageData, string imagePath)
        {
            // New request content
            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            // New image content of request
            ByteArrayContent imageContent = new ByteArrayContent(imageData);

            // Add header indicating file type
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/" + this.GetImageExtensionType(imagePath));

            // Add POST parameter image=@file
            requestContent.Add(imageContent, "image", imagePath);

            return requestContent;
        }

        private string GetImageExtensionType(string imagePath)
        {
            string extension = imagePath.Substring(imagePath.LastIndexOf('.') + 1).ToLower();

            // return jpeg instead of jpg - mime does not know "jpg"
            return extension.Equals("jpg") ? "jpeg" : extension;
        }

        private byte[] GetImageData(Image image)
        {
            // Save image data into stream and return final stream
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, image.RawFormat);
                return stream.ToArray();
            }
        }
    }

    public struct ImageCaption
    {
        public string Caption { get; }
        public float Probability { get; }

        public ImageCaption(string caption, float probability)
        {
            this.Caption = caption;
            this.Probability = probability;
        }

        public override string ToString()
        {
            return $"Caption: {this.Caption}\nProbability: {this.Probability}";
        }
    }
}
