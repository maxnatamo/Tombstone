using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Tombstone.Extensions;

namespace Tombstone.Execution
{
    public class DiagnosticMessage(CSharpSyntaxNode node)
    {
        /// <summary>
        ///   Location of the symbol.
        /// </summary>
        public Location Location => node.GetLocation();

        public override string ToString() =>
            node switch
            {
                MethodDeclarationSyntax n => n.GetMethodSignature(),

                _ => node.ToString()
            };
    }
}