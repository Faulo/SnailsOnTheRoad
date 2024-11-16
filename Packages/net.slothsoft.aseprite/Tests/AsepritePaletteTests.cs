using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepritePalette))]
    sealed class AsepritePaletteTests {

        [TestCase(1)]
        [TestCase(10)]
        public void GivenColors_WhenGetAllColors_ThenReturn(int colorCount) {
            var colors = Enumerable
                .Range(0, colorCount)
                .Select(i => (byte)i)
                .Select(i => new Color32(i, 0, 0, 255))
                .ToArray();

            var sut = new AsepritePalette(colors);

            Assert.That(sut.colors, Is.EqualTo(colors));
        }

        [TestCase(1)]
        [TestCase(10)]
        public void GivenColors_WhenAccessFirstColor_ThenReturnBlack(int colorCount) {
            var colors = Enumerable
                .Range(0, colorCount)
                .Select(i => (byte)i)
                .Select(i => new Color32(i, 0, 0, 255))
                .ToArray();

            var sut = new AsepritePalette(colors);

            Assert.That(sut[Vector2Int.zero], Is.EqualTo(new Color32(0, 0, 0, 255)));
        }

        [TestCase(1)]
        [TestCase(10)]
        public void GivenColors_WhenCompareEquals_ThenExpectSame(int colorCount) {
            var colors = Enumerable
                .Range(0, colorCount)
                .Select(i => (byte)i)
                .Select(i => new Color32(i, 0, 0, 255))
                .ToArray();

            var sut = new AsepritePalette(colors);

            Assert.That(new AsepritePalette(colors), Is.EqualTo(sut));
        }

        [TestCase(1)]
        [TestCase(10)]
        public void GivenColors_WhenSerializeDeserialize_ThenExpectSame(int colorCount) {
            var colors = Enumerable
                .Range(0, colorCount)
                .Select(i => (byte)i)
                .Select(i => new Color32(i, 0, 0, 255))
                .ToArray();

            var sut = new AsepritePalette(colors);

            string json = JsonUtility.ToJson(sut);
            var actual = JsonUtility.FromJson<AsepritePalette>(json);

            Assert.That(actual, Is.EqualTo(sut));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 5, 5)]
        public void GivenColors_WhenTryGetColor32_ThenReturn(int hue, int shade, int expectedColorId) {
            var colors = Enumerable
                .Range(0, 256)
                .Select(i => (byte)i)
                .Select(i => new Color32(i, 0, 0, i))
                .ToArray();
            var expectedColor = colors[expectedColorId];

            var sut = new AsepritePalette(colors);

            Assert.That(sut.TryGetColor32(hue, shade, out var actualColor), Is.True);
            Assert.That(actualColor, Is.EqualTo(expectedColor));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0, 5, 5)]
        public void GivenColors_WhenTryGetHueAndShade_ThenReturn(int expectedHue, int expectedShade, int colorId) {
            var colors = Enumerable
                .Range(0, 256)
                .Select(i => (byte)i)
                .Select(i => new Color32(i, 0, 0, 255))
                .ToArray();
            var color = colors[colorId];

            var sut = new AsepritePalette(colors);

            Assert.That(sut.TryGetHueAndShade(color, out int actualHue, out int actualShade), Is.True);
            Assert.That(actualHue, Is.EqualTo(expectedHue));
            Assert.That(actualShade, Is.EqualTo(expectedShade));
        }
    }
}
