using Application.GitVersioning.Handlers;
using Application.GitVersioning.Queries;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Semver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.GitVersioning
{
    public class GetIncrementFromCommitHintsHandlerTests
    {
        private IEnumerable<string> Tags { get; } = new List<string> {"v2.4.1", "v2.4.2", "v3.1.0", "v3.1.0-beta.336", "v3.1.0-rc.371"};

        [Fact]
        public async Task Handler_CallsDependencies()
        {
            // Arrange
            var commits = new List<GitCommit> { new("1889a29", "feat(Template): Added ability to see template in Visual Studio") };
            var versionInfos = new List<GitCommitVersionInfo> { new("feat", "Template", commits.First().Subject)};
            var gitService = new Mock<IGitService>();
            gitService.Setup(x => x.GetTags(It.IsAny<string>())).Returns(Tags);
            gitService.Setup(x => x.GetCommits(It.IsAny<string>(), It.IsAny<string>())).Returns(commits);
            var gitVersioningService = new Mock<IGitVersioningService>();
            gitVersioningService.Setup(x => x.GetLatestVersionTag(Tags)).Returns(new KeyValuePair<string, SemVersion>("v3.1.0", new SemVersion(3, 1)));
            gitVersioningService.Setup(x => x.GetCommitVersionInfo(commits.Select(x => x.Subject))).Returns(versionInfos);
            gitVersioningService.Setup(x => x.DeterminePriorityIncrement(versionInfos.Select(x => x.VersionIncrement))).Returns(VersionIncrement.None);
            var logger = new NullLogger<GetIncrementFromCommitHintsHandler>();
            var sut = new GetIncrementFromCommitHintsHandler(gitService.Object, gitVersioningService.Object, logger);

            // Act
            _ = await sut.Handle(new GetIncrementFromCommitHintsQuery(), CancellationToken.None);

            // Assert
            gitService.Verify(x => x.GetTags(It.IsAny<string>()), Times.Once);
            gitService.Verify(x => x.GetCommits(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            gitVersioningService.Verify(x => x.GetLatestVersionTag(It.IsAny<IEnumerable<string>>()), Times.Once);
            gitVersioningService.Verify(x => x.GetCommitVersionInfo(It.IsAny<IEnumerable<string>>()), Times.Once);
            gitVersioningService.Verify(x => x.DeterminePriorityIncrement(It.IsAny<IEnumerable<VersionIncrement>>()), Times.Once);
        }

        [Fact]
        public async Task Handler_ReturnsNoneIncrement_WhenNoCommitsFound()
        {
            // Arrange
            var gitService = new Mock<IGitService>();
            gitService.Setup(x => x.GetTags(It.IsAny<string>())).Returns(Tags);
            gitService.Setup(x => x.GetCommits(It.IsAny<string>(), It.IsAny<string>())).Returns(new List<GitCommit>());
            var gitVersioningService = new Mock<IGitVersioningService>();
            gitVersioningService.Setup(x => x.GetLatestVersionTag(Tags)).Returns(new KeyValuePair<string, SemVersion>("v3.1.0", new SemVersion(3, 1)));
            var logger = new NullLogger<GetIncrementFromCommitHintsHandler>();
            var sut = new GetIncrementFromCommitHintsHandler(gitService.Object, gitVersioningService.Object, logger);

            // Act
            VersionIncrement increment = await sut.Handle(new GetIncrementFromCommitHintsQuery(), CancellationToken.None);

            // Assert
            Assert.Equal(VersionIncrement.None, increment);
        }
    }
}
