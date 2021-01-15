#nullable enable
using Domain.Constants;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public partial class GitIntegrationTests
    {
        [Fact]
        public void CanGetGitCommits()
        {
            // Arrange
            var sut = new GitService(new PowerShellService());

            // Act
            List<GitCommit> results = sut.GetCommits(TestRepoDirectory);

            // Assert
            Assert.True(results.Any());
        }

        [Fact]
        public void CanGetGitTags()
        {
            // Arrange
            var sut = new GitService(new PowerShellService());

            // Act
            IEnumerable<string> results = sut.GetTags(TestRepoDirectory);

            // Assert
            Assert.True(results.Any());
        }

        [Fact]
        public void CanGetVersionTag()
        {
            // Arrange
            var sut = new GitService(new PowerShellService());
            var regex = new Regex(RegexPatterns.GitTagVersion);

            // Act
            IEnumerable<string> results = sut.GetTags(TestRepoDirectory);

            // Assert
            Assert.Contains(results, x => regex.Match(x).Success);
        }

        [Fact]
        public void CanCreateGitCommit()
        {
            // Arrange
            var pwsh = new PowerShellService();
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
    }
}
