using CursedBroom.Aseprite.Editor;
using UnityEditor.SettingsManagement;

namespace CursedBroom.Aseprite {
    static class AsepriteProjectSettings {
        [UserSetting("Color Palette Settings", "Master Color Palette", "")]
        static UserSetting<string> masterColorPalettePathSetting = new(AsepriteProjectSettingsProvider.settings, nameof(masterColorPalettePathSetting), default);
        internal static string masterColorPalettePath => masterColorPalettePathSetting.value;
    }
}
