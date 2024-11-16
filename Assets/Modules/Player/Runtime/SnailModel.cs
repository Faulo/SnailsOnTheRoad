using MyBox;
using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    public sealed class SnailModel : ScriptableObject {
        [Space]
        [SerializeField]
        internal float yawSpeed = 1;

        [Space]
        [SerializeField]
        internal GameObject meshPrefab;
        [SerializeField]
        internal float dragMinimum = 0;
        [SerializeField]
        internal float dragMaximum = 5;
        [SerializeField]
        internal float dragBrakeMultiplier = 1;
        [SerializeField]
        internal float liftCoefficient = 1;
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

        [SerializeField, ReadOnly]
        public float lift = 1;
        [SerializeField]
        float maxLift = 1;

        [SerializeField, ReadOnly]
        public float size = 1;
        [SerializeField]
        float maxSize = 5;

        [SerializeField, ReadOnly]
        public float nitro = 1;
        [SerializeField]
        float maxNitro = 3;

#if UNITY_EDITOR
        void OnValidate() {
            manoeuvrability = yawSpeed / maxManoeuvrability;
            lift = liftCoefficient / maxLift;
            size = maxHealth / maxSize;
            nitro = boostMultiplier / maxNitro;
        }
#endif
    }
}
