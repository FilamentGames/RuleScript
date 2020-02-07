/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSRuleTableData.cs
 * Purpose: Serializable data about a rule table.
*/

using System;
using BeauData;
using BeauUtil;

namespace RuleScript.Data
{
    [Serializable]
    public class RSRuleTableData : ISerializedObject, ISerializedVersion, ICopyCloneable<RSRuleTableData>
    {
        #region Inspector

        public int Id;
        public string Name;

        public RSRuleData[] Rules;
        public RSTriggerId[] UniqueTriggers;

        #endregion // Inspector

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("id", ref Id, FieldOptions.PreferAttribute);
            ioSerializer.Serialize("name", ref Name, string.Empty, FieldOptions.PreferAttribute | FieldOptions.Optional);

            ioSerializer.ObjectArray("rules", ref Rules, FieldOptions.Optional);
            ioSerializer.Int32ProxyArray("triggers", ref UniqueTriggers, FieldOptions.Optional);
        }

        #endregion // ISerializedObject

        #region ICloneable

        public RSRuleTableData Clone()
        {
            RSRuleTableData data = new RSRuleTableData();
            data.Id = Id;
            data.Name = Name;
            data.Rules = CloneUtils.DeepClone(Rules);
            data.UniqueTriggers = CloneUtils.Clone(UniqueTriggers);
            return data;
        }

        public void CopyFrom(RSRuleTableData inTable)
        {
            Id = inTable.Id;
            Name = inTable.Name;
            Rules = CloneUtils.DeepClone(inTable.Rules);
            UniqueTriggers = CloneUtils.Clone(inTable.UniqueTriggers);
        }

        #endregion // ICloneable
    }
}