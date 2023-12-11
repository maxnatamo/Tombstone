namespace Tombstone.Syntax
{
    [Flags]
    public enum SyntaxNodeVisibility
    {
        None                = 0,
        Public              = 1,
        Protected           = 2,
        Internal            = 4,
        Private             = 8,
        ProtectedInternal   = Protected | Internal,
        PrivateProtected    = Private | Protected
    }
}