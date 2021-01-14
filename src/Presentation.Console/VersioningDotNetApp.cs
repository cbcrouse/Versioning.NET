using McMaster.Extensions.CommandLineUtils;
using System.Reflection;
using System.Threading.Tasks;

namespace Presentation.Console
{
    /// <summary>
    /// The versioning commandline application.
    /// </summary>
    [Command]
    public class VersioningDotNetApp
    {
        private Task<int> OnExecuteAsync()
        {
            string version = Assembly.GetEntryAssembly()!.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            System.Console.WriteLine($"Current version: {version}");
            System.Console.ReadKey();
            return Task.FromResult(0);
        }
    }
}
