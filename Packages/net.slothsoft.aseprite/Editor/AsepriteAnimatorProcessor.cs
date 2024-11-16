using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MyBox;
using Slothsoft.UnityExtensions;
using Slothsoft.UnityExtensions.Editor;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Slothsoft.Aseprite.Editor {
    [ImplementationFor(typeof(IAsepriteProcessor), "Import as Animator")]
    [Serializable]
    sealed class AsepriteAnimatorProcessor : IAsepriteProcessor {
        const float TIME_MULTIPLIER = 0.001f;

        public string key => m_key;

        [SerializeField]
        string m_key = "animator";
        [SerializeField]
        internal bool includeEmptyAnimations = false;
        [Header("Animator Options")]
        [SerializeField]
        internal bool importAsAnimator = default;
        [SerializeField]
        internal bool importAsAnimatorOverride = default;
        [SerializeField, ConditionalField(nameof(importAsAnimatorOverride))]
        internal AnimatorController referenceController;
        [SerializeField, ConditionalField(nameof(importAsAnimatorOverride))]
        internal bool importOverridePrefab;

        public IEnumerable<(string key, Object asset)> CreateAssets(FileInfo asepriteFile, AsepriteData info, AsepritePalette palette) {
            if (asepriteFile is null) {
                throw new ArgumentNullException(nameof(asepriteFile));
            }

            if (info is null) {
                throw new ArgumentNullException(nameof(info));
            }

            if (palette is null) {
                throw new ArgumentNullException(nameof(palette));
            }

            if (importAsAnimatorOverride && !referenceController) {
                Debug.LogError("Trying to create an OverrideController, but no ReferenceController was provided");
                yield break;
            }

            var settings = new AsepriteSettings {
                packSheet = !info.HasLayer("Emission"),
                extractEmission = info.HasLayer("Emission")
            };

            if (AsepriteFile.TryCreateInstance(asepriteFile, out var resource, settings, palette.ApplyMasterPalette)) {
                yield return ("aseprite", resource);
                yield return (nameof(resource.albedo), resource.albedo);

                if (resource.emission) {
                    yield return (nameof(resource.emission), resource.emission);
                }

                var spritesToInclude = new HashSet<Sprite>();

                var slices = new List<string>();
                bool hasSlices = resource.info.meta.slices.Length > 0;
                if (hasSlices) {
                    slices.AddRange(resource.info.meta.slices.Select(slice => slice.name));
                } else {
                    slices.Add(string.Empty);
                }

                var tags = resource.info.meta.frameTags;

                var animationClips = new Dictionary<string, AnimationClip>();

                foreach (string sliceName in slices) {
                    foreach (var tag in tags) {
                        var indexedSprites = Enumerable
                            .Range(tag.from, tag.to - tag.from + 1)
                            .Select(i => (i, resource.TryGetSpriteForSlice(i, sliceName, out var sprite) ? sprite : default))
                            .ToList<(int frameId, Sprite sprite)>();

                        if (!includeEmptyAnimations) {
                            bool allSpritesAreTransparent = indexedSprites
                                .Select(frame => frame.sprite)
                                .All(sprite => !sprite || sprite.IsFullyTransparent());
                            if (allSpritesAreTransparent) {
                                continue;
                            }
                        }

                        string tagName = tag.name;

                        string animationName = hasSlices
                            ? $"{sliceName}_{tagName}"
                            : $"{tagName}";

                        //Animation Clip Setup
                        var clip = new AnimationClip {
                            name = $"{resource.name}_{animationName}.anim"
                        };

                        //Keyframe Setup
                        var keyframes = new List<ObjectReferenceKeyframe>();
                        int time = 0;
                        foreach (var (i, sprite) in indexedSprites) {
                            if (sprite) {
                                spritesToInclude.Add(sprite);
                            }

                            keyframes.Add(new() {
                                time = time * TIME_MULTIPLIER,
                                value = sprite,
                            });

                            time += resource.info.frames[i].duration;
                        }

                        var lastSprite = tag.isLooping
                            ? indexedSprites[0].sprite
                            : indexedSprites[^1].sprite;

                        keyframes.Add(new() {
                            time = time * TIME_MULTIPLIER,
                            value = lastSprite,
                        });

                        clip.SetSpriteKeyframes(keyframes.ToArray());

                        //Looping
                        var clipSettings = AnimationUtility.GetAnimationClipSettings(clip);
                        clipSettings.loopTime = tag.isLooping;
                        AnimationUtility.SetAnimationClipSettings(clip, clipSettings);

                        animationClips[animationName] = clip;

                        string key = $"anim_{animationName}";
                        yield return (key, clip);
                    }
                }

                if (importAsAnimator) {
                    var anim = new AnimatorController() {
                        name = $"{resource.name}_AnimatorController.controller"
                    };

                    anim.AddLayer("Animations");

                    // state machine missing?

                    foreach (var clip in animationClips) {
                        var state = anim.layers[0].stateMachine.AddState(clip.Key);

                        state.motion = clip.Value;
                    }

                    yield return ("animator", anim);
                }

                //Animator override import      
                if (importAsAnimatorOverride) {
                    var animOverride = new AnimatorOverrideController(referenceController) {
                        name = $"{resource.name}_OverrideController.overrideController"
                    };
                    yield return ("animatorOverrideController", animOverride);

                    if (importOverridePrefab) {
                        var prefab = new GameObject() {
                            name = $"{resource.name}.prefab"
                        };
                        var anim = prefab.AddComponent<Animator>();
                        anim.runtimeAnimatorController = animOverride;
                        var renderer = prefab.AddComponent<SpriteRenderer>();
                        renderer.sprite = resource.firstSprite;
                        yield return ("prefab", prefab);
                    }

                    // Set override clips
                    var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(animOverride.overridesCount);
                    animOverride.GetOverrides(overrides);

                    var originals = overrides.Select(pair => pair.Key.name).ToList();
                    var originalNames = new List<string>();
                    for (int i = 0; i < originals.Count; i++) {
                        string name = originals[i].Split("__")[^1];
                        originalNames.Add(name);
                        overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(overrides[i].Key, animationClips[name]);
                    }

                    //compare tags to originals
                    foreach (string tag in animationClips.Keys) {
                        if (!originalNames.Contains(tag)) {
                            Debug.LogWarning($"AsepriteFile has the tag '{tag}', but the referenced animator controller doesn't.");
                        }
                    }

                    // Assign Clips
                    animOverride.ApplyOverrides(overrides);
                }

                foreach (var sprite in spritesToInclude) {
                    int i = resource
                        .indexedSprites
                        .First(keyval => keyval.sprite == sprite)
                        .index;
                    yield return ($"sprite_{i}", sprite);
                }
            }
        }
    }
}
