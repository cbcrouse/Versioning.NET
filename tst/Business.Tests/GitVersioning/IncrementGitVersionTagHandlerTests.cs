using Application.AssemblyVersioning.Commands;
using Application.GitVersioning.Commands;
using Application.GitVersioning.Handlers;
using Application.GitVersioning.Queries;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enumerations;
using MediatR;
using Moq;
using Semver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Business.Tests.GitVersioning
{
    public class IncrementGitVersionTagHandlerTests
    {
        private List<GitCommit> Commits {get; } = new List<GitCommit>
        {
            new GitCommit("27d8879","fake(Build): Updated build.yml to target the template solution file [skip hint]", new List<GitCommitFileInfo>()),
            new GitCommit("27d8880","build(Pipeline): Downgraded to .net core 3.1 from 5 [skip ci]", new List<GitCommitFileInfo>()),
            new GitCommit("27d8881","ci(Versioning): Increment version 0.0.0 -> 0.0.0 [skip ci] [skip hint]", new List<GitCommitFileInfo>())
        };

        private IEnumerable<GitCommitVersionInfo> VersionInfos { get; } = new List<GitCommitVersionInfo>
        {
            new GitCommitVersionInfo("fake", "build", "fake(Build): Updated build.yml to target the template solution file [skip hint]"),
            new GitCommitVersionInfo("build", "Pipeline", "build(Pipeline): Downgraded to .net core 3.1 from 5 [skip ci]"),
            new GitCommitVersionInfo("ci", "Versioning", "Increment version 0.0.0 -> 0.0.0 [skip ci] [skip hint]")
        };

        [Fact]
        public async Task Handler_ExitsWhen_IncrementIsNone()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var gitService = new Mock<IGitService>();
            var assemblyVersioningService = new Mock<IAssemblyVersioningService>();

            mediator.Setup(x => x.Send(It.IsAny<GetIncrementFromCommitHintsQuery>(), CancellationToken.None)).ReturnsAsync(VersionIncrement.None);
            mediator.Setup(x => x.Send(It.IsAny<IncrementAssemblyVersionCommand>(), CancellationToken.None)).ReturnsAsync(Unit.Value);
            var sut = new IncrementVersionWithGitIntegrationHandler(mediator.Object, gitService.Object, assemblyVersioningService.Object);

            // Act
            await sut.Handle(new IncrementVersionWithGitIntegrationCommand(), CancellationToken.None);

            // Assert
            mediator.Verify(x => x.Send(It.IsAny<GetIncrementFromCommitHintsQuery>(), CancellationToken.None), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<IncrementAssemblyVersionCommand>(), CancellationToken.None), Times.Never);
            gitService.Verify(x => x.GetCommits(It.IsAny<string>()), Times.Never);
            assemblyVersioningService.Verify(x => x.GetLatestAssemblyVersion(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handler_ExitsWhen_IncrementIsUnknown()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var gitService = new Mock<IGitService>();
            var assemblyVersioningService = new Mock<IAssemblyVersioningService>();

            mediator.Setup(x => x.Send(It.IsAny<GetIncrementFromCommitHintsQuery>(), CancellationToken.None)).ReturnsAsync(VersionIncrement.Unknown);
            mediator.Setup(x => x.Send(It.IsAny<IncrementAssemblyVersionCommand>(), CancellationToken.None)).ReturnsAsync(Unit.Value);
            var sut = new IncrementVersionWithGitIntegrationHandler(mediator.Object, gitService.Object, assemblyVersioningService.Object);

            // Act
            await sut.Handle(new IncrementVersionWithGitIntegrationCommand(), CancellationToken.None);

            // Assert
            mediator.Verify(x => x.Send(It.IsAny<GetIncrementFromCommitHintsQuery>(), CancellationToken.None), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<IncrementAssemblyVersionCommand>(), CancellationToken.None), Times.Never);
            gitService.Verify(x => x.GetCommits(It.IsAny<string>()), Times.Never);
            assemblyVersioningService.Verify(x => x.GetLatestAssemblyVersion(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handler_CallsDependencies()
        {
            // Arrange
            var request = new IncrementVersionWithGitIntegrationCommand
            {
                GitDirectory = "C:\\Temp",
                CommitAuthorEmail = "support@versioning.net"
            };

            var mediator = new Mock<IMediator>();
            var gitService = new Mock<IGitService>();
            var assemblyVersioningService = new Mock<IAssemblyVersioningService>();

            mediator.Setup(x => x.Send(It.IsAny<GetIncrementFromCommitHintsQuery>(), CancellationToken.None)).ReturnsAsync(VersionIncrement.Minor);
            mediator.Setup(x => x.Send(It.IsAny<IncrementAssemblyVersionCommand>(), CancellationToken.None)).ReturnsAsync(Unit.Value);

            gitService.Setup(x => x.GetCommits(It.IsAny<string>())).Returns(Commits);
            assemblyVersioningService.Setup(x => x.GetLatestAssemblyVersion(request.GitDirectory)).Returns(new SemVersion(0));
            var sut = new IncrementVersionWithGitIntegrationHandler(mediator.Object, gitService.Object, assemblyVersioningService.Object);

            // Act
            await sut.Handle(request, CancellationToken.None);

            // Assert
            mediator.Verify(x => x.Send(It.IsAny<GetIncrementFromCommitHintsQuery>(), CancellationToken.None), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<IncrementAssemblyVersionCommand>(), CancellationToken.None), Times.Once);
            assemblyVersioningService.Verify(x => x.GetLatestAssemblyVersion(request.GitDirectory), Times.Exactly(2));
            gitService.Verify(x => x.CommitChanges(request.GitDirectory, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            gitService.Verify(x => x.GetCommits(request.GitDirectory), Times.Once);
            gitService.Verify(x => x.CreateTag(request.GitDirectory, It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
