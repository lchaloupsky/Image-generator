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
        // IDrawable inteface properties
        public Vector2? Position { get; set; }
        public int ZIndex { get; set; } = 0;
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned => this.Position != null;

        public string DependencyType { get; private set; }

        public Image Image { get; }

        public IDrawable Group { get; set; }

        private List<Noun> Nouns { get; }

        private List<IPositionateEdge> Dependencies { get; }

        public List<Adposition> Adpositions { get; set; }

        private EdgeFactory EdgeFactory { get; }

        private ElementFactory ElementFactory { get; }

        public bool Processed { get; set; } = false;

        public int Id { get; }

        public NounSet(ElementFactory elementFactory, EdgeFactory edgeFactory, params Noun[] args)
        {
            this.Nouns = new List<Noun>();
            this.Dependencies = new List<IPositionateEdge>();
            this.Nouns.AddRange(args);
            this.Id = args[0].Id;
            this.DependencyType = args[0].DependencyType;
            this.ElementFactory = elementFactory;
            this.EdgeFactory = edgeFactory;

            foreach (Noun noun in args)
            {
                if(noun.Dependencies.Count == 0)
                    this.Dependencies.AddRange(noun.Dependencies);
            }
        }

        public IProcessable Process(IProcessable element, SentenceGraph graph)
        {
            switch (element)
            {
                case Noun noun: return this.ProcessElement(noun, graph);
                case Adjective adj: return this.ProcessElement(adj, graph);
                default: break;
            }

            return this;
        }

        private IProcessable ProcessElement(Noun noun, SentenceGraph graph)
        {
            if (noun.DependencyType == "conj")
                this.Nouns.Add(noun);
            else
            {
                //REDO
                //this.Dependencies.Add(this.EdgeFactory.Create(this, noun, GetAdpositions()));
                //this.ClearAdpositions();
                Console.WriteLine();
            }

            return this;
        }

        private IProcessable ProcessElement(Adjective adj, SentenceGraph graph)
        {
            foreach (var noun in this.Nouns)
                noun.Process(adj, graph);

            return this;
        }

        public IProcessable FinalizeProcessing(SentenceGraph graph)
        {
            this.Nouns.ForEach(noun => noun.FinalizeProcessing(graph));
            return this.Nouns.First();
        }

        public void CombineIntoGroup(IDrawable drawable)
        {
            //TODO
        }

        public void Draw(Renderer renderer, ImageManager manager)
        {
            this.Nouns.ForEach(noun => noun.Draw(renderer, manager));
        }

        public override string ToString()
        {
            return string.Join(", ", this.Nouns);
        }
    }
}
