using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Slothsoft.UnityExtensions.Editor;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Editor {
    [ScriptedImporter(VERSION, null, overrideExts: new[] { AsepriteFile.FILE_EXTENSION_MAIN }, importQueueOffset: 1000)]
    sealed class AsepriteImporter : ScriptedImporter {
        const int VERSION = 34;

        [SerializeField]
        internal bool useMasterPalette = false;

        [SerializeField]
        internal ProcessorContainer importVariants = new();

        public override void OnImportAsset(AssetImportContext context) {
            if (importVariants.processors.Length == 0) {
                return;
            }

            AsepriteContainer masterColorPalette = default;

            if (useMasterPalette && !context.TryDependOnArtifact(AsepriteProjectSettings.masterColorPalettePath, out masterColorPalette)) {
                return;
            }

            var settings = new AsepriteSettings() {
                includeHiddenLayers = true,
                packSheet = false,
            };

            if (!AsepriteFile.TryCreateInfo(new(context.assetPath), out var info, settings)) {
                return;
            }

            var result = ScriptableObject.CreateInstance<AsepriteContainer>();
            result.name = new FileInfo(context.assetPath).Name;
            result.info = info;

            if (AsepriteFile.TryCreatePalette(new(context.assetPath), out var palette)) {
                palette.masterPalette = masterColorPalette;
                result.palette = palette;
            } else {
                palette = new();
                result.palette = palette;
            }

            UnityObject root = result;

            var dictionary = new Dictionary<string, UnityObject>();

            Texture2D firstTexture = null;

            var paletteColors = new HashSet<Color32>(palette is null ? Enumerable.Empty<Color32>() : palette.colors);
            var allColors = new HashSet<Color32>();
            var missingColors = new HashSet<Color32>();

            foreach (var processor in importVariants.processors) {
                if (processor is null) {
                    throw new NullReferenceException($"Something terrible happened: A processor of {this} disappeared!");
                }

                string mainKey = $"/{processor.key}/";

                foreach (var (subKey, asset) in processor.CreateAssets(new(context.assetPath), info, palette)) {
                    string key = mainKey + subKey;

                    if (dictionary.TryGetValue(key, out var oldAsset)) {
                        Debug.LogWarning($"Key '{key}' has already been imported as asset '{oldAsset}'!");
                        continue;
                    }

                    dictionary[key] = asset;

                    if (subKey == "prefab") {
                        root = asset;
                    }

                    if (asset is Texture2D texture) {
                        if (!firstTexture) {
                            firstTexture = texture;
                        }

                        foreach (var color in texture.GetPixels32(0)) {
                            allColors.Add(color);
                            if (!paletteColors.Contains(color)) {
                                missingColors.Add(color);
                            }
                        }

                        context.AddObjectToAsset(key, asset, texture);
                    } else {
                        context.AddObjectToAsset(key, asset);
                    }
                }
            }

            result.usedColors = allColors.ToArray();
            result.usedColorsNotInPalette = missingColors.ToArray();
            result.assets.SetItems(dictionary);

            context.AddObjectToAsset("/", result, firstTexture);
            context.SetMainObject(root);

            AssetDatabase.ForceReserializeAssets(new string[] { assetPath }, ForceReserializeAssetsOptions.ReserializeMetadata);
        }
    }
}
