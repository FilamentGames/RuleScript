using System;
using System.Collections.Generic;

namespace RuleScript.Data
{
    public struct EntityRef : IEquatable<EntityRef>
    {
        public readonly RSEntityId Entity;
        public readonly string Descriptor;
        public readonly object UserData;

        private EntityRef(RSEntityId inSource, string inDescriptor, object inUserData)
        {
            Entity = inSource;
            Descriptor = inDescriptor;
            UserData = inUserData;
        }

        #region Modifications

        public EntityRef WithDescriptor(string inDescriptor)
        {
            return new EntityRef(Entity, inDescriptor, UserData);
        }

        public EntityRef WithDescriptor(string inDescriptor, object inArgument)
        {
            return new EntityRef(Entity, string.Format(inDescriptor, inArgument), UserData);
        }

        public EntityRef WithDescriptor(string inDescriptor, params object[] inArguments)
        {
            return new EntityRef(Entity, string.Format(inDescriptor, inArguments), UserData);
        }

        public EntityRef WithUserData(object inUserData)
        {
            return new EntityRef(Entity, Descriptor, inUserData);
        }

        #endregion // Modifications

        #region Statics

        static public EntityRef FromEntity(IRSEntity inEntity)
        {
            return new EntityRef(inEntity == null ? RSEntityId.Null : inEntity.Id, null, null);
        }

        #endregion // Statics

        #region IEquatable

        public bool Equals(EntityRef other)
        {
            return Entity == other.Entity &&
                Descriptor == other.Descriptor &&
                UserData == other.UserData;
        }

        #endregion // IEquatable

        // TODO(Beau): Add compare operation?

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is EntityRef)
            {
                return Equals((EntityRef) obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = Entity.GetHashCode();
            if (Descriptor != null)
                hash = (hash >> 4) ^ Descriptor.GetHashCode();
            if (UserData != null)
                hash = (hash << 3) ^ UserData.GetHashCode();
            return hash;
        }

        static public bool operator ==(EntityRef lhs, EntityRef rhs)
        {
            return lhs.Equals(rhs);
        }

        static public bool operator !=(EntityRef lhs, EntityRef rhs)
        {
            return !lhs.Equals(rhs);
        }

        #endregion // Overrides
    }
}