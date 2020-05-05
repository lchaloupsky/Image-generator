using ImageGeneratorInterfaces.Graph.DrawableElement;
using ImagePositioner.Helpers;
using ImagePositionerTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Numerics;

namespace ImagePositionerTests
{
    [TestClass]
    public class HelperTests
    {
        private const int OBJECT_WIDTH = 240;
        private const int OBJECT_HEIGHT = 120;

        private const int WIDTH = 1280;
        private const int HEIGHT = 720;

        private ConflictHelper ConflictHelper { get; }
        private PositionHelper PositionHelper { get; }

        public HelperTests()
        {
            this.ConflictHelper = new ConflictHelper();
            this.PositionHelper = new PositionHelper() { Width = WIDTH, Height = HEIGHT };
        }

        [TestMethod]
        public void ConflictHelperCheckExistingConflicts()
        {
            var position = new Vector2(OBJECT_WIDTH, OBJECT_HEIGHT);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };

            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));
        }

        [TestMethod]
        public void ConflictHelperCheckNonExistingConflicts()
        {
            var position = new Vector2(OBJECT_WIDTH, OBJECT_HEIGHT);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = 2 * position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };

            Assert.IsFalse(this.ConflictHelper.CheckConflict(left, right));
        }

        [TestMethod]
        public void ConflictHelperResolveCornerExistingConflict()
        {
            var position = new Vector2(0, 0);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));

            this.ConflictHelper.ResolveCornerConflict(left, right, WIDTH, HEIGHT);
            Assert.IsFalse(this.ConflictHelper.CheckConflict(left, right));
        }

        [TestMethod]
        public void ConflictHelperResolveHorizontalExistingConflict()
        {
            var position = new Vector2(0, 0);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));

            this.ConflictHelper.ResolveHorizontalConflict(left, right);
            Assert.IsFalse(this.ConflictHelper.CheckConflict(left, right));
        }

        [TestMethod]
        public void ConflictHelperResolveVerticalExistingConflict()
        {
            var position = new Vector2(0, 0);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));

            this.ConflictHelper.ResolveVerticalConflict(left, right);
            Assert.IsFalse(this.ConflictHelper.CheckConflict(left, right));
        }

        [TestMethod]
        public void ConflictHelperGetExistingConflicts()
        {
            var position = new Vector2(0, 0);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var third = new IDrawableMock() { Position = position + new Vector2(OBJECT_WIDTH, OBJECT_HEIGHT), Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));
            Assert.IsFalse(this.ConflictHelper.CheckConflict(left, third));

            var result = this.ConflictHelper.GetConflictingVertices(left, new List<IDrawable> { right, third });
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
            Assert.AreEqual(result[0], right);
        }

        [TestMethod]
        public void ConflictHelperGetShift()
        {
            var position = new Vector2(0, 0);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));

            var result = this.ConflictHelper.GetShift(left, right);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.X >= OBJECT_WIDTH);
            Assert.IsTrue(result.Y >= 0);
        }

        [TestMethod]
        public void ConflictHelperGetOverlapX()
        {
            var position = new Vector2(0, 0);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));

            var result = this.ConflictHelper.GetOverlapX(left, right);
            Assert.IsTrue(result.X >= OBJECT_WIDTH);
        }

        [TestMethod]
        public void ConflictHelperGetOverlapY()
        {
            var position = new Vector2(0, 0);
            var left = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = position, Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            Assert.IsTrue(this.ConflictHelper.CheckConflict(left, right));

            var result = this.ConflictHelper.GetOverlapY(left, right);
            Assert.IsTrue(result.Y >= OBJECT_HEIGHT);
        }

        [TestMethod]
        public void PositionHelperGetEmptyPosition()
        {
            var left = new IDrawableMock() { Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };

            var leftPos = this.PositionHelper.GetEmptyPosition(left);
            var rightPos = this.PositionHelper.GetEmptyPosition(right);
            left.Position = leftPos;
            right.Position = rightPos;

            Assert.IsNotNull(leftPos);
            Assert.IsNotNull(rightPos);
            Assert.AreNotEqual(leftPos, rightPos);
            Assert.IsTrue(leftPos.Value.X + left.Width <= rightPos.Value.X);
            Assert.IsFalse(this.ConflictHelper.CheckConflict(left, right));
        }

        [TestMethod]
        public void PositionHelperGetPaddingX()
        {
            var left = new IDrawableMock() { Position = new Vector2(0, 0), Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = new Vector2(100, 0), Width = OBJECT_WIDTH / 3, Height = OBJECT_HEIGHT / 3 };

            var result = this.PositionHelper.GetPaddingX(left, right, WIDTH);
            Assert.IsTrue(result != 0);
            Assert.AreEqual(((right.Position.Value.X + right.Width) / 2 + left.Position.Value.X + result), WIDTH / 2);
        }

        [TestMethod]
        public void PositionHelperGetPaddingY()
        {
            var left = new IDrawableMock() { Position = new Vector2(0, 0), Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT };
            var right = new IDrawableMock() { Position = new Vector2(0, 100), Width = OBJECT_WIDTH / 3, Height = OBJECT_HEIGHT / 3 };

            var result = this.PositionHelper.GetPaddingY(left, right, HEIGHT);
            Assert.IsTrue(result != 0);
            Assert.AreEqual(((right.Position.Value.Y + right.Height) / 2 + left.Position.Value.Y + result), HEIGHT / 2);
        }

        [TestMethod]
        public void PositionHelperCenterVertices()
        {
            var list = new List<IDrawable>() {
                new IDrawableMock() { Position = new Vector2(-100, 0), Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT },
                new IDrawableMock() { Position = new Vector2(0, 1000), Width = OBJECT_WIDTH, Height = OBJECT_HEIGHT }
            };

            this.PositionHelper.CenterVertices(list, WIDTH, HEIGHT);

            // Check if they are all inside the canvas
            IDrawable leftMost = list[0];
            IDrawable topMost = list[0];
            IDrawable bottomMost = list[0];
            IDrawable rightMost = list[0];
            list.ForEach(vertex =>
            {
                int vX = (int)vertex.Position.Value.X;
                int vY = (int)vertex.Position.Value.Y;

                leftMost = leftMost.Position.Value.X < vX ? leftMost : vertex;
                rightMost = rightMost.Position.Value.X + rightMost.Width > vX + vertex.Width ? rightMost : vertex;
                topMost = topMost.Position.Value.Y < vY ? topMost : vertex;
                bottomMost = bottomMost.Position.Value.Y + bottomMost.Height > vY + vertex.Height ? bottomMost : vertex;

                Assert.IsTrue(vertex.Position.Value.X > 0);
                Assert.IsTrue(vertex.Position.Value.X + vertex.Width <= WIDTH);
                Assert.IsTrue(vertex.Position.Value.Y > 0);
                Assert.IsTrue(vertex.Position.Value.Y + vertex.Height <= HEIGHT);
            });

            Assert.AreEqual((int)(leftMost.Position.Value.X + (rightMost.Position.Value.X + rightMost.Width - leftMost.Position.Value.X) / 2), WIDTH / 2);
            Assert.AreEqual((int)(topMost.Position.Value.Y + (bottomMost.Position.Value.Y + bottomMost.Height - topMost.Position.Value.Y) / 2), HEIGHT / 2);
        }
    }
}
