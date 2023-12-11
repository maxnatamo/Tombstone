using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace Tombstone
{
    public static class Workspace
    {
        public static MSBuildWorkspace CreateWorkspace()
        {
            VisualStudioInstance instance = GetVisualStudioInstance();
            MSBuildLocator.RegisterMSBuildPath(instance.MSBuildPath);

            return MSBuildWorkspace.Create();
        }

        private static VisualStudioInstance GetVisualStudioInstance()
            => MSBuildLocator.QueryVisualStudioInstances().First();
    }
}