
using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    public sealed class ProfileModel : ScriptableObject {
        [SerializeField]
        internal float dragMultiplier = 1;

        [SerializeField]
        internal float frictionMultiplier = 1;

        [SerializeField]
        internal float boostMultiplier = 1;
    }
}
