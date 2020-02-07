using BeauUtil;

namespace RuleScript.Data
{
    [LabeledEnum]
    public enum RegisterIndex : byte
    {
        [Label("Register 0")]
        Register0,

        [Label("Register 1")]
        Register1,

        [Label("Register 2")]
        Register2,

        [Label("Register 3")]
        Register3,

        [Label("Register 4")]
        Register4,

        [Label("Register 5")]
        Register5,

        [Label("Register 6")]
        Register6,

        [Label("Register 7")]
        Register7,

        [Hidden]
        Invalid = 255,
    }

    static internal class RSRegisters
    {
        public const int MaxRegisters = 8;
    }
}