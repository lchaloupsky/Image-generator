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
    /// <summary>
    /// Class representing noun in sentence
    /// </summary>
    class Noun : Element, IDrawable
    {
        // --------Public properties--------
        // Noun actions
        public List<Verb> Actions { get; }

        // Tree dependencies of this noun
        public List<IPositionateEdge> Dependencies { get; }

        // List of adpositions belonging to this noun
        public List<Adposition> Adpositions { get; set; }

        // Number of instances of this noun, if its plural
        public int Number { get; set; }

        // Flag idicating if noun is plural or not
        public bool IsPlural { get; set; } = false;

        // Noun image
        public Image Image { get; set; }

        // Group in which noun belongs
        public IDrawableGroup Group { get; set; }

        // Noun position
        public Vector2? Position { get; set; }

        // Noun z-index
        public int ZIndex { get; set; } = 0;

        // Noun width
        public int Width { get; set; }

        // Noun height
        public int Height { get; set; }

        // Flag indicating if noun is positioned
        public bool IsPositioned => this.Position != null;

        // Flag indicating if noun is fixed
        public bool IsFixed { get; set; } = false;

        // -------Private properties--------
        // Default number for plurals
        private const int NUMBER_OF_INSTANCES = 3;

        // Future also propnouns, adverbs?..
        private List<IProcessable> Extensions { get; }

        // suffix extensions of noun (some numerals, propernouns..)
        private List<Element> Suffixes { get; }

        // Factory for creating edges
        private EdgeFactory EdgeFactory { get; }

        // Factory for creating elements
        private ElementFactory ElementFactory { get; }

        // Image manager for getting images
        private ImageManager Manager { get; }

        public Noun(int Id, string Lemma, string Dependency, EdgeFactory factory, ElementFactory elementFactory, ImageManager manager, int width, int height) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<IProcessable>();
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
            if (verb.DependingDrawables.Count != 0)
            {
                verb.DependingDrawables.ForEach(dd => this.Process(dd, graph));
                verb.DependingDrawables.Clear();
                //return this.Process(verb.DependingDrawable, graph);
            }

            verb.RelatedActions.ForEach(ra => this.Process(ra, graph));
            verb.RelatedActions.Clear();

            if (verb.Object != null && graph.Vertices.Contains((IDrawable)verb.Object))
                graph.ReplaceVertex(this, (IDrawable)verb.Object);

            return this;
        }

        private IProcessable ProcessElement(Numeral num, SentenceGraph graph)
        {
            if (num.DependencyType == "appos")
                return this.Process(num.DependingDrawable, graph);

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
            if (adp.GetAdpositions().Count() == 1)
                this.Adpositions.Add(adp);
            else
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

            if (noun.DependencyType.Contains(":poss"))
                return this;

            // Processing relationship between noun and this
            this.ProcessEdge(graph, noun, noun.Adpositions);

            // Finalize processed noun
            noun.FinalizeProcessing(graph);

            return this;
        }

        private void ProcessEdge<T>(SentenceGraph graph, T drawable, List<Adposition> adpositions) where T : IProcessable, IDrawable
        {
            // Get adpositions from adpositions combinations
            List<string> leftAdp = this.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList();
            List<string> rightAdp = adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList();
            IPositionateEdge edge = this.EdgeFactory.Create(this, drawable, leftAdp, rightAdp);

            // Clear used adpositions
            if (leftAdp.Count == 0)
                this.Adpositions.Clear();
            if (rightAdp.Count == 0)
                adpositions.Clear();

            // Add only not null edge
            if (edge != null)
                graph.AddEdge(edge);
            else
            {
                // Check if drawable contains "of" -> then it is an extension of this
                if (adpositions.Count == 1 && adpositions[0].ToString() == "of")
                {
                    // replace vertex
                    graph.ReplaceVertex(this, drawable);

                    // Add to extensions
                    this.Extensions.Add(drawable);
                }
                else
                    graph.AddVertex(drawable);
            }
        }

        private IProcessable ProcessElement(NounSet nounSet, SentenceGraph graph)
        {
            if (nounSet.DependencyType == "conj")
            {
                nounSet.DependencyType = this.DependencyType;
                nounSet.Nouns.Insert(0, this);
                nounSet.Adpositions.AddRange(this.Adpositions);
                this.Adpositions.Clear();

                return nounSet;
            }

            if (nounSet.DependencyType == "compound")
            {
                this.Extensions.Add(nounSet);
                return this;
            }

            // Processing relationship between noun and this
            this.ProcessEdge(graph, nounSet, nounSet.Adpositions);

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
            lock (this.Manager)
                return (this.Image = this.Manager.GetImage(this.GetFinalWordSequence()));
        }

        private string GetFinalWordSequence()
        {
            string final = "";
            if (this.Extensions.Count != 0)
                final += $"{string.Join(" ", this.Extensions)} ";

            final += $"{base.ToString()}";

            if (this.Suffixes.Count != 0)
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
