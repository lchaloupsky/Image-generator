using Image_Generator.Models.Edges;
using Image_Generator.Models.Factories;
using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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

        public List<Verb> Actions { get; }

        // suffix extensions of noun (some numerals, propernouns..)
        private List<Element> Suffixes { get; }

        // Tree dependencies of this noun
        public List<IPositionateEdge> Dependencies { get; }

        // List of adpositions belonging to this noun
        public List<Adposition> Adpositions { get; set; }

        // Number of instances of this noun, if its plural
        public int Number { get; set; }

        // Flag idicating if noun is plural or not
        public bool IsPlural { get; set; } = false;

        // Drawable of this noun
        public Image Image { get; set; }

        public IDrawableGroup Group { get; set; }

        // Factory for creating edges
        private EdgeFactory EdgeFactory { get; }

        // Factory for creating elements
        private ElementFactory ElementFactory { get; }

        private ImageManager Manager { get; }

        // IDrawable inteface properties
        public Vector2? Position { get; set; }
        public int ZIndex { get; set; } = 0;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned => this.Position != null;
        public bool IsFixed { get; set; } = false;

        public Noun(int Id, string Lemma, string Dependency, EdgeFactory factory, ElementFactory elementFactory, ImageManager manager, int width, int height) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<Element>();
            this.Suffixes = new List<Element>();
            this.Actions = new List<Verb>();
            this.Dependencies = new List<IPositionateEdge>();
            this.Adpositions = new List<Adposition>();
            this.EdgeFactory = factory;
            this.ElementFactory = elementFactory;
            this.Manager = manager;
            this.Width = width;
            this.Height = height;
        }

        public void Draw(Renderer renderer, ImageManager manager)
        {
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, this.Width, this.Height);
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
                case Numeral num: return this.ProcessElement(num, graph);
                case Verb verb: return this.ProcessElement(verb, graph);
                case Adverb adv: return this.ProcessElement(adv, graph);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Verb verb, SentenceGraph graph)
        {
            if (verb.DependencyType == "cop")
                return this;

            this.Actions.Add(verb);
            if(verb.DependingDrawable != null)
            {
                return this.Process(verb.DependingDrawable, graph);
            }

            return this;
        }

        private IProcessable ProcessElement(Numeral num, SentenceGraph graph)
        {
            if (num.DependencyType != "nummod")
            {
                if (num.Id > this.Id)
                    this.Suffixes.Add(num);
                else
                    this.Extensions.Add(num);

                return this;
            }

            return new NounSet(this.ElementFactory, this.EdgeFactory, this, num.GetValue());
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

        private IProcessable ProcessElement(Adverb adv, SentenceGraph graph)
        {
            this.Extensions.Add(adv);
            return this;
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            if (noun.DependencyType == "conj")
                return this.ElementFactory.Create(this, noun, graph);

            if (noun.DependencyType == "compound")
            {
                this.Extensions.Add(noun);
                return this;
            }

            // Get adpositions from adpositions combinations
            IPositionateEdge edge = this.EdgeFactory.Create(this, noun, this.Adpositions, noun.Adpositions);
            if (edge != null)
                graph.AddEdge(edge);
            else
                graph.AddVertex(noun);

            // Finalize processed noun
            noun.FinalizeProcessing(graph);

            return this;
        }

        // REDO
        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            if (nounSet.DependencyType == "conj")
                return nounSet.Process(this, graph);

            // Get adpositions from adpositions combinations
            IPositionateEdge edge = this.EdgeFactory.Create(this, nounSet, this.Adpositions, nounSet.Adpositions);
            if (edge != null)
                graph.AddEdge(edge);
            else
                graph.AddVertex(nounSet);

            // Finalize processed noun
            nounSet.FinalizeProcessing(graph);

            return this;
        }

        public override IProcessable FinalizeProcessing(SentenceGraph graph)
        {
            if (this.Image == null)
                this.GetImage();

            IPositionateEdge newEdge;
            if (this.IsPlural)
            {
                var finalizingElement = new NounSet(this.ElementFactory, this.EdgeFactory, this, NUMBER_OF_INSTANCES);
                newEdge = this.EdgeFactory.Create(finalizingElement, this.Adpositions);
                return finalizingElement;
            }

            newEdge = this.EdgeFactory.Create(this, this.Adpositions);
            if (newEdge != null)
                graph.AddEdge(newEdge);

            return this;
        }
        #endregion

        public void CombineIntoGroup(IDrawable drawable)
        {
            if (drawable is MetaNoun)
            {
                drawable.CombineIntoGroup(this);
                this.Group = (MetaNoun)drawable;
                return;
            }

            IDrawableGroup group = null;
            if (this.Group == null && drawable.Group == null)
                group = new MetaNoun(this, drawable);
            else if (this.Group == null)
            {
                group = drawable.Group;
                group.CombineIntoGroup(this);
            }
            else
            {
                group = this.Group;
                group.CombineIntoGroup(drawable);   
            }

            this.Group = group;
            drawable.Group = group;
        }

        public Noun Copy()
        {
            var noun = new Noun(this.Id, this.Lemma, this.DependencyType, this.EdgeFactory, this.ElementFactory, this.Manager, this.Width, this.Height);

            noun.Extensions.AddRange(this.Extensions);
            noun.Suffixes.AddRange(this.Suffixes);
            noun.Actions.AddRange(this.Actions);

            return noun;
        }

        private Image GetImage()
        {
            return (this.Image = this.Manager.GetImage(this.GetFinalWordSequence()));
        }

        private string GetFinalWordSequence()
        {
            string final = "";
            if (this.Extensions.Count != 0)
                final += $"{string.Join(" ", this.Extensions)} ";

            final += $"{base.ToString()}";

            if(this.Suffixes.Count != 0)
                final += $" {string.Join(" ", this.Suffixes)}";

            if (this.Actions.Count != 0)
                final += $" {string.Join(" and ", this.Actions)}";

            return final;
        }

        public override string ToString()
        {
            return this.GetFinalWordSequence();
        }
    }
}
