#nullable enable
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
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for returning the <see cref="VersionIncrement"/> based on git commit messages.
    /// </summary>
    public class GetIncrementFromCommitHintsHandler : IRequestHandler<GetIncrementFromCommitHintsQuery, VersionIncrement>
    {
        private readonly IGitService _gitService;
        private readonly IGitVersioningService _gitVersioningService;
        private readonly ILogger<GetIncrementFromCommitHintsHandler> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="gitService">An abstraction to facilitate testing without using the git integration.</param>
        /// <param name="gitVersioningService">An abstraction for retrieving version hint info from git commit messages.</param>
        /// <param name="logger">A generic interface for logging.</param>
        public GetIncrementFromCommitHintsHandler(IGitService gitService, IGitVersioningService gitVersioningService, ILogger<GetIncrementFromCommitHintsHandler> logger)
        {
            _gitService = gitService;
            _gitVersioningService = gitVersioningService;
            _logger = logger;
        }

        /// <summary>Handles a request</summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from the request</returns>
        public Task<VersionIncrement> Handle(GetIncrementFromCommitHintsQuery request, CancellationToken cancellationToken)
        {
            var tags = _gitService.GetTags(request.GitDirectory);
            var latestTag = _gitVersioningService.GetLatestVersionTag(tags);
            var untilHash = _gitService.GetTagId(request.GitDirectory, latestTag.Key);
            var fromHash = _gitService.GetBranchTipId(request.GitDirectory, request.TipBranchName);
            var filter = new GitCommitFilter
            {
                FromHash = fromHash,
                UntilHash = untilHash
            };
            List<GitCommit> commits = _gitService.GetCommitsByFilter(request.GitDirectory, filter);

            if (!commits.Any())
            {
                _logger.LogInformation("No new commits were found.");
                return Task.FromResult(VersionIncrement.None);
            }

            IEnumerable<GitCommitVersionInfo> versionInfos = _gitVersioningService.GetCommitVersionInfo(commits);
            VersionIncrement increment = _gitVersioningService.DeterminePriorityIncrement(versionInfos.Select(x => x.VersionIncrement));
            _logger.LogInformation($"Increment '{increment}' was determined from the commits.");
            return Task.FromResult(increment);
        }
    }
}
