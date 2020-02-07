using RuleScript.Data;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    [CustomPropertyDrawer(typeof(RSGroupId))]
    public sealed class GroupIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty value = property.FindPropertyRelative("m_Value");
                value.intValue = (int) LibraryGUI.GroupSelector(position, label, new RSGroupId(value.intValue), RSEditorUtility.EditorPlugin.Library);
            }
            EditorGUI.EndProperty();
        }
    }
}