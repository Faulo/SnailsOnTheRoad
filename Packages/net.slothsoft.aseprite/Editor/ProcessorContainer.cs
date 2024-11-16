using System;
using UnityEngine;

namespace Slothsoft.Aseprite.Editor {
    [Serializable]
    sealed class ProcessorContainer {
        [SerializeReference]
        internal IAsepriteProcessor[] processors = Array.Empty<IAsepriteProcessor>();
    }
}
