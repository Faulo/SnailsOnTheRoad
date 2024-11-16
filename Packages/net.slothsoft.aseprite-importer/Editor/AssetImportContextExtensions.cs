using UnityEditor;
using UnityEditor.AssetImporters;
using UnityObject = UnityEngine.Object;

namespace CursedBroom.Core.Editor {
    public static class AssetImportContextExtensions {
        public static bool TryDependOnArtifact<T>(this AssetImportContext context, string path, out T artifact)
            where T : UnityObject {
            if (string.IsNullOrEmpty(path)) {
                artifact = default;
                return false;
            }

            context.DependsOnArtifact(path);
            artifact = AssetDatabase.LoadAssetAtPath<T>(path);
            return artifact;
        }
    }
}
