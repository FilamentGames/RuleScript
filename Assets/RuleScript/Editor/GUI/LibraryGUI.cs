using RuleScript.Data;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Editor
{
    static public class LibraryGUI
    {
        #region Trigger

        /// <summary>
        /// A trigger selector.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, RSTriggerId inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUI.RSElementSelector(inPosition, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, RSTriggerId inCurrentId, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUI.RSElementSelector(inPosition, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, GUIContent inLabel, RSTriggerId inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUI.RSElementSelector(inPosition, inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, GUIContent inLabel, RSTriggerId inCurrentId, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUI.RSElementSelector(inPosition, inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, RSTriggerId inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUI.RSElementSelector(inPosition, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, RSTriggerId inCurrentId, IRSEntity inEntity, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUI.RSElementSelector(inPosition, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        /// <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, GUIContent inLabel, RSTriggerId inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements);

            int trigger = RSGUI.RSElementSelector(inPosition, inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        // <summary>
        /// A trigger selector for a specific entity.
        /// </summary>
        static public RSTriggerId TriggerSelector(Rect inPosition, GUIContent inLabel, RSTriggerId inCurrentId, IRSEntity inEntity, RSTypeInfo inParameterType, RSLibrary inLibrary)
        {
            RSEditorUtility.s_TriggerElements.Clear();
            inLibrary.GetAllTriggersForEntity(inEntity, RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);
            inLibrary.GetAllGlobalTriggers(RSEditorUtility.s_TriggerElements, inParameterType ?? RSBuiltInTypes.Void);

            int trigger = RSGUI.RSElementSelector(inPosition, inLabel, (int) inCurrentId, RSEditorUtility.s_TriggerElements);
            return new RSTriggerId(trigger);
        }

        #endregion // Trigger

        #region Query

        #region Generic

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(Rect inPosition, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(Rect inPosition, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(Rect inPosition, GUIContent inLabel, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector.
        /// </summary>
        static public int QuerySelector(Rect inPosition, GUIContent inLabel, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Generic

        #region Global

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(Rect inPosition, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(Rect inPosition, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(Rect inPosition, GUIContent inLabel, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for the global scope.
        /// </summary>
        static public int QuerySelectorGlobal(Rect inPosition, GUIContent inLabel, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllGlobalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Global

        #region Unknown Entity

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(Rect inPosition, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(Rect inPosition, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(Rect inPosition, GUIContent inLabel, int inCurrentId, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a unknown entity.
        /// </summary>
        static public int QuerySelectorUnknown(Rect inPosition, GUIContent inLabel, int inCurrentId, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllLocalQueries(RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Unknown Entity

        #region Entity

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(Rect inPosition, int inCurrentId, IRSEntity inEntity, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(Rect inPosition, int inCurrentId, IRSEntity inEntity, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(Rect inPosition, GUIContent inLabel, int inCurrentId, IRSEntity inEntity, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, null, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        /// <summary>
        /// A query selector for a specific entity.
        /// </summary>
        static public int QuerySelector(Rect inPosition, GUIContent inLabel, int inCurrentId, IRSEntity inEntity, RSTypeInfo inReturnType, bool inbNoParams, RSLibrary inLibrary)
        {
            RSEditorUtility.s_QueryElements.Clear();
            inLibrary.GetAllQueriesForEntity(inEntity, RSEditorUtility.s_QueryElements, inReturnType, inbNoParams);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_QueryElements);
        }

        #endregion // Entity

        #endregion // Query

        #region Action

        #region Generic

        /// <summary>
        /// A action selector.
        /// </summary>
        static public int ActionSelector(Rect inPosition, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActions(RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector.
        /// </summary>
        static public int ActionSelector(Rect inPosition, GUIContent inLabel, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActions(RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Generic

        #region Global

        /// <summary>
        /// A action selector for the global scope.
        /// </summary>
        static public int ActionSelectorGlobal(Rect inPosition, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllGlobalActions(RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector for the global scope.
        /// </summary>
        static public int ActionSelectorGlobal(Rect inPosition, GUIContent inLabel, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllGlobalActions(RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Global

        #region Unknown Entity

        /// <summary>
        /// A action selector for a unknown entity.
        /// </summary>
        static public int ActionSelectorUnknown(Rect inPosition, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllLocalActions(RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector for a unknown entity.
        /// </summary>
        static public int ActionSelectorUnknown(Rect inPosition, GUIContent inLabel, int inCurrentId, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllLocalActions(RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Unknown Entity

        #region Entity

        /// <summary>
        /// A action selector for a specific entity.
        /// </summary>
        static public int ActionSelector(Rect inPosition, int inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActionsForEntity(inEntity, RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        /// <summary>
        /// A action selector for a specific entity.
        /// </summary>
        static public int ActionSelector(Rect inPosition, GUIContent inLabel, int inCurrentId, IRSEntity inEntity, RSLibrary inLibrary)
        {
            RSEditorUtility.s_ActionElements.Clear();
            inLibrary.GetAllActionsForEntity(inEntity, RSEditorUtility.s_ActionElements);

            return RSGUI.RSElementSelector(inPosition, inLabel, inCurrentId, RSEditorUtility.s_ActionElements);
        }

        #endregion // Entity

        #endregion // Action

        #region Group

        /// <summary>
        /// A group selector.
        /// </summary>
        static public RSGroupId GroupSelector(Rect inPosition, RSGroupId inCurrentGroup, RSLibrary inLibrary)
        {
            RSEditorUtility.s_GroupElements.Clear();
            inLibrary.GetAllGroups(RSEditorUtility.s_GroupElements);

            int group = RSGUI.RSElementSelector(inPosition, (int) inCurrentGroup, RSEditorUtility.s_GroupElements);
            return new RSGroupId(group);
        }

        /// <summary>
        /// A group selector.
        /// </summary>
        static public RSGroupId GroupSelector(Rect inPosition, GUIContent inLabel, RSGroupId inCurrentGroup, RSLibrary inLibrary)
        {
            RSEditorUtility.s_GroupElements.Clear();
            inLibrary.GetAllGroups(RSEditorUtility.s_GroupElements);

            int group = RSGUI.RSElementSelector(inPosition, inLabel, (int) inCurrentGroup, RSEditorUtility.s_GroupElements);
            return new RSGroupId(group);
        }

        #endregion // Group
    }
}