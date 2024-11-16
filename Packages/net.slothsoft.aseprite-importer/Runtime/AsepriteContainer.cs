using System;
using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace CursedBroom.Aseprite {
    public sealed class AsepriteContainer : ScriptableObject {
#if UNITY_EDITOR
        const string ASSET_FOLDER = "Assets/Art";

        public static IEnumerable<(string path, AsepriteContainer asset)> allAsepriteAssets => AssetDatabase
            .FindAssets($"t:{typeof(AsepriteContainer).Name}", new[] { ASSET_FOLDER })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(path => (path, AssetDatabase.LoadAssetAtPath<AsepriteContainer>(path)));
#endif

        [SerializeField]
        internal AsepriteData info = new();
        [SerializeField]
        internal Color32[] usedColors = Array.Empty<Color32>();
        [SerializeField]
        internal Color32[] usedColorsNotInPalette = Array.Empty<Color32>();
        [SerializeField]
        internal AsepritePalette palette = new();
        [SerializeField]
        internal SerializableKeyValuePairs<string, UnityObject> assets = new();

        public IEnumerable<T> GetAssets<T>() where T : UnityObject {
            return assets
                .Values
                .OfType<T>();
        }
    }
}
