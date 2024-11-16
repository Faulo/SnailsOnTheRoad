using System;
using System.Collections.Generic;
using System.IO;
using CursedBroom.Core;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace CursedBroom.Aseprite.Editor {
    [ImplementationFor(typeof(IAsepriteProcessor), "Import as Sprite")]
    [Serializable]
    sealed class AsepriteSpriteProcessor : IAsepriteProcessor {
        public string key => m_key;

        [Header("Sprite Processor Options")]
        [SerializeField]
        string m_key = "sprite";
        [SerializeField]
        AsepriteSettings m_settings = new();

        public IEnumerable<(string key, UnityObject asset)> CreateAssets(FileInfo asepriteFile, AsepriteData info, AsepritePalette palette) {
            if (asepriteFile is null) {
                throw new ArgumentNullException(nameof(asepriteFile));
            }

            if (info is null) {
                throw new ArgumentNullException(nameof(info));
            }

            if (palette is null) {
                throw new ArgumentNullException(nameof(palette));
            }

            var settings = m_settings with {
                extractEmission = m_settings.extractEmission && info.HasLayer("Emission")
            };

            if (AsepriteFile.TryCreateInstance(asepriteFile, out var resource, settings, palette.ApplyMasterPalette)) {
                yield return ("aseprite", resource);

                yield return (nameof(resource.albedo), resource.albedo);

                if (resource.emission) {
                    yield return (nameof(resource.emission), resource.emission);
                }

                foreach (var (i, sprite) in resource.indexedSprites) {
                    yield return ($"sprite_{i}", sprite);
                }
            }
        }
    }
}
