using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestAsset.NineSlice_Data, TestOf = typeof(AsepriteData))]
    sealed class AsepriteDataNineSliceTests {
        readonly FileInfo assetFile;
        readonly string json;

        public AsepriteDataNineSliceTests(TestAsset asset) {
            var info = new TestInfo();
            assetFile = new TestInfo().GetFile(asset);
            json = File.ReadAllText(assetFile.FullName);
        }

        AsepriteData sut;

        [SetUp]
        public void SetUp() {
            sut = AsepriteData.FromJson(json);
        }

        [Test]
        public void TestAsepriteSimplePasses() {
            Assert.IsNotNull(sut);
        }

        [Test]
        public void TestMetaSliceCenter() {
            Assert.AreEqual(new AsepriteDataRect(1, 16, 14, 16), sut.meta.slices[0].center);
        }

        [Test]
        public void GivenNineSlicedFile_WhenTryGetNineSlice_ThenReturnTrue() {
            Assert.That(sut.meta.slices[0].TryGetNineSlice(out _), Is.True);
        }

        [Test]
        public void GivenNineSlicedFile_WhenTryGetNineSlice_ThenSetBorder() {
            sut.meta.slices[0].TryGetNineSlice(out var border);

            Assert.That(border, Is.EqualTo(new Vector4(1, 16, 1, 16)));
        }
    }
}