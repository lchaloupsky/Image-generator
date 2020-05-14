using ImageManagement;
using ImageManagement.Captioning;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ImageManagerTests
{
    [TestClass]
    public class ImageManagerTests
    {
        private const int WIDTH = 200;
        private const int HEIGHT = 200;

        private FileManager FileManager { get; set; }
        private Downloader Downloader { get; set; }
        private ImageManager ImageManager { get; set; }
        private IBMCaptioner IBMCaptioner { get; }
        private LDistanceMeter LDistanceMeter { get; }

        private string Location { get; set; }
        private string ImageLocation { get; set; }

        public ImageManagerTests()
        {
            this.IBMCaptioner = new IBMCaptioner();
            this.LDistanceMeter = new LDistanceMeter();
            this.ImageLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_image.jpg");
        }

        [TestInitialize]
        public void InitTest()
        {
            this.Location = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetRandomFileName());
        }

        [TestCleanup]
        public void CleanTest()
        {
            if (this.FileManager != null)
                this.FileManager.Close();

            if (this.ImageManager != null)
                this.ImageManager.Close();

            this.FileManager = null;
            this.Downloader = null;
            this.ImageManager = null;

            if (Directory.Exists(this.Location))
                Directory.Delete(this.Location, true);
        }

        [TestMethod]
        public void DownloadImageWithNoCaptioning()
        {
            Directory.CreateDirectory(this.Location);
            this.Downloader = new Downloader(ConfigurationManager.AppSettings["apiKey"], ConfigurationManager.AppSettings["secret"], this.Location);
            Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 0);

            using (var image = this.Downloader.DownloadImage("dog", "dog"))
            {
                Assert.IsNotNull(image);
                Assert.IsInstanceOfType(image, typeof(Image));
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 1);
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Last().Contains("dog"));
            }
        }

        [TestMethod]
        public void DownloadImageWithCaptioning()
        {
            Directory.CreateDirectory(this.Location);
            this.Downloader = new Downloader(ConfigurationManager.AppSettings["apiKey"], ConfigurationManager.AppSettings["secret"], this.Location);
            Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 0);

            using (var image = this.Downloader.DownloadImage("dog", "dog", true))
            {
                Assert.IsNotNull(image);
                Assert.IsInstanceOfType(image, typeof(Image));
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 1);
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Last().Contains("dog"));
            }
        }

        [TestMethod]
        public void FileManagerLoadImage()
        {
            this.FileManager = new FileManager(this.Location);
            string imageName = "test";
            Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 1);

            Image originalImage = new Bitmap(WIDTH, HEIGHT);
            string imageLocation = Path.Combine(this.Location, imageName + ".jpg");
            this.FileManager.SaveImage(originalImage, imageLocation);
            originalImage.Dispose();

            using (Image image = this.FileManager.LoadImage(imageName))
            {
                Assert.IsNotNull(image);
                Assert.IsInstanceOfType(image, typeof(Image));
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 2);
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Last().Contains(imageName + ".jpg"));
            }
        }

        [TestMethod]
        public void FileManagerSaveImage()
        {
            this.FileManager = new FileManager(this.Location);
            Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 1);

            using (Image originalImage = new Bitmap(WIDTH, HEIGHT))
            {
                string imageLocation = Path.Combine(this.Location, "test.jpg");
                this.FileManager.SaveImage(originalImage, imageLocation);

                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 2);
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Last().Contains("test.jpg"));
            }
        }

        [TestMethod]
        public void FileManagerDeleteImages()
        {
            this.FileManager = new FileManager(this.Location);
            Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 1);

            using (Image originalImage = new Bitmap(WIDTH, HEIGHT))
            {
                for (int i = 0; i < 10; i++)
                {
                    string imageLocation = Path.Combine(this.Location, $"test{i}.jpg");
                    this.FileManager.SaveImage(originalImage, imageLocation);
                }
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 11);

                this.FileManager.DeleteAll();
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 1);
                var dsdasd = Directory.EnumerateFiles(this.Location).Last();
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Last().Contains(".lockfile"));
            }
        }

        [TestMethod]
        public void FileManagerCheckImageExistence()
        {
            this.FileManager = new FileManager(this.Location);
            Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 1);

            using (Image originalImage = new Bitmap(WIDTH, HEIGHT))
            {
                string imageLocation = Path.Combine(this.Location, "test.jpg");
                this.FileManager.SaveImage(originalImage, imageLocation);
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 2);
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Last().Contains("test.jpg"));

                Assert.IsTrue(this.FileManager.CheckImageExistence("test"));
                Assert.IsFalse(this.FileManager.CheckImageExistence("random"));
                Assert.IsFalse(this.FileManager.CheckImageExistence("rest"));
            }
        }

        [TestMethod]
        public void ImageManagerCheckImageExistence()
        {
            this.ImageManager = new ImageManager(this.Location);
            var imageName = "testing";

            Assert.IsFalse(this.ImageManager.CheckImageExistence(imageName));
            using (Image image = this.ImageManager.GetImage(imageName))
            {
                Assert.IsTrue(this.ImageManager.CheckImageExistence(imageName));
                Assert.IsFalse(this.ImageManager.CheckImageExistence("random"));
                Assert.IsFalse(this.ImageManager.CheckImageExistence("rest"));
            }

            imageName = "testtest";
            using (Image originalImage = new Bitmap(WIDTH, HEIGHT))
            {
                string imageLocation = Path.Combine(this.Location, imageName + ".jpg");
                originalImage.Save(imageLocation);
                Assert.IsTrue(Directory.EnumerateFiles(this.Location).Count() == 3);

                Assert.IsTrue(this.ImageManager.CheckImageExistence(imageName));
                Assert.IsFalse(this.ImageManager.CheckImageExistence("random"));
                Assert.IsFalse(this.ImageManager.CheckImageExistence("rest"));
            }

            this.ImageManager.DeleteAllImages();
        }

        [TestMethod]
        public void ImageManagerGetImage()
        {
            this.ImageManager = new ImageManager(this.Location);

            var imageName = "testing";
            Assert.IsFalse(this.ImageManager.CheckImageExistence(imageName));
            using (Image image = this.ImageManager.GetImage(imageName))
            {
                Assert.IsNotNull(image);
                Assert.IsTrue(this.ImageManager.CheckImageExistence(imageName));
            }

            imageName = "test";
            this.ImageManager.UseImageCaptioning = true;
            Assert.IsFalse(this.ImageManager.CheckImageExistence(imageName));
            using (Image image = this.ImageManager.GetImage(imageName))
            {
                Assert.IsNotNull(image);
                Assert.IsTrue(this.ImageManager.CheckImageExistence(imageName));
            }

            imageName = "testtest";
            this.ImageManager.UseImageCaptioning = false;
            using (Image image = new Bitmap(WIDTH, HEIGHT))
            {
                string imageLocation = Path.Combine(this.Location, imageName + ".jpg");
                image.Save(imageLocation);

                using (var newImage = this.ImageManager.GetImage(imageName))
                {
                    Assert.IsNotNull(newImage);
                    Assert.IsTrue(this.ImageManager.CheckImageExistence(imageName));
                    Assert.AreEqual(newImage.Width, image.Width);
                    Assert.AreEqual(newImage.Height, image.Height);
                }
            }

            this.ImageManager.DeleteAllImages();
        }

        [TestMethod]
        public void IBMCaptionerGetCaptions()
        {
            // Create empty image
            Image image = Image.FromFile(this.ImageLocation);
            var captions = this.IBMCaptioner.GetCaptionsFromImage(image, "test");

            Assert.IsNotNull(captions);
            Assert.IsInstanceOfType(captions, typeof(List<ImageCaption>));
            Assert.AreEqual(3, captions.Count);
            captions.ForEach(capt => Assert.IsNotNull(capt.Caption));
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void IBMCaptionerGetCaptionsFromNullException()
        {
            var captions = this.IBMCaptioner.GetCaptionsFromImage(null, "test");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void LDistanceMeterOperandNull()
        {
            this.LDistanceMeter.CalculateStringDistance(null, null);
        }

        [TestMethod]
        public void LDistanceMeterGetDistance()
        {
            string first, second;

            first = "aaa";
            second = "";
            Assert.AreEqual(first.Length, this.LDistanceMeter.CalculateStringDistance(first, second));

            first = "";
            second = "aaa";
            Assert.AreEqual(second.Length, this.LDistanceMeter.CalculateStringDistance(first, second));

            first = "aaa";
            second = "aaa";
            Assert.AreEqual(0, this.LDistanceMeter.CalculateStringDistance(first, second));

            first = "";
            second = "";
            Assert.AreEqual(0, this.LDistanceMeter.CalculateStringDistance(first, second));

            first = "Random very long string";
            second = "Not so random very very long string";
            Assert.AreEqual(13, this.LDistanceMeter.CalculateStringDistance(first, second));

            first = "first string";
            second = "second string";
            Assert.AreEqual(6, this.LDistanceMeter.CalculateStringDistance(first, second));
        }
    }
}
