using System;
using System.Reflection;
using System.Text;
using BeauData;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Metadata
{
    public sealed class RSActionInfo : RSMethodInfo
    {
        public readonly bool CheckEntityActive;

        public RSActionInfo(RSActionAttribute inAttribute, MethodInfo inMethodInfo) : base(inAttribute, inMethodInfo)
        {
            CheckEntityActive = inAttribute.IgnoreIfInactive;

            if (inAttribute.UsesRegisters)
                Flags |= RSMemberFlags.UsesRegisters;
        }

        public void PopulateDefaultArguments(RSActionData ioData)
        {
            if (Parameters == null || Parameters.Length == 0)
            {
                ioData.Arguments = null;
            }
            else
            {
                Array.Resize(ref ioData.Arguments, Parameters.Length);
                for (int i = 0; i < Parameters.Length; ++i)
                {
                    RSResolvableValueData.SetAsValue(ref ioData.Arguments[i], Parameters[i].Default);
                }
            }
        }

        internal override void Link(RSTypeAssembly inAssembly)
        {
            m_MethodSettings.Configure(m_MethodInfo, m_Parameters, true);
            OwnerType = m_MethodSettings.BoundType;

            GenerateParameters(inAssembly);
        }

        internal ActionResult Invoke(IRSRuntimeEntity inTarget, RSValue[] inValues, ExecutionScope inContext)
        {
            if (CheckEntityActive && !inTarget.IsActive())
            {
                inContext.Logger?.Warn("Unable to perform action {0} on Entity \"{1}\" ({2}) - entity is inactive", Id, inTarget.Name, inTarget.Id);
                return ActionResult.Inactive();
            }

            object thisArg;
            bool bValid = PrepContext(inTarget, inContext, out thisArg);
            if (!bValid)
            {
                if (m_MethodSettings.ComponentType != null)
                {
                    inContext.Logger?.Error("Unable to perform action {0} on Entity \"{1}\" ({2}) - missing component type {3}", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null, m_MethodSettings.ComponentType);
                    return ActionResult.NoComponent(m_MethodSettings.ComponentType);
                }
                else
                {
                    inContext.Logger?.Error("Unable to perform action {0} on Entity \"{1}\" ({2})", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null);
                    return ActionResult.NoEntity();
                }
            }

            PrepArguments(inValues, inContext);

            object result = m_MethodInfo.Invoke(thisArg, m_CachedArguments);
            return new ActionResult(result);
        }

        internal ActionResult InvokeWithCachedArgs(IRSRuntimeEntity inTarget, ExecutionScope inContext)
        {
            if (CheckEntityActive && !inTarget.IsActive())
            {
                inContext.Logger?.Warn("Unable to perform action {0} on Entity \"{1}\" ({2}) - entity is inactive", Id, inTarget.Name, inTarget.Id);
                return ActionResult.Inactive();
            }

            object thisArg;
            bool bValid = PrepContext(inTarget, inContext, out thisArg);
            if (!bValid)
            {
                if (m_MethodSettings.ComponentType != null)
                {
                    inContext.Logger?.Error("Unable to perform action {0} on Entity \"{1}\" ({2}) - missing component type {3}", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null, m_MethodSettings.ComponentType);
                    return ActionResult.NoComponent(m_MethodSettings.ComponentType);
                }
                else
                {
                    inContext.Logger?.Error("Unable to perform action {0} on Entity \"{1}\" ({2})", Id, inTarget?.Name, inTarget != null ? inTarget.Id : RSEntityId.Null);
                    return ActionResult.NoEntity();
                }
            }

            object result = m_MethodInfo.Invoke(thisArg, m_CachedArguments);
            return new ActionResult(result);
        }

        public override JSON Export()
        {
            JSON baseExport = base.Export();
            baseExport["checkActive"].AsBool = CheckEntityActive;
            return baseExport;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ACTION ");
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
            builder.Append("\nRequires Entity Active: ").Append(CheckEntityActive);
            return builder.ToString();
        }
    }
}