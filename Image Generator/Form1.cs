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
using Image_Generator.Models.Text_elements;
using Image_Generator.Models.Interfaces;

namespace Image_Generator
{
    /// <summary>
    /// Form to image generation
    /// </summary>
    public partial class Form1 : Form
    {
        private ImageManager Manager { get; }
        private Renderer MyRenderer { get; }
        private Bitmap MyBitmap { get; set; }
        private UDPipeParser MyParser { get; }
        private Positioner MyPositioner { get; }

        public Form1()
        {
            InitializeComponent();
            this.MyRenderer = new Renderer(this.generatedImage.Width, this.generatedImage.Height);
            this.Manager = new ImageManager();
            this.MyParser = new UDPipeParser("english-ud-1.2-160523");
            this.MyPositioner = new Positioner();
        }

        //Temporary func to check sentence
        private bool CheckSentence()
        {
            //TODO Check then by response from REST API
            if (this.sentenceBox.Text == "")
            {
                ShowErrorMessage("You have to write sentence first");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Function to image generation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            // Check some basic info about text
            if (!this.CheckSentence())
                return;

            // Catch there or catch in the original function and then return new Exception???
            try
            {
                // Parsing given text
                var result = this.MyParser.ParseText(this.sentenceBox.Text, this.generatedImage.Width, this.generatedImage.Height);

                // Clear draw field
                this.MyRenderer.ResetImage();

                // positioning and drawing phase of generation
                foreach (var graph in result)
                {
                    // positioning given sentence graph with given with and height
                    this.MyPositioner.Positionate(graph, this.generatedImage.Width, this.generatedImage.Height);

                    // drawing each vertex of graph
                    foreach (var vertex in graph.Vertices.OrderBy(v => v.ZIndex))
                        vertex.Draw(this.MyRenderer, this.Manager);
                }

                // Get drawn image bitmap to show it in form window
                this.generatedImage.Image = MyRenderer.GetImage();
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                    ShowErrorMessage("Internet connection failure");
                else if (ex is IOException)
                    ShowErrorMessage("IO error");
                else
                    ShowErrorMessage("Unknown exception\n" + ex.ToString());
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
        /// <returns>Configured SaveFileDialog</returns>
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
        /// <param name="message">Message to show</param>
        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Image Generator", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Only calls generate function when Enter key is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Pressed key</param>
        private void SentenceBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                GenerateButton_Click(sender, e);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.MyRenderer != null && this.generatedImage != null)
                this.MyRenderer.UpdateSizes(this.generatedImage.Width, this.generatedImage.Height);
        }
    }
}
