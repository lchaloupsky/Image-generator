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
    class NounSet : IDrawable, IProcessable
    {
        // Default number for plurals
        private const int NUMBER_OF_INSTANCES = 3;

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

        public string DependencyType { get; private set; }

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

        // REDO under some interface
        private List<Noun> Nouns { get; }

        private List<IPositionateEdge> Dependencies { get; }

        public List<Verb> Actions { get; }

        public List<Adposition> Adpositions { get; set; }

        private EdgeFactory EdgeFactory { get; }

        private ElementFactory ElementFactory { get; }

        private string PluralForm { get; }

        public bool Processed { get; set; } = false;

        public int Id { get; }

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


        public NounSet(ElementFactory elementFactory, EdgeFactory edgeFactory, SentenceGraph graph, params Noun[] args) : this (elementFactory, edgeFactory)
        {
            this.Nouns.AddRange(args);
            this.Id = args[0].Id;
            this.DependencyType = args[0].DependencyType;

            foreach (var noun in args)
            {
                var edge = this.EdgeFactory.Create(this, noun.Adpositions);
                if(edge != null)
                    graph.AddEdge(edge);
                else
                    this.Adpositions.AddRange(noun.Adpositions);

                noun.Adpositions.Clear();
            }
        }

        private void GenerateNouns(int numberOfInstances)
        {
            for (int i = 0; i < numberOfInstances - 1; i++)
            {
                this.Nouns.Add(this.Nouns[0].Copy());
            }
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

            if (verb.DependingDrawable != null)
            {
                this.Process(verb.DependingDrawable, graph);
                verb.DependingDrawable = null;
            }

            foreach (var noun in this.Nouns)
                noun.Process(verb, graph);

            return this;
        }

        private IProcessable ProcessElement(Numeral num, SentenceGraph graph)
        {
            if (num.DependencyType != "nummod")
            {
                this.Nouns.ForEach(noun => noun.Process(num, graph));
                return this;
            }

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

        private IProcessable ProcessElement(NounSet nounset, SentenceGraph graph)
        {
            if (nounset.DependencyType == "conj")
            {
                // TODO Merge them
                Console.WriteLine();
            }

            // Get adpositions from adpositions combinations
            IPositionateEdge edge = this.EdgeFactory.Create(this, nounset, this.Adpositions, nounset.Adpositions);
            if (edge != null)
                graph.AddEdge(edge);
            else
                graph.AddVertex(nounset);

            // Finalize processed noun
            nounset.FinalizeProcessing(graph);

            return this;
        }

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            foreach (var noun in this.Nouns)
                noun.Process(adj, graph);

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
            this.Adpositions.Insert(0, adp);
            return this;
        }

        public IProcessable FinalizeProcessing(SentenceGraph graph)
        {
            if (this.Nouns.Count == 1)
                this.GenerateNouns(NUMBER_OF_INSTANCES);

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

                // REDO
                newX = Math.Min(newX, finalDim - finalWidth);
                newY = Math.Min(newY, finalDim - finalHeight);

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
