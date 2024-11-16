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

        void Start() {
            player.health = snail.maxHealth;
            player.isBoosting = false;
            player.deadTime = 0;
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
                ? Time.deltaTime * snail.moveSpeed * transform.forward
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

            attachedRigidbody.drag = snail.dragMaximum;
            // attachedRigidbody.drag = Mathf.Lerp(snail.dragMaximum, snail.dragMinimum, player.alignment) * snail.area * player.normalizedHealth;
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

            if (Mathf.Approximately(attachedRigidbody.velocity.sqrMagnitude, 0)) {
                ProcessDeathTimeout();
            }
        }

        void ProcessDeathTimeout() {
            player.deadTime += Time.deltaTime;
        }
    }
}
