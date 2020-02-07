/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSActionAttribute.cs
 * Purpose: Attribute tagging a method as an action.
 */

using System;

namespace RuleScript
{
    /// <summary>
    /// Tags a method as an action for rule scripting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class RSActionAttribute : RSMemberAttribute
    {
        public bool IgnoreIfInactive { get; set; }

        public bool UsesRegisters { get; set; }

        public RSActionAttribute(string inId)
            : base(inId)
        { }
    }
}