using UnityEditor;

namespace Slothsoft.Aseprite.Editor {
    sealed class AsepriteAssetListEditor : EditorWindow {
        const string ASSET_FOLDER = "Assets/Art";

        [MenuItem("Window/Cursed Broom/Aseprite Asset List")]
        public static void ShowWindow() {
            GetWindow<AsepriteAssetListEditor>("Aseprite Asset List");
        }

        void OnEnable() {
            var assets = SearchForAssets();

            rootVisualElement.Add(new AsepriteTableHead(assets));
            rootVisualElement.Add(new AsepriteTableBody(assets));
        }

        static AssetList<AsepriteContainer> SearchForAssets() {
            var assets = new AssetList<AsepriteContainer>();

            string[] assetGuids = AssetDatabase.FindAssets($"t:{typeof(AsepriteContainer).Name}", new[] { ASSET_FOLDER });
            foreach (string assetGuid in assetGuids) {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (AssetDatabase.LoadAssetAtPath<AsepriteContainer>(assetPath) is AsepriteContainer asset) {
                    assets.Add(asset, assetPath);
                }
            }

            return assets;
        }
    }
}
