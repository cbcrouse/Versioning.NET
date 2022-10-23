using McMaster.Extensions.CommandLineUtils;

namespace Presentation.Console.Commands
{
    /// <summary>
    /// The versioning commandline application.
    /// </summary>
    [Command("dotnet-version")]
    [Subcommand(typeof(IncrementVersion), typeof(IncrementVersionWithGit), typeof(IncrementVersionWithGitHints))]
    public class App
    {
        /// <summary>
        /// Show the version information.
        /// </summary>
        [Option(Description = "Show the version information.")]
        public bool Version { get; set; }

        // ReSharper disable once UnusedMember.Local
        private int OnExecuteAsync(CommandLineApplication app)
        {
            if (Version)
            {
                app.VersionOptionFromAssemblyAttributes(typeof(App).Assembly);
                app.ShowVersion();
                return 0;
            }

            app.ShowHelp();
            return 0;
        }
    }
}
