using System.Collections.Generic;
using UnityObject = UnityEngine.Object;

namespace CursedBroom.Aseprite.Editor {
    sealed class AssetList<T> where T : UnityObject {

        public IReadOnlyList<AssetListEntry<T>> entries => m_entries;

        readonly List<AssetListEntry<T>> m_entries = new();

        public void SetAllChecked(bool value) {
            foreach (var entry in m_entries) {
                entry.isChecked = value;
            }
        }

        public void SetEachVisibleIfNameContains(string name) {
            foreach (var entry in m_entries) {
                entry.isVisible = entry.asset.name.ToLower().Contains(name.ToLower());
            }
        }

        internal void Add(T asset, string assetPath) {
            m_entries.Add(new(asset, assetPath));
        }

        internal void Clear() => m_entries.Clear();
    }
}
