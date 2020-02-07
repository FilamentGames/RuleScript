using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    [CustomPropertyDrawer(typeof(RSActionSelectorAttribute))]
    public sealed class ActionSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            {
                property.intValue = LibraryGUI.ActionSelector(position, label, property.intValue, RSEditorUtility.EditorPlugin.Library);
            }
            EditorGUI.EndProperty();
        }
    }
}