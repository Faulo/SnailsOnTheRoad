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
        Rigidbody2D attachedRigidbody;

        [SerializeField]
        Vector2 boostStep;

        [SerializeField]
        Vector2 velocity = Vector2.zero;

        [SerializeField]
        Vector2 intendedDirection;

        [SerializeField]
        float intendedYaw;

        [SerializeField]
        float yawVelocity;

        [SerializeField]
        float yawSmoothTime = 0.1f;

        public float currentYaw {
            get => attachedRigidbody.rotation;
            set => attachedRigidbody.MoveRotation(value);
        }

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

            //attachedRigidbody.MoveRotation(Quaternion.Euler(0.0f, 0.0f, input.intendedYaw));
            //attachedRigidbody.transform.rotation = Quaternion.Euler(0.0f, 0.0f, input.intendedYaw);
            //attachedRigidbody.transform.up = input.intendedDirection.SwizzleXY();

            currentYaw = Mathf.SmoothDampAngle(currentYaw, input.intendedYaw, ref yawVelocity, yawSmoothTime);

            // ---------------
            // velocity update

            velocity = attachedRigidbody.velocity;

            boostStep = input.intendsBoost
                ? Time.deltaTime * snail.boostMultiplier * transform.up.SwizzleXY()
                : Vector2.zero;

            velocity += boostStep;

            attachedRigidbody.velocity = velocity;
        }
    }
}
