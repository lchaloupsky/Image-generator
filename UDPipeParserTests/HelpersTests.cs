using System;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UDPipeParserTests.Mocks;
using UDPipeParsing.Text_elements.Helpers;

namespace UDPipeParserTests
{
    [TestClass]
    public class HelpersTests
    {
        private const int WIDTH = 240;
        private const int HEIGHT = 120;

        private DrawableHelper DrawableHelper { get; }
        private VerbFormHelper VerbFormHelper { get; }

        public HelpersTests()
        {
            this.DrawableHelper = new DrawableHelper();
            this.VerbFormHelper = new VerbFormHelper();
        }

        [TestMethod]
        public void VerbFormTest()
        {
            string verb, verbGerundForm;

            verb = "pee";
            verbGerundForm = "peeing";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "dye";
            verbGerundForm = "dyeing";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "draw";
            verbGerundForm = "drawing";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "hit";
            verbGerundForm = "hitting";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "stop";
            verbGerundForm = "stopping";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "die";
            verbGerundForm = "dying";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "mix";
            verbGerundForm = "mixing";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "come";
            verbGerundForm = "coming";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "play";
            verbGerundForm = "playing";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));

            verb = "hover";
            verbGerundForm = "hovering";
            Assert.AreEqual(verbGerundForm, VerbFormHelper.CreatePastParticipleTense(verb));
        }

        [TestMethod]
        public void DrawableHelperCombineTest()
        {
            var left = new IDrawableMock() { Width = WIDTH, Height = HEIGHT, Position = new Vector2(0, 0) };
            var right = new IDrawableMock() { Width = WIDTH, Height = HEIGHT, Position = new Vector2(WIDTH, 0) };

            this.DrawableHelper.CombineIntoGroup(left, right);
            Assert.AreEqual(left.Group, right.Group);
            Assert.AreEqual(left.Group.Width, 2 * WIDTH);
            Assert.AreEqual(left.Group.Height, HEIGHT);
            Assert.AreEqual(left.Group.Position, left.Position);

            left.Group = null;
            right.Group = null;
            right.Position = new Vector2(0, HEIGHT);
            this.DrawableHelper.CombineIntoGroup(left, right);
            Assert.AreEqual(left.Group, right.Group);
            Assert.AreEqual(left.Group.Width, WIDTH);
            Assert.AreEqual(left.Group.Height, 2 * HEIGHT);
            Assert.AreEqual(left.Group.Position, left.Position);

            left.Group = null;
            left.Position = new Vector2(WIDTH, 0);
            this.DrawableHelper.CombineIntoGroup(left, right);
            Assert.AreEqual(left.Group, right.Group);
            Assert.AreEqual(left.Group.Width, 2 * WIDTH);
            Assert.AreEqual(left.Group.Height, 2 * HEIGHT);
            Assert.AreEqual(left.Group.Position, new Vector2(0, 0));

            right.Group = null;
            right.Position = new Vector2(WIDTH, 0);
            this.DrawableHelper.CombineIntoGroup(left, right);
            Assert.AreEqual(left.Group, right.Group);
            Assert.AreEqual(left.Group.Width, 2 * WIDTH);
            Assert.AreEqual(left.Group.Height, 2 * HEIGHT);
            Assert.AreEqual(left.Group.Position, new Vector2(0, 0));
        }
    }
}
