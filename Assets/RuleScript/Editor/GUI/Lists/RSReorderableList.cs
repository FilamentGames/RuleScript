using System;
using System.Reflection;
using BeauUtil;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RuleScript.Editor
{
    [Serializable]
    internal class RSReorderableList<T> : ReorderableList
    {
        // Since ReorderableList doesn't expose id (or many other important fields, for that matter)
        // we need to access it via reflection
        static private readonly FieldInfo ID_FIELD = typeof(ReorderableList).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);

        public ReorderCallbackDelegate onWillReorderCallback;

        public RSReorderableList(T[] inArray) : base(inArray, typeof(T), true, false, true, true)
        {
            onMouseDragCallback = (l) => SetDragging(true);
            onMouseUpCallback = (l) => SetDragging(false);
            onReorderCallback = (l) => SetDragging(false);
        }

        public T[] array
        {
            get { return (T[]) list; }
            set { list = value; }
        }

        [NonSerialized] private bool m_MouseDragging;

        private void SetDragging(bool inbDragging)
        {
            if (m_MouseDragging == inbDragging)
                return;

            m_MouseDragging = inbDragging;
            if (inbDragging && onWillReorderCallback != null)
                onWillReorderCallback(this);
        }

        public void DoLayout()
        {
            // Prevent anything other than up, down, and escape
            // keyboard events from affecting the list
            Event currentEvent = Event.current;
            Event eventClone = null;

            if (currentEvent.type == EventType.KeyDown)
            {
                int myId = (int) ID_FIELD.GetValue(this);
                if (myId == GUIUtility.keyboardControl)
                {
                    switch (currentEvent.keyCode)
                    {
                        case KeyCode.Escape:
                        case KeyCode.DownArrow:
                        case KeyCode.UpArrow:
                            break;

                        default:
                            eventClone = new Event(currentEvent);
                            currentEvent.Use();
                            break;
                    }
                }
            }

            DoLayoutList();

            if (eventClone != null)
                Event.current = eventClone;
        }
    }
}