using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.Graph;
using ImageGeneratorInterfaces.ImageManager;
using ImageGeneratorInterfaces.Parsing;
using System.Collections.Generic;
using UDPipeParsing.Text_elements;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParsing.Factories
{
    /// <summary>
    /// Class for creating elements
    /// </summary>
    public class ElementFactory
    {
        private const string ADJECTIVE_NEGATION = "no";
        private const float UP_SCALE = 1.5f;
        private const float DOWN_SCALE = 0.75f;

        private IEdgeFactory EdgeFactory { get; }
        private IImageManager Manager { get; }
        private int DrawableObjectWidth { get; } = 240;
        private int DrawableObjectHeight { get; } = 180;
        private DependencyTypeHelper DependencyTypeHelper { get; } = new DependencyTypeHelper();

        // List of negations
        private HashSet<string> Negations { get; } = new HashSet<string>() {
            "not", "neither", "n't", "'t", "never"
        };

        // Known cases of nouns and adverbs to map to the adpositions
        private HashSet<string> KnownCasesToMap { get; } = new HashSet<string> {
            "top", "front", "down", "middle", "left", "right", "next", "midst", "bottom", "corner", "outside", "near", "edge", "behind", "up", "opposite", "to", "below"
        };

        // Functional adjectives, that are scaling up
        private HashSet<string> DefaultUpScales { get; } = new HashSet<string>()
        {
            "big", "large", "tall", "great", "widespread", "wide", "grand", "broad"
        };

        // Functional adjectives, that are scaling up
        private HashSet<string> LargeUpScales { get; } = new HashSet<string>()
        {
             "gigantic", "tremendous", "huge", "massive", "enormous", "giant", "immense", "vast", "robust", "mighty"
        };

        // Functional adjectives, that are scaling down
        private HashSet<string> DefaultDownScales { get; } = new HashSet<string>()
        {
            "small", "short", "slight", "little", "low", "minor", "baby", "compact"
        };

        // Functional adjectives, that are scaling down a lot
        private HashSet<string> TinyDownScales { get; } = new HashSet<string>()
        {
            "miniature", "tiny", "mini", "petite", "puny", "micro"
        };

        public ElementFactory(IImageManager manager, IEdgeFactory edgeFactory)
        {
            this.EdgeFactory = edgeFactory;
            this.Manager = manager;
        }

        public ElementFactory(IImageManager manager, IEdgeFactory edgeFactory, int defaultWidth, int defaultHeight) : this(manager, edgeFactory)
        {
            this.DrawableObjectWidth = defaultWidth;
            this.DrawableObjectHeight = defaultHeight;
        }

        /// <summary>
        /// Creates root element
        /// </summary>
        /// <param name="sentence">Input description sentence</param>
        /// <returns>New Root</returns>
        public Root CreateRoot(string sentence)
        {
            return new Root(sentence, this.Manager);
        }

        /// <summary>
        /// Method for creating new text elements
        /// </summary>
        /// <param name="parts">UDPipe service response line</param>
        /// <returns>New text element</returns>
        public IProcessable Create(string[] parts)
        {
            // Map known cases
            this.MapKnownCases(parts[2], ref parts[3]);

            IProcessable part = null;
            switch (parts[3])
            {
                case "PROPN":
                case "NOUN":
                    part = this.ProcessNoun(parts);
                    break;
                case "ADJ":
                    part = this.ProcessAdj(parts);
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
                case "CONJ":
                    part = new Coordination(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                case "NEG":
                    part = new Negation(int.Parse(parts[0]), parts[2], parts[7]);
                    break;
                default:
                    break;
            }

            return part;
        }

        /// <summary>
        /// Processes verb response line 
        /// </summary>
        /// <param name="parts">response line</param>
        /// <returns>Verb or Auxiliary</returns>
        private IProcessable ProcessVerb(string[] parts)
        {
            // for "be" word lemma return auxiliary
            if (parts[2] == "be")
                return new Auxiliary(int.Parse(parts[0]), parts[2], parts[7]);

            // Assign correct verb form
            VerbForm form = VerbForm.NORMAL;
            if (parts[5].Contains("VerbForm=Part"))
                form = VerbForm.PARTICIPLE;

            if (parts[5].Contains("VerbForm=Ger"))
                form = VerbForm.GERUND;

            return new Verb(int.Parse(parts[0]), parts[2], parts[7], form, parts[1].ToLower());
        }

        /// <summary>
        /// Processes noun response line
        /// </summary>
        /// <param name="parts">response line</param>
        /// <returns>Noun or NounSet</returns>
        private IProcessable ProcessNoun(string[] parts)
        {
            var noun = new Noun(int.Parse(parts[0]), parts[2], parts[7], this.EdgeFactory, this, this.Manager, DrawableObjectWidth, DrawableObjectHeight);

            // Check if noun is plural -> create new nounset
            if (parts[5].Contains("Number=Plur") && !this.DependencyTypeHelper.IsNounPhrase(parts[7]))
                return new NounSet(this, this.EdgeFactory, noun, parts[1]);

            return noun;
        }

        /// <summary>
        /// Creates new NounSet from given nouns
        /// </summary>
        /// <param name="left">First noun</param>
        /// <param name="right">Second noun</param>
        /// <param name="graph">Graph</param>
        /// <returns>New NounSet</returns>
        public IProcessable Create(Noun left, Noun right, ISentenceGraph graph)
        {
            return new NounSet(this, this.EdgeFactory, graph, left, right);
        }

        /// <summary>
        /// Processes adjective response line
        /// </summary>
        /// <param name="parts">Response line</param>
        /// <returns>New Adjective or FunctionalAdjective</returns>
        private IProcessable ProcessAdj(string[] parts)
        {
            // Check upscales adjectives
            if (this.DefaultUpScales.Contains(parts[2].ToLower()))
                return new FunctionalAdjective(int.Parse(parts[0]), parts[1].ToLower(), parts[7], UP_SCALE);

            if (this.LargeUpScales.Contains(parts[2].ToLower()))
                return new FunctionalAdjective(int.Parse(parts[0]), parts[1].ToLower(), parts[7], 2 * UP_SCALE);

            // Check downscales adjectives
            if (this.DefaultDownScales.Contains(parts[2].ToLower()))
                return new FunctionalAdjective(int.Parse(parts[0]), parts[1].ToLower(), parts[7], DOWN_SCALE);

            if (this.TinyDownScales.Contains(parts[2].ToLower()))
                return new FunctionalAdjective(int.Parse(parts[0]), parts[1].ToLower(), parts[7], DOWN_SCALE / 2);

            // return default adjective
            return new Adjective(int.Parse(parts[0]), parts[1].ToLower(), parts[7]);
        }

        /// <summary>
        /// Maps know word cases to different element type
        /// </summary>
        /// <param name="lemma">word lemma</param>
        /// <param name="type">words part of speech</param>
        private void MapKnownCases(string lemma, ref string type)
        {
            string lemmaToFind = lemma.ToLower();

            // Check know cases
            if ((type == "NOUN" || type == "ADV" || type == "PART") && this.KnownCasesToMap.Contains(lemmaToFind))
                type = "ADP";

            // Check adjective
            if (lemmaToFind == ADJECTIVE_NEGATION)
                type = "ADJ";

            // Map negations
            if (this.Negations.Contains(lemmaToFind))
                type = "NEG";
        }
    }
}
