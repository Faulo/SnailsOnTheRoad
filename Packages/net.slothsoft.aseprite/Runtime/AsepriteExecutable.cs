using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityDebug = UnityEngine.Debug;

namespace Slothsoft.Aseprite {
    readonly struct AsepriteExecutable {
#if UNITY_EDITOR_WIN
        const string EXECUTABLE = "Packages/" + AssemblyInfo.PACKAGE_ID + "/.windows/Aseprite.exe";
        const string CACHE = "Packages/" + AssemblyInfo.PACKAGE_ID + "/.cache";
#else
        const string EXECUTABLE = "Packages/" + AssemblyInfo.PACKAGE_ID + "/.linux/aseprite";
        const string CACHE = "Packages/" + AssemblyInfo.PACKAGE_ID + "/.cache";
#endif
        static readonly FileInfo executable = new(EXECUTABLE);
        static readonly DirectoryInfo cache = new(CACHE);

        public static bool TryFindAseprite(out AsepriteExecutable aseprite) {
            aseprite = new();

#if UNITY_EDITOR

            if (!executable.Exists) {
                return false;
            }

            if (!cache.Exists) {
                cache.Create();
            }

            return true;
#else
            return false;
#endif
        }

        public override string ToString() => EXECUTABLE;

        public string Execute(params string[] args)
            => Execute(args as IEnumerable<string>);

        public static FileInfo CacheFile(FileInfo assetPath, string extension = default)
            => CacheFile(assetPath.ToString(), extension);
        public static FileInfo CacheFile(string assetPath, string extension = default) {
            if (!string.IsNullOrEmpty(extension)) {
                assetPath += $".{extension}";
            }

            var cachedFile = new FileInfo(Path.Combine(cache.FullName, assetPath));
            if (!cachedFile.Directory.Exists) {
                cachedFile.Directory.Create();
            }

            return cachedFile;
        }

        public string Execute(IEnumerable<string> args) {
            var process = new Process();
            process.StartInfo.FileName = executable.FullName;
            process.StartInfo.Arguments = string.Join(' ', args.Prepend("--batch"));
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.WaitForExit();

            string output = process.StartInfo.FileName + " " + process.StartInfo.Arguments + "\n" + process.StandardOutput.ReadToEnd() + "\n" + process.StandardError.ReadToEnd();
            if (process.ExitCode != 0) {
                UnityDebug.LogWarning(output);
            }

            return output;
        }
    }
}
