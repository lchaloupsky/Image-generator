using ImageGeneratorInterfaces.Edges;
using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImagePositioner.Edges;
using ImagePositionerTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Numerics;

namespace ImagePositionerTests
{
    [TestClass]
    public class EdgeTests
    {
        private const int WIDTH = 1280;
        private const int HEIGHT = 720;

        private const int DRAWABLE_WIDTH = 240;
        private const int DRAWABLE_HEIGHT = 120;

        IDrawable Left { get; set; }
        IDrawable Right { get; set; }

        [TestInitialize]
        public void InitializeDrawables()
        {
            Left = new IDrawableMock
            {
                Width = DRAWABLE_WIDTH,
                Height = DRAWABLE_HEIGHT
            };

            Right = new IDrawableMock
            {
                Width = DRAWABLE_WIDTH,
                Height = DRAWABLE_HEIGHT
            };
        }

        [TestMethod]
        public void PositionateAbsoluteOnTopEdge()
        {
            var edge = new OnTopEdge();
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.Y, 0);
        }

        [TestMethod]
        public void PositionateAbsoluteAtBottomEdge()
        {
            var edge = new AtBottomEdge();
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, HEIGHT);
        }

        [TestMethod]
        public void PositionateAbsoluteToLeftEdge()
        {
            var edge = new ToLeftEdge();
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X, 0);
        }

        [TestMethod]
        public void PositionateAbsoluteToRightEdge()
        {
            var edge = new ToRightEdge();
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, WIDTH);
        }

        [TestMethod]
        public void PositionateAbsoluteInMiddleEdge()
        {
            var edge = new InMiddleEdge();
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, WIDTH / 2);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height / 2, HEIGHT / 2);
        }

        [TestMethod]
        public void PositionateAbsoluteInCornerEdge()
        {
            var edge = new InCornerEdge();
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);

            edge = new InCornerEdge(VerticalPlace.TOP);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.Y, 0);

            edge = new InCornerEdge(VerticalPlace.BOTTOM);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, HEIGHT);

            edge = new InCornerEdge(HorizontalPlace.LEFT);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X, 0);

            edge = new InCornerEdge(HorizontalPlace.RIGHT);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, WIDTH);

            edge = new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X, 0);
            Assert.AreEqual(edge.Left.Position.Value.Y, 0);

            edge = new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, WIDTH);
            Assert.AreEqual(edge.Left.Position.Value.Y, 0);

            edge = new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X, 0);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, HEIGHT);

            edge = new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Left.Position);
            Assert.AreEqual(edge.Left, this.Left);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, WIDTH);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, HEIGHT);
        }

        [TestMethod]
        public void PositionateRightRelativeOnTopEdge()
        {
            var leftPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var rightPosition = leftPosition + new Vector2(0, this.Left.Height);

            var edge = new OnTopEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = leftPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateLeftRelativeOnTopEdge()
        {
            var rightPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var leftPosition = rightPosition - new Vector2(0, this.Left.Height);

            var edge = new OnTopEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = rightPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateRightRelativeOnEdge()
        {
            var leftPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var rightPosition = leftPosition + new Vector2(0, this.Left.Height);

            var edge = new OnEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = leftPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateLeftRelativeOnEdge()
        {
            var rightPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var leftPosition = rightPosition - new Vector2(0, this.Left.Height);

            var edge = new OnEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = rightPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateRightRelativeUnderEdge()
        {
            var leftPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var rightPosition = leftPosition - new Vector2(0, this.Right.Height);

            var edge = new UnderEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = leftPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateLeftRelativeUnderEdge()
        {
            var rightPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var leftPosition = rightPosition + new Vector2(0, this.Right.Height);

            var edge = new UnderEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = rightPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateRightRelativeToLeftEdge()
        {
            var leftPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var rightPosition = leftPosition + new Vector2(this.Left.Width, 0);

            var edge = new ToLeftEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = leftPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateLeftRelativeToLeftEdge()
        {
            var rightPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var leftPosition = rightPosition - new Vector2(this.Left.Width, 0);

            var edge = new ToLeftEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = rightPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateRightRelativeToRightEdge()
        {
            var leftPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var rightPosition = leftPosition - new Vector2(this.Left.Width, 0);

            var edge = new ToRightEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = leftPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateLeftRelativeToRightEdge()
        {
            var rightPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var leftPosition = rightPosition + new Vector2(this.Left.Width, 0);

            var edge = new ToRightEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = rightPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
        }

        [TestMethod]
        public void PositionateRightRelativeInFrontEdge()
        {
            var leftPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var rightPosition = leftPosition - new Vector2(this.Left.Width / 2, -this.Left.Height / 2);

            var edge = new InFrontEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = leftPosition;

            AssertEdgeBasic(edge, rightPosition, leftPosition);
            AssertEdgeVerticesEqualDimensions(edge);
            Assert.AreNotEqual(edge.Left.ZIndex, edge.Right.ZIndex);
            Assert.IsTrue(edge.Left.ZIndex > edge.Right.ZIndex);
        }

        [TestMethod]
        public void PositionateLeftRelativeInFrontEdge()
        {
            var rightPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var leftPosition = rightPosition - new Vector2(this.Right.Width / 2, -this.Right.Height / 2);

            var edge = new InFrontEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = rightPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
            Assert.AreNotEqual(edge.Left.ZIndex, edge.Right.ZIndex);
            Assert.IsTrue(edge.Left.ZIndex > edge.Right.ZIndex);
        }

        [TestMethod]
        public void PositionateRightRelativeBehindEdge()
        {
            var leftPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var rightPosition = leftPosition + new Vector2(this.Left.Width / 2, -this.Left.Height / 2);

            var edge = new BehindEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = leftPosition;

            AssertEdgeBasic(edge, rightPosition, leftPosition);
            AssertEdgeVerticesEqualDimensions(edge);
            Assert.AreNotEqual(edge.Left.ZIndex, edge.Right.ZIndex);
            Assert.IsTrue(edge.Left.ZIndex < edge.Right.ZIndex);
        }

        [TestMethod]
        public void PositionateLeftRelativeBehindEdge()
        {
            var rightPosition = new Vector2(WIDTH / 2, HEIGHT / 2);
            var leftPosition = rightPosition + new Vector2(this.Right.Width / 2, -this.Right.Height / 2);

            var edge = new BehindEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = rightPosition;

            AssertEdgeBasic(edge, leftPosition, rightPosition);
            AssertEdgeVerticesEqualDimensions(edge);
            Assert.AreNotEqual(edge.Left.ZIndex, edge.Right.ZIndex);
            Assert.IsTrue(edge.Left.ZIndex < edge.Right.ZIndex);
        }

        [TestMethod]
        public void PositionateRightRelativeInEdge()
        {
            var edge = new InEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
        }

        [TestMethod]
        public void PositionateLeftRelativeInEdge()
        {
            var edge = new InEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
        }

        [TestMethod]
        public void PositionateRightRelativeAtEdge()
        {
            var edge = new AtEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
        }

        [TestMethod]
        public void PositionateLeftRelativeAtEdge()
        {
            var edge = new AtEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
        }

        [TestMethod]
        public void PositionateRightRelativeAtBottomEdge()
        {
            var edge = new AtBottomEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Right.Position.Value.X + edge.Right.Height, edge.Left.Position.Value.X + edge.Left.Height);
        }

        [TestMethod]
        public void PositionateLeftRelativeAtBottomEdge()
        {
            var edge = new AtBottomEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Right.Position.Value.X + edge.Right.Height, edge.Left.Position.Value.X + edge.Left.Height);
        }

        [TestMethod]
        public void PositionateRightRelativeInMiddleEdge()
        {
            var edge = new InMiddleEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width / 2);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height / 2, edge.Right.Position.Value.Y + edge.Right.Height / 2);
        }

        [TestMethod]
        public void PositionateLeftRelativeInMiddleEdge()
        {
            var edge = new InMiddleEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);

            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width / 2);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height / 2, edge.Right.Position.Value.Y + edge.Right.Height / 2);
        }

        [TestMethod]
        public void PositionateRightRelativeInCornerEdge()
        {
            var edge = new InCornerEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);

            edge = new InCornerEdge(VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.Y, edge.Right.Position.Value.Y);

            edge = new InCornerEdge(VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new InCornerEdge(HorizontalPlace.LEFT);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X, edge.Right.Position.Value.X);

            edge = new InCornerEdge(HorizontalPlace.RIGHT);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, edge.Right.Position.Value.X + edge.Right.Width);

            edge = new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y, edge.Right.Position.Value.Y);

            edge = new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y, edge.Right.Position.Value.Y);

            edge = new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, edge.Right.Position.Value.Y + edge.Right.Height);
        }

        [TestMethod]
        public void PositionateLeftRelativeInCornerEdge()
        {
            var edge = new InCornerEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);

            edge = new InCornerEdge(VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.Y, edge.Right.Position.Value.Y);

            edge = new InCornerEdge(VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new InCornerEdge(HorizontalPlace.LEFT);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X, edge.Right.Position.Value.X);

            edge = new InCornerEdge(HorizontalPlace.RIGHT);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, edge.Right.Position.Value.X + edge.Right.Width);

            edge = new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y, edge.Right.Position.Value.Y);

            edge = new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y, edge.Right.Position.Value.Y);

            edge = new InCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new InCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge);
            Assert.AreEqual(edge.Left.Position.Value.X, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height, edge.Right.Position.Value.Y + edge.Right.Height);
        }

        [TestMethod]
        public void PositionateRightRelativeOnCornerEdge()
        {
            var edge = new OnCornerEdge();
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);

            edge = new OnCornerEdge(VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height / 2, edge.Right.Position.Value.Y);

            edge = new OnCornerEdge(VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height - (edge.Left.Height / 2), edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new OnCornerEdge(HorizontalPlace.LEFT);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X);

            edge = new OnCornerEdge(HorizontalPlace.RIGHT);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width - edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width);

            edge = new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width - edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y + (edge.Left.Height / 2), edge.Right.Position.Value.Y);

            edge = new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y + (edge.Left.Height / 2), edge.Right.Position.Value.Y);

            edge = new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width - edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height - (edge.Left.Height / 2), edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Left.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height - (edge.Left.Height / 2), edge.Right.Position.Value.Y + edge.Right.Height);
        }

        [TestMethod]
        public void PositionateLeftRelativeOnCornerEdge()
        {
            var edge = new OnCornerEdge();
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);

            edge = new OnCornerEdge(VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height / 2, edge.Right.Position.Value.Y);

            edge = new OnCornerEdge(VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height - (edge.Left.Height / 2), edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new OnCornerEdge(HorizontalPlace.LEFT);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X);

            edge = new OnCornerEdge(HorizontalPlace.RIGHT);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width - edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width);

            edge = new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width - edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y + (edge.Left.Height / 2), edge.Right.Position.Value.Y);

            edge = new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.TOP);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y + (edge.Left.Height / 2), edge.Right.Position.Value.Y);

            edge = new OnCornerEdge(HorizontalPlace.RIGHT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width - edge.Left.Width / 2, edge.Right.Position.Value.X + edge.Right.Width);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height - (edge.Left.Height / 2), edge.Right.Position.Value.Y + edge.Right.Height);

            edge = new OnCornerEdge(HorizontalPlace.LEFT, VerticalPlace.BOTTOM);
            edge.Add(this.Left, this.Right);
            this.Right.Position = new Vector2(WIDTH / 2, HEIGHT / 2);
            AssertEdgeInside(edge, 1);
            Assert.AreEqual(edge.Left.Position.Value.X + edge.Left.Width / 2, edge.Right.Position.Value.X);
            Assert.AreEqual(edge.Left.Position.Value.Y + edge.Left.Height - (edge.Left.Height / 2), edge.Right.Position.Value.Y + edge.Right.Height);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PositionateRelativeEdgeWithNoRight()
        {
            var edge = new OnEdge();
            edge.Add(this.Left, null);
            edge.Positionate(WIDTH, HEIGHT);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PositionateRelativeEdgeWithNoLeft()
        {
            var edge = new OnEdge();
            edge.Add(null, this.Right);
            edge.Positionate(WIDTH, HEIGHT);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PositionateAbsoluteEdgeWithNoLeft()
        {
            var edge = new OnTopEdge();
            edge.Add(null, null);
            edge.Positionate(WIDTH, HEIGHT);
        }

        private void AssertEdgeInside(IPositionateEdge edge, int divideFactor = 2)
        {
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Right.Position);
            Assert.IsNotNull(edge.Left.Position);
            Assert.IsTrue(edge.Right.IsPositioned);
            Assert.IsTrue(edge.Left.IsPositioned);
            Assert.IsTrue(edge.Left.ZIndex > edge.Right.ZIndex);
            Assert.IsTrue(edge.Left.Width <= edge.Right.Width / divideFactor);
            Assert.IsTrue(edge.Left.Height <= edge.Right.Height / divideFactor);
        }

        private void AssertEdgeBasic(IPositionateEdge edge, Vector2 leftPosition, Vector2 rightPosition)
        {
            edge.Positionate(WIDTH, HEIGHT);

            Assert.IsNotNull(edge.Right.Position);
            Assert.IsNotNull(edge.Left.Position);
            Assert.IsTrue(edge.Right.IsPositioned);
            Assert.IsTrue(edge.Left.IsPositioned);
            Assert.AreEqual(edge.Left.Position, leftPosition);
            Assert.AreEqual(edge.Right.Position, rightPosition);
        }

        private void AssertEdgeVerticesEqualDimensions(IPositionateEdge edge)
        {
            Assert.AreEqual(edge.Right.Width, edge.Left.Width);
            Assert.AreEqual(edge.Right.Height, edge.Left.Height);
        }
    }
}
