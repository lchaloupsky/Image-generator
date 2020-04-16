using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using ImageGeneratorInterfaces.Parsing;
using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Rendering;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.Edges.Factory;
using UDPipeParsing.Factories;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Text_elements
{
    public class NounSet : IDrawable, IProcessable
    {
        // Default number for plurals
        private int NumberOfInstances { get; set; } = 3;

        private int ZIndex_;
        private int Width_;
        private int Height_;
        private Image Image_;
        private Vector2? Position_;

        public Vector2? Position
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this.Position_;
            }
            set
            {
                this.Position_ = value;
            }
        }

        public int ZIndex
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this.ZIndex_;
            }
            set
            {
                this.ZIndex_ = value;
            }
        }

        public int Width
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this.Width_;
            }
            set
            {
                this.Width_ = value;
            }
        }

        public int Height
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this.Height_;
            }
            set
            {
                this.Height_ = value;
            }
        }

        public bool IsPositioned => this.Position != null;

        public bool IsFixed { get; set; } = false;

        public string DependencyType { get; set; }

        private bool IsFinalized { get; set; } = false;

        public Image Image
        {
            get
            {
                if (!IsFinalized)
                    this.FinalizeSet();

                return this.Image_;
            }
            set
            {
                this.Image_ = value;
            }
        }

        public IDrawableGroup Group { get; set; }

        public List<Noun> Nouns { get; }

        private List<IPositionateEdge> Dependencies { get; }

        public List<Verb> Actions { get; }

        public List<Adposition> Adpositions { get; set; }

        private IEdgeFactory EdgeFactory { get; }

        private ElementFactory ElementFactory { get; }

        private ImageCombineHelper ImageCombineHelper { get; }

        private string PluralForm { get; }

        public bool Processed { get; set; } = false;

        public int Id { get; }

        public bool IsNegated { get; private set; } = false;

        private int LastProcessedNoun { get; set; } = 0;

        private CoordinationType CoordinationType { get; set; } = CoordinationType.AND;

        private NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory)
        {
            this.Nouns = new List<Noun>();
            this.Dependencies = new List<IPositionateEdge>();
            this.Adpositions = new List<Adposition>();
            this.ElementFactory = elementFactory;
            this.EdgeFactory = edgeFactory;
            this.ImageCombineHelper = new ImageCombineHelper();
        }

        public NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory, Noun noun, int numberOfInstances) : this(elementFactory, edgeFactory)
        {
            this.Id = noun.Id;
            this.DependencyType = noun.DependencyType;
            this.Nouns.Add(noun);
            this.GenerateNouns(numberOfInstances);
        }

        public NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory, Noun noun, string pluralForm) : this(elementFactory, edgeFactory)
        {
            this.Id = noun.Id;
            this.DependencyType = noun.DependencyType;
            this.PluralForm = pluralForm;
            this.Nouns.Add(noun);
        }

        public NounSet(ElementFactory elementFactory, IEdgeFactory edgeFactory, ISentenceGraph graph, params Noun[] args) : this(elementFactory, edgeFactory)
        {
            this.Nouns.AddRange(args);
            this.Id = args[0].Id;
            this.DependencyType = args[0].DependencyType;

            foreach (var noun in args)
            {
                var edge = this.EdgeFactory.Create(this, this.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
                if (edge != null)
                    graph.AddEdge(edge);
                else
                    this.Adpositions.AddRange(noun.Adpositions);

                noun.Adpositions.Clear();
            }
        }

        private void GenerateNouns(int numberOfInstances)
        {
            for (int i = 0; i < numberOfInstances - 1; i++)
                this.Nouns.Add(this.Nouns[0].Copy());
        }

        public IProcessable Process(IProcessable element, ISentenceGraph graph)
        {
            if (element is Negation)
                return this.ProcessNegation((Negation)element);

            if (element.IsNegated && !(element is Verb))
                return this;

            if (this.IsNegated && element.DependencyType == "nsubj")
                return element;

            if (!this.IsAllowedCoordination() && element.DependencyType == "conj")
            {
                this.CoordinationType = CoordinationType.AND;
                return this;
            }

            var returnElement = this.ProcessElement(element, graph);
            return returnElement.IsNegated && returnElement != this ? this : returnElement;
        }

        private IProcessable ProcessElement(IProcessable element, ISentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case NounSet nounset: return this.ProcessElement(nounset, graph);
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
            if (verb.DependencyType == "cop")
                return this;

            if (verb.DependingDrawables.Count != 0)
            {
                verb.DependingDrawables.ForEach(dd => this.Process(dd, graph));
                verb.DependingDrawables.Clear();
            }

            verb.RelatedActions.ForEach(ra =>
            {
                ra.DependingDrawables.ForEach(dd => this.Process(ra, graph));
                ra.DependingDrawables.Clear();
            });

            verb.RelatedActions.Clear();

            if (verb.DrawableAdposition != null)
                this.Process(verb.DrawableAdposition, graph);

            if (verb.Object != null && graph.Vertices.Contains((IDrawable)verb.Object))
                graph.ReplaceVertex(this, (IDrawable)verb.Object);

            if(!verb.IsNegated)
                foreach (var noun in this.Nouns)
                    noun.Process(verb, graph);

            return this;
        }

        private IProcessable ProcessElement(Numeral num, ISentenceGraph graph)
        {
            if (num.DependencyType == "appos" && num.GetValue() < this.NumberOfInstances)
            {
                this.Nouns[num.GetValue() - 1 + LastProcessedNoun].Process(num.DependingDrawable, graph);
                LastProcessedNoun += num.GetValue();
                return this;
            }

            if (num.DependencyType != "nummod")
            {
                this.Nouns.ForEach(noun => noun.Process(num, graph));
                return this;
            }

            this.NumberOfInstances = num.GetValue();
            this.GenerateNouns(num.GetValue());
            return this;
        }

        private IProcessable ProcessElement(Noun noun, ISentenceGraph graph)
        {
            if (noun.DependencyType == "conj" && (this.CoordinationType == CoordinationType.AND || this.CoordinationType == CoordinationType.NOR))
            {
                IPositionateEdge newEdge = this.EdgeFactory.Create(this, noun, new List<string>(), noun.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
                if (newEdge != null)
                {
                    noun.Adpositions.Clear();
                    graph.AddEdge(newEdge);
                    noun.FinalizeProcessing(graph);
                }
                else
                {
                    this.GetAllNouns();
                    this.Nouns.Add(noun);
                }

                return this;
            }

            if (noun.DependencyType.Contains(":poss"))
                return this;

            if (noun.DependencyType == "nmod:npmod" || noun.DependencyType == "compound")
            {
                this.Nouns.ForEach(n => n.Process(noun, graph));
                return this;
            }

            if (this.IsNegated && this.DependencyType == "dobj")
                return noun;

            // Processing relationship between noun and this
            this.ProcessEdge(graph, noun, noun.Adpositions);

            // Finalize processed noun
            noun.FinalizeProcessing(graph);

            return this;
        }

        private IProcessable ProcessElement(NounSet nounSet, ISentenceGraph graph)
        {
            if (nounSet.DependencyType == "conj" && (this.CoordinationType == CoordinationType.AND || this.CoordinationType == CoordinationType.NOR))
            {
                IPositionateEdge newEdge = this.EdgeFactory.Create(this, nounSet, new List<string>(), nounSet.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
                if (newEdge != null)
                {
                    nounSet.Adpositions.Clear();
                    graph.AddEdge(newEdge);
                    nounSet.FinalizeProcessing(graph);
                    return this;
                }

                // Merge them
                this.GetAllNouns();
                this.Nouns.AddRange(nounSet.GetAllNouns());
                nounSet.Nouns.Clear();
                graph.ReplaceVertex(this, nounSet);
                return this;
            }

            if (this.IsNegated && this.DependencyType == "dobj")
                return nounSet;

            // Processing relationship between nounset and this
            this.ProcessEdge(graph, nounSet, nounSet.Adpositions);

            // Finalize processed noun
            nounSet.FinalizeProcessing(graph);

            return this;
        }

        private List<Noun> GetAllNouns()
        {
            if (this.Nouns.Count == 1)
                this.GenerateNouns(this.NumberOfInstances);

            return Nouns;
        }

        private bool ProcessEdge<T>(ISentenceGraph graph, T drawable, List<Adposition> adpositions) where T : IProcessable, IDrawable
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
                    // Replace vertex in graph
                    graph.ReplaceVertex(this, drawable);

                    // Add to extensions
                    drawable.DependencyType = "compound";
                    this.Nouns.ForEach(n => n.Process(drawable, graph));
                }
                else
                    graph.AddVertex(drawable);
            }

            return edge != null;
        }

        private IProcessable ProcessElement(Adjective adj, ISentenceGraph graph)
        {
            foreach (var noun in this.Nouns)
                noun.Process(adj, graph);

            // TODO do extensions
            return this;
        }

        private IProcessable ProcessElement(Adverb adv, ISentenceGraph graph)
        {
            foreach (var noun in this.Nouns)
                noun.Process(adv, graph);

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, ISentenceGraph graph)
        {
            if (adp.GetAdpositions().Count() == 1)
                this.Adpositions.Add(adp);
            else
                this.Adpositions.Insert(0, adp);

            return this;
        }

        public IProcessable FinalizeProcessing(ISentenceGraph graph)
        {
            if (this.Image_ != null)
                return this;

            this.GetAllNouns().ForEach(noun => noun.FinalizeProcessing(graph));

            IPositionateEdge newEdge = this.EdgeFactory.Create(this, this.Adpositions.SelectMany(a => a.GetAdpositions()).Select(a => a.ToString()).ToList());
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

        private void FinalizeSet()
        {
            if (this.Nouns.Count > 4)
                this.SetProperties(this.ImageCombineHelper.PlaceInCircle(this.Nouns));
            else
                this.SetProperties(this.ImageCombineHelper.PlaceInRow(this.Nouns));

            this.IsFinalized = true;
        }

        private void SetProperties(Tuple<int, int, int, Image> dimTuple)
        {
            this.Width_ = dimTuple.Item1;
            this.Height_ = dimTuple.Item2;
            this.ZIndex_ = dimTuple.Item3;
            this.Image_ = dimTuple.Item4;
        }

        private bool IsAllowedCoordination()
        {
            return this.CoordinationType != CoordinationType.OR && this.CoordinationType != CoordinationType.NOR;
        }

        public void Draw(IRenderer renderer, IImageManager manager)
        {
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, this.Width, this.Height);
        }

        public override string ToString()
        {
            return this.PluralForm ?? string.Join(", ", this.Nouns);
        }

        public void Dispose()
        {
            this.Image = null;
            foreach (var noun in this.Nouns)
            {
                noun.Dispose();
            }
        }
    }
}
