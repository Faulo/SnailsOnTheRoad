using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteFile))]
    sealed class AsepriteFileSliceTests {
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_0.aseprite", false, 1)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_0.aseprite", true, 0)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_Blue_0.aseprite", false, 0)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_Blue_0.aseprite", true, 1)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_Red_0.aseprite", false, 0)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_Red_0.aseprite", true, 1)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_Yellow_0.aseprite", false, 0)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_Yellow_0.aseprite", true, 1)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_NotExisting_0.aseprite", false, 0)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "TEST_Aseprite_SlicedDoor_NotExisting_0.aseprite", true, 0)]
        public void TestImportSlices(TestAsset asset, string slice, bool importSlices, int expectedCount) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = importSlices,
            };
            bool result = AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            Assert.IsTrue(result);

            Assert.AreEqual(expectedCount, sut.indexedSprites.Count(keyval => keyval.sprite.name.Contains(slice)), $"Expected a slice with a name '{slice}'!\n{string.Join("\n", sut.indexedSprites)}");
        }

        [TestCase(TestAsset.SlicedDoor_Aseprite, 0, 0, 0, 16, 48)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, 1, 16, 0, 16, 48)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, 2, 32, 0, 16, 48)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, 0, 0, 128, 64, 64)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, 1, 128, 128, 64, 64)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, 4, 0, 64, 64, 64)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, 8, 0, 0, 64, 64)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, 10, 64, 128, 64, 64)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, 11, 192, 128, 64, 64)]
        [TestCase(TestAsset.Buttons_Aseprite, 0, 0, 32, 16, 16)]
        [TestCase(TestAsset.Buttons_Aseprite, 1, 32, 32, 16, 16)]
        [TestCase(TestAsset.Buttons_Aseprite, 1, 32, 32, 16, 16)]
        [TestCase(TestAsset.Buttons_Aseprite, 5, 304, 16, 16, 16)]
        [TestCase(TestAsset.Buttons_Aseprite, 6, 288, 16, 16, 16)]
        public void TestImportedSliceRects(TestAsset asset, int index, int x, int y, int width, int height) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            Assert.IsTrue(sut.TryGetSprite(index, out var sprite));

            Assert.AreEqual(new Rect(x, y, width, height), sprite.rect);
        }

        [TestCase(TestAsset.SlicedDoor_Aseprite, "Blue", true)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "RED", true)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "yellow", true)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "NotExisting", false)]
        [TestCase(TestAsset.PortraitsWithSlices_Aseprite, "neutral", true)]
        [TestCase(TestAsset.PortraitsWithSlices_Aseprite, "crying", true)]
        public void TestTryGetSpriteBySliceNameResult(TestAsset asset, string slice, bool expected) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            bool actual = sut.TryGetSpriteBySlice(slice, out _);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestTryGetSpriteBySliceRecordResult(bool expected) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(TestAsset.SlicedDoor_Aseprite), out var sut, settings);

            var slice = expected
                ? new AsepriteDataSlice("Blue", "#0000ffff", "", new(0, 0, 16, 48))
                : new AsepriteDataSlice();

            bool actual = sut.TryGetSpriteBySlice(slice, out _);
            Assert.AreEqual(expected, actual);
        }
        [TestCase(TestAsset.SlicedDoor_Aseprite, "Blue", 0)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "RED", 1)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, "yellow", 2)]
        [TestCase(TestAsset.PortraitsWithSlices_Aseprite, "neutral", 0)]
        [TestCase(TestAsset.PortraitsWithSlices_Aseprite, "crying", 8)]
        public void TestTryGetSpriteBySliceNameAsset(TestAsset asset, string slice, int expectedIndex) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            sut.TryGetSpriteBySlice(slice, out var actual);
            Assert.IsTrue(sut.TryGetSprite(expectedIndex, out var expectedSprite));
            Assert.AreEqual(expectedSprite, actual);
        }
        [Test]
        public void TestTryGetSpriteBySliceRecordAsset() {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(TestAsset.SlicedDoor_Aseprite), out var sut, settings);

            var slice = new AsepriteDataSlice("Red", "#0000ffff", "", new(16, 0, 16, 48));

            sut.TryGetSpriteBySlice(slice, out var actual);

            Assert.IsTrue(sut.TryGetSprite(1, out var expectedSprite));
            Assert.AreEqual(expectedSprite, actual);
        }

        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, "default", "neutral", true)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, "default", "NotExisting", false)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, "NotExisting", "neutral", false)]
        public void TestTryGetSpriteBySliceAndTagResult(TestAsset asset, string slice, string tag, bool expected) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            bool actual = sut.TryGetSpriteBySliceAndTag(slice, tag, out _);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, "hatless", "neutral", 0)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, "hatless", "shocked", 1)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, "default", "neutral", 10)]
        [TestCase(TestAsset.PortraitsWithSlicesAndTags_Aseprite, "default", "shocked", 11)]
        public void TestTryGetSpriteBySliceAndTagAsset(TestAsset asset, string slice, string tag, int expectedIndex) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            sut.TryGetSpriteBySliceAndTag(slice, tag, out var actual);
            Assert.IsTrue(sut.TryGetSprite(expectedIndex, out var expectedSprite));
            Assert.AreEqual(expectedSprite, actual);
        }

        [TestCase(TestAsset.NineSlice_Aseprite, 1, 16, 1, 16)]
        public void TestImportNineSlices(TestAsset asset, int left, int bottom, int right, int top) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            bool result = AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            Assert.IsTrue(result);
            Assert.AreEqual(new Vector4(left, bottom, right, top), sut.indexedSprites.ToArray()[0].sprite.border);
        }

        [TestCase(TestAsset.NineSlice_Aseprite, 8, 48)]
        public void TestImportSlicePivot(TestAsset asset, int expectedX, int expectedY) {
            var settings = new AsepriteSettings() {
                importSlicesIfAvailable = true,
            };
            bool result = AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            Assert.IsTrue(result);
            Assert.AreEqual(new Vector2(expectedX, expectedY), sut.indexedSprites.ToArray()[0].sprite.pivot);
        }
    }
}
