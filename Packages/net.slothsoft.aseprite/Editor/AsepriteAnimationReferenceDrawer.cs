using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Slothsoft.Aseprite.Editor {
    [CustomPropertyDrawer(typeof(AsepriteAnimationReference))]
    sealed class AsepriteAnimationReferenceDrawer : PropertyDrawer {
        const string FIELD_CONTEXT = nameof(AsepriteAnimationReference.source);
        const string FIELD_KEY = nameof(AsepriteAnimationReference.animation);

        bool hasBeenWarned;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, label);

            var contextField = property.FindPropertyRelative(FIELD_CONTEXT);
            var animationField = property.FindPropertyRelative(FIELD_KEY);

            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, contextField);
            position.y += EditorGUIUtility.singleLineHeight;
            if (contextField.objectReferenceValue is AsepriteFile aseprite) {
                var animations = aseprite.allAnimationNames
                    .ToList();

                if (string.IsNullOrEmpty(animationField.stringValue)) {
                    animationField.stringValue = animations[0];
                }

                int currentAnimation = animations.IndexOf(animationField.stringValue);
                if (currentAnimation == -1) {
                    currentAnimation = animations.Count;
                    animations.Add(animationField.stringValue);
                    if (!hasBeenWarned) {
                        hasBeenWarned = true;
                        Debug.LogWarning($"Aseprite file {aseprite} does not contain an animation '{animationField.stringValue}'!");
                    }
                }

                currentAnimation = EditorGUI.Popup(position, animationField.displayName, currentAnimation, animations.ToArray());

                animationField.stringValue = animations[currentAnimation];
                animationField.serializedObject.ApplyModifiedProperties();
            } else {
                EditorGUI.PropertyField(position, animationField);
            }

            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = base.GetPropertyHeight(property, label);
            height += EditorGUIUtility.singleLineHeight;
            height += EditorGUIUtility.singleLineHeight;
            return height;
        }
    }
}
