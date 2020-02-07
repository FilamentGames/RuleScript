using System;
using System.Collections.Generic;
using BeauData;
using RuleScript.Data;
using UnityEngine;

namespace RuleScript.Metadata
{
    public sealed class RSTypeInfo
    {
        public readonly Type SystemType;
        public readonly string FriendlyName;
        public readonly RSValue DefaultValue;
        public readonly TypeFlags Flags;

        private List<RSTypeInfo> m_Conversions;
        private CompareOperator[] m_AllowedOperators;
        private RSTypeInfo m_BaseType;

        internal RSTypeInfo(Type inType, string inFriendlyName, RSValue inDefault, TypeFlags inFlags = 0)
        {
            SystemType = inType;
            FriendlyName = inFriendlyName ?? inType.Name;
            DefaultValue = inDefault;
            Flags = inFlags | GetFlags(inType);
        }

        internal void InitializeEnum()
        {
            SetBase(RSBuiltInTypes.Enum);
            AddConversions(RSBuiltInTypes.Int);
            AllowNumericComparisons();
        }

        internal void SetBase(RSTypeInfo inBase)
        {
            m_BaseType = inBase;
            AddConversions(m_BaseType);
        }

        internal void AddConversions(params RSTypeInfo[] inTypes)
        {
            if (m_Conversions == null)
                m_Conversions = new List<RSTypeInfo>(inTypes);
            else
                m_Conversions.AddRange(inTypes);
        }

        internal void AddComparisons(params CompareOperator[] inComparisons)
        {
            int currentSize = m_AllowedOperators != null ? m_AllowedOperators.Length : 0;
            Array.Resize(ref m_AllowedOperators, currentSize + inComparisons.Length);
            inComparisons.CopyTo(m_AllowedOperators, currentSize);
        }

        internal void AllowNumericComparisons()
        {
            AddComparisons(CompareOperator.LessThanOrEqualTo, CompareOperator.LessThan, CompareOperator.EqualTo, CompareOperator.NotEqualTo, CompareOperator.GreaterThan, CompareOperator.GreaterThanOrEqualTo);
        }

        internal void AllowEqualityComparisons()
        {
            AddComparisons(CompareOperator.EqualTo, CompareOperator.NotEqualTo);
        }

        internal void AllowBooleanComparisons()
        {
            AddComparisons(CompareOperator.True, CompareOperator.False);
        }

        internal void AllowStringComparisons()
        {
            AddComparisons(CompareOperator.Contains, CompareOperator.DoesNotContain,
                CompareOperator.StartsWith, CompareOperator.DoesNotStartWith,
                CompareOperator.EndsWith, CompareOperator.DoesNotEndWith,
                CompareOperator.IsEmpty, CompareOperator.IsNotEmpty,
                CompareOperator.Matches, CompareOperator.DoesNotMatch);
        }

        public bool CanConvert(RSTypeInfo inTarget)
        {
            if ((Flags & TypeFlags.SkipConversionCheck) != 0)
                return true;

            if (inTarget == this || inTarget == m_BaseType)
                return true;

            if (inTarget == RSBuiltInTypes.String || inTarget == RSBuiltInTypes.Any)
                return true;

            if ((inTarget.Flags & TypeFlags.IsEnum) != 0 && (Flags & TypeFlags.IsEnum) != 0)
                return true;

            if (m_Conversions != null && m_Conversions.Contains(inTarget))
                return true;

            if (m_BaseType != null)
                return m_BaseType.CanConvert(inTarget);

            return false;
        }

        public CompareOperator[] AllowedOperators()
        {
            return m_AllowedOperators;
        }

        public bool IsOperatorAllowed(CompareOperator inOperator)
        {
            return Array.IndexOf(m_AllowedOperators, inOperator) >= 0;
        }

        static internal TypeFlags GetFlags(Type inType)
        {
            TypeFlags flags = 0;
            if (inType.IsEnum)
            {
                flags |= TypeFlags.IsEnum;
                if (inType.IsDefined(typeof(FlagsAttribute), true))
                    flags |= TypeFlags.IsFlags;
            }
            if (typeof(IRSEntity).IsAssignableFrom(inType))
            {
                flags |= TypeFlags.IsEntity;
            }
            if (typeof(IRSComponent).IsAssignableFrom(inType))
            {
                flags |= TypeFlags.IsComponent;
            }
            return flags;
        }

        public override string ToString()
        {
            return FriendlyName;
        }
    }
}