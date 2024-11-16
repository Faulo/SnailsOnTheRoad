using System.Collections.Generic;
using System.IO;
using System.Linq;
using CursedBroom.Aseprite.Editor;
using NUnit.Framework;
using Slothsoft.UnityExtensions;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace CursedBroom.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteAnimatorProcessor))]
    sealed class AsepriteAnimatorProcessorTestsImportSlicedAnimations {
        static readonly Dictionary<FileInfo, AsepriteData> dataCache = new();
        static AsepriteData LookupData(FileInfo file) {
            return dataCache.TryGetValue(file, out var data)
                ? data
                : dataCache[file] = AsepriteFile.TryCreateInfo(file, out var info, new())
                    ? info
                    : new();
        }

        readonly AsepriteAnimatorProcessor sut = new();

        FileInfo asepriteFile => new TestInfo().GetFile(TestAsset.AnimatorWithSlices);
        AsepriteData info => LookupData(asepriteFile);
        Dictionary<string, UnityObject> m_assets;
        Dictionary<string, UnityObject> assets => m_assets ??= sut
            .CreateAssets(asepriteFile, info, new())
            .ToDictionary();
        T GetAsset<T>(string name) where T : UnityObject {
            Assert.That(assets, Contains.Key(name));
            Assert.IsInstanceOf<T>(assets[name]);
            return assets[name] as T;
        }
        AsepriteFile aseprite => GetAsset<AsepriteFile>(nameof(aseprite));
        Texture2D albedo => GetAsset<Texture2D>(nameof(albedo));
        Texture2D emission => GetAsset<Texture2D>(nameof(emission));

        IEnumerable<string> clipKeys => new[] {
            "anim_Nekkid_Idling", "anim_Nekkid_Running",
            "anim_Default_Idling", "anim_Default_Running",
        };

        IEnumerable<AnimationClip> clips => clipKeys
            .Select(GetAsset<AnimationClip>);

        #region 0 general
        [Test]
        public void T00_WhenCreateAssets_ThenReturnUniqueKeys() {
            CollectionAssert.AllItemsAreUnique(assets.Keys);
        }

        [Test]
        public void T01_WhenCreateAssets_ThenReturnUniqueAssets() {
            CollectionAssert.AllItemsAreUnique(assets.Values);
        }
        #endregion

        #region 1 asperite
        [Test]
        public void T10_WhenCreateAssets_ThenAsepriteExists() {
            Assert.IsNotNull(aseprite);
        }
        #endregion

        #region 4 clips
        [Test]
        public void T40_WhenCreateAssets_ThenAllClipsExist() {
            CollectionAssert.AllItemsAreNotNull(clips);
        }

        [Test]
        public void T41_WhenCreateAssets_ThenAllClipsAreNotEmpty() {
            foreach (var clip in clips) {
                Assert.That(clip.empty, Is.False);
            }
        }
        #endregion

    }
}
