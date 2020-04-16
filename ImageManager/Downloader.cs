using FlickrNet;
using ImageManagment.Captioning;
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

namespace ImageManagment
{
    /// <summary>
    /// Class to downloading images, that are not already stored
    /// </summary>
    public class Downloader
    {
        // Limit for tags
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

        private SemaphoreSlim Semaphore { get; }

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
            this.Semaphore = new SemaphoreSlim(5, 5);
            this.Location = location;

            ServicePointManager.DefaultConnectionLimit = 200;
            ServicePointManager.Expect100Continue = false;
        }

        /// <summary>
        /// Method for downloading new image
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>New downloaded image</returns>
        public Image DownloadImage(string imageName, string element, bool useImageCaptioning = false)
        {
            var tags = this.GetTags(imageName);
            var imageFind = imageName;
            Image image = null;
            int tryCount = 0;

            while (image == null)
            {
                tryCount++;
                if (tryCount > tags.Length)
                    tags = "";

                try
                {
                    var photos = this.MyFlickr.PhotosSearch(ConfigurePhotoSearchOptions(imageFind, tags));
                    if (photos.Count == 0)
                    {
                        Thread.Sleep(100);
                        imageFind = this.GetImageNameSubstring(imageFind, element);
                        continue;
                    }

                    // __________ IMAGE CAPTION OPTION ___________
                    if (useImageCaptioning)
                        return GetBestImage(photos, imageName);

                    // ___________   REGULAR OPTION    ___________
                    return this.GetImage(photos[0], imageName);
                }
                catch (WebException ex)
                {
                    // 500 INTERNAL !
                    Console.WriteLine("Network Error");
                    if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                        throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unknown" + ex.Message);
                    imageFind = this.GetImageNameSubstring(imageFind, element);
                }
            }

            // null if nothing were downloaded
            return null;
        }

        private Image GetImage(Photo photo, string imageName)
        {
            string fileName = "";
            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
                fileName = imageName.ToLower() + photo.Medium640Url.Substring(photo.Medium640Url.LastIndexOf('.'));
                client.DownloadFile(new Uri(photo.Medium640Url), ReturnImageAdress(fileName));
            }

            // return newly dowloaded image
            return new Bitmap(ReturnImageAdress(fileName));
        }

        private Image GetBestImage(PhotoCollection photos, string imageName)
        {
            Image bestImage = null;
            long bestRating = int.MaxValue;
            object imageLock = new object();

            // wait until semaphore is free to use
            Semaphore.Wait();

            foreach (var photo in photos)
            {
                Image image = null;
                using (WebClient client = new WebClient())
                {
                    client.Proxy = null;
                    using (var stream = client.OpenRead(photo.Medium640Url))
                        image = Image.FromStream(stream);
                }

                var fileName = imageName.ToLower() + '.' + Converter.ConvertToString(image.RawFormat);
                List<ImageCaption> captions = null;
                while (captions == null)
                {
                    try
                    {
                        captions = this.IBMCaptioner.GetCaptionsFromImage(image, fileName);
                    }
                    catch (Exception e)
                    {
                        if (e is WebException || e is AggregateException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        throw;
                    }
                }

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
            }

            // release semaphore
            Semaphore.Release();

            // save image
            bestImage.Save(ReturnImageAdress(imageName + '.' + Converter.ConvertToString(bestImage.RawFormat)));

            // return best image
            return bestImage;
        }

        private string GetImageNameSubstring(string imageName, string element)
        {
            var lastCut = imageName.Substring(Math.Max(0, imageName.LastIndexOf(' ')));
            if (lastCut == element)
                return imageName.Substring(imageName.IndexOf(' '));

            return imageName.Substring(0, imageName.LastIndexOf(' '));
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
                PerPage = 5,
                Text = imageName,
                Page = 1,
                Tags = tags
            };
        }
    }
}