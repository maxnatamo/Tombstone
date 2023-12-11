using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Tombstone.Extensions;
using Tombstone.Syntax;

namespace Tombstone.Execution
{
    public abstract class ValidationRuleBase
    {
        /// <summary>
        ///   Validate the rule against <paramref name="context"/>.
        /// </summary>
        public abstract Task ValidateAsync(
            SyntaxContext context,
            CancellationToken cancellationToken);

        /// <summary>
        ///   Yield all declarations of the given type, <typeparamref name="TSyntax"/>.
        /// </summary>
        /// <remarks>
        ///   If <see cref="SyntaxContext.IgnorePublicMembers"/> is <see langword="true" />, ignores public members.  
        /// </remarks>
        protected async IAsyncEnumerable<TSyntax> GetDeclarationsAsync<TSyntax>(
            SyntaxContext context,
            [EnumeratorCancellation] CancellationToken cancellationToken)
            where TSyntax : MemberDeclarationSyntax
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            foreach(TSyntax node in context.TreeRoot.GetAllOfType<TSyntax>())
            {
                cancellationToken.ThrowIfCancellationRequested();

                SyntaxNodeVisibility visibility = node.GetSymbolVisibility();

                // Ignore public members, if using a library
                if(context.IgnorePublicMembers && visibility == SyntaxNodeVisibility.Public)
                {
                    continue;
                }

                yield return node;
            }

            await Task.CompletedTask;
        }

        /// <summary>
        ///   Get all references of the given type, <typeparamref name="TReference"/>,
        ///   and yield the member that they reference of type <typeparamref name="TMember"/>.
        /// </summary>
        protected async IAsyncEnumerable<TMember> GetReferencedPropertiesAsync<TMember, TReference>(
            SyntaxContext context,
            [EnumeratorCancellation] CancellationToken cancellationToken)
            where TMember : SyntaxNode
            where TReference : SyntaxNode
        {
            ArgumentNullException.ThrowIfNull(context, nameof(context));

            IEnumerable<TReference> referenceExpressions =
                context.TreeRoot.GetAllOfType<TReference>();

            foreach(TReference referenceExpression in referenceExpressions)
            {
                cancellationToken.ThrowIfCancellationRequested();

                SymbolInfo symbol = context.SemanticModel.GetSymbolInfo(
                    referenceExpression,
                    cancellationToken);

                // If no locations are defined,
                // it might come from an external library of sorts.
                if(symbol.Symbol?.Locations.FirstOrDefault() is not Location location)
                {
                    continue;
                }

                // Again, skip if it comes from some unknown source.
                if(location.SourceTree is null)
                {
                    continue;
                }

                SyntaxNode locationRootNode = await location.SourceTree.GetRootAsync(cancellationToken);
                SyntaxNode invokedMethod = locationRootNode.FindNode(location.SourceSpan);

                if(invokedMethod is not TMember property)
                {
                    continue;
                }

                yield return property;
            }
        }
    }
}