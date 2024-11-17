using System;
using System.Collections.Generic;
using System.Linq;
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
            [SerializeField]
            internal ProfileModel profile;
        }

        [Header("Config")]
        [SerializeField]
        SnailConfig defaultConfig = new();
        [SerializeField]
        SnailConfig inShellConfig = new();
        [SerializeField]
        internal float shellCooldown = 0.1f;
        [SerializeField]
        float profileMaximum = 1;
        [SerializeField]
        internal ProfileModel defaultProfile => isInShell
            ? inShellConfig.profile
            : defaultConfig.profile;

        internal float yawSmoothTime => isInShell
            ? inShellConfig.yawSmoothTime
            : defaultConfig.yawSmoothTime;

        float boostMultiplier => isInShell
            ? inShellConfig.boostMultiplier * profileBoostMultiplier
            : defaultConfig.boostMultiplier * profileBoostMultiplier;

        internal float boostStep => boostMultiplier * friction * Time.deltaTime;

        internal float drag => isInShell
            ? inShellConfig.drag * profileDragMultiplier
            : defaultConfig.drag * profileDragMultiplier;

        internal float friction => isInShell
            ? inShellConfig.material.friction * ground.friction * profileFrictionMultiplier
            : defaultConfig.material.friction * ground.friction * profileFrictionMultiplier;

        internal float bounciness => isInShell
            ? inShellConfig.material.bounciness
            : defaultConfig.material.bounciness;

        internal void ResetRuntime(PhysicsMaterial2D ground) {
            this.ground = ground;
            velocity = Vector2.zero;
            yawVelocity = 0;
            isInShell = false;
            knownProfiles.Clear();
            profiles.Clear();
        }

        [Header("Runtime")]
        [SerializeField]
        internal PhysicsMaterial2D ground;

        [SerializeField]
        internal Vector2 velocity = Vector2.zero;

        [SerializeField]
        internal float yawVelocity;

        [SerializeField]
        internal bool isInShell;

        internal readonly HashSet<ProfileModel> knownProfiles = new();
        internal readonly Dictionary<ProfileModel, float> profiles = new();

        float profileBoostMultiplier => profiles
            .Keys
            .Select(p => p.boostMultiplier)
            .Aggregate(1f, (a, b) => a * b);

        float profileDragMultiplier => profiles
            .Keys
            .Select(p => p.dragMultiplier)
            .Aggregate(1f, (a, b) => a * b);

        float profileFrictionMultiplier => profiles
            .Keys
            .Select(p => p.frictionMultiplier)
            .Aggregate(1f, (a, b) => a * b);

        public void AddProfile(ProfileModel profile, float gain) {
            if (profiles.TryGetValue(profile, out float value)) {
                profiles[profile] = Mathf.Min(profileMaximum, value + gain);
            } else {
                profiles.Add(profile, Mathf.Min(profileMaximum, value));
            }

            knownProfiles.Add(profile);
        }

        public void LoseProfile(ProfileModel profile, float loss) {
            if (profiles.TryGetValue(profile, out float value)) {
                if (loss >= value) {
                    profiles.Remove(profile);
                } else {
                    profiles[profile] = value - loss;
                }
            }
        }
    }
}
