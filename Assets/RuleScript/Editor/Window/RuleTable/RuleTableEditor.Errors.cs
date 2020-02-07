using System;
using BeauData;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Validation;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RuleScript.Editor
{
    public sealed partial class RuleTableEditor : EditorWindow
    {
        private RSValidationState m_LastValidationState;

        private void ErrorGUI()
        {
            using(new EditorGUI.DisabledScope(m_SelectionState.Table == null))
            {
                using(new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Check for Issues"))
                    {
                        ScanForIssues();
                    }

                    int warningCount = 0;
                    int errorCount = 0;

                    if (m_LastValidationState != null)
                    {
                        warningCount = m_LastValidationState.WarningCount;
                        errorCount = m_LastValidationState.ErrorCount;
                    }

                    GUILayout.FlexibleSpace();

                    using(new RSGUI.ColorScope(Color.red))
                    {
                        GUILayout.Label("✖");
                        GUILayout.Label(errorCount.ToString());
                    }

                    using(new RSGUI.ColorScope(Color.yellow))
                    {
                        GUILayout.Label("?");
                        GUILayout.Label(warningCount.ToString());
                    }
                }

                if (m_LastValidationState != null)
                {
                    EditorGUILayout.Separator();

                    if (string.IsNullOrEmpty(m_LastValidationState.Output))
                    {
                        GUILayout.Label("No errors or warnings!", RSGUIStyles.ErrorsStyle);
                    }
                    else
                    {
                        m_ScrollState.ErrorScroll = EditorGUILayout.BeginScrollView(m_ScrollState.ErrorScroll, false, true, GUILayout.Height(200));
                        GUILayout.Label(m_LastValidationState.Output, RSGUIStyles.ErrorsStyle);
                        EditorGUILayout.EndScrollView();
                    }
                }
            }
        }

        private void ScanForIssues()
        {
            m_LastValidationState = RSValidator.Validate(m_SelectionState.Table, m_Context);
        }
    }
}