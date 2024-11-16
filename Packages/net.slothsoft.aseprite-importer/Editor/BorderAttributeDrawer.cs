using UnityEditor;
using UnityEngine;

namespace CursedBroom.Aseprite.Editor {
    [CustomPropertyDrawer(typeof(BorderAttribute))]
    sealed class BorderAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(position, label);
            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(Vector4.x)), new GUIContent("left"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(Vector4.y)), new GUIContent("bottom"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(Vector4.z)), new GUIContent("right"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(Vector4.w)), new GUIContent("top"));
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 6 * EditorGUIUtility.singleLineHeight;
        }
    }
}
