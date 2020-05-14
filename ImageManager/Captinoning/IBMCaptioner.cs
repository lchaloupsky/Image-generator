using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace ImageManagement.Captioning
{
    /// <summary>
    /// Class for getting image captions via REST API call to IBM Image Caption Generator service
    /// </summary>
    public class IBMCaptioner
    {
        // Base URL of web service
        private const string BaseUrl = @"http://max-image-caption-generator.codait-prod-41208c73af8fca213512856c7a09db52-0000.us-east.containers.appdomain.cloud/model/predict";

        /// <summary>
        /// Gets captions for image via REST API call to web service
        /// </summary>
        /// <param name="image">Image for get captions</param>
        /// <param name="imageName">Image name</param>
        /// <returns>List of captions to given image</returns>
        public List<ImageCaption> GetCaptionsFromImage(Image image, string imageName)
        {
            // Get image byte data
            byte[] imageData = this.GetImageData(image);

            // Create request         
            var boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x", NumberFormatInfo.InvariantInfo);
            HttpWebRequest request = this.CreateRequest(boundary);
            boundary = "--" + boundary;

            // Write data into request stream
            this.WriteRequestIntoStream(request, imageData, imageName, boundary);

            // reading response from service
            string resultJson = null;
            using (var response = request.GetResponse())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    resultJson = new StreamReader(responseStream).ReadToEnd();
                    response.Close();
                }
            }

            // parse returned json
            JObject parsedJson = JObject.Parse(resultJson);

            // create final list of image captions from response
            List<ImageCaption> imageCaptions = new List<ImageCaption>();
            foreach (JToken caption in parsedJson["predictions"])
                imageCaptions.Add(new ImageCaption(caption["caption"].ToString(), float.Parse(caption["probability"].ToString())));

            // stop request
            request.Abort();

            return imageCaptions;
        }

        /// <summary>
        /// Creates request stream and writes all necessary data to stream
        /// </summary>
        /// <param name="request">Request object</param>
        /// <param name="imageData">Byte array of image data</param>
        /// <param name="imageName">Image name</param>
        /// <param name="boundary">Boundary to distinguish between requests</param>
        private void WriteRequestIntoStream(HttpWebRequest request, byte[] imageData, string imageName, string boundary)
        {
            using (var requestStream = request.GetRequestStream())
            {
                // boundary
                var buffer = Encoding.ASCII.GetBytes(boundary + Environment.NewLine);
                requestStream.Write(buffer, 0, buffer.Length);

                // Content disposition header
                buffer = Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"{2}", "image", imageName, Environment.NewLine));
                requestStream.Write(buffer, 0, buffer.Length);

                // Content type header
                buffer = Encoding.ASCII.GetBytes(string.Format("Content-Type: {0}{1}{1}", "image/jpeg", Environment.NewLine));
                requestStream.Write(buffer, 0, buffer.Length);

                // Image data
                requestStream.Write(imageData, 0, imageData.Length);

                // final new line
                buffer = Encoding.ASCII.GetBytes(Environment.NewLine);
                requestStream.Write(buffer, 0, buffer.Length);

                // boundary
                var boundaryBuffer = Encoding.ASCII.GetBytes(boundary + "--");
                requestStream.Write(boundaryBuffer, 0, boundaryBuffer.Length);
            }
        }

        /// <summary>
        /// Creates configured http request object
        /// </summary>
        /// <param name="boundary">boundary to distinguish between requests</param>
        /// <returns>Configured http web request</returns>
        private HttpWebRequest CreateRequest(string boundary)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BaseUrl);
            request.Timeout = 10000;
            request.ReadWriteTimeout = 32000;
            request.KeepAlive = false;
            request.Pipelined = true;
            request.Proxy = null;
            request.Method = "POST";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Accept = "application/json";

            return request;
        }

        /// <summary>
        /// Gets image extenstion from given path
        /// </summary>
        /// <param name="imagePath">Path to an image</param>
        /// <returns>Image extension</returns>
        private string GetImageExtensionType(string imagePath)
        {
            string extension = imagePath.Substring(imagePath.LastIndexOf('.') + 1).ToLower();

            // return jpeg instead of jpg - mime does not know "jpg"
            return extension.Equals("jpg") ? "jpeg" : extension;
        }

        /// <summary>
        /// Gets byte array data from given image
        /// </summary>
        /// <param name="image">Image to get the data</param>
        /// <returns>Byte array of image</returns>
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

    /// <summary>
    /// Structure representing one received caption
    /// </summary>
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
