using RuleScript.Data;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    [CustomPropertyDrawer(typeof(RSTriggerId))]
    public sealed class TriggerIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty value = property.FindPropertyRelative("m_Value");
                value.intValue = (int) LibraryGUI.TriggerSelector(position, label, new RSTriggerId(value.intValue), RSEditorUtility.EditorPlugin.Library);
            }
            EditorGUI.EndProperty();
        }
    }
}