using System.Runtime.CompilerServices;
using SotR.Player;

[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_EDITOR)]
[assembly: InternalsVisibleTo(AssemblyInfo.NAMESPACE_TESTS)]

namespace SotR.Player {
    static class AssemblyInfo {
        internal const string NAMESPACE_RUNTIME = "SotR.Player";
        internal const string NAMESPACE_EDITOR = "SotR.Player.Editor";
        internal const string NAMESPACE_TESTS = "SotR.Player.Tests";
    }
}