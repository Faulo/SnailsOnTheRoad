using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Slothsoft.Aseprite.Editor {
    sealed class AsepriteTableBodyRow : VisualElement {
        public AsepriteTableBodyRow(AssetListEntry<AsepriteContainer> entry) {
            style.flexDirection = FlexDirection.Row;

            var assetToggle = new Toggle {
                value = entry.isChecked
            };
            assetToggle.RegisterValueChangedCallback(evt => entry.isChecked = evt.newValue);
            assetToggle.style.width = EditorGUIUtility.singleLineHeight;
            assetToggle.style.height = EditorGUIUtility.singleLineHeight;
            Add(assetToggle);

            var assetPreview = new Image {
                image = AssetDatabase.GetCachedIcon(entry.path)
            };
            assetPreview.style.width = EditorGUIUtility.singleLineHeight;
            assetPreview.style.height = EditorGUIUtility.singleLineHeight;
            Add(assetPreview);

            var objectField = new ObjectField() { value = entry.asset, objectType = entry.asset.GetType() };
            objectField.SetEnabled(false);
            objectField.style.width = 24 * EditorGUIUtility.singleLineHeight;
            objectField.style.height = EditorGUIUtility.singleLineHeight;
            Add(objectField);

            Add(CreateColorsContainer(entry.asset.usedColorsNotInPalette));

            Add(CreatePaletteButton(entry.asset));

            entry.onChange += () => {
                assetToggle.value = entry.isChecked;

                style.display = entry.isVisible
                    ? DisplayStyle.Flex
                    : DisplayStyle.None;
            };
        }

        VisualElement CreatePaletteButton(AsepriteContainer asset) {
            var container = new VisualElement();

            if (asset.palette.masterPalette) {
                var paletteColors = new HashSet<Color32>(asset.palette.masterPalette.palette.colors);
                var missingColors = new HashSet<Color32>();
                foreach (var color in asset.usedColors) {
                    if (!paletteColors.Contains(color)) {
                        missingColors.Add(color);
                    }
                }

                if (missingColors.Count == 0) {
                    var button = CreateHelpButton(HelpBoxMessageType.Info);
                    button.tooltip = "Asset is using a master palette, and all its colors are found in that palette.";
                    button.clicked += () => {
                        Selection.activeObject = asset;
                    };
                    container.Add(button);
                } else {
                    container.Add(CreateColorsButton(missingColors));
                }
            } else {
                var button = CreateHelpButton(HelpBoxMessageType.Warning);
                button.tooltip = "Asset is not using a master palette.";
                button.clicked += () => {
                    Selection.activeObject = asset;
                };
                container.Add(button);
            }

            return container;

        }

        Button CreateHelpButton(HelpBoxMessageType type) {
            var container = new VisualElement();
            var button = new Button();
            button.AddToClassList(HelpBox.ussClassName);
            string className = type switch {
                HelpBoxMessageType.Info => HelpBox.iconInfoUssClassName,
                HelpBoxMessageType.Warning => HelpBox.iconwarningUssClassName,
                HelpBoxMessageType.Error => HelpBox.iconErrorUssClassName,
                _ => null,
            };
            if (!string.IsNullOrEmpty(className)) {
                button.AddToClassList(className);
            }

            button.style.width = EditorGUIUtility.singleLineHeight;
            button.style.height = EditorGUIUtility.singleLineHeight;
            return button;
        }

        VisualElement CreateColorsContainer(IReadOnlyCollection<Color32> colorsNotInPalette) {
            var container = new VisualElement();

            if (colorsNotInPalette.Count > 0) {
                container.Add(CreateColorsButton(colorsNotInPalette));
            }

            container.style.width = 8 * EditorGUIUtility.singleLineHeight;
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;

            return container;
        }

        VisualElement CreateColorsButton(IReadOnlyCollection<Color32> colorsNotInPalette) {
            var button = CreateHelpButton(HelpBoxMessageType.Error);

            button.tooltip = $"Asset uses {colorsNotInPalette.Count} colors from outside its palette! Click to show.";

            button.clicked += () => {
                int i = 0;
                foreach (var color in colorsNotInPalette) {
                    if (i == 16) {
                        Add(new Label($"... ({colorsNotInPalette.Count} total)"));
                        break;
                    }

                    var field = new ColorField {
                        value = color,
                        showEyeDropper = false
                    };
                    field.style.width = EditorGUIUtility.singleLineHeight;
                    field.style.height = EditorGUIUtility.singleLineHeight;
                    button.parent.Add(field);
                    i++;
                }

                button.parent.Remove(button);
            };
            return button;
        }
    }
}
