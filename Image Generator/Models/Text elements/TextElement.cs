using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    /// <summary>
    /// Abstract class for parts of speech
    /// </summary>
    class TextElement
    {
        // Word form in text
        public string Word { get; protected set; }

        // Lemma of the word
        public string Lemma { get; protected set; }

        // Word ID in the text
        public int ID { get; }

        // Drawable element of the sentence
        public Drawable MyDrawable { get; set; }

        // TODO some needed properties?
        // TODO in FUTURE some links to other parts?

        public TextElement(string word, string lemma, int id)
        {
            this.Word = word;
            this.Lemma = lemma;
        }

        /// <summary>
        /// Method for debug listing  
        /// </summary>
        public void Print()
        {
            Console.WriteLine("Word: {0}\n"
                            + "Lemma: {1}\n"
                            , this.Word, this.Lemma);
        }

        /// <summary>
        /// Method to get image of a Noun after all text processing
        /// (In the future will be return value void)
        /// </summary>
        /// <param name="manager">Manager to get image</param>
        /// <returns>Image for that Noun</returns>
        public Image GetImage(ImageManager manager)
        {
            MyDrawable = new Drawable(manager.GetImage(this.Word));
            return MyDrawable.MyImage;
        }

        public void Draw(Renderer renderer)
        {
            this.MyDrawable.Draw(renderer);
        }

        private void AddWordToBeginning(string word)
        {
            this.Word = word + " " + this.Word;
        }

        private void AddWordToEnd(string word)
        {
            this.Word += " " + word;
        }

        /// <summary>
        /// Overriden method ToString
        /// </summary>
        /// <returns>String representation of part of speech</returns>
        public override string ToString()
        {
            return $"Word: {this.Word}, Lemma: {this.Lemma}";
        }

        // TODO in FUTURE necessary abstract methods to process parts?
    }
}
