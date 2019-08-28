using Image_Generator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Generator
{
    public partial class Form1 : Form
    {
        private ImageManager Manager { get; }
        private Renderer MyRenderer { get;}
        private Bitmap MyBitmap { get; set; }
    
        public Form1()
        {
            InitializeComponent();
            this.MyRenderer = new Renderer(this.generatedImage.Width, this.generatedImage.Height);
            this.Manager = new ImageManager();
        }

        private void generateButton_Click(object sender, EventArgs e)
        {
            this.MyRenderer.DrawPicture(this.Manager.GetImages(this.sentenceBox.Text.Split(' ')), this.generatedImage.Width, this.generatedImage.Height);
            this.generatedImage.Image = MyRenderer.GetImage();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var dialog = returnConfiguredSaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                this.Manager.SaveImage(this.generatedImage.Image, dialog.FileName);
        }

        private SaveFileDialog returnConfiguredSaveFileDialog()
        {
            return new SaveFileDialog()
            {
                Title = "Save generated image",
                Filter = "JPEG files (*.jpg)|*.jpg|BMP files (*.bmp)|*.bmp|PNG files (*.png)|*.png|GIF files (*.gif)|*.gif",
                FilterIndex = 1,
                DefaultExt = "jpg",
                CheckPathExists = true,
                RestoreDirectory = true
            };
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //TODO
        }
    }
}
