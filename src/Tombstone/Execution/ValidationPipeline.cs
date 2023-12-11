namespace Tombstone.Execution
{
    /// <summary>
    ///   Pipeline for executing the entire validation ruleset.
    /// </summary>
    public interface IValidationPipeline
    {
        /// <summary>
        ///   Execute the entire validation ruleset against the given context, <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to validate.</param>
        /// <param name="cancellationToken">An optional cancellation token.</param>
        Task ExecuteAsync(
            SyntaxContext context,
            CancellationToken cancellationToken);
    }

    public class ValidationPipeline(IEnumerable<ValidationRuleBase> rules) : IValidationPipeline
    {
        public async Task ExecuteAsync(
            SyntaxContext context,
            CancellationToken cancellationToken)
        {
            foreach(ValidationRuleBase rule in rules)
            {
                await rule.ValidateAsync(context, cancellationToken);
            }
        }
    }
}