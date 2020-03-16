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
        private HashSet<string> KnownCasesToMap { get; } = new HashSet<string> {
            "top", "front", "down", "middle", "left", "right", "next", "midst", "bottom", "corner", "outside", "near", "edge"
        };

        public ElementFactory(ImageManager manager)
        {
            this.EdgeFactory = new EdgeFactory();
            this.Manager = manager;
        }

        public IProcessable Create(string[] parts)
        {
            IProcessable part = null;

            this.MapKnownCases(parts[2], ref parts[3]);
            switch (parts[3])
            {
                case "PROPN":
                case "NOUN":
                    part = this.ProcessNoun(parts);
                    break;
                case "ADJ":
                    part = new Adjective(int.Parse(parts[0]), parts[1].ToLower(), parts[7]);
                    break;
                case "ADP":
                    part = new Adposition(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                case "NUM":
                    part = new Numeral(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                case "VERB":
                    part = this.ProcessVerb(parts);
                    break;
                case "ADV":
                    part = new Adverb(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                default:
                    break;
            }

            return part;
        }

        private IProcessable ProcessVerb(string[] parts)
        {
            VerbForm form = VerbForm.NORMAL;

            if (parts[5].Contains("VerbForm=Part"))
                form = VerbForm.PARTICIPLE;

            if (parts[5].Contains("VerbForm=Ger"))
                form = VerbForm.GERUND;

            return new Verb(int.Parse(parts[0]), parts[2], parts[7], form, parts[1].ToLower());
        }

        private IProcessable ProcessNoun(string[] parts)
        {
            var noun = new Noun(int.Parse(parts[0]), parts[2], parts[7], this.EdgeFactory, this, this.Manager, DEFAULT_OBJECT_WIDTH, DEFAULT_OBJECT_HEIGHT);

            if (parts[5].Contains("Number=Plur"))
                return new NounSet(this, this.EdgeFactory, noun, parts[1]);

            return noun;
        }

        public IProcessable Create(Noun left, Noun right, SentenceGraph graph)
        {
            return new NounSet(this, this.EdgeFactory, graph, left, right);
        }

        //REDO SOMETIME
        private void MapKnownCases(string lemma, ref string type)
        {
            if ((type == "NOUN" || type == "ADV") && this.KnownCasesToMap.Contains(lemma.ToLower()))
                type = "ADP";
        }
    }
}
