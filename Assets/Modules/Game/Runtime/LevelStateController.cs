using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.Events;

namespace SotR.Game {
    sealed class LevelStateController : MonoBehaviour {
        [SerializeField, Expandable]
        internal GameSettings settings;

        void OnDisable() {
            settings.ResetRuntime();
        }

        internal void EnterState(LevelState state) {
            if (settings.state == state) {
                return;
            }

            switch ((settings.state, state)) {
                case (LevelState.Start, LevelState.Racing):
                    onStartRace.Invoke();
                    break;
                case (LevelState.Racing, LevelState.Start):
                    onAbortRace.Invoke();
                    break;
                case (LevelState.Racing, LevelState.Goal):
                    if (settings.timer >= 0) {
                        onFinishRace.Invoke();
                    } else {
                        onFailRace.Invoke();
                    }

                    break;
                case (LevelState.Goal, LevelState.Racing):
                    break;
                case (LevelState.Goal, LevelState.Start):
                    if (settings.canLoadNextLevel) {
                        onLoadNextLevel.Invoke();
                    }

                    break;
                case (LevelState.Start, LevelState.Goal):
                    if (settings.canLoadPreviousLevel) {
                        onLoadPreviousLevel.Invoke();
                    }

                    break;
                case (_, LevelState.Goal):
                    break;
                case (_, LevelState.Start):
                    break;
                default:
                    state = LevelState.Invalid;
                    break;
            }

            settings.state = state;
        }

        [SerializeField]
        internal UnityEvent onStartRace = new();

        [SerializeField]
        internal UnityEvent onAbortRace = new();
        [SerializeField]
        internal UnityEvent onFinishRace = new();
        [SerializeField]
        internal UnityEvent onFailRace = new();
        [SerializeField]
        internal UnityEvent onLoadNextLevel = new();
        [SerializeField]
        internal UnityEvent onLoadPreviousLevel = new();
    }
}
