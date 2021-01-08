using Domain.Constants;
using Domain.Entities;
using Infrastructure.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace Infrastructure.Tests.Services
{
    public class GitServiceTests
    {
        [Fact]
        public void CanGetGitCommits()
        {
            // Arrange
            var service = new GitService(new PowerShellService());

            // Act
            List<GitCommit> results = service.GetCommits("G:\\Git\\CleanArchitecture");

            // Assert
            Assert.True(results.Any());
        }

        [Fact]
        public void CanGetGitTags()
        {
            // Arrange
            var service = new GitService(new PowerShellService());

            // Act
            IEnumerable<string> results = service.GetTags("G:\\Git\\CleanArchitecture");

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
            IEnumerable<string> results = sut.GetTags("G:\\Git\\CleanArchitecture");

            // Assert
            Assert.Contains(results, x => regex.Match(x).Success);
        }
    }
}
