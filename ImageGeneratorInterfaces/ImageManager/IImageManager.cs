using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.ImageManager
{
    /// <summary>
    /// Interface for the image manager.
    /// Image manager is for getting corresponding images
    /// </summary>
    public interface IImageManager
    {
        /// <summary>
        /// Flag indicating if image captioning is used 
        /// </summary>
        bool UseImageCaptioning { get; set; }
        
        /// <summary>
        /// Method for checking if image already exists in the manageer
        /// </summary>
        /// <param name="imageName">Image name to find</param>
        /// <returns>True if exists</returns>
        bool CheckImageExistence(string imageName);

        /// <summary>
        /// Method for getting corresponding image for given text
        /// </summary>
        /// <param name="imageName">Image name</param>
        /// <param name="element">Base element in the image name</param>
        /// <returns>Corresponding image</returns>
        Image GetImage(string imageName, string element);

        /// <summary>
        /// Method for getting images at once for multiple image names
        /// </summary>
        /// <param name="items">Image names</param>
        /// <returns>Enumerable of images</returns>
        List<Image> GetImages(IEnumerable<string> items);

        /// <summary>
        /// Method for saving given image in the given location
        /// </summary>
        /// <param name="image">Image to be saved</param>
        /// <param name="newLocation">Location where image should be saved</param>
        void SaveImage(Image image, string newLocation);

        /// <summary>
        /// Method for deleting all currently saved images
        /// </summary>
        void DeleteAllImages();
    }
}
