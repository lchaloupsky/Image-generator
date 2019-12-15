using Image_Generator.Models.Edges;
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

        private List<Noun> Nouns { get; }

        private List<DefaultEdge> Dependecies { get; }

        public bool Processed { get; set; } = false;

        public int Id { get; }

        public NounSet()
        {
            this.Nouns = new List<Noun>();
            this.Dependecies = new List<DefaultEdge>();
        }

        public NounSet(params Noun[] args) : this()
        {
            this.Nouns.AddRange(args);
            this.Id = args[0].Id;

            foreach (Noun noun in args)
                this.Dependecies.AddRange(noun.Dependencies);
        }

        public IProcessable Process(IProcessable element)
        {
            switch (element)
            {
                case Noun noun:
                    this.Nouns.Add(noun);
                    break;
                case Adjective adj:
                    foreach (var noun in this.Nouns)
                        noun.Process(adj);
                    break;
                default:
                    break;
            }

            return this;
        }

        public void Draw(Renderer renderer, ImageManager manager)
        {
            this.Nouns.ForEach(noun => noun.Draw(renderer, manager));
        }
    }
}
