using RuleScript.Metadata;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    static public class RSGUILayout
    {
        #region RSElement

        static internal int RSElementSelector<T>(int inCurrentId, RSElementList<T> inElementList, params GUILayoutOption[] inOptions) where T : IRSInfo
        {
            using(new GUILayout.HorizontalScope())
            {
                return RSElementSelectorImpl<T>(null, inCurrentId, inElementList, inOptions);
            }
        }

        static internal int RSElementSelector<T>(GUIContent inLabel, int inCurrentId, RSElementList<T> inElementList, params GUILayoutOption[] inOptions) where T : IRSInfo
        {
            using(new GUILayout.HorizontalScope())
            {
                return RSElementSelectorImpl<T>(inLabel, inCurrentId, inElementList, inOptions);
            }
        }

        static internal int RSElementSelectorImpl<T>(GUIContent inLabel, int inCurrentId, RSElementList<T> inElementList, params GUILayoutOption[] inOptions) where T : IRSInfo
        {
            inElementList.RefreshInspectorList();

            int currentIdx = inElementList.IndexOf(inCurrentId);
            int nextIdx = EditorGUILayout.Popup(inLabel, currentIdx, inElementList.InspectorList(), inOptions);

            if (nextIdx < 0)
            {
                GUILayout.Label(RSGUI.NullHelpTooltip(typeof(T)), RSGUIStyles.HelpTooltipStyle, RSGUI.HelpTooltipLayoutOptions());
                return inCurrentId;
            }

            var element = inElementList.ElementAt(nextIdx);
            if (element == null)
            {
                GUILayout.Label(RSGUI.NullHelpTooltip(typeof(T)), RSGUIStyles.HelpTooltipStyle, RSGUI.HelpTooltipLayoutOptions());
                return 0;
            }

            GUILayout.Label(RSGUI.HelpTooltip(element.Tooltip), RSGUIStyles.HelpTooltipStyle, RSGUI.HelpTooltipLayoutOptions());
            return element.IdHash;
        }

        #endregion // RSElement
    }
}