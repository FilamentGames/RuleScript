namespace RuleScript.Data
{
    public enum EntityScopeType : byte
    {
        Null = 0, // no entity
        
        // NO ARGS, SINGLE TARGET
        Self = 1, // evaluating entity
        Argument = 2, // trigger argument as entity
        Global = 3, // global entity

        // ONE ARG, SINGLE TARGET
        ObjectById = 8, // entity with the given id
        ObjectInRegister = 9, // entity at the given register

        // ONE ARG, MULTIPLE TARGETS
        ObjectsWithGroup = 16, // entities with the given group
        ObjectsWithName = 17, // entities with the given name (supports wildcard)
        ObjectsWithPrefab = 18, // entities with the given prefab name (supports wildcard)

        // INVALIDATED
        Invalid = 255 // Invalid entity
    }
}