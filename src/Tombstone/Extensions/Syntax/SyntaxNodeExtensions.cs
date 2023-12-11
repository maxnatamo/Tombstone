using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Tombstone.Syntax;

namespace Tombstone.Extensions
{
    public static partial class DeclarationSyntaxExtensions
    {
        /// <summary>
        ///   Get all descendants of the given type, <typeparamref name="T"/>.
        /// </summary>
        /// <param name="includeSelf">Whether to include <paramref name="node"/> in the search.</param>
        public static IEnumerable<T> GetAllOfType<T>(this SyntaxNode node, bool includeSelf = false)
            where T : SyntaxNode
        {
            ArgumentNullException.ThrowIfNull(node, nameof(node));

            IEnumerable<SyntaxNode> descendants = includeSelf
                ? node.DescendantNodesAndSelf()
                : node.DescendantNodes();

            return descendants
                .OfType<T>()
                .Cast<T>();
        }

        /// <summary>
        ///   Get the modifers for the node, <paramref name="node"/>.
        /// </summary>
        public static SyntaxNodeModifier GetSymbolModifiers(this MemberDeclarationSyntax node)
        {
            ArgumentNullException.ThrowIfNull(node, nameof(node));
            SyntaxNodeModifier modifiers = SyntaxNodeModifier.None;

            foreach(SyntaxToken modifier in node.Modifiers)
            {
                modifiers |= modifier.ValueText switch
                {
                    "abstract" => SyntaxNodeModifier.Abstract,
                    "async" => SyntaxNodeModifier.Async,
                    "const" => SyntaxNodeModifier.Const,
                    "event" => SyntaxNodeModifier.Event,
                    "extern" => SyntaxNodeModifier.Extern,
                    "override" => SyntaxNodeModifier.Override,
                    "readonly" => SyntaxNodeModifier.ReadOnly,
                    "sealed" => SyntaxNodeModifier.Sealed,
                    "static" => SyntaxNodeModifier.Static,
                    "unsafe" => SyntaxNodeModifier.Unsafe,
                    "virtual" => SyntaxNodeModifier.Virtual,
                    "volatile" => SyntaxNodeModifier.Volatile,

                    _ => SyntaxNodeModifier.None
                };
            }

            return modifiers;
        }

        /// <summary>
        ///   Get the visibility for the node, <paramref name="node"/>.
        /// </summary>
        public static SyntaxNodeVisibility GetSymbolVisibility(this MemberDeclarationSyntax node)
        {
            ArgumentNullException.ThrowIfNull(node, nameof(node));
            SyntaxNodeVisibility visibility = SyntaxNodeVisibility.None;

            foreach(SyntaxToken modifier in node.Modifiers)
            {
                visibility |= modifier.ValueText switch
                {
                    "public" => SyntaxNodeVisibility.Public,
                    "private" => SyntaxNodeVisibility.Private,
                    "protected" => SyntaxNodeVisibility.Protected,
                    "internal" => SyntaxNodeVisibility.Internal,

                    _ => SyntaxNodeVisibility.None
                };
            }

            if(visibility == SyntaxNodeVisibility.None)
            {
                return node.GetDefaultVisibility();
            }

            return visibility;
        }

        /// <summary>
        ///   Get the default visibility for the type, <paramref name="node"/>.
        /// </summary>
        public static SyntaxNodeVisibility GetDefaultVisibility(this MemberDeclarationSyntax node)
        {
            ArgumentNullException.ThrowIfNull(node, nameof(node));

            return node switch
            {
                // Interface types
                InterfaceDeclarationSyntax
                    => SyntaxNodeVisibility.Internal,

                // Nested types
                TypeDeclarationSyntax when node.Parent is TypeDeclarationSyntax
                    => SyntaxNodeVisibility.Private,

                // Non-nested types
                TypeDeclarationSyntax
                    => SyntaxNodeVisibility.Internal,

                // Nested delegate
                DelegateDeclarationSyntax when node.Parent is TypeDeclarationSyntax
                    => SyntaxNodeVisibility.Private,

                // Non-nested delegate
                DelegateDeclarationSyntax
                    => SyntaxNodeVisibility.Internal,

                // Member types
                MemberDeclarationSyntax m => m switch
                {
                    MemberDeclarationSyntax when node.Parent is EnumDeclarationSyntax
                        => SyntaxNodeVisibility.Public,

                    MemberDeclarationSyntax when node.Parent is ClassDeclarationSyntax
                        => SyntaxNodeVisibility.Private,

                    MemberDeclarationSyntax when node.Parent is StructDeclarationSyntax
                        => SyntaxNodeVisibility.Private,

                    MemberDeclarationSyntax when node.Parent is InterfaceDeclarationSyntax
                        => SyntaxNodeVisibility.Public,

                    _ => SyntaxNodeVisibility.None
                },

                _ => SyntaxNodeVisibility.None
            };
        }
    }
}