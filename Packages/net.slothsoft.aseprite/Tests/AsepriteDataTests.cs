using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(TestAsset.Avatar_Data, TestOf = typeof(AsepriteData))]
    sealed class AsepriteDataTests {
        readonly FileInfo assetFile;
        readonly string json;

        public AsepriteDataTests(TestAsset asset) {
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
        public void T00_GivenAseprite_WhenCallFromJson_ThenNotNull() {
            Assert.IsNotNull(sut);
        }

        [TestCase(33)]
        public void T10_GivenFrames_WhenGetArray_ThenReturnCorrectSize(int frameCount) {
            Assert.IsNotNull(sut.frames);
            Assert.AreEqual(frameCount, sut.frames.Length);
            Assert.IsNotNull(sut.frames[0]);
        }

        [TestCase(0)]
        public void T11_GivenFrames_WhenGetFrame_ThenReturnCorrectData(int frameId) {
            var frame = sut.frames[frameId];

            Assert.AreEqual("S_ENT_Avatar 0.aseprite", frame.filename);
            Assert.AreEqual(0, frame.frame.x);
            Assert.AreEqual(0, frame.frame.y);
            Assert.AreEqual(160, frame.frame.w);
            Assert.AreEqual(32, frame.frame.h);
            Assert.AreEqual(160, frame.sourceSize.w);
            Assert.AreEqual(32, frame.sourceSize.h);
            Assert.AreEqual(3500, frame.duration);
            Assert.AreEqual(false, frame.rotated);
            Assert.AreEqual(false, frame.trimmed);
            Assert.AreEqual(0, frame.spriteSourceSize.x);
            Assert.AreEqual(0, frame.spriteSourceSize.y);
            Assert.AreEqual(160, frame.spriteSourceSize.w);
            Assert.AreEqual(32, frame.spriteSourceSize.h);
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        public void T12_GivenFrames_WhenGetViaIndex_ThenReturnFrame(int id) {
            Assert.AreEqual(sut.frames[id], sut[$"S_ENT_Avatar {id}.aseprite"]);
        }
        [TestCase("I8")]
        public void T20_GivenMeta_WhenGetVersionAndFormat_ThenReturnCorrectData(string format) {
            Assert.IsNotNull(sut.meta);
            Assert.IsNotEmpty(sut.meta.version);
            Assert.AreEqual(format, sut.meta.format);
        }

        [TestCase(960, 160, 1)]
        public void T21_GivenMeta_WhenGetSize_ThenReturnCorrectData(int width, int height, int scale) {
            Assert.IsNotNull(sut.meta.size);
            Assert.AreEqual(width, sut.meta.size.w);
            Assert.AreEqual(height, sut.meta.size.h);
            Assert.AreEqual(scale, sut.meta.scale);
        }

        [TestCase(1)]
        public void T30_GivenMetaLayer_WhenGetArray_ThenReturnCorrectSize(int layerCount) {
            Assert.IsNotNull(sut.meta.layers);
            Assert.AreEqual(layerCount, sut.meta.layers.Length);
            for (int i = 0; i < layerCount; i++) {
                Assert.IsNotNull(sut.meta.layers[i]);
            }
        }

        [TestCase(0, "Flattened", 255, "normal")]
        public void T31_GivenMetaLayer_WhenGet_ThenReturnCorrecData(int index, string name, int opacity, string blendMode) {
            Assert.AreEqual(name, sut.meta.layers[index].name);
            Assert.AreEqual(opacity, sut.meta.layers[index].opacity);
            Assert.AreEqual(blendMode, sut.meta.layers[index].blendMode);
        }

        [TestCase(15)]
        public void T40_GivenMetaTag_WhenGetArray_ThenReturnCorrectSize(int frameCount) {
            Assert.IsNotNull(sut.meta.frameTags);
            Assert.AreEqual(frameCount, sut.meta.frameTags.Length);
            for (int i = 0; i < frameCount; i++) {
                Assert.IsNotNull(sut.meta.frameTags[i]);
            }
        }

        [TestCase(0, "Idle")]
        [TestCase(1, "Run")]
        [TestCase(2, "Walk")]
        public void T41_GivenMetaTag_WhenGetName_ThenReturnCorrecData(int index, string name) {
            Assert.AreEqual(name, sut.meta.frameTags[index].name);
        }

        [TestCase(0, 0, 1)]
        [TestCase(1, 2, 5)]
        [TestCase(2, 6, 7)]
        public void T42_GivenMetaTag_WhenGetFromAndTo_ThenReturnCorrecData(int index, int from, int to) {
            Assert.AreEqual(from, sut.meta.frameTags[index].from);
            Assert.AreEqual(to, sut.meta.frameTags[index].to);
        }

        [TestCase(0, AsepriteDataFrameDirection.forward)]
        [TestCase(1, AsepriteDataFrameDirection.reverse)]
        [TestCase(2, AsepriteDataFrameDirection.pingpong)]
        public void T43_GivenMetaTag_WhenGetDirection_ThenReturnCorrecData(int index, AsepriteDataFrameDirection direction) {
            Assert.AreEqual(direction, sut.meta.frameTags[index].directionValue);
        }

        [TestCase(0, "#fe5b59ff")]
        [TestCase(1, "#6acd5bff")]
        public void T44_GivenMetaTag_WhenGetColor_ThenReturnCorrecData(int index, string color) {
            Assert.IsTrue(ColorUtility.TryParseHtmlString(color, out var expected));
            Assert.AreEqual(expected, sut.meta.frameTags[index].colorValue);
        }

        [TestCase(0, true)]
        [TestCase(3, false)]
        public void T45_GivenMetaTag_WhenGetIsLooping_ThenReturnCorrecData(int index, bool isLooping) {
            Assert.AreEqual(isLooping, sut.meta.frameTags[index].isLooping);
        }

        [TestCase(5)]
        public void T50_GivenMetaSlice_WhenGetArray_ThenReturnCorrectSize(int sliceCount) {
            Assert.IsNotNull(sut.meta.slices);
            Assert.AreEqual(sliceCount, sut.meta.slices.Length);
            for (int i = 0; i < sliceCount; i++) {
                Assert.IsNotNull(sut.meta.slices[i]);
            }
        }

        [TestCase(0, "Up")]
        [TestCase(1, "Down")]
        [TestCase(2, "DownLeft")]
        [TestCase(3, "Left")]
        [TestCase(4, "UpLeft")]
        public void T51_GivenMetaSlice_WhenGetName_ThenReturnCorrectData(int index, string name) {
            Assert.AreEqual(name, sut.meta.slices[index].name);
        }

        [TestCase(0, "up")]
        [TestCase(1, "down")]
        [TestCase(2, "down-left")]
        [TestCase(3, "left")]
        [TestCase(4, "up-left")]
        public void T52_GivenMetaSlice_WhenGetUserData_ThenReturnCorrectData(int index, string data) {
            Assert.AreEqual(data, sut.meta.slices[index].data);
        }

        [TestCase(0, "#d186dfff")]
        [TestCase(1, "#fe5b59ff")]
        public void T53_GivenMetaSlice_WhenGetColor_ThenReturnCorrectData(int index, string color) {
            Assert.IsTrue(ColorUtility.TryParseHtmlString(color, out var expected));
            Assert.AreEqual(expected, sut.meta.slices[index].colorValue);
        }

        [TestCase(0, 0, 0, 32, 32)]
        [TestCase(1, 32, 0, 32, 32)]
        [TestCase(2, 64, 0, 32, 32)]
        [TestCase(3, 96, 0, 32, 32)]
        [TestCase(4, 128, 0, 32, 32)]
        public void T54_GivenMetaSlice_WhenGetBounds_ThenReturnCorrectData(int index, int x, int y, int width, int height) {
            Assert.AreEqual(new AsepriteDataRect(x, y, width, height), sut.meta.slices[index].bounds);
        }

        [TestCase(0, false)]
        public void T54_GivenMetaSlice_WhenGetNineSlice_ThenReturnCorrectData(int index, bool expected) {
            Assert.AreEqual(expected, sut.meta.slices[index].TryGetNineSlice(out _));
        }

        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, true)]
        public void T55_GivenMetaSlice_WhenTryGetPivot_ThenReturnSuccess(int index, bool expected) {
            bool actual = sut.meta.slices[index].TryGetPivot(out _);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(1, 0.5f, 0.5f)]
        [TestCase(2, 0.0f, 1.0f)]
        public void T56_GivenMetaSlice_WhenTryGetPivot_ThenSetPivot(int index, float x, float y) {
            sut.meta.slices[index].TryGetPivot(out var actual);

            Assert.That(actual, Is.EqualTo(new Vector2(x, y)));
        }

        [TestCase(0)]
        public void T56_GivenMetaSlice_WhenGetUnsetSlicePivot_ThenReturnNull(int index) {
            Assert.That(sut.meta.slices[index].pivot, Is.Null);
        }

        [TestCase(1, 16, 16)]
        [TestCase(2, 0, 0)]
        public void T57_GivenMetaSlice_WhenGetSetSlicePivot_ThenReturnData(int index, int x, int y) {
            Assert.That(sut.meta.slices[index].pivot, Is.EqualTo(new AsepriteDataPosition(x, y)));
        }

        [Test]
        public void T60_GivenAnotherParsedAseprite_WhenCompareUsingEquals_ThenIsEqual() {
            Assert.AreEqual(AsepriteData.FromJson(json), sut);
        }

        [Test]
        public void T61_GivenAnotherParsedAseprite_WhenCompareUsingEquality_ThenIsEqual() {
            Assert.IsTrue(AsepriteData.FromJson(json) == sut);
        }

        [Test]
        public void T62_GivenEmptyAseprite_WhenCompareUsingEquals_ThenIsNotEqual() {
            Assert.AreNotEqual(new AsepriteData(), sut);
        }

        [Test]
        public void T63_GivenEmptyAseprite_WhenCompareUsingInequality_ThenIsNotEqual() {
            Assert.IsTrue(new AsepriteData() != sut);
        }

        [Test]
        public void T64_GivenAsepriteHashSet_WhenAddMultiple_ThenOnlyAddUnique() {
            var set = new HashSet<AsepriteData>();
            Assert.IsTrue(set.Add(sut));
            Assert.IsFalse(set.Add(sut));
            Assert.IsFalse(set.Add(AsepriteData.FromJson(json)));

            Assert.IsTrue(set.Add(new AsepriteData()));
            Assert.IsFalse(set.Add(new AsepriteData()));
        }
    }
}