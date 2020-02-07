/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSComponentData.cs
 * Purpose: Serializable abstract class for holding data about a component.
*/

using BeauData;

namespace RuleScript.Data
{
    public abstract class RSComponentData : ISerializedObject, ISerializedVersion
    {
        public int ComponentType;

        public virtual ushort Version { get { return 1; } }

        public virtual void Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("type", ref ComponentType);
        }
    }
}