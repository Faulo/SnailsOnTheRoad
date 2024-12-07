using System.Collections.Generic;
using System.Linq;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SotR.Player {
    public sealed class SnailController : MonoBehaviour {
        [SerializeField]
        Rigidbody2D attachedRigidbody;
        [SerializeField]
        Collider2D attachedCollider;
        [SerializeField]
        Animator attachedAnimator;

        [SerializeField, Expandable]
        InputModel input;

        [SerializeField, Expandable]
        public SnailModel model;

        public float currentYaw {
            get => attachedRigidbody.rotation;
            set => attachedRigidbody.MoveRotation(value);
        }

        public Vector2 currentVelocity {
            get => attachedRigidbody.velocity;
            set => attachedRigidbody.velocity = value;
        }

        public float currentSpeed => currentVelocity.magnitude;

        [SerializeField]
        float maximumSpeed = 150;

        public float currentSpeedNormalized => currentSpeed / maximumSpeed;

        public bool isInShell => model.isInShell;

        float currentDrag {
            get => attachedRigidbody.drag;
            set => attachedRigidbody.drag = value;
        }

        PhysicsMaterial2D currentMaterial {
            get => attachedRigidbody.sharedMaterial;
            set => attachedRigidbody.sharedMaterial = value;
        }

        void OnEnable() {
            model.ResetRuntime(currentMaterial);
        }

        void OnDisable() {
            model.ResetRuntime(default);
        }

        float shellTimer = 0;

        [SerializeField]
        LayerMask groundLayers = new();
        static Collider2D[] overlapColliders = new Collider2D[8];
        static int overlapCount = 0;

        void Start() {
            input.intendedDirection = transform.up;
            currentMaterial = new PhysicsMaterial2D(nameof(SnailController));
        }

        void FixedUpdate() {
            UpdateGround();

            UpdateProfiles();

            UpdateEffectors();

            UpdateShell();

            UpdateAnimator();

            UpdateRotation();

            UpdateBoost();

            UpdatePhysics();
        }

        void UpdateGround() {
            overlapCount = Physics2D.OverlapPointNonAlloc(attachedRigidbody.position, overlapColliders, groundLayers);
            if (overlapCount > 0) {
                model.ground = overlapColliders
                    .Take(overlapCount)
                    .OrderBy(c => c.transform.position.z)
                    .Select(c => c.sharedMaterial)
                    .First();
            }
        }

        void UpdateEffectors() {
            foreach (var effector in effectors.Keys) {
                effector.EffectSnail(this);
            }
        }

        void UpdateShell() {
            if (shellTimer > 0) {
                shellTimer -= Time.deltaTime;
            } else {
                if (model.isInShell != input.intendsShell) {
                    model.isInShell = input.intendsShell;
                    shellTimer = model.shellCooldown;
                }
            }
        }

        void UpdateProfiles() {
            foreach (var profile in model.knownProfiles) {
                model.LoseProfile(profile, Time.deltaTime * currentSpeed);
            }
        }

        void UpdateAnimator() {
            attachedAnimator.SetBool(nameof(model.isInShell), model.isInShell);
        }

        void UpdateRotation() {
            float oldYaw = currentYaw;
            float newYaw = Mathf.SmoothDampAngle(oldYaw, input.intendedYaw, ref model.yawVelocity, model.yawSmoothTime);

            var oldVelocity = currentVelocity;
            var newVelocity = (Quaternion.Euler(0, 0, newYaw - oldYaw) * oldVelocity.SwizzleXY()).SwizzleXY();

            currentYaw = newYaw;
            currentVelocity = Vector2.Lerp(oldVelocity, newVelocity, model.friction);
        }

        void UpdateBoost() {
            if (input.intendsBoost) {
                currentVelocity += model.boostStep * transform.up.SwizzleXY();
            }
        }

        void UpdatePhysics() {
            currentDrag = model.drag;

            currentMaterial.friction = model.friction;
            currentMaterial.bounciness = model.bounciness;
        }

        Dictionary<ISnailEffector, int> effectors = new();

        void OnTriggerEnter2D(Collider2D collider) {
            if (collider.TryGetComponent<ISnailEffector>(out var effector)) {
                if (effectors.TryGetValue(effector, out int count)) {
                    effectors[effector] = count + 1;
                } else {
                    effectors.Add(effector, 1);
                    effector.EnterSnail(this);
                }
            }
        }

        void OnTriggerExit2D(Collider2D collider) {
            if (collider.TryGetComponent<ISnailEffector>(out var effector)) {
                if (effectors.TryGetValue(effector, out int count)) {
                    if (count == 1) {
                        effectors.Remove(effector);
                        effector.ExitSnail(this);
                    } else {
                        effectors[effector] = count - 1;
                    }
                }
            }
        }

        /// <summary>
        /// <seealso href="https://discussions.unity.com/t/can-i-make-2d-physics-bounce-the-same-as-the-multiply-bounce-combine-mode-in-3d-physics/673281/9"/>
        /// </summary>
        /// <param name="other"></param>
        void OnCollisionEnter2D(Collision2D collision) {
            float myBounciness = GetBounciness(attachedRigidbody, attachedCollider);
            float theirBounciness = GetBounciness(collision.rigidbody, collision.collider);

            // Unity already collided by using the maximum bounciness, so if the average is lower, we gotta reduce it.
            if (Mathf.Approximately(myBounciness, theirBounciness)) {
                return;
            }

            float average = (myBounciness + theirBounciness) * 0.5f;
            float maximum = Mathf.Max(myBounciness, theirBounciness);

            attachedRigidbody.velocity *= average / maximum;
        }

        static float GetBounciness(Rigidbody2D rigidbody, Collider2D collider) {
            if (rigidbody is { sharedMaterial: { bounciness: float rigidbodyBounciness } }) {
                return rigidbodyBounciness;
            }

            if (collider is { sharedMaterial: { bounciness: float colliderBounciness } }) {
                return colliderBounciness;
            }

            return 0.5f;
        }
    }
}
