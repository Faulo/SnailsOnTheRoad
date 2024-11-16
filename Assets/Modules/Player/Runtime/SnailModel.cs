using System;
using UnityEngine;

namespace SotR.Player {
    [CreateAssetMenu]
    public sealed class SnailModel : ScriptableObject {
        [Serializable]
        sealed class SnailConfig {
            [SerializeField]
            internal float yawSmoothTime = 0.2f;
            [SerializeField]
            internal float boostMultiplier = 1;
            [SerializeField]
            internal float drag = 1;
            [SerializeField]
            internal PhysicsMaterial2D material;
        }

        [Header("Config")]
        [SerializeField]
        SnailConfig defaultConfig = new();
        [SerializeField]
        SnailConfig inShellConfig = new();
        [SerializeField]
        internal float shellCooldown = 0.1f;

        internal float yawSmoothTime => isInShell
            ? inShellConfig.yawSmoothTime
            : defaultConfig.yawSmoothTime;

        internal float boostMultiplier => isInShell
            ? inShellConfig.boostMultiplier
            : defaultConfig.boostMultiplier;

        internal float drag => isInShell
            ? inShellConfig.drag
            : defaultConfig.drag;

        internal PhysicsMaterial2D material => isInShell
            ? inShellConfig.material
            : defaultConfig.material;

        internal void ResetRuntime(PhysicsMaterial2D ground) {
            this.ground = ground;
            boostStep = 0;
            velocity = Vector2.zero;
            yawVelocity = 0;
            isInShell = false;
        }

        [Header("Runtime")]
        [SerializeField]
        internal PhysicsMaterial2D ground;

        internal float frictionMultiplier => ground.friction;

        [SerializeField]
        internal float boostStep;

        [SerializeField]
        internal Vector2 velocity = Vector2.zero;

        [SerializeField]
        internal float yawVelocity;

        [SerializeField]
        internal bool isInShell;
    }
}
