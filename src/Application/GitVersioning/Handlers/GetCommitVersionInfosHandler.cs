using Application.GitVersioning.Queries;
using Application.Interfaces;
using Domain.Entities;
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
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for retrieving a collection of <see cref="GitCommitVersionInfo"/>s.
    /// </summary>
    public class GetCommitVersionInfosHandler : IRequestHandler<GetCommitVersionInfosQuery, IEnumerable<GitCommitVersionInfo>>
    {
        private readonly IGitService _gitService;
        private readonly IGitVersioningService _gitVersioningService;
        private readonly ILogger<GetCommitVersionInfosHandler> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="gitService">An abstraction to facilitate testing without using the git integration.</param>
        /// <param name="gitVersioningService">An abstraction for retrieving version hint info from git commit messages.</param>
        /// <param name="logger">A generic interface for logging.</param>
        public GetCommitVersionInfosHandler(IGitService gitService, IGitVersioningService gitVersioningService, ILogger<GetCommitVersionInfosHandler> logger)
        {
            _gitService = gitService;
            _gitVersioningService = gitVersioningService;
            _logger = logger;
        }

        /// <summary>Handles a request</summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from the request</returns>
        public Task<IEnumerable<GitCommitVersionInfo>> Handle(GetCommitVersionInfosQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<string> tags = _gitService.GetTags(request.GitDirectory);
            KeyValuePair<string, SemVersion>? latestTag = _gitVersioningService.GetLatestVersionTag(tags);

            if (latestTag == null)
            {
                _logger.LogInformation("A valid semver tag (e.g. v1.0.0) was not found. Please add a valid semver tag to your repository and try again.");
                return Task.FromResult<IEnumerable<GitCommitVersionInfo>>(new List<GitCommitVersionInfo>());
            }

            string untilHash = _gitService.GetTagId(request.GitDirectory, latestTag?.Key ?? "");
            string fromHash = _gitService.GetBranchTipId(request.GitDirectory, request.RemoteTarget, request.TipBranchName);
            var filter = new GitCommitFilter(fromHash, untilHash);
            List<GitCommit> commits = _gitService.GetCommitsByFilter(request.GitDirectory, filter);

            if (!commits.Any())
            {
                _logger.LogInformation("No new commits were found.");
                return Task.FromResult<IEnumerable<GitCommitVersionInfo>>(new List<GitCommitVersionInfo>());
            }

            IEnumerable<GitCommitVersionInfo> versionInfos = _gitVersioningService.GetCommitVersionInfo(commits);

            return Task.FromResult(versionInfos);
        }
    }
}
