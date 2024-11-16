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

        [Header("Runtime")]
        [SerializeField]
        internal Vector2 boostStep;

        [SerializeField]
        internal Vector2 velocity = Vector2.zero;

        [SerializeField]
        internal float yawVelocity;

        [Space]
        [SerializeField]
        internal bool isInShell;
    }
}
