using Microsoft.VisualStudio.TestTools.UnitTesting;
using UDPipeParsing;
using UDPipeParsing.Preprocessors;

namespace UDPipeParserTests
{
    [TestClass]
    public class PreprocessorsTests
    {
        private MissingArticlePreprocessor MissingArticlePreprocessor { get; }
        private CaptitalLetterPreprocessor CaptitalLetterPreprocessor { get; }
        private TextToNumberPreprocessor TextToNumberPreprocessor { get; }

        public PreprocessorsTests()
        {
            this.MissingArticlePreprocessor = new MissingArticlePreprocessor(new UDPipeClient("english-ud-1.2-160523"));
            this.TextToNumberPreprocessor = new TextToNumberPreprocessor();
            this.CaptitalLetterPreprocessor = new CaptitalLetterPreprocessor();

        }

        [TestMethod]
        public void CapitalLetterConvertTest()
        {
            string text = "lower text";
            var preprocessed = this.CaptitalLetterPreprocessor.Preprocess(text);

            Assert.IsTrue(char.IsUpper(preprocessed[0]));
            Assert.AreEqual(preprocessed[0], char.ToUpper(text[0]));
            Assert.AreEqual(preprocessed.Substring(1), text.Substring(1));

            text = "Upper text";
            preprocessed = this.CaptitalLetterPreprocessor.Preprocess(text);
            Assert.AreEqual(text, preprocessed);
        }

        [TestMethod]
        public void TextToNumberConvertTest()
        {
            string number, preprocessed;

            number = "one";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("1", preprocessed);

            number = "twenty-two";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("22", preprocessed);

            number = "one hundred";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("100", preprocessed);

            number = "1";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("1", preprocessed);

            number = "21312412";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("1000000", preprocessed);

            number = "one hundred and forty two";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("142", preprocessed);

            number = "One thousand";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("1000", preprocessed);

            number = "seventeen";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("17", preprocessed);

            number = "eighty";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("80", preprocessed);

            number = "Eleven";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("11", preprocessed);

            number = "Twenty-two thousand one hundred and thirty nine";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.AreEqual("22139", preprocessed);

            number = "several";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) > 1);
            Assert.IsTrue(int.Parse(preprocessed) <= 11);

            number = "thousands";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) >= 1000);

            number = "hundreds";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) >= 100);

            number = "a large quantity of";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) >= 20);

            number = "a few";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) <= 11);
            Assert.IsTrue(int.Parse(preprocessed) > 1);

            number = "very few";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) <= 6);
            Assert.IsTrue(int.Parse(preprocessed) > 1);

            number = "not very few";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) >= 2);

            number = "not a lot of";
            preprocessed = this.TextToNumberPreprocessor.Preprocess(number);
            Assert.IsTrue(int.Parse(preprocessed) <= 26);
            Assert.IsTrue(int.Parse(preprocessed) > 1);
        }

        [TestMethod]
        public void MissingArticlePreprocessTest()
        {
            string textToPreprocess, correctText, preprocessedText;

            textToPreprocess = "Big woman on table";
            correctText = "The big woman on the table";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "a woman on table in room";
            correctText = "a woman on the table in the room";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "woman on a table in a room";
            correctText = "The woman on a table in a room";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "a woman is playing football";
            correctText = "a woman is playing the football";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "Very large woman is playing with ball on top of the mountain";
            correctText = "The very large woman is playing with the ball on the top of the mountain";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "very large woman is coming from home";
            correctText = "The very large woman is coming from the home";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "a very large woman is coming from the home";
            correctText = "a very large woman is coming from the home";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "Artists are coming home";
            correctText = "Artists are coming home";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "Women are painting a picture";
            correctText = "Women are painting a picture";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);

            textToPreprocess = "Some men are playing sports";
            correctText = "Some men are playing sports";
            preprocessedText = this.MissingArticlePreprocessor.Preprocess(textToPreprocess);
            Assert.AreEqual(correctText, preprocessedText);
        }
    }
}
