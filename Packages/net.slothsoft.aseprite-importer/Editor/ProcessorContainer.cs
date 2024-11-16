using System;
using UnityEngine;

namespace CursedBroom.Aseprite.Editor {
    [Serializable]
    sealed class ProcessorContainer {
        [SerializeReference]
        internal IAsepriteProcessor[] processors = Array.Empty<IAsepriteProcessor>();
    }
}
