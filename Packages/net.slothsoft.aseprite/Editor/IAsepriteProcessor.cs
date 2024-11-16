using System.Collections.Generic;
using System.IO;
using UnityObject = UnityEngine.Object;

namespace Slothsoft.Aseprite.Editor {
    public interface IAsepriteProcessor {
        string key { get; }
        IEnumerable<(string key, UnityObject asset)> CreateAssets(FileInfo asepriteFile, AsepriteData info, AsepritePalette palette);
    }
}
