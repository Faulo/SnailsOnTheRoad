using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Slothsoft.Aseprite.Editor;
using Slothsoft.UnityExtensions;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.TestTools;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Tests {
    [TestFixture(TestOf = typeof(AsepriteAnimatorProcessor))]
    sealed class AsepriteAnimatorProcessorAnimatorOverrideTests {
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
        FileInfo flawedAsepriteFile => new TestInfo().GetFile(TestAsset.AnimatorAdditionalTag);

        AsepriteData info => LookupData(asepriteFile);
        AsepriteData flawedInfo => LookupData(flawedAsepriteFile);

        Dictionary<string, UnityObject> m_assets;
        Dictionary<string, UnityObject> assets => m_assets ??= sut
            .CreateAssets(asepriteFile, info, new())
            .ToDictionary();
        T GetAsset<T>(string name) where T : UnityObject {
            Assert.That(assets, Contains.Key(name));
            Assert.IsInstanceOf<T>(assets[name]);
            return assets[name] as T;
        }
        AnimatorOverrideController animatorOverrideController => GetAsset<AnimatorOverrideController>(nameof(animatorOverrideController));
        GameObject prefab => GetAsset<GameObject>(nameof(prefab));

        [SetUp]
        public void Setup() {
            sut.importAsAnimatorOverride = true;
            sut.importOverridePrefab = true;
            sut.referenceController = new TestInfo().GetAsset<AnimatorController>(TestAsset.AnimatorController);
        }

        [Test]
        public void T00_WhenReferenceControllerIsNull_ThenThrowError() {
            var sut = new AsepriteAnimatorProcessor {
                importAsAnimatorOverride = true,
                importOverridePrefab = true,
                referenceController = null
            };

            var actual = sut.CreateAssets(asepriteFile, info, new());
            LogAssert.Expect(LogType.Error, "Trying to create an OverrideController, but no ReferenceController was provided");

            CollectionAssert.IsEmpty(actual);
        }

        [Test]
        public void T01_WhenCreateAssets_ThenAnimatorOverrideControllerExists() {
            Assert.IsNotNull(animatorOverrideController);
        }

        [Test]
        public void T02_WhenCreateAssets_ThenAnimatorOverrideControllerHasReferenceController() {
            Assert.That(animatorOverrideController.runtimeAnimatorController, Is.EqualTo(sut.referenceController));
        }

        [TestCase("NotSupported")]
        public void T03_WhenTagsNotMatchingWithReferenceControllerClips_ThenThrowWarning(string tag) {
            var sut = new AsepriteAnimatorProcessor {
                importAsAnimatorOverride = true,
                importOverridePrefab = true,
                referenceController = new TestInfo().GetAsset<AnimatorController>(TestAsset.AnimatorController),
            };

            var actual = sut.CreateAssets(flawedAsepriteFile, flawedInfo, new()).ToList();
            CollectionAssert.IsNotEmpty(actual);
            LogAssert.Expect(LogType.Warning, $"AsepriteFile has the tag '{tag}', but the referenced animator controller doesn't.");
        }

        [TestCase("ANIM_LO_Bird__FlyingDown")]
        [TestCase("ANIM_LO_Bird__FlyingEven")]
        [TestCase("ANIM_LO_Bird__FlyingUp")]
        [TestCase("ANIM_LO_Bird__Resting")]
        public void T04_WhenCreateAssets_OverrideAnimatorHasOriginalClips(string originalClip) {
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(overrides);

            var actual = overrides
                .First(keyval => keyval.Key.name == originalClip);

            Assert.IsNotNull(actual);
        }

        [TestCase("ANIM_LO_Bird__FlyingDown")]
        [TestCase("ANIM_LO_Bird__FlyingEven")]
        [TestCase("ANIM_LO_Bird__FlyingUp")]
        [TestCase("ANIM_LO_Bird__Resting")]
        public void T05_WhenCreateAssets_OverrideAnimatorHasOverrideClips(string originalClip) {
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(overrides);

            var overrideClip = overrides
                .First(keyval => keyval.Key.name == originalClip)
                .Value;

            Assert.IsNotNull(overrideClip);
        }

        [TestCase("ANIM_LO_Bird__FlyingDown", "TEST_Aseprite_Animator_FlyingDown.anim")]
        [TestCase("ANIM_LO_Bird__FlyingEven", "TEST_Aseprite_Animator_FlyingEven.anim")]
        [TestCase("ANIM_LO_Bird__FlyingUp", "TEST_Aseprite_Animator_FlyingUp.anim")]
        [TestCase("ANIM_LO_Bird__Resting", "TEST_Aseprite_Animator_Resting.anim")]
        public void T05_WhenCreateAssets_OverrideAnimatiorHasMatchingOverrideClips(string originalClip, string expectedOverride) {
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(overrides);

            string actualAnimation = overrides
                .First(keyval => keyval.Key.name == originalClip)
                .Value
                .name;

            Assert.That(actualAnimation, Is.EqualTo(expectedOverride));
        }

        [Test]
        public void T10_WhenCreateAssets_ThenAnimatorOverridePrefabExists() {
            Assert.IsNotNull(prefab);
        }

        [Test]
        public void T11_WhenCreateAssets_ThenAnimatorOverridePrefabHasAnimator() {
            var animator = prefab.GetComponent<Animator>();
            Assert.IsNotNull(animator);
        }

        [Test]
        public void T12_WhenCreateAssets_ThenAnimatorHasOverrideController() {
            var animator = prefab.GetComponent<Animator>();
            Assert.That(animator.runtimeAnimatorController, Is.EqualTo(animatorOverrideController));
        }

        [Test]
        public void T13_WhenCreateAssets_ThenAnimatorOverridePrefabHasSpriteRenderer() {
            var renderer = prefab.GetComponent<SpriteRenderer>();
            Assert.IsNotNull(renderer);
        }
    }
}
