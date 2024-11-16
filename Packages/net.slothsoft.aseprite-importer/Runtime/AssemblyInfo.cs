using System.Runtime.CompilerServices;
using CursedBroom.Aseprite;

[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_EDITOR)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS)]

namespace CursedBroom.Aseprite {
    static class AssemblyInfo {
        public const string NAMESPACE_RUNTIME = "CursedBroom.Aseprite";
        public const string NAMESPACE_EDITOR = "CursedBroom.Aseprite.Editor";
        public const string NAMESPACE_TESTS = "CursedBroom.Aseprite.Tests";
    }
}
