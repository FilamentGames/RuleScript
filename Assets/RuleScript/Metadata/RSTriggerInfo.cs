using System;
using System.Reflection;
using BeauData;
using RuleScript.Data;
using UnityEngine;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Metadata for a rule scripting parameter.
    /// </summary>
    public sealed class RSTriggerInfo : RSInfo
    {
        public readonly RSTriggerId TriggerId;

        public RSParameterInfo ParameterType { get; private set; }

        public RSTriggerInfo(RSTriggerAttribute inAttribute, FieldInfo inFieldInfo) : base(inAttribute, inFieldInfo)
        {
            TriggerId = new RSTriggerId(IdHash);

            if (inAttribute.ParameterType != null)
            {
                ParameterType = new RSParameterInfo(inAttribute.ParameterName, inAttribute.ParameterDescription, inAttribute.ParameterType, false);
            }

            OwnerType = inAttribute.Global ? null : (inAttribute.OwnerType ?? OwnerType);
        }

        protected override void ConstructTooltip(System.Text.StringBuilder ioBuilder)
        {
            base.ConstructTooltip(ioBuilder);

            if (ParameterType != null)
            {
                ioBuilder.Append("\n - Parameter")
                    .Append("\n   + ").Append(ParameterType.Tooltip);
            }
        }

        internal override void Link(RSTypeAssembly inAssembly)
        {
            ParameterType?.Link(inAssembly);
        }

        public override string ToString()
        {
            return string.Format("TRIGGER {0}({1}) [id {2}]\nBinding: {3}\nName: {4}\nDescription: {5}",
                Id,
                ParameterType,
                IdHash,
                OwnerType?.Name ?? "Global",
                Name,
                Description);
        }

        public override JSON Export()
        {
            JSON exported = base.Export();
            if (ParameterType != null)
            {
                exported["parameter"] = ParameterType.Export();
            }
            return exported;
        }
    }
}