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

        Vector3 velocity;

        void Update() {
            var target = controller.transform.position + (distance * speedToLook.Evaluate(controller.currentSpeedNormalized) * controller.transform.up);
            transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
        }
    }
}
