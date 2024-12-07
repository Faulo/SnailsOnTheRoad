using UnityEngine;

namespace SotR.Player {
    [ExecuteAlways]
    sealed class SnailLookAhead : MonoBehaviour {
        [SerializeField]
        SnailController controller;

        [SerializeField]
        float distance = 10;

        [SerializeField]
        AnimationCurve speedToLook = new();

        [SerializeField]
        float smoothTime = 1;

        Vector3 targetPosition => controller.transform.position + (distance * speedToLook.Evaluate(controller.currentSpeedNormalized) * controller.transform.up);

        Vector3 velocity;

        void Update() {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

#if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = new Color(255, 255, 0, 0.5f);
            Gizmos.DrawLine(controller.transform.position, targetPosition);
            Gizmos.color = new Color(255, 127, 0, 0.5f);
            Gizmos.DrawLine(controller.transform.position, transform.position);
            Gizmos.color = new Color(255, 0, 0, 0.5f);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
#endif
    }
}
