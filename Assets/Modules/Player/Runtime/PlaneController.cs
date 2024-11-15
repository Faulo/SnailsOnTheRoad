using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SitS.Player {
    [ExecuteAlways]
    sealed class PlaneController : MonoBehaviour {

        [SerializeField, Expandable]
        InputModel input;
        [SerializeField, Expandable]
        PlayerModel player;

        PlaneModel plane => player.plane;

        [Space]
        [SerializeField]
        Rigidbody attachedRigidbody;

        GameObject modelPrefab => plane
            ? plane.meshPrefab
            : null;
        GameObject modelInstance => transform.childCount > 0
            ? transform.GetChild(0).gameObject
            : null;

        void Start() {
            player.health = plane.maxHealth;
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
        Vector3 boostStep;

        [SerializeField]
        Vector3 liftStep;

        [SerializeField]
        Vector3 velocity = Vector3.zero;

        void FixedUpdate() {
            if (!Application.isPlaying) {
                return;
            }

            if (!player.isAlive || transform.position.y < 0) {
                ProcessDeath();
                return;
            }

            player.leftBrake = input.intendedLeftBrake;
            player.rightBrake = input.intendedRightBrake;

            float deltaYaw = plane.yawSpeed * input.intendedYaw;
            float deltaPitch = plane.pitchSpeed * input.intendedPitch;
            float deltaRoll = plane.rollSpeed * input.intendedRoll;

            var deltaRotation = Quaternion.Euler(-deltaYaw, deltaPitch, -deltaRoll);

            attachedRigidbody.angularVelocity = attachedRigidbody.rotation * new Vector3(deltaYaw, deltaPitch, -deltaRoll);

            velocity = attachedRigidbody.velocity;

            velocity = deltaRotation * velocity;

            var direction = velocity.normalized;

            ProcessAlignment(direction);

            ProcessBoost();

            boostStep = player.isBoosting
                ? Time.deltaTime * plane.boostMultiplier * transform.forward
                : Vector3.zero;

            liftStep = velocity == Vector3.zero
                ? Vector3.zero
                : Time.deltaTime * plane.liftCoefficient * plane.area * player.normalizedHealth * velocity.sqrMagnitude * player.alignment * transform.up;

            velocity += boostStep + liftStep;

            attachedRigidbody.velocity = velocity;
        }

        void ProcessAlignment(in Vector3 direction) {
            float dot = direction == Vector3.zero
                ? 0
                : Vector3.Dot(transform.forward, direction);

            player.alignment = Mathf.InverseLerp(0, 1, dot);

            attachedRigidbody.drag = Mathf.Lerp(plane.dragMaximum, plane.dragMinimum, player.alignment) * plane.area * player.normalizedHealth;
            attachedRigidbody.drag += player.leftBrake * plane.dragBrakeMultiplier;
            attachedRigidbody.drag += player.rightBrake * plane.dragBrakeMultiplier;
        }

        void ProcessBoost() {
            player.isBoosting = input.intendsBoost && player.canBoost;

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
