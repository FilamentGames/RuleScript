using BeauUtil.Editor;
using RuleScript.Data;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    [CustomPropertyDrawer(typeof(RSEntityId))]
    public sealed class EntityIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty value = property.FindPropertyRelative("m_Value");
                if (value.hasMultipleDifferentValues)
                {
                    EditorGUI.LabelField(position, label, EditorGUIUtility.TrTextContent("—", "Mixed Values", (Texture) null));
                }
                else if (value.intValue == 0)
                {
                    EditorGUI.LabelField(position, label, EditorGUIUtility.TrTempContent("Unassigned"));
                }
                else
                {
                    EditorGUI.LabelField(position, label, EditorGUIUtility.TrTempContent(value.intValue.ToString()));
                }
            }
            EditorGUI.EndProperty();
        }
    }
}