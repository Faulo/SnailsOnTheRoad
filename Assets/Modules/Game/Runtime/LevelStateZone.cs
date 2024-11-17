using SotR.Player;
using UnityEngine;

namespace SotR.Game {
    sealed class LevelStateZone : MonoBehaviour, ISnailEffector {
        [SerializeField]
        LevelStateController controller;

        [SerializeField]
        LevelState state;

        public void EnterSnail(SnailController snail) {
            controller.EnterState(state);
        }
        public void EffectSnail(SnailController snail) {
        }
        public void ExitSnail(SnailController snail) {
        }
    }
}
