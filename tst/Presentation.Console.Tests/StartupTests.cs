using Domain.Enumerations;
using System.Threading.Tasks;
using Xunit;

namespace Presentation.Console.Tests
{
    public class StartupTests
    {
        [Fact]
        public async Task CanStartup_Base()
        {
            // Arrange
            var args = new string[0];

            // Act
            int resultCode = await Program.Main(args);

            // Assert
            Assert.Equal(0, resultCode);
        }

        [Fact]
        public async Task CanStartup_Version()
        {
            // Arrange
            var args = new[] { "--version" };

            // Act
            int resultCode = await Program.Main(args);

            // Assert
            Assert.Equal(0, resultCode);
        }

        [Fact]
        public async Task CanStartupHelp_IncrementVersion()
        {
            // Arrange
            var args = new[] { "increment-version", "--help" };

            // Act
            int resultCode = await Program.Main(args);

            // Assert
            Assert.Equal(0, resultCode);
        }

        [Fact]
        public async Task CanStartupHelp_IncrementVersionWithGit()
        {
            // Arrange
            var args = new[] { "increment-version-with-git", "--help"};

            // Act
            int resultCode = await Program.Main(args);

            // Assert
            Assert.Equal(0, resultCode);
        }

        [Fact]
        public async Task CanStartup_IncrementVersion()
        {
            // Arrange
            var args = new[] { "increment-version", "-d", "fake directory", "-v", VersionIncrement.Patch.ToString() };

            // Act
            int resultCode = await Program.Main(args);

            // Assert
            Assert.Equal(-1, resultCode);
        }

        [Fact]
        public async Task CanStartup_IncrementVersionWithGit()
        {
            // Arrange
            var args = new[] { "increment-version-with-git", "-g", "fake directory", "-a", "devops@versioning.net", "-b", "main" };

            // Act
            int resultCode = await Program.Main(args);

            // Assert
            Assert.Equal(-1, resultCode);
        }
    }
}
