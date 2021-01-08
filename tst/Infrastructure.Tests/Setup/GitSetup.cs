using Infrastructure.Services;
using System;
using System.IO;

namespace Infrastructure.Tests.Setup
{
    /// <summary>
    /// Helper class for tests to setup git directory
    /// </summary>
    public class GitSetup : IDisposable
    {
        protected string TestRepoDirectory { get; private set; }
        private string _testRepoParentDirectory;
        private readonly PowerShellService _powerShell = new PowerShellService();

        public GitSetup()
        {
            CloneTestRepository();
        }

        private void CloneTestRepository()
        {
            CreateTempDirectory();
            _powerShell.RunScript(_testRepoParentDirectory, "git clone https://github.com/cbcrouse/Versioning.NET.Tests");
            TestRepoDirectory = Path.Join(_testRepoParentDirectory, "Versioning.NET.Tests");
        }

        private void CreateTempDirectory()
        {
            _testRepoParentDirectory = Path.Join(Path.GetTempPath(), "Versioning.NET.Tests." + Guid.NewGuid());

            if (!Directory.Exists(_testRepoParentDirectory))
            {
                Directory.CreateDirectory(_testRepoParentDirectory);
            }
        }

        private void DeleteTempDirectory()
        {
            if (!Directory.Exists(_testRepoParentDirectory))
                return;
            // Directory.Delete does not work here "Access is denied".
            var script = $"Remove-Item -Path \"{_testRepoParentDirectory}\" -Recurse -Force";
            _powerShell.RunScript(Path.GetTempPath(), script);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            DeleteTempDirectory();
        }
    }
}
