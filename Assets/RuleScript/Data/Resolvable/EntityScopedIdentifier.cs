using System;
using BeauData;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Data
{
    /// <summary>
    /// Identifier on a scope/target.
    /// </summary>
    [Serializable]
    public struct EntityScopedIdentifier : ISerializedObject, ISerializedVersion, IRSPreviewable, IEquatable<EntityScopedIdentifier>
    {
        public enum Type : byte
        {
            Query,
            Action
        }

        #region Inspector

        [SerializeField] private EntityScopeData m_Scope;
        [SerializeField] private int m_Id;

        #endregion // Inspector

        public EntityScopeData Scope { get { return m_Scope; } }
        public int Id { get { return m_Id; } }

        public EntityScopedIdentifier(EntityScopeData inScope, int inId)
        {
            m_Scope = inScope;
            m_Id = inId;
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Object("scope", ref m_Scope);
            ioSerializer.Serialize("id", ref m_Id);
        }

        #endregion // ISerializedObject

        #region IPreviewable

        string IRSPreviewable.GetPreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            return GetPreviewStringAsQuery(inTriggerContext, inLibrary);
        }

        public string GetPreviewStringAsAction(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            string actionId = inLibrary.GetAction(m_Id)?.Name ?? "null";
            return string.Format("{0}:{1}", m_Scope.GetPreviewString(inTriggerContext, inLibrary), actionId);
        }

        public string GetPreviewStringAsQuery(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            string queryId = inLibrary.GetQuery(m_Id)?.Name ?? "null";
            return string.Format("{0}:{1}", m_Scope.GetPreviewString(inTriggerContext, inLibrary), queryId);
        }

        #endregion // IPreviewable

        #region IEquatable

        public bool Equals(EntityScopedIdentifier other)
        {
            return m_Scope == other.m_Scope
                && m_Id == other.m_Id;
        }

        #endregion // IEquatable

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is EntityScopedIdentifier)
                return Equals((EntityScopedIdentifier) obj);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = m_Scope.GetHashCode();
            hash = (hash << 5) ^ m_Id.GetHashCode();
            return hash;
        }

        static public bool operator==(EntityScopedIdentifier a, EntityScopedIdentifier b)
        {
            return a.Equals(b);
        }

        static public bool operator!=(EntityScopedIdentifier a, EntityScopedIdentifier b)
        {
            return !a.Equals(b);
        }

        #endregion // Overrides
    }
}