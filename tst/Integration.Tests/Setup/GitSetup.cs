using LibGit2Sharp;
using System;
using System.IO;

namespace Integration.Tests.Setup
{
    /// <summary>
    /// Helper class for tests to setup git directory
    /// </summary>
    public class GitSetup : IDisposable
    {
        protected string TestRepoDirectory { get; private set; }
        private string _testRepoParentDirectory;

        public GitSetup()
        {
            CloneTestRepository();
        }

        private void CloneTestRepository()
        {
            CreateTempDirectory();
            var gitPath = Repository.Clone("https://github.com/cbcrouse/Versioning.NET.Tests", _testRepoParentDirectory);
            TestRepoDirectory = Directory.GetParent(gitPath)!.Parent!.FullName;
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

            var di = new DirectoryInfo(_testRepoParentDirectory);

            ClearAttributes(_testRepoParentDirectory);
            foreach (FileInfo file in di.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo directory in di.EnumerateDirectories())
            {
                directory.Delete(true);
            }
            Directory.Delete(_testRepoParentDirectory, true);
        }

        /// <summary>
        /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/b79d5e07-bd8a-446e-8c44-3a2f2a262d5e/iodirectorydelete-readonly-quotaccess-deniedquot
        /// </summary>
        private static void ClearAttributes(string currentDir)
        {
            if (!Directory.Exists(currentDir)) return;
            string[] subDirs = Directory.GetDirectories(currentDir);
            foreach(string dir in subDirs)
                ClearAttributes(dir);
            string[] files = Directory.GetFiles(currentDir);
            foreach (string file in files)
                File.SetAttributes(file, FileAttributes.Normal);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                DeleteTempDirectory();
            }
            // free native resources if there are any.
        }
    }
}
