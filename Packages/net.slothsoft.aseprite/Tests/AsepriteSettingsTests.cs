using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(true, true, TestOf = typeof(AsepriteSettings))]
    [TestFixture(true, false, TestOf = typeof(AsepriteSettings))]
    [TestFixture(false, true, TestOf = typeof(AsepriteSettings))]
    [TestFixture(false, false, TestOf = typeof(AsepriteSettings))]
    sealed class AsepriteSettingsTests {

        readonly TestInfo info = new();

        FileInfo asepriteFile;
        FileInfo sheetFile;
        FileInfo dataFile;

        public AsepriteSettingsTests(bool initializeSheet, bool initializeData) {
            asepriteFile = info.GetFile(TestAsset.Icon_Aseprite);

            if (initializeSheet) {
                sheetFile = info.GetFile(TestAsset.Icon_Sheet);
            }

            if (initializeData) {
                dataFile = info.GetFile(TestAsset.Icon_Data);
            }
        }

        [TestCase(TestAsset.PortraitsWithTags_Aseprite)]
        [TestCase(TestAsset.SpiderWeb_Aseprite)]
        public void TestCreateArgumentsAseprite(int asepriteAsset) {
            var sut = new AsepriteSettings();

            var asepriteFile = info.GetFile((TestAsset)asepriteAsset);

            string expected = $"\"{asepriteFile.FullName}\"";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            CollectionAssert.Contains(actual, expected);
        }

        [TestCase(-1)]
        [TestCase(TestAsset.PortraitsWithTags_Sheet)]
        [TestCase(TestAsset.SpiderWeb_Sheet)]
        public void TestCreateArgumentsSheet(int sheetAsset) {
            var sut = new AsepriteSettings();

            bool isMissing = sheetAsset == -1;
            var sheetFile = isMissing
                ? default
                : info.GetFile((TestAsset)sheetAsset);

            string expected = $"--sheet \"{sheetFile?.FullName}\"";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (isMissing) {
                CollectionAssert.DoesNotContain(actual, expected);
            } else {
                CollectionAssert.Contains(actual, expected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreateArgumentsPackSheet(bool packSheet) {
            var sut = new AsepriteSettings() {
                packSheet = packSheet
            };

            string expected = "--sheet-pack";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (packSheet) {
                CollectionAssert.Contains(actual, expected);
            } else {
                CollectionAssert.DoesNotContain(actual, expected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreateArgumentsIncludeHiddenLayers(bool includeHiddenLayers) {
            var sut = new AsepriteSettings() {
                includeHiddenLayers = includeHiddenLayers
            };

            string expected = "--all-layers";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (includeHiddenLayers) {
                CollectionAssert.Contains(actual, expected);
            } else {
                CollectionAssert.DoesNotContain(actual, expected);
            }
        }

        [TestCase("")]
        [TestCase("Albedo")]
        [TestCase("Emission")]
        public void TestCreateArgumentsFilterByLayer(string layer) {
            var sut = new AsepriteSettings() {
                singleLayer = layer
            };

            string expected = $"--layer \"{layer}\"";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (string.IsNullOrEmpty(layer)) {
                CollectionAssert.DoesNotContain(actual, expected);
            } else {
                CollectionAssert.Contains(actual, expected);
            }
        }

        [TestCase("")]
        [TestCase("Idle")]
        [TestCase("Walk")]
        public void TestCreateArgumentsFilterByTag(string tag) {
            var sut = new AsepriteSettings() {
                singleTag = tag
            };

            string expected = $"--tag \"{tag}\"";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (string.IsNullOrEmpty(tag)) {
                CollectionAssert.DoesNotContain(actual, expected);
            } else {
                CollectionAssert.Contains(actual, expected);
            }
        }

        [TestCase("")]
        [TestCase("Nekkid")]
        [TestCase("WithHat")]
        public void TestCreateArgumentsFilerBySlice(string slice) {
            var sut = new AsepriteSettings() {
                singleSlice = slice
            };

            string expected = $"--slice \"{slice}\"";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (string.IsNullOrEmpty(slice)) {
                CollectionAssert.DoesNotContain(actual, expected);
            } else {
                CollectionAssert.Contains(actual, expected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreateArgumentsSplitLayers(bool splitLayers) {
            var sut = new AsepriteSettings() {
                splitByLayers = splitLayers
            };

            string expected = "--split-layers";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (splitLayers) {
                CollectionAssert.Contains(actual, expected);
            } else {
                CollectionAssert.DoesNotContain(actual, expected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreateArgumentsSplitTags(bool splitTags) {
            var sut = new AsepriteSettings() {
                splitByTags = splitTags
            };

            string expected = "--split-tags";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (splitTags) {
                CollectionAssert.Contains(actual, expected);
            } else {
                CollectionAssert.DoesNotContain(actual, expected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreateArgumentsSplitSlices(bool splitSlices) {
            var sut = new AsepriteSettings() {
                splitBySlices = splitSlices
            };

            string expected = "--split-slices";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (splitSlices) {
                CollectionAssert.Contains(actual, expected);
            } else {
                CollectionAssert.DoesNotContain(actual, expected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreateArgumentsSplitGrid(bool splitGrid) {
            var sut = new AsepriteSettings() {
                splitByTileGrid = splitGrid
            };

            string expected = "--split-grid";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (splitGrid) {
                CollectionAssert.Contains(actual, expected);
            } else {
                CollectionAssert.DoesNotContain(actual, expected);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestCreateArgumentsIgnoreEmpty(bool ignoreEmpty) {
            var sut = new AsepriteSettings() {
                ignoreEmptySprites = ignoreEmpty
            };

            string expected = "--ignore-empty";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (ignoreEmpty) {
                CollectionAssert.Contains(actual, expected);
            } else {
                CollectionAssert.DoesNotContain(actual, expected);
            }
        }

        [TestCase(-1)]
        [TestCase(TestAsset.PortraitsWithTags_Data)]
        [TestCase(TestAsset.SpiderWeb_Data)]
        public void TestCreateArgumentsData(int dataAsset) {
            var sut = new AsepriteSettings();

            bool isMissing = dataAsset == -1;
            var dataFile = isMissing
                ? default
                : info.GetFile((TestAsset)dataAsset);

            string expected = $"--data \"{dataFile?.FullName}\"";

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (isMissing) {
                CollectionAssert.DoesNotContain(actual, expected);
            } else {
                CollectionAssert.Contains(actual, expected);
            }
        }

        [TestCase("--list-layers")]
        [TestCase("--list-tags")]
        [TestCase("--list-slices")]
        [TestCase("--format json-array")]
        public void TestCreateArgumentsDataOptions(string expected) {
            var sut = new AsepriteSettings();

            var actual = sut.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (dataFile is null) {
                CollectionAssert.DoesNotContain(actual, expected);
            } else {
                CollectionAssert.Contains(actual, expected);
            }
        }

        [Test]
        public void TestCreateArgumentsArgumentNullException() {
            var sut = new AsepriteSettings();

            Assert.Throws<ArgumentNullException>(() => sut.CreateArguments(default, sheetFile, dataFile).ToList());
        }
    }
}
