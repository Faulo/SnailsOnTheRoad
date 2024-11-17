using UnityEngine;

namespace SotR.Game {
    sealed class Timer : MonoBehaviour {
        [SerializeField]
        LevelStateController controller;
        [SerializeField]
        LevelLoader loader;

        void OnEnable() {
            controller.onStartRace.AddListener(StartTimer);
        }

        void OnDisable() {
            controller.onStartRace.RemoveListener(StartTimer);
        }

        void StartTimer() {
            controller.settings.timer = loader.loadLevel.timeLimit;
        }

        void Update() {
            if (controller.settings.state == LevelState.Racing) {
                controller.settings.timer -= Time.deltaTime;
            }
        }
    }
}
