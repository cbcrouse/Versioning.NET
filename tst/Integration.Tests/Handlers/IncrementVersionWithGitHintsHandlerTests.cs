using Application.GitVersioning.Commands;
using Application.GitVersioning.Handlers;
using Application.Interfaces;
using Integration.Tests.Setup;
using LibGit2Sharp;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Integration.Tests.Handlers
{
    public class IncrementVersionWithGitHintsHandlerTests : GitSetup, IClassFixture<Orchestrator>
    {
        private readonly IServiceProvider _serviceProvider;

        public IncrementVersionWithGitHintsHandlerTests(Orchestrator orchestrator)
        {
            _serviceProvider = orchestrator.BuildServiceProvider();
        }

        [Fact]
        public void Handle_WorksAsExpected()
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
            var mediator = _serviceProvider.GetRequiredService<IMediator>();
            var gitService = _serviceProvider.GetRequiredService<IGitService>();
            var gitVersioningService = _serviceProvider.GetRequiredService<IGitVersioningService>();
            var versioningService = _serviceProvider.GetRequiredService<IAssemblyVersioningService>();
            var logger = _serviceProvider.GetRequiredService<ILogger<IncrementVersionWithGitHintsHandler>>();
            var sut = new IncrementVersionWithGitHintsHandler(mediator, gitService, gitVersioningService, versioningService, logger);

            var command = new IncrementVersionWithGitHintsCommand
            {
                GitDirectory = TestRepoDirectory,
                BranchName = "main",
                RemoteTarget = "origin",
                CommitAuthorEmail = "support@versioning.net"
            };

            // Act
            //_ = await sut.Handle(command, CancellationToken.None);

            // Assert
        }
    }
}
