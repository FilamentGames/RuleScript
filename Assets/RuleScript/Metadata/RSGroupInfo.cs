using System;
using System.Reflection;
using BeauData;
using RuleScript.Data;
using UnityEngine;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Metadata for an entity group.
    /// </summary>
    public sealed class RSGroupInfo : IRSInfo
    {
        public readonly string Id;
        public readonly int IdHash;
        public readonly RSGroupId GroupId;

        public readonly string Name;
        public readonly string Description;
        public readonly string Icon;
        public readonly string Tooltip;

        public RSGroupInfo(RSGroupAttribute inAttribute, FieldInfo inFieldInfo)
        {
            Id = inAttribute.Id;
            IdHash = ScriptUtils.Hash(Id);
            GroupId = new RSGroupId(IdHash);

            Name = inAttribute.Name ?? inFieldInfo.Name;
            Description = inAttribute.Description ?? string.Empty;
            Icon = inAttribute.Icon ?? string.Empty;

            using(var psb = PooledStringBuilder.Alloc())
            {
                psb.Builder.Append(Name);
                if (!string.IsNullOrEmpty(Description))
                {
                    psb.Builder.Append("\n - ").Append(Description);
                }
                Tooltip = psb.Builder.ToString();
            }
        }

        #region IRSInfo

        string IRSInfo.Id { get { return Id; } }

        int IRSInfo.IdHash { get { return IdHash; } }

        string IRSInfo.Name { get { return Name; } }

        string IRSInfo.Description { get { return Description; } }

        string IRSInfo.Icon { get { return Icon; } }

        string IRSInfo.Tooltip { get { return Tooltip; } }

        public JSON Export()
        {
            JSON element = JSON.CreateObject();
            element["id"].AsString = Id;
            element["idHash"].AsInt = IdHash;
            element["name"].AsString = Name;
            element["description"].AsString = Description;
            element["icon"].AsString = Icon;
            return element;
        }

        #endregion // IRSInfo
    }
}