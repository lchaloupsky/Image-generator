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

namespace UDPipeParserLibrary.Text_elements
{
    class NounSet : IDrawable, IProcessable
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

        private EdgeFactory EdgeFactory { get; }

        private ElementFactory ElementFactory { get; }

        private string PluralForm { get; }

        public bool Processed { get; set; } = false;

        public int Id { get; }

        private int LastProcessedNoun { get; set; } = 0;

        private NounSet(ElementFactory elementFactory, EdgeFactory edgeFactory)
        {
            this.Nouns = new List<Noun>();
            this.Dependencies = new List<IPositionateEdge>();
            this.Adpositions = new List<Adposition>();
            this.ElementFactory = elementFactory;
            this.EdgeFactory = edgeFactory;
        }

        public NounSet(ElementFactory elementFactory, EdgeFactory edgeFactory, Noun noun, int numberOfInstances) : this(elementFactory, edgeFactory)
        {
            this.Id = noun.Id;
            this.DependencyType = noun.DependencyType;
            this.Nouns.Add(noun);
            this.GenerateNouns(numberOfInstances);
        }

        public NounSet(ElementFactory elementFactory, EdgeFactory edgeFactory, Noun noun, string pluralForm) : this(elementFactory, edgeFactory)
        {
            this.Id = noun.Id;
            this.DependencyType = noun.DependencyType;
            this.PluralForm = pluralForm;
            this.Nouns.Add(noun);
        }

        public NounSet(ElementFactory elementFactory, EdgeFactory edgeFactory, SentenceGraph graph, params Noun[] args) : this(elementFactory, edgeFactory)
        {
            this.Nouns.AddRange(args);
            this.Id = args[0].Id;
            this.DependencyType = args[0].DependencyType;

            foreach (var noun in args)
            {
                var edge = this.EdgeFactory.Create(this, noun.Adpositions);
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

        public IProcessable Process(IProcessable element, SentenceGraph graph)
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
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Verb verb, SentenceGraph graph)
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

            if (verb.Object != null && graph.Vertices.Contains((IDrawable)verb.Object))
                graph.ReplaceVertex(this, (IDrawable)verb.Object);

            foreach (var noun in this.Nouns)
                noun.Process(verb, graph);

            return this;
        }

        private IProcessable ProcessElement(Numeral num, SentenceGraph graph)
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

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            if (noun.DependencyType == "conj")
            {
                this.Nouns.Add(noun);
                return this;
            }

            // Processing relationship between noun and this
            this.ProcessEdge(graph, noun, noun.Adpositions);

            // Finalize processed noun
            noun.FinalizeProcessing(graph);

            return this;
        }

        private IProcessable ProcessElement(NounSet nounset, SentenceGraph graph)
        {
            if (nounset.DependencyType == "conj")
            {
                // TODO Merge them
                Console.WriteLine();
            }

            // Processing relationship between nounset and this
            this.ProcessEdge(graph, nounset, nounset.Adpositions);

            // Finalize processed noun
            nounset.FinalizeProcessing(graph);

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
                    // Replace vertex in graph
                    graph.ReplaceVertex(this, drawable);

                    // Add to extensions
                    drawable.DependencyType = "compound";
                    this.Nouns.ForEach(n => n.Process(drawable, graph));
                }
                else
                    graph.AddVertex(drawable);
            }
        }

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            foreach (var noun in this.Nouns)
                noun.Process(adj, graph);

            // TODO do extensions
            return this;
        }

        private IProcessable ProcessElement(Adverb adv, SentenceGraph graph)
        {
            foreach (var noun in this.Nouns)
                noun.Process(adv, graph);

            return this;
        }

        private IProcessable ProcessElement(Adposition adp, SentenceGraph graph)
        {
            if (adp.GetAdpositions().Count() == 1)
                this.Adpositions.Add(adp);
            else
                this.Adpositions.Insert(0, adp);

            return this;
        }

        public IProcessable FinalizeProcessing(SentenceGraph graph)
        {
            if (this.Nouns.Count == 1)
                this.GenerateNouns(NumberOfInstances);

            this.Nouns.ForEach(noun => noun.FinalizeProcessing(graph));

            IPositionateEdge newEdge = this.EdgeFactory.Create(this, this.Adpositions);
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

        private void FinalizeSet()
        {
            if (this.Nouns.Count > 4)
                this.PlaceInCircle();
            else
                this.PlaceInRow();

            this.IsFinalized = true;
        }

        private void PlaceInRow()
        {
            double scale = 1;
            foreach (var noun in this.Nouns)
            {
                this.Width_ += noun.Group?.Width ?? noun.Width;
                this.Height_ = Math.Max(this.Height_, noun.Group?.Height ?? noun.Height);
                this.ZIndex_ = Math.Min(this.ZIndex_, noun.ZIndex);
                scale *= 0.75;

                if (noun.Group != null)
                    noun.Group.IsValid = false;
            }

            this.Height_ = (int)(this.Height_ * scale);
            this.Width_ = (int)(this.Width_ * scale);
            this.Image_ = new Bitmap(this.Width_, this.Height_);
            Graphics graphics = Graphics.FromImage(this.Image_);

            int LastX = 0;
            IDrawable finalElement;
            foreach (IDrawable noun in this.Nouns)
            {
                finalElement = noun.Group ?? noun;

                lock (finalElement.Image)
                    graphics.DrawImage(finalElement.Image, LastX, (int)(this.Height_ - finalElement.Height * scale), (int)(scale * finalElement.Width), (int)(scale * finalElement.Height));

                LastX += (int)(finalElement.Width * scale);
            }
        }

        private void PlaceInCircle()
        {
            int MaxWidth = 0;
            int MaxHeight = 0;
            foreach (var noun in this.Nouns)
            {
                MaxWidth = Math.Max(MaxWidth, noun.Group?.Width ?? noun.Width);
                MaxHeight = Math.Max(MaxHeight, noun.Group?.Height ?? noun.Height);
                this.ZIndex_ = Math.Min(this.ZIndex_, noun.ZIndex);

                if (noun.Group != null)
                    noun.Group.IsValid = false;
            }

            // REDO Scaling
            //double scale = Math.Max(Math.Pow(0.8, this.Nouns.Count), 0.05);
            double scale = Math.Pow(0.9, this.Nouns.Count);
            //var finalDim = (int)(Math.Max(MaxWidth, MaxHeight) * Math.Pow(this.Nouns.Count, scale));
            var finalDim = (int)Math.Log(Math.Max(MaxWidth, MaxHeight) * this.Nouns.Count) / 3 * 100;

            this.Height_ = finalDim;
            this.Width_ = finalDim;
            this.Image_ = new Bitmap(this.Width_, this.Height_);
            Graphics graphics = Graphics.FromImage(this.Image_);

            int newX = 0;
            int newY = 0;
            Random rand = new Random();
            IDrawable finalElement;
            foreach (IDrawable noun in this.Nouns)
            {
                finalElement = noun.Group ?? noun;
                int finalWidth = Math.Max(34, (int)(finalElement.Width * scale));
                int finalHeight = Math.Max(20, (int)(finalElement.Height * scale));

                int distance = rand.Next(finalDim / 2);
                double angleInRadians = rand.Next(360) / (2 * Math.PI);

                newX = (int)(distance * Math.Cos(angleInRadians)) + finalDim / 2;
                newY = (int)(distance * Math.Sin(angleInRadians)) + finalDim / 2;

                newX = Math.Min(newX, finalDim - finalWidth);
                newY = Math.Min(newY, finalDim - finalHeight);

                lock (finalElement.Image)
                    graphics.DrawImage(finalElement.Image, newX, newY, finalWidth, finalHeight);
            }
        }

        public void Draw(Renderer renderer, ImageManager manager)
        {
            renderer.DrawImage(this.Image, (int)this.Position.Value.X, (int)this.Position.Value.Y, this.Width, this.Height);
        }

        public override string ToString()
        {
            return this.PluralForm ?? string.Join(", ", this.Nouns);
        }
    }
}
