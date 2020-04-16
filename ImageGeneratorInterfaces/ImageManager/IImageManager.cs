using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.ImageManager
{
    public interface IImageManager
    {
        bool UseImageCaptioning { get; set; }

        bool CheckImageExistence(string imageName);
        Image GetImage(string imageName, string element);
        List<Image> GetImages(IEnumerable<string> items);
        void SaveImage(Image image, string newLocation);
        void DeleteAllImages();
    }
}
