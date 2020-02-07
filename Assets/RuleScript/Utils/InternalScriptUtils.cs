using System;
using System.Reflection;
using System.Text;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript
{
    static internal class InternalScriptUtils
    {
        static internal RSValue[] Convert(params object[] inValues)
        {
            RSValue[] converted = new RSValue[inValues.Length];
            for (int i = 0; i < inValues.Length; ++i)
                converted[i] = RSInterop.ToRSValue(inValues[i]);
            return converted;
        }

        static internal void Convert(object[] inValues, RSValue[] ioRSValues)
        {
            for (int i = 0; i < inValues.Length; ++i)
                ioRSValues[i] = RSInterop.ToRSValue(inValues[i]);
        }

        static internal string MemberName(this MemberInfo inInfo)
        {
            return string.Format("{0}::{1}", inInfo.DeclaringType.Name, inInfo.Name);
        }

        static internal Type GetLikelyBindingType(Type inType)
        {
            if (inType == null)
                return null;

            if (typeof(IRSComponent).IsAssignableFrom(inType))
                return inType;

            if (typeof(IRSEntity).IsAssignableFrom(inType))
                return inType;

            if (inType.IsAbstract && inType.IsSealed)
            {
                if (inType.IsNested)
                {
                    Type declaringType = inType.DeclaringType;
                    return GetLikelyBindingType(declaringType);
                }
            }

            return null;
        }
    }
}