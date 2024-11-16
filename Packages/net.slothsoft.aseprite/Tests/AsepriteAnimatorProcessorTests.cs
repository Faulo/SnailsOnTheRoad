using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Slothsoft.Aseprite.Editor;
using Slothsoft.UnityExtensions;
using Slothsoft.UnityExtensions.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools.Utils;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteAnimatorProcessor))]
    sealed class AsepriteAnimatorProcessorTests {
        static readonly Dictionary<FileInfo, AsepriteData> dataCache = new();
        static AsepriteData LookupData(FileInfo file) {
            return dataCache.TryGetValue(file, out var data)
                ? data
                : dataCache[file] = AsepriteFile.TryCreateInfo(file, out var info, new())
                    ? info
                    : new();
        }

        readonly AsepriteAnimatorProcessor sut = new();

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
        AnimatorController animator => GetAsset<AnimatorController>(nameof(animator));

        IEnumerable<string> spriteKeys => new[] {
            "sprite_0",
            "sprite_1",
            "sprite_2",
            "sprite_3",
            "sprite_4",
            "sprite_5",
            "sprite_6",
            "sprite_7",
            "sprite_8",
            "sprite_9",
            "sprite_10",
            "sprite_11",
            "sprite_12",
            "sprite_13",
            "sprite_14",
            "sprite_15",
            "sprite_16",
            "sprite_17",
            "sprite_18",
            "sprite_19",
        };
        IEnumerable<Sprite> sprites => spriteKeys
            .Select(GetAsset<Sprite>);

        IEnumerable<string> clipKeys => new[] {
            "anim_Resting", "anim_FlyingUp",
            "anim_FlyingEven", "anim_FlyingDown",
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

        #region 2 albedo
        [Test]
        public void T20_WhenCreateAssets_ThenAlbedoExists() {
            Assert.IsNotNull(albedo);
        }

        [Test]
        public void T21_GivenCreatedAssets_Albedo_IsPointFiltered() {
            Assert.That(albedo.filterMode, Is.EqualTo(FilterMode.Point));
        }

        [Test]
        public void T22_GivenCreatedAssets_Albedo_IsTransparent() {
            Assert.That(albedo.alphaIsTransparency, Is.True);
        }
        #endregion

        #region 3 emission

        [Test]
        public void T30_WhenCreateAssets_ThenEmissionExists() {
            Assert.IsNotNull(emission);
        }

        [Test]
        public void T31_GivenCreatedAssets_Emission_IsPointFiltered() {
            Assert.That(emission.filterMode, Is.EqualTo(FilterMode.Point));
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

        [TestCase("Resting", 5)]
        [TestCase("FlyingUp", 8)]
        [TestCase("FlyingEven", 8)]
        [TestCase("FlyingDown", 3)]
        public void T42_WhenCreateAssets_ThenCheckKeyframeCount(string name, int count) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");

            var frames = clip.GetSpriteKeyframes();

            Assert.That(frames, Is.Not.Null);
            Assert.That(frames.Length, Is.EqualTo(count));
        }

        [TestCase("Resting", "TEST_Aseprite_Animator_Resting.anim")]
        [TestCase("FlyingUp", "TEST_Aseprite_Animator_FlyingUp.anim")]
        [TestCase("FlyingEven", "TEST_Aseprite_Animator_FlyingEven.anim")]
        [TestCase("FlyingDown", "TEST_Aseprite_Animator_FlyingDown.anim")]
        public void T43_WhenCreateAssets_ThenAssetName(string name, string expected) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");

            Assert.That(clip.name, Is.EqualTo(expected));
        }

        [TestCase("Resting", 0, 0)]
        [TestCase("Resting", 1, 1)]
        [TestCase("Resting", 4, 0)]
        [TestCase("FlyingUp", 0, 4)]
        [TestCase("FlyingUp", 2, 6)]
        [TestCase("FlyingEven", 0, 11)]
        [TestCase("FlyingEven", 3, 14)]
        [TestCase("FlyingDown", 0, 18)]
        [TestCase("FlyingDown", 1, 19)]
        public void T44_WhenCreateAssets_ThenCheckSpriteMatches(string name, int offset, int spriteNo) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");

            var frames = clip.GetSpriteKeyframes();

            Assert.That(frames, Is.Not.Null);
            Assert.That(frames[offset].value, Is.EqualTo(GetAsset<Sprite>($"sprite_{spriteNo}")));
        }

        [TestCase("Resting", true)]
        [TestCase("FlyingUp", false)]
        [TestCase("FlyingEven", true)]
        [TestCase("FlyingDown", true)]
        public void T45_WhenCreateAssets_ThenCheckIsLooping(string name, bool isLooping) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");

            Assert.That(clip.isLooping, Is.EqualTo(isLooping));
        }

        [TestCase("Resting", 0, 0)]
        [TestCase("Resting", 1, 0.15f)]
        [TestCase("Resting", 2, 0.25f)]
        [TestCase("Resting", 3, 0.35f)]
        [TestCase("Resting", 4, 0.5f)]
        [TestCase("FlyingDown", 0, 0)]
        [TestCase("FlyingDown", 1, 0.1f)]
        [TestCase("FlyingDown", 2, 0.2f)]
        public void T46_WhenCreateAssets_ThenCheckKeyframeTimings(string name, int offset, float expected) {
            var clip = GetAsset<AnimationClip>($"anim_{name}");
            float timing = clip.GetSpriteKeyframes()[offset].time;

            Assert.That(timing, Is.EqualTo(expected).Using(new FloatEqualityComparer(0.00001f)));
        }
        #endregion

        #region 5 sprites

        [Test]
        public void T50_WhenCreateAssets_ThenAllSpritesExist() {
            CollectionAssert.AllItemsAreNotNull(sprites);
        }

        [Test]
        public void T51_GivenCreatedAssets_Sprites_UsesAlbedo() {
            foreach (var sprite in sprites) {
                Assert.That(sprite.texture, Is.EqualTo(albedo));
            }
        }

        [Test]
        public void T52_GivenCreatedAssets_Sprites_HaveSecondarySpriteTexture() {
            foreach (var sprite in sprites) {
                Assert.That(sprite.GetSecondaryTextureCount(), Is.EqualTo(1));
            }
        }

        [Test]
        public void T53_GivenCreatedAssets_Sprites_HaveEmissionName() {
            foreach (var sprite in sprites) {
                var textures = new SecondarySpriteTexture[1];
                sprite.GetSecondaryTextures(textures);
                Assert.That(textures[0].name, Is.EqualTo("_EmissionTex"));
            }
        }

        [Test]
        public void T54_GivenCreatedAssets_Sprites_UseEmissionTexture() {
            foreach (var sprite in sprites) {
                var textures = new SecondarySpriteTexture[1];
                sprite.GetSecondaryTextures(textures);
                Assert.That(textures[0].texture, Is.EqualTo(emission));
            }
        }

        [TestCase("sprite_20")]
        [TestCase("sprite_21")]
        public void T55_WhenCreateAssets_ThenDoNotIncludeEmptySprite(string name) {
            Assert.That(assets, Does.Not.ContainKey(name));
        }
        #endregion

        public void T60_WhenImportAnimator_AnimatorStatesCreated() {
            sut.importAsAnimator = true;
            var states = animator.layers[0].stateMachine.states;
            Debug.Log(states.Length);
        }
    }
}
