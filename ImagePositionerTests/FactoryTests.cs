using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImagePositioner.Edges;
using ImagePositioner.Factories;
using ImagePositionerTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ImagePositionerTests
{
    [TestClass]
    public class FactoryTests
    {
        IDrawable Left { get; set; }
        IDrawable Right { get; set; }
        EdgeFactory Factory { get; set; }

        [TestInitialize]
        public void InitializeDrawables()
        {
            Left = new IDrawableMock();
            Right = new IDrawableMock();
            Factory = new EdgeFactory();
        }

        [TestMethod]
        public void CreateAbsoluteEdgeOnTop()
        {
            var type = typeof(OnTopEdge);
            var placeType = PlaceType.HORIZONTAL;
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "on", "top" }), placeType, type);
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "at", "top" }), placeType, type);
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "down", "from", "top" }), placeType, type);
        }

        [TestMethod]
        public void CreateAbsoluteEdgeAtBottom()
        {
            var type = typeof(AtBottomEdge);
            var placeType = PlaceType.HORIZONTAL;
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "at", "bottom" }), placeType, type);
        }

        [TestMethod]
        public void CreateAbsoluteEdgeInCorner()
        {
            var type = typeof(InCornerEdge);
            var placeType = PlaceType.CORNER;

            var edge = this.Factory.Create(this.Left, new List<string>() { "in", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "left", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "right", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "top", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "bottom", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "top", "left", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "left", "top", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "bottom", "left", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "left", "bottom", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "top", "right", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "right", "top", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "bottom", "right", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "in", "right", "bottom", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);

            // On corner == in corner

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "left", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "right", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "top", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "bottom", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "top", "left", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "left", "top", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "bottom", "left", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "left", "bottom", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "top", "right", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "right", "top", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "bottom", "right", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, new List<string>() { "on", "right", "bottom", "corner" });
            AssertAbsoluteEdge(edge, placeType, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);
        }

        [TestMethod]
        public void CreateAbsoluteEdgeOnLeft()
        {
            var type = typeof(ToLeftEdge);
            var placeType = PlaceType.VERTICAL;
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "on", "left" }), placeType, type);
        }

        [TestMethod]
        public void CreateAbsoluteEdgeOnRight()
        {
            var type = typeof(ToRightEdge);
            var placeType = PlaceType.VERTICAL;
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "on", "right" }), placeType, type);
        }

        [TestMethod]
        public void CreateAbsoluteEdgeInMiddle()
        {
            var type = typeof(InMiddleEdge);
            var placeType = PlaceType.MIDDLE;
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "in", "middle" }), placeType, type);
            AssertAbsoluteEdge(this.Factory.Create(this.Left, new List<string>() { "in", "midst" }), placeType, type);
        }

        [TestMethod]
        public void CreateNotSupportedAbsoluteEdge()
        {
            var edge = this.Factory.Create(this.Left, new List<string>() { "random", "string" });

            Assert.IsNull(edge);
        }

        [TestMethod]
        public void CreateNotSupportedRelativeEdge()
        {
            var edge = this.Factory.Create(this.Left, this.Right, new List<string>() { "random", "string" }, new List<string> { "more", "random" }, false);

            Assert.IsNull(edge);
        }

        [TestMethod]
        public void CreateRelativeEdgeInMiddle()
        {
            var type = typeof(InMiddleEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "in", "middle" }, new List<string>() { "of" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "in", "midst" }, new List<string>() { "of" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "between" }, new List<string>(), true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "across" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeOnEdge()
        {
            var type = typeof(OnEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "above" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "upon" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "on" }, new List<string>(), true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "up" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeInEdge()
        {
            var type = typeof(InEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "inside" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "in" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "within" }, new List<string>(), true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "into" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeAtEdge()
        {
            var type = typeof(AtEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "at" }, false), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeUnderEdge()
        {
            var type = typeof(UnderEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "under" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "below" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "beneath" }, new List<string>(), true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "down" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeUnderEdgeReversed()
        {
            var type = typeof(UnderEdge);
            UnderEdge underEdge = null;

            var edge = this.Factory.Create(this.Left, this.Right, new List<string>() { "at" }, new List<string>() { "below" }, false);
            AssertRelativeEdge(edge, type);
            underEdge = (UnderEdge)edge;
            Assert.IsTrue(underEdge.IsReversed);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>() { "in", "below" }, new List<string>(), true);
            AssertRelativeEdge(edge, type);
            underEdge = (UnderEdge)edge;
            Assert.IsTrue(underEdge.IsReversed);
        }

        [TestMethod]
        public void CreateRelativeEdgeOnTopEdge()
        {
            var type = typeof(OnTopEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "top", "of" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "from", "top", "of" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "at", "top", "of" }, new List<string>(), true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeToLeftEdge()
        {
            var type = typeof(ToLeftEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "to", "left", "of" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "after" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "from" }, new List<string>(), true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "against" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeToRightEdge()
        {
            var type = typeof(ToRightEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "to", "right", "of" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "near" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "for" }, new List<string>(), true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "along" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeAtBottomEdge()
        {
            var type = typeof(AtBottomEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "at", "bottom", "of" }, false), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeInFrontEdge()
        {
            var type = typeof(InFrontEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "front", "of" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "outside" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "outside" }, new List<string>() { "of" }, true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "around" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeInFrontEdgeReversed()
        {
            var type = typeof(InFrontEdge);
            InFrontEdge inFrontEdge = null;

            var edge = this.Factory.Create(this.Left, this.Right, new List<string>() { "in" }, new List<string>() { "front" }, false);
            AssertRelativeEdge(edge, type);
            inFrontEdge = (InFrontEdge)edge;
            Assert.IsTrue(inFrontEdge.IsReversed);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>() { "with", "in", "front" }, new List<string>(), true);
            AssertRelativeEdge(edge, type);
            inFrontEdge = (InFrontEdge)edge;
            Assert.IsTrue(inFrontEdge.IsReversed);
        }

        [TestMethod]
        public void CreateRelativeEdgeBehindEdge()
        {
            var type = typeof(BehindEdge);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "opposite", "of" }, false), type);
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "behind" }, new List<string>(), false), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "beyond" }, new List<string>(), true), type);
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "past" }, true), type);
        }

        [TestMethod]
        public void CreateRelativeEdgeBehindEdgeReversed()
        {
            var type = typeof(BehindEdge);
            BehindEdge inFrontEdge = null;

            var edge = this.Factory.Create(this.Left, this.Right, new List<string>() { "from" }, new List<string>() { "behind" }, false);
            AssertRelativeEdge(edge, type);
            inFrontEdge = (BehindEdge)edge;
            Assert.IsTrue(inFrontEdge.IsReversed);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>() { "in", "behind" }, new List<string>(), true);
            AssertRelativeEdge(edge, type);
            inFrontEdge = (BehindEdge)edge;
            Assert.IsTrue(inFrontEdge.IsReversed);
        }

        [TestMethod]
        public void CreateRelativeEdgeInCornerEdge()
        {
            var type = typeof(InCornerEdge);

            var edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "in", "left", "corner", "of" }, true);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "right", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "top", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "bottom", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "in", "top", "left", "corner", "of" }, true);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "left", "top", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "bottom", "left", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "left", "bottom", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "top", "right", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "in", "right", "top", "corner", "of" }, true);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "bottom", "right", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "in", "right", "bottom", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);
        }

        [TestMethod]
        public void CreateRelativeEdgeOnCornerEdge()
        {
            var type = typeof(OnCornerEdge);

            var edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "on", "left", "corner", "of" }, true);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "right", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "top", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "bottom", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "on", "top", "left", "corner", "of" }, true);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "left", "top", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "bottom", "left", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "left", "bottom", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.LEFT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "top", "right", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>() { "on", "right", "top", "corner", "of" }, true);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.TOP, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "bottom", "right", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);

            edge = this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>() { "on", "right", "bottom", "corner", "of" }, false);
            AssertRelativeEdge(edge, type);
            AssertCornerEdge(edge, VerticalPlace.BOTTOM, HorizontalPlace.RIGHT);
        }

        [TestMethod]
        public void CreateRelativeEdgeFromNonExistingParts()
        {
            // Left is not subject
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "on" }, new List<string>() { "in", "front", "of" }, false), typeof(InFrontEdge));
            AssertRelativeEdge(this.Factory.Create(this.Left, this.Right, new List<string>() { "outside" }, new List<string>() { "on" }, false), typeof(OnEdge));

            // Left is subject
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "under" }, new List<string>() { "of" }, true), typeof(UnderEdge));
            AssertRelativeEdge(this.Factory.Create(this.Right, this.Left, new List<string>() { "in left corner of" }, new List<string>() { "around" }, true), typeof(InCornerEdge));
        }

        [TestMethod]
        public void CreateRelativeEdgeFromEmpty()
        {
            Assert.AreEqual(this.Factory.Create(this.Left, this.Right, new List<string>(), new List<string>(), false), null);
            Assert.AreEqual(this.Factory.Create(this.Right, this.Left, new List<string>(), new List<string>(), true), null);
        }

        [TestMethod]
        public void CreateAbsoluteEdgeFromEmpty()
        {
            Assert.AreEqual(this.Factory.Create(this.Left, new List<string>()), null);
        }

        private void AssertAbsoluteEdge(IAbsolutePositionateEdge edge, PlaceType placeType, Type type)
        {
            Assert.IsNotNull(edge);
            Assert.IsInstanceOfType(edge, typeof(AbsoluteEdge));
            Assert.IsInstanceOfType(edge, type);
            Assert.AreEqual(edge.Type, placeType);
            Assert.IsNotNull(edge.Left);
            Assert.AreEqual(edge.Left, this.Left);
        }

        private void AssertRelativeEdge(IPositionateEdge edge, Type type)
        {
            Assert.IsNotNull(edge);
            Assert.IsInstanceOfType(edge, typeof(Edge));
            Assert.IsInstanceOfType(edge, type);
            Assert.IsNotNull(edge.Left);
            Assert.IsNotNull(edge.Right);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Right, this.Right);
        }

        private void AssertCornerEdge(IPositionateEdge edge, VerticalPlace vertical)
        {
            VerticalPlace edgeVertical;
            if (edge is InCornerEdge cornerEdge)
                edgeVertical = cornerEdge.Vertical;
            else
            {
                OnCornerEdge onCornerEdge = (OnCornerEdge)edge;
                edgeVertical = onCornerEdge.Vertical;
            }

            Assert.AreEqual(edgeVertical, vertical);
        }

        private void AssertCornerEdge(IPositionateEdge edge, HorizontalPlace horizontal)
        {
            HorizontalPlace edgeHorizontal;
            if (edge is InCornerEdge cornerEdge)
                edgeHorizontal = cornerEdge.Horizontal;
            else
            {
                OnCornerEdge onCornerEdge = (OnCornerEdge)edge;
                edgeHorizontal = onCornerEdge.Horizontal;
            }

            Assert.AreEqual(edgeHorizontal, horizontal);
        }

        private void AssertCornerEdge(IPositionateEdge edge, VerticalPlace vertical, HorizontalPlace horizontal)
        {
            VerticalPlace edgeVertical;
            HorizontalPlace edgeHorizontal;
            if (edge is InCornerEdge cornerEdge)
            {
                edgeVertical = cornerEdge.Vertical;
                edgeHorizontal = cornerEdge.Horizontal;
            }
            else
            {
                OnCornerEdge onCornerEdge = (OnCornerEdge)edge;
                edgeVertical = onCornerEdge.Vertical;
                edgeHorizontal = onCornerEdge.Horizontal;
            }

            Assert.AreEqual(edgeHorizontal, horizontal);
            Assert.AreEqual(edgeVertical, vertical);
        }
    }
}
