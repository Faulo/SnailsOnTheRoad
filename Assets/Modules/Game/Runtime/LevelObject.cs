using System.Collections.Generic;
using UnityEngine;

namespace SotR.Game {
    [ExecuteAlways]
    sealed class LevelObject : MonoBehaviour {
        [SerializeField]
        SpriteRenderer _renderer;

        [SerializeField]
        PolygonCollider2D _collider;

        [SerializeField, Range(0.001f, 10)]
        float _tolerance = 0.05f;

        record SpriteHash(Sprite sprite, float tolerance, bool flipX, bool flipY);

        SpriteHash _hash;

        Vector2Int flipMultiplier => new(
            _hash.flipX ? -1 : 1,
            _hash.flipY ? -1 : 1
        );

        [SerializeField]
        Vector2 _forward = Vector2.up;

        public Vector2 forward => (_forward * flipMultiplier).normalized;

        void UpdatePolygonCollider2DIfHashDiffers(PolygonCollider2D collider, SpriteHash hash) {
            if (_hash != hash) {
                _hash = hash;

                var points = new List<Vector2>();
                var simplifiedPoints = new List<Vector2>();

                collider.pathCount = hash.sprite.GetPhysicsShapeCount();

                for (int i = 0; i < collider.pathCount; i++) {
                    hash.sprite.GetPhysicsShape(i, points);

                    for (int j = 0; j < points.Count; j++) {
                        points[j] *= flipMultiplier;
                    }

                    LineUtility.Simplify(points, hash.tolerance, simplifiedPoints);
                    collider.SetPath(i, simplifiedPoints);
                }
            }
        }

        internal void UpdatePolygonCollider2D() {
            if (!_renderer && !TryGetComponent(out _renderer)) {
                return;
            }

            if (!_collider && !TryGetComponent(out _collider)) {
                return;
            }

            if (this is { _renderer: { sprite: Sprite sprite, flipX: bool flipX, flipY: bool flipY }, _collider: PolygonCollider2D collider }) {
                UpdatePolygonCollider2DIfHashDiffers(collider, new(sprite, _tolerance, flipX, flipY));
            }
        }

        void Start() {
            UpdatePolygonCollider2D();
        }

#if UNITY_EDITOR
        [SerializeField]
        bool snapTransformToPixelGrid = true;

        Vector3 position {
            get => transform.position;
            set {
                if (transform.position != value) {
                    transform.position = value;
                    if (!Application.isPlaying) {
                        UnityEditor.EditorUtility.SetDirty(transform);
                    }
                }
            }
        }

        void Update() {
            if (snapTransformToPixelGrid) {
                position = Vector3Int.RoundToInt(position);
            }

            UpdatePolygonCollider2D();
        }
#endif
    }
}
