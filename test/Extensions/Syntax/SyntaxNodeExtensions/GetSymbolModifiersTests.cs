using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Tombstone.Extensions;
using Tombstone.Syntax;

namespace Tombstone.Tests.Extensions.Syntax.SyntaxNodeExtensionsTests
{
    public class GetSymbolModifiersTests
    {
        [Fact]
        public void GetSymbolModifiers_ReturnsNone_GivenEmptyModifiers()
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            // Act
            SyntaxNodeModifier modifiers = node.GetSymbolModifiers();

            // Assert
            modifiers.Should().Be(SyntaxNodeModifier.None);
        }

        [Theory]
        [InlineData(SyntaxKind.AbstractKeyword, SyntaxNodeModifier.Abstract)]
        [InlineData(SyntaxKind.AsyncKeyword, SyntaxNodeModifier.Async)]
        [InlineData(SyntaxKind.ConstKeyword, SyntaxNodeModifier.Const)]
        [InlineData(SyntaxKind.EventKeyword, SyntaxNodeModifier.Event)]
        [InlineData(SyntaxKind.ExternKeyword, SyntaxNodeModifier.Extern)]
        [InlineData(SyntaxKind.OverrideKeyword, SyntaxNodeModifier.Override)]
        [InlineData(SyntaxKind.ReadOnlyKeyword, SyntaxNodeModifier.ReadOnly)]
        [InlineData(SyntaxKind.SealedKeyword, SyntaxNodeModifier.Sealed)]
        [InlineData(SyntaxKind.StaticKeyword, SyntaxNodeModifier.Static)]
        [InlineData(SyntaxKind.UnsafeKeyword, SyntaxNodeModifier.Unsafe)]
        [InlineData(SyntaxKind.VirtualKeyword, SyntaxNodeModifier.Virtual)]
        [InlineData(SyntaxKind.VolatileKeyword, SyntaxNodeModifier.Volatile)]
        public void GetSymbolModifiers_ReturnsModifer_GivenSingleModifier(SyntaxKind modifier, SyntaxNodeModifier expected)
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            node = node.AddModifiers(SyntaxFactory.Token(modifier));

            // Act
            SyntaxNodeModifier modifiers = node.GetSymbolModifiers();

            // Assert
            modifiers.Should().Be(expected);
        }

        [Fact]
        public void GetSymbolModifiers_ReturnsModifers_GivenMultipleModifiers()
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            node = node
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));

            // Act
            SyntaxNodeModifier modifiers = node.GetSymbolModifiers();

            // Assert
            modifiers.Should()
                .HaveFlag(SyntaxNodeModifier.Async).And
                .HaveFlag(SyntaxNodeModifier.Override);
        }
    }
}