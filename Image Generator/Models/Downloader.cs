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
    class Downloader
    {
        private const string LOCATION = @"C:\Users\lukas\source\repos\Image Generator\Image Generator\Models\Images\";

        private string ApiKey { get; }
        private Flickr myFlickr { get; }

        public Downloader(string apiKey, string secret)
        {
            this.ApiKey = apiKey;
            this.myFlickr = new Flickr(this.ApiKey, secret);
        }

        public List<Image> DownloadImages(List<string> items)
        {
            var downloaded = new List<Image>();
            foreach (var item in items)
            {
                downloaded.Add(DownloadImage(item));
            }

            return downloaded;
        }

        public Image DownloadImage(string imageName)
        {
            var photos = this.myFlickr.PhotosSearch(new PhotoSearchOptions()
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
                catch (WebException e)
                {
                    Console.WriteLine("Network Error");
                    Console.WriteLine(e.Message);
                }
            }

            return new Bitmap(LOCATION + fileName);
        }
    }
}
