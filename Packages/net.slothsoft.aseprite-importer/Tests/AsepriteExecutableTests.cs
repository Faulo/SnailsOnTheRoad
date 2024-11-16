using System.IO;
using NUnit.Framework;
using UnityEngine;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteExecutable))]
    sealed class AsepriteExecutableTests {
        [Test]
        public void TestFindExecutable() {
            Assert.IsTrue(AsepriteExecutable.TryFindAseprite(out _));
        }

        [TestCase("--version", "Aseprite")]
        public void TestExecuteVersion(string command, string expected) {
            if (!AsepriteExecutable.TryFindAseprite(out var exe)) {
                Assert.Inconclusive();
                return;
            }

            string actual = exe.Execute(command);
            StringAssert.Contains(expected, actual);
        }

        [TestCase("test.file", default, "Assets/Modules/Aseprite/.cache/test.file")]
        [TestCase("folder/test.file", "", "Assets/Modules/Aseprite/.cache/folder/test.file")]
        [TestCase("folder/test.file", "png", "Assets/Modules/Aseprite/.cache/folder/test.file.png")]
        public void TestCache(string file, string extension, string expected) {
            if (!AsepriteExecutable.TryFindAseprite(out var exe)) {
                Assert.Inconclusive();
                return;
            }

            var actual = AsepriteExecutable.CacheFile(file, extension);
            Assert.AreEqual(new FileInfo(expected).FullName, actual.FullName);
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite, 576, 64)]
        [TestCase(TestAsset.PortraitsWithSlices_Aseprite, 192, 192)]
        [TestCase(TestAsset.Icon_Aseprite, 256, 256)]
        public void TestExecuteExportSheet(TestAsset asset, int expectedWidth, int expectedHeight) {
            if (!AsepriteExecutable.TryFindAseprite(out var exe)) {
                Assert.Inconclusive();
                return;
            }

            var assetFile = new TestInfo().GetFile(asset);
            var cacheFile = AsepriteExecutable.CacheFile(assetFile, "png");

            exe.Execute(
                $"--sheet \"{cacheFile.FullName}\"",
                $"\"{assetFile.FullName}\""
            );

            FileAssert.Exists(cacheFile);

            var texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(cacheFile.FullName));
            Assert.AreEqual(new Vector2Int(expectedWidth, expectedHeight), new Vector2Int(texture.width, texture.height));
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite)]
        [TestCase(TestAsset.PortraitsWithSlices_Aseprite)]
        public void TestExecuteExportData(TestAsset asset) {
            if (!AsepriteExecutable.TryFindAseprite(out var exe)) {
                Assert.Inconclusive();
                return;
            }

            var assetFile = new TestInfo().GetFile(asset);
            var cacheFile = AsepriteExecutable.CacheFile(assetFile, "json");

            exe.Execute(
                "--format json-array",
                $"--data \"{cacheFile.FullName}\"",
                $"\"{assetFile.FullName}\""
            );

            FileAssert.Exists(cacheFile);
        }
    }
}
