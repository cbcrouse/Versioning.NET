using McMaster.Extensions.CommandLineUtils;

namespace Presentation.Console.Commands
{
    /// <summary>
    /// The versioning commandline application.
    /// </summary>
    [Command("dotnet-version")]
    [Subcommand(typeof(IncrementVersion), typeof(IncrementVersionWithGit))]
    public class App
    {
        // ReSharper disable once UnusedMember.Local
        private int OnExecuteAsync(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }
    }
}
