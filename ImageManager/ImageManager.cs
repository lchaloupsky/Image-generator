﻿using ImageGeneratorInterfaces.ImageManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;

namespace ImageManagement
{
    /// <summary>
    /// Class for image management for image generation
    /// </summary>
    public class ImageManager : IImageManager
    {
        private const int CacheLimit = 1024;

        private Downloader ImageDownloader { get; }
        private FileManager FileManager { get; }
        private LimitedDictionary<string, Image> Cache { get; set; }
        private Dictionary<string, object> Locks { get; }

        public bool UseImageCaptioning { get; set; }

        public ImageManager()
        {
            string location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            this.ImageDownloader = new Downloader(ConfigurationManager.AppSettings["apiKey"], ConfigurationManager.AppSettings["secret"], location);
            this.FileManager = new FileManager(location);
            this.Locks = new Dictionary<string, object>();
            this.Cache = new LimitedDictionary<string, Image>(CacheLimit, this.Locks);
        }

        public ImageManager(string location)
        {
            this.ImageDownloader = new Downloader(ConfigurationManager.AppSettings["apiKey"], ConfigurationManager.AppSettings["secret"], location);
            this.FileManager = new FileManager(location);
            this.Locks = new Dictionary<string, object>();
            this.Cache = new LimitedDictionary<string, Image>(CacheLimit, this.Locks);
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
            if (this.FileManager.CheckImageExistence(imageName))
            {
                Image image = null;
                while (image == null)
                {
                    try
                    {
                        image = this.FileManager.LoadImage(imageName);

                        // Add only if not already in cache
                        if (!this.Cache.ContainsKey(imageName))
                            this.Cache.Add(imageName, image);
                        else
                            image.Dispose();
                    }
                    catch (IOException)
                    {
                        // Image is still used in some other thread
                        Thread.Sleep(200);
                        continue;
                    }
                    catch (InvalidOperationException)
                    {
                        // Image was deleted
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Return image for drawing
        /// </summary>
        /// <param name="imageName">image caption to get</param>
        /// <param name="element">base element if image caption to get</param>
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
                    var image = this.ImageDownloader.DownloadImage(imageName, element, this.UseImageCaptioning);

                    // Add only if not downloaded
                    if (!this.Cache.ContainsKey(imageName))
                        this.Cache.Add(imageName, image);
                    else
                        image.Dispose();
                }

                return new Bitmap(this.Cache[imageName]);
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
            this.FileManager.SaveImage(image, newLocation);
        }

        /// <summary>
        /// Method for deleting all saved and cached images
        /// </summary>
        public void DeleteAllImages()
        {
            lock (this.Locks)
            {
                this.Cache.RemoveAll();
                this.FileManager.DeleteAll();
            }
        }

        /// <summary>
        /// Closes lock for the local repository
        /// </summary>
        public void Close()
        {
            this.FileManager.Close();
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
        private ConcurrentDictionary<K, V> Dictionary { get; }
        private ConcurrentQueue<K> KeyQueue { get; set; }
        private Dictionary<K, object> Locks { get; }

        public LimitedDictionary(int limit, Dictionary<K, object> locks)
        {
            this.Limit = limit;
            this.Dictionary = new ConcurrentDictionary<K, V>();
            this.KeyQueue = new ConcurrentQueue<K>();
            this.Locks = locks;
        }

        public void Add(K key, V value)
        {
            if (this.Limit <= KeyQueue.Count)
            {
                KeyQueue.TryDequeue(out K dequeValue);
                this.Remove(dequeValue);
            }

            KeyQueue.Enqueue(key);
            Dictionary.TryAdd(key, value);
        }

        public void RemoveAll()
        {
            this.KeyQueue = new ConcurrentQueue<K>();
            foreach (var key in this.Dictionary.Keys)
                this.Dictionary[key].Dispose();

            this.Dictionary.Clear();
        }

        public bool Remove(K key)
        {
            lock (this.Locks[key])
            {
                this.Dictionary[key].Dispose();
                this.Dictionary.TryRemove(key, out _);
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
