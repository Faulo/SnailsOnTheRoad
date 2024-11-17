using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SotR.Player {
    sealed class SnailTrail : MonoBehaviour {
        [SerializeField, Expandable]
        SnailController _snail;
        [SerializeField]
        ParticleSystem _particleSystem;

        ParticleSystem.MainModule _main;

        [SerializeField]
        float _emissionMultiplier = 100;

        [SerializeField]
        float _defaultEmission = 1;

        void Start() {
            _main = _particleSystem.main;
        }

        void Update() {
            EmitParticles(Time.deltaTime * _emissionMultiplier * _snail.currentSpeed);
        }

        void EmitParticles(float multiplier) {
            EmitParticlesForProfile(_snail.model.defaultProfile, _defaultEmission * multiplier);

            foreach (var (profile, size) in _snail.model.profiles) {
                EmitParticlesForProfile(profile, size * multiplier);
            }
        }

        void EmitParticlesForProfile(ProfileModel profile, float size) {
            _main.startColor = profile.color;
            _particleSystem.Emit(Mathf.RoundToInt(size));
        }
    }
}
