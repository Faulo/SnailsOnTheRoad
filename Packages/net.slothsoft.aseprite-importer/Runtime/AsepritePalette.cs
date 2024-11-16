using System;
using System.Collections.Generic;
using System.Linq;
using CursedBroom.Core.Extensions;
using UnityEngine;

namespace CursedBroom.Aseprite {
    [Serializable]
    public sealed record AsepritePalette {
        [Serializable]
        struct AsepritePaletteHue {
            [SerializeField]
            public Color32[] shades;
        }

        const int SHADES_PER_ROW = 9;

        public Color32 this[Vector2Int position] {
            get {
                if (position.y >= 0 && position.y < height) {
                    var hue = hues[position.y];
                    if (position.x >= 0 && position.x < width) {
                        return hue.shades[position.x];
                    }
                }

                Debug.LogWarning($"Invalid palette access {position}");
                return default;
            }
        }

        [SerializeField]
        internal AsepriteContainer masterPalette;

        [SerializeField]
        AsepritePaletteHue[] hues = Array.Empty<AsepritePaletteHue>();
        [SerializeField]
        int m_colorCount;
        public int colorCount => m_colorCount;

        public IEnumerable<Color32> colors => hues
            .SelectMany(h => h.shades)
            .Take(colorCount);

        public IEnumerable<IReadOnlyList<Color32>> colorsByHue => hues
            .Select(h => h.shades);

        public IEnumerable<(Vector2Int position, Color32 color)> colorsByPosition {
            get {
                for (int hue = 0; hue < height; hue++) {
                    for (int shade = 0; shade < width; shade++) {
                        yield return (new(shade, hue), hues[hue].shades[shade]);
                    }
                }
            }
        }

        public int width => SHADES_PER_ROW;

        public int height => hues.Length;

        public AsepritePalette() {
        }
        public AsepritePalette(IEnumerable<Color32> colors) : this(colors.ToList()) {
        }
        public AsepritePalette(IReadOnlyList<Color32> colors) {
            m_colorCount = colors.Count;

            int hueCount = Mathf.CeilToInt((float)m_colorCount / SHADES_PER_ROW);

            hues = new AsepritePaletteHue[hueCount];
            for (int i = 0; i < hueCount; i++) {
                hues[i].shades = new Color32[SHADES_PER_ROW];
            }

            int x = 0;
            int y = 0;
            foreach (var color in colors) {
                if (color.a != 0) {
                    hues[y].shades[x] = color;
                }

                x++;
                if (x == width) {
                    x = 0;
                    y++;
                }
            }
        }

        public bool Equals(AsepritePalette other)
            => other is not null
            && colorCount == other.colorCount
            && colors.SequenceEqual(other.colors);

        public override int GetHashCode() => colorCount;

        public void ApplyMasterPalette(Texture2D texture) {
            if (!masterPalette) {
                return;
            }

            bool hasChanged = false;
            var pixels = texture.GetPixels32();
            for (int i = 0; i < pixels.Length; i++) {
                if (TryGetHueAndShade(pixels[i], out int hue, out int shade) && masterPalette.palette.TryGetColor32(hue, shade, out var color)) {
                    pixels[i] = color;
                    hasChanged = true;
                }
            }

            if (hasChanged) {
                texture.SetPixels32(pixels);
                texture.Apply();
            }
        }

        internal bool TryGetColor32(int hue, int shade, out Color32 color) {
            if (hue == 0 && shade == 0) {
                color = default;
                return true;
            }

            if (hue >= 0 && hue < height) {
                if (shade >= 0 && shade < width) {
                    color = hues[hue].shades[shade];
                    return color.a != 0;
                }
            }

            color = default;
            return false;
        }

        internal bool TryGetHueAndShade(Color32 color, out int hue, out int shade) {
            if (color.a == 0) {
                hue = 0;
                shade = 0;
                return true;
            }

            for (hue = 0; hue < height; hue++) {
                for (shade = 0; shade < width; shade++) {
                    if (hues[hue].shades[shade].IsEqualTo(color)) {
                        return true;
                    }
                }
            }

            shade = width;
            return false;
        }
    }
}