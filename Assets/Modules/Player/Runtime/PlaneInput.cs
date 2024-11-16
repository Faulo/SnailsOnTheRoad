using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SotR.Player {
    [RequireComponent(typeof(PlayerInput))]
    sealed class PlaneInput : MonoBehaviour {
        [SerializeField, Expandable]
        InputModel input;

        public void OnTilt(InputValue input) {
            (this.input.intendedRoll, this.input.intendedYaw) = input.Get<Vector2>();
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
            input.intendedYaw = 0;
            input.intendedLeftBrake = 0;
            input.intendedRightBrake = 0;
            input.intendedRoll = 0;
            input.intendsBoost = false;
        }
    }
}
