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
using System.Diagnostics;

namespace Image_Generator
{
    /// <summary>
    /// UI form for Image Generator Application
    /// </summary>
    public partial class Form1 : Form
    {
        private ImageManager Manager { get; }
        private Renderer MyRenderer { get; }
        private Bitmap MyBitmap { get; set; }
        private UDPipeParser MyParser { get; }
        private Positioner MyPositioner { get; }
        private ResolutionItem ImageResolution { get; set; }

        private bool UsingDataset { get; set; } = false;
        private string DatasetFileName { get; set; }
        private int Processed { get; set; } = 0;

        public Form1()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.FillResolutionBox();
            this.MyRenderer = new Renderer(this.ImageResolution.Width, this.ImageResolution.Height);
            this.Manager = new ImageManager();
            this.MyParser = new UDPipeParser("english-ud-1.2-160523", this.Manager);
            this.MyPositioner = new Positioner();
        }

        /// <summary>
        /// Function to check input sentence
        /// </summary>
        /// <returns>True if sentence in not empty</returns>
        private bool CheckSentence()
        {
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
            if (!this.DataSetCheckBox.Checked)
                this.GenerateImageForView();
            else
                this.GenerateImagesForDataset();
        }

        /// <summary>
        /// Function to image generation from dataset
        /// </summary>
        private void GenerateImagesForDataset()
        {
            // check if dataset is loaded
            if (this.DatasetFileName == "")
                ShowErrorMessage("You have to select dataset first");

            // choosing directory
            string directory;
            var dialog = ReturnConfiguredFolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                directory = dialog.SelectedPath;
            else
            {
                ShowErrorMessage("You have to select directory to save images first");
                return;
            }

            // generate all images for descriptions from dataset in independent tasks
            using (StreamReader streamReader = File.OpenText(this.DatasetFileName))
            {
                string str;
                int counter = 0;

                while ((str = streamReader.ReadLine()) != null)
                {
                    counter++;
                    this.CreateImageGeneratingTask(str, directory, counter).Start();
                }
            }
        }

        /// <summary>
        /// Methods that prepare new Thread Task for generating image
        /// </summary>
        /// <param name="str">Description of image</param>
        /// <param name="directory">Directory to save</param>
        /// <param name="counter">Dataset image number</param>
        /// <returns></returns>
        private Task CreateImageGeneratingTask(string str, string directory, int counter)
        {
            return new Task(() =>
            {
                try
                {
                    var result = this.GenerateImage(str.Substring(str.IndexOf(str.First(c => char.IsWhiteSpace(c))) + 1));
                    lock (this.Manager)
                    {
                        // Draw image
                        this.DrawImage(result);

                        // SaveImage
                        this.Manager.SaveImage(this.MyRenderer.GetImage(), Path.Combine(directory, counter + ".jpg"));
                    }
                }
                catch (Exception e)
                {
                    // TODO: LOGS?
                    if (!e.Message.Contains("Index"))
                    {
                        Console.WriteLine();
                    }
                    Console.Error.WriteLine("__________ERROR__________" + counter);
                    Console.Error.WriteLine(e);
                }
                finally
                {
                    // update count
                    this.Processed++;
                    this.ProcessedImages.BeginInvoke((Action)(() => {
                        this.ShowProcessedImagesCount();
                    }));
                }
            });
        }

        // Method for drawing final positioned images
        private void DrawImage(List<SentenceGraph> result)
        {
            // Clear draw field
            this.MyRenderer.ResetImage();

            // Draw whole graph
            foreach (var graph in result)
            {
                // drawing each vertex(group) of a graph
                var drawables = graph.Groups ?? graph.Vertices;
                foreach (var vertex in drawables.OrderBy(v => v.ZIndex))
                    vertex.Draw(this.MyRenderer, this.Manager);
            }
        }

        /// <summary>
        /// Function to image generation for form
        /// </summary>
        private void GenerateImageForView()
        {
            // Check some basic info about text
            if (!this.CheckSentence())
                return;

            try
            {
                // Generating image
                var result = this.GenerateImage(this.sentenceBox.Text);

                // Drawing image
                this.DrawImage(result);

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
        /// Function to image generation from given description
        /// </summary>
        /// <param name="description">Image description given as simple text</param>
        /// <returns></returns>
        private List<SentenceGraph> GenerateImage(string description)
        {
            // Parsing given text
            var result = this.MyParser.ParseText(description, this.resolutionBox.Width, this.resolutionBox.Height);

            // positioning and drawing phase of generation
            foreach (var graph in result)
            {
                // positioning given sentence graph with given with and height
                this.MyPositioner.Positionate(graph, this.ImageResolution.Width, this.ImageResolution.Height);
            }

            return result;
        }

        private void ShowProcessedImagesCount()
        {
            this.ProcessedImages.Text = "Processed: " + this.Processed;
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

        /// <summary>
        /// Resolution select initialization
        /// </summary>
        private void FillResolutionBox()
        {
            this.resolutionBox.DataSource = new ResolutionItem[]
            {
                new ResolutionItem(1920, 1080),
                new ResolutionItem(1280, 1080),
                new ResolutionItem(1280, 720),
                new ResolutionItem(960, 540),
                new ResolutionItem(640, 360),
                new ResolutionItem(640, 480)
            };

            this.resolutionBox.SelectedIndex = 3;
        }

        /// <summary>
        /// Method for changing resolution in which should be images generated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResolutionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResolutionItem item = (ResolutionItem)this.resolutionBox.SelectedValue;

            this.ImageResolution = item;
            this.MyRenderer?.SetResolution(item.Width, item.Height);
        }

        /// <summary>
        /// Choosing if dataset will be used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.UsingDataset = DataSetCheckBox.Checked;
        }

        /// <summary>
        /// Method for preparing open file dialog to load dataset
        /// </summary>
        /// <returns>Configured open file dialog</returns>
        private OpenFileDialog ReturnConfiguredOpenFileDialog()
        {
            return new OpenFileDialog()
            {
                CheckFileExists = true,
                AddExtension = true,
                Filter = "TXT files (*.txt)|*.txt"
            };
        }

        /// <summary>
        /// Method for preparing folder browser dialog where images from dataset should be saved
        /// </summary>
        /// <returns>Configured folder browser dialog</returns>
        private FolderBrowserDialog ReturnConfiguredFolderBrowserDialog()
        {
            return new FolderBrowserDialog()
            {
                Description = "Choose directory for saving images",
                ShowNewFolderButton = true
            };
        }

        /// <summary>
        /// Method for loading dataset from dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadDataset_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = this.ReturnConfiguredOpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                this.DatasetFileName = openFileDialog.FileName;
        }

        /// <summary>
        /// Internal struct representing resolution for images
        /// </summary>
        private struct ResolutionItem
        {
            public int Width { get; }
            public int Height { get; }

            public ResolutionItem(int width, int height)
            {
                this.Width = width;
                this.Height = height;
            }

            public override string ToString()
            {
                return $"{this.Width}x{this.Height} px";
            }
        }
    }
}
