using BeauRoutine;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Library
{
    static internal class BaseQueries
    {
        #region Entities

        [RSQuery("entity:identity", Name = "Entity/Self",
            Description = "Returns the identity of the entity")]
        static private IRSRuntimeEntity Self(this IRSRuntimeEntity inEntity)
        {
            return inEntity;
        }

        [RSQuery("entity:name", Name = "Entity/Name",
            Description = "Returns the name of the entity")]
        static private string Name(this IRSRuntimeEntity inEntity)
        {
            return inEntity.Name;
        }

        [RSQuery("entity:prefab-name", Name = "Entity/Prefab Name",
            Description = "Returns the prefab name of the entity")]
        static private string PrefabName(this IRSRuntimeEntity inEntity)
        {
            return inEntity.Prefab;
        }

        [RSQuery("entity:group", Name = "Entity/Group",
            Description = "Returns the group of the entity")]
        static private RSGroupId Group(this IRSRuntimeEntity inEntity)
        {
            return inEntity.Group;
        }

        [RSQuery("entity:is-active", Name = "Entity/Is Active",
            Description = "Returns if the entity is active")]
        static private bool IsActive(this IRSRuntimeEntity inEntity)
        {
            return inEntity.IsActive();
        }

        [RSQuery("entity:is-locked", Name = "Entity/Is Locked",
            Description = "Returns if the entity is prevented from responding to triggers")]
        static private bool IsLocked(this IRSRuntimeEntity inEntity)
        {
            return inEntity.IsLocked();
        }

        #endregion // Entities

        #region Application

        [RSQuery("app:platform", Name = "Application/Platform",
            Description = "Returns the application platform")]
        static private RuntimePlatform Platform()
        {
            return Application.platform;
        }

        [RSQuery("app:system-language", Name = "Application/System Language",
            Description = "Returns the system language")]
        static private SystemLanguage Language()
        {
            return Application.systemLanguage;
        }

        #endregion // Application

        #region Time

        [RSQuery("time:time-scale", Name = "Time/Time Scale",
            Description = "Returns the Unity timescale")]
        static private float TimeScale()
        {
            return Time.timeScale;
        }

        [RSQuery("time:realtime-since-startup", Name = "Time/Realtime Since Startup",
            Description = "Returns the elapsed real time since application startup")]
        static private float RealtimeSinceStartup()
        {
            return Time.realtimeSinceStartup;
        }

        [RSQuery("time:delta-time", Name = "Time/Delta Time",
            Description = "Returns the Unity delta time for the current frame")]
        static private float DeltaTime()
        {
            return Routine.DeltaTime;
        }

        #endregion // Time

        #region Entities

        [RSQuery("entities:count-with-name", Name = "Entities/Count/Entities with Name",
            Description = "Returns the number of entities with the given name")]
        static public int EntityCountWithName(IScriptContext inContext,
            [RSParameter("Name Filter", "Entity name. Supports leading and trailing wildcard")] string inName)
        {
            return inContext.Entities.Lookup.CountEntitiesWithName(inName);
        }

        [RSQuery("entities:count-with-group", Name = "Entities/Count/Entities with Group",
            Description = "Returns the number of entities with the given group")]
        static public int EntityCountWithGroup(IScriptContext inContext,
            [RSParameter("Group Filter", "Entity group")] RSGroupId inGroup)
        {
            return inContext.Entities.Lookup.CountEntitiesWithGroup(inGroup);
        }

        [RSQuery("entities:count-with-prefab", Name = "Entities/Count/Entities with Prefab",
            Description = "Returns the number of entities with the given prefab name")]
        static public int EntityCountWithPrefab(IScriptContext inContext,
            [RSParameter("Prefab Filter", "Entity prefab name. Supports leading and trailing wildcard.")] string inPrefab)
        {
            return inContext.Entities.Lookup.CountEntitiesWithPrefab(inPrefab);
        }

        #endregion // Entities

        #region Math

        [RSQuery("math:round", Name = "Math/Round")]
        static private int Round([RSParameter("Number")] float inNumber)
        {
            return Mathf.RoundToInt(inNumber);
        }

        [RSQuery("math:floor", Name = "Math/Floor")]
        static private int Floor([RSParameter("Number")] float inNumber)
        {
            return Mathf.FloorToInt(inNumber);
        }

        [RSQuery("math:ceil", Name = "Math/Ceil")]
        static private int Ceil([RSParameter("Number")] float inNumber)
        {
            return Mathf.CeilToInt(inNumber);
        }

        [RSQuery("math:abs", Name = "Math/Absolute Value")]
        static private float Abs([RSParameter("Number")] float inNumber)
        {
            return Mathf.Abs(inNumber);
        }

        [RSQuery("math:clamp", Name = "Math/Clamp")]
        static private float Clamp([RSParameter("Number")] float inNumber, [RSParameter("Min")] float inMin, [RSParameter("Max")] float inMax)
        {
            return Mathf.Clamp(inNumber, inMin, inMax);
        }

        [RSQuery("math:lerp", Name = "Math/Lerp")]
        static private float Lerp([RSParameter("Min")] float inMin, [RSParameter("Max")] float inMax, [RSParameter("Percent")] float inPercent)
        {
            return Mathf.LerpUnclamped(inMin, inMax, inPercent);
        }

        [RSQuery("math:inv-lerp", Name = "Math/Inverse Lerp")]
        static private float InvLerp([RSParameter("Value")] float inValue, [RSParameter("Min")] float inMin, [RSParameter("Max")] float inMax)
        {
            return Mathf.InverseLerp(inValue, inMin, inMax);
        }

        [RSQuery("math:min2", Name = "Math/Min/2")]
        static private float Min2([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1)
        {
            return Mathf.Min(inNumber0, inNumber1);
        }

        [RSQuery("math:min3", Name = "Math/Min/3")]
        static private float Min3([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2)
        {
            return Mathf.Min(inNumber0, inNumber1, inNumber2);
        }

        [RSQuery("math:min4", Name = "Math/Min/4")]
        static private float Min4([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2, [RSParameter("Number 3")] float inNumber3)
        {
            return Mathf.Min(inNumber0, inNumber1, inNumber2, inNumber3);
        }

        [RSQuery("math:max2", Name = "Math/Max/2")]
        static private float Max2([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1)
        {
            return Mathf.Max(inNumber0, inNumber1);
        }

        [RSQuery("math:max3", Name = "Math/Max/3")]
        static private float Max3([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2)
        {
            return Mathf.Max(inNumber0, inNumber1, inNumber2);
        }

        [RSQuery("math:max4", Name = "Math/Max/4")]
        static private float Max4([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2, [RSParameter("Number 3")] float inNumber3)
        {
            return Mathf.Max(inNumber0, inNumber1, inNumber2, inNumber3);
        }

        [RSQuery("math:sign", Name = "Math/Sign")]
        static private float Sign([RSParameter("Number")] float inNumber)
        {
            return Mathf.Sign(inNumber);
        }

        [RSQuery("math:add2", Name = "Math/Add/2")]
        static private float Add2([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1)
        {
            return inNumber0 + inNumber1;
        }

        [RSQuery("math:add3", Name = "Math/Add/3")]
        static private float Add3([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2)
        {
            return inNumber0 + inNumber1 + inNumber2;
        }

        [RSQuery("math:add4", Name = "Math/Add/4")]
        static private float Add4([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2, [RSParameter("Number 3")] float inNumber3)
        {
            return inNumber0 + inNumber1 + inNumber2 + inNumber3;
        }

        [RSQuery("math:sub2", Name = "Math/Subtract/2")]
        static private float Subtract2([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1)
        {
            return inNumber0 - inNumber1;
        }

        [RSQuery("math:sub3", Name = "Math/Subtract/3")]
        static private float Subtract3([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2)
        {
            return inNumber0 - inNumber1 - inNumber2;
        }

        [RSQuery("math:sub4", Name = "Math/Subtract/4")]
        static private float Subtract4([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2, [RSParameter("Number 3")] float inNumber3)
        {
            return inNumber0 - inNumber1 - inNumber2 - inNumber3;
        }

        [RSQuery("math:multiply2", Name = "Math/Multiply/2")]
        static private float Multiply2([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1)
        {
            return inNumber0 * inNumber1;
        }

        [RSQuery("math:multiply3", Name = "Math/Multiply/3")]
        static private float Multiply3([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2)
        {
            return inNumber0 * inNumber1 * inNumber2;
        }

        [RSQuery("math:multiply4", Name = "Math/Multiply/4")]
        static private float Multiply4([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2, [RSParameter("Number 3")] float inNumber3)
        {
            return inNumber0 * inNumber1 * inNumber2 * inNumber3;
        }

        [RSQuery("math:divide2", Name = "Math/Divide/2")]
        static private float Divide2([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1)
        {
            return inNumber0 / inNumber1;
        }

        [RSQuery("math:divide3", Name = "Math/Divide/3")]
        static private float Divide3([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2)
        {
            return inNumber0 / inNumber1 / inNumber2;
        }

        [RSQuery("math:divide4", Name = "Math/Divide/4")]
        static private float Divide4([RSParameter("Number 0")] float inNumber0, [RSParameter("Number 1")] float inNumber1, [RSParameter("Number 2")] float inNumber2, [RSParameter("Number 3")] float inNumber3)
        {
            return inNumber0 / inNumber1 / inNumber2 / inNumber3;
        }

        #endregion // Math

        #region Color

        [RSQuery("color:lerp", Name = "Color/Lerp")]
        static private Color Lerp([RSParameter("Min")] Color inMin, [RSParameter("Max")] Color inMax, [RSParameter("Percent")] float inPercent)
        {
            return Color.LerpUnclamped(inMin, inMax, inPercent);
        }

        [RSQuery("color:multiply", Name = "Color/Multiply")]
        static private Color Multiply([RSParameter("Color 0")] Color inColor0, [RSParameter("Color 1")] Color inColor1)
        {
            return inColor0 * inColor1;
        }

        [RSQuery("color:with-alpha", Name = "Color/With Alpha")]
        static private Color WithAlpha([RSParameter("Color")] Color inColor, [RSParameter("Alpha")] float inAlpha)
        {
            inColor.a = inAlpha;
            return inColor;
        }

        [RSQuery("color:red", Name = "Color/Red")]
        static private float Red([RSParameter("Color")] Color inColor0)
        {
            return inColor0.r;
        }

        [RSQuery("color:green", Name = "Color/Green")]
        static private float Green([RSParameter("Color")] Color inColor0)
        {
            return inColor0.g;
        }

        [RSQuery("color:blue", Name = "Color/Blue")]
        static private float Blue([RSParameter("Color")] Color inColor0)
        {
            return inColor0.b;
        }

        [RSQuery("color:alpha", Name = "Color/Alpha")]
        static private float Alpha([RSParameter("Color")] Color inColor0)
        {
            return inColor0.a;
        }

        #endregion // Color
    }
}