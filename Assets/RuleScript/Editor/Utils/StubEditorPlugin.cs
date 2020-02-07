using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuleScript.Editor
{
    internal sealed class StubEditorPlugin : IRSEditorPlugin
    {
        static internal readonly StubEditorPlugin Instance = new StubEditorPlugin();

        public RSLibrary Library { get { return RSEditorUtility.DefaultLibrary(); } }

        public RSEntityId EntityIdGUIField(GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager, params GUILayoutOption[] inOptions)
        {
            int currentId = (int) inValue;
            int nextId = EditorGUILayout.DelayedIntField(inLabel, currentId, inOptions);
            return new RSEntityId(nextId);
        }

        public RSEntityId EntityIdGUIField(Rect inPosition, GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager)
        {
            int currentId = (int) inValue;
            int nextId = EditorGUI.DelayedIntField(inPosition, inLabel, currentId);
            return new RSEntityId(nextId);
        }

        public RSEntityId ComponentGUIField(GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager, System.Type inRequiredType, params GUILayoutOption[] inOptions)
        {
            int currentId = (int) inValue;
            int nextId = EditorGUILayout.DelayedIntField(inLabel, currentId, inOptions);
            return new RSEntityId(nextId);
        }

        public RSEntityId ComponentGUIField(Rect inPosition, GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager, System.Type inRequiredType)
        {
            int currentId = (int) inValue;
            int nextId = EditorGUI.DelayedIntField(inPosition, inLabel, currentId);
            return new RSEntityId(nextId);
        }

        public IRSRuleTableSource RuleTableSourceField(GUIContent inLabel, IRSRuleTableSource inValue, IRSRuleTableMgr inManager, params GUILayoutOption[] inOptions)
        {
            EditorGUILayout.LabelField(inLabel, "No default inspector for rule table sources", inOptions);
            return inValue;
        }

        public IRSRuleTableSource RuleTableSourceField(Rect inPosition, GUIContent inLabel, IRSRuleTableSource inValue, IRSRuleTableMgr inManager)
        {
            EditorGUI.LabelField(inPosition, inLabel, "No default inspector for rule table sources");
            return inValue;
        }

        public bool TryGetEntity(Object inObject, out IRSEntity outEntity)
        {
            outEntity = null;
            return false;
        }

        public bool TryGetEntityManager(Object inObject, out IRSEntityMgr outManager)
        {
            outManager = null;
            return false;
        }

        public bool TryGetEntityManager(Scene inScene, out IRSEntityMgr outManager)
        {
            outManager = null;
            return false;
        }

        public bool TryGetRuleTable(Object inObject, out IRSRuleTableSource outTableSource)
        {
            outTableSource = null;
            return false;
        }

        public bool TryGetRuleTableManager(Object inObject, out IRSRuleTableMgr outManager)
        {
            outManager = null;
            return false;
        }

        public bool TryGetRuleTableManager(Scene inScene, out IRSRuleTableMgr outManager)
        {
            outManager = null;
            return false;
        }

        public void OnRuleTableModified(IRSRuleTableSource inTableSource)
        {
            if (inTableSource is UnityEngine.Object)
                Undo.RegisterCompleteObjectUndo((UnityEngine.Object) inTableSource, "Modified Rule Table");
        }
    }
}