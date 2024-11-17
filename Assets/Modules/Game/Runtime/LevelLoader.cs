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

        internal Level loadLevel { get; private set; }

        public void LoadNextLevel() {
            settings.currentLevelIndex++;
        }

        public void LoadPreviousLevel() {
            settings.currentLevelIndex--;
        }

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

                    loadLevel = loadedLevelInstance.GetComponent<Level>();
                }
            }
        }
    }
}
