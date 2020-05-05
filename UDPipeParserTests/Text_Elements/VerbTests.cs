using System;
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
    public class VerbTests
    {
        private const int WIDTH = 240;
        private const int HEIGHT = 120;

        private Verb Verb { get; set; }
        private SentenceGraph SentenceGraph { get; set; }

        private IEdgeFactory EdgeFactory { get; }
        private IImageManager ImageManager { get; }
        private ElementFactory ElementFactory { get; }

        public VerbTests()
        {
            this.EdgeFactory = new IEdgeFactoryMock();
            this.ImageManager = new IImageManagerMock();
            this.ElementFactory = new ElementFactory(this.ImageManager, this.EdgeFactory);
        }

        [TestInitialize]
        public void InitNoun()
        {
            this.Verb = new Verb(1, "test", "_", VerbForm.NORMAL, "test");
            this.SentenceGraph = new SentenceGraph();
        }

        [TestMethod]
        public void VerbProcessAdjective()
        {
            Assert.AreEqual(this.Verb.ExtensionsAfter.Count, 0);
            Assert.AreEqual(this.Verb.ExtensionsBefore.Count, 0);

            var adj = new Adjective(0, "adj", "_");
            this.Verb.Process(adj, this.SentenceGraph);

            Assert.AreEqual(this.Verb.ExtensionsAfter.Count, 0);
            Assert.AreEqual(this.Verb.ExtensionsBefore[0], adj);
            Assert.AreEqual(this.Verb.ExtensionsBefore.Count, 1);
        }

        [TestMethod]
        public void VerbProcessAdverb()
        {
            Assert.AreEqual(this.Verb.ExtensionsAfter.Count, 0);
            Assert.AreEqual(this.Verb.ExtensionsBefore.Count, 0);

            var adv = new Adverb(0, "adv", "_");
            this.Verb.Process(adv, this.SentenceGraph);
            Assert.AreEqual(this.Verb.ExtensionsAfter.Count, 0);
            Assert.AreEqual(this.Verb.ExtensionsBefore[0], adv);
            Assert.AreEqual(this.Verb.ExtensionsBefore.Count, 1);

            adv.DependencyType = "compound";
            this.Verb.Process(adv, this.SentenceGraph);
            Assert.AreEqual(this.Verb.PhrasePart, adv);
        }

        [TestMethod]
        public void VerbProcessAdposition()
        {
            Assert.AreEqual(this.Verb.ExtensionsAfter.Count, 0);
            Assert.AreEqual(this.Verb.ExtensionsBefore.Count, 0);

            var adp = new Adposition(0, "adp", "_");
            this.Verb.Process(adp, this.SentenceGraph);
            Assert.AreEqual(this.Verb.DrawableAdposition, adp);

            adp.DependencyType = "compound";
            this.Verb.Process(adp, this.SentenceGraph);
            Assert.AreEqual(this.Verb.PhrasePart, adp);
        }

        [TestMethod]
        public void VerbProcessVerb()
        {
            Assert.AreEqual(this.Verb.RelatedActions.Count, 0);
            var verb = new Verb(2, "verb", "_", VerbForm.NORMAL, "verb");
            this.Verb.Process(verb, this.SentenceGraph);

            Assert.AreEqual(this.Verb.RelatedActions.Count, 1);
            Assert.AreEqual(this.Verb.RelatedActions[0], verb);
        }

        [TestMethod]
        public void VerbProcessNoun()
        {
            var noun = new Noun(0, "noun", "dobj", this.EdgeFactory, this.ElementFactory, this.ImageManager, WIDTH, HEIGHT);
            this.Verb.Process(noun, this.SentenceGraph);
            Assert.AreEqual(this.Verb.Object, noun);
            this.Verb.Object = null;

            noun.DependencyType = "_";
            this.Verb.Process(noun, this.SentenceGraph);
            Assert.AreEqual(this.Verb.DependingDrawables.Count, 1);
            Assert.AreEqual(this.Verb.DependingDrawables[0], noun);
            this.Verb.DependingDrawables.Clear();

            noun.DependencyType = "nsubj";
            this.Verb.Process(noun, this.SentenceGraph);
            Assert.AreEqual(noun.Actions.Count, 1);
            Assert.AreEqual(noun.Actions[0], this.Verb);
        }

        [TestMethod]
        public void VerbProcessNounSet()
        {
            var noun = new Noun(0, "noun", "dobj", this.EdgeFactory, this.ElementFactory, this.ImageManager, WIDTH, HEIGHT);
            var nounSet = new NounSet(this.ElementFactory, this.EdgeFactory, this.SentenceGraph, noun);

            this.Verb.Process(nounSet, this.SentenceGraph);
            Assert.AreEqual(this.Verb.Object, nounSet);
            this.Verb.Object = null;

            nounSet.DependencyType = "_";
            this.Verb.Process(nounSet, this.SentenceGraph);
            Assert.AreEqual(this.Verb.DependingDrawables.Count, 1);
            Assert.AreEqual(this.Verb.DependingDrawables[0], nounSet);
            this.Verb.DependingDrawables.Clear();

            nounSet.DependencyType = "nsubj";
            this.Verb.Process(nounSet, this.SentenceGraph);
            nounSet.Nouns.ForEach(n =>
            {
                Assert.AreEqual(n.Actions.Count, 1);
                Assert.AreEqual(n.Actions[0], this.Verb);
            });
        }

        [TestMethod]
        public void VerbFinalize()
        {
            var noun = new Noun(0, "noun", "_", this.EdgeFactory, this.ElementFactory, this.ImageManager, WIDTH, HEIGHT);

            this.Verb.Process(noun, this.SentenceGraph);
            var elem = this.Verb.FinalizeProcessing(this.SentenceGraph);
            Assert.IsInstanceOfType(elem, typeof(Noun));
            Assert.AreEqual(noun.Actions.Count, 1);
            Assert.AreEqual(noun.Actions[0], this.Verb);
            noun.Actions.Clear();
            this.Verb.DependingDrawables.Clear();

            noun.DependencyType = "dobj";
            this.Verb.Process(noun, this.SentenceGraph);
            elem = this.Verb.FinalizeProcessing(this.SentenceGraph);
            Assert.IsInstanceOfType(elem, typeof(Noun));
            Assert.AreEqual(noun.Actions.Count, 1);
            Assert.AreEqual(noun.Actions[0], this.Verb);
            noun.Actions.Clear();
            this.Verb.Object = null;

            elem = this.Verb.FinalizeProcessing(this.SentenceGraph);
            Assert.IsInstanceOfType(elem, typeof(Verb));
            Assert.AreEqual(elem, this.Verb);
        }
    }
}
