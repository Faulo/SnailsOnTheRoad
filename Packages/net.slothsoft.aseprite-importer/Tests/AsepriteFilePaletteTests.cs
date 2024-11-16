using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteFile))]
    sealed class AsepriteFilePaletteTests {
        readonly FileInfo assetFile = new TestInfo().GetFile(TestAsset.ColorPalette);

        AsepriteFile sut;

        [Test]
        public void GivenColorPalette_WhenTryCreatePalette_ThenReturnTrue() {
            bool actual = AsepriteFile.TryCreatePalette(assetFile, out _);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void GivenColorPalette_WhenTryCreatePalette_ThenReturnPalette() {
            AsepriteFile.TryCreatePalette(assetFile, out var actual);

            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void GivenColorPalette_WhenTryCreatePalette_ThenSetColor() {
            AsepriteFile.TryCreatePalette(assetFile, out var actual);

            Assert.That(actual[Vector2Int.zero].a, Is.EqualTo(0));
        }

        [TestCase(7 * 9)]
        public void GivenColorPalette_WhenTryCreatePalette_ThenSetColor(int colorCount) {
            AsepriteFile.TryCreatePalette(assetFile, out var actual);

            Assert.That(actual.colorCount, Is.EqualTo(colorCount));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GivenColorPalette_WhenTryCreatePalette_ThenSetFirstColorToWhite(int row) {
            AsepriteFile.TryCreatePalette(assetFile, out var actual);

            ColorUtility.TryParseHtmlString("#e0e0d8", out var white);

            Assert.That(actual[new(0, row)], Is.EqualTo((Color32)white));
        }
    }
}
