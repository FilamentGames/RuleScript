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
        static private bool DetectContextClick(Rect inRect)
        {
            return (Event.current.type == EventType.ContextClick && inRect.Contains(Event.current.mousePosition));
        }

        static private bool DetectContextClickLayout()
        {
            return DetectContextClick(GUILayoutUtility.GetLastRect());
        }

        static private readonly GUIContent s_ContextMenuCopyLabel = new GUIContent("Copy");
        static private readonly GUIContent s_ContextMenuPasteOverwriteLabel = new GUIContent("Paste (Overwrite)");
        static private readonly GUIContent s_ContextMenuPasteInsertLabel = new GUIContent("Paste (Insert)");
        static private readonly GUIContent s_ContextMenuDeleteLabel = new GUIContent("Delete");

        static private readonly GUIContent s_ContextMenuPasteAddToEndLabel = new GUIContent("Paste (Add)");
        static private readonly GUIContent s_ContextMenuDeleteAllLabel = new GUIContent("Delete All");
    }
}