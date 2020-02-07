using System;
using BeauData;
using BeauUtil;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Data
{
    /// <summary>
    /// Scope/target.
    /// </summary>
    [Serializable]
    public struct EntityScopeData : ISerializedObject, ISerializedVersion, IEquatable<EntityScopeData>, IRSPreviewable
    {
        #region Inspector

        [SerializeField] private EntityScopeType m_Type;
        [SerializeField] private int m_Id;
        [SerializeField] private string m_Search;
        [SerializeField] private bool m_UseFirst;
        [SerializeField] private string m_LinkSearch;
        [SerializeField] private bool m_UseFirstLink;

        #endregion // Inspector

        public EntityScopeType Type { get { return m_Type; } }

        #region Arguments

        public RSEntityId IdArg
        {
            get
            {
                Assert.True(m_Type == EntityScopeType.ObjectById, "IdArg not available for mode {0}", m_Type);
                return new RSEntityId(m_Id);
            }
        }

        public RSGroupId GroupArg
        {
            get
            {
                Assert.True(m_Type == EntityScopeType.ObjectsWithGroup, "GroupArg not available for mode {0}", m_Type);
                return new RSGroupId(m_Id);
            }
        }

        public RegisterIndex RegisterArg
        {
            get
            {
                Assert.True(m_Type == EntityScopeType.ObjectInRegister, "RegisterArg not available for mode {0}", m_Type);
                return (RegisterIndex) m_Id;
            }
        }

        public string SearchArg
        {
            get
            {
                Assert.True(m_Type == EntityScopeType.ObjectsWithName || m_Type == EntityScopeType.ObjectsWithPrefab, "SearchArg not available for mode {0}", m_Type);
                return m_Search;
            }
        }

        public bool UseFirst
        {
            get
            {
                Assert.True(m_Type == EntityScopeType.ObjectsWithGroup || m_Type == EntityScopeType.ObjectsWithName || m_Type == EntityScopeType.ObjectsWithPrefab, "UseFirst not available for mode {0}", m_Type);
                return m_UseFirst;
            }
        }

        public string LinksArg
        {
            get { return m_LinkSearch; }
        }

        public bool UseFirstLink
        {
            get { return m_UseFirstLink; }
        }

        #endregion // Arguments

        public bool IsKnownTarget()
        {
            if (!string.IsNullOrEmpty(m_LinkSearch))
                return false;

            switch (m_Type)
            {
                case EntityScopeType.Null:
                case EntityScopeType.Self:
                case EntityScopeType.Global:
                case EntityScopeType.ObjectById:
                case EntityScopeType.Invalid:
                    return true;

                default:
                    return false;
            }
        }

        public bool IsMultiTarget()
        {
            if (!string.IsNullOrEmpty(m_LinkSearch))
                return !m_UseFirstLink;

            switch (m_Type)
            {
                case EntityScopeType.ObjectsWithGroup:
                case EntityScopeType.ObjectsWithName:
                case EntityScopeType.ObjectsWithPrefab:
                    return !m_UseFirst;

                default:
                    return false;
            }
        }

        public bool SupportsLinks()
        {
            switch (m_Type)
            {
                case EntityScopeType.Null:
                case EntityScopeType.Invalid:
                case EntityScopeType.Global:
                    return false;

                default:
                    return true;
            }
        }

        public bool HasLinks()
        {
            return !string.IsNullOrEmpty(m_LinkSearch);
        }

        public EntityScopeData WithLinks(string inLinks, bool inbUseFirstLink)
        {
            if (!SupportsLinks())
                return this;

            m_LinkSearch = inLinks;
            m_UseFirstLink = inbUseFirstLink;
            return this;
        }

        internal EntityScopeData(EntityScopeType inType, int inId = 0, string inSearch = null, bool inbUseFirst = false)
        {
            m_Type = inType;
            m_Id = inId;
            m_Search = inSearch;
            m_UseFirst = inbUseFirst;
            m_LinkSearch = null;
            m_UseFirstLink = false;
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 3; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Enum("type", ref m_Type, FieldOptions.PreferAttribute);
            switch (m_Type)
            {
                case EntityScopeType.ObjectById:
                    ioSerializer.Serialize("id", ref m_Id, FieldOptions.PreferAttribute);
                    break;

                case EntityScopeType.ObjectInRegister:
                    ioSerializer.Serialize("register", ref m_Id, FieldOptions.PreferAttribute);
                    break;

                case EntityScopeType.ObjectsWithGroup:
                    {
                        ioSerializer.Serialize("group", ref m_Id, FieldOptions.PreferAttribute);
                        ioSerializer.Serialize("useFirst", ref m_UseFirst, false, FieldOptions.PreferAttribute);
                        break;
                    }

                case EntityScopeType.ObjectsWithName:
                    ioSerializer.Serialize("name", ref m_Search, FieldOptions.PreferAttribute);
                    ioSerializer.Serialize("useFirst", ref m_UseFirst, false, FieldOptions.PreferAttribute);
                    break;

                case EntityScopeType.ObjectsWithPrefab:
                    ioSerializer.Serialize("prefab", ref m_Search, FieldOptions.PreferAttribute);
                    ioSerializer.Serialize("useFirst", ref m_UseFirst, false, FieldOptions.PreferAttribute);
                    break;
            }

            if (ioSerializer.ObjectVersion >= 2)
            {
                ioSerializer.Serialize("links", ref m_LinkSearch, string.Empty, FieldOptions.PreferAttribute | FieldOptions.Optional);
                if (!string.IsNullOrEmpty(m_LinkSearch))
                    ioSerializer.Serialize("useFirstLink", ref m_UseFirstLink, false, FieldOptions.PreferAttribute | FieldOptions.Optional);
            }
        }

        #endregion // ISerializedObject

        #region IEquatable

        public bool Equals(EntityScopeData other)
        {
            if (m_Type != other.m_Type)
                return false;

            if (!StringUtils.NullEquivalentEquals(m_LinkSearch, other.m_LinkSearch) || m_UseFirstLink != other.m_UseFirstLink)
                return false;

            switch (m_Type)
            {
                case EntityScopeType.ObjectById:
                case EntityScopeType.ObjectInRegister:
                    return m_Id == other.m_Id;

                case EntityScopeType.ObjectsWithGroup:
                    return m_UseFirst == other.m_UseFirst && m_Id == other.m_Id;

                case EntityScopeType.ObjectsWithName:
                case EntityScopeType.ObjectsWithPrefab:
                    return m_UseFirst == other.m_UseFirst && StringUtils.NullEquivalentEquals(m_Search, other.m_Search);

                default:
                    return true;
            }
        }

        #endregion // IEquatable

        #region IPreviewable

        public string GetPreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            string preview = GetBasePreviewString(inTriggerContext, inLibrary);
            if (!string.IsNullOrEmpty(m_LinkSearch))
            {
                preview += "." + m_LinkSearch;
                if (m_UseFirstLink)
                {
                    preview += "[0]";
                }
            }
            return preview;
        }

        private string GetBasePreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            switch (m_Type)
            {
                case EntityScopeType.Null:
                    return "Null";
                case EntityScopeType.Self:
                    return "Self";
                case EntityScopeType.Argument:
                    return "Arg(" + (inTriggerContext?.ParameterType?.Name ?? "TriggerArg") + ")";
                case EntityScopeType.Global:
                    return "Global";
                case EntityScopeType.ObjectById:
                    return string.Format("Entity(Id: {0})", m_Id);
                case EntityScopeType.ObjectInRegister:
                    return string.Format("Register({0})", RegisterArg);
                case EntityScopeType.ObjectsWithName:
                    {
                        if (m_UseFirst)
                            return string.Format("Entity(Name: {0})", m_Search);
                        return string.Format("Entities(Name: {0})", m_Search);
                    }
                case EntityScopeType.ObjectsWithPrefab:
                    {
                        if (m_UseFirst)
                            return string.Format("Entity(Prefab: {0})", m_Search);
                        return string.Format("Entities(Prefab: {0})", m_Search);
                    }
                case EntityScopeType.ObjectsWithGroup:
                    {
                        using(PooledStringBuilder psb = PooledStringBuilder.Alloc())
                        {
                            var sb = psb.Builder;

                            if (m_UseFirst)
                                sb.Append("Entity");
                            else
                                sb.Append("Entities");

                            sb.Append("(Group: ");
                            if (m_Id == 0)
                            {
                                sb.Append("No Group");
                            }
                            else
                            {
                                var groupInfo = inLibrary?.GetGroup(m_Id);
                                if (groupInfo == null)
                                    sb.Append(m_Id);
                                else
                                    sb.Append(groupInfo.Name);
                            }

                            sb.Append(")");
                            return sb.ToString();
                        }
                    }
                case EntityScopeType.Invalid:
                    return "Invalid";

                default:
                    return "Unknown";
            }
        }

        #endregion // IPreviewable

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is EntityScopeData)
                return Equals((EntityScopeData) obj);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = m_Type.GetHashCode();
            hash = (hash << 5) ^ m_Id.GetHashCode();
            hash = (hash >> 3) ^ m_UseFirst.GetHashCode();
            if (m_Search != null)
                hash = (hash << 2) ^ m_Search.GetHashCode();
            if (m_LinkSearch != null)
                hash = (hash >> 3) ^ m_LinkSearch.GetHashCode();
            hash = (hash << 5) ^ m_UseFirstLink.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return GetPreviewString(null, null);
        }

        static public bool operator ==(EntityScopeData a, EntityScopeData b)
        {
            return a.Equals(b);
        }

        static public bool operator !=(EntityScopeData a, EntityScopeData b)
        {
            return !a.Equals(b);
        }

        #endregion // Overrides

        #region Static

        static public EntityScopeData Null() { return new EntityScopeData(EntityScopeType.Null); }

        static public EntityScopeData Self() { return new EntityScopeData(EntityScopeType.Self); }
        static public EntityScopeData Global() { return new EntityScopeData(EntityScopeType.Global); }
        static public EntityScopeData Argument() { return new EntityScopeData(EntityScopeType.Argument); }

        static public EntityScopeData Entity(RSEntityId inId)
        {
            if (inId == RSEntityId.Invalid)
                return Invalid();
            return new EntityScopeData(EntityScopeType.ObjectById, (int) inId);
        }
        static public EntityScopeData Register(RegisterIndex inIndex) { return new EntityScopeData(EntityScopeType.ObjectInRegister, (int) inIndex); }

        static public EntityScopeData WithGroup(RSGroupId inGroup, bool inbUseFirst = false) { return new EntityScopeData(EntityScopeType.ObjectsWithGroup, (int) inGroup, null, inbUseFirst); }
        static public EntityScopeData WithName(string inName, bool inbUseFirst = false) { return new EntityScopeData(EntityScopeType.ObjectsWithName, 0, inName, inbUseFirst); }
        static public EntityScopeData WithPrefab(string inPrefab, bool inbUseFirst = false) { return new EntityScopeData(EntityScopeType.ObjectsWithPrefab, 0, inPrefab, inbUseFirst); }

        static public EntityScopeData Invalid() { return new EntityScopeData(EntityScopeType.Invalid); }

        #endregion // Static
    }
}