using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    [CustomPropertyDrawer(typeof(RSQuerySelectorAttribute))]
    public sealed class QuerySelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty value = property.FindPropertyRelative("m_Value");
                property.intValue = LibraryGUI.QuerySelector(position, label, property.intValue, false, RSEditorUtility.EditorPlugin.Library);
            }
            EditorGUI.EndProperty();
        }
    }
}