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
        SceneReference endingScene = new();
        public void LoadEnding() {
            endingScene.LoadSceneAsync();
        }

        [SerializeField]
        GameObject[] levels = Array.Empty<GameObject>();

        [ContextMenu(nameof(ResetRuntime))]
        internal void ResetRuntime() {
            currentLevelIndex = 0;
            maximumLevelIndex = 1;
            state = LevelState.Invalid;
            timer = 0;
        }

        [Header("Runtime")]
        [SerializeField, Range(0, 10)]
        internal int currentLevelIndex = 0;
        [SerializeField]
        internal int maximumLevelIndex = 1;
        [SerializeField]
        internal LevelState state = LevelState.Invalid;
        [SerializeField]
        internal float timer = 0;

        internal GameObject currentLevelPrefab => levels.ElementAtOrDefault(currentLevelIndex);

        internal bool canLoadPreviousLevel => currentLevelIndex > 0;

        internal bool canLoadNextLevel => currentLevelIndex < Mathf.Min(maximumLevelIndex, levels.Length);

        internal bool currentLevelIsLastLevel => currentLevelIndex == levels.Length - 1;
    }
}
