/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSTriggerAttribute.cs
 * Purpose: Attribute tagging a field as a trigger id.
 */

using System;

namespace RuleScript
{
    /// <summary>
    /// Registers a new trigger type and stores the id on this field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class RSTriggerAttribute : RSMemberAttribute
    {
        public string ParameterName { get; set; }
        public string ParameterDescription { get; set; }
        public Type ParameterType { get; set; }

        public Type OwnerType { get; set; }
        public bool Global { get; set; }

        public RSTriggerAttribute(string inId) : base(inId) { }
    }
}