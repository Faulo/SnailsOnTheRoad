using MyBox;
using UnityEngine;
using UnityEngine.Windows;

namespace SotR.Player {
    [CreateAssetMenu]
    sealed class InputModel : ScriptableObject {
        [SerializeField]
        internal Vector3 intendedDirection;
        [SerializeField]
        internal bool intendsBoost;

        internal float cachedIntendedYaw = 0.0f;

        internal float intendedYaw() {
            if (intendedDirection != Vector3.zero) {
                cachedIntendedYaw = Vector3.SignedAngle(Vector3.left, intendedDirection, Vector3.up);
            }
            return cachedIntendedYaw;
        }
    }
}
