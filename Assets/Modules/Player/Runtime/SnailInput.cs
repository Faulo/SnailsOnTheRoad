using Slothsoft.UnityExtensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SotR.Player {
    [RequireComponent(typeof(PlayerInput))]
    sealed class SnailInput : MonoBehaviour {
        [SerializeField, Expandable]
        InputModel input;

        void Start() {
            input.intendedDirection = Vector3.left;
            input.intendsBoost = false;
        }

        public void OnSetDirection(InputValue input) {
            this.input.intendedDirection = input.Get<Vector2>();
        }

        public void OnBoost(InputValue input) {
            this.input.intendsBoost = input.isPressed;
        }

        public void OnShell(InputValue input) {
            this.input.intendsShell = input.isPressed;
        }
    }
}
