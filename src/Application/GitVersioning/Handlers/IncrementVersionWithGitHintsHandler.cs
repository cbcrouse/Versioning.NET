using Application.AssemblyVersioning.Commands;
using Application.GitVersioning.Commands;
using Application.GitVersioning.Queries;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enumerations;
using MediatR;
using Microsoft.Extensions.Logging;
using Semver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.GitVersioning.Handlers
{
    /// <summary>
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing the version with git integration based on git commit messages.
    /// </summary>
    public class IncrementVersionWithGitHintsHandler : IRequestHandler<IncrementVersionWithGitHintsCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly IGitService _gitService;
        private readonly IGitVersioningService _gitVersioningService;
        private readonly IAssemblyVersioningService _assemblyVersioningService;
        private readonly ILogger<IncrementVersionWithGitHintsHandler> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
        /// <param name="gitService">An abstraction to facilitate testing without using the git integration.</param>
        /// <param name="gitVersioningService">An abstraction for retrieving version hint info from git commit messages.</param>
        /// <param name="assemblyVersioningService">An abstraction for working with assembly versions.</param>
        /// <param name="logger">A generic interface for logging.</param>
        public IncrementVersionWithGitHintsHandler(
            IMediator mediator,
            IGitService gitService,
            IGitVersioningService gitVersioningService,
            IAssemblyVersioningService assemblyVersioningService,
            ILogger<IncrementVersionWithGitHintsHandler> logger)
        {
            _mediator = mediator;
            _gitService = gitService;
            _gitVersioningService = gitVersioningService;
            _assemblyVersioningService = assemblyVersioningService;
            _logger = logger;
        }

        /// <summary>Handles the request to increment the version with git integration.</summary>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> object responsible for incrementing the version with git integration.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        public async Task<Unit> Handle(IncrementVersionWithGitHintsCommand request, CancellationToken cancellationToken)
        {
            var query = new GetCommitVersionInfosQuery { GitDirectory = request.GitDirectory, RemoteTarget = request.RemoteTarget, TipBranchName = request.BranchName };
            List<GitCommitVersionInfo> versionInfos = (await _mediator.Send(query, cancellationToken)).ToList();

            VersionIncrement increment = _gitVersioningService.DeterminePriorityIncrement(versionInfos.Select(x => x.VersionIncrement));
            _logger.LogInformation($"Increment '{increment}' was determined from the commits.");

            if (increment is VersionIncrement.None or VersionIncrement.Unknown)
            {
                return Unit.Value;
            }

            if (string.IsNullOrWhiteSpace(request.TargetDirectory))
            {
                request.TargetDirectory = request.GitDirectory;
            }

            SemVersion originalAssemblyVersion = _assemblyVersioningService.GetLatestAssemblyVersion(request.TargetDirectory, request.SearchOption);

            var command = new IncrementAssemblyVersionCommand
            {
                Directory = request.TargetDirectory,
                SearchOption = request.SearchOption,
                VersionIncrement = increment,
                ExitBeta = versionInfos.Any(x => x.ExitBeta)
            };
            await _mediator.Send(command, cancellationToken);

            SemVersion currentAssemblyVersion = _assemblyVersioningService.GetLatestAssemblyVersion(request.TargetDirectory, request.SearchOption);

            var commitMessage = $"ci(Versioning): Increment version {originalAssemblyVersion} -> {currentAssemblyVersion} [skip ci] [skip hint]";
            _gitService.CommitChanges(request.GitDirectory, commitMessage, request.CommitAuthorEmail);

            string commitId = _gitService.GetCommits(request.GitDirectory).First(x => x.Subject.Equals(commitMessage)).Id;
            string tagValue = $"{request.TagPrefix}{currentAssemblyVersion}{request.TagSuffix}";

            _gitService.PushRemote(request.GitDirectory, request.RemoteTarget, $"refs/heads/{request.BranchName}");
            _gitService.CreateTag(request.GitDirectory, tagValue, commitId);
            _gitService.PushRemote(request.GitDirectory, request.RemoteTarget, $"refs/tags/{tagValue}");

            return Unit.Value;
        }
    }
}
