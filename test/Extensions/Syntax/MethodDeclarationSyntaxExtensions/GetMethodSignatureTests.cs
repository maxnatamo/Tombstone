using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Tombstone.Extensions;

namespace Tombstone.Tests.Extensions.Syntax.MethodDeclarationSyntaxExtensionsTests
{
    public class GetMethodSignatureTests
    {
        [Fact]
        public void GetMethodSignature_ReturnsSignature_GivenMethod()
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            // Act
            string signature = node.GetMethodSignature();

            // Assert
            signature.Should().Be("Example()");
        }

        [Fact]
        public void GetMethodSignature_ReturnsSignatureWithoutVisibility_GivenMethodWithVisibilityModifiers()
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            node = node
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));

            // Act
            string signature = node.GetMethodSignature();

            // Assert
            signature.Should().Be("Example()");
        }

        [Theory]
        [InlineData("(int value)", "Example(int value)")]
        [InlineData("(int value, bool flag)", "Example(int value, bool flag)")]
        [InlineData("(int value = 1)", "Example(int value = 1)")]
        public void GetMethodSignature_ReturnsSignature_GivenMethodWithParameters(string parameters, string expected)
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            node = node
                .AddParameterListParameters(
                    SyntaxFactory.ParseParameterList(parameters)
                    .Parameters.ToArray());

            // Act
            string signature = node.GetMethodSignature();

            // Assert
            signature.Should().Be(expected);
        }
    }
}