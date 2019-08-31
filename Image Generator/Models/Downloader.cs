using FlickrNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class to downloading images, that are not already stored
    /// </summary>
    class Downloader
    {
        // Where to save images
        private const string LOCATION = @"..\..\Models\Images\";

        // Flickr API for downloading images
        private Flickr MyFlickr { get; }

        /// <summary>
        /// Constructor of Downloader
        /// </summary>
        /// <param name="apiKey">api key needed for access to Flickr</param>
        /// <param name="secret">secret key needed for access to Flickr</param>
        public Downloader(string apiKey, string secret)
        {
            this.MyFlickr = new Flickr(apiKey, secret);
        }

        /// <summary>
        /// Function for downloading multiple images
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
        /// Function for downloading new image
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>New downloaded image</returns>
        public Image DownloadImage(string imageName)
        {
            var photos = this.MyFlickr.PhotosSearch(new PhotoSearchOptions()
            {
                SortOrder = PhotoSearchSortOrder.Relevance,
                MediaType = MediaType.Photos,
                PerPage = 1,
                Tags = imageName +", single",
                Page = 1,
                ContentType = ContentTypeSearch.PhotosOnly
            });

            string fileName = "";
            using (WebClient client = new WebClient())
            {
                try
                {
                    fileName = imageName.ToLower() + photos[0].Medium640Url.Substring(photos[0].Medium640Url.LastIndexOf('.'));
                    client.DownloadFile(new Uri(photos[0].Medium640Url), LOCATION + fileName);
                }
                catch (WebException)
                {
                    // TODO log here in future
                    Console.WriteLine("Network Error");
                    throw;
                }
            }

            // Returning of newly dowloaded image
            return new Bitmap(LOCATION + fileName);
        }
    }
}
