using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SotR.Player {
    [ExecuteAlways]
    sealed class SnailController : MonoBehaviour {

        [SerializeField, Expandable]
        InputModel input;
        [SerializeField, Expandable]
        PlayerModel player;

        SnailModel snail => player.snail;

        [Space]
        [SerializeField]
        Rigidbody attachedRigidbody;

        GameObject modelPrefab => snail
            ? snail.meshPrefab
            : null;
        GameObject modelInstance => transform.childCount > 0
            ? transform.GetChild(0).gameObject
            : null;

        void Start() {
            player.health = snail.maxHealth;
            player.isBoosting = false;
            player.leftBrake = 0;
            player.rightBrake = 0;
            player.deadTime = 0;

            RecreateModel();
        }

#if UNITY_EDITOR
        void Update() {
            RecreateModel();
        }
#endif

        void RecreateModel() {
            if (modelPrefab) {
                if (modelInstance && modelInstance.name != modelPrefab.name) {
                    DestroyModel();
                }

                if (!modelInstance) {
                    CreateModel();
                }
            } else {
                DestroyModel();
            }
        }

        void DestroyModel() {
            if (modelInstance) {
                if (Application.isPlaying) {
                    Destroy(modelInstance);
                } else {
                    DestroyImmediate(modelInstance);
                }
            }
        }

        void CreateModel() {
#if UNITY_EDITOR
            if (Application.isPlaying) {
#endif
                var instance = Instantiate(modelPrefab, transform);
                instance.name = modelPrefab.name;
#if UNITY_EDITOR
            } else {
                var instance = UnityEditor.PrefabUtility.InstantiatePrefab(modelPrefab, transform);
                instance.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
                instance.name = modelPrefab.name;
            }
#endif
        }

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

            //if (!player.isAlive || transform.position.y < 0) {
            //    ProcessDeath();
            //    return;
            //}

            // -- only for debugging
            intendedDirection = input.intendedDirection;
            intendedYaw = input.intendedYaw();
            // --

            player.leftBrake = input.intendedLeftBrake;
            player.rightBrake = input.intendedRightBrake;

            currentYaw = Vector3.SignedAngle(Vector3.forward, attachedRigidbody.transform.forward, Vector3.up);
            yawGap = Mathf.DeltaAngle(currentYaw, input.intendedYaw());
            yawSpeedStrength = yawGap < 3.0f ? Mathf.Abs(yawGap) / 180.0f : 1.0f;
            deltaYaw = snail.yawSpeed * yawSpeedStrength * Mathf.Sign(yawGap);

            var deltaRotation = Quaternion.Euler(0.0f, -deltaYaw, 0.0f);

            attachedRigidbody.angularVelocity = attachedRigidbody.rotation * new Vector3(0.0f, deltaYaw, 0.0f);

            velocity = attachedRigidbody.velocity;
            velocity = deltaRotation * velocity;

            var direction = velocity.normalized;

            ProcessAlignment(direction);
            ProcessBoost();

            moveStep = input.intendedDirection != Vector3.zero
                ? Time.deltaTime * 2.0f * transform.forward
                : Vector3.zero;

            boostStep = player.isBoosting
                ? Time.deltaTime * snail.boostMultiplier * transform.forward
                : Vector3.zero;

            velocity += moveStep + boostStep;

            attachedRigidbody.velocity = velocity;
        }

        void ProcessAlignment(in Vector3 direction) {
            float dot = direction == Vector3.zero
                ? 0
                : Vector3.Dot(transform.forward, direction);

            player.alignment = Mathf.InverseLerp(0, 1, dot);

            attachedRigidbody.drag = Mathf.Lerp(snail.dragMaximum, snail.dragMinimum, player.alignment) * snail.area; // * player.normalizedHealth
            attachedRigidbody.drag += player.leftBrake * snail.dragBrakeMultiplier;
            attachedRigidbody.drag += player.rightBrake * snail.dragBrakeMultiplier;
        }

        void ProcessBoost() {
            player.isBoosting = input.intendsBoost; // && player.canBoost

            if (player.isBoosting) {
                player.health -= Mathf.Clamp01(Time.deltaTime * player.burnSpeed);
            }
        }

        void ProcessDeath() {
            player.health -= Mathf.Clamp01(Time.deltaTime * player.burnSpeed);
            player.isBoosting = false;
            player.leftBrake = 0;
            player.rightBrake = 0;

            if (Mathf.Approximately(attachedRigidbody.velocity.sqrMagnitude, 0)) {
                ProcessDeathTimeout();
            }
        }

        void ProcessDeathTimeout() {
            player.deadTime += Time.deltaTime;
        }
    }
}
