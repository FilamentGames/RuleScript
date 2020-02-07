using System;
using System.Collections.Generic;
using RuleScript.Metadata;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    static public class RSGUI
    {
        #region Help Tooltips

        public const float HelpTooltipWidth = 16;

        static private GUIContent s_HelpTooltip;
        static private Dictionary<Type, GUIContent> s_NullHelpTooltips;
        static private GUILayoutOption[] s_HelpTooltipOptions;

        static internal GUIContent NullHelpTooltip(Type inType)
        {
            if (s_NullHelpTooltips == null)
            {
                s_NullHelpTooltips = new Dictionary<Type, GUIContent>();
            }

            GUIContent content;
            if (!s_NullHelpTooltips.TryGetValue(inType, out content))
            {
                content = new GUIContent();
                content.text = "?";
                content.tooltip = string.Format("No {0} selected", inType.Name);
                s_NullHelpTooltips.Add(inType, content);
            }

            return content;
        }

        static internal GUIContent HelpTooltip(string inTooltip)
        {
            if (s_HelpTooltip == null)
            {
                s_HelpTooltip = new GUIContent();
                s_HelpTooltip.text = "?";
            }

            s_HelpTooltip.tooltip = inTooltip;
            s_HelpTooltip.image = null;

            return s_HelpTooltip;
        }

        static internal GUILayoutOption[] HelpTooltipLayoutOptions()
        {
            if (s_HelpTooltipOptions == null)
            {
                s_HelpTooltipOptions = new GUILayoutOption[]
                {
                    GUILayout.Width(HelpTooltipWidth)
                };
            }

            return s_HelpTooltipOptions;
        }

        #endregion // Help Tooltips

        #region Scopes

        internal class LabelWidthScope : GUI.Scope
        {
            private float m_PrevLabelWidth;

            public LabelWidthScope(float inLabelWidth)
            {
                m_PrevLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = inLabelWidth;
            }

            protected override void CloseScope()
            {
                EditorGUIUtility.labelWidth = m_PrevLabelWidth;
            }
        }

        internal class ColorScope : GUI.Scope
        {
            private Color m_PrevColor;

            public ColorScope(Color inColor)
            {
                m_PrevColor = GUI.color;
                GUI.color = inColor;
            }

            protected override void CloseScope()
            {
                GUI.color = m_PrevColor;
            }
        }

        #endregion // Scopes

        #region RSElement

        static internal int RSElementSelector<T>(Rect inPosition, int inCurrentId, RSElementList<T> inElementList) where T : IRSInfo
        {
            inElementList.RefreshInspectorList();

            int currentIdx = inElementList.IndexOf(inCurrentId);
            int nextIdx = EditorGUI.Popup(inPosition, currentIdx, inElementList.InspectorList());

            if (nextIdx < 0)
                return inCurrentId;

            var element = inElementList.ElementAt(nextIdx);
            return element == null ? 0 : element.IdHash;
        }

        static internal int RSElementSelector<T>(Rect inPosition, GUIContent inLabel, int inCurrentId, RSElementList<T> inElementList) where T : IRSInfo
        {
            inElementList.RefreshInspectorList();

            int currentIdx = inElementList.IndexOf(inCurrentId);
            int nextIdx = EditorGUI.Popup(inPosition, inLabel, currentIdx, inElementList.InspectorList());

            if (nextIdx < 0)
                return inCurrentId;

            var element = inElementList.ElementAt(nextIdx);
            return element == null ? 0 : element.IdHash;
        }

        #endregion // RSElement
    }
}