using UnityEngine;

namespace CursedBroom.Aseprite {
    [CreateAssetMenu]
    public sealed class ColorAsset : ScriptableObject {
        [SerializeField]
        public Color32 color32 = new(255, 255, 255, 255);
        [SerializeField]
        public Color color = Color.white;

        public static implicit operator Color(ColorAsset asset) => asset.color;
        public static implicit operator Color32(ColorAsset asset) => asset.color32;
    }
}
