using System;
using System.Collections.Generic;
using System.IO;
using MyBox;
using UnityEngine;

namespace CursedBroom.Aseprite {
    [Serializable]
    public sealed record AsepriteSettings {
        [Header("Sheet filter")]
        [SerializeField]
        internal string singleLayer = string.Empty;
        [SerializeField]
        internal string singleTag = string.Empty;
        [SerializeField]
        internal string singleSlice = string.Empty;

        [Header("Sheet format")]
        [SerializeField]
        public bool useTilesetLayers = false;
        [SerializeField]
        public bool splitByTileGrid = false;
        [SerializeField]
        internal bool splitByLayers = false;
        [SerializeField]
        internal bool splitByTags = false;
        [SerializeField]
        internal bool splitBySlices = false;

        [Header("Sprite optimizations")]
        [SerializeField]
        internal bool exportAsSheet = true;
        [SerializeField]
        internal bool exportInTrueColor = true;
        [SerializeField]
        internal bool packSheet = true;
        [SerializeField]
        public bool ignoreEmptySprites = false;

        [Header("Texture configuration")]
        [SerializeField]
        internal bool includeHiddenLayers = false;
        [SerializeField]
        internal bool extractEmission = true;
        [SerializeField, ConditionalField(nameof(extractEmission)), ReadOnly]
        internal string emissionLayerName = "Emission";
        [SerializeField, ConditionalField(nameof(extractEmission)), ReadOnly]
        internal string emissionTextureName = "_EmissionTex";

        [Header("Post processing")]
        [SerializeField]
        public FilterMode textureFilterMode = FilterMode.Point;
        [SerializeField]
        public TextureWrapMode textureWrapMode = TextureWrapMode.Clamp;
        [SerializeField]
        public bool textureMipChaining = false;
        [SerializeField]
        public Vector2 pivot = new(0.5f, 0.5f);
        [SerializeField, Border]
        public Vector4 border = new(0, 0, 0, 0);
        [SerializeField]
        public float pixelsPerUnit = 16;
        [SerializeField]
        public bool importSlicesIfAvailable = true;

        internal string CreateName(FileInfo asepriteFile) {
            string name = asepriteFile.Name[..^asepriteFile.Extension.Length];
            if (!string.IsNullOrEmpty(singleSlice)) {
                name += "_" + singleSlice;
            }

            return name;
        }

        internal IEnumerable<string> CreateArguments(FileInfo asepriteFile, FileInfo sheetFile = null, FileInfo dataFile = null) {
            if (asepriteFile is null) {
                throw new ArgumentNullException(nameof(asepriteFile));
            }

            if (!string.IsNullOrEmpty(singleLayer)) {
                yield return $"--layer \"{singleLayer}\"";
            }

            if (!string.IsNullOrEmpty(singleTag)) {
                yield return $"--tag \"{singleTag}\"";
            }

            if (!string.IsNullOrEmpty(singleSlice)) {
                yield return $"--slice \"{singleSlice}\"";
            }

            if (includeHiddenLayers) {
                yield return "--all-layers";
            }

            if (splitByLayers) {
                yield return "--split-layers";
            }

            if (splitByTags) {
                yield return "--split-tags";
            }

            if (splitBySlices) {
                yield return "--split-slices";
            }

            if (splitByTileGrid) {
                yield return "--split-grid";
            }

            if (useTilesetLayers) {
                yield return "--export-tileset";
            }

            if (asepriteFile is not null) {
                yield return $"\"{asepriteFile.FullName}\"";
            }

            if (exportInTrueColor) {
                yield return "--color-mode rgb";
            }

            if (packSheet) {
                yield return "--sheet-pack";
            }

            if (ignoreEmptySprites) {
                yield return "--ignore-empty";
            }

            if (sheetFile is not null) {
                if (exportAsSheet) {
                    yield return $"--sheet \"{sheetFile.FullName}\"";
                } else {
                    yield return $"--save-as \"{sheetFile.FullName}\"";
                }
            }

            if (dataFile is not null) {
                yield return "--list-layers";
                yield return "--list-tags";
                yield return "--list-slices";
                yield return "--format json-array";
                yield return "--filename-format \"{title}_{frame}.{extension}\"";
                yield return $"--data \"{dataFile.FullName}\"";
            }
        }
    }
}
