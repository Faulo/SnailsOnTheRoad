using MyBox;
using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    public sealed class SnailModel : ScriptableObject {
        [Space]
        [SerializeField]
        internal float yawSpeed = 1;
        [SerializeField]
        internal float moveSpeed = 1;

        [Space]
        [SerializeField]
        internal float dragMinimum = 0;
        [SerializeField]
        internal float dragMaximum = 5;
        [SerializeField]
        internal float boostMultiplier = 1;
        [SerializeField]
        internal float area = 1;
        [SerializeField]
        internal float minHealth = 0;
        [SerializeField]
        internal float maxHealth = 1;

        [Header("Stats")]
        [SerializeField, ReadOnly]
        public float manoeuvrability = 1;
        [SerializeField]
        float maxManoeuvrability = 3;

#if UNITY_EDITOR
        void OnValidate() {
            manoeuvrability = yawSpeed / maxManoeuvrability;
        }
#endif
    }
}
