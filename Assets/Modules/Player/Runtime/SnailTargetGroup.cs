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
            group.m_Targets[0].radius = snail.currentSpeed * radiusMultiplier;
            group.m_Targets[1].radius = snail.currentSpeed * radiusMultiplier;
        }
    }
}
