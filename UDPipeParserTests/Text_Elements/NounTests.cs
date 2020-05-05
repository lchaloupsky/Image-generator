using System;
using System.Drawing;
using System.Linq;
using ImageGeneratorInterfaces.Edges.Factory;
using ImageGeneratorInterfaces.ImageManager;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UDPipeParserTests.Mocks;
using UDPipeParsing;
using UDPipeParsing.Factories;
using UDPipeParsing.Text_elements;

namespace UDPipeParserTests.Text_Elements
{
    [TestClass]
    public class NounTests
    {
        private const int WIDTH = 240;
        private const int HEIGHT = 120;

        private Noun Noun { get; set; }
        private SentenceGraph SentenceGraph { get; set; }

        private IEdgeFactory EdgeFactory { get; }
        private IImageManager ImageManager { get; }
        private ElementFactory ElementFactory { get; }

        public NounTests()
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
        }

        [TestMethod]
        public void NounProcessAdjective()
        {
            Assert.AreEqual(this.Noun.Extensions.Count, 0);

            var adj = new Adjective(2, "adj", "_");
            this.Noun.Process(adj, this.SentenceGraph);

            Assert.AreEqual(this.Noun.Extensions.Count, 1);
            Assert.AreEqual(this.Noun.Extensions[0], adj);
        }

        [TestMethod]
        public void NounProcessFunctionalAdjective()
        {
            Assert.AreEqual(this.Noun.Extensions.Count, 0);

            var adj = new FunctionalAdjective(2, "adj", "_", 0.5f);
            this.Noun.Process(adj, this.SentenceGraph);

            Assert.AreEqual(this.Noun.Extensions.Count, 1);
            Assert.AreEqual(this.Noun.Extensions[0], adj);
            Assert.AreEqual(this.Noun.Scale, 0.5f);
        }

        [TestMethod]
        public void NounProcessAdverb()
        {
            Assert.AreEqual(this.Noun.Extensions.Count, 0);

            var adv = new Adverb(2, "adv", "_");
            this.Noun.Process(adv, this.SentenceGraph);

            Assert.AreEqual(this.Noun.Extensions.Count, 1);
            Assert.AreEqual(this.Noun.Extensions[0], adv);
        }

        [TestMethod]
        public void NounProcessAdposition()
        {
            Assert.AreEqual(this.Noun.Adpositions.Count, 0);

            var adp = new Adposition(2, "adp", "_");
            this.Noun.Process(adp, this.SentenceGraph);
            Assert.AreEqual(this.Noun.Adpositions.Count, 1);
            Assert.AreEqual(this.Noun.Adpositions[0], adp);

            this.Noun.Adpositions.Clear();
            adp.Process(new Adposition(3, "adp1", "_"), this.SentenceGraph);
            this.Noun.Process(adp, this.SentenceGraph);
            Assert.AreEqual(this.Noun.Adpositions.SelectMany(a => a.GetAdpositions()).Count(), 2);
            Assert.AreEqual(this.Noun.Adpositions[0], adp);
        }

        [TestMethod]
        public void NounProcessNumeral()
        {
            var num = new Numeral(2, "3", "nummod");
            var elem = this.Noun.Process(num, this.SentenceGraph);
            Assert.IsInstanceOfType(elem, typeof(NounSet));

            this.Noun.DependencyType = "nmod:npmod";
            this.Noun.Process(num, this.SentenceGraph);
            Assert.IsTrue(this.Noun.Extensions[0] == num);
        }

        [TestMethod]
        public void NounProcessVerb()
        {
            Assert.AreEqual(this.Noun.Actions.Count, 0);
            var verb = new Verb(2, "verb", "_", VerbForm.NORMAL, "verb");
            this.Noun.Process(verb, this.SentenceGraph);

            Assert.AreEqual(this.Noun.Actions.Count, 1);
            Assert.AreEqual(this.Noun.Actions[0], verb);
        }

        [TestMethod]
        public void NounProcessNoun()
        {
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 0);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 0);
            var noun = new Noun(2, "noun2", "_", this.EdgeFactory, this.ElementFactory, this.ImageManager, WIDTH, HEIGHT);
            this.Noun.Process(noun, this.SentenceGraph);

            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], this.Noun);
            Assert.AreEqual(this.SentenceGraph[this.Noun].ToList()[0].Right, noun);

            this.SentenceGraph = new SentenceGraph();
            noun.DependencyType = "nsubj";
            noun.Image = null;
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 0);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 0);
            this.Noun.Process(noun, this.SentenceGraph);

            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], noun);
            Assert.AreEqual(this.SentenceGraph[noun].ToList()[0].Right, this.Noun);

            this.SentenceGraph = new SentenceGraph();
            noun.DependencyType = "conj";
            noun.Image = null;
            var elem = this.Noun.Process(noun, this.SentenceGraph);
            Assert.IsInstanceOfType(elem, typeof(Noun));
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 2);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], this.Noun);
            Assert.AreEqual(this.SentenceGraph[this.Noun].ToList()[0].Right, noun);

            this.SentenceGraph = new SentenceGraph();
            noun.DependencyType = "compound";
            noun.Image = null;
            this.Noun.Process(noun, this.SentenceGraph);
            Assert.AreEqual(this.Noun.Extensions[0], noun);
            Assert.AreEqual(this.Noun.Extensions.Count, 1);
        }

        [TestMethod]
        public void NounProcessNounSet()
        {
            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 0);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 0);
            var noun = new Noun(2, "noun2", "_", this.EdgeFactory, this.ElementFactory, this.ImageManager, WIDTH, HEIGHT);
            var nounSet = new NounSet(this.ElementFactory, this.EdgeFactory, this.SentenceGraph, noun);
            this.Noun.Process(nounSet, this.SentenceGraph);

            Assert.AreEqual(this.SentenceGraph.Vertices.Count(), 5);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 6);
            Assert.AreEqual(this.SentenceGraph.Vertices.ToList()[0], nounSet);
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
        public void NounFinalize()
        {
            Assert.AreEqual(this.Noun.Image, null);

            this.Noun.FinalizeProcessing(this.SentenceGraph);
            Assert.IsTrue(this.SentenceGraph.Vertices.Contains(this.Noun));
            Assert.AreEqual(this.SentenceGraph[this.Noun].Count(), 1);
            Assert.AreEqual(this.SentenceGraph.Edges.Count(), 1);

            Assert.IsInstanceOfType(this.Noun.Image, typeof(Image));
            Assert.IsNotNull(this.Noun.Image);
        }
    }
}
