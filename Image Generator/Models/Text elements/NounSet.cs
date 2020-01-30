﻿using Image_Generator.Models.Edges;
using Image_Generator.Models.Factories;
using Image_Generator.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Text_elements
{
    class NounSet : IProcessable, IDrawable
    {
        // IDrawable inteface properties
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsPositioned { get; set; } = false;

        public string DependencyType { get; private set; }

        private List<Noun> Nouns { get; }

        private List<IPositionateEdge> Dependencies { get; }

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

        public List<Adposition> GetAdpositions()
        {
            return this.Nouns.SelectMany(x => x.GetAdpositions()).ToList();
        }

        public void ClearAdpositions()
        {
            this.Nouns.ForEach(noun => noun.ClearAdpositions());
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
                this.Dependencies.Add(this.EdgeFactory.Create(this, noun, GetAdpositions()));
                this.ClearAdpositions();
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
