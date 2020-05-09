using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Parsing;
using ImageGeneratorInterfaces.Rendering;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using UDPipeParsing.Factories;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Class representing noun in sentence
    /// </summary>
    public class Noun : Element, IDrawable
    {
        // --------Public properties--------
        // Noun actions
        public List<Verb> Actions { get; }

        // List of adpositions belonging to this noun
        public List<Adposition> Adpositions { get; set; }

        // Noun extensions
        public List<IProcessable> Extensions { get; }

        // suffix extensions of noun (some numerals, propernouns..)
        public List<Element> Suffixes { get; }

        // Flag idicating if noun is plural or not
        public bool IsPlural { get; set; } = false;

        // Scale of Noun
        public float Scale { get; set; } = 1;

        // -------IDrawable interface props-------
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

        // Factory for creating edges
        private IEdgeFactory EdgeFactory { get; }

        // Factory for creating elements
        private ElementFactory ElementFactory { get; }

        // Image manager for getting images
        private IImageManager Manager { get; }

        private DrawableHelper DrawableHelper { get; }

        public Noun(int Id, string Lemma, string Dependency, IEdgeFactory factory, ElementFactory elementFactory, IImageManager manager, int width, int height) : base(Id, Lemma, Dependency)
        {
            this.Extensions = new List<IProcessable>();
            this.Suffixes = new List<Element>();
            this.Actions = new List<Verb>();
            this.Adpositions = new List<Adposition>();
            this.EdgeFactory = factory;
            this.ElementFactory = elementFactory;
            this.Manager = manager;
            this.Width = width;
            this.Height = height;
            this.DrawableHelper = new DrawableHelper();
        }

        #region Public methods

        public void Draw(IRenderer renderer, IImageManager manager)
        {
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, this.Width, this.Height);
        }

        /// <summary>
        /// Returns copy of this noun
        /// </summary>
        /// <returns>Copy of this</returns>
        public Noun Copy()
        {
            var noun = new Noun(this.Id, this.Lemma, this.DependencyType, this.EdgeFactory, this.ElementFactory, this.Manager, this.Width, this.Height);

            noun.Extensions.AddRange(this.Extensions);
            noun.Suffixes.AddRange(this.Suffixes);
            noun.Actions.AddRange(this.Actions);
            noun.Scale = this.Scale;

            return noun;
        }

        public override string ToString()
        {
            return this.GetFinalWordSequence();
        }

        public void Dispose()
        {
            this.Image = null;
        }

        #endregion

        #region Processing elements

        protected override IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
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
                case Coordination cor: return this.ProcessCoordination(cor);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Verb verb, ISentenceGraph graph)
        {
            // skip copula
            if (this.DependencyHelper.IsCopula(verb.DependencyType))
                return this;

            IProcessable processElement = this;
            // Dont process negated verb
            if (!verb.IsNegated)
                this.Actions.Add(verb);

            // Process all depending drawables
            if (verb.DependingDrawables.Count != 0)
            {
                verb.DependingDrawables.ForEach(dd => processElement = processElement.Process(dd, graph));
                verb.DependingDrawables.Clear();
            }

            // Process all related actions
            verb.RelatedActions.ForEach(ra => processElement.Process(ra, graph));
            verb.RelatedActions.Clear();

            // Process unprocessed adposition
            if (verb.DrawableAdposition != null)
                processElement.Process(verb.DrawableAdposition, graph);

            // Replace verb object in the graph
            if (verb.Object != null)
            {
                if (verb.Object is NounSet)
                    ((NounSet)verb.Object).Nouns.ForEach(n =>
                    {
                        if (graph.Vertices.Contains(n))
                            graph.ReplaceVertex((IDrawable)processElement, n);
                    });

                if (graph.Vertices.Contains((IDrawable)verb.Object))
                    graph.ReplaceVertex((IDrawable)processElement, (IDrawable)verb.Object);
            }

            return processElement;
        }

        private IProcessable ProcessElement(Numeral num, ISentenceGraph graph)
        {
            // Process appositinal
            if (this.DependencyHelper.IsAppositional(num.DependencyType))
            {
                IProcessable processElem = this;
                processElem = processElem.Process(num.DependingDrawable, graph);
                processElem = processElem.Process(num.DependingAction, graph);
                return processElem;
            }

            // Process numeral expressing part of noun phrase
            if (this.DependencyHelper.IsNounPhrase(this.DependencyType))
            {
                this.Extensions.Add(num);
                return this;
            }

            // We dont process time
            if (this.DependencyHelper.IsTime(num.DependencyType))
                return this;

            // Add extension if numeral is not modifying number of instances
            if (!this.DependencyHelper.IsNumeralModifier(num.DependencyType))
            {
                if (num.Id > this.Id)
                    this.Suffixes.Add(num);
                else
                    this.Extensions.Add(num);

                return this;
            }

            // no need to create noun set
            if (num.GetValue() <= 1)
                return this;

            // Create new noun with given number of values
            return new NounSet(this.ElementFactory, this.EdgeFactory, this, num.GetValue());
        }

        private IProcessable ProcessElement(Adposition adp, ISentenceGraph graph)
        {
            if (adp.GetAdpositions().Count() == 1)
                this.Adpositions.Add(adp);
            else
                this.Adpositions.Insert(0, adp);

            return this;
        }

        private IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            // Use the scale of the functional adjective
            if (adj is FunctionalAdjective)
                this.Scale *= ((FunctionalAdjective)adj).Scale;

            this.Extensions.Add(adj);
            return this;
        }

        private IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            this.Extensions.Add(adv);
            return this;
        }

        private IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            // Process merging coordination type
            if (this.DependencyHelper.IsConjuction(noun.DependencyType) && this.CoordinationTypeHelper.IsMergingCoordination(this.CoordinationType))
            {
                // Try to create edge between elements
                IPositionateEdge newEdge = this.EdgeFactory.Create(
                    this,
                    noun,
                    new List<string>(),
                    noun.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList(),
                    this.DependencyHelper.IsSubject(noun.DependencyType)
                );

                if (newEdge != null)
                {
                    noun.Adpositions.Clear();
                    graph.AddEdge(newEdge);
                    noun.FinalizeProcessing(graph);
                    return this;
                }

                // If no edge was found create new nounset
                return this.ElementFactory.Create(this, noun, graph);
            }

            // Part of noun phrase
            if (this.DependencyHelper.IsCompound(noun.DependencyType) || this.DependencyHelper.IsNounPhrase(noun.DependencyType) || this.DependencyHelper.IsName(noun.DependencyType))
            {
                this.Extensions.Add(noun);
                return this;
            }

            // Skip possesive relation
            if (this.DependencyHelper.IsPossesive(noun.DependencyType))
                return this;

            // Return depending drawable if this is negated
            if (this.IsNegated && this.DependencyHelper.IsObject(this.DependencyType))
                return noun;

            // Processing relationship between nounset and this
            this.DrawableHelper.ProcessEdge(graph, this.EdgeFactory, this, noun, this.Adpositions, noun.Adpositions, this.DependencyHelper.IsSubject(noun.DependencyType), () =>
             {
                 // Add to extensions
                 this.Extensions.Add(noun);
             });

            // Finalize processed noun
            noun.FinalizeProcessing(graph);

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            // Process merging coordination type
            if (this.DependencyHelper.IsConjuction(nounSet.DependencyType) && this.CoordinationTypeHelper.IsMergingCoordination(this.CoordinationType))
            {
                // Try to create edge between elements
                IPositionateEdge newEdge = this.EdgeFactory.Create(
                    this,
                    nounSet,
                    new List<string>(),
                    nounSet.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList(),
                    this.DependencyHelper.IsSubject(nounSet.DependencyType)
                );

                if (newEdge != null)
                {
                    nounSet.Adpositions.Clear();
                    graph.AddEdge(newEdge);
                    nounSet.FinalizeProcessing(graph);
                    return this;
                }

                // If no edge was found insert this noun into the nounset
                nounSet.DependencyType = this.DependencyType;
                nounSet.Nouns.Insert(0, this);
                nounSet.Adpositions.AddRange(this.Adpositions);
                this.Adpositions.Clear();

                return nounSet;
            }

            // Part of this noun
            if (this.DependencyHelper.IsCompound(nounSet.DependencyType) || this.DependencyHelper.IsNounPhrase(nounSet.DependencyType) || this.DependencyHelper.IsName(nounSet.DependencyType))
            {
                this.Extensions.Add(nounSet);
                return this;
            }

            // Skip negated
            if (this.IsNegated && this.DependencyHelper.IsObject(this.DependencyType))
                return nounSet;

            // Processing relationship between nounset and this
            this.DrawableHelper.ProcessEdge(graph, this.EdgeFactory, this, nounSet, this.Adpositions, nounSet.Adpositions, this.DependencyHelper.IsSubject(nounSet.DependencyType), () =>
            {
                // Add to extensions
                this.Extensions.Add(nounSet);
            });

            // Finalize processed noun
            nounSet.FinalizeProcessing(graph);

            return this;
        }

        public override IProcessable FinalizeProcessing(ISentenceGraph graph)
        {
            if (this.Image != null)
                return this;

            this.GetImage();

            // If this is plural finalize wih nounset
            IPositionateEdge newEdge;
            if (this.IsPlural)
            {
                var finalizingElement = new NounSet(this.ElementFactory, this.EdgeFactory, this, NUMBER_OF_INSTANCES);
                newEdge = this.EdgeFactory.Create(finalizingElement, this.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
                return finalizingElement;
            }

            // Try to create new absolute edge
            newEdge = this.EdgeFactory.Create(this, this.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
            if (newEdge != null)
                graph.AddEdge(newEdge);

            // Scaling from accumulated scale
            this.Width = (int)(this.Width * this.Scale);
            this.Height = (int)(this.Height * this.Scale);

            return this;
        }

        public void CombineIntoGroup(IDrawable drawable)
        {
            this.DrawableHelper.CombineIntoGroup(this, drawable);
        }

        #endregion

        #region Rest of private methods

        /// <summary>
        /// Gets image from image manager
        /// </summary>
        /// <returns>Image</returns>
        private Image GetImage()
        {
            this.Image = this.Manager.GetImage(this.GetFinalWordSequence(), this.Lemma);
            this.DrawableHelper.ResizeToImage(this);

            return this.Image;
        }

        /// <summary>
        /// Gets final string word seuqence
        /// </summary>
        /// <returns>String representation</returns>
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

        #endregion
    }
}
