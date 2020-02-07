using System;
using System.Reflection;
using BeauData;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Metadata for a parameter.
    /// </summary>
    public sealed class RSParameterInfo
    {
        public readonly string Name;
        public readonly string Description;

        public RSTypeInfo Type { get; private set; }
        public RSValue Default { get; private set; }

        public bool NotNull { get; private set; }
        public RSTypeInfo TriggerParameterType { get; private set; }

        public string Tooltip
        {
            get
            {
                if (m_Tooltip == null)
                {
                    m_Tooltip = ConstructTooltip();
                }
                return m_Tooltip;
            }
        }

        private readonly ParameterInfo m_ParameterInfo;
        private readonly Type m_ParameterType;
        private readonly Type m_TriggerParameterType;

        private string m_Tooltip;

        public RSParameterInfo(RSParameterAttribute inAttribute, ParameterInfo inInfo)
        {
            Name = inAttribute?.Name ?? inInfo.Name;
            Description = inAttribute?.Description ?? string.Empty;

            m_ParameterInfo = inInfo;
            m_ParameterType = inInfo.ParameterType;
            m_TriggerParameterType = inAttribute?.TriggerParameterType;

            NotNull = inAttribute == null ? false : inAttribute.NotNull;
        }

        public RSParameterInfo(string inName, string inDescription, Type inSystemType, bool inbNotNull)
        {
            Name = inName ?? inSystemType.Name;
            Description = inDescription ?? string.Empty;

            m_ParameterType = inSystemType;
            NotNull = inbNotNull;
        }

        private string ConstructTooltip()
        {
            using(var psb = PooledStringBuilder.Alloc())
            {
                psb.Builder.Append(Name);
                psb.Builder.Append(" (").Append(Type.FriendlyName).Append(")");
                if (!string.IsNullOrEmpty(Description))
                {
                    psb.Builder.Append(": ").Append(Description);
                }
                return psb.ToString();
            }
        }

        internal void Link(RSTypeAssembly inAssembly)
        {
            Type = RSInterop.RSTypeFor(m_ParameterType, inAssembly);
            if (m_ParameterInfo != null && m_ParameterInfo.HasDefaultValue)
            {
                Default = RSInterop.ToRSValue(m_ParameterInfo.DefaultValue);
            }
            else
            {
                Default = Type.DefaultValue;
            }

            if (m_TriggerParameterType != null)
                TriggerParameterType = RSInterop.RSTypeFor(m_TriggerParameterType, inAssembly);
        }

        public JSON Export()
        {
            JSON element = JSON.CreateObject();
            element["name"].AsString = Name;
            element["description"].AsString = Description;
            element["type"].AsString = Type.ToString();
            element["default"].AsString = Default.ToString();
            return element;
        }

        public override string ToString()
        {
            if (Default.GetInnerType() == RSValue.InnerType.String)
                return string.Format("{1}: {0} = \"{2}\"", Type, Name, Default);
            return string.Format("{1}: {0} = {2}", Type, Name, Default);
        }

        public string ToStringWithoutDefault()
        {
            return string.Format("{1}: {0}", Type, Name);
        }
    }
}