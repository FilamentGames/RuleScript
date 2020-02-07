/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSComponentAttribute.cs
 * Purpose: Attribute tagging a component as a component in the scripting system.
*/

using System;

namespace RuleScript
{
    /// <summary>
    /// Tags a class as a component type for rule scripting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class RSComponentAttribute : RSMemberAttribute
    {
        public RSComponentAttribute(string inId)
            : base(inId)
        {
        }
    }
}