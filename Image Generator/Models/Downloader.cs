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
        private const int MAX_NUMBER_OF_TAGS = 20;

        // Where to save images
        private string Location { get; }

        // Flickr API for downloading images
        private Flickr MyFlickr { get; }

        // IBM caption downloader
        private IBMCaptioner IBMCaptioner { get; }

        // Levenstein Distance meter
        private LDistanceMeter LDistanceMeter { get; }

        // Format converter
        private ImageFormatConverter Converter { get; }

        /// <summary>
        /// Constructor of Downloader
        /// </summary>
        /// <param name="apiKey">api key needed for access to Flickr</param>
        /// <param name="secret">secret key needed for access to Flickr</param>
        /// <param name="location">Location where to store downloaded images</param>
        public Downloader(string apiKey, string secret, string location)
        {
            this.MyFlickr = new Flickr(apiKey, secret);
            this.IBMCaptioner = new IBMCaptioner();
            this.LDistanceMeter = new LDistanceMeter();
            this.Converter = new ImageFormatConverter();
            this.Location = location;
            //ServicePointManager.Expect100Continue = false;
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
            var fileName = "";
            var tags = this.GetTags(imageName);
            var imageFind = imageName;
            Image image = null;

            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                while (image == null)
                {
                    try
                    {
                        var photos = this.MyFlickr.PhotosSearch(ConfigurePhotoSearchOptions(imageFind, tags));
                        if (photos.Count == 0)
                        {
                            imageFind = this.GetRandomPartsSubstring(imageFind);
                            continue;
                        }

                        // __________ IMAGE CAPTION OPTION ___________
                        //image = GetBestImage(photos, imageName);
                        //image.Save(ReturnImageAdress(imageName + '.' + Converter.ConvertToString(image.RawFormat)));

                        // ___________   REGULAR OPTION    ___________
                        fileName = imageName.ToLower() + photos[0].Medium640Url.Substring(photos[0].Medium640Url.LastIndexOf('.'));
                        client.DownloadFile(new Uri(photos[0].Medium640Url), ReturnImageAdress(fileName));
                        return new Bitmap(ReturnImageAdress(fileName));
                    }
                    catch (WebException)
                    {
                        // TODO log here in future
                        Console.WriteLine("Network Error");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Unknown" + ex.Message);
                        imageFind = this.GetRandomPartsSubstring(imageFind);
                    }
                }
            }

            // Return newly dowloaded image
            return image;
        }

        private Image GetBestImage(PhotoCollection photos, string imageName)
        {
            Image bestImage = null;
            long bestRating = int.MaxValue;
            object imageLock = new object();

            Parallel.ForEach(photos, (photo) =>
            {
                Image image = null;
                using (WebClient client = new WebClient())
                {
                    client.Proxy = null;
                    using (var stream = client.OpenRead(photo.Medium640Url))
                        image = Image.FromStream(stream);

                }

                var fileName = imageName.ToLower() + '.' + Converter.ConvertToString(image.RawFormat);
                var captions = this.IBMCaptioner.GetCaptionsFromImage(image, fileName);

                long br = int.MaxValue;
                foreach (var capt in captions)
                {
                    long rating = (long)(this.LDistanceMeter.CalculateStringDistance(imageName, capt.Caption) / capt.Probability);
                    br = rating < br ? rating : br;
                }

                lock (imageLock)
                {
                    if (br < bestRating)
                    {
                        bestImage = image;
                        bestRating = br;
                    }
                    else
                        image.Dispose();
                }
            });


            return bestImage;
        }

        private string GetRandomPartsSubstring(string imageName)
        {
            // stupid cut
            return imageName.Substring(imageName.LastIndexOf(' '));
        }

        private string GetTags(string imageName)
        {
            var parts = imageName.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            var tags = parts.Length > MAX_NUMBER_OF_TAGS ? string.Join(",", parts.Take(MAX_NUMBER_OF_TAGS)) : string.Join(",", parts.Take(MAX_NUMBER_OF_TAGS));

            return tags;
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
        private PhotoSearchOptions ConfigurePhotoSearchOptions(string imageName, string tags)
        {
            return new PhotoSearchOptions()
            {
                SortOrder = PhotoSearchSortOrder.Relevance,
                Extras = PhotoSearchExtras.Views,
                MediaType = MediaType.Photos,
                PerPage = 1,
                Text = imageName,
                Page = 1,
                Tags = tags
            };
        }
    }
}