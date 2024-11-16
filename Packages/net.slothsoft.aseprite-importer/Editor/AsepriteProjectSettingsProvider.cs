using UnityEditor;
using UnityEditor.SettingsManagement;

namespace CursedBroom.Aseprite.Editor {
    static class AsepriteProjectSettingsProvider {
        const string PACKAGE_NAME = "com.cursedbroom.aseprite";

        internal static Settings settings => m_settings ??= new Settings(PACKAGE_NAME);
        static Settings m_settings;

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider() => new UserSettingsProvider(
            "Cursed Broom/Aseprite",
            settings,
            new[] { typeof(AsepriteProjectSettingsProvider).Assembly },
            SettingsScope.Project
        );
    }
}
