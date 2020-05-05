using ImageGeneratorInterfaces.ImageManager;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UDPipeParserTests.Mocks
{
    class IImageManagerMock : IImageManager
    {
        public bool UseImageCaptioning { get; set; }

        public bool CheckImageExistence(string imageName)
        {
            return true;
        }

        public void DeleteAllImages() { }

        public Image GetImage(string imageName, string element)
        {
            return new Bitmap(200, 200);
        }

        public List<Image> GetImages(IEnumerable<string> items)
        {
            var imageList = new List<Image>();
            imageList.AddRange(Enumerable.Repeat<Image>(new Bitmap(200, 200), items.Count()));
            return imageList;
        }

        public void SaveImage(Image image, string newLocation) { }
    }
}
