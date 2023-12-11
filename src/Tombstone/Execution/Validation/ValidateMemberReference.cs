using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Serilog.Core;

namespace Tombstone.Execution
{
    public class ValidateMemberReference(Logger logger) : ValidationRuleBase
    {
        public override async Task ValidateAsync(
            SyntaxContext context,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(context);

            IAsyncEnumerable<MemberDeclarationSyntax> memberDeclarations =
                this.GetDeclarationsAsync<MemberDeclarationSyntax>(
                    context,
                    cancellationToken);

            IAsyncEnumerable<MemberDeclarationSyntax> memberInvocations =
                this.GetReferencedPropertiesAsync<MemberDeclarationSyntax, InvocationExpressionSyntax>(
                    context,
                    cancellationToken);

            await foreach(MemberDeclarationSyntax declaration in memberDeclarations)
            {
                if (declaration is not FieldDeclarationSyntax &&
                    declaration is not PropertyDeclarationSyntax &&
                    declaration is not EventDeclarationSyntax)
                {
                    continue;
                }

                bool isMethodCalled = await memberInvocations.AnyAsync(
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
                        "{FilePath}({Line},{Column}) Member '{Signature}' can be removed, as it's not used.",
                        positionSpan.Path,
                        positionSpan.StartLinePosition.Line + 1,
                        positionSpan.StartLinePosition.Character + 1,
                        diagnostic.ToString());
                }
            }
        }
    }
}