using RuleScript.Data;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    [CustomPropertyDrawer(typeof(RSEntitySelectorAttribute))]
    public sealed class EntityIDSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            IRSEntityMgr manager = null;
            RSEditorUtility.EditorPlugin.TryGetEntityManager(property.serializedObject.targetObject, out manager);

            label = EditorGUI.BeginProperty(position, label, property);
            {
                SerializedProperty value = property.FindPropertyRelative("m_Value");
                RSEntityId newId = RSEditorUtility.EditorPlugin.EntityIdGUIField(position, label, new RSEntityId(value.intValue), manager);
                value.intValue = (int) newId;
            }
            EditorGUI.EndProperty();
        }
    }
}