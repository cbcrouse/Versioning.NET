using Application.GitVersioning.Commands;
using Application.GitVersioning.Queries;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enumerations;
using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly IGitVersioningService _gitVersioningService;
        private readonly ILogger<IncrementVersionWithGitHintsHandler> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
        /// <param name="gitVersioningService">An abstraction for retrieving version hint info from git commit messages.</param>
        /// <param name="logger">A generic interface for logging.</param>
        public IncrementVersionWithGitHintsHandler(
            IMediator mediator,
            IGitVersioningService gitVersioningService,
            ILogger<IncrementVersionWithGitHintsHandler> logger)
        {
            _mediator = mediator;
            _gitVersioningService = gitVersioningService;
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

            var command = new IncrementVersionWithGitCommand { GitDirectory = request.GitDirectory, TargetDirectory = request.TargetDirectory, 
                SearchOption = request.SearchOption, CommitAuthorEmail = request.CommitAuthorEmail, RemoteTarget = request.RemoteTarget, 
                BranchName = request.BranchName, TagPrefix = request.TagPrefix, TagSuffix = request.TagSuffix, 
                ExitBeta = versionInfos.Any(x=>x.ExitBeta), VersionIncrement = increment};
            return await _mediator.Send(command, cancellationToken);
        }
    }
}
