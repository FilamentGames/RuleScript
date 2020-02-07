using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using BeauData;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Metadata
{
    public sealed class RSQueryInfo : RSMethodInfo
    {
        public RSTypeInfo ReturnType { get; private set; }

        private readonly object m_DefaultSystemValue;
        private RSValue m_DefaultValue;

        public RSQueryInfo(RSQueryAttribute inAttribute, MethodInfo inMethod) : base(inAttribute, inMethod)
        {
            m_DefaultSystemValue = inAttribute.DefaultValue;

            if (inAttribute.UsesRegisters)
                Flags |= RSMemberFlags.UsesRegisters;
        }

        public void PopulateDefaultArguments(RSResolvableValueData ioValue, int inStartIndex = 0)
        {
            if (Parameters == null || Parameters.Length == 0)
            {
                ioValue.QueryArguments = null;
            }
            else
            {
                Array.Resize(ref ioValue.QueryArguments, Parameters.Length);
                for (int i = inStartIndex; i < Parameters.Length; ++i)
                    ioValue.QueryArguments[i] = NestedValue.FromValue(Parameters[i].Default);
            }
        }

        internal override void Link(RSTypeAssembly inAssembly)
        {
            m_MethodSettings.Configure(m_MethodInfo, m_Parameters, true);
            OwnerType = m_MethodSettings.BoundType;

            ReturnType = RSInterop.RSTypeFor(m_MethodInfo.ReturnType, inAssembly);
            m_DefaultValue = m_DefaultSystemValue != null ? RSInterop.ToRSValue(m_DefaultSystemValue) : ReturnType.DefaultValue;

            GenerateParameters(inAssembly);
        }

        internal RSValue Invoke(IRSRuntimeEntity inTarget, RSValue[] inArguments, ExecutionScope inContext)
        {
            object thisArg;
            bool bValid = PrepContext(inTarget, inContext, out thisArg);
            if (!bValid)
            {
                if (m_MethodSettings.ComponentType != null)
                {
                    inContext.Logger?.Warn("Unable to evaluate query {0} on Entity \"{1}\" ({2}) - missing component type {3}", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null, m_MethodSettings.ComponentType);
                }
                else
                {
                    inContext.Logger?.Warn("Unable to evaluate query {0} on Entity \"{1}\" ({2})", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null);
                }
                return m_DefaultValue;
            }

            PrepArguments(inArguments, inContext);

            object returnVal = m_MethodInfo.Invoke(thisArg, m_CachedArguments);
            return RSInterop.ToRSValue(returnVal);
        }

        internal RSValue InvokeWithCachedArgs(IRSRuntimeEntity inTarget, ExecutionScope inContext)
        {
            object thisArg;
            bool bValid = PrepContext(inTarget, inContext, out thisArg);
            if (!bValid)
            {
                if (m_MethodSettings.ComponentType != null)
                {
                    inContext.Logger?.Warn("Unable to evaluate query {0} on Entity \"{1}\" ({2}) - missing component type {3}", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null, m_MethodSettings.ComponentType);
                }
                else
                {
                    inContext.Logger?.Warn("Unable to evaluate query {0} on Entity \"{1}\" ({2})", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null);
                }
                return m_DefaultValue;
            }

            object returnVal = m_MethodInfo.Invoke(thisArg, m_CachedArguments);
            return RSInterop.ToRSValue(returnVal);
        }

        protected override void ConstructTooltip(System.Text.StringBuilder ioBuilder)
        {
            base.ConstructTooltip(ioBuilder);

            ioBuilder.Append("\n - Returns: ").Append(ReturnType.FriendlyName);
        }

        public override JSON Export()
        {
            JSON baseExport = base.Export();
            baseExport["returnType"].AsString = ReturnType.ToString();
            return baseExport;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("QUERY ");
            builder.Append(ReturnType).Append(" ");
            builder.Append(Id).Append("(");
            for (int i = 0; i < Parameters.Length; ++i)
            {
                if (i > 0)
                    builder.Append(", ");
                builder.Append(Parameters[i]);
            }
            builder.Append(") [id ").Append(IdHash).Append("]");
            builder.Append("\nBinding: ").Append(OwnerType?.Name ?? "Global");
            builder.Append("\nName: ").Append(Name);
            builder.Append("\nDescription: ").Append(Description);
            return builder.ToString();
        }
    }
}