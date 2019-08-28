using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class ImageManager
    {
        private const string LOCATION = @"C:\Users\lukas\source\repos\Image Generator\Image Generator\Models\Images\";
        private const string GENERATED_LOCATION = LOCATION + @"Generated\";

        private Downloader MyDownloader { get; }
        private FileManager MyManager { get; }
        private Dictionary<string, Image> Cache { get; set; } //TODO OWN CACHE MEMORY ??
        private int Counter { get; } = 1;

        public ImageManager()
        {
            this.MyDownloader = new Downloader("b08f510eefde3467b03f2fac4775990d", "6e90af99b854c6dd"); //TODO LOAD FROM CONFIG
            this.MyManager = new FileManager(@"..\..\Models\Images\");
            this.Cache = new Dictionary<string, Image>();
        }

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

        public Image GetImage(string imageName)
        {
            // If image is not in cache or in directory, then download it!
            if (!CheckImageExistence(imageName))
                this.Cache.Add(imageName, this.MyDownloader.DownloadImage(imageName));

            return this.Cache[imageName];
        }

        public List<Image> GetImages(IEnumerable<string> items)
        {
            var images = new List<Image>();
            foreach (var item in items)
                images.Add(GetImage(item));

            return images;
        }

        //TODO SAVE IN THE RIGHT RESOLUTION OR LEAVE IT AS IT IS?
        public void SaveImage(Image image, string newLocation)
        {
            ImageFormat format;
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
