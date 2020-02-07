using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using RuleScript.Data;
using UnityEngine;

namespace RuleScript.Runtime
{
    public interface IRSRuntimeEntity : IRSEntity
    {
        #region Properties

        new RSEntityId Id { get; }
        new string Prefab { get; }
        new string Name { get; }
        new RSGroupId Group { get; }

        #endregion // Properties

        #region Components

        IRSRuntimeComponent GetRSComponent(Type inType);
        T GetRSComponent<T>() where T : IRSRuntimeComponent;
        IReadOnlyList<IRSRuntimeComponent> GetRSComponents();

        #endregion // Components

        #region Active/Locked

        bool IsActive();
        bool SetActive(bool inbActive);
        bool SetActiveWithoutNotify(bool inbActive);

        bool IsLocked();
        bool SetLocked(bool inbLocked);

        bool IsAlive();

        #endregion // Active/Locked

        #region Rules

        RSRuntimeRuleTable RuleTable { get; }

        MonoBehaviour ProxyObject { get; }
        RoutinePhase ExecutionPhase { get; }

        #endregion // Rules

        #region Lifecycle

        new IRSRuntimeEntityMgr Manager { get; }

        void InitializeTable();
        void DestroyTable();

        void OnPreInitialize();
        void OnInitialize();
        void OnPostInitialize();

        void OnDestroyed();

        #endregion // Lifecycle

        #region Links

        new IRSRuntimeEntityLinkSet<IRSRuntimeEntity> Links { get; }

        #endregion // Children
    }

    static public class IRSRuntimeEntityExt
    {
        static public void Trigger(this IRSRuntimeEntity inEntity, RSTriggerId inTriggerId, object inArgument = null, bool inbForce = false)
        {
            IRSRuntimeEntityMgr mgr = inEntity?.Manager;
            if (mgr != null && mgr.IsReady() && mgr.Context != null)
            {
                mgr.Context.Trigger(inEntity, inTriggerId, inArgument, inbForce);
            }
        }

        static public void Broadcast(this IRSRuntimeEntity inEntity, RSTriggerId inTriggerId, object inArgument = null, bool inbForce = false)
        {
            IRSRuntimeEntityMgr mgr = inEntity?.Manager;
            if (mgr != null && mgr.IsReady() && mgr.Context != null)
            {
                mgr.Context.Broadcast(inTriggerId, inArgument, inbForce);
            }
        }

        static public bool IsListeningForTrigger(this IRSRuntimeEntity inEntity, RSTriggerId inTriggerId)
        {
            if (inTriggerId == RSTriggerId.Null)
                return false;

            RSTriggerId[] triggers = (inEntity.RuleTable?.Data?.UniqueTriggers);
            if (triggers != null)
                return Array.IndexOf(triggers, inTriggerId) >= 0;

            return false;
        }
    }
}