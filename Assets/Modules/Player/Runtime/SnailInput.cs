using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SotR.Player {
    [RequireComponent(typeof(PlayerInput))]
    sealed class SnailInput : MonoBehaviour {
        [SerializeField, Expandable]
        InputModel input;

        public void OnSetDirection(InputValue input) {
            this.input.intendedDirection.x = input.Get<Vector2>().x;
            this.input.intendedDirection.y = 0.0f;
            this.input.intendedDirection.z = input.Get<Vector2>().y;
        }

        public void OnBrakeLeft(InputValue input) {
            this.input.intendedLeftBrake = input.Get<float>();
        }

        public void OnBrakeRight(InputValue input) {
            this.input.intendedRightBrake = input.Get<float>();
        }

        public void OnBoost(InputValue input) {
            this.input.intendsBoost = input.isPressed;
        }

        void Start() {
            input.intendedDirection = Vector3.right;
            input.intendedLeftBrake = 0;
            input.intendedRightBrake = 0;
            input.intendsBoost = false;
        }
    }
}
