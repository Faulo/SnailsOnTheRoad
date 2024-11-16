using MyBox;
using UnityEngine;
using UnityEngine.Windows;

namespace SotR.Player {
    [CreateAssetMenu]
    sealed class InputModel : ScriptableObject {
        [SerializeField]
        internal Vector2 intendedDirection;
        [SerializeField]
        internal bool intendsBoost;

        internal float intendedYaw;
    }
}
