using Image_Generator.Models;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.ImageManager;
using ImageManagment;
using ImagePositioner;
using ImagePositioner.Factories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDPipeParsing;

namespace Image_Generator
{
    /// <summary>
    /// UI form for Image Generator Application
    /// </summary>
    public partial class Form1 : Form
    {
        private IImageManager Manager { get; }
        private Renderer MyRenderer { get; }
        private Bitmap MyBitmap { get; set; }
        private UDPipeParser MyParser { get; }
        private Positioner MyPositioner { get; }
        private ResolutionItem ImageResolution { get; set; }

        private bool UsingCaptioning { get; set; } = false;
        private bool InProcess { get; set; } = false;
        private bool ProcessingDataset { get; set; } = false;
        private string DatasetFileName { get; set; }

        public Form1()
        {
            InitializeComponent();
            this.FillResolutionBox();
            this.MyRenderer = new Renderer(this.ImageResolution.Width, this.ImageResolution.Height);
            this.Manager = new ImageManager();
            this.MyParser = new UDPipeParser("english-ud-1.2-160523", this.Manager, new EdgeFactory());
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
        /// Function to image generation from input text
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            this.StartGeneration(this.GenerateImageForView);
        }

        /// <summary>
        /// Function to image generation from dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateDatasetButton_Click(object sender, EventArgs e)
        {
            this.StartGeneration(this.GenerateImagesForDataset);
        }

        private void StartGeneration(Action generateAction)
        {
            if (this.InProcess)
                this.ShowErrorMessage("Generation is running, you have to wait to finish previous task");

            this.InProcess = true;
            generateAction();
        }

        /// <summary>
        /// Function to image generation from dataset
        /// </summary>
        private void GenerateImagesForDataset()
        {
            // check if dataset is loaded and exists
            if (this.DatasetFileName == null || !File.Exists(this.DatasetFileName))
            {
                this.InProcess = false;
                ShowErrorMessage("You have to select existing dataset first.");
                return;
            }

            // choosing directory
            string directory;
            var dialog = ReturnConfiguredFolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                directory = dialog.SelectedPath;
            else
            {
                this.InProcess = false;
                return;
            }

            this.ProcessedImages.Visible = true;
            this.ProcessedBar.Visible = true;
            this.ProcessedBar.Value = 0;
            this.ProcessingDataset = true;

            // generate all images for descriptions from dataset in independent tasks
            using (StreamReader streamReader = File.OpenText(this.DatasetFileName))
            {
                string str;
                int counter = 0;
                int linesCount = File.ReadAllLines(this.DatasetFileName).Count();
                this.ProcessedBar.Maximum = linesCount;

                try
                {
                    while ((str = streamReader.ReadLine()) != null)
                    {
                        counter++;
                        this.CreateImageGeneratingTask(str, directory, counter).Start();
                    }
                }
                catch (IOException)
                {
                    ShowErrorMessage("Error while reading from loaded dataset.");
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
            // Create new task, that will be running in background
            return new Task(() =>
            {
                try
                {
                    var result = this.GenerateImage(str.Substring(str.IndexOf(str.First(c => char.IsWhiteSpace(c))) + 1));
                    lock (this.MyRenderer)
                    {
                        // Draw image
                        this.DrawImage(result);

                        // SaveImage
                        this.Manager.SaveImage(this.MyRenderer.GetImage(), Path.Combine(directory, counter + ".jpg"));
                    }
                }
                catch (Exception e)
                {
                    if (!e.Message.Contains("Index"))
                    {
                        Console.WriteLine();
                    }
                    Console.Error.WriteLine("__________ERROR__________" + counter);
                    Console.Error.WriteLine(e);
                }
                finally
                {
                    // update processed image count
                    this.ProcessedImages.BeginInvoke((Action)(() =>
                    {
                        this.ShowProcessedImagesCount();
                    }));
                }
            });
        }

        // Method for drawing final positioned images
        private void DrawImage(List<ISentenceGraph> result)
        {
            this.SetProcessStatus("Drawing final image");

            // Clear draw field
            this.MyRenderer.ResetImage();

            // Draw whole graph
            foreach (var graph in result)
            {
                // drawing each vertex(group) of a graph
                var drawables = graph.Groups ?? graph.Vertices;
                foreach (var vertex in drawables.OrderBy(v => v.ZIndex))
                    vertex.Draw(this.MyRenderer, this.Manager);

                graph.Dispose();
            }
        }

        /// <summary>
        /// Function to image generation for formdoes 
        /// </summary>
        private void GenerateImageForView()
        {
            // Check some basic info about text
            if (!this.CheckSentence())
            {
                this.InProcess = false;
                return;
            }

            this.ProcessStatus.Visible = true;

            Task.Run(() =>
            {
                try
                {
                    // Generating image
                    var result = this.GenerateImage(this.sentenceBox.Text);

                    // Drawing image
                    this.DrawImage(result);

                    // Get drawn image bitmap to show it in form window
                    this.generatedImage.BeginInvoke((Action)(() =>
                    {
                        this.generatedImage.Image = MyRenderer.GetImage();
                    }));

                    this.SetProcessStatus("Image succesfully generated");
                }
                catch (Exception ex)
                {
                    if (ex is WebException)
                        ShowErrorMessage("Internet connection failure");
                    else if (ex is IOException)
                        ShowErrorMessage("IO error");
                    else
                        ShowErrorMessage("Unknown exception\n" + ex.ToString());

                    this.SetProcessStatus("Failed to generate image");
                }
                finally
                {
                    this.InProcess = false;
                }
            });
        }

        /// <summary>
        /// Auxiliary method for setting actual status of image processing
        /// </summary>
        /// <param name="message"></param>
        private void SetProcessStatus(string message)
        {
            if (this.ProcessingDataset)
                return;

            this.ProcessStatus.BeginInvoke((Action)(() => 
            {
                this.ProcessStatus.Text = message;
                this.ProcessStatus.Refresh();
            }));        
        }

        /// <summary>
        /// Function to image generation from given description
        /// </summary>
        /// <param name="description">Image description given as simple text</param>
        /// <returns></returns>
        private List<ISentenceGraph> GenerateImage(string description)
        {
            this.SetProcessStatus("Parsing image description and loading/downloading images");

            // Parsing given text
            var result = this.MyParser.ParseText(description, this.resolutionBox.Width, this.resolutionBox.Height);

            this.SetProcessStatus("Positioning image");

            // positioning and drawing phase of generation
            foreach (var graph in result)
            {
                // positioning given sentence graph with given with and height
                this.MyPositioner.Positionate(graph, this.ImageResolution.Width, this.ImageResolution.Height);
            }

            return result;
        }

        /// <summary>
        /// Auxiliray method for showing actual count of processed images
        /// </summary>
        private void ShowProcessedImagesCount()
        {
            this.ProcessedBar.Value++;
            this.ProcessedImages.Text = "Processed " + this.ProcessedBar.Value + " / " + this.ProcessedBar.Maximum + " images";

            if (this.ProcessedBar.Value == this.ProcessedBar.Maximum)
            {
                this.InProcess = false;
                this.ProcessingDataset = false;
                System.GC.Collect();
            }
        }

        /// <summary>
        /// Function to save new generated image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (this.generatedImage.Image == null)
            {
                ShowErrorMessage("You have to generate image first!");
                return;
            }

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
            this.generatedImage.Width = (int)(this.generatedImage.Height * item.Ratio);
            this.generatedImage.Image = null;
        }

        /// <summary>
        /// Choosing if dataset will be used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataSetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.SentenceGeneratePanel.Visible = !this.DataSetCheckBox.Checked;
            this.DatasetPanel.Visible = this.DataSetCheckBox.Checked;
            this.DataSetCheckBox.ImageIndex = 1 - this.DataSetCheckBox.ImageIndex;
        }

        /// <summary>
        /// Choosing if imageCaptioning will be used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImageCaptCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            this.Manager.UseImageCaptioning = this.ImageCaptCheckbox.Checked;
            this.ImageCaptCheckbox.ImageIndex = 1 - this.ImageCaptCheckbox.ImageIndex;
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
                Filter = "txt files (*.txt)|*.txt|token files (*.token)|*.token"
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
            {
                this.DatasetFileName = openFileDialog.FileName;
                this.ChosenFile.Text = Path.GetFileName(this.DatasetFileName);
            }
        }

        /// <summary>
        /// Internal struct representing resolution for images
        /// </summary>
        private struct ResolutionItem
        {
            public int Width { get; }
            public int Height { get; }
            public float Ratio => this.Width * 1f / this.Height;

            public ResolutionItem(int width, int height)
            {
                this.Width = width;
                this.Height = height;
            }

            public override string ToString()
            {
                return $"{this.Width} x {this.Height} px";
            }
        }

        private void DeleteImages_Click(object sender, EventArgs e)
        {
            if (this.InProcess)
                ShowErrorMessage("You have wait to generetaion completes");

            var dialogResult = MessageBox.Show("Are you sure to delete all saved images?", "Confirm", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
                return;

            if (this.generatedImage.Image != null)
            {
                //this.generatedImage.Image.Dispose();
                this.generatedImage.Image = null;
            }

            try
            {
                this.Manager.DeleteAllImages();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                ShowErrorMessage("You have to wait until program drops all references to images.\n Try it later please.");
            }
        }
    }
}
