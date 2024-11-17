using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    sealed class InputModel : ScriptableObject {
        [SerializeField]
        Vector2 _intendedDirection;

        internal Vector2 intendedDirection {
            get => _intendedDirection;
            set {
                _intendedDirection = value;
                if (value != Vector2.zero) {
                    intendedYaw = Vector2.SignedAngle(Vector2.up, value);
                }
            }
        }

        [field: SerializeField]
        internal float intendedYaw { get; private set; }

        [Space]
        [SerializeField]
        internal bool intendsBoost;
        [SerializeField]
        internal bool intendsShell;
    }
}
