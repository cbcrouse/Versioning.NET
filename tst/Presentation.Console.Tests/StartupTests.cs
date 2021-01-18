using System.Threading.Tasks;
using Xunit;

namespace Presentation.Console.Tests
{
    public class StartupTests
    {
        [Fact]
        public async Task CanStartup_IncrementVersion()
        {
            // Arrange
            var args = new[] {"increment-version"};

            // Act
            await Program.Main(args);
        }

        [Fact]
        public async Task CanStartup_IncrementVersionWithGit()
        {
            // Arrange
            var args = new[] {"increment-version-with-git"};

            // Act
            await Program.Main(args);
        }
    }
}
