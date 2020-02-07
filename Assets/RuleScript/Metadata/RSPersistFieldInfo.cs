using System;
using System.Reflection;
using BeauData;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Metadata for a persistent field.
    /// </summary>
    public sealed class RSPersistFieldInfo
    {
        public readonly string Name;

        public RSTypeInfo Type { get; private set; }

        private readonly FieldInfo m_FieldInfo;
        private readonly Type m_FieldType;

        public RSPersistFieldInfo(RSPersistFieldAttribute inAttribute, FieldInfo inInfo)
        {
            Name = inAttribute?.Name ?? inInfo.Name;

            m_FieldInfo = inInfo;
            m_FieldType = inInfo.FieldType;
        }

        internal void Link(RSTypeAssembly inAssembly)
        {
            Type = RSInterop.RSTypeFor(m_FieldType, inAssembly);
        }

        internal RSValue Persist(IRSRuntimeComponent inComponent)
        {
            object persistValue = m_FieldInfo.GetValue(inComponent);
            return RSInterop.ToRSValue(persistValue);
        }

        internal void Restore(IRSRuntimeComponent inComponent, RSValue inValue, RSEnvironment inEnvironment)
        {
            object restoreValue = RSInterop.ToObject(m_FieldType, inValue, inEnvironment.StaticScope);
            m_FieldInfo.SetValue(inComponent, restoreValue);
        }

        public JSON Export()
        {
            JSON element = JSON.CreateObject();
            element["name"].AsString = Name;
            element["type"].AsString = Type.ToString();
            return element;
        }

        public override string ToString()
        {
            return string.Format("{1}: {0}", Type, Name);
        }
    }
}