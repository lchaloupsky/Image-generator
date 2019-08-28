using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models
{
    class FileManager
    {
        private string Path { get; }

        public FileManager(string path)
        {
            this.Path = path;
        }

        public bool CheckImageExistence(string imageName)
        {
            return Directory.EnumerateFiles(this.Path, imageName + "*").Any();
        }

        public Image LoadImage(string imageName)
        {
            return new Bitmap(Directory.EnumerateFiles(this.Path, imageName + "*").First());
        }
    }
}
