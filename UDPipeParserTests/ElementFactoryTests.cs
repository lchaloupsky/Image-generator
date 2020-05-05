using System;
using ImageGeneratorInterfaces.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UDPipeParserTests.Mocks;
using UDPipeParsing;
using UDPipeParsing.Factories;
using UDPipeParsing.Text_elements;

namespace UDPipeParserTests
{
    [TestClass]
    public class ElementFactoryTests
    {
        private const int WIDTH = 220;
        private const int HEIGHT = 220;
        private const string PROPER_NOUN = "PROPN";
        private const string NOUN = "NOUN";
        private const string ADJECTIVE = "ADJ";
        private const string ADVERB = "ADV";
        private const string ADPOSITION = "ADP";
        private const string VERB = "VERB";
        private const string NEGATION = "NEG";
        private const string NUMERAL = "NUM";
        private const string COORDINATION = "CONJ";
        private const string PARTICLE = "PART";
        private const int DEFAULT_ID = 1;
        private const string DEFAULT_WORD = "test";

        private ElementFactory ElementFactory { get; }

        public ElementFactoryTests()
        {
            this.ElementFactory = new ElementFactory(new IImageManagerMock(), new IEdgeFactoryMock(), WIDTH, HEIGHT);
        }

        [TestMethod]
        public void CreateNounTest()
        {
            Noun noun;
            var nounParts = this.CreateUDPipeResponseLine(NOUN);
            var element = this.ElementFactory.Create(nounParts);
            AssertProcessableBasic(element, typeof(Noun));
            noun = (Noun)element;
            Assert.AreEqual(noun.Lemma, DEFAULT_WORD);
            Assert.AreEqual(noun.Width, WIDTH);
            Assert.AreEqual(noun.Height, HEIGHT);
            Assert.AreEqual(noun.IsPlural, false);

            AddParams(nounParts, "Number=Sing");
            element = this.ElementFactory.Create(nounParts);
            AssertProcessableBasic(element, typeof(Noun));
            noun = (Noun)element;
            Assert.AreEqual(noun.Lemma, DEFAULT_WORD);
            Assert.AreEqual(noun.Width, WIDTH);
            Assert.AreEqual(noun.Height, HEIGHT);
            Assert.IsFalse(false);
            Assert.IsFalse(noun.IsPositioned);

            nounParts = this.CreateUDPipeResponseLine(PROPER_NOUN);
            element = this.ElementFactory.Create(nounParts);
            AssertProcessableBasic(element, typeof(Noun));
            noun = (Noun)element;
            Assert.AreEqual(noun.Lemma, DEFAULT_WORD);
            Assert.AreEqual(noun.Width, WIDTH);
            Assert.AreEqual(noun.Height, HEIGHT);
            Assert.AreEqual(noun.IsPlural, false);
        }

        [TestMethod]
        public void CreateNounSetTest()
        {
            NounSet nounSet;
            var nounSetParts = this.CreateUDPipeResponseLine(NOUN);
            AddParams(nounSetParts, "Number=Plur");
            var element = this.ElementFactory.Create(nounSetParts);
            AssertProcessableBasic(element, typeof(NounSet));
            nounSet = (NounSet)element;
            Assert.IsNotNull(nounSet.Nouns);
            Assert.IsTrue(nounSet.Nouns.Count > 0);

            var nounParts = this.CreateUDPipeResponseLine(NOUN);
            nounSet = (NounSet)this.ElementFactory.Create((Noun)this.ElementFactory.Create(nounParts), (Noun)this.ElementFactory.Create(nounParts), new SentenceGraph());
            Assert.IsNotNull(nounSet.Nouns);
            Assert.AreEqual(nounSet.Nouns.Count, 2);
        }

        [TestMethod]
        public void CreateAdjectiveTest()
        {
            var adjectiveParts = this.CreateUDPipeResponseLine(ADJECTIVE);
            var element = this.ElementFactory.Create(adjectiveParts);
            AssertProcessableBasic(element, typeof(Adjective));

            ChangeWord(adjectiveParts, "big");
            element = this.ElementFactory.Create(adjectiveParts);
            AssertProcessableBasic(element, typeof(FunctionalAdjective));

            ChangeWord(adjectiveParts, "small");
            element = this.ElementFactory.Create(adjectiveParts);
            AssertProcessableBasic(element, typeof(FunctionalAdjective));
        }

        [TestMethod]
        public void CreateAdverbTest()
        {
            var adjectiveParts = this.CreateUDPipeResponseLine(ADVERB);
            var element = this.ElementFactory.Create(adjectiveParts);
            AssertProcessableBasic(element, typeof(Adverb));
        }

        [TestMethod]
        public void CreateNumeralTest()
        {
            var numeralParts = this.CreateUDPipeResponseLine(NUMERAL);
            var element = this.ElementFactory.Create(numeralParts);
            AssertProcessableBasic(element, typeof(Numeral));
        }

        [TestMethod]
        public void CreateVerbTest()
        {
            Verb verb;
            var verbParts = this.CreateUDPipeResponseLine(VERB);
            var element = this.ElementFactory.Create(verbParts);
            verb = (Verb)element;
            AssertProcessableBasic(element, typeof(Verb));
            Assert.AreEqual(verb.Form, VerbForm.NORMAL);
            Assert.AreEqual(verb.Word, DEFAULT_WORD);

            AddParams(verbParts, "VerbForm=Part");
            element = this.ElementFactory.Create(verbParts);
            verb = (Verb)element;
            AssertProcessableBasic(element, typeof(Verb));
            Assert.AreEqual(verb.Form, VerbForm.PARTICIPLE);
            Assert.AreEqual(verb.Word, DEFAULT_WORD);

            AddParams(verbParts, "VerbForm=Ger");
            element = this.ElementFactory.Create(verbParts);
            verb = (Verb)element;
            AssertProcessableBasic(element, typeof(Verb));
            Assert.AreEqual(verb.Form, VerbForm.GERUND);
            Assert.AreEqual(verb.Word, DEFAULT_WORD);
        }

        [TestMethod]
        public void CreateAuxiliaryTest()
        {
            var verbParts = this.CreateUDPipeResponseLine(VERB, "be");
            var element = this.ElementFactory.Create(verbParts);
            AssertProcessableBasic(element, typeof(Auxiliary));
        }

        [TestMethod]
        public void CreateAdpositionTest()
        {
            var adpParts = this.CreateUDPipeResponseLine(ADPOSITION);
            var element = this.ElementFactory.Create(adpParts);
            AssertProcessableBasic(element, typeof(Adposition));

            adpParts = this.CreateUDPipeResponseLine(NOUN, "top");
            element = this.ElementFactory.Create(adpParts);
            AssertProcessableBasic(element, typeof(Adposition));
            Assert.AreEqual(((Element)element).Lemma, "top");
        }

        [TestMethod]
        public void CreateNegationTest()
        {
            var negParts = this.CreateUDPipeResponseLine(NEGATION);
            var element = this.ElementFactory.Create(negParts);
            AssertProcessableBasic(element, typeof(Negation));

            negParts = this.CreateUDPipeResponseLine(PARTICLE);
            ChangeWord(negParts, "not");
            element = this.ElementFactory.Create(negParts);
            AssertProcessableBasic(element, typeof(Negation));
        }

        [TestMethod]
        public void CreateCoordinationTest()
        {
            var conjParts = this.CreateUDPipeResponseLine(COORDINATION, "and");
            var element = this.ElementFactory.Create(conjParts);
            AssertProcessableBasic(element, typeof(Coordination));
        }

        [TestMethod]
        public void CreateRootTest()
        {
            var element = this.ElementFactory.CreateRoot("sentence");
            AssertProcessableBasic(element, typeof(Root), 0, "root");
            Assert.AreEqual(element.Lemma, "sentence");
        }

        [TestMethod]
        public void CreateUnsupportedTest()
        {
            var unsupParts = this.CreateUDPipeResponseLine("BLA");
            var element = this.ElementFactory.Create(unsupParts);
            Assert.IsNull(element);
        }

        private void AssertProcessableBasic(IProcessable processable, Type t, int id = DEFAULT_ID, string dependency = "_", bool isNegated = false)
        {
            Assert.IsNotNull(processable);
            Assert.IsInstanceOfType(processable, t);
            Assert.AreEqual(processable.Id, id);
            Assert.AreEqual(processable.DependencyType, dependency);
            Assert.AreEqual(processable.IsNegated, isNegated);
        }

        private void ChangeDependency(string[] UDPipeLine, string dependency)
        {
            UDPipeLine[7] = dependency;
        }

        private void ChangeWord(string[] UDPipeLine, string word)
        {
            UDPipeLine[1] = word;
            UDPipeLine[2] = word;
        }

        private void AddParams(string[] UDPipeLine, string param)
        {
            UDPipeLine[5] = param;
        }

        private string[] CreateUDPipeResponseLine(string elementType, string word = "test", int id = DEFAULT_ID)
        {
            return new string[] { id.ToString(), word, word, elementType, "_", "_", "0", "_", "_", "_" };
        }
    }
}
