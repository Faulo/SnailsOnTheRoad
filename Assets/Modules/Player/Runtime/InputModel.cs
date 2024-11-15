using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    sealed class InputModel : ScriptableObject {
        [SerializeField]
        internal float intendedYaw;
        [SerializeField]
        internal float intendedRoll;
        [SerializeField]
        internal bool intendsBoost;
        [SerializeField]
        internal float intendedLeftBrake;
        [SerializeField]
        internal float intendedRightBrake;

        internal float intendedPitch => intendedRightBrake - intendedLeftBrake;
    }
}
