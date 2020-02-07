using System.Collections;
using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuleScript.Library
{
    static internal class BaseActions
    {
        #region Entity Active

        [RSAction("entity:activate", Name = "Entity/Active/Activate",
            Description = "Activates this entity")]
        static private void Activate(this IRSRuntimeEntity inEntity)
        {
            inEntity.SetActive(true);
        }

        [RSAction("entity:deactivate", Name = "Entity/Active/Deactivate",
            Description = "Deactivates this entity")]
        static private void Deactivate(this IRSRuntimeEntity inEntity)
        {
            inEntity.SetActive(false);
        }

        [RSAction("entity:toggle-active", Name = "Entity/Active/Toggle Active",
            Description = "Toggles this entity's active state")]
        static private void ToggleActive(this IRSRuntimeEntity inEntity)
        {
            inEntity.SetActive(!inEntity.IsActive());
        }

        [RSAction("entity:set-active", Name = "Entity/Active/Set Active",
            Description = "Sets this entity's active state")]
        static private void SetActive(this IRSRuntimeEntity inEntity, [RSParameter("Active")] bool inbActive)
        {
            inEntity.SetActive(inbActive);
        }

        #endregion // Entity Active

        #region Entity Lock

        [RSAction("entity:lock", Name = "Entity/Lock/Lock",
            Description = "Locks this entity from responding to triggers")]
        static private void Lock(this IRSRuntimeEntity inEntity)
        {
            inEntity.SetLocked(true);
        }

        [RSAction("entity:unlock", Name = "Entity/Lock/Unlock",
            Description = "Unlocks this entity from responding to triggers")]
        static private void Unlock(this IRSRuntimeEntity inEntity)
        {
            inEntity.SetLocked(false);
        }

        [RSAction("entity:toggle-locked", Name = "Entity/Lock/Toggle Locked",
            Description = "Toggles whether or not the entity will respond to triggers")]
        static private void ToggleLocked(this IRSRuntimeEntity inEntity)
        {
            inEntity.SetLocked(!inEntity.IsLocked());
        }

        [RSAction("entity:set-locked", Name = "Entity/Lock/Set Locked",
            Description = "Sets whether or not the entity will respond to triggers")]
        static private void SetLocked(this IRSRuntimeEntity inEntity, [RSParameter("Locked")] bool inbLocked)
        {
            inEntity.SetLocked(inbLocked);
        }

        #endregion // Entity Lock

        #region Trigger

        [RSAction("entity:trigger", Name = "Entity/Trigger",
            Description = "Sends a trigger to the given entity")]
        static private void DispatchTrigger(this IRSRuntimeEntity inEntity, IScriptContext inContext, [RSParameter("Trigger", TriggerParameterType = typeof(void))] RSTriggerId inTrigger)
        {
            inContext?.Trigger(inEntity, inTrigger);
        }

        [RSAction("entity:trigger-string", Name = "Entity/Trigger With String",
            Description = "Sends a trigger to the given entity, with a string parameter")]
        static private void DispatchTriggerWithString(this IRSRuntimeEntity inEntity, IScriptContext inContext, [RSParameter("Trigger", TriggerParameterType = typeof(string))] RSTriggerId inTrigger, [RSParameter("Trigger Parameter")] string inArgument)
        {
            inContext?.Trigger(inEntity, inTrigger, inArgument);
        }

        [RSAction("entity:broadcast", Name = "Entity/Broadcast",
            Description = "Broadcasts a trigger to all entities listening for it")]
        static private void BroadcastTrigger(IScriptContext inContext, [RSParameter("Trigger", TriggerParameterType = typeof(void))] RSTriggerId inTrigger)
        {
            inContext?.Broadcast(inTrigger);
        }

        [RSAction("entity:broadcast-string", Name = "Entity/Broadcast With String",
            Description = "Broadcasts a trigger to all entities listening for it, with a string parameter")]
        static private void BroadcastTriggerWithString(IScriptContext inContext, [RSParameter("Trigger", TriggerParameterType = typeof(string))] RSTriggerId inTrigger, [RSParameter("Trigger Parameter")] string inArgument)
        {
            inContext?.Broadcast(inTrigger, inArgument);
        }

        #endregion // Trigger

        #region Registers

        [RSAction("__:load-register", Name = "Local Vars/Load", UsesRegisters = true,
            Description = "Loads a value into the given local variable")]
        static private void LoadRegister(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] RSValue inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-int", Name = "Local Vars/Load Int", UsesRegisters = true,
            Description = "Loads an integer into the given local variable")]
        static private void LoadRegisterInt(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] int inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-float", Name = "Local Vars/Load Float", UsesRegisters = true,
            Description = "Loads a float into the given local variable")]
        static private void LoadRegisterFloat(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] float inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-bool", Name = "Local Vars/Load Bool", UsesRegisters = true,
            Description = "Loads a bool into the given local variable")]
        static private void LoadRegisterBool(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] bool inbValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inbValue);
            inContext.LoadRegister(inRegister, inbValue);
        }

        [RSAction("__:load-register-string", Name = "Local Vars/Load String", UsesRegisters = true,
            Description = "Loads a string into the given local variable")]
        static private void LoadRegisterString(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] string inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-color", Name = "Local Vars/Load Color", UsesRegisters = true,
            Description = "Loads a color into the given local variable")]
        static private void LoadRegisterColor(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] Color inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-entity", Name = "Local Vars/Load Entity", UsesRegisters = true,
            Description = "Loads an entity into the given local variable")]
        static private void LoadRegisterEntity(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] IRSRuntimeEntity inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-vector2", Name = "Local Vars/Load Vector2", UsesRegisters = true,
            Description = "Loads a Vector2 into the given local variable")]
        static private void LoadRegisterVector2(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] Vector2 inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-vector3", Name = "Local Vars/Load Vector3", UsesRegisters = true,
            Description = "Loads a Vector3 into the given local variable")]
        static private void LoadRegisterVector3(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] Vector3 inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        [RSAction("__:load-register-vector4", Name = "Local Vars/Load Vector4", UsesRegisters = true,
            Description = "Loads a Vector4 into the given local variable")]
        static private void LoadRegisterVector4(IScriptContext inContext, [RSParameter("Register")] RegisterIndex inRegister, [RSParameter("Value")] Vector4 inValue)
        {
            inContext?.Logger?.Log("Loading register {0} with value {1}", inRegister, inValue);
            inContext.LoadRegister(inRegister, inValue);
        }

        #endregion // Registers

        #region Rules

        [RSAction("entity:enable-rule", Name = "Entity/Rules/Enable Rule",
            Description = "Enables all rules with the given name on this entity")]
        static private void EnableRule(this IRSRuntimeEntity inEntity, [RSParameter("Rule Name")] string inRuleName)
        {
            inEntity.RuleTable.EnableRule(inRuleName);
        }

        [RSAction("entity:disable-rule", Name = "Entity/Rules/Disable Rule",
            Description = "Disables all rules with the given name on this entity")]
        static private void DisableRule(this IRSRuntimeEntity inEntity, [RSParameter("Rule Name")] string inRuleName)
        {
            inEntity.RuleTable.DisableRule(inRuleName);
        }

        [RSAction("entity:stop-rule", Name = "Entity/Rules/Stop Rule",
            Description = "Stops any running instances of the rules with the given name on this entity")]
        static private void StopRule(this IRSRuntimeEntity inEntity, [RSParameter("Rule Name")] string inRuleName)
        {
            inEntity.RuleTable.StopRule(inRuleName);
        }

        #endregion // Rules

        #region Application

        [RSAction("app:load-scene", Name = "Application/Load Scene",
            Description = "Loads a new scene in Unity")]
        static private void LoadScene([RSParameter("Scene Name")] string inSceneName, [RSParameter("Load Scene Mode")] LoadSceneMode inLoadMode = LoadSceneMode.Single)
        {
            SceneManager.LoadScene(inSceneName, inLoadMode);
        }

        #endregion // Application

        #region Time

        [RSAction("time:set-time-scale", Name = "Time/Set Time Scale",
            Description = "Sets the Unity timescale")]
        static private void SetTimeScale([RSParameter("Time Scale")] float inTimeScale = 1)
        {
            Time.timeScale = inTimeScale;
        }

        [RSAction("time:wait", Name = "Time/Wait",
            Description = "Waits the given number of seconds before proceeding")]
        static private IEnumerator Wait([RSParameter("Seconds")] float inSeconds)
        {
            yield return inSeconds;
        }

        #endregion // Time

        #region Other

        [RSAction("debug:breakpoint", Name = "Debug/Breakpoint",
            Description = "Pauses Unity execution at the end of this frame")]
        static private void Trace()
        {
            Debug.Break();
        }

        [RSAction("debug:trace", Name = "Debug/Trace",
            Description = "Traces a value to the Unity console")]
        static private void Trace(IScriptContext inContext, [RSParameter("Value")] string inValue)
        {
            inContext?.Logger?.Log(inValue);
        }

        #endregion // Other
    }
}