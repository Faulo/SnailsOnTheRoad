using NUnit.Framework;
using NUnit.Framework.Internal;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteFile))]
    sealed class AsepriteFileTagTests {
        void SetUp(TestAsset asset) {
            sut = new TestInfo().GetAsset<AsepriteFile>(asset);
            Assert.IsTrue(sut);
        }

        AsepriteFile sut;

        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "TEST_Aseprite_PortraitsWithTags_0.aseprite", true)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "TEST_Aseprite_PortraitsWithTags_1.aseprite", true)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "TEST_Aseprite_PortraitsWithTags_NotExisting.aseprite", false)]
        public void TestGetSpriteByFileName(TestAsset asset, string name, bool expectedResult) {
            SetUp(asset);

            bool actualResult = sut.TryGetSpriteByFileName(name, out var actualSprite);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "TEST_Aseprite_PortraitsWithTags_0.aseprite", 0)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "TEST_Aseprite_PortraitsWithTags_1.aseprite", 1)]
        public void TestGetSpriteByFileName(TestAsset asset, string name, int expectedId) {
            SetUp(asset);

            sut.TryGetSpriteByFileName(name, out var actualSprite);

            sut.TryGetSprite(expectedId, out var expectedSprite);

            Assert.AreEqual(expectedSprite, actualSprite);
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "neutral", true)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "nEuTRAl", true)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "ANGRY", true)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "not_existing", false)]
        public void TestGetSpriteByStringTag(TestAsset asset, string tag, bool expectedResult) {
            SetUp(asset);

            bool actualResult = sut.TryGetSpriteByTag(tag, out var actualSprite);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "neutral", 0)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "nEuTRAl", 0)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "ANGRY", 7)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, "not_existing", -1)]
        public void TestGetSpriteByStringTag(TestAsset asset, string tag, int expectedId) {
            SetUp(asset);

            sut.TryGetSpriteByTag(tag, out var actualSprite);

            sut.TryGetSprite(expectedId, out var expectedSprite);

            Assert.AreEqual(expectedSprite, actualSprite);
        }

        public enum TestEnum {
            Neutral,
            Angry,
            NotExisting
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.Neutral, true)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.Angry, true)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.NotExisting, false)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.NotExisting | TestEnum.Angry, false)]
        public void TestGetSpriteByEnumTag(TestAsset asset, TestEnum tag, bool expectedResult) {
            SetUp(asset);

            bool actualResult = sut.TryGetSpriteByTag(tag, out var actualSprite);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.Neutral, 0)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.Angry, 7)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.NotExisting, -1)]
        [TestCase(TestAsset.PortraitsWithTags_Aseprite, TestEnum.NotExisting | TestEnum.Angry, -1)]
        public void TestGetSpriteByEnumTag(TestAsset asset, TestEnum tag, int expectedId) {
            SetUp(asset);

            sut.TryGetSpriteByTag(tag, out var actualSprite);

            sut.TryGetSprite(expectedId, out var expectedSprite);

            Assert.AreEqual(expectedSprite, actualSprite);
        }
    }
}
