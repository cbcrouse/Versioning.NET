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
    public class GetCommitVersionInfosHandlerTests
    {
        private IEnumerable<string> Tags { get; } = new List<string> {"v2.4.1", "v2.4.2", "v3.1.0", "v3.1.0-beta.336", "v3.1.0-rc.371"};

        [Fact]
        public async Task Handler_CallsDependencies()
        {
            // Arrange
            var commits = new List<GitCommit> { new("1889a29", "feat(Template): Added ability to see template in Visual Studio", new List<GitCommitFileInfo>()) };
            var versionInfos = new List<GitCommitVersionInfo> { new("feat", "Template", commits.First().Subject)};
            var gitService = new Mock<IGitService>();
            gitService.Setup(x => x.GetTagId(It.IsAny<string>(), It.IsAny<string>())).Returns("a tag id");
            gitService.Setup(x => x.GetBranchTipId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns("a branch tip id");
            gitService.Setup(x => x.GetTags(It.IsAny<string>())).Returns(Tags);
            gitService.Setup(x => x.GetCommitsByFilter(It.IsAny<string>(), It.IsAny<GitCommitFilter>())).Returns(commits);
            var gitVersioningService = new Mock<IGitVersioningService>();
            gitVersioningService.Setup(x => x.GetLatestVersionTag(Tags)).Returns(new KeyValuePair<string, SemVersion>("v3.1.0", new SemVersion(3, 1)));
            gitVersioningService.Setup(x => x.GetCommitVersionInfo(commits)).Returns(versionInfos);
            gitVersioningService.Setup(x => x.DeterminePriorityIncrement(versionInfos.Select(x => x.VersionIncrement))).Returns(VersionIncrement.None);
            var logger = new NullLogger<GetCommitVersionInfosHandler>();
            var sut = new GetCommitVersionInfosHandler(gitService.Object, gitVersioningService.Object, logger);

            // Act
            _ = await sut.Handle(new GetCommitVersionInfosQuery(), CancellationToken.None);

            // Assert
            gitService.Verify(x => x.GetTags(It.IsAny<string>()), Times.Once);
            gitService.Verify(x => x.GetTagId(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            gitService.Verify(x => x.GetBranchTipId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            gitService.Verify(x => x.GetCommitsByFilter(It.IsAny<string>(), It.IsAny<GitCommitFilter>()), Times.Once);
            gitVersioningService.Verify(x => x.GetLatestVersionTag(It.IsAny<IEnumerable<string>>()), Times.Once);
            gitVersioningService.Verify(x => x.GetCommitVersionInfo(It.IsAny<IEnumerable<GitCommit>>()), Times.Once);
        }

        [Fact]
        public async Task Handler_ReturnsEmptyCollection_WhenNoCommitsFound()
        {
            // Arrange
            var gitService = new Mock<IGitService>();
            gitService.Setup(x => x.GetTagId(It.IsAny<string>(), It.IsAny<string>())).Returns(new System.Guid().ToString());
            gitService.Setup(x => x.GetBranchTipId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new System.Guid().ToString());
            gitService.Setup(x => x.GetCommits(It.IsAny<string>())).Returns(new List<GitCommit>());
            gitService.Setup(x => x.GetCommitsByFilter(It.IsAny<string>(), It.IsAny<GitCommitFilter>())).Returns(new List<GitCommit>());
            var gitVersioningService = new Mock<IGitVersioningService>();
            gitVersioningService.Setup(x => x.GetLatestVersionTag(It.IsAny<IEnumerable<string>>())).Returns(It.IsAny<KeyValuePair<string, SemVersion>>());
            var logger = new NullLogger<GetCommitVersionInfosHandler>();
            var sut = new GetCommitVersionInfosHandler(gitService.Object, gitVersioningService.Object, logger);

            // Act
            var result = await sut.Handle(new GetCommitVersionInfosQuery(), CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handler_ReturnsEmptyCollection_WhenNoTagsFound()
        {
            // Arrange
            var gitService = new Mock<IGitService>();
            gitService.Setup(x => x.GetTags(It.IsAny<string>())).Returns(new List<string>());
            gitService.Setup(x => x.GetCommits(It.IsAny<string>())).Returns(new List<GitCommit>());
            gitService.Setup(x => x.GetCommitsByFilter(It.IsAny<string>(), It.IsAny<GitCommitFilter>())).Returns(new List<GitCommit>());
            var gitVersioningService = new Mock<IGitVersioningService>();
            var logger = new NullLogger<GetCommitVersionInfosHandler>();
            var sut = new GetCommitVersionInfosHandler(gitService.Object, gitVersioningService.Object, logger);

            // Act
            var result = await sut.Handle(new GetCommitVersionInfosQuery(), CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handler_ReturnsEmptyCollection_WhenHeadCommitNotFound()
        {
            // Arrange
            var gitService = new Mock<IGitService>();
            gitService.Setup(x => x.GetTags(It.IsAny<string>())).Returns(new List<string>());
            gitService.Setup(x => x.GetTagId(It.IsAny<string>(), It.IsAny<string>())).Returns(It.IsAny<string>());
            gitService.Setup(x => x.GetBranchTipId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);
            var gitVersioningService = new Mock<IGitVersioningService>();
            gitVersioningService.Setup(x => x.GetLatestVersionTag(It.IsAny<IEnumerable<string>>())).Returns(It.IsAny<KeyValuePair<string, SemVersion>>());
            var logger = new NullLogger<GetCommitVersionInfosHandler>();
            var sut = new GetCommitVersionInfosHandler(gitService.Object, gitVersioningService.Object, logger);

            // Act
            var result = await sut.Handle(new GetCommitVersionInfosQuery(), CancellationToken.None);

            // Assert
            Assert.Empty(result);
        }
    }
}
