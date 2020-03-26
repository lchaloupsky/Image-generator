using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class for image managment for image generation
    /// </summary>
    class ImageManager
    {
        private const int CACHE_LIMIT = 2_500;

        private Downloader MyDownloader { get; }
        private FileManager MyManager { get; }
        private LimitedDictionary<string, Image> Cache { get; set; }
        private Dictionary<string, object> Dowloaded { get; }

        public ImageManager()
        {
            string location = Path.Combine("..", "..", "Models", "Images");
            this.MyDownloader = new Downloader(ConfigurationManager.AppSettings["apiKey"], ConfigurationManager.AppSettings["secret"], location);
            this.MyManager = new FileManager(location);
            this.Cache = new LimitedDictionary<string, Image>(CACHE_LIMIT);
            this.Dowloaded = new Dictionary<string, object>();
        }

        /// <summary>
        /// Function for checking if image is in cache or if is already stored
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Truth value if image already exists or not</returns>
        public bool CheckImageExistence(string imageName)
        {
            // Check if image is in cache
            if (this.Cache.ContainsKey(imageName))
                return true;

            // Check if image is saved already
            if (this.MyManager.CheckImageExistence(imageName))
            {
                var image = this.MyManager.LoadImage(imageName);
                if (!this.Cache.ContainsKey(imageName))
                    this.Cache.Add(imageName, image);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Return image for drawing
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Wanted image</returns>
        public Image GetImage(string imageName)
        {
            imageName = imageName.ToLower();
            lock (this.Dowloaded)
                if (!this.Dowloaded.ContainsKey(imageName))
                    this.Dowloaded.Add(imageName, new object());

            lock (this.Dowloaded[imageName])
            {
                // If image is not in cache or in directory, then download it!
                if (!CheckImageExistence(imageName))
                {
                    var image = this.MyDownloader.DownloadImage(imageName);
                    if (!this.Cache.ContainsKey(imageName))
                        this.Cache.Add(imageName, image);
                }
            }

            return this.Cache[imageName];
        }

        /// <summary>
        /// Returns list of images needed for image generation 
        /// </summary>
        /// <param name="items"></param>
        /// <returns>List of wanted images</returns>
        public List<Image> GetImages(IEnumerable<string> items)
        {
            var images = new List<Image>();
            foreach (var item in items)
                images.Add(GetImage(item));

            return images;
        }

        /// <summary>
        /// Function for saving newly generated image
        /// </summary>
        /// <param name="image">Image to store</param>
        /// <param name="newLocation">Location to store image</param>
        public void SaveImage(Image image, string newLocation)
        {
            ImageFormat format;

            //Saving in the specified image format
            switch (newLocation.Substring(newLocation.LastIndexOf('.')).ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    format = ImageFormat.Jpeg;
                    break;
                case ".png":
                    format = ImageFormat.Png;
                    break;
                case ".bmp":
                    format = ImageFormat.Bmp;
                    break;
                case ".gif":
                    format = ImageFormat.Gif;
                    break;
                default:
                    throw new ArgumentException();
            }

            // Saving the image itself
            image.Save(newLocation, format);
        }
    }

    /// <summary>
    /// Class representing cache with limited number of instances
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    internal class LimitedDictionary<K, V>
    {
        private int Limit { get; set; }
        private Dictionary<K, V> Dictionary { get; }
        private Queue<K> KeyQueue { get; }

        public LimitedDictionary(int limit)
        {
            this.Limit = limit;
            this.Dictionary = new Dictionary<K, V>();
            this.KeyQueue = new Queue<K>();
        }

        public void Add(K key, V value)
        {
            if (this.Limit == KeyQueue.Count)
                Dictionary.Remove(KeyQueue.Dequeue());

            KeyQueue.Enqueue(key);
            Dictionary.Add(key, value);
        }

        public bool ContainsKey(K key)
        {
            return Dictionary.ContainsKey(key);
        }

        public V this[K key]
        {
            get => Dictionary[key];
            set => Dictionary[key] = value;
        }
    }
}
