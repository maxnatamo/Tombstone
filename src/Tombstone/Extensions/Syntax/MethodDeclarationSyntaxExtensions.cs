using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Tombstone.Extensions
{
    public static partial class DeclarationSyntaxExtensions
    {
        /// <summary>
        ///   Get the method signature for the given declaration, <paramref name="node"/>.
        /// </summary>
        /// <example>
        ///   Here's an example of what the given method will return.
        ///   <code>
        ///     private static void Method(bool flag, int value = 10)
        ///     { }
        ///
        ///     MethodDeclarationSyntax methodDeclaration = ...;
        ///     string signature = methodSignature.GetMethodSignature();
        ///     
        ///     // signature will contain:
        ///     // Method(bool flag, int value = 10)
        ///   </code>
        /// </example>
        public static string GetMethodSignature(this MethodDeclarationSyntax node)
        {
            ArgumentNullException.ThrowIfNull(node, nameof(node));

            IEnumerable<string> parameterSignatures = 
                node.ParameterList.Parameters
                    .Select(v => v.GetText().ToString());

            string parameterSignature = string.Join(", ", parameterSignatures);

            return
                node.Identifier.ToString() +
                $"({parameterSignature})";
        }
    }
}