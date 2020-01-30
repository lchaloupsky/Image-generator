using Image_Generator.Models.Edges;
using Image_Generator.Models.Factories;
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
        public List<IPositionateEdge> Dependencies { get; }

        // List of adpositions belonging to this noun
        public List<Adposition> Adpositions { get; set; }

        // Number of instances of this noun, if its plural
        public int Number { get; set; }

        // Flag idicating if noun is plural or not
        private bool Plural { get; } = false;

        // Drawable of this noun
        private Image MyDrawable { get; set; }
        
        // Factory for creating edges
        private EdgeFactory EdgeFactory { get; } 

        // Factory for creating elements
        private ElementFactory ElementFactory { get; }

        // IDrawable inteface properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned { get; set; } = false;

        public Noun(int Id, string Lemma, string Dependency, EdgeFactory factory, ElementFactory elementFactory, int width, int height) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<Element>();
            this.Dependencies = new List<IPositionateEdge>();
            this.Adpositions = new List<Adposition>();
            this.EdgeFactory = factory;
            this.ElementFactory = elementFactory;
            this.Width = width;
            this.Height = height;
        }

        public Noun(int Id, string Lemma, string Dependecy, bool Plural, EdgeFactory factory, ElementFactory elementFactory, int width, int height) : this(Id, Lemma, Dependecy, factory, elementFactory, width, height)
        {
            this.Plural = Plural;
            this.Number = NUMBER_OF_INSTANCES;
        }

        public void Draw(Renderer renderer, ImageManager manager)
        {
            renderer.DrawImage(this.GetImage(manager), renderer.LastX, renderer.LastY, this.Width, this.Height);
        }

        public IEnumerable<Adposition> GetAdpositions()
        {
            return this.Adpositions.SelectMany(x => x.GetAdpositions());
        }
         
        public void ClearAdpositions()
        {
            this.Adpositions.Clear();
        }

        #region Processing depending elements
        public override IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Adjective adj: return this.ProcessElement(adj, graph);
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Adposition adp: return this.ProcessElement(adp, graph);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, SentenceGraph graph)
        {
            this.Adpositions.Insert(0, adp);
            return this;
        }

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            this.Extensions.Add(adj);
            return this;
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            if (noun.DependencyType == "conj")
                return this.ElementFactory.Create(this, noun);
    
            this.Adpositions = this.GetAdpositions().Concat(noun.GetAdpositions()).ToList();
            //TODO Adposition combinations
            graph.AddEdge(this.EdgeFactory.Create(this, noun, this.Adpositions));
            this.ClearAdpositions();

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            // TODO: NOUNSET + NOUN / NOUNSET + NOUNSET etc.
            //if (nounSet.DependencyType == "conj")
            //    return this.ElementFactory.Create(this, nounSet);

            this.Adpositions = this.GetAdpositions().Concat(nounSet.GetAdpositions()).ToList();
            this.Dependencies.Add(this.EdgeFactory.Create(this, nounSet, this.Adpositions));
            this.ClearAdpositions();

            return this;
        }

        public override IProcessable FinalizeProcessing(SentenceGraph graph)
        {
            IPositionateEdge newEdge = this.EdgeFactory.Create(this, this.GetAdpositions().ToList());
            if (!(newEdge is DefaultEdge))
                graph.AddEdge(newEdge);

            return this;
        }
        #endregion

        private Image GetImage(ImageManager manager)
        {
            return (this.MyDrawable = manager.GetImage(this.GetFinalWordSequence()));
        }

        private string GetFinalWordSequence()
        {
            if (this.Extensions.Count == 0)
                return base.ToString();

            return $"{string.Join(" ", this.Extensions)} {this.Lemma}";
        }

        public override string ToString()
        {
            return this.GetFinalWordSequence();
        }
    }
}
