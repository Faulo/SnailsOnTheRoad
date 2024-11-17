using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SotR.Game {
    [ExecuteAlways]
    sealed class LevelLoader : MonoBehaviour {
        [SerializeField, Expandable]
        GameSettings settings;
        [SerializeField]
        Transform levelPivot;

        GameObject loadedLevelPrefab;
        GameObject loadedLevelInstance;

        void Update() {
            if (!settings || !levelPivot) {
                return;
            }

            if (loadedLevelPrefab != settings.currentLevelPrefab) {
                loadedLevelPrefab = settings.currentLevelPrefab;
                levelPivot.Clear();

                if (loadedLevelPrefab) {
                    if (Application.isPlaying) {
                        loadedLevelInstance = Instantiate(loadedLevelPrefab, levelPivot);
                    } else {
#if UNITY_EDITOR
                        loadedLevelInstance = UnityEditor.PrefabUtility.InstantiatePrefab(loadedLevelPrefab, levelPivot) as GameObject;
                        loadedLevelInstance.hideFlags = HideFlags.DontSave;
#endif
                    }
                }
            }
        }
    }
}
