using System.Globalization;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Core;

using Tombstone.Execution;

namespace Tombstone
{
#pragma warning disable CA1062 // Validate arguments of public methods
    public sealed class Program
    {
        private static void PrintHelp()
        {
            AssemblyName appName = Assembly.GetExecutingAssembly().GetName();

            Console.WriteLine("Tombstone ({0})", appName.Version?.ToString(fieldCount: 4) ?? "no version");
            Console.WriteLine("Usage: tombstone [solution-path]");
        }

        public static async Task Main(string[] args)
        {
            if (args.Length <= 0)
            {
                PrintHelp();
                return;
            }

            // Register interrupt handler
            using CancellationTokenSource cancellationTokenSource = new();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();

            // Create logger
            using Logger logger = new LoggerConfiguration()
                .WriteTo.Async(v => v.Console(formatProvider: CultureInfo.InvariantCulture))
                .CreateLogger();

            // Open solution
            using MSBuildWorkspace workspace = Workspace.CreateWorkspace();
            workspace.WorkspaceFailed += (_, e) => logger.Fatal(e.Diagnostic.Message);

            Solution solution = await workspace.OpenSolutionAsync(args[0]);

            // Create service provider
            ServiceCollection services = new();
            services.AddScoped(_ => logger);
            services.AddScoped(_ => solution);
            services.AddScoped<IValidationPipeline, ValidationPipeline>();
            services.AddScoped<IValidationExecutor, ValidationExecutor>();

            // Add validation rules
            services.AddScoped<ValidationRuleBase, ValidateMethodInvocation>();
            services.AddScoped<ValidationRuleBase, ValidateMemberReference>();

            IServiceProvider provider = services.BuildServiceProvider();

            // Finally, validate
            await provider
                .GetRequiredService<IValidationExecutor>()
                .ExecuteAsync(cancellationToken);
        }
    }
#pragma warning restore CA1062 // Validate arguments of public methods
}