/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSGroupAttribute.cs
 * Purpose: Attribute tagging a field as a group id.
 */

using System;

namespace RuleScript
{
    /// <summary>
    /// Registers a new entity group and stores the id on this field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class RSGroupAttribute : Attribute
    {
        public readonly string Id;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public RSGroupAttribute(string inId)
        {
            Id = inId;
        }
    }
}