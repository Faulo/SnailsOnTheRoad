using Cinemachine;
using UnityEngine;

namespace SotR.Player {
    sealed class SnailTargetGroup : MonoBehaviour {
        [SerializeField]
        CinemachineTargetGroup group;
        [SerializeField]
        SnailController snail;
        [SerializeField]
        float radiusMultiplier = 1;

        void Update() {
            for (int i = 0; i < group.m_Targets.Length; i++) {
                group.m_Targets[i].radius = snail.currentSpeed * radiusMultiplier;
            }
        }
    }
}
