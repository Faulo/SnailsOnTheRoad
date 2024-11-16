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

            if (this.input.intendedDirection != Vector3.zero) {
                this.input.intendedYaw = Vector3.SignedAngle(Vector3.left, this.input.intendedDirection, Vector3.up);
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
