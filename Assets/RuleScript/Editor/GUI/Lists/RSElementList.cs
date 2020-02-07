using System;
using System.Collections;
using System.Collections.Generic;
using RuleScript.Metadata;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    internal sealed class RSElementList<T> : IList<T> where T : IRSInfo
    {
        private readonly List<T> m_InnerList = new List<T>();
        private readonly string m_NullElement;

        private GUIContent[] m_InspectorElements = null;
        private bool m_RequireRefresh;

        public RSElementList(string inNullElement)
        {
            m_NullElement = inNullElement;
        }

        public void RefreshInspectorList()
        {
            if (!m_RequireRefresh)
                return;

            m_InnerList.Sort(s_Comparer);
            Array.Resize(ref m_InspectorElements, m_InnerList.Count + 1);
            PopulateContent(ref m_InspectorElements[0], m_NullElement);
            for (int i = 0; i < m_InnerList.Count; ++i)
            {
                PopulateContent(ref m_InspectorElements[i + 1], m_InnerList[i]);
            }
            m_RequireRefresh = false;
        }

        public GUIContent[] InspectorList()
        {
            return m_InspectorElements;
        }

        public int IndexOf(T inElement)
        {
            if (inElement == null)
                return 0;

            int index = m_InnerList.IndexOf(inElement);
            if (index >= 0)
                return index + 1;

            return -1;
        }

        public int IndexOf(int inIdHash)
        {
            if (inIdHash == 0)
                return 0;

            for (int i = 0; i < m_InnerList.Count; ++i)
            {
                if (m_InnerList[i].IdHash == inIdHash)
                    return i + 1;
            }

            return -1;
        }

        public T ElementAt(int inIndex, T inDefault = default(T))
        {
            if (inIndex <= 0)
                return inDefault;

            if (inIndex == 0)
                return default(T);

            return m_InnerList[inIndex - 1];
        }

        public bool Contains(T inElement)
        {
            return inElement == null || IndexOf(inElement) > 0;
        }

        public bool Contains(int inIdHash)
        {
            return inIdHash == 0 || IndexOf(inIdHash) > 0;
        }

        public void Clear()
        {
            m_InnerList.Clear();
            m_RequireRefresh = true;
        }

        static private void PopulateContent(ref GUIContent ioContent, T inElement)
        {
            if (ioContent == null)
                ioContent = new GUIContent();

            ioContent.text = inElement.Name;
            ioContent.tooltip = inElement.Description;
        }

        static private void PopulateContent(ref GUIContent ioContent, string inText)
        {
            if (ioContent == null)
                ioContent = new GUIContent();

            ioContent.text = inText;
        }

        #region IList

        T IList<T>.this[int index] { get => ((IList<T>) m_InnerList) [index]; set => ((IList<T>) m_InnerList) [index] = value; }

        int ICollection<T>.Count { get { return ((IList<T>) m_InnerList).Count; } }

        bool ICollection<T>.IsReadOnly => ((IList<T>) m_InnerList).IsReadOnly;

        void ICollection<T>.Add(T item)
        {
            ((IList<T>) m_InnerList).Add(item);
            m_RequireRefresh = true;
        }

        void ICollection<T>.Clear()
        {
            ((IList<T>) m_InnerList).Clear();
            m_RequireRefresh = true;
        }

        bool ICollection<T>.Contains(T item)
        {
            return ((IList<T>) m_InnerList).Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            ((IList<T>) m_InnerList).CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IList<T>) m_InnerList).GetEnumerator();
        }

        int IList<T>.IndexOf(T item)
        {
            return ((IList<T>) m_InnerList).IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            ((IList<T>) m_InnerList).Insert(index, item);
            m_RequireRefresh = true;
        }

        bool ICollection<T>.Remove(T item)
        {
            m_RequireRefresh = true;
            return ((IList<T>) m_InnerList).Remove(item);
        }

        void IList<T>.RemoveAt(int index)
        {
            ((IList<T>) m_InnerList).RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IList<T>) m_InnerList).GetEnumerator();
        }

        #endregion // IList

        #region Comparisons

        static private readonly Comparator s_Comparer = new Comparator();

        private class Comparator : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                return EditorUtility.NaturalCompare(x.Name, y.Name);
            }
        }

        #endregion // Comparisons
    }
}