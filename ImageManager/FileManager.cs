using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManagment
{
    /// <summary>
    /// Class for stored images managment
    /// </summary>
    public class FileManager
    {
        // lock file
        private const string LOCKFILENAME = ".lockfile";

        // Location to manage
        private string Location { get; }

        // allowed image extensions
        private HashSet<string> AllowedExtenstions { get; } = new HashSet<string>() { ".jpg", ".jpeg", ".png", ".gif" };

        public FileManager(string location)
        {
            this.Location = location;
            this.CreateFolderIfNotExists();
            if (!this.CreateLockFileIfNotExists())
                File.OpenWrite(Path.Combine(this.Location, LOCKFILENAME));
        }

        /// <summary>
        /// Checks image existence in the specified location
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Truth value if image is already stored in the current location</returns>
        public bool CheckImageExistence(string imageName)
        {
            return Directory.EnumerateFiles(this.Location, imageName + "." + "*")
                .Where(file => this.AllowedExtenstions.Contains(Path.GetExtension(file).ToLower()))
                .Any();
        }

        /// <summary>
        /// Loads image from specified location
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Loaded image</returns>
        public Image LoadImage(string imageName)
        {
            return new Bitmap(
                Directory.EnumerateFiles(this.Location, imageName + "." + "*").
                    Where(file => this.AllowedExtenstions.Contains(Path.GetExtension(file).ToLower())).
                    Last()
                );
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
            switch (Path.GetExtension(newLocation).ToLower())
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
        /// Deletes all saved images
        /// </summary>
        public void DeleteAll()
        {
            foreach (var image in new DirectoryInfo(this.Location).GetFiles().Where(file => file.Name != LOCKFILENAME))
            {
                image.Delete();
            }
        }

        /// <summary>
        /// Creates Image folder if it does not exist yet
        /// </summary>
        private void CreateFolderIfNotExists()
        {
            if (!Directory.Exists(this.Location))
                Directory.CreateDirectory(this.Location);
        }

        /// <summary>
        /// Creates lockfile, that prevent another program or user to delete folder during generation
        /// </summary>
        /// <returns>True if lockfile was created</returns>
        private bool CreateLockFileIfNotExists()
        {
            var lockFilePath = Path.Combine(this.Location, LOCKFILENAME);
            if (!File.Exists(lockFilePath))
            {
                File.Create(lockFilePath);
                File.SetAttributes(lockFilePath, FileAttributes.Hidden);
                return true;
            }

            return false;
        }
    }
}
