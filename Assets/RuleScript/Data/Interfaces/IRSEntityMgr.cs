namespace RuleScript.Data
{
    public interface IRSEntityMgr
    {
        IRSEntityLookup<IRSEntity> Lookup { get; }

        bool RegisterEntity(IRSEntity inEntity);
        bool DeregisterEntity(IRSEntity inEntity);
    }
}