using System.Collections.Generic;
using CursedBroom.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace CursedBroom.Aseprite.Editor {
    [CustomPropertyDrawer(typeof(ProcessorContainer))]
    sealed class ProcessorContainerDrawer : PropertyDrawer {
        #region ProcessorCreator

        static readonly ImplementationLocator<IAsepriteProcessor> locator = new();
        static IReadOnlyList<Implementation<IAsepriteProcessor>> creators => locator.implementations;

        static GenericMenu CreateMenu(ReorderableList list) {
            var menu = new GenericMenu();
            if (creators.Count == 0) {
                menu.AddDisabledItem(new($"Implement {typeof(IAsepriteProcessor)} and mark the implementation with {typeof(ImplementationForAttribute)}!"));
            }

            foreach (var creator in creators) {
                menu.AddItem(
                    new(creator.label),
                    false,
                    () => {
                        int index = list.serializedProperty.arraySize;
                        list.serializedProperty.arraySize++;
                        list.index = index;
                        var element = list.serializedProperty.GetArrayElementAtIndex(index);
                        element.managedReferenceValue = creator.CreateInstance();
                        list.serializedProperty.serializedObject.ApplyModifiedProperties();
                    }
                );
            }

            menu.ShowAsContext();
            return menu;
        }
        #endregion

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        ReorderableList actionsList;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label) {
            label.text += $" ({nameof(ProcessorContainer)})";

            EditorGUI.BeginProperty(rect, label, property);

            EditorGUI.indentLevel = property.depth;

            var actionsProperty = property.FindPropertyRelative(nameof(ProcessorContainer.processors));

            Assert.IsNotNull(actionsProperty, "Failed to find actions");

            actionsList ??= new ReorderableList(property.serializedObject, actionsProperty) {
                drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, label);
                },
                elementHeightCallback = (int index) => {
                    var property = actionsProperty.GetArrayElementAtIndex(index);
                    int startingDepth = property.depth;

                    float height = lineHeight;
                    bool enterChildren = true;
                    while (property.NextVisible(enterChildren) && property.depth > startingDepth) {
                        enterChildren = false;
                        height += EditorGUI.GetPropertyHeight(property);
                    }

                    return height;
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    var property = actionsProperty.GetArrayElementAtIndex(index);
                    int startingDepth = property.depth;

                    string name = property.managedReferenceFullTypename.Split('.')[^1];

                    EditorGUI.BeginProperty(rect, new(name), property);

                    rect.height = lineHeight;
                    EditorGUI.indentLevel = property.depth;
                    EditorGUI.LabelField(rect, name, EditorStyles.boldLabel);
                    rect.y += rect.height;

                    bool enterChildren = true;
                    while (property.NextVisible(enterChildren) && property.depth > startingDepth) {
                        enterChildren = false;
                        rect.height = EditorGUI.GetPropertyHeight(property);
                        EditorGUI.indentLevel = property.depth;
                        EditorGUI.PropertyField(rect, property, true);
                        rect.y += rect.height;
                    }

                    EditorGUI.EndProperty();
                },
                onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {
                    var menu = CreateMenu(list);
                    menu.ShowAsContext();
                },
                elementHeight = lineHeight + spacing,
            };

            actionsList.DoLayoutList();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return lineHeight;
        }
    }
}
