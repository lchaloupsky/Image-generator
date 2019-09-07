using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Parts_of_speech
{
    /// <summary>
    /// Class that represents Noun in sentence
    /// </summary>
    class Noun : PartOfSpeech
    {
        // Drawable element of the sentence
        public Drawable MyDrawable { get; set; }

        public Noun(string word, string lemma, int id) : base(word, lemma, id) { }

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

        // Method to merging Noun (Adjective?,...) together
        // Maybe in future wont be needed
        public override void Merge(List<PartOfSpeech> parts)
        {
            foreach (var part in parts)
            {
                if (part is Noun)
                    AddWordToBeginning(part.Word);
            }
        }
    }
}
