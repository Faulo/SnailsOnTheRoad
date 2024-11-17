using SotR.Player;
using UnityEngine;

namespace SotR.Game {
    sealed class BoostField : MonoBehaviour, ISnailEffector {
        [SerializeField]
        LevelObject _levelObject;
        [SerializeField]
        float _boostSpeed = 10;

        Vector2 targetVelocity => _boostSpeed * _levelObject.forward;

        [SerializeField]
        float _boostTime = 1;

        Vector2 _acceleration;

        public void EnterSnail(SnailController controller) {
            _acceleration = Vector2.zero;
        }

        public void EffectSnail(SnailController controller) {
            controller.currentVelocity = Vector2.SmoothDamp(controller.currentVelocity, targetVelocity, ref _acceleration, _boostTime);
        }

        public void ExitSnail(SnailController controller) {
            controller.currentVelocity = targetVelocity;
        }
    }
}
