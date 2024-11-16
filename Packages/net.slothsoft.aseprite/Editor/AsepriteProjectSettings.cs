using UnityEditor.SettingsManagement;

namespace Slothsoft.Aseprite.Editor {
    static class AsepriteProjectSettings {
        [UserSetting("Color Palette Settings", "Master Color Palette", "")]
        static UserSetting<string> masterColorPalettePathSetting = new(AsepriteProjectSettingsProvider.settings, nameof(masterColorPalettePathSetting), default);
        internal static string masterColorPalettePath => masterColorPalettePathSetting.value;
    }
}
