using MyBox;
using UnityEngine;

namespace SotR.Game {
    [CreateAssetMenu]
    public class GameSettings : ScriptableObject {
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
    }
}
