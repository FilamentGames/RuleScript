/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSMemberAttribute.cs
 * Purpose: Base class for rule script attributes.
*/

using System;

namespace RuleScript
{
    /// <summary>
    /// Base class for a rule script attribute.
    /// </summary>
    public abstract class RSMemberAttribute : Attribute
    {
        public readonly string Id;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }

        public RSMemberAttribute(string inId)
        {
            Id = inId;
        }
    }
}