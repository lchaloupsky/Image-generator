using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    /// <summary>
    /// Class for stored images managment
    /// </summary>
    class FileManager
    {
        // Location to manage
        private string Location { get; }

        public FileManager(string location)
        {
            this.Location = location;
        }

        /// <summary>
        /// Checks image existence in the specified location
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Truth value if image is already stored in the current location</returns>
        public bool CheckImageExistence(string imageName)
        {
            return Directory.EnumerateFiles(this.Location, imageName + "*").Any();
        }

        /// <summary>
        /// Loads image from specified location
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns>Loaded image</returns>
        public Image LoadImage(string imageName)
        {
            return new Bitmap(Directory.EnumerateFiles(this.Location, imageName + "*").First());
        }
    }
}
