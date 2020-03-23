using FlickrNet;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class to downloading images, that are not already stored
    /// </summary>
    class Downloader
    {
        // Where to save images
        private string Location { get; }

        // Flickr API for downloading images
        private Flickr MyFlickr { get; }

        /// <summary>
        /// Constructor of Downloader
        /// </summary>
        /// <param name="apiKey">api key needed for access to Flickr</param>
        /// <param name="secret">secret key needed for access to Flickr</param>
        /// <param name="location">Location where to store downloaded images</param>
        public Downloader(string apiKey, string secret, string location)
        {
            this.MyFlickr = new Flickr(apiKey, secret);
            this.Location = location;
        }

        /// <summary>
        /// Method for downloading multiple images
        /// </summary>
        /// <param name="items"></param>
        /// <returns>List of newly dowloaded images</returns>
        public List<Image> DownloadImages(List<string> items)
        {
            var downloaded = new List<Image>();
            foreach (var item in items)
            {
                downloaded.Add(DownloadImage(item));
            }

            return downloaded;
        }

        /// <summary>
        /// Method for downloading new image
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>New downloaded image</returns>
        public Image DownloadImage(string imageName)
        {
            string fileName = "";
            var photos = this.MyFlickr.PhotosSearch(ConfigurePhotoSearchOptions(imageName));

            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                try
                {
                    fileName = imageName
                               .ToLower() + photos[0]
                               .Medium640Url
                               .Substring(photos[0].Medium640Url.LastIndexOf('.'));

                    client.DownloadFile(new Uri(photos[0].Medium640Url), ReturnImageAdress(fileName));
                }
                catch (WebException)
                {
                    // TODO log here in future
                    Console.WriteLine("Network Error");
                    throw;
                }
            }

            return GetCapt(fileName).GetAwaiter().GetResult();
        }

        private async Task<Image> GetCapt(string fileName)
        {
            var captions = new List<ImageCaption>();
            var a = AssignValue(fileName, captions);
            while (!a.IsCompleted)
                await Task.Yield();

            // Return newly dowloaded image
            return new Bitmap(ReturnImageAdress(fileName));
        }

        private async Task AssignValue(string fileName, List<ImageCaption> captions)
        {
            IBMCaptioner capt = new IBMCaptioner();
            captions = await capt.GetCaptionsFromImage(Image.FromFile(ReturnImageAdress(fileName)), fileName);
            while (captions.Count == 0)
                await Task.Yield();

            return;
        }

        /// <summary>
        /// Auxiliary method for creating image adress to store
        /// </summary>
        /// <param name="fileName">Name of an image to download</param>
        /// <returns>Created image adress</returns>
        private string ReturnImageAdress(string fileName)
        {
            return Location + System.IO.Path.DirectorySeparatorChar + fileName;
        }

        /// <summary>
        /// Auxiliary method to configure photo search options
        /// </summary>
        /// <param name="imageName">name to search for</param>
        /// <returns>Configured PhotoSearchOptions</returns>
        private PhotoSearchOptions ConfigurePhotoSearchOptions(string imageName)
        {
            return new PhotoSearchOptions()
            {
                SortOrder = PhotoSearchSortOrder.Relevance,
                MediaType = MediaType.Photos,
                PerPage = 1,
                Text = imageName,
                Page = 1
            };
        }
    }
}