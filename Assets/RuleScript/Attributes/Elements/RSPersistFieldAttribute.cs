/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    21 Nov 2019
 * 
 * File:    RSPersistFieldAttribute.cs
 * Purpose: Attribute tagging a field as as a persistent field.
 */

using System;

namespace RuleScript
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false)]
    public sealed class RSPersistFieldAttribute : Attribute
    {
        public readonly string Name;

        public RSPersistFieldAttribute()
        {
            Name = null;
        }

        public RSPersistFieldAttribute(string inName)
        {
            Name = inName;
        }
    }
}