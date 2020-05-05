using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.ImageManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Linq;
using UDPipeParserTests.Mocks;
using UDPipeParsing;
using UDPipeParsing.Factories;
using UDPipeParsing.Text_elements;

namespace UDPipeParserTests.Text_Elements
{
    [TestClass]
    public class NounSetTests
    {
        private const int WIDTH = 240;
        private const int HEIGHT = 120;

        private Noun Noun { get; set; }
        private NounSet NounSet { get; set; }
        private SentenceGraph SentenceGraph { get; set; }

        private IEdgeFactory EdgeFactory { get; }
        private IImageManager ImageManager { get; }
        private ElementFactory ElementFactory { get; }

        public NounSetTests()
        {
            this.EdgeFactory = new IEdgeFactoryMock();
            this.ImageManager = new IImageManagerMock();
            this.ElementFactory = new ElementFactory(this.ImageManager, this.EdgeFactory);
        }

        [TestInitialize]
        public void InitNoun()
        {
            this.Noun = new Noun(1, "test", "_", EdgeFactory, ElementFactory, ImageManager, WIDTH, HEIGHT);
            this.SentenceGraph = new SentenceGraph();
            this.NounSet = new NounSet(ElementFactory, EdgeFactory, this.SentenceGraph, this.Noun);
        }

        [TestMethod]
        public void NounSetProcessAdjective()
        {
            this.NounSet.Nouns.ForEach(n => Assert.AreEqual(this.Noun.Extensions.Count, 0));

            var adj = new Adjective(2, "adj", "_");
            this.NounSet.Process(adj, this.SentenceGraph);

            this.NounSet.Nouns.ForEach(n =>
            {
                Assert.AreEqual(n.Extensions.Count, 1);
                Assert.AreEqual(n.Extensions[0], adj);
            });
        }

        [TestMethod]
        public void NounSetProcessFunctionalAdjective()
        {
            this.NounSet.Nouns.ForEach(n => Assert.AreEqual(this.Noun.Extensions.Count, 0));

            var adj = new FunctionalAdjective(2, "adj", "_", 0.5f);
            this.NounSet.Process(adj, this.SentenceGraph);

            this.NounSet.Nouns.ForEach(n =>
            {
                Assert.AreEqual(n.Extensions.Count, 1);
                Assert.AreEqual(n.Extensions[0], adj);
                Assert.AreEqual(n.Scale, 0.5f);
            });
        }

        [TestMethod]
        public void NounSetProcessAdverb()
        {
            this.NounSet.Nouns.ForEach(n => Assert.AreEqual(this.Noun.Extensions.Count, 0));

            var adv = new Adverb(2, "adv", "_");
            this.NounSet.Process(adv, this.SentenceGraph);

            this.NounSet.Nouns.ForEach(n =>
            {
                Assert.AreEqual(n.Extensions.Count, 1);
                Assert.AreEqual(n.Extensions[0], adv);
            });
        }

        [TestMethod]
        public void NounSetProcessAdposition()
        {
            this.NounSet.Nouns.ForEach(n => Assert.AreEqual(this.Noun.Adpositions.Count, 0));

            var adp = new Adposition(2, "adp", "_");
            this.NounSet.Process(adp, this.SentenceGraph);
            Assert.AreEqual(this.NounSet.Adpositions.Count, 1);
            Assert.AreEqual(this.NounSet.Adpositions[0], adp);

            this.NounSet.Adpositions.Clear();
            adp.Process(new Adposition(3, "adp1", "_"), this.SentenceGraph);
            this.NounSet.Process(adp, this.SentenceGraph);
            Assert.AreEqual(this.NounSet.Adpositions.SelectMany(a => a.GetAdpositions()).Count(), 2);
            Assert.AreEqual(this.NounSet.Adpositions[0], adp);
        }

        [TestMethod]
        public void NounSetProcessNumeral()
        {
            var num = new Numeral(2, "5", "nummod");
            this.NounSet.Process(num, this.SentenceGraph);
            this.NounSet.FinalizeProcessing(this.SentenceGraph);
            Assert.AreEqual(this.NounSet.Nouns.Count(), 5);

            num.DependencyType = "nmod:npmod";
            this.NounSet.Process(num, this.SentenceGraph);
            this.NounSet.Nouns.ForEach(n =>
            {
                Assert.IsTrue(n.Suffixes[0] == num);
            });
        }

        [TestMethod]
        public void NounSetProcessVerb()
        {
            this.NounSet.Nouns.ForEach(n => Assert.AreEqual(this.Noun.Actions.Count, 0));
            var verb = new Verb(2, "verb", "_", VerbForm.NORMAL, "verb");
            this.NounSet.Process(verb, this.SentenceGraph);

            this.NounSet.Nouns.ForEach(n =>
            {
                Assert.AreEqual(n.Actions.Count, 1);
                Assert.AreEqual(n.Actions[0], verb);
            });
        }

        [TestMethod]
        public void NounSetProcessNoun()
        {
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 1);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 1);
            var noun = new Noun(2, "noun2", "_", this.EdgeFactory, this.ElementFactory, this.ImageManager, WIDTH, HEIGHT);
            this.NounSet.Process(noun, this.SentenceGraph);

            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 3);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], this.NounSet);
            Assert.AreEqual(this.SentenceGraph[this.NounSet].Where(e => e.Right == noun).ToList()[0].Right, noun);

            this.SentenceGraph = new SentenceGraph();
            noun.DependencyType = "nsubj";
            noun.Image = null;
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 0);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 0);
            this.NounSet.Process(noun, this.SentenceGraph);

            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], noun);
            Assert.AreEqual(this.SentenceGraph[noun].ToList()[0].Right, this.NounSet);

            this.SentenceGraph = new SentenceGraph();
            noun.DependencyType = "conj";
            noun.Image = null;
            var elem = this.NounSet.Process(noun, this.SentenceGraph);
            Assert.IsInstanceOfType(elem, typeof(NounSet));
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], this.NounSet);
            Assert.AreEqual(this.SentenceGraph[this.NounSet].Where(e => e.Right == noun).ToList()[0].Right, noun);

            this.SentenceGraph = new SentenceGraph();
            noun.DependencyType = "compound";
            noun.Image = null;
            this.NounSet.Process(noun, this.SentenceGraph);
            this.NounSet.Nouns.ForEach(n =>
            {
                Assert.AreEqual(n.Extensions[0], noun);
                Assert.AreEqual(n.Extensions.Count, 1);
            });

        }

        [TestMethod]
        public void NounSetProcessNounSet()
        {
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 1);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 1);
            var noun = new Noun(2, "noun2", "_", this.EdgeFactory, this.ElementFactory, this.ImageManager, WIDTH, HEIGHT);
            var nounSet = new NounSet(this.ElementFactory, this.EdgeFactory, this.SentenceGraph, noun);
            this.Noun.Process(nounSet, this.SentenceGraph);

            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 6);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 7);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[1], nounSet);
            Assert.AreEqual(this.SentenceGraph[this.Noun].ToList()[0].Right, nounSet);

            this.SentenceGraph = new SentenceGraph();
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 0);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 0);
            nounSet = new NounSet(this.ElementFactory, this.EdgeFactory, this.SentenceGraph, noun)
            {
                DependencyType = "nsubj"
            };
            noun.Image = null;

            this.Noun.Process(nounSet, this.SentenceGraph);
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 5);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 6);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], nounSet);
            Assert.AreEqual(this.SentenceGraph[nounSet].ToList().Find(e => e.Right == this.Noun).Right, this.Noun);
        }

        [TestMethod]
        public void NounSetFinalize()
        {
            this.NounSet.FinalizeProcessing(this.SentenceGraph);

            Assert.IsTrue(this.SentenceGraph.Vertices.Contains(this.NounSet));
            Assert.IsTrue(this.SentenceGraph.Vertices.Count() == this.NounSet.Nouns.Count() + 1);
            Assert.AreEqual(this.SentenceGraph[this.NounSet].Count(), 5);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 5);

            Assert.IsInstanceOfType(this.NounSet.Image, typeof(Image));
            Assert.IsNotNull(this.NounSet.Image);
            this.NounSet.Nouns.ForEach(n =>
            {
                Assert.IsNotNull(n.Image);
                Assert.AreNotEqual(n.Image, this.NounSet.Image);
            });

        }
    }
}
