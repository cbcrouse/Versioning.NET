using System;
using Domain.Enumerations;
using System.Threading.Tasks;
using Xunit;

namespace Versioning.NET.Tests;

public class StartupTests
{
    [Fact]
    public async Task CanStartup_Base()
    {
        // Arrange
        var args = Array.Empty<string>();

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
    public async Task CanStartupHelp_IncrementVersionWithGitHints()
    {
        // Arrange
        var args = new[] { "increment-version-with-git-hints", "--help"};

        // Act
        int resultCode = await Program.Main(args);

        // Assert
        Assert.Equal(0, resultCode);
    }

    [Fact]
    public async Task CanStartup_IncrementVersion()
    {
        // Arrange
        var args = new[] { "increment-version", "-d", "fake directory", "-v", nameof(VersionIncrement.Patch) };

        // Act
        int resultCode = await Program.Main(args);

        // Assert
        Assert.Equal(-1, resultCode);
    }

    [Fact]
    public async Task CanStartup_IncrementVersionWithGit()
    {
        // Arrange
        var args = new[] { "increment-version-with-git", "-g", "fake directory", "-a", "devops@versioning.net", "-b", "main", "-v", nameof(VersionIncrement.Patch), "-tp", "prefix", "-ts", "suffix" };

        // Act
        int resultCode = await Program.Main(args);

        // Assert
        Assert.Equal(-1, resultCode);
    }

    [Fact]
    public async Task CanStartup_IncrementVersionWithGitHints()
    {
        // Arrange
        var args = new[] { "increment-version-with-git-hints", "-g", "fake directory", "-a", "devops@versioning.net", "-b", "main", "-tp", "prefix", "-ts", "suffix"  };

        // Act
        int resultCode = await Program.Main(args);

        // Assert
        Assert.Equal(-1, resultCode);
    }
}