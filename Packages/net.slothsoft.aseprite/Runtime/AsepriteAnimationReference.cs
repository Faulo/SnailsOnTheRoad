using System;
using UnityEngine;

namespace Slothsoft.Aseprite {
    [Serializable]
    public sealed record AsepriteAnimationReference {
        [SerializeField]
        public AsepriteFile source;
        [SerializeField]
        public string animation;

        public AsepriteAnimation Resolve() {
            var resolved = source.GetAnimation(animation);
#if UNITY_EDITOR
            if (!resolved.isValid) {
                Debug.LogWarning($"Aseprite file {source} does not contain an animation '{animation}'!");
            }
#endif
            return resolved;
        }

        public override string ToString() {
            return animation;
        }
    }
}
