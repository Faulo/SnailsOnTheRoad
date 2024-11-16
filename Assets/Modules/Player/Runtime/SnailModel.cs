using MyBox;
using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    public sealed class SnailModel : ScriptableObject {
        [Space]
        [SerializeField]
        internal float yawSpeed = 1;
        [SerializeField]
        internal float boostMultiplier = 1;
    }
}
