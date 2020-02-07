using System;
using System.Collections.Generic;
using System.Reflection;
using BeauData;
using BeauUtil;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Metadata for a rule scripting component.
    /// </summary>
    public sealed class RSComponentInfo : RSInfo
    {
        private Dictionary<string, RSPersistFieldInfo> m_PersistFields;
        private bool m_UseCustomDataField;

        public bool HasPersistentFields { get; private set; }

        public RSComponentInfo(RSComponentAttribute inAttribute, Type inType) : base(inAttribute, inType) { }

        internal override void Link(RSTypeAssembly inAssembly)
        {
            m_UseCustomDataField = typeof(IRSCustomPersistDataProvider).IsAssignableFrom(OwnerType);

            GeneratePersistFields(inAssembly);

            HasPersistentFields = m_UseCustomDataField || (m_PersistFields != null && m_PersistFields.Count > 0);
        }

        internal void GeneratePersistFields(RSTypeAssembly inAssembly)
        {
            using(PooledList<RSPersistFieldInfo> persistFields = PooledList<RSPersistFieldInfo>.Alloc())
            {
                foreach (var persistField in Reflect.FindFields<RSPersistFieldAttribute>(OwnerType, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    FieldInfo field = persistField.Info;
                    RSPersistFieldAttribute fieldAttr = persistField.Attribute;

                    RSPersistFieldInfo fieldInfo = new RSPersistFieldInfo(fieldAttr, field);
                    persistFields.Add(fieldInfo);
                }

                if (persistFields.Count > 0)
                {
                    m_PersistFields = new Dictionary<string, RSPersistFieldInfo>(persistFields.Count);
                    for (int i = 0; i < persistFields.Count; ++i)
                    {
                        RSPersistFieldInfo info = persistFields[i];
                        info.Link(inAssembly);
                        m_PersistFields.Add(info.Name, info);
                    }
                }
            }
        }

        internal void Persist(IRSRuntimeComponent inComponent, int inFlags, ref RSPersistComponentData outData)
        {
            if (outData == null)
            {
                outData = new RSPersistComponentData();
            }

            outData.ComponentType = IdHash;

            if (m_PersistFields != null)
            {
                Array.Resize(ref outData.NamedValues, m_PersistFields.Count);

                int idx = 0;
                foreach (var persistField in m_PersistFields.Values)
                {
                    string name = persistField.Name;
                    RSValue value = persistField.Persist(inComponent);
                    outData.NamedValues[idx++] = new RSNamedValue(name, value);
                }
            }

            if (m_UseCustomDataField)
            {
                ((IRSCustomPersistDataProvider) inComponent).GetCustomPersistData(ref outData.CustomData, inFlags);
            }
        }

        internal void Restore(IRSRuntimeComponent inComponent, RSPersistComponentData inData, RSEnvironment inEnvironment, int inFlags)
        {
            if (inData.NamedValues != null)
            {
                for (int i = 0; i < inData.NamedValues.Length; ++i)
                {
                    string name = inData.NamedValues[i].Name;
                    RSValue value = inData.NamedValues[i].Value;

                    RSPersistFieldInfo fieldInfo;
                    m_PersistFields.TryGetValue(name, out fieldInfo);
                    if (fieldInfo != null)
                    {
                        fieldInfo.Restore(inComponent, value, inEnvironment);
                    }
                }
            }

            if (m_UseCustomDataField)
            {
                ((IRSCustomPersistDataProvider) inComponent).RestoreCustomPersistData(inData.CustomData, inFlags, inEnvironment);
            }
        }

        public override JSON Export()
        {
            JSON json = base.Export();

            JSON fields = JSON.CreateArray();
            if (m_PersistFields != null)
            {
                foreach (var field in m_PersistFields.Values)
                {
                    fields.Add(field.Export());
                }
            }

            json["persistFields"] = fields;

            return json;
        }
    }
}