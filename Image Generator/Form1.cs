using Image_Generator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Image_Generator
{
    /// <summary>
    /// Form to image generation
    /// </summary>
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

        //Temporary func to check sentence
        private bool CheckSentence()
        {
            //TODO Check then by response from REST API
            if (this.sentenceBox.Text == "")
                return false;

            return true;
        }

        /// <summary>
        /// Function to image generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            if (!CheckSentence())
            {
                ShowErrorMessage("You have to write sentence first");
                return;
            }

            //Catch there or catch in the original function and then return new Exception???
            try
            {
                this.MyRenderer.DrawPicture(
                    this.Manager.GetImages(this.sentenceBox.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)), 
                    this.generatedImage.Width, 
                    this.generatedImage.Height
                    );

                this.generatedImage.Image = MyRenderer.GetImage();
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                    ShowErrorMessage("Internet connection failure");
                else if (ex is IOException)
                    ShowErrorMessage("IO error");
                else
                    ShowErrorMessage("Unknown exception");
            }
        }

        /// <summary>
        /// Function to save new generated image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            var dialog = ReturnConfiguredSaveFileDialog();
            try
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    this.Manager.SaveImage(this.generatedImage.Image, dialog.FileName);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                    ShowErrorMessage("You have to generate image first!");
                else if (ex is ArgumentNullException)
                    ShowErrorMessage("You have to specify name of newly generated image");
                else
                    ShowErrorMessage("Unknown error\n" + ex.Message);
            }
        }

        /// <summary>
        /// Return new configured SaveFileDialog
        /// </summary>
        /// <returns></returns>
        private SaveFileDialog ReturnConfiguredSaveFileDialog()
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

        /// <summary>
        /// Function to showing error messages
        /// </summary>
        /// <param name="message"></param>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Image Generator", MessageBoxButtons.OK);
        }
    }
}
