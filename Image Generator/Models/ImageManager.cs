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
        private Downloader MyDownloader { get; }
        private FileManager MyManager { get; }
        private Dictionary<string, Image> Cache { get; set; }

        public ImageManager()
        {
            this.MyDownloader = new Downloader(ConfigurationManager.AppSettings["apiKey"], ConfigurationManager.AppSettings["secret"]);
            this.MyManager = new FileManager(@"..\..\Models\Images\");
            this.Cache = new Dictionary<string, Image>();
        }

        /// <summary>
        /// Function for checking if image is in cache or if is already stored
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Truth value if image already exists or not</returns>
        public bool CheckImageExistence(string imageName)
        {
            if (this.Cache.ContainsKey(imageName))
                return true;

            if (this.MyManager.CheckImageExistence(imageName))
            {
                this.Cache.Add(imageName, this.MyManager.LoadImage(imageName));
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
            // If image is not in cache or in directory, then download it!
            if (!CheckImageExistence(imageName))
                this.Cache.Add(imageName, this.MyDownloader.DownloadImage(imageName));

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

            image.Save(newLocation, format);
        }
    }
}
