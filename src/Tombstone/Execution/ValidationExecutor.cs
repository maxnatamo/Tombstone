using Microsoft.CodeAnalysis;

namespace Tombstone.Execution
{
    /// <summary>
    ///   Executor for setting up pipelines and reading projects.
    /// </summary>
    public interface IValidationExecutor
    {
        /// <summary>
        ///   Run the executor and setup pipelines.
        /// </summary>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task ExecuteAsync(
            CancellationToken? cancellationToken = null);
    }

    public class ValidationExecutor(
        Solution solution,
        IValidationPipeline pipeline) : IValidationExecutor
    {
        public async Task ExecuteAsync(
            CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;

            foreach (Project project in solution.Projects)
            {
                foreach (Document document in project.Documents)
                {
                    SyntaxNode? syntaxRoot = await document.GetSyntaxRootAsync();
                    SemanticModel? semanticModel = await document
                        .GetSemanticModelAsync(cancellationToken.Value);

                    if(syntaxRoot is null || semanticModel is null)
                    {
                        continue;
                    }

                    SyntaxContext context = new(
                        project,
                        document,
                        syntaxRoot,
                        semanticModel);

                    await pipeline.ExecuteAsync(context, cancellationToken.Value);
                }
            }
        }
    }
}