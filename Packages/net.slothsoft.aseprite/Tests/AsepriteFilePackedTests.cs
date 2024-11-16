using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteFile))]
    sealed class AsepriteFilePackedTests {
        [TestCase(TestAsset.SlicedDoor_Aseprite, false)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, true)]
        [TestCase(TestAsset.CharacterWithEmission, false)]
        [TestCase(TestAsset.CharacterWithEmission, true)]
        public void T00_GivenAsepriteWithEmission_WhenTryCreateInstance_ThenSucceed(TestAsset asset, bool packSheet) {
            var settings = new AsepriteSettings() {
                packSheet = packSheet,
            };
            bool result = AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            Assert.IsTrue(result);
        }

        [TestCase(TestAsset.SlicedDoor_Aseprite, false)]
        [TestCase(TestAsset.SlicedDoor_Aseprite, true)]
        [TestCase(TestAsset.CharacterWithEmission, false)]
        [TestCase(TestAsset.CharacterWithEmission, true)]
        public void T01_GivenAsepriteWithEmission_WhenTryCreateInstance_ThenEmissionSameSizeAsAlbedo(TestAsset asset, bool packSheet) {
            var settings = new AsepriteSettings() {
                packSheet = packSheet,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            var emission = sut.emission;
            var albedo = sut.albedo;

            Assert.AreEqual(albedo.width, emission.width);
            Assert.AreEqual(albedo.height, emission.height);
        }

        [TestCase(TestAsset.CharacterWithEmission, 0, true)]
        [TestCase(TestAsset.CharacterWithEmission, 11, true)]
        [TestCase(TestAsset.CharacterWithEmission, 12, false)]
        [TestCase(TestAsset.CharacterWithEmission, 15, true)]
        [TestCase(TestAsset.CharacterWithEmission, 18, false)]
        [TestCase(TestAsset.CharacterWithEmission, 54, true)]
        public void T02_GivenAsepriteWithEmission_WhenGivenSpriteWithoutEmission_ThenSecondaryTextureShouldBeTransparent(TestAsset asset, int spriteId, bool shouldBeBlack) {
            var settings = new AsepriteSettings() {
                packSheet = true,
            };
            AsepriteFile.TryCreateInstance(new TestInfo().GetFile(asset), out var sut, settings);

            sut.TryGetSprite(spriteId, out var sprite);
            var secondaryTextures = new SecondarySpriteTexture[1];
            sprite.GetSecondaryTextures(secondaryTextures);
            var pixels = secondaryTextures[0].texture.GetPixels32(sprite.textureRect);

            Assert.AreEqual(shouldBeBlack, pixels.All(IsBlack));
        }

        bool IsBlack(Color32 pixel) {
            return pixel.r == 0
                && pixel.g == 0
                && pixel.b == 0;
        }
    }
}
