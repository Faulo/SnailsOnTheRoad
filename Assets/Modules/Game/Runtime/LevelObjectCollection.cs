using Slothsoft.Aseprite;
using Slothsoft.UnityExtensions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SotR.Game {
    [CreateAssetMenu]
    sealed class LevelObjectCollection : ScriptableObject {
        [SerializeField]
        GameObject _prefab;
        [SerializeField]
        AsepriteFile _spritesheet;

#if UNITY_EDITOR
        [CustomEditor(typeof(LevelObjectCollection))]
        sealed class LevelObjectCollectionEditor : RuntimeEditorTools<LevelObjectCollection> {
            protected override void DrawEditorTools() {
                DrawButton("Create Prefabs", target.CreatePrefabs);
            }
        }

        [ContextMenu(nameof(CreatePrefabs))]
        void CreatePrefabs() {
            if (!_prefab || !_spritesheet) {
                return;
            }

            foreach (var slice in _spritesheet.info.meta.slices) {
                if (!_spritesheet.TryGetSpriteBySlice(slice.name, out var sprite)) {
                    Debug.LogError($"Failed to find sprite '{slice.name}'!");
                    continue;
                }

                string path = AssetDatabase.GetAssetPath(_prefab);
                string name = $"{_prefab.name}_{slice.name}";
                string variantPath = path.Replace(_prefab.name, name);

                var prefabVariant = AssetDatabase.LoadAssetAtPath<GameObject>(variantPath);
                if (!prefabVariant) {
                    var prefabInstance = PrefabUtility.InstantiatePrefab(_prefab) as GameObject;
                    prefabVariant = PrefabUtility.SaveAsPrefabAsset(prefabInstance, variantPath);
                    DestroyImmediate(prefabInstance);
                }

                var renderer = prefabVariant.GetComponent<SpriteRenderer>();
                renderer.sprite = sprite;

                var levelObject = prefabVariant.GetComponent<LevelObject>();
                levelObject.UpdatePolygonCollider2D();

                EditorUtility.SetDirty(renderer);
            }
        }
#endif
    }
}
