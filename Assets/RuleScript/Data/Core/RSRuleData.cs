/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSRuleData.cs
 * Purpose: Serializable data about a rule.
 */

using System;
using BeauData;
using BeauUtil;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    [Serializable]
    public class RSRuleData : ISerializedObject, ISerializedVersion, ICopyCloneable<RSRuleData>, IRSDefaultInitialize, IRSPreviewable
    {
        #region Inspector

        public string Id = null;
        public string Name = null;
        public string RoutineGroup = null;
        public RuleFlags Flags;

        public bool Enabled = true;
        public bool OnlyOnce = false;
        public bool DontInterrupt = false;

        public RSTriggerId TriggerId;

        public RSConditionData[] Conditions;
        public Subset ConditionSubset;

        public RSActionData[] Actions;

        #endregion // Inspector

        public RSRuleData()
        {
            DefaultInitialize();
        }

        public RSRuleData(bool inbAssignId)
        {
            DefaultInitialize();
            if (inbAssignId)
            {
                Id = ScriptUtils.NewId();
            }
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 4; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            if (ioSerializer.ObjectVersion >= 4)
            {
                ioSerializer.Serialize("id", ref Id, string.Empty, FieldOptions.Optional | FieldOptions.PreferAttribute);
            }
            else
            {
                Id = string.Empty;
            }

            ioSerializer.Serialize("name", ref Name, string.Empty, FieldOptions.Optional | FieldOptions.PreferAttribute);
            ioSerializer.Serialize("group", ref RoutineGroup, string.Empty, FieldOptions.Optional | FieldOptions.PreferAttribute);

            if (ioSerializer.ObjectVersion >= 3)
            {
                ioSerializer.Enum("flags", ref Flags, FieldOptions.PreferAttribute);
            }
            else
            {
                Flags = 0;
            }

            ioSerializer.Serialize("enabled", ref Enabled, true, FieldOptions.PreferAttribute);
            ioSerializer.Serialize("onlyOnce", ref OnlyOnce, false, FieldOptions.Optional | FieldOptions.PreferAttribute);
            if (ioSerializer.ObjectVersion >= 2)
            {
                ioSerializer.Serialize("dontInterrupt", ref DontInterrupt, false, FieldOptions.PreferAttribute);
            }
            else
            {
                DontInterrupt = false;
            }

            ioSerializer.Int32Proxy("triggerId", ref TriggerId);

            ioSerializer.ObjectArray("conditions", ref Conditions, FieldOptions.Optional);
            if (Conditions != null && Conditions.Length > 0)
                ioSerializer.Enum("conditionSubset", ref ConditionSubset, Subset.All);

            ioSerializer.ObjectArray("actions", ref Actions);
        }

        #endregion // ISerializedObject

        #region ICloneable

        public RSRuleData Clone()
        {
            RSRuleData clone = new RSRuleData();
            clone.Id = ScriptUtils.NewId();
            clone.Name = Name;
            clone.RoutineGroup = RoutineGroup;
            clone.Enabled = Enabled;
            clone.OnlyOnce = OnlyOnce;
            clone.TriggerId = TriggerId;
            clone.Conditions = CloneUtils.DeepClone(Conditions);
            clone.ConditionSubset = ConditionSubset;
            clone.Actions = CloneUtils.DeepClone(Actions);
            return clone;
        }

        public void CopyFrom(RSRuleData inRule)
        {
            Name = inRule.Name;
            RoutineGroup = inRule.RoutineGroup;
            Enabled = inRule.Enabled;
            OnlyOnce = inRule.OnlyOnce;
            TriggerId = inRule.TriggerId;
            Conditions = CloneUtils.DeepClone(inRule.Conditions);
            ConditionSubset = inRule.ConditionSubset;
            Actions = CloneUtils.DeepClone(inRule.Actions);
        }

        #endregion // ICloneable

        #region IDefaultInitialize

        public void DefaultInitialize()
        {
            Name = "New Rule";
            RoutineGroup = null;
            Enabled = true;
            OnlyOnce = false;
            TriggerId = RSTriggerId.Null;
            Conditions = null;
            ConditionSubset = Subset.All;
            Actions = null;
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

                if (!string.IsNullOrEmpty(Name))
                    sb.Append(Name).Append(" :: ");

                sb.Append(TriggerId.ToString(inLibrary));

                return sb.ToString();
            }
        }

        #endregion // IPreviewable
    }
}