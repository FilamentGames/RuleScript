/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSEntityData.cs
 * Purpose: Serializable data about an entity.
 */

using System;
using System.Collections.Generic;
using BeauData;
using BeauUtil;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    [Serializable]
    public class RSEntityData : ISerializedObject, ISerializedVersion, IRSEntity
    {
        #region Links

        private sealed class LinksAccessor : IRSEntityLinkSet
        {
            private readonly RSEntityData m_Source;

            public LinksAccessor(RSEntityData inSource)
            {
                m_Source = inSource;
            }

            public int Count
            {
                get
                {
                    if (m_Source.Links == null)
                        return 0;
                    return m_Source.Links.Length;
                }
            }

            public bool AddLink(RSEntityId inEntity, string inLinkId)
            {
                int currentIndex = IndexOf(inEntity, inLinkId);
                if (currentIndex >= 0)
                    return false;

                ArrayUtils.Add(ref m_Source.Links, new RSEntityLinkData(inEntity, inLinkId));
                return true;
            }

            public IEnumerable<KeyValuePair<RSEntityId, string>> AllLinks()
            {
                if (m_Source.Links != null)
                {
                    foreach (var link in m_Source.Links)
                    {
                        if (link.EntityId != RSEntityId.Null)
                        {
                            yield return new KeyValuePair<RSEntityId, string>(link.EntityId, link.Name);
                        }
                    }
                }
            }

            public bool Clear()
            {
                if (m_Source.Links != null && m_Source.Links.Length > 0)
                {
                    ArrayUtils.Clear(ref m_Source.Links);
                    return true;
                }

                return false;
            }

            public IEnumerable<RSEntityId> EntitiesWithLink(StringSlice inLinkId)
            {
                if (m_Source.Links == null)
                    yield break;

                for (int i = 0; i < m_Source.Links.Length; ++i)
                {
                    if (m_Source.Links[i].Name == inLinkId)
                    {
                        yield return m_Source.Links[i].EntityId;
                    }
                }
            }

            public RSEntityId EntityWithLink(StringSlice inLinkId)
            {
                if (m_Source.Links == null)
                    return RSEntityId.Null;

                for (int i = 0; i < m_Source.Links.Length; ++i)
                {
                    if (m_Source.Links[i].Name == inLinkId)
                    {
                        return m_Source.Links[i].EntityId;
                    }
                }

                return RSEntityId.Null;
            }

            public bool RemoveAllLinks(RSEntityId inEntity)
            {
                if (m_Source.Links == null)
                    return false;

                List<RSEntityLinkData> linkList = new List<RSEntityLinkData>(m_Source.Links);

                bool bChanged = false;
                for (int i = linkList.Count - 1; i >= 0; --i)
                {
                    if (linkList[i].EntityId == inEntity)
                    {
                        linkList.RemoveAt(i);
                        bChanged = true;
                    }
                }

                if (bChanged)
                    m_Source.Links = linkList.ToArray();
                return bChanged;
            }

            public bool RemoveAllLinks(string inLinkId)
            {
                if (m_Source.Links == null)
                    return false;

                List<RSEntityLinkData> linkList = new List<RSEntityLinkData>(m_Source.Links);

                bool bChanged = false;
                for (int i = linkList.Count - 1; i >= 0; --i)
                {
                    if (linkList[i].Name == inLinkId)
                    {
                        linkList.RemoveAt(i);
                        bChanged = true;
                    }
                }

                if (bChanged)
                    m_Source.Links = linkList.ToArray();
                return bChanged;
            }

            public bool RemoveLink(RSEntityId inEntity, string inLinkId)
            {
                int currentIndex = IndexOf(inEntity, inLinkId);
                if (currentIndex < 0)
                    return false;

                ArrayUtils.RemoveAt(ref m_Source.Links, currentIndex);
                return true;
            }

            private int IndexOf(RSEntityId inEntity, string inLinkId)
            {
                if (m_Source.Links == null)
                    return -1;

                for (int i = 0; i < m_Source.Links.Length; ++i)
                {
                    if (m_Source.Links[i].EntityId == inEntity && m_Source.Links[i].Name == inLinkId)
                    {
                        return i;
                    }
                }

                return -1;
            }
        }

        #endregion // Links

        #region Inspector

        public RSEntityId Id = RSEntityId.Null;
        public string Prefab = null;

        public bool Enabled = true;
        public string Name = null;
        public RSGroupId Group = RSGroupId.Null;

        public RSComponentData[] Components;
        public int TableId = -1;

        public RSEntityLinkData[] Links;

        #endregion // Inspector

        private LinksAccessor m_LinkAccess;

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 2; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Int32Proxy("id", ref Id, FieldOptions.PreferAttribute);
            ioSerializer.Serialize("prefab", ref Prefab, string.Empty, FieldOptions.Optional | FieldOptions.PreferAttribute);

            ioSerializer.Serialize("enabled", ref Enabled, true, FieldOptions.PreferAttribute);
            ioSerializer.Serialize("name", ref Name, string.Empty, FieldOptions.Optional | FieldOptions.PreferAttribute);
            ioSerializer.Int32Proxy("group", ref Group, RSGroupId.Null);

            ioSerializer.ObjectArray("components", ref Components, FieldOptions.Optional);
            ioSerializer.Serialize("tableId", ref TableId, -1, FieldOptions.Optional);

            if (ioSerializer.ObjectVersion >= 2)
            {
                ioSerializer.ObjectArray("links", ref Links);
            }
        }

        #endregion // ISerializedObject

        #region IRSEntity

        RSEntityId IRSEntity.Id
        {
            get { return Id; }
            set { Id = value; }
        }

        string IRSEntity.Prefab
        {
            get { return Prefab; }
        }

        string IRSEntity.Name
        {
            get { return Name; }
            set { Name = value; }
        }

        RSGroupId IRSEntity.Group
        {
            get { return Group; }
            set { Group = value; }
        }

        IRSEntityMgr IRSEntity.Manager { get; set; }

        IEnumerable<Type> IRSEntity.GetRSComponentTypes(RSLibrary inLibrary)
        {
            if (Components != null)
            {
                for (int i = 0; i < Components.Length; ++i)
                {
                    RSComponentInfo compInfo = inLibrary.GetComponent(Components[i].ComponentType);
                    if (compInfo != null)
                        yield return compInfo.OwnerType;
                }
            }
        }

        IRSEntityLinkSet IRSEntity.Links
        {
            get
            {
                if (m_LinkAccess == null)
                {
                    m_LinkAccess = new LinksAccessor(this);
                }
                return m_LinkAccess;
            }
        }

        #endregion // IRSEntity
    }
}