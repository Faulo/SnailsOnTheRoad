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

        [SerializeField]
        int currentLevelIndex = 0;

        internal GameObject currentLevelPrefab => levels.ElementAtOrDefault(currentLevelIndex);
    }
}
