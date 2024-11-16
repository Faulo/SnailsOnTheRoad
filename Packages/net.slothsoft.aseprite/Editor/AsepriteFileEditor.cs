using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Editor {
    [CustomEditor(typeof(AsepriteFile))]
    sealed class AsepriteFileEditor : UnityEditor.Editor {
        bool TryGetSheet(out Texture2D sheet) {
            sheet = target is AsepriteFile aseprite
                ? aseprite.albedo
                : default;
            return sheet;
        }
        public override Texture2D RenderStaticPreview(string assetPath, UnityObject[] subAssets, int width, int height) {
            return TryGetSheet(out var sheet)
                ? Instantiate(sheet)
                : base.RenderStaticPreview(assetPath, subAssets, width, height);
        }

        public override bool HasPreviewGUI() => TryGetSheet(out _);

        public override void OnPreviewGUI(Rect position, GUIStyle background) {
            if (TryGetSheet(out var sheet)) {
                EditorGUI.DrawTextureTransparent(position, sheet, ScaleMode.ScaleToFit);
            } else {
                base.OnPreviewGUI(position, background);
            }
        }
    }
}
