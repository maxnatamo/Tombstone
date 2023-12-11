namespace Tombstone.Syntax
{
    [Flags]
    public enum SyntaxNodeModifier
    {
        None        = 0,
        Abstract    = 1,
        Async       = 2,
        Const       = 4,
        Event       = 8,
        Extern      = 16,
        Override    = 32,
        ReadOnly    = 64,
        Sealed      = 128,
        Static      = 256,
        Unsafe      = 512,
        Virtual     = 1024,
        Volatile    = 2048
    }
}