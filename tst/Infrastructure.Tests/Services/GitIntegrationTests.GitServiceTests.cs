#nullable enable
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public partial class GitIntegrationTests
    {
        [Fact]
        public void CanGetGitCommits()
        {
            // Arrange
            var sut = new GitService(new PowerShellService(new NullLogger<PowerShellService>()));

            // Act
            List<GitCommit> results = sut.GetCommits(TestRepoDirectory);

            // Assert
            Assert.True(results.Any());
        }

        [Fact]
        public void CanGetGitCommitsWithRevision()
        {
            // Arrange
            var sut = new GitService(new PowerShellService(new NullLogger<PowerShellService>()));
            const string revision = "v0.0.0..HEAD";

            // Act
            List<GitCommit> results = sut.GetCommits(TestRepoDirectory, revision);

            // Assert
            Assert.True(results.Any());
        }

        [Fact]
        public void NoGitCommitsReturnAtHead()
        {
            // Arrange
            var sut = new GitService(new PowerShellService(new NullLogger<PowerShellService>()));
            const string revision = "HEAD -0";

            // Act
            List<GitCommit> results = sut.GetCommits(TestRepoDirectory, revision);

            // Assert
            Assert.False(results.Any());
        }

        [Fact]
        public void CanGetGitTags()
        {
            // Arrange
            var sut = new GitService(new PowerShellService(new NullLogger<PowerShellService>()));

            // Act
            IEnumerable<string> results = sut.GetTags(TestRepoDirectory);

            // Assert
            Assert.True(results.Any());
        }

        [Fact]
        public void CanGetVersionTag()
        {
            // Arrange
            var sut = new GitService(new PowerShellService(new NullLogger<PowerShellService>()));

            // Act
            IEnumerable<string> results = sut.GetTags(TestRepoDirectory);

            // Assert
            Assert.Contains(results, x => SemVersion.TryParse(x.ToLower().TrimStart('v'), out SemVersion _));
        }

        [Fact]
        public void CanCreateGitCommit()
        {
            // Arrange
            var pwsh = new PowerShellService(new NullLogger<PowerShellService>());
            var sut = new GitService(pwsh);
            var avs = new AssemblyVersioningService();
            avs.IncrementVersion(VersionIncrement.Patch, TestRepoDirectory);

            // Preserve original git config values
            PSObject? originalConfigEmail = pwsh.RunScript(TestRepoDirectory, "git config --global --get user.email").FirstOrDefault();
            PSObject? originalConfigName = pwsh.RunScript(TestRepoDirectory, "git config --global --get user.name").FirstOrDefault();

            PSObject? expectedConfigEmail = pwsh.RunScript(TestRepoDirectory, "git config --global --unset user.email").FirstOrDefault();
            PSObject? expectedConfigName = pwsh.RunScript(TestRepoDirectory, "git config --global --unset user.name").FirstOrDefault();

            // Act
            sut.CommitChanges(TestRepoDirectory, "Test Commit Message", "support@test.com");

            // Assert
            List<GitCommit> results = sut.GetCommits(TestRepoDirectory);
            Assert.Contains(results, x => x.Subject.Equals("Test Commit Message"));

            PSObject? actualConfigEmail = pwsh.RunScript(TestRepoDirectory, "git config --global --get user.email").FirstOrDefault();
            PSObject? actualConfigName = pwsh.RunScript(TestRepoDirectory, "git config --global --get user.name").FirstOrDefault();

            Assert.Equal(expectedConfigEmail, actualConfigEmail);
            Assert.Equal(expectedConfigName, actualConfigName);

            // Reset git config
            pwsh.RunScript(TestRepoDirectory, $"git config --global user.email {originalConfigEmail}");
            pwsh.RunScript(TestRepoDirectory, $"git config --global user.name {originalConfigName}");
        }

        [Fact]
        public void CanCreateGitTag()
        {
            // Arrange
            var pwsh = new PowerShellService(new NullLogger<PowerShellService>());
            var sut = new GitService(pwsh);
            var commitId = sut.GetCommits(TestRepoDirectory).First().Id;
            var tag = Guid.NewGuid().ToString();

            // Act
            sut.CreateTag(TestRepoDirectory, tag, commitId);

            // Assert
            Assert.Contains(sut.GetTags(TestRepoDirectory), x => x.Equals(tag));
        }
    }
}
