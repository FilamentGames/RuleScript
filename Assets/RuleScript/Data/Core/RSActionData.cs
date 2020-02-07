/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSActionData.cs
 * Purpose: Serializable data for executing an action.
 */

using System;
using BeauData;
using BeauUtil;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    [Serializable]
    public class RSActionData : ISerializedObject, ISerializedVersion, ICopyCloneable<RSActionData>, IRSDefaultInitialize, IRSPreviewable
    {
        #region Inspector

        public bool Enabled = true;

        public EntityScopedIdentifier Action;
        public RSResolvableValueData[] Arguments;

        #endregion // Inspector

        public RSActionData()
        {
            DefaultInitialize();
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("enabled", ref Enabled, true, FieldOptions.PreferAttribute);

            ioSerializer.Object("action", ref Action);
            ioSerializer.ObjectArray("args", ref Arguments, FieldOptions.Optional);
        }

        #endregion // ISerializedObject

        #region ICloneable

        public RSActionData Clone()
        {
            RSActionData clone = new RSActionData();
            clone.Enabled = Enabled;
            clone.Action = Action;
            clone.Arguments = CloneUtils.DeepClone(Arguments);
            return clone;
        }

        public void CopyFrom(RSActionData inAction)
        {
            Enabled = inAction.Enabled;
            Action = inAction.Action;
            Arguments = CloneUtils.DeepClone(inAction.Arguments);
        }

        #endregion // ICloneable

        #region IDefaultInitialize

        public void DefaultInitialize()
        {
            Enabled = true;

            Action = new EntityScopedIdentifier(EntityScopeData.Self(), 0);
            Arguments = null;
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

                sb.Append(Action.GetPreviewStringAsAction(inTriggerContext, inLibrary));

                if (Arguments != null && Arguments.Length > 0)
                {
                    sb.Append("(");

                    RSActionInfo actionInfo = inLibrary.GetAction(Action.Id);
                    for (int i = 0; i < Arguments.Length; ++i)
                    {
                        if (i > 0)
                            sb.Append("; ");

                        if (actionInfo != null && i < actionInfo.Parameters.Length)
                            sb.Append(actionInfo.Parameters[i].Name);
                        else
                            sb.Append(i);

                        sb.Append(": ");

                        sb.Append(Arguments[i].GetPreviewString(inTriggerContext, inLibrary));
                    }

                    sb.Append(")");
                }

                return sb.ToString();
            }
        }

        #endregion // IPreviewable
    }
}