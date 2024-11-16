using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CursedBroom.Core;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace CursedBroom.Aseprite.Editor {
    [ImplementationFor(typeof(IAsepriteProcessor), "Import as Palette")]
    [Serializable]
    sealed class AsepritePaletteProcessor : IAsepriteProcessor {
        public string key => m_key;

        [Header("Palette Processor Options")]
        [SerializeField]
        string m_key = "palette";

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

            var texture = new Texture2D(palette.width, palette.height, TextureFormat.RGBA32, false) {
                name = $"{asepriteFile.Name}.palette",
                alphaIsTransparency = true,
                wrapMode = TextureWrapMode.Clamp,
                anisoLevel = 0,
                filterMode = FilterMode.Point,
            };

            texture.SetPixels32(palette.colorsByHue.Reverse().SelectMany(hue => hue).ToArray());

            texture.Apply();

            yield return ("texture", texture);

            var sprite = Sprite.Create(
                texture,
                new Rect(0, 0, palette.width, palette.height),
                new Vector2(0.5f, 0.5f),
                1,
                0,
                SpriteMeshType.FullRect,
                Vector4.zero,
                false
            );
            sprite.name = $"{asepriteFile.Name}.palette";

            yield return ("sprite", sprite);

            foreach (var (position, color) in palette.colorsByPosition) {
                string html = ColorUtility.ToHtmlStringRGB(color);
                var asset = ScriptableObject.CreateInstance<ColorAsset>();
                asset.name = $"{asepriteFile.Name}.color.{position.y:D2}_{position.x:D2}: #{html}";
                asset.color32 = color;
                asset.color = color;

                yield return ($"color/{position.x}x{position.y}", asset);
            }
        }
    }
}
