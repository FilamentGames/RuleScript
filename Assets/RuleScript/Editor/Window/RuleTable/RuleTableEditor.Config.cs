using System;
using BeauData;
using BeauUtil.Editor;
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
        [SerializeField] private Serializer.Format m_SerializationFormat = Serializer.Format.JSON;
        [SerializeField] private bool m_FilterSelections = true;

        private RSValidationFlags GetBaseFlags()
        {
            if (m_FilterSelections)
                return RSValidationFlags.FilterSelection;
            return 0;
        }

        private void ConfigGUI()
        {
            using(new EditorGUI.DisabledScope(m_SelectionState.Table == null))
            {
                {
                    bool bFilterSelections = EditorGUILayout.Toggle(EditorGUIUtility.TrTextContent("Filter Selections", "If unchecked, all triggers, queries, and actions will be presented for all entities."), m_FilterSelections);
                    if (bFilterSelections != m_FilterSelections)
                    {
                        m_SelfUndoTarget.MarkDirty("Changed filter mode");
                        m_FilterSelections = bFilterSelections;
                    }
                }

                EditorGUILayout.Space();

                using(new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Export"))
                    {
                        SaveTable(m_SelectionState.Table, m_SerializationFormat, null);
                    }

                    if (GUILayout.Button("Import"))
                    {
                        RSRuleTableData table = LoadTable(null);
                        if (table != null)
                        {
                            SelectAction(-1);
                            SelectCondition(-1);
                            SelectRule(-1);

                            m_TargetState.UndoTarget.MarkDirty("Reloaded Table", true);
                            m_SelectionState.Table.CopyFrom(table);
                        }
                    }

                    Serializer.Format newFormat = ListGUILayout.Popup(m_SerializationFormat, RSEditorUtility.s_SerializeFormats, GUILayout.Width(60));
                    if (newFormat != m_SerializationFormat)
                    {
                        m_SelfUndoTarget.MarkDirty("Changed Serialization Format");
                        m_SerializationFormat = newFormat;
                    }
                }
            }
        }

        #region Save/Load

        static private readonly string[] FILE_FILTERS = new string[] { "Rule Tables", "rule" };

        /// <summary>
        /// Prompts the user to save a table to disk.
        /// </summary>
        static private bool SaveTable(RSRuleTableData inData, Serializer.Format inFormat, string inInitialPath)
        {
            string filePath = EditorUtility.SaveFilePanel("Save Rule Table", inInitialPath ?? Application.dataPath, inData.Name, "rule");
            if (string.IsNullOrEmpty(filePath))
                return false;

            try
            {
                Serializer.WriteFile(inData, filePath, OutputOptions.PrettyPrint, inFormat);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorUtility.DisplayDialog("Error while writing table", "Exception encountered; see console for details", "Okay");
                return false;
            }
        }

        /// <summary>
        /// Prompts the user to load a table from disk.
        /// </summary>
        static private RSRuleTableData LoadTable(string inInitialPath)
        {
            string filePath = EditorUtility.OpenFilePanelWithFilters("Load Rule Table", inInitialPath ?? Application.dataPath, FILE_FILTERS);
            if (string.IsNullOrEmpty(filePath))
                return null;

            try
            {
                RSRuleTableData ruleTableData = null;
                bool bSuccess = Serializer.ReadFile(ref ruleTableData, filePath);
                if (!bSuccess)
                {
                    EditorUtility.DisplayDialog("Error while reading table", "See console for details", "Okay");
                    return null;
                }

                return ruleTableData;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorUtility.DisplayDialog("Error while reading table", "Exception encountered; see console for details", "Okay");
                return null;
            }
        }

        #endregion // Save/Load
    }
}