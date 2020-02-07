/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSParameterAttribute.cs
 * Purpose: Attribute providing additional info about a parameter.
 */

using System;

namespace RuleScript
{
    /// <summary>
    /// Tags a parameter with additional metadata.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RSParameterAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public bool NotNull { get; set; }
        public Type TriggerParameterType { get; set; }

        public RSParameterAttribute() { }

        public RSParameterAttribute(string inName, string inDescription = null)
        {
            Name = inName;
            Description = inDescription ?? string.Empty;
        }
    }
}