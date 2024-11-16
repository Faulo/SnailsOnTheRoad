using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Slothsoft.Aseprite.Editor;
using Slothsoft.UnityExtensions;
using Slothsoft.UnityExtensions.Editor;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteAnimatorProcessor))]
    sealed class AsepriteAnimatorProcessorTestsIncludeEmptyAnimations {
        static readonly Dictionary<FileInfo, AsepriteData> dataCache = new();
        static AsepriteData LookupData(FileInfo file) {
            return dataCache.TryGetValue(file, out var data)
                ? data
                : dataCache[file] = AsepriteFile.TryCreateInfo(file, out var info, new())
                    ? info
                    : new();
        }

        readonly AsepriteAnimatorProcessor sut = new() {
            includeEmptyAnimations = true,
        };

        FileInfo asepriteFile => new TestInfo().GetFile(TestAsset.Animator);
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
            "anim_Resting", "anim_FlyingUp",
            "anim_FlyingEven", "anim_FlyingDown",
            "anim_NoSprite"
        };

        IEnumerable<AnimationClip> clips => clipKeys
            .Select(GetAsset<AnimationClip>);

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

        [TestCase("NoSprite", 3)]
        public void T42_WhenCreateAssets_ThenCheckKeyframeCount(string name, int count) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");

            var frames = clip.GetSpriteKeyframes();

            Assert.That(frames, Is.Not.Null);
            Assert.That(frames.Length, Is.EqualTo(count));
        }

        [TestCase("NoSprite", "TEST_Aseprite_Animator_NoSprite.anim")]
        public void T43_WhenCreateAssets_ThenAssetName(string name, string expected) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");

            Assert.That(clip.name, Is.EqualTo(expected));
        }

        [TestCase("NoSprite", 0, 20)]
        public void T44_WhenCreateAssets_ThenCheckSpriteMatches(string name, int offset, int spriteNo) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");

            var frames = clip.GetSpriteKeyframes();

            Assert.That(frames, Is.Not.Null);
            Assert.That(frames[offset].value, Is.EqualTo(GetAsset<Sprite>($"sprite_{spriteNo}")));
        }
        #endregion

        #region 5 sprites
        [TestCase("sprite_20")]
        [TestCase("sprite_21")]
        public void T50_WhenCreateAssets_ThenDoIncludeEmptySprite(string name) {
            Assert.That(assets, Does.ContainKey(name));
        }
        #endregion
    }
}
