using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Extensions.IEnumerable.Tests
{
    [TestClass]
    public class IEnumerableExtensionsTest
    {
        private static readonly Box redBox = new Box
        {
            Color = "red",
            Height = 25,
            Width = 25
        };
        private static readonly Box blackBox = new Box
        {
            Color = "black",
            Height = 25,
            Width = 25
        };
        private static readonly Box whiteBox = new Box
        {
            Color = "white",
            Height = 50,
            Width = 50,
            CotainedBoxes = new List<Box>
            {
                redBox,
                blackBox
            }
        };
        private static readonly Box greenBox = new Box
        {
            Color = "green",
            Height = 50,
            Width = 50
        };
        private static readonly Box yellowBox = new Box
        {
            Color = "yellow",
            Height = 100,
            Width = 100,
            CotainedBoxes = new List<Box>
            {
                greenBox,
                whiteBox
            }
        };


        private static readonly Box brownBox = new Box
        {
            Color = "brown",
            Height = 25,
            Width = 25
        };
        private static readonly Box violetBox = new Box
        {
            Color = "violet",
            Height = 25,
            Width = 25
        };
        private static readonly Box grayBox = new Box
        {
            Color = "gray",
            Height = 50,
            Width = 50,
            CotainedBoxes = new List<Box>
            {
                violetBox,
                brownBox
            }
        };

        public readonly IEnumerable<Box> Boxes = new List<Box>
        {
            yellowBox,
            grayBox
        };

        [TestMethod]
        public void Should_Flatten_Boxes_Hierarchy()
        {
            // Arrange
            var expectedResult = new List<Box>
            {
                redBox,
                blackBox,
                whiteBox,
                greenBox,
                yellowBox,
                brownBox,
                violetBox,
                grayBox
            }.OrderBy(b => b.Color);

            // Act
            var result = Boxes.SelectManyRecursive(b => b.CotainedBoxes).OrderBy(b => b.Color);

            // Assert
            Assert.IsTrue(result.SequenceEqual(expectedResult));
        }

        [TestMethod]
        public void Should_Find_RedBox_Among_The_Hierarchy()
        {
            // Arrange and Act
            var foundBox = Boxes.FirstOrDefaultRecursive(b => b.Color == "red", b => b.CotainedBoxes);

            // Assert
            Assert.IsNotNull(foundBox);
            Assert.AreEqual(foundBox.Color, redBox.Color);
            Assert.AreEqual(foundBox.Width, redBox.Width);
            Assert.AreEqual(foundBox.Height, redBox.Height);
            Assert.AreEqual(foundBox.CotainedBoxes.Count, redBox.CotainedBoxes.Count);
        }

        [TestMethod]
        public void Should_Correctly_Create_Groups()
        {
            // Arrange
            var redBox = new Box
            {
                Color = "red",
                Height = 25,
                Width = 25
            };

            var blackBox = new Box
            {
                Color = "black",
                Height = 25,
                Width = 50
            };

            var whiteBox = new Box
            {
                Color = "white",
                Height = 50,
                Width = 50
            };
            var boxes = new List<Box>
            {
                redBox,
                blackBox,
                whiteBox
            };

            // Act
            var groupedBoxes = boxes.GroupBy(new string[] { nameof(Box.Height), nameof(Box.Width) });

            // Assert
            Assert.IsNotNull(groupedBoxes);
            Assert.AreEqual(groupedBoxes.Count(), 2);
            Assert.AreEqual(groupedBoxes.FirstOrDefault().NestedGroups.Count, 2);
            Assert.IsTrue(groupedBoxes.FirstOrDefault().NestedGroups.FirstOrDefault().Items.Contains(redBox));
            Assert.IsTrue(groupedBoxes.FirstOrDefault().NestedGroups.LastOrDefault().Items.Contains(blackBox));
            Assert.IsTrue(groupedBoxes.LastOrDefault().NestedGroups.FirstOrDefault().Items.Contains(whiteBox));
        }
    }

    public class Box
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Color { get; set; }

        public List<Box> CotainedBoxes { get; set; }

        public Box()
        {
            CotainedBoxes = new List<Box>();
        }
    }
}
