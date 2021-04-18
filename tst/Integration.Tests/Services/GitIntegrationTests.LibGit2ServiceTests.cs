using Domain.Entities;
using Infrastructure.Services;
using LibGit2Sharp;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Integration.Tests.Services
{
    public partial class GitIntegrationTests
    {
        [Fact]
        public void CanGetCommits()
        {
            // Arrange
            var sut = new LibGit2Service();

            // Act
            var commits = sut.GetCommits(TestRepoDirectory);

            // Assert
            Assert.Contains(commits, x => x.Subject == "ci(Versioning): Increment version 0.0.2 -> 0.0.3 [skip ci] [skip hint]" && x.Id == "9ed2a15974ccc61c63a432689aa0bfaa4554af1f");
            foreach (GitCommit gitCommit in commits)
            {
                switch (gitCommit.Subject)
                {
                    case "ci(Versioning): Increment version 0.0.1 -> 0.0.2 [skip ci] [skip hint]":
                        Assert.Equal(9, gitCommit.Updates.Count);
                        break;
                    case "style(Projects): Fixed XML Formatting":
                        Assert.Equal(15, gitCommit.Updates.Count);
                        break;
                    case "Initial Code Commit":
                        Assert.Equal(84, gitCommit.Updates.Count);
                        break;
                    case "Initial commit":
                        Assert.Single(gitCommit.Updates);
                        break;
                    default:
                        Assert.NotEmpty(gitCommit.Updates);
                        break;
                }
            }
        }

        [Theory]
        [InlineData("7097663ae5037990a057417aa7b1a8f99730f1ad", "35c6091b24ff88a053e477a67d4ad23c62e7953c", 3)] // Separate branch
        [InlineData("7097663ae5037990a057417aa7b1a8f99730f1ad", "25e49482f1e97f9cc169c76fa99e178b59cbc8c9", 6)] // Same branch
        [InlineData("7097663ae5037990a057417aa7b1a8f99730f1ad", "9248a828e8037bf2ea0b59ee29af81eaccad134e", 4)] // Same branch
        [InlineData("9248a828e8037bf2ea0b59ee29af81eaccad134e", "25e49482f1e97f9cc169c76fa99e178b59cbc8c9", 2)] // Same branch
        public void CanGetFilteredCommits(string fromHash, string untilHash, int expectedCount)
        {
            // Arrange
            var sut = new LibGit2Service();
            var filter = new GitCommitFilter(fromHash, untilHash);

            // Act
            var commits = sut.GetCommitsByFilter(TestRepoDirectory, filter);

            // Assert
            Assert.Equal(expectedCount, commits.Count);
            Assert.Equal(fromHash, commits[0].Id);
            Assert.DoesNotContain(commits, commit => commit.Id == untilHash);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("not-exist", "")]
        [InlineData("v0.0.3", "9ed2a15974ccc61c63a432689aa0bfaa4554af1f")]
        [InlineData("v0.0.1", "9248a828e8037bf2ea0b59ee29af81eaccad134e")]
        [InlineData("v0.0.0", "25e49482f1e97f9cc169c76fa99e178b59cbc8c9")]
        public void CanGetTagId(string tagValue, string expectedId)
        {
            // Arrange
            var sut = new LibGit2Service();

            // Act
            var actualId = sut.GetTagId(TestRepoDirectory, tagValue);

            // Assert
            Assert.Equal(expectedId, actualId);
        }

        [Fact]
        public void CanGetBranchTipId()
        {
            // Arrange
            var sut = new LibGit2Service();
            var expectedHash = "7097663ae5037990a057417aa7b1a8f99730f1ad";
            var remoteTarget = "origin";
            var branchName = "branch-tip-control-state";

            // Act
            var actualHash = sut.GetBranchTipId(TestRepoDirectory, remoteTarget, branchName);

            // Assert
            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void CanGetTags()
        {
            // Arrange
            var sut = new LibGit2Service();

            // Act
            var tags = sut.GetTags(TestRepoDirectory);

            // Assert
            Assert.Contains(tags, x => x == "v0.0.2");
        }

        [Fact]
        public void CanCreateGitCommit()
        {
            // Arrange
            var sut = new LibGit2Service();
            var commitMessage = "Test Commit Message";
            var authorEmail = "devops@versioning.net";
            File.WriteAllText(Path.Join(TestRepoDirectory, "README.md"), "Changes");

            // Act
            sut.CommitChanges(TestRepoDirectory, commitMessage, authorEmail);

            // Assert
            var commits = sut.GetCommits(TestRepoDirectory);
            Assert.Contains(commits, x => x.Subject == commitMessage);
        }

        [Fact]
        public void CanCreateGitTag()
        {
            // Arrange
            var sut = new LibGit2Service();
            var commitId = sut.GetCommits(TestRepoDirectory).First().Id;
            var tag = Guid.NewGuid().ToString();

            // Act
            sut.CreateTag(TestRepoDirectory, tag, commitId);

            // Assert
            Assert.Contains(sut.GetTags(TestRepoDirectory), x => x.Equals(tag));
        }

        [Fact]
        public void CanPushRemoteBranch()
        {
            // Arrange
            using var repo = new Repository(TestRepoDirectory);
            repo.Network.Remotes.Update("origin", updater =>
            {
                var token = Environment.GetEnvironmentVariable("GitHubAccessToken");
                var url = $"https://cbcrouse:{token}@github.com/cbcrouse/Versioning.NET.Tests.git";
                updater.Url = url;
                updater.PushUrl = url;
            });
            var sut = new LibGit2Service();
            var commitMessage = "Test Commit Message";
            var authorEmail = "devops@versioning.net";
            File.WriteAllText(Path.Join(TestRepoDirectory, "README.md"), Guid.NewGuid().ToString());
            var branchName = Guid.NewGuid().ToString();
            var branch = repo.CreateBranch(branchName);
            Commands.Checkout(repo, branch);
            sut.CommitChanges(TestRepoDirectory, commitMessage, authorEmail);

            // Act
            sut.PushRemote(TestRepoDirectory, "origin", $"refs/heads/{branchName}");

            // Assert
            Assert.NotNull(branch);
            repo.Network.Push(repo.Network.Remotes["origin"], $":refs/heads/{branchName}");
            Commands.Checkout(repo, "main");
            repo.Branches.Remove(branchName);
            var deletedBranch = repo.Branches[branchName];
            Assert.Null(deletedBranch);
        }

        [Fact]
        public void CanPushRemoteTag()
        {
            // Arrange
            using var repo = new Repository(TestRepoDirectory);
            repo.Network.Remotes.Update("origin", updater =>
            {
                var token = Environment.GetEnvironmentVariable("GitHubAccessToken");
                var url = $"https://cbcrouse:{token}@github.com/cbcrouse/Versioning.NET.Tests.git";
                updater.Url = url;
                updater.PushUrl = url;
            });
            var sut = new LibGit2Service();
            var tagName = Guid.NewGuid().ToString();
            Tag tag = repo.ApplyTag(tagName);
            Commands.Checkout(repo, tag.Target.Id.ToString());

            // Act
            sut.PushRemote(TestRepoDirectory, "origin", $":refs/heads/{tagName}");

            // Assert
            Assert.NotNull(tag);
            repo.Network.Push(repo.Network.Remotes["origin"], $":refs/heads/{tagName}");
            Commands.Checkout(repo, "main");
            repo.Tags.Remove(tagName);
            Tag deletedTag = repo.Tags[tagName];
            Assert.Null(deletedTag);
        }
    }
}
