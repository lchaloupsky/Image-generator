using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageGeneratorInterfaces.Rendering
{
    public interface IRenderer
    {
        int Width { get; }
        int Height { get; }

        Image GetImage();
        void DrawImage(Image image, int x, int y, int width, int height);
        void SetResolution(int width, int height);
        void ResetImage();
    }
}
