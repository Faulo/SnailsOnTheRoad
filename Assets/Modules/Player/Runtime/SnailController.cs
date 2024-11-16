using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SotR.Player {
    [ExecuteAlways]
    sealed class SnailController : MonoBehaviour {

        [SerializeField, Expandable]
        InputModel input;

        [SerializeField, Expandable]
        SnailModel snail;

        [Space]
        [SerializeField]
        Rigidbody attachedRigidbody;

        [SerializeField]
        Vector3 moveStep;

        [SerializeField]
        Vector3 boostStep;

        [SerializeField]
        Vector3 velocity = Vector3.zero;

        [SerializeField]
        Vector2 intendedDirection;

        [SerializeField]
        float intendedYaw;

        [SerializeField]
        float currentYaw;

        [SerializeField]
        float yawGap;

        [SerializeField]
        float yawSpeedStrength;

        [SerializeField]
        float deltaYaw;

        void FixedUpdate() {
            if (!Application.isPlaying) {
                return;
            }

            // -- only for debugging
            intendedDirection = input.intendedDirection;
            intendedYaw = input.intendedYaw;
            // --

            // ---------------
            // yaw update

            currentYaw = Vector3.SignedAngle(Vector3.forward, attachedRigidbody.transform.forward, Vector3.up);
            yawGap = Mathf.DeltaAngle(currentYaw, input.intendedYaw);
            yawSpeedStrength = yawGap < 3.0f ? Mathf.Abs(yawGap) / 180.0f : 1.0f;
            deltaYaw = snail.yawSpeed * yawSpeedStrength * Mathf.Sign(yawGap);

            var deltaRotation = Quaternion.Euler(0.0f, -deltaYaw, 0.0f);

            //attachedRigidbody.angularVelocity = attachedRigidbody.rotation * new Vector3(0.0f, deltaYaw, 0.0f);
            attachedRigidbody.MoveRotation(Quaternion.Euler(0.0f, input.intendedYaw, 0.0f));

            // ---------------
            // velocity update

            velocity = attachedRigidbody.velocity;
            //velocity = deltaRotation * velocity;

            var direction = velocity.normalized;

            boostStep = input.intendsBoost
                ? Time.deltaTime * snail.boostMultiplier * transform.forward
                : Vector3.zero;

            velocity += boostStep;

            attachedRigidbody.velocity = velocity;
        }
    }
}
