using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using MyBox;
using Slothsoft.UnityExtensions;
#if UNITY_EDITOR
using UnityEditor.U2D.Aseprite;
#endif
using UnityEngine;

namespace Slothsoft.Aseprite {
    public sealed class AsepriteFile : ScriptableObject {
        internal const string FILE_EXTENSION_MAIN = "aseprite";
        internal const string FILE_EXTENSION_SHEET = "png";
        internal const string FILE_EXTENSION_INFO = "json";

        [SerializeField, ReadOnly]
        Vector2 m_pivot = new(0.5f, 0.5f);
        public Vector2 pivot => m_pivot;

        [field: SerializeField, ReadOnly]
        public int pixelsPerUnit { get; } = 16;

        [field: SerializeField, ReadOnly]
        public Texture2D albedo { get; private set; }

        [field: SerializeField, ReadOnly]
        public Texture2D emission { get; private set; }

        [SerializeField, ReadOnly]
        Sprite[] m_sprites;

        /// <summary>
        /// The first of this aseprite's files which actually got created during the import step.
        /// </summary>
        public Sprite firstSprite {
            get {
                for (int i = 0; i < m_sprites.Length; i++) {
                    if (m_sprites[i]) {
                        return m_sprites[i];
                    }
                }

                return default;
            }
        }

        /// <summary>
        /// All sprites that got created during the import step, paired with their aseprite ID.
        /// </summary>
        public IEnumerable<(int index, Sprite sprite)> indexedSprites {
            get {
                for (int i = 0; i < m_sprites.Length; i++) {
                    if (m_sprites[i]) {
                        yield return (i, m_sprites[i]);
                    }
                }
            }
        }

        [SerializeField, ReadOnly]
        AsepriteData m_info;
        public AsepriteData info => m_info ?? new();

        int frameCount => info.frames.Length;

        public bool TryGetSprite(int id, out Sprite sprite) {
            if (0 <= id && id < m_sprites.Length) {
                sprite = m_sprites[id];
                return sprite;
            }

            sprite = default;
            return false;
        }

        public bool TryGetSpriteByFileName(string name, out Sprite sprite) {
            for (int i = 0; i < info.frames.Length; i++) {
                var frame = info.frames[i];
                if (name.Equals(frame.filename, StringComparison.OrdinalIgnoreCase)) {
                    return TryGetSprite(i, out sprite);
                }
            }

            sprite = default;
            return false;
        }

        public bool TryGetSpriteBySlice(string name, out Sprite sprite) {
            for (int i = 0; i < info.meta.slices.Length; i++) {
                var slice = info.meta.slices[i];
                if (name.Equals(slice.name, StringComparison.OrdinalIgnoreCase)) {
                    return TryGetSprite(i * frameCount, out sprite);
                }
            }

            sprite = default;
            return false;
        }

        public bool TryGetSpriteBySlice(AsepriteDataSlice slice, out Sprite sprite) {
            for (int i = 0; i < info.meta.slices.Length; i++) {
                if (info.meta.slices[i] == slice) {
                    return TryGetSprite(i * frameCount, out sprite);
                }
            }

            sprite = default;
            return false;
        }

        public bool TryGetSpriteByTag(string name, out Sprite sprite) {
            foreach (var tag in info.meta.frameTags) {
                if (name.Equals(tag.name, StringComparison.OrdinalIgnoreCase)) {
                    return TryGetSprite(tag.from, out sprite);
                }
            }

            sprite = default;
            return false;
        }

        public bool TryGetSpriteByTag<T>(T name, out Sprite sprite) where T : struct
            => TryGetSpriteByTag(name.ToString(), out sprite);

        public bool TryGetSpriteBySliceAndTag(string sliceName, string tagName, out Sprite sprite) {
            for (int i = 0; i < info.meta.slices.Length; i++) {
                var slice = info.meta.slices[i];
                if (sliceName.Equals(slice.name, StringComparison.OrdinalIgnoreCase)) {
                    foreach (var tag in info.meta.frameTags) {
                        if (tagName.Equals(tag.name, StringComparison.OrdinalIgnoreCase)) {
                            return TryGetSprite((i * frameCount) + tag.from, out sprite);
                        }
                    }
                }
            }

            sprite = default;
            return false;
        }

        public bool TryGetSpriteForSlice(int frameId, string sliceName, out Sprite sprite) {
            int sliceOffset = string.IsNullOrEmpty(sliceName) || info.meta.slices.Length == 0
                ? 0
                : info.meta.slices.FirstIndex(slice => sliceName.Equals(slice.name, StringComparison.OrdinalIgnoreCase));

            return TryGetSprite((sliceOffset * frameCount) + frameId, out sprite);
        }

        public IEnumerable<string> allAnimationNames => info.meta.frameTags
            .Select(tag => tag.name);

        readonly Dictionary<string, AsepriteAnimation> animationCache = new();

        public AsepriteAnimation GetAnimation(string tagName) {
            return animationCache.TryGetValue(tagName, out var animation)
                ? animation
                : animationCache[tagName] = CreateAnimation(tagName);
        }

        AsepriteAnimation CreateAnimation(string tagName) {
            foreach (var tag in info.meta.frameTags) {
                if (tagName.Equals(tag.name, StringComparison.OrdinalIgnoreCase)) {
                    var keyframes = Enumerable.Range(tag.from, tag.to - tag.from + 1)
                        .Select(frame => (info.frames[frame].duration, frame));
                    return new(keyframes, tag.isLooping);
                }
            }

            return default;
        }

#if UNITY_EDITOR
        public static bool TryCreateInstance(FileInfo asepriteFile, out AsepriteFile asset, AsepriteSettings settings, Action<Texture2D> texturePostProcessor = default) {
            asset = CreateInstance<AsepriteFile>();

            asset.name = settings.CreateName(asepriteFile);

            if (!AsepriteExecutable.TryFindAseprite(out var exe)) {
                return false;
            }

            var sheetFile = AsepriteExecutable.CacheFile(asepriteFile, "albedo.png");
            if (sheetFile.Exists) {
                sheetFile.Delete();
            }

            var dataFile = AsepriteExecutable.CacheFile(asepriteFile, FILE_EXTENSION_INFO);
            if (dataFile.Exists) {
                dataFile.Delete();
            }

            var args = settings.CreateArguments(asepriteFile, sheetFile, dataFile);

            if (settings.extractEmission) {
                args = args.Prepend($"--ignore-layer \"{settings.emissionLayerName}\"");
            }

            string result = exe.Execute(args);

            sheetFile.Refresh();
            dataFile.Refresh();

            if (!sheetFile.Exists || !dataFile.Exists) {
                Debug.LogError($"Aseprite Import for {asepriteFile} failed!\n{result}");
                return false;
            }

            asset.albedo = new(2, 2, TextureFormat.RGBA32, settings.textureMipChaining) {
                name = sheetFile.Name,
                filterMode = settings.textureFilterMode,
                alphaIsTransparency = true,
                anisoLevel = 0,
                wrapMode = settings.textureWrapMode,
            };
            asset.albedo.LoadImage(File.ReadAllBytes(sheetFile.FullName));

            texturePostProcessor?.Invoke(asset.albedo);

            asset.m_info = AsepriteData.FromJson(File.ReadAllText(dataFile.FullName));

            var secondaryTexturesList = new List<SecondarySpriteTexture>();
            if (settings.extractEmission) {
                var emissionFile = AsepriteExecutable.CacheFile(asepriteFile, "emission.png");
                if (emissionFile.Exists) {
                    emissionFile.Delete();
                }

                var emissionInfoFile = AsepriteExecutable.CacheFile(asepriteFile, "emission.json");

                var emissionArgs = settings.CreateArguments(asepriteFile, emissionFile, emissionInfoFile);
                emissionArgs = emissionArgs.Prepend($"--layer \"{settings.emissionLayerName}\"");
                exe.Execute(emissionArgs);

                var emissionTexture = new Texture2D(2, 2, TextureFormat.RGBA32, settings.textureMipChaining) {
                    name = emissionFile.Name,
                    filterMode = settings.textureFilterMode,
                    alphaIsTransparency = false,
                    anisoLevel = 0,
                    wrapMode = settings.textureWrapMode,
                };

                emissionFile.Refresh();

                if (emissionFile.Exists
                 && emissionTexture.LoadImage(File.ReadAllBytes(emissionFile.FullName))
                 && emissionTexture.GetPixels().Any(color => !Mathf.Approximately(color.a, 0))) {
                    var emissionInfo = AsepriteData.FromJson(File.ReadAllText(emissionInfoFile.FullName));

                    Texture2D newEmissionTexture = new(asset.albedo.width, asset.albedo.height, TextureFormat.RGBA32, settings.textureMipChaining) {
                        name = emissionFile.Name,
                        filterMode = settings.textureFilterMode,
                        alphaIsTransparency = false,
                        anisoLevel = 0,
                        wrapMode = settings.textureWrapMode,
                    };
                    newEmissionTexture.SetPixels32(new Color32[asset.albedo.width * asset.albedo.height]);

                    for (int i = 0; i < asset.m_info.frames.Length; i++) {
                        var albedo = (RectInt)asset.m_info.frames[i].frame;
                        albedo.y = asset.m_info.meta.size.h - albedo.height - albedo.y;
                        var emission = (RectInt)emissionInfo.frames[i].frame;
                        emission.y = emissionInfo.meta.size.h - emission.height - emission.y;

                        var pixels = emissionTexture.GetPixels32(emission);
                        newEmissionTexture.SetPixels32(albedo, pixels);
                    }

                    newEmissionTexture.Apply();

                    asset.emission = newEmissionTexture;

                    texturePostProcessor?.Invoke(asset.emission);

                    secondaryTexturesList.Add(new() { name = settings.emissionTextureName, texture = newEmissionTexture });
                }
            }

            var secondaryTextures = secondaryTexturesList.ToArray();

            int sliceCount = Mathf.Max(1, asset.info.meta.slices.Length);

            int spriteCount = asset.info.frames.Length;

            asset.m_sprites = new Sprite[sliceCount * spriteCount];

            bool hasSlices = asset.info.meta.slices.Length > 0;

            if (settings.importSlicesIfAvailable && hasSlices) {
                for (int s = 0; s < asset.info.meta.slices.Length; s++) {
                    var slice = asset.info.meta.slices[s];

                    for (int f = 0; f < asset.info.frames.Length; f++) {
                        var frame = asset.info.frames[f];

                        var rect = (Rect)slice.bounds;
                        rect.x += frame.frame.x;
                        rect.y += frame.frame.y;
                        rect.y = asset.info.meta.size.h - rect.height - rect.y;

                        if (!slice.TryGetNineSlice(out var border)) {
                            border = settings.border;
                        }

                        if (!slice.TryGetPivot(out var pivot)) {
                            pivot = settings.pivot;
                        }

                        var sprite = Sprite.Create(asset.albedo, rect, pivot, settings.pixelsPerUnit, 0, SpriteMeshType.FullRect, border, false, secondaryTextures);

                        string[] name = frame.filename.Split('_');
                        sprite.name = string.Join('_', name[..^1].Append(slice.name).Append(name[^1]));

                        if (settings.ignoreEmptySprites && sprite.IsFullyTransparent()) {
                            continue;
                        }

                        asset.m_sprites[f + (s * spriteCount)] = sprite;
                    }
                }
            } else {
                for (int f = 0; f < asset.info.frames.Length; f++) {
                    var frame = asset.info.frames[f];

                    var rect = (Rect)frame.frame;
                    rect.y = asset.info.meta.size.h - rect.y - rect.height;
                    var sprite = Sprite.Create(asset.albedo, rect, settings.pivot, settings.pixelsPerUnit, 0, SpriteMeshType.FullRect, settings.border, false, secondaryTextures);

                    string[] name = frame.filename.Split('.');
                    name[^2] = string.Join('_', name[^2].Split('_')[..^1].Append(f.ToString()));
                    sprite.name = string.Join('.', name);

                    if (settings.ignoreEmptySprites && sprite.IsFullyTransparent()) {
                        continue;
                    }

                    asset.m_sprites[f] = sprite;
                }
            }

            return true;
        }

        public static bool TryCreateInfo(FileInfo asepriteFile, out AsepriteData info, AsepriteSettings settings) {
            if (!AsepriteExecutable.TryFindAseprite(out var exe)) {
                info = default;
                return false;
            }

            var dataFile = AsepriteExecutable.CacheFile(asepriteFile, FILE_EXTENSION_INFO);
            var args = settings.CreateArguments(asepriteFile, dataFile: dataFile);
            exe.Execute(args);

            info = AsepriteData.FromJson(File.ReadAllText(dataFile.FullName));
            return true;
        }

        public static bool TryCreatePalette(FileInfo asepriteFile, out AsepritePalette palette) {
            try {
                using var aseprite = AsepriteReader.ReadFile(asepriteFile.ToString());
                palette = new(GetPaletteChunkColors(aseprite));
                return true;
            } catch (Exception e) {
                Debug.LogException(e);
                palette = default;
                return false;
            }
        }

        const string PALETTE_PROVIDER_INTERFACE = "UnityEditor.U2D.Aseprite.IPaletteProvider";
        static readonly Type paletteProviderInterface = typeof(PaletteChunk)
            .Assembly
            .GetType(PALETTE_PROVIDER_INTERFACE, true);

        const string ENTRIES_PROPERTY = "entries";
        static readonly PropertyInfo entriesProperty = paletteProviderInterface
            .GetProperty(ENTRIES_PROPERTY, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        static IEnumerable<Color32> GetPaletteChunkColors(UnityEditor.U2D.Aseprite.AsepriteFile file) {
            try {
                foreach (var frame in file.frameData) {
                    foreach (var chunk in frame.chunks) {
                        if (chunk is PaletteChunk { entries: ReadOnlyCollection<PaletteEntry> paletteEntries }) {
                            return paletteEntries.Select(entry => entry.color);
                        }

                        if (paletteProviderInterface.IsAssignableFrom(chunk.GetType()) && entriesProperty.GetValue(chunk) is ReadOnlyCollection<PaletteEntry> interfaceEntries) {
                            return interfaceEntries.Select(entry => entry.color);
                        }
                    }
                }
            } catch (ArgumentOutOfRangeException e) {
                Debug.LogException(e);
            }

            throw new Exception($"Failed to find PaletteChunk in {file}.");
        }
#endif
    }
}
