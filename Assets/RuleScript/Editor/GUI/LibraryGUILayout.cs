using RuleScript.Data;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Editor
{
    static public class LibraryGUILayout
    {
        #region Trigger

        #region Generic

        /// <summary>
        /// A trigger selector.
        /// </summary>
        static public RSTriggerId TriggerSelector(RSTriggerId inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUILayout.RSElementSelector((int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector.
        /// </summary>
        static public RSTriggerId TriggerSelector(RSTriggerId inCurrentId, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUILayout.RSElementSelector((int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(GUIContent inLabel, RSTriggerId inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUILayout.RSElementSelector(inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(GUIContent inLabel, RSTriggerId inCurrentId, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUILayout.RSElementSelector(inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        #endregion // Generic

        #region Entity

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(RSTriggerId inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUILayout.RSElementSelector((int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(RSTriggerId inCurrentId, IRSEntity inEntity, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUILayout.RSElementSelector((int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(GUIContent inLabel, RSTriggerId inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUILayout.RSElementSelector(inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(GUIContent inLabel, RSTriggerId inCurrentId, IRSEntity inEntity, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUILayout.RSElementSelector(inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        #endregion // Entity

        #endregion // Trigger

        #region Query

        #region Generic

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(GUIContent inLabel, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(GUIContent inLabel, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Generic

        #region Global

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(GUIContent inLabel, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(GUIContent inLabel, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Global

        #region Unknown Entity

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(GUIContent inLabel, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(GUIContent inLabel, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Unknown Entity

        #region Entity

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(int inCurrentId, IRSEntity inEntity, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(int inCurrentId, IRSEntity inEntity, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(GUIContent inLabel, int inCurrentId, IRSEntity inEntity, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(GUIContent inLabel, int inCurrentId, IRSEntity inEntity, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Entity

        #endregion // Query

        #region Action

        #region Generic

        /// <summary>
        /// A action selector.
        /// </summary>
        static public int ActionSelector(int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActions(RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector.
        /// </summary>
        static public int ActionSelector(GUIContent inLabel, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActions(RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Generic

        #region Global

        /// <summary>
        /// A action selector for the global scope.
        /// </summary>
        static public int ActionSelectorGlobal(int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllGlobalActions(RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector for the global scope.
        /// </summary>
        static public int ActionSelectorGlobal(GUIContent inLabel, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllGlobalActions(RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Global

        #region Unknown Entity

        /// <summary>
        /// A action selector for a unknown entity.
        /// </summary>
        static public int ActionSelectorUnknown(int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllLocalActions(RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector for a unknown entity.
        /// </summary>
        static public int ActionSelectorUnknown(GUIContent inLabel, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllLocalActions(RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Unknown Entity

        #region Entity

        /// <summary>
        /// A action selector for a specific entity.
        /// </summary>
        static public int ActionSelector(int inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActionsForEntity(inEntity, RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector for a specific entity.
        /// </summary>
        static public int ActionSelector(GUIContent inLabel, int inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActionsForEntity(inEntity, RSEditorUtility.s_ActionElements);

            return RSGUILayout.RSElementSelector(inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Entity

        #endregion // Action

        #region Group

        /// <summary>
        /// A group selector.
        /// </summary>
        static public RSGroupId GroupSelector(RSGroupId inCurrentGroup, RSLibrary inLibrary)
        {
            RSEditorUtility.s_GroupElements.Clear();
            inLibrary.GetAllGroups(RSEditorUtility.s_GroupElements);

            int group = RSGUILayout.RSElementSelector((int) inCurrentGroup, RSEditorUtility.s_GroupElements);
            return new RSGroupId(group);
        }

        /// <summary>
        /// A group selector.
        /// </summary>
        static public RSGroupId GroupSelector(GUIContent inLabel, RSGroupId inCurrentGroup, RSLibrary inLibrary)
        {
            RSEditorUtility.s_GroupElements.Clear();
            inLibrary.GetAllGroups(RSEditorUtility.s_GroupElements);

            int group = RSGUILayout.RSElementSelector(inLabel, (int) inCurrentGroup, RSEditorUtility.s_GroupElements);
            return new RSGroupId(group);
        }

        #endregion // Group
    }
}