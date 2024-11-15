using MyBox;
using Slothsoft.UnityExtensions;
using UnityEngine;

namespace SitS.Player {
    [CreateAssetMenu]
    public sealed class PlayerModel : ScriptableObject {
        [SerializeField]
        internal GameObject prefab;

        internal GameObject instance;

        internal void SpawnIfNotSpawnedAlready(Transform transform) {
            if (!prefab) {
                return;
            }

            if (instance) {
                return;
            }

            instance = Instantiate(prefab, transform);
        }

        [SerializeField, Expandable]
        public PlaneModel plane;

        [SerializeField]
        float _health = 1;
        internal float health {
            get => _health;
            set => _health = Mathf.Clamp(value, 0, plane.maxHealth);
        }

        public float normalizedHealth => health / plane.maxHealth;

        public bool isAlive => health > plane.minHealth;

        internal bool canBoost => _health > 0;

        [SerializeField]
        internal float alignment;

        [SerializeField]
        bool _isBoosting = false;
        public bool isBoosting {
            get => _isBoosting;
            internal set => _isBoosting = value;
        }

        [SerializeField]
        internal float burnSpeed = 1;

        [SerializeField]
        float _leftBrake = 0;
        public float leftBrake {
            get => _leftBrake;
            internal set => _leftBrake = Mathf.Clamp01(value);
        }

        [SerializeField]
        float _rightBrake = 0;
        public float rightBrake {
            get => _rightBrake;
            internal set => _rightBrake = Mathf.Clamp01(value);
        }

        [Header("Collectibles")]
        [SerializeField]
        public uint collectiblesCollected = 0;

        [SerializeField]
        public uint collectiblesMax = 10;

        public bool collectedEverything => collectiblesCollected == collectiblesMax;

        [Header("Death")]
        [SerializeField]
        float _deadTime = 0;
        internal float deadTime {
            get => _deadTime;
            set {
                _deadTime = value;
                if (_deadTime >= maxDeadTime) {
                    deathScene.LoadScene();
                }
            }
        }
        [SerializeField]
        float maxDeadTime = 1;
        [SerializeField]
        SceneReference deathScene = new();
    }
}
