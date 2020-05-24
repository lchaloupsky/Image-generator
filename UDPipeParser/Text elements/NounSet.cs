using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Parsing;
using ImageGeneratorInterfaces.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using UDPipeParsing.Factories;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Text_elements
{
    /// <summary>
    /// Represents set of nouns, that semantically belongs together.
    /// Also represents plural form of nouns.
    /// </summary>
    public class NounSet : IDrawable, IProcessable
    {
        // ---- IDrawable interface properties ----
        public Vector2? Position
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this._position;
            }
            set
            {
                this._position = value;
            }
        }
        public int ZIndex
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this._zIndex;
            }
            set
            {
                this._zIndex = value;
            }
        }
        public int Width
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this._width;
            }
            set
            {
                this._width = value;
            }
        }
        public int Height
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this._height;
            }
            set
            {
                this._height = value;
            }
        }
        public Image Image
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this._image;
            }
            set
            {
                this._image = value;
            }
        }
        public bool IsPositioned => this.Position != null;
        public bool IsFixed { get; set; } = false;
        public IDrawableGroup Group { get; set; }

        // ---- IProcessable interface properties ----
        public string DependencyType { get; set; }
        public int Id { get; }
        public bool IsNegated { get; private set; } = false;

        // ---- Rest of public properties ----
        public List<Noun> Nouns { get; }
        public List<Adposition> Adpositions { get; set; }

        // ---- Private properties -----
        private bool IsFinalized { get; set; } = false;
        private int LastProcessedNoun { get; set; } = 0;
        private string PluralForm { get; }
        private Noun BaseNoun { get; }
        private ElementFactory ElementFactory { get; }
        private IEdgeFactory EdgeFactory { get; }
        private CoordinationType CoordinationType { get; set; } = CoordinationType.AND;
        private int NumberOfInstances { get; set; } = 3; // Default number for plurals

        // ---- Helpers ----
        private ImageCombineHelper ImageCombineHelper { get; }
        private DependencyTypeHelper DependencyTypeHelper { get; }
        private CoordinationTypeHelper CoordinationTypeHelper { get; }
        private DrawableHelper DrawableHelper { get; }
        private ProcessableHelper ProcessableHelper { get; }

        // ---- Private fields for properties ----
        private int _zIndex;
        private int _width;
        private int _height;
        private Image _image;
        private Vector2? _position;

        #region Constructors

        // Private constructor to initialize object
        private NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory)
        {
            this.Nouns = new List<Noun>();
            this.Adpositions = new List<Adposition>();
            this.ElementFactory = elementFactory;
            this.EdgeFactory = edgeFactory;
            this.ImageCombineHelper = new ImageCombineHelper();
            this.DependencyTypeHelper = new DependencyTypeHelper();
            this.CoordinationTypeHelper = new CoordinationTypeHelper();
            this.DrawableHelper = new DrawableHelper();
            this.ProcessableHelper = new ProcessableHelper();
        }

        // Constructs noun set with given noun and given number of instances
        public NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory, Noun noun, int numberOfInstances) : this(elementFactory, edgeFactory)
        {
            this.Id = noun.Id;
            this.DependencyType = noun.DependencyType;
            this.Nouns.Add(noun);
            this.GenerateNouns(numberOfInstances);
            this.NumberOfInstances = numberOfInstances;
            this.PluralForm = noun.Lemma;
            this.BaseNoun = noun;
        }

        // Constructs noun set of noun plural form with default number of instances
        public NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory, Noun noun, string pluralForm) : this(elementFactory, edgeFactory)
        {
            this.Id = noun.Id;
            this.DependencyType = noun.DependencyType;
            this.PluralForm = pluralForm;
            this.BaseNoun = noun;
            this.Nouns.Add(noun);
        }

        // Constructs regular noun set with given nouns
        public NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory, ISentenceGraph graph, params Noun[] args) : this(elementFactory, edgeFactory)
        {
            this.Nouns.AddRange(args);
            this.Id = args[0].Id;
            this.DependencyType = args[0].DependencyType;

            foreach (var noun in args)
            {
                var edge = this.EdgeFactory.Create(this, this.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
                if (edge != null)
                {
                    graph.AddEdge(edge);
                    this.Adpositions.Clear();
                }
                else
                    this.Adpositions.AddRange(noun.Adpositions);

                noun.Adpositions.Clear();
            }
        }

        #endregion

        #region Public methods

        public void Draw(IRenderer renderer, IImageManager manager)
        {
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, this.Width, this.Height);
        }

        public override string ToString()
        {
            var diff = this.PluralForm == null ? null : this.Nouns.Where(noun => noun.ToString() != this.BaseNoun.ToString()).FirstOrDefault();
            var finalPluralForm = this.PluralForm == null ? null : this.BaseNoun.ToString().Replace(this.BaseNoun.Lemma, this.PluralForm);

            return finalPluralForm != null ?
                (diff == null ?
                    finalPluralForm :
                    $"{finalPluralForm}, " + string.Join(", ", diff)) :
                this.DependencyTypeHelper.IsObject(this.DependencyType) ?
                    string.Join(", ", this.Nouns.Select(n => n.ToString()).Distinct()) :
                    string.Join(", ", this.Nouns);
        }

        public void Dispose()
        {
            Dispose(true);   
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Processing elements

        /// <summary>
        /// IProcessable interface method
        /// </summary>
        /// <param name="element">Element to process</param>
        /// <param name="graph">Sentence graph</param>
        /// <returns>Processed element</returns>
        public IProcessable Process(IProcessable element, ISentenceGraph graph)
        {
            if (element == null)
                return this;

            if (element is Negation neg)
                return this.ProcessNegation(neg);

            if (element.IsNegated && !(element is Verb))
            {
                if(element is IDrawable drawable)
                    graph.RemoveVertex(drawable, true);

                return this;
            }

            if (this.IsNegated && this.DependencyTypeHelper.IsSubject(element.DependencyType))
                return element;

            // Check if coordination type is allowed
            if (!this.CoordinationTypeHelper.IsAllowedCoordination(this.CoordinationType) && this.DependencyTypeHelper.IsConjuction(element.DependencyType))
            {
                this.CoordinationType = CoordinationType.AND;
                if (element is IDrawable vertex)
                    graph.RemoveVertex(vertex, true);

                if (element is Verb verb)
                    this.ProcessableHelper.RemoveVerbFromGraph(verb, graph);

                return this;
            }

            // Process element
            var returnElement = this.ProcessElement(element, graph);
            return returnElement.IsNegated && returnElement != this ? this : returnElement;
        }

        private IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounSet: return this.ProcessElement(nounSet, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
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
            // Skip copula
            if (this.DependencyTypeHelper.IsCopula(verb.DependencyType))
                return this;

            IProcessable processElement = this;
            // Process all depending drawables of the verb
            if (verb.DependingDrawables.Count != 0)
            {
                verb.DependingDrawables.ForEach(dd => processElement = processElement.Process(dd, graph));
                verb.DependingDrawables.Clear();
            }

            // Process all related actions
            verb.RelatedActions.ForEach(ra => processElement.Process(ra, graph));
            verb.RelatedActions.Clear();

            // Process non used adposition
            if (verb.DrawableAdposition != null)
            {
                processElement.Process(verb.DrawableAdposition, graph);
                verb.DrawableAdposition = null;
            }

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

            // Process only non negated verbs
            if (!verb.IsNegated && processElement == this)
                foreach (var noun in this.Nouns)
                    noun.Process(verb, graph);

            return processElement;
        }

        private IProcessable ProcessElement(Numeral num, ISentenceGraph graph)
        {
            // Appositinal represents offset to nouns in the set
            if (this.DependencyTypeHelper.IsAppositional(num.DependencyType) && num.GetValue() < this.NumberOfInstances)
            {
                for (int i = this.LastProcessedNoun; i <= num.GetValue() - 1 + LastProcessedNoun; i++)
                {
                    num.DependingDrawables.ForEach(dd => this.Nouns[i].Process(dd, graph));
                    num.DependingActions.ForEach(da => this.Nouns[i].Process(da, graph));
                }

                LastProcessedNoun += num.GetValue();
                return this;
            }

            // We don't process time
            if (this.DependencyTypeHelper.IsTime(num.DependencyType))
                return this;

            // Process if not numeral modifier
            if (!this.DependencyTypeHelper.IsNumeralModifier(num.DependencyType))
            {
                this.Nouns.ForEach(noun => noun.Process(num, graph));
                return this;
            }

            // Generate number of instances with given value
            this.NumberOfInstances = num.GetValue();
            this.GenerateNouns(num.GetValue());
            return this;
        }

        private IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            // If noun is in coordination relation
            if (this.DependencyTypeHelper.IsConjuction(noun.DependencyType)
                && this.CoordinationTypeHelper.IsMergingCoordination(this.CoordinationType))
            {
                // Try to create new edge between elements
                IPositionateEdge newEdge = this.EdgeFactory.Create(
                    this,
                    noun,
                    new List<string>(),
                    noun.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList(),
                    this.DependencyTypeHelper.IsSubject(noun.DependencyType)
                );

                if (newEdge != null)
                {
                    noun.Adpositions.Clear();
                    graph.AddEdge(newEdge);
                    noun.FinalizeProcessing(graph);
                }
                // If no exists add noun into this set
                else
                {
                    this.GetAllNouns();
                    this.Nouns.Add(noun);
                }

                return this;
            }

            // If this element is possessive return depending element
            if (this.DependencyTypeHelper.IsPossesive(noun.DependencyType))
                return this;

            // Let each noun process noun phrase
            if (this.DependencyTypeHelper.IsNounPhrase(noun.DependencyType) || this.DependencyTypeHelper.IsCompound(noun.DependencyType) || this.DependencyTypeHelper.IsName(noun.DependencyType))
            {
                this.Nouns.ForEach(n => n.Process(noun, graph));
                graph.ReplaceVertex(this, noun);

                return this;
            }

            // If this is negated return depending element
            if (this.IsNegated && this.DependencyTypeHelper.IsObject(this.DependencyType))
                return noun;

            // Processing relationship between noun and this
            this.DrawableHelper.ProcessEdge(graph, this.EdgeFactory, this, noun, this.Adpositions, noun.Adpositions, this.DependencyTypeHelper.IsSubject(noun.DependencyType), () =>
            {
                // Add to extensions
                noun.DependencyType = "compound";
                this.Nouns.ForEach(n => n.Process(noun, graph));
            });

            // Finalize processed noun
            noun.FinalizeProcessing(graph);

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            // If noun set is in coordination relation
            if (this.DependencyTypeHelper.IsConjuction(nounSet.DependencyType)
                && this.CoordinationTypeHelper.IsMergingCoordination(this.CoordinationType))
            {
                // Try create edge between elements
                IPositionateEdge newEdge = this.EdgeFactory.Create(
                    this,
                    nounSet,
                    new List<string>(),
                    nounSet.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList(),
                    this.DependencyTypeHelper.IsSubject(nounSet.DependencyType)
                );

                if (newEdge != null)
                {
                    nounSet.Adpositions.Clear();
                    graph.AddEdge(newEdge);
                    nounSet.FinalizeProcessing(graph);
                    return this;
                }

                // If no edge exists merge them
                this.GetAllNouns();
                this.Nouns.AddRange(nounSet.GetAllNouns());
                nounSet.Nouns.Clear();
                graph.ReplaceVertex(this, nounSet);
                return this;
            }

            // Part of this noun
            if (this.DependencyTypeHelper.IsCompound(nounSet.DependencyType) || this.DependencyTypeHelper.IsNounPhrase(nounSet.DependencyType) || this.DependencyTypeHelper.IsName(nounSet.DependencyType))
            {
                this.Nouns.ForEach(n => n.Process(nounSet, graph));
                graph.ReplaceVertex(this, nounSet);

                return this;
            }

            // Return noun set if this is negated
            if (this.IsNegated && this.DependencyTypeHelper.IsObject(this.DependencyType))
                return nounSet;

            // Processing relationship between noun set and this
            this.DrawableHelper.ProcessEdge(graph, this.EdgeFactory, this, nounSet, this.Adpositions, nounSet.Adpositions, this.DependencyTypeHelper.IsSubject(nounSet.DependencyType), () =>
            {
                // Add to extensions
                nounSet.DependencyType = "compound";
                this.Nouns.ForEach(n => n.Process(nounSet, graph));
            });

            // Finalize processed noun
            nounSet.FinalizeProcessing(graph);

            return this;
        }

        private IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            // Let each noun process adjective
            foreach (var noun in this.Nouns)
                noun.Process(adj, graph);

            return this;
        }

        private IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            // Let each noun process the adverb
            foreach (var noun in this.Nouns)
                noun.Process(adv, graph);

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, ISentenceGraph graph)
        {
            // Store new adposition
            if (adp.GetAdpositions().Count() == 1)
                this.Adpositions.Add(adp);
            else
                this.Adpositions.Insert(0, adp);

            return this;
        }

        private IProcessable ProcessNegation(Negation negation)
        {
            this.IsNegated = true;
            return this;
        }

        private IProcessable ProcessCoordination(Coordination coordination)
        {
            this.CoordinationType = coordination.CoordinationType;
            return this;
        }

        public IProcessable FinalizeProcessing(ISentenceGraph graph)
        {
            // Don't create image if already is created
            if (this._image != null)
                return this;

            // Finalize all nouns
            this.GetAllNouns().ForEach(noun => noun.FinalizeProcessing(graph));

            // Try to create new absolute edge
            IPositionateEdge newEdge = this.EdgeFactory.Create(this, this.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
            if (newEdge != null)
            {
                this.Adpositions.Clear();
                graph.AddEdge(newEdge);
            }

            return this;
        }

        public void CombineIntoGroup(IDrawable drawable)
        {
            this.DrawableHelper.CombineIntoGroup(this, drawable);
        }

        #endregion

        #region Rest of private methods

        /// <summary>
        /// Finalizes set image from all current nouns.
        /// Two possibilities -> Place in row or place in the circle
        /// </summary>
        private void FinalizeSet()
        {
            if (this.Nouns.Count > 4)
                this.SetProperties(this.ImageCombineHelper.PlaceInCircle(this.Nouns));
            else
                this.SetProperties(this.ImageCombineHelper.PlaceInRow(this.Nouns));

            this.IsFinalized = true;
        }

        /// <summary>
        /// Sets noun set dimensions and image
        /// </summary>
        /// <param name="dimTuple">Tuple containing width, height, zindex, image</param>
        private void SetProperties(Tuple<int, int, int, Image> dimTuple)
        {
            this._width = dimTuple.Item1;
            this._height = dimTuple.Item2;
            this._zIndex = dimTuple.Item3;
            this._image = dimTuple.Item4;
        }

        /// <summary>
        /// Generates given number of nouns
        /// </summary>
        /// <param name="numberOfInstances">Number of instances to generate</param>
        private void GenerateNouns(int numberOfInstances)
        {
            for (int i = 0; i < numberOfInstances - 1; i++)
                this.Nouns.Add(this.Nouns[0].Copy());
        }

        /// <summary>
        /// Gets all noun instances
        /// </summary>
        /// <returns>List of noun instances</returns>
        private List<Noun> GetAllNouns()
        {
            if (this.Nouns.Count == 1)
                this.GenerateNouns(this.NumberOfInstances);

            return Nouns;
        }

        /// <summary>
        /// Disposes managed objects
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (this.Nouns.Count == 0)
                return;

            this._image?.Dispose();
            this._image = null;
            foreach (var noun in this.Nouns)
            {
                noun.Dispose();
            }

            this.Nouns.Clear();
        }

        #endregion
    }
}
