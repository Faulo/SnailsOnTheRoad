using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestAsset.PortraitsWithTags_Aseprite, false, TestOf = typeof(AsepriteFile))]
    [TestFixture(TestAsset.Icon_Aseprite, true, TestOf = typeof(AsepriteFile))]
    sealed class AsepriteFileTests {
        readonly AsepriteSettings assetSettings = new() {
            exportInTrueColor = false,
        };
        readonly FileInfo assetFile;
        readonly TestInfo.SheetInfo expectedSheet;
        readonly TestInfo.DataInfo expectedInfo;
        readonly TestInfo.SpriteInfo[] expectedSprites;
        readonly bool hasEmissionTexture;

        TestInfo.SpriteInfo GetExpectedSprite(int id) {
            return id < expectedSprites.Length
                ? expectedSprites[id]
                : default;
        }

        public AsepriteFileTests(TestAsset asset, bool hasEmissionTexture) {
            var info = new TestInfo();
            assetFile = new TestInfo().GetFile(asset);
            expectedSheet = asset;
            expectedInfo = asset;
            expectedSprites = TestInfo.SpriteInfo.GetSpriteInfos(asset);
            this.hasEmissionTexture = hasEmissionTexture;
        }

        AsepriteFile sut;
        TestInfo.SheetInfo actualSheet => sut.albedo;
        TestInfo.DataInfo actualInfo => sut.info;
        TestInfo.SpriteInfo GetActualSprite(int id) {
            return sut.TryGetSprite(id, out var sprite)
                ? sprite
                : default;
        }

        [Test]
        public void TestImportSetsName() {
            AsepriteFile.TryCreateInstance(assetFile, out sut, assetSettings);

            Assert.AreEqual(assetFile.Name[..^assetFile.Extension.Length], sut.name);
        }

        [Test]
        public void TestImportSetsSheet() {
            AsepriteFile.TryCreateInstance(assetFile, out sut, assetSettings);

            Assert.AreEqual(expectedSheet, actualSheet);
        }

        [Test]
        public void TestImportSetsInfo() {
            AsepriteFile.TryCreateInstance(assetFile, out sut, assetSettings);

            Assert.AreEqual(expectedInfo, actualInfo);
        }

        [TestCase(0)]
        [TestCase(1)]
        public void TestGetSpriteById(int id) {
            AsepriteFile.TryCreateInstance(assetFile, out sut, assetSettings);

            Assert.AreEqual(GetExpectedSprite(id), GetActualSprite(id));
        }

        [TestCase(false, "NotExistingEmission", false)]
        [TestCase(false, "Emission", false)]
        [TestCase(true, "NotExistingEmission", false)]
        [TestCase(true, "Emission", true)]
        public void TestExtractEmission(bool extractEmission, string emissionLayer, bool expectedEmission) {
            var settings = new AsepriteSettings() {
                extractEmission = extractEmission,
                emissionLayerName = emissionLayer,
            };
            AsepriteFile.TryCreateInstance(assetFile, out sut, settings);

            if (hasEmissionTexture && expectedEmission) {
                Assert.IsTrue(sut.emission);
            } else {
                Assert.IsNull(sut.emission);
            }
        }

        [TestCase(0, "_EmissionTex")]
        [TestCase(0, "_MainTex")]
        public void TestAssignEmissionToSprite(int id, string emissionTextureName) {
            var settings = new AsepriteSettings() {
                extractEmission = true,
                emissionTextureName = emissionTextureName
            };

            AsepriteFile.TryCreateInstance(assetFile, out sut, settings);

            var textures = new SecondarySpriteTexture[1];
            Assert.IsTrue(sut.TryGetSprite(id, out var sprite));
            int actual = sprite.GetSecondaryTextures(textures);

            Assert.AreEqual(hasEmissionTexture ? 1 : 0, actual);

            if (hasEmissionTexture) {
                Assert.IsNotNull(textures[0]);
                Assert.AreEqual(emissionTextureName, textures[0].name);
                Assert.AreEqual(sut.emission, textures[0].texture);
            }
        }

        [TestCase(0.5f, 0.5f)]
        [TestCase(0, 0)]
        [TestCase(0, 1)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        public void TestAssignPivot(float x, float y) {
            var pivot = new Vector2(x, y);

            var settings = new AsepriteSettings() {
                pivot = pivot
            };

            AsepriteFile.TryCreateInstance(assetFile, out sut, settings);

            foreach (var (_, sprite) in sut.indexedSprites) {
                Assert.AreEqual(pivot * sprite.rect.size, sprite.pivot);
            }
        }

        [TestCase(1f / 16)]
        [TestCase(16)]
        [TestCase(64)]
        public void TestAssignPixelsPerUnit(float pixelsPerUnit) {
            var settings = new AsepriteSettings() {
                pixelsPerUnit = pixelsPerUnit
            };

            AsepriteFile.TryCreateInstance(assetFile, out sut, settings);

            foreach (var (_, sprite) in sut.indexedSprites) {
                Assert.AreEqual(pixelsPerUnit, sprite.pixelsPerUnit);
            }
        }
    }
}
