using Application.AssemblyVersioning.Commands;
using Application.GitVersioning.Commands;
using Application.GitVersioning.Queries;
using Application.Interfaces;
using Domain.Enumerations;
using MediatR;
using Semver;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.GitVersioning.Handlers
{
    /// <summary>
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing the version with git integration.
    /// </summary>
    public class IncrementVersionWithGitIntegrationHandler : IRequestHandler<IncrementVersionWithGitIntegrationCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly IGitService _gitService;
        private readonly IAssemblyVersioningService _assemblyVersioningService;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
        /// <param name="gitService">An abstraction to facilitate testing without using the git integration.</param>
        /// <param name="assemblyVersioningService">An abstraction for working with assembly versions.</param>
        public IncrementVersionWithGitIntegrationHandler(
            IMediator mediator,
            IGitService gitService,
            IAssemblyVersioningService assemblyVersioningService)
        {
            _mediator = mediator;
            _gitService = gitService;
            _assemblyVersioningService = assemblyVersioningService;
        }

        /// <summary>Handles the request to increment the version with git integration.</summary>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> object responsible for incrementing the version with git integration.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        public async Task<Unit> Handle(IncrementVersionWithGitIntegrationCommand request, CancellationToken cancellationToken)
        {
            var query = new GetIncrementFromCommitHintsQuery { GitDirectory = request.GitDirectory, Revision = request.Revision};
            VersionIncrement increment = await _mediator.Send(query, cancellationToken);

            if (increment == VersionIncrement.None || increment == VersionIncrement.Unknown)
            {
                return Unit.Value;
            }

            SemVersion originalAssemblyVersion = _assemblyVersioningService.GetLatestAssemblyVersion(request.GitDirectory);

            var command = new IncrementAssemblyVersionCommand
            {
                Directory = request.GitDirectory,
                VersionIncrement = increment
            };
            await _mediator.Send(command, cancellationToken);

            SemVersion currentAssemblyVersion = _assemblyVersioningService.GetLatestAssemblyVersion(request.GitDirectory);

            var commitMessage = $"ci(Versioning): Increment version {originalAssemblyVersion} -> {currentAssemblyVersion} [skip ci] [skip hint]";
            _gitService.CommitChanges(request.GitDirectory, commitMessage, request.CommitAuthorEmail);

            string commitId = _gitService.GetCommits(request.GitDirectory).First(x => x.Subject.Equals(commitMessage)).Id;
            string tagValue = $"v{currentAssemblyVersion}";

            _gitService.PushRemote(request.GitDirectory, request.RemoteTarget, request.BranchName);
            _gitService.CreateTag(request.GitDirectory, tagValue, commitId);
            _gitService.PushRemote(request.GitDirectory, request.RemoteTarget, tagValue);

            return Unit.Value;
        }
    }
}
