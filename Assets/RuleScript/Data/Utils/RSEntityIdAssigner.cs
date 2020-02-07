using System;
using System.Collections;
using System.Collections.Generic;
using BeauData;
using UnityEngine;

namespace RuleScript.Data
{
    [Serializable]
    public sealed class RSEntityIdAssigner
    {
        [SerializeField] private int m_NextId = 0;
        [NonSerialized] private bool m_UseRuntime;

        public RSEntityId NextId()
        {
            int index = m_NextId++;
            byte flags = (byte) (m_UseRuntime ? 2 : 1);
            return RSEntityId.GenerateId(index, flags);
        }

        public void Reset()
        {
            m_NextId = 0;
            m_UseRuntime = false;
        }

        public bool UseRuntimeIds()
        {
            if (!m_UseRuntime)
            {
                m_UseRuntime = true;
                m_NextId = 0;
                return true;
            }

            return false;
        }
    }
}