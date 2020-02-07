using System;
using System.Collections;
using System.Collections.Generic;

namespace RuleScript.Runtime
{
    /// <summary>
    /// Result of an action.
    /// </summary>
    internal struct ActionResult : IEquatable<ActionResult>
    {
        public readonly ActionResultType Type;
        public readonly object Value;

        public ActionResult(ActionResultType inCommand)
        {
            Type = inCommand;
            Value = null;
        }

        public ActionResult(ActionResultType inCommand, object inValue)
        {
            Type = inCommand;
            Value = inValue;
        }

        public ActionResult(object inValue)
        {
            Type = inValue is IEnumerator ? ActionResultType.Iterator : ActionResultType.Returned;
            Value = inValue;
        }

        static public ActionResult Default() { return new ActionResult(); }
        static public ActionResult Inactive() { return new ActionResult(ActionResultType.Inactive); }
        static public ActionResult Invalid() { return new ActionResult(ActionResultType.Invalid); }
        static public ActionResult NoEntity() { return new ActionResult(ActionResultType.Invalid_NoEntity); }
        static public ActionResult NoComponent(Type inComponentType) { return new ActionResult(ActionResultType.Invalid_NoComponent, inComponentType); }

        #region IEquatable

        bool IEquatable<ActionResult>.Equals(ActionResult other)
        {
            return Type == other.Type &&
                EqualityComparer<object>.Default.Equals(Value, other.Value);
        }

        #endregion // IEquatable

        #region Overrides

        public override string ToString()
        {
            switch (Type)
            {
                case ActionResultType.Invalid:
                    return "INVALID";

                case ActionResultType.Invalid_NoComponent:
                    return "NO COMPONENT: " + Value?.ToString();

                case ActionResultType.Invalid_NoEntity:
                    return "NO ENTITY";

                case ActionResultType.Inactive:
                    return "ENTITY INACTIVE";

                default:
                    return Value?.ToString() ?? "null";
            }
        }

        public override int GetHashCode()
        {
            int hash = Type.GetHashCode();
            if (Value != null)
                hash = (hash << 5) ^ Value.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ActionResult)
                return Equals((ActionResult) obj);
            return false;
        }

        static public bool operator ==(ActionResult inA, ActionResult inB)
        {
            return inA.Equals(inB);
        }

        static public bool operator !=(ActionResult inA, ActionResult inB)
        {
            return !inA.Equals(inB);
        }

        #endregion // Overrides
    }

    public enum ActionResultType : byte
    {
        Returned,
        Iterator,

        Inactive = 128,

        Invalid_NoComponent = 253,
        Invalid_NoEntity = 254,
        Invalid = 255,
    }
}