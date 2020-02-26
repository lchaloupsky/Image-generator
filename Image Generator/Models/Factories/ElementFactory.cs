using Image_Generator.Models.Interfaces;
using Image_Generator.Models.Text_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Image_Generator.Models.Factories
{
    /// <summary>
    /// Class for creating elements
    /// </summary>
    class ElementFactory
    {
        private const int DEFAULT_OBJECT_WIDTH = 180;
        private const int DEFAULT_OBJECT_HEIGHT = 120;

        private EdgeFactory EdgeFactory { get; }
        private ImageManager Manager { get; }
        private HashSet<string> KnownCasesToMap { get; } = new HashSet<string> { "top", "front", "down" };

        public ElementFactory(ImageManager manager)
        {
            this.EdgeFactory = new EdgeFactory();
            this.Manager = manager;
        }

        public IProcessable Create(string[] parts)
        {
            Element part = null;

            this.MapKnownCases(parts[2], ref parts[3]);
            switch (parts[3])
            {
                case "NOUN":
                    //TODO Plurals! Maybe Dimensions set separately!
                    part = new Noun(int.Parse(parts[0]), parts[2], parts[7], this.EdgeFactory, this, this.Manager, DEFAULT_OBJECT_WIDTH, DEFAULT_OBJECT_HEIGHT);
                    break;
                case "ADJ":
                    part = new Adjective(int.Parse(parts[0]), parts[1].ToLower(), parts[7]);
                    break;
                case "ADP":
                    part = new Adposition(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                default:
                    break;
            }

            return part;
        }

        public IProcessable Create(Noun left, Noun right)
        {
            return new NounSet(this, this.EdgeFactory, left, right);
        }
        
        //REDO SOMETIME
        private void MapKnownCases(string lemma, ref string type)
        {
            if (type == "NOUN" && this.KnownCasesToMap.Contains(lemma.ToLower()))
                type = "ADP";
        }
    }
}
