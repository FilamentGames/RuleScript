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
        static public class Content
        {
            static public readonly GUIContent TableNameLabel = new GUIContent("Name", "Name of the table\nCurrently unused.");
            static public readonly GUIContent TableRuleListLabel = new GUIContent("Rules", "List of trigger/condition/action responses");

            static public readonly GUIContent RuleConditionListLabel = new GUIContent("Conditions", "List of conditions that must pass for the rule to execute");
            static public readonly GUIContent RuleConditionSubsetLabel = new GUIContent("Subset", "Subset of the above conditions that must pass for the rule to execute");
            static public readonly GUIContent RuleActionListLabel = new GUIContent("Actions", "List of actions to execute");
        }
    }
}