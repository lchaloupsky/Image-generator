using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class for image managment for image generation
    /// </summary>
    class ImageManager
    {
        private const int CACHE_LIMIT = 1000;

        private Downloader MyDownloader { get; }
        private FileManager MyManager { get; }
        private LimitedDictionary<string, Image> Cache { get; set; }
        private Dictionary<string, object> Locks { get; }

        public bool UseImageCaptioning { get; set; }

        public ImageManager()
        {
            string location = Path.Combine("..", "..", "Models", "Images");
            this.MyDownloader = new Downloader(ConfigurationManager.AppSettings["apiKey"], ConfigurationManager.AppSettings["secret"], location);
            this.MyManager = new FileManager(location);
            this.Locks = new Dictionary<string, object>();
            this.Cache = new LimitedDictionary<string, Image>(CACHE_LIMIT, this.Locks);
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
                // Image is still used in some other thread
                while (true)
                {
                    try
                    {
                        var image = this.MyManager.LoadImage(imageName);
                        if (!this.Cache.ContainsKey(imageName))
                            this.Cache.Add(imageName, image);

                        break;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(200);
                        continue;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Return image for drawing
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Wanted image</returns>
        public Image GetImage(string imageName, string element = null)
        {
            imageName = imageName.ToLower();
            lock (this.Locks)
                if (!this.Locks.ContainsKey(imageName))
                    this.Locks.Add(imageName, new object());

            lock (this.Locks[imageName])
            {
                // If image is not in cache or in directory, then download it!
                if (!CheckImageExistence(imageName))
                {
                    var image = this.MyDownloader.DownloadImage(imageName, element, this.UseImageCaptioning);
                    if (!this.Cache.ContainsKey(imageName))
                        this.Cache.Add(imageName, image);
                }

                return this.Cache[imageName];
            }
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

        /// <summary>
        /// Method for deleting all saved and cached images
        /// </summary>
        public void DeleteAllImages()
        {
            this.Cache.RemoveAll();
            this.MyManager.DeleteAll();
        }
    }

    /// <summary>
    /// Class representing cache with limited number of instances
    /// </summary>
    /// <typeparam name="K">Key type</typeparam>
    /// <typeparam name="V">Value type</typeparam>
    internal class LimitedDictionary<K, V> where V : IDisposable
    {
        private int Limit { get; set; }
        private Dictionary<K, V> Dictionary { get; }
        private Queue<K> KeyQueue { get; }
        private Dictionary<K, object> Locks { get; }

        public LimitedDictionary(int limit, Dictionary<K, object> locks)
        {
            this.Limit = limit;
            this.Dictionary = new Dictionary<K, V>();
            this.KeyQueue = new Queue<K>();
            this.Locks = locks;
        }

        public void Add(K key, V value)
        {
            if (this.Limit == KeyQueue.Count)
                this.Remove(KeyQueue.Dequeue());

            KeyQueue.Enqueue(key);
            Dictionary.Add(key, value);
        }

        public void RemoveAll()
        {
            this.KeyQueue.Clear();
            foreach (var key in this.Dictionary.Keys)
                this.Dictionary[key].Dispose();

            this.Dictionary.Clear();
        }

        public bool Remove(K key)
        {
            lock (this.Locks[key])
            {
                this.Dictionary.Remove(key);
            }

            return true;
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
