using System.Collections.Generic;
using UnityEngine;

namespace SotR.Game {
    sealed class LevelObject : MonoBehaviour {
        [SerializeField]
        SpriteRenderer _renderer;

        [SerializeField]
        PolygonCollider2D _collider;

        [SerializeField]
        float _tolerance = 0.05f;

        readonly List<Vector2> _points = new();
        readonly List<Vector2> _simplifiedPoints = new();

        void UpdatePolygonCollider2D() {
            if (!_renderer && !TryGetComponent(out _renderer)) {
                return;
            }

            if (!_collider && !TryGetComponent(out _collider)) {
                return;
            }

            if (this is { _renderer: { sprite: Sprite sprite }, _collider: PolygonCollider2D collider }) {
                collider.pathCount = sprite.GetPhysicsShapeCount();
                Debug.Log(collider.pathCount);

                for (int i = 0; i < collider.pathCount; i++) {
                    sprite.GetPhysicsShape(i, _points);
                    LineUtility.Simplify(_points, _tolerance, _simplifiedPoints);
                    collider.SetPath(i, _simplifiedPoints);
                }
            }
        }

        void Start() {
            UpdatePolygonCollider2D();
        }

#if UNITY_EDITOR
        void OnValidate() {
            UpdatePolygonCollider2D();
        }
#endif
    }
}
