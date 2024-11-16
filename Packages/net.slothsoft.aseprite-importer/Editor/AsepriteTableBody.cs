using UnityEngine.UIElements;

namespace CursedBroom.Aseprite.Editor {
    sealed class AsepriteTableBody : VisualElement {
        public AsepriteTableBody(AssetList<AsepriteContainer> assets) {
            style.flexGrow = 0.7f;

            var scrollView = new ScrollView();
            foreach (var entry in assets.entries) {
                scrollView.Add(new AsepriteTableBodyRow(entry));
            }

            Add(scrollView);
        }
    }
}
