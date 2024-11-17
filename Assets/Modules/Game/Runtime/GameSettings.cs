using System;
using System.Linq;
using MyBox;
using UnityEngine;

namespace SotR.Game {
    [CreateAssetMenu]
    sealed class GameSettings : ScriptableObject {
        [SerializeField]
        SceneReference mainMenuScene = new();
        public void LoadMainMenu() {
            mainMenuScene.LoadSceneAsync();
        }

        [SerializeField]
        SceneReference introScene = new();
        public void LoadIntro() {
            introScene.LoadSceneAsync();
        }

        [SerializeField]
        SceneReference gameScene = new();
        public void LoadGame() {
            gameScene.LoadSceneAsync();
        }

        [SerializeField]
        GameObject[] levels = Array.Empty<GameObject>();

        internal void ResetRuntime() {
            state = LevelState.Invalid;
            currentLevelIndex = 0;
            timer = 0;
        }

        [Header("Runtime")]
        [SerializeField, Range(0, 10)]
        internal int currentLevelIndex = 0;
        [SerializeField]
        internal int maximumLevelIndex = 1;
        [SerializeField]
        internal LevelState state;
        [SerializeField]
        internal float timer;

        internal GameObject currentLevelPrefab => levels.ElementAtOrDefault(currentLevelIndex);

        internal bool canLoadPreviousLevel => currentLevelIndex > 0;

        internal bool canLoadNextLevel => currentLevelIndex < Mathf.Min(maximumLevelIndex, levels.Length);
    }
}
