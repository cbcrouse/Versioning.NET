using Application.AssemblyVersioning.Commands;
using Application.GitVersioning.Commands;
using Application.Interfaces;
using Domain.Enumerations;
using MediatR;
using Semver;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.GitVersioning.Handlers;

/// <summary>
/// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing the version with git integration.
/// </summary>
/// <param name="mediator">An abstraction for accessing application behaviors.</param>
/// <param name="gitService">An abstraction to facilitate testing without using the git integration.</param>
/// <param name="assemblyVersioningService">An abstraction for working with assembly versions.</param>
public class IncrementVersionWithGitHandler(
    IMediator mediator,
    IGitService gitService,
    IAssemblyVersioningService assemblyVersioningService)
    : IRequestHandler<IncrementVersionWithGitCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(IncrementVersionWithGitCommand request, CancellationToken cancellationToken)
    {
        if (request.VersionIncrement is VersionIncrement.None or VersionIncrement.Unknown)
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
            VersionIncrement = request.VersionIncrement,
            ExitBeta = request.ExitBeta
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