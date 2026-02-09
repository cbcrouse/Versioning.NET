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

namespace Application.GitVersioning.Handlers;

/// <summary>
/// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing the version with git integration based on git commit messages.
/// </summary>
/// <param name="mediator">An abstraction for accessing application behaviors.</param>
/// <param name="gitService">An abstraction to facilitate testing without using the git integration.</param>
/// <param name="gitVersioningService">An abstraction for retrieving version hint info from git commit messages.</param>
/// <param name="assemblyVersioningService">An abstraction for working with assembly versions.</param>
/// <param name="logger">A generic interface for logging.</param>
public class IncrementVersionWithGitHintsHandler(
    IMediator mediator,
    IGitService gitService,
    IGitVersioningService gitVersioningService,
    IAssemblyVersioningService assemblyVersioningService,
    ILogger<IncrementVersionWithGitHintsHandler> logger)
    : IRequestHandler<IncrementVersionWithGitHintsCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(IncrementVersionWithGitHintsCommand request, CancellationToken cancellationToken)
    {
        var query = new GetCommitVersionInfosQuery { GitDirectory = request.GitDirectory, RemoteTarget = request.RemoteTarget, TipBranchName = request.BranchName };
        List<GitCommitVersionInfo> versionInfos = (await mediator.Send(query, cancellationToken)).ToList();

        VersionIncrement increment = gitVersioningService.DeterminePriorityIncrement(versionInfos.Select(x => x.VersionIncrement));
        logger.LogInformation($"Increment '{increment}' was determined from the commits.");

        if (increment is VersionIncrement.None or VersionIncrement.Unknown)
        {
            return Unit.Value;
        }

        if (string.IsNullOrWhiteSpace(request.TargetDirectory))
        {
            request.TargetDirectory = request.GitDirectory;
        }

        SemVersion originalAssemblyVersion = assemblyVersioningService.GetLatestAssemblyVersion(request.TargetDirectory, request.SearchOption);

        var command = new IncrementAssemblyVersionCommand
        {
            Directory = request.TargetDirectory,
            SearchOption = request.SearchOption,
            VersionIncrement = increment,
            ExitBeta = versionInfos.Any(x => x.ExitBeta)
        };
        await mediator.Send(command, cancellationToken);

        SemVersion currentAssemblyVersion = assemblyVersioningService.GetLatestAssemblyVersion(request.TargetDirectory, request.SearchOption);

        var commitMessage = $"ci(Versioning): Increment version {originalAssemblyVersion} -> {currentAssemblyVersion} [skip ci] [skip hint]";
        gitService.CommitChanges(request.GitDirectory, commitMessage, request.CommitAuthorEmail);

        string commitId = gitService.GetCommits(request.GitDirectory).First(x => x.Subject.Equals(commitMessage)).Id;
        string tagValue = $"{request.TagPrefix}{currentAssemblyVersion}{request.TagSuffix}";

        gitService.PushRemote(request.GitDirectory, request.RemoteTarget, $"refs/heads/{request.BranchName}");
        gitService.CreateTag(request.GitDirectory, tagValue, commitId);
        gitService.PushRemote(request.GitDirectory, request.RemoteTarget, $"refs/tags/{tagValue}");

        return Unit.Value;
    }
}