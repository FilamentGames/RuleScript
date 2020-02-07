using System;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    public struct UndoTarget
    {
        [NonSerialized] private UnityEngine.Object m_UndoTarget;
        [NonSerialized] private string m_UndoString;

        public UndoTarget(UnityEngine.Object inUndoTarget, string inName = null)
        {
            if (inUndoTarget)
            {
                m_UndoTarget = inUndoTarget;
                m_UndoString = inName ?? inUndoTarget.GetType().Name;
            }
            else
            {
                m_UndoString = "null";
                m_UndoTarget = null;
            }
        }

        public void MarkDirty(string inAction, bool inbArrayModified = false)
        {
            if (m_UndoTarget != null)
            {
                if (inbArrayModified)
                {
                    Undo.RegisterCompleteObjectUndo(m_UndoTarget, m_UndoString + ": " + inAction);
                }
                else
                {
                    Undo.RecordObject(m_UndoTarget, m_UndoString + ": " + inAction);
                }

                EditorUtility.SetDirty(m_UndoTarget);
            }
        }

        public void MarkDirtyWithoutUndo(string inAction, bool inbArrayModified = false)
        {
            if (m_UndoTarget != null)
            {
                EditorUtility.SetDirty(m_UndoTarget);
            }
        }

        static private readonly UndoTarget s_Null = default(UndoTarget);

        static public UndoTarget Null { get { return s_Null; } }

        static public implicit operator UndoTarget(UnityEngine.Object inObject)
        {
            return new UndoTarget(inObject);
        }

        public bool IsValid()
        {
            if (ReferenceEquals(m_UndoTarget, null))
                return true;

            if (!m_UndoTarget)
                return false;

            return true;
        }

        public bool IsEmpty()
        {
            return ReferenceEquals(m_UndoTarget, null);
        }
    }
}