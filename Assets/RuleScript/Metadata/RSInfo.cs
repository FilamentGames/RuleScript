using System;
using System.Reflection;
using System.Text;
using BeauData;
using UnityEngine;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Metadata for a rule scripting element.
    /// </summary>
    public abstract class RSInfo : IRSInfo
    {
        public readonly string Id;
        public readonly int IdHash;

        public readonly string Name;
        public readonly string Description;
        public readonly string Icon;

        private string m_Tooltip;

        public string Tooltip
        {
            get
            {
                if (m_Tooltip == null)
                {
                    using(var psb = PooledStringBuilder.Alloc())
                    {
                        ConstructTooltip(psb.Builder);
                        m_Tooltip = psb.ToString();
                    }
                }
                return m_Tooltip;
            }
        }

        public Type OwnerType { get; protected set; }

        public RSInfo(RSMemberAttribute inAttribute, MemberInfo inMember)
        {
            Id = inAttribute.Id;
            IdHash = ScriptUtils.Hash(Id);

            Name = inAttribute.Name ?? inMember.Name;
            Description = inAttribute.Description ?? string.Empty;
            Icon = inAttribute.Icon ?? string.Empty;

            if (inMember is Type)
                OwnerType = InternalScriptUtils.GetLikelyBindingType((Type) inMember);
            else
                OwnerType = InternalScriptUtils.GetLikelyBindingType(inMember.DeclaringType);
        }

        internal abstract void Link(RSTypeAssembly inAssembly);

        protected virtual void ConstructTooltip(StringBuilder ioBuilder)
        {
            ioBuilder.Append(Name);
            if (!string.IsNullOrEmpty(Description))
            {
                ioBuilder.Append("\n - ").Append(Description);
            }
        }

        #region IRSInfo

        string IRSInfo.Id { get { return Id; } }

        int IRSInfo.IdHash { get { return IdHash; } }

        string IRSInfo.Name { get { return Name; } }

        string IRSInfo.Description { get { return Description; } }

        string IRSInfo.Icon { get { return Icon; } }

        string IRSInfo.Tooltip { get { return Tooltip; } }

        public virtual JSON Export()
        {
            JSON element = JSON.CreateObject();
            element["id"].AsString = Id;
            element["idHash"].AsInt = IdHash;
            element["name"].AsString = Name;
            element["description"].AsString = Description;
            element["icon"].AsString = Icon;
            element["binding"].AsString = OwnerType?.FullName;
            return element;
        }

        #endregion // IRSInfo
    }
}