using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SotR.Player {
    sealed class SnailController : MonoBehaviour {
        [SerializeField]
        Rigidbody2D attachedRigidbody;

        [SerializeField, Expandable]
        InputModel input;

        [SerializeField, Expandable]
        SnailModel snail;

        internal float currentYaw {
            get => attachedRigidbody.rotation;
            set => attachedRigidbody.MoveRotation(value);
        }

        internal Vector2 currentVelocity {
            get => attachedRigidbody.velocity;
            set => attachedRigidbody.velocity = value;
        }

        internal float currentDrag {
            get => attachedRigidbody.drag;
            set => attachedRigidbody.drag = value;
        }

        internal PhysicsMaterial2D currentMaterial {
            get => attachedRigidbody.sharedMaterial;
            set => attachedRigidbody.sharedMaterial = value;
        }


        float shellTimer = 0;
        void FixedUpdate() {
            if (shellTimer > 0) {
                shellTimer -= Time.deltaTime;
            } else {
                if (snail.isInShell != input.intendsShell) {
                    snail.isInShell = input.intendsShell;
                    shellTimer = snail.shellCooldown;
                }
            }

            // ---------------
            // yaw update

            currentYaw = Mathf.SmoothDampAngle(currentYaw, input.intendedYaw, ref snail.yawVelocity, snail.yawSmoothTime);

            // ---------------
            // velocity update

            snail.boostStep = input.intendsBoost
                ? Time.deltaTime * snail.boostMultiplier * transform.up.SwizzleXY()
                : Vector2.zero;

            currentVelocity += snail.boostStep;

            currentDrag = snail.drag;

            currentMaterial = snail.material;
        }
    }
}
