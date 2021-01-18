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
            if (string.IsNullOrWhiteSpace(request.Revision))
            {
                List<string> tags = _gitService.GetTags(request.GitDirectory).ToList();
                string latestTag = _gitVersioningService.GetLatestVersionTag(tags).Key;
                request.Revision = latestTag + "..HEAD";
            }

            List<GitCommit> commits = _gitService.GetCommits(request.GitDirectory, request.Revision);

            if (!commits.Any())
            {
                _logger.LogInformation($"No commits were found using revision: {request.Revision}");
                return Task.FromResult(VersionIncrement.None);
            }

            IEnumerable<GitCommitVersionInfo> versionInfos = _gitVersioningService.GetCommitVersionInfo(commits.Select(x => x.Subject));
            VersionIncrement increment = _gitVersioningService.DeterminePriorityIncrement(versionInfos.Select(x => x.VersionIncrement));
            _logger.LogInformation($"Increment '{increment}' was determined from the commits found with revision: {request.Revision}.");
            return Task.FromResult(increment);
        }
    }
}
