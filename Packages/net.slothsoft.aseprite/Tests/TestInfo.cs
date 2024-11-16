using System.IO;
using UnityEngine;

namespace Slothsoft.Aseprite.Tests {
    sealed class TestInfo : TestInfoBase<TestAsset> {
        public record SheetInfo(
            string name,
            int width,
            int height,
            bool alphaIsTransparency = true,
            int anisoLevel = 0,
            FilterMode filterMode = FilterMode.Point,
            TextureWrapMode wrapMode = TextureWrapMode.Clamp,
            bool mipmapsEnabled = false) {

            public static implicit operator SheetInfo(Texture2D texture) => texture
                ? new(
                    texture.name,
                    texture.width,
                    texture.height,
                    texture.alphaIsTransparency,
                    texture.anisoLevel,
                    texture.filterMode,
                    texture.wrapMode,
                    texture.streamingMipmaps
                )
                : default;

            public static implicit operator SheetInfo(TestAsset asset) => asset switch {
                TestAsset.PortraitsWithTags_Aseprite => new("TEST_Aseprite_PortraitsWithTags.aseprite.albedo.png", 192, 192),
                TestAsset.Icon_Aseprite => new("TEST_Aseprite_Icon.aseprite.albedo.png", 256, 256),
                _ => throw new System.NotImplementedException(),
            };
        }
        public record DataInfo(AsepriteData data) {

            public DataInfo(FileInfo file) : this(AsepriteData.FromJson(File.ReadAllText(file.FullName))) {
            }

            public static implicit operator DataInfo(AsepriteData data) => new(data);

            public static implicit operator DataInfo(TestAsset asset) {
                var info = new TestInfo();
                return asset switch {
                    TestAsset.PortraitsWithSlices_Aseprite => new(info.GetFile(TestAsset.PortraitsWithSlices_Data)),
                    TestAsset.PortraitsWithTags_Aseprite => new(info.GetFile(TestAsset.PortraitsWithTags_Data)),
                    TestAsset.Icon_Aseprite => new(info.GetFile(TestAsset.Icon_Data)),
                    _ => throw new System.NotImplementedException(),
                };
            }
        }

        public record SpriteInfo(string name, Rect rect, float pixelsPerUnit = 16, Vector4 border = default) {
            public static implicit operator SpriteInfo(Sprite sprite) => sprite
                ? new(sprite.name, sprite.rect, sprite.pixelsPerUnit, sprite.border)
                : default;

            public static SpriteInfo[] GetSpriteInfos(TestAsset asset) {
                return asset switch {
                    TestAsset.PortraitsWithTags_Aseprite => new SpriteInfo[] {
                        new("TEST_Aseprite_PortraitsWithTags_0.aseprite", new Rect(0, 128, 64, 64)),
                        new("TEST_Aseprite_PortraitsWithTags_1.aseprite", new Rect(64, 128, 64, 64))
                    },
                    TestAsset.Icon_Aseprite => new SpriteInfo[] {
                        new("TEST_Aseprite_Icon_0.aseprite", new Rect(0, 0, 256, 256))
                    },
                    _ => throw new System.NotImplementedException(),
                };
            }
        }

        protected override string GetAssetPath(TestAsset asset) => asset switch {
            TestAsset.PortraitsWithTags_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_PortraitsWithTags.aseprite",
            TestAsset.PortraitsWithTags_Sheet => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_PortraitsWithTags.png",
            TestAsset.PortraitsWithTags_Data => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_PortraitsWithTags.json",
            TestAsset.PortraitsWithSlices_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_PortraitsWithSlices.aseprite",
            TestAsset.PortraitsWithSlices_Sheet => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_PortraitsWithSlices.png",
            TestAsset.PortraitsWithSlices_Data => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_PortraitsWithSlices.json",
            TestAsset.Icon_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_Icon.aseprite",
            TestAsset.Icon_Sheet => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_Icon.png",
            TestAsset.Icon_Data => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_Icon.json",
            TestAsset.Avatar_Data => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_Avatar.json",
            TestAsset.SpiderWeb_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_SpiderWeb.aseprite",
            TestAsset.SpiderWeb_Sheet => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_SpiderWeb.png",
            TestAsset.SpiderWeb_Data => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_SpiderWeb.json",
            TestAsset.SlicedDoor_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_SlicedDoor.aseprite",
            TestAsset.PortraitsWithSlicesAndTags_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_PortraitWithSlicesAndTags.aseprite",
            TestAsset.Buttons_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_Buttons.aseprite",
            TestAsset.Animator => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_Animator.aseprite",
            TestAsset.AnimatorController => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_AnimatorController.controller",
            TestAsset.AnimatorAdditionalTag => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_AnimatorWithAdditionalTag.aseprite",
            TestAsset.AnimatorWithSlices => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_AnimatorWithSlices.aseprite",
            TestAsset.CharacterWithEmission => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_CharacterWithEmission.aseprite",
            TestAsset.NineSlice_Aseprite => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_NineSlice.aseprite",
            TestAsset.NineSlice_Data => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_NineSlice.json",
            TestAsset.ColorPalette => $"Packages/{AssemblyInfo.PACKAGE_ID}/TestAssets/TEST_Aseprite_ColorPalette.aseprite",
            _ => throw new System.NotImplementedException(),
        };
    }
}
