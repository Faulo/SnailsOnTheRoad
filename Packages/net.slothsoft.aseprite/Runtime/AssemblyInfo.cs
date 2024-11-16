using System.Runtime.CompilerServices;
using Slothsoft.Aseprite;

[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_EDITOR)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS)]

namespace Slothsoft.Aseprite {
    static class AssemblyInfo {
        public const string PACKAGE_ID = "net.slothsoft.aseprite";

        public const string NAMESPACE_RUNTIME = "Slothsoft.Aseprite";
        public const string NAMESPACE_EDITOR = "Slothsoft.Aseprite.Editor";
        public const string NAMESPACE_TESTS = "Slothsoft.Aseprite.Tests";
    }
}
