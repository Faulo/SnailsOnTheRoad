using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SotR.Player {
    sealed class SnailController : MonoBehaviour {
        [SerializeField]
        Rigidbody2D attachedRigidbody;
        [SerializeField]
        Animator attachedAnimator;

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

        void OnEnable() {
            snail.ResetRuntime(currentMaterial);
        }

        void OnDisable() {
            snail.ResetRuntime(default);
        }

        float shellTimer = 0;

        [SerializeField]
        LayerMask groundLayers = new();
        static Collider2D[] overlapColliders = new Collider2D[8];
        static int overlapCount = 0;

        void FixedUpdate() {
            UpdateGround();

            UpdateSnail();

            UpdateAnimator();

            UpdatePhysics();
        }

        void UpdateSnail() {
            if (shellTimer > 0) {
                shellTimer -= Time.deltaTime;
            } else {
                if (snail.isInShell != input.intendsShell) {
                    snail.isInShell = input.intendsShell;
                    shellTimer = snail.shellCooldown;
                }
            }

            snail.boostStep = snail.boostMultiplier * snail.frictionMultiplier;
        }

        void UpdateAnimator() {
            attachedAnimator.SetBool(nameof(snail.isInShell), snail.isInShell);
        }

        void UpdateGround() {
            overlapCount = Physics2D.OverlapPointNonAlloc(attachedRigidbody.position, overlapColliders, groundLayers);
            for (int i = 0; i < overlapCount; i++) {
                if (overlapColliders[i].sharedMaterial) {
                    snail.ground = overlapColliders[i].sharedMaterial;
                }
            }
        }

        void UpdatePhysics() {
            currentYaw = Mathf.SmoothDampAngle(currentYaw, input.intendedYaw, ref snail.yawVelocity, snail.yawSmoothTime);

            if (input.intendsBoost) {
                currentVelocity += Time.deltaTime * snail.boostStep * transform.up.SwizzleXY();
            }

            currentDrag = snail.drag;

            currentMaterial = snail.material;
        }
    }
}
