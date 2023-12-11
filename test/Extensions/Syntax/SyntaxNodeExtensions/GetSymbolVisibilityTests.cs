using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Tombstone.Extensions;
using Tombstone.Syntax;

namespace Tombstone.Tests.Extensions.Syntax.SyntaxNodeExtensionsTests
{
    public class GetSymbolVisibilityTests
    {
        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenInterfaceDeclaration()
        {
            // Arrange
            InterfaceDeclarationSyntax node = SyntaxFactory.InterfaceDeclaration(identifier: "Example");

            // Act
            SyntaxNodeVisibility visibility = node.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Internal);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenClassDeclaration()
        {
            // Arrange
            TypeDeclarationSyntax node = SyntaxFactory.ClassDeclaration(identifier: "Example");

            // Act
            SyntaxNodeVisibility visibility = node.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Internal);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenClassDeclarationWithinType()
        {
            // Arrange
            ClassDeclarationSyntax child = CSharpSyntaxTree.ParseText(@"
                class Parent
                {
                    class Child
                    {

                    }
                }")
                .GetRoot()
                .GetAllOfType<ClassDeclarationSyntax>()
                .First(v => v.Identifier.ToString() == "Child");

            // Act
            SyntaxNodeVisibility visibility = child.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Private);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenDelegateDeclaration()
        {
            // Arrange
            DelegateDeclarationSyntax child = CSharpSyntaxTree.ParseText(@"
                delegate void Test();")
                .GetRoot()
                .GetAllOfType<DelegateDeclarationSyntax>()
                .First();

            // Act
            SyntaxNodeVisibility visibility = child.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Internal);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenDelegateDeclarationWithinType()
        {
            // Arrange
            DelegateDeclarationSyntax childClass = CSharpSyntaxTree.ParseText(@"
                class Parent
                {
                    delegate void Test();
                }")
                .GetRoot()
                .GetAllOfType<DelegateDeclarationSyntax>()
                .First();

            // Act
            SyntaxNodeVisibility visibility = childClass.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Private);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenEnumMemberDeclaration()
        {
            // Arrange
            EnumMemberDeclarationSyntax child = CSharpSyntaxTree.ParseText(@"
                enum Flags
                {
                    Flag
                }")
                .GetRoot()
                .GetAllOfType<EnumMemberDeclarationSyntax>()
                .First();

            // Act
            SyntaxNodeVisibility visibility = child.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Public);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenClassMemberDeclaration()
        {
            // Arrange
            BaseFieldDeclarationSyntax child = CSharpSyntaxTree.ParseText(@"
                class Parent
                {
                    long Member;
                }")
                .GetRoot()
                .GetAllOfType<BaseFieldDeclarationSyntax>()
                .First();

            // Act
            SyntaxNodeVisibility visibility = child.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Private);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenStructMemberDeclaration()
        {
            // Arrange
            BaseFieldDeclarationSyntax child = CSharpSyntaxTree.ParseText(@"
                struct Parent
                {
                    long Member;
                }")
                .GetRoot()
                .GetAllOfType<BaseFieldDeclarationSyntax>()
                .First();

            // Act
            SyntaxNodeVisibility visibility = child.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Private);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsDefaultVisibility_GivenInterfaceMemberDeclaration()
        {
            // Arrange
            MethodDeclarationSyntax child = CSharpSyntaxTree.ParseText(@"
                interface IParent
                {
                    void Method();
                }")
                .GetRoot()
                .GetAllOfType<MethodDeclarationSyntax>()
                .First();

            // Act
            SyntaxNodeVisibility visibility = child.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(SyntaxNodeVisibility.Public);
        }

        [Theory]
        [InlineData(SyntaxKind.PublicKeyword, SyntaxNodeVisibility.Public)]
        [InlineData(SyntaxKind.ProtectedKeyword, SyntaxNodeVisibility.Protected)]
        [InlineData(SyntaxKind.PrivateKeyword, SyntaxNodeVisibility.Private)]
        [InlineData(SyntaxKind.InternalKeyword, SyntaxNodeVisibility.Internal)]
        public void GetSymbolVisibility_ReturnsVisibility_GivenSingleModifier(SyntaxKind modifier, SyntaxNodeVisibility expected)
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            node = node.AddModifiers(SyntaxFactory.Token(modifier));

            // Act
            SyntaxNodeVisibility visibility = node.GetSymbolVisibility();

            // Assert
            visibility.Should().Be(expected);
        }

        [Fact]
        public void GetSymbolVisibility_ReturnsVisibility_GivenMultipleModifiers()
        {
            // Arrange
            MethodDeclarationSyntax node = SyntaxFactory.MethodDeclaration(
                returnType: SyntaxFactory.ParseTypeName("void"),
                identifier: "Example");

            node = node
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.ProtectedKeyword))
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.InternalKeyword));

            // Act
            SyntaxNodeVisibility visibility = node.GetSymbolVisibility();

            // Assert
            visibility.Should().HaveFlag(SyntaxNodeVisibility.ProtectedInternal);
        }
    }
}