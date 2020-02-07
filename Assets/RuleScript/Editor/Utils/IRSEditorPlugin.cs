/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    IRSEditorPlugin.cs
 * Purpose: Interface for an editor plugin for the rule table editor.
 */

using System;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuleScript.Editor
{
    public interface IRSEditorPlugin
    {
        RSLibrary Library { get; }

        // get rule table for objects
        bool TryGetRuleTable(UnityEngine.Object inObject, out IRSRuleTableSource outTableSource);
        bool TryGetRuleTableManager(UnityEngine.Object inObject, out IRSRuleTableMgr outManager);
        bool TryGetRuleTableManager(Scene inScene, out IRSRuleTableMgr outManager);
        void OnRuleTableModified(IRSRuleTableSource inTable);

        // get entity for objects
        bool TryGetEntity(UnityEngine.Object inObject, out IRSEntity outEntity);
        bool TryGetEntityManager(UnityEngine.Object inObject, out IRSEntityMgr outManager);
        bool TryGetEntityManager(Scene inObject, out IRSEntityMgr outManager);

        // Entity Id inspector
        RSEntityId EntityIdGUIField(GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager, params GUILayoutOption[] inOptions);
        RSEntityId EntityIdGUIField(Rect inPosition, GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager);
        RSEntityId ComponentGUIField(GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager, Type inRequiredType, params GUILayoutOption[] inOptions);
        RSEntityId ComponentGUIField(Rect inPosition, GUIContent inLabel, RSEntityId inValue, IRSEntityMgr inManager, Type inRequiredType);
        IRSRuleTableSource RuleTableSourceField(GUIContent inLabel, IRSRuleTableSource inValue, IRSRuleTableMgr inManager, params GUILayoutOption[] inOptions);
        IRSRuleTableSource RuleTableSourceField(Rect inPosition, GUIContent inLabel, IRSRuleTableSource inValue, IRSRuleTableMgr inManager);
    }
}