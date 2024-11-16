using System;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Editor {
    sealed class AssetListEntry<T> where T : UnityObject {
        public Action onChange;

        public AssetListEntry(T asset, string path) {
            this.asset = asset;
            this.path = path;

            isChecked = false;
            isVisible = true;
        }

        public readonly T asset;
        public readonly string path;

        public bool isChecked {
            get => m_isChecked;
            set {
                if (m_isChecked != value) {
                    m_isChecked = value;
                    onChange?.Invoke();
                }
            }
        }
        bool m_isChecked;

        public bool isVisible {
            get => m_isVisible;
            set {
                if (m_isVisible != value) {
                    m_isVisible = value;
                    onChange?.Invoke();
                }
            }
        }
        bool m_isVisible;
    }
}
