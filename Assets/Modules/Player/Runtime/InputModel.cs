using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    sealed class InputModel : ScriptableObject {
        [SerializeField]
        internal Vector2 intendedDirection;
        [SerializeField]
        internal float intendedYaw;

        [Space]
        [SerializeField]
        internal bool intendsBoost;
        [SerializeField]
        internal bool intendsShell;
    }
}
