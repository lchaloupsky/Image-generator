using FlickrNet;
using ImageManagment.Captioning;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

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

        // Semaphore for controlling number of downloaded images at a time 
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
            Image image = null;

            // Get tags
            var imageFind = imageName;
            var tags = this.GetTags(imageName);

            int tryCount = 0;
            while (image == null)
            {

                tryCount++;

                // throw away tags, if Flickr cannot find and image after some number of trials
                if (tryCount > tags.Length)
                    tags = "";

                try
                {
                    // Get objects describing images from Flickr
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
                    // Not connected to the internet
                    if (ex.Status == WebExceptionStatus.NameResolutionFailure)
                        throw;
                }
                catch (Exception)
                {
                    // Try use imagename substring
                    imageFind = this.GetImageNameSubstring(imageFind, element);
                }
            }

            // null if nothing were downloaded
            return null;
        }

        /// <summary>
        /// Downloads one image without captioning
        /// </summary>
        /// <param name="photo">Photo Flickr object with urls to the image</param>
        /// <param name="imageName">Image name</param>
        /// <returns>Downloaded image</returns>
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

        /// <summary>
        /// Gets an image with using a captioning 
        /// </summary>
        /// <param name="photos">Collection of Flickr photos</param>
        /// <param name="imageName">Image name</param>
        /// <returns>Most corresponding image</returns>
        private Image GetBestImage(PhotoCollection photos, string imageName)
        {
            Image bestImage = null;
            float bestRating = float.MaxValue;
            string imageTextToCompare = this.GetTextWithoutArticles(imageName.ToLower());

            // wait until semaphore is free to use
            Semaphore.Wait();

            foreach (var photo in photos)
            {
                // Download image
                Image image = null;
                using (WebClient client = new WebClient())
                {
                    client.Proxy = null;
                    using (var stream = client.OpenRead(photo.Medium640Url))
                        image = Image.FromStream(stream);
                }

                // Get image filneName
                var fileName = imageName.ToLower() + '.' + Converter.ConvertToString(image.RawFormat);

                // Get captions form IBM captioner
                List<ImageCaption> captions = null;
                while (captions == null)
                {
                    try
                    {
                        captions = this.IBMCaptioner.GetCaptionsFromImage(image, fileName);
                    }
                    catch (WebException e)
                    {
                        if (e.Status == WebExceptionStatus.NameResolutionFailure)
                            throw;

                        Thread.Sleep(1000);
                        continue;
                    }
                    catch (Exception e)
                    {
                        if (e is AggregateException)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }

                        throw;
                    }
                }

                // rate received image captions                
                var imageRating = this.RateCaptionsForImage(imageTextToCompare, captions);

                // Checks total best rating from all captions
                if (imageRating < bestRating)
                {
                    bestImage = image;
                    bestRating = imageRating;
                }
                else
                    image.Dispose();

            }

            // release semaphore
            Semaphore.Release();

            // save image
            bestImage.Save(ReturnImageAdress(imageName + '.' + Converter.ConvertToString(bestImage.RawFormat)));

            // return best image
            return bestImage;
        }

        /// <summary>
        /// Rates given captions with given text to compare with
        /// </summary>
        /// <param name="imageTextToCompare">Text to compare captions with</param>
        /// <param name="captions">Captions to rate</param>
        /// <returns>Best rating from given captions</returns>
        private float RateCaptionsForImage(string imageTextToCompare, List<ImageCaption> captions)
        {
            float bestActualRating = float.MaxValue;

            // Rate received captions
            foreach (var capt in captions)
            {
                string captionToCompare = this.GetTextWithoutArticles(capt.Caption);

                // Normalized Levenshtein distance
                float rating = (this.LDistanceMeter.CalculateStringDistance(imageTextToCompare, captionToCompare) * 1f)
                                / Math.Max(imageTextToCompare.Length, captionToCompare.Length) / capt.Probability;

                // Choose best actual rating
                if (rating < bestActualRating)
                    bestActualRating = rating;
            }

            return bestActualRating;
        }

        /// <summary>
        /// Get rid of articles in given text in lower case
        /// </summary>
        /// <param name="text">Text to delete articles</param>
        /// <returns>Text without articles</returns>
        private string GetTextWithoutArticles(string text)
        {
            return new StringBuilder(Regex.Replace(text, "^(a |an |the )", ""))
                .Replace(" a ", " ")
                .Replace(" an ", " ")
                .Replace(" the ", " ")
                .ToString();
        }

        /// <summary>
        /// Gets substring of image name.
        /// This happens when flickr doesnt return images for original image name.
        /// </summary>
        /// <param name="imageName">Original image name</param>
        /// <param name="element">Base element of image name</param>
        /// <returns>Image name substring</returns>
        private string GetImageNameSubstring(string imageName, string element)
        {
            if (!imageName.Contains(' '))
                return imageName;

            var lastCut = imageName.Substring(Math.Max(0, imageName.LastIndexOf(' ')));
            if (lastCut == element)
                return imageName.Substring(imageName.IndexOf(' '));

            return imageName.Substring(0, imageName.LastIndexOf(' '));
        }

        /// <summary>
        /// Gets tags for given image name
        /// </summary>
        /// <param name="imageName">Image name</param>
        /// <returns>Tags string</returns>
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