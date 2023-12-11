using System.Collections.ObjectModel;
using Microsoft.CodeAnalysis;

namespace Tombstone.Execution
{
    public class SyntaxContext
    {
        /// <summary>
        ///   The parent project being validated, which contains <see cref="Document" />.
        /// </summary>
        public Project Project => this._project;

        /// <summary>
        ///   The document being validated.
        /// </summary>
        public Document Document => this._document;

        /// <summary>
        ///   The root of the document syntax tree.
        /// </summary>
        public SyntaxNode TreeRoot => this._treeRoot;

        /// <summary>
        ///   The diagnostics found in the context.
        /// </summary>
        public ReadOnlyCollection<DiagnosticMessage> Diagnostics
            => this._diagnostics.AsReadOnly();

        public SemanticModel SemanticModel => this._semanticModel;

        /// <summary>
        ///   Whether the current project is a library.
        /// </summary>
        public bool IsLibrary
            => this.Project.CompilationOptions?.OutputKind == OutputKind.DynamicallyLinkedLibrary;

        /// <summary>
        ///   Whether to ignore public members, when detecting dead code.
        /// </summary>
        public bool IgnorePublicMembers
            => this.Project.CompilationOptions?.OutputKind == OutputKind.DynamicallyLinkedLibrary;

        private Project _project { get; set; }
        private Document _document { get; set; }
        private SyntaxNode _treeRoot { get; set; }
        private SemanticModel _semanticModel { get; set; }

        private List<DiagnosticMessage> _diagnostics { get; set; } = [];

        public SyntaxContext(
            Project project,
            Document document,
            SyntaxNode root,
            SemanticModel semanticModel)
        {
            this._project = project;
            this._document = document;
            this._treeRoot = root;
            this._semanticModel = semanticModel;
        }

        public void ReportDiagnostic(DiagnosticMessage diagnostic)
            => this._diagnostics.Add(diagnostic);
    }
}