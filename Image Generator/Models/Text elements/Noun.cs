using Image_Generator.Models.Edges;
using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class Noun : Element, IDrawable
    {
        // Default number for plurals
        private const int NUMBER_OF_INSTANCES = 3;

        // Future also propnouns, adverbs?..
        private List<Element> Extensions { get; }

        // Tree dependencies of this noun
        public List<DefaultEdge> Dependencies { get; }

        // Number of instances of this noun, if its plural
        public int Number { get; set; } = NUMBER_OF_INSTANCES;

        // Flag idicating if noun is plural or not
        private bool Plural { get; } = false;

        // Drawable of this noun
        private Drawable MyDrawable { get; set; }

        // IDrawable inteface properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Noun(int Id, string Lemma, string Dependency) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<Element>();
            this.Dependencies = new List<DefaultEdge>();
        }

        public Noun(int Id, string Lemma, string Dependency, int Number) : this(Id, Lemma, Dependency)
        {
            this.Number = Number;
            this.Plural = true;
        }

        public Noun(int Id, string Lemma, string Dependecy, bool Plural) : this(Id, Lemma, Dependecy)
        {
            this.Plural = Plural;
        }

        public void Draw(Renderer renderer, ImageManager manager)
        {
            this.Dependencies.ForEach(edge =>
            {
                if (edge.Right is IDrawable)
                    ((IDrawable)(edge.Right)).Draw(renderer, manager);
            });

            renderer.DrawImage(this.GetImage(manager).MyImage);
        }

        private Drawable GetImage(ImageManager manager)
        {
            return (this.MyDrawable = new Drawable(manager.GetImage(this.GetFinalWordSequence())));
        }

        public override IProcessable Process(IProcessable element)
        {
            switch (element)
            {
                case Adjective adj:
                    this.Extensions.Add(adj);
                    break;
                case Noun noun:
                    if (noun.DependencyType == "conj")
                        return new NounSet(this, noun);
                    else
                        this.Dependencies.Add(new DefaultEdge(this, noun));
                    break;
                case NounSet nounSet:
                    this.Dependencies.Add(new DefaultEdge(this, nounSet));
                    break;
                default:
                    break;
            }

            return this;
        }

        private string GetFinalWordSequence()
        {
            string finalWordSequence = "";
            foreach (var ext in this.Extensions)
                finalWordSequence += $" {ext}";

            return $"{finalWordSequence} {this.Lemma}";
        }

        public override string ToString()
        {
            return this.GetFinalWordSequence();
        }
    }
}
