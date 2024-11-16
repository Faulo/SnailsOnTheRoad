using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SotR.Player {
    [RequireComponent(typeof(PlayerInput))]
    sealed class SnailInput : MonoBehaviour {
        [SerializeField, Expandable]
        InputModel input;

        public void OnSetDirection(InputValue input) {
            this.input.intendedDirection = input.Get<Vector2>();

            if (this.input.intendedDirection != Vector2.zero) {
                this.input.intendedYaw = Vector2.SignedAngle(Vector2.up, this.input.intendedDirection);
            }
        }

        public void OnBoost(InputValue input) {
            this.input.intendsBoost = input.isPressed;
        }

        void Start() {
            input.intendedDirection = Vector3.left;
            input.intendsBoost = false;
        }
    }
}
