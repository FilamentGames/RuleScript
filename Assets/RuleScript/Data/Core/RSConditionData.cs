/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSConditionData.cs
 * Purpose: Serializable data for evaluating a condition.
 */

using System;
using BeauData;
using BeauUtil;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    [Serializable]
    public class RSConditionData : ISerializedObject, ISerializedVersion, ICopyCloneable<RSConditionData>, IRSDefaultInitialize, IRSPreviewable
    {
        #region Inspector

        public bool Enabled = true;

        public RSResolvableValueData Query;
        public CompareOperator Operator;
        public RSResolvableValueData Target;

        public Subset MultiQuerySubset;

        #endregion // Inspector

        public RSConditionData()
        {
            DefaultInitialize();
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("enabled", ref Enabled, true, FieldOptions.PreferAttribute);

            ioSerializer.Object("query", ref Query);

            ioSerializer.Enum("operator", ref Operator);
            switch (Operator)
            {
                case CompareOperator.True:
                case CompareOperator.False:
                    break;

                default:
                    ioSerializer.Object("target", ref Target);
                    break;
            }

            if (Query.IsMultiValue())
                ioSerializer.Enum("subset", ref MultiQuerySubset, Subset.All, FieldOptions.PreferAttribute);
        }

        #endregion // ISerializedObject

        #region ICloneable

        public RSConditionData Clone()
        {
            RSConditionData clone = new RSConditionData();
            clone.Enabled = Enabled;
            clone.Query = Query?.Clone();
            clone.Operator = Operator;
            clone.Target = Target?.Clone();
            clone.MultiQuerySubset = MultiQuerySubset;
            return clone;
        }

        public void CopyFrom(RSConditionData inCondition)
        {
            Enabled = inCondition.Enabled;
            Query = inCondition.Query?.Clone();
            Operator = inCondition.Operator;
            Target = inCondition.Target?.Clone();
            MultiQuerySubset = inCondition.MultiQuerySubset;
        }

        #endregion // ICloneable

        #region IDefaultInitialize

        public void DefaultInitialize()
        {
            Enabled = true;
            RSResolvableValueData.SetAsQuery(ref Query, new EntityScopedIdentifier(EntityScopeData.Self(), 0));
            Operator = CompareOperator.True;
            RSResolvableValueData.SetAsValue(ref Target, RSValue.Null);
            MultiQuerySubset = Subset.All;
        }

        #endregion // IDefaultInitialize

        #region IPreviewable

        public string GetPreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            using(PooledStringBuilder psb = PooledStringBuilder.Alloc())
            {
                var sb = psb.Builder;

                if (!Enabled)
                    sb.Append("[Disabled] ");

                sb.Append(Query.GetPreviewString(inTriggerContext, inLibrary));

                sb.Append(" ").Append(Operator.Symbol());

                if (Operator.IsBinary())
                    sb.Append(" ").Append(Target.GetPreviewString(inTriggerContext, inLibrary));

                return sb.ToString();
            }
        }

        #endregion // IPreviewable
    }
}