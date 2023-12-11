using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Serilog.Core;

using Tombstone.Extensions;
using Tombstone.Syntax;

namespace Tombstone.Execution
{
    public class ValidateMethodInvocation(Logger logger) : ValidationRuleBase
    {
        public override async Task ValidateAsync(
            SyntaxContext context,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(context);

            IAsyncEnumerable<MethodDeclarationSyntax> methodsDeclarations =
                this.GetMethodDeclarationsAsync(
                    context,
                    cancellationToken);

            IAsyncEnumerable<MethodDeclarationSyntax> methodInvocations =
                this.GetReferencedPropertiesAsync<MethodDeclarationSyntax, InvocationExpressionSyntax>(
                    context,
                    cancellationToken);

            await foreach(MethodDeclarationSyntax declaration in methodsDeclarations)
            {
                bool isMethodCalled = await methodInvocations.AnyAsync(
                    v => v.Equals(declaration),
                    cancellationToken);

                if(!isMethodCalled)
                {
                    DiagnosticMessage diagnostic = new(declaration);
                    FileLinePositionSpan positionSpan =
                        diagnostic.Location.SourceTree!.GetLineSpan(
                            diagnostic.Location.SourceSpan,
                            cancellationToken);

                    logger.Error(
                        "{FilePath}({Line},{Column}) Method '{Signature}' can be removed, as it's not used.",
                        positionSpan.Path,
                        positionSpan.StartLinePosition.Line + 1,
                        positionSpan.StartLinePosition.Character + 1,
                        diagnostic.ToString());
                }
            }
        }

        /// <summary>
        ///   Retrieve all of the method declarations in the solution, that are viable to be validated.
        /// </summary>
        private async IAsyncEnumerable<MethodDeclarationSyntax> GetMethodDeclarationsAsync(
            SyntaxContext context,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            IAsyncEnumerable<MethodDeclarationSyntax> methodDeclarations =
                this.GetDeclarationsAsync<MethodDeclarationSyntax>(
                    context,
                    cancellationToken);

            await foreach(MethodDeclarationSyntax methodDeclaration in methodDeclarations)
            {
                cancellationToken.ThrowIfCancellationRequested();

                SyntaxNodeModifier modifers = methodDeclaration.GetSymbolModifiers();
                SyntaxNodeVisibility visibility = methodDeclaration.GetSymbolVisibility();

                // Ignore public members, if using a library
                if(context.IgnorePublicMembers && visibility == SyntaxNodeVisibility.Public)
                {
                    continue;
                }

                // Ignore Main method
                if (methodDeclaration.Parent is ClassDeclarationSyntax parentClass &&
                    parentClass.Identifier.ToString() == "Program" &&
                    methodDeclaration.Identifier.ToString() == "Main" &&
                    modifers.HasFlag(SyntaxNodeModifier.Static))
                {
                    continue;
                }

                yield return methodDeclaration;
            }
        }
    }
}