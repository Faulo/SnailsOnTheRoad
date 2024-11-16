using UnityEngine.UIElements;

namespace CursedBroom.Aseprite.Editor {
    sealed class AsepriteTableHead : Box {

        readonly AssetList<AsepriteContainer> assets;

        public AsepriteTableHead(AssetList<AsepriteContainer> assets) {
            this.assets = assets;

            style.flexDirection = FlexDirection.Row;

            var allToggle = new Toggle();
            allToggle.RegisterValueChangedCallback(OnAllToggleValueChanged);
            Add(allToggle);

            var searchBar = new TextField("Search");
            searchBar.RegisterValueChangedCallback(OnSearchQueryChanged);
            searchBar.style.flexGrow = 0.5f;
            Add(searchBar);
        }

        void OnAllToggleValueChanged(ChangeEvent<bool> evt) {
            assets.SetAllChecked(evt.newValue);
        }

        void OnSearchQueryChanged(ChangeEvent<string> evt) {
            assets.SetEachVisibleIfNameContains(evt.newValue);
        }
    }
}
