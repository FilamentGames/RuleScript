using BeauUtil;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    static public class RSGUIStyles
    {
        static private GUIStyle s_RuleHeaderStyle;
        static private GUIStyle s_SubHeaderStyle;
        static private GUIStyle s_ErrorsStyle;
        static private GUIStyle s_ReadOnlyStyle;
        static private GUIStyle s_HelpTooltipStyle;

        static private bool s_Initialized;

        static private void Initialize()
        {
            if (s_Initialized)
                return;

            s_RuleHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
            s_RuleHeaderStyle.fontSize = 13;
            s_RuleHeaderStyle.wordWrap = true;

            s_SubHeaderStyle = new GUIStyle(EditorStyles.boldLabel);

            s_ErrorsStyle = new GUIStyle(EditorStyles.label);
            s_ErrorsStyle.wordWrap = true;
            s_ErrorsStyle.richText = true;
            s_ErrorsStyle.clipping = TextClipping.Clip;

            s_ReadOnlyStyle = new GUIStyle(EditorStyles.label);
            s_ReadOnlyStyle.normal.textColor = ColorBank.DarkGray;

            s_HelpTooltipStyle = new GUIStyle(EditorStyles.label);
            s_HelpTooltipStyle.normal.textColor = ColorBank.Aqua;

            s_Initialized = true;
        }

        static public GUIStyle RuleHeaderStyle { get { Initialize(); return s_RuleHeaderStyle; } }
        static public GUIStyle SubHeaderStyle { get { Initialize(); return s_SubHeaderStyle; } }
        static public GUIStyle ErrorsStyle { get { Initialize(); return s_ErrorsStyle; } }
        static public GUIStyle ReadOnlyStyle { get { Initialize(); return s_ReadOnlyStyle; } }
        static public GUIStyle HelpTooltipStyle { get { Initialize(); return s_HelpTooltipStyle; } }
    }
}