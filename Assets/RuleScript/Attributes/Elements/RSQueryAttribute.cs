/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSQueryAttribute.cs
 * Purpose: Attribute tagging a method as a query.
 */

using System;

namespace RuleScript
{
    /// <summary>
    /// Tags a method as a query for rule scripting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class RSQueryAttribute : RSMemberAttribute
    {
        public object DefaultValue;

        public bool UsesRegisters { get; set; }

        public RSQueryAttribute(string inId) : base(inId) { }
    }
}