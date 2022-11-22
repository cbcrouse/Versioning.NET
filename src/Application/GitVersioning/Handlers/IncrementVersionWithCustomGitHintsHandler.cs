using Application.GitVersioning.Commands;
using Application.GitVersioning.Queries;
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
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing the version with git integration based on git commit messages, with custom hints.
    /// </summary>
    public class IncrementVersionWithCustomGitHintsHandler : IRequestHandler<IncrementVersionWithCustomGitHintsCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<IncrementVersionWithCustomGitHintsHandler> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="mediator">An abstraction for accessing application behaviors.</param>
        /// <param name="logger">A generic interface for logging.</param>
        public IncrementVersionWithCustomGitHintsHandler(
            IMediator mediator,
            ILogger<IncrementVersionWithCustomGitHintsHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>Handles the request to increment the version with git integration.</summary>
        /// <param name="request">The <see cref="IRequest{TResponse}"/> object responsible for incrementing the version with git integration.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        public async Task<Unit> Handle(IncrementVersionWithCustomGitHintsCommand request, CancellationToken cancellationToken)
        {
            var query = new GetCommitVersionInfosQuery { GitDirectory = request.GitDirectory, RemoteTarget = request.RemoteTarget, TipBranchName = request.BranchName };
            IEnumerable<GitCommitVersionInfo> versionInfos = await _mediator.Send(query, cancellationToken);

            VersionIncrement increment = this.DeterminePriorityIncrement(versionInfos, request.CaseInsensitiveHints,
                request.SkipTypeHints, request.SkipScopeHints, request.SkipSubjectHints,
                request.MajorTypeHints, request.MajorScopeHints, request.MajorSubjectHints,
                request.MinorTypeHints, request.MinorScopeHints, request.MinorSubjectHints,
                request.PatchTypeHints, request.PatchScopeHints, request.PatchSubjectHints);
            _logger.LogInformation($"Increment '{increment}' was determined from the commits.");

            var command = new IncrementVersionWithGitCommand { GitDirectory = request.GitDirectory, TargetDirectory = request.TargetDirectory, 
                SearchOption = request.SearchOption, CommitAuthorEmail = request.CommitAuthorEmail, RemoteTarget = request.RemoteTarget, 
                BranchName = request.BranchName, TagPrefix = request.TagPrefix, TagSuffix = request.TagSuffix, 
                ExitBeta = (request.ExitBeta ?? versionInfos.Any(x=>x.ExitBeta)), VersionIncrement = increment};
            return await _mediator.Send(command, cancellationToken);
        }

        /// <summary>
        /// Returns the prioritized <see cref="VersionIncrement"/> from a collection of <see cref="GitCommitVersionInfo"/>s.
        /// </summary>
        /// <param name="gitCommitVersionInfos">A collection of <see cref="GitCommitVersionInfo"/>s.</param>
        /// <param name="caseInsensitiveHints">Enable case-insensitive comparaison</param>
        /// <param name="skipTypeHints">A collection of commit's types how will create a none increment</param>
        /// <param name="skipScopeHints">A collection of commit's scopes how will create a none increment</param>
        /// <param name="skipSubjectHints">A collection of commit's subjects hints how will create a none increment</param>
        /// <param name="majorTypeHints">A collection of commit's types how will create a major increment</param>
        /// <param name="majorScopeHints">A collection of commit's scopes how will create a major increment</param>
        /// <param name="majorSubjectHints">A collection of commit's subjects hints how will create a major increment</param>
        /// <param name="minorTypeHints">A collection of commit's types how will create a minor increment</param>
        /// <param name="minorScopeHints">A collection of commit's scopes how will create a minor increment</param>
        /// <param name="minorSubjectHints">A collection of commit's subjects hints how will create a minor increment</param>
        /// <param name="patchTypeHints">A collection of commit's types how will create a patch increment</param>
        /// <param name="patchScopeHints">A collection of commit's scopes how will create a patch increment</param>
        /// <param name="patchSubjectHints">A collection of commit's subjects hints how will create a patch increment</param>
        /// <returns></returns>
        private VersionIncrement DeterminePriorityIncrement(IEnumerable<GitCommitVersionInfo> gitCommitVersionInfos, bool caseInsensitiveHints,
            IEnumerable<string> skipTypeHints, IEnumerable<string> skipScopeHints, IEnumerable<string> skipSubjectHints,
            IEnumerable<string> majorTypeHints, IEnumerable<string> majorScopeHints, IEnumerable<string> majorSubjectHints,
            IEnumerable<string> minorTypeHints, IEnumerable<string> minorScopeHints, IEnumerable<string> minorSubjectHints,
            IEnumerable<string> patchTypeHints, IEnumerable<string> patchScopeHints, IEnumerable<string> patchSubjectHints)
        {
            IEnumerable<GitCommitVersionInfo> CommitsList = gitCommitVersionInfos;
            skipScopeHints.Append("[skip hint]");

            if (caseInsensitiveHints)
            {
                CommitsList = CommitsList.Select(c => new GitCommitVersionInfo(c.Type.ToLower(), c.Scope.ToLower(), c.Subject.ToLower()));
                skipTypeHints = skipTypeHints.Select(h => h.ToLower());
                skipScopeHints = skipScopeHints.Select(h => h.ToLower());
                skipSubjectHints = skipSubjectHints.Select(h => h.ToLower());
                majorTypeHints = majorTypeHints.Select(h => h.ToLower());
                majorScopeHints = majorScopeHints.Select(h => h.ToLower());
                majorSubjectHints = majorSubjectHints.Select(h => h.ToLower());
                minorTypeHints = minorTypeHints.Select(h => h.ToLower());
                minorScopeHints = minorScopeHints.Select(h => h.ToLower());
                minorSubjectHints = minorSubjectHints.Select(h => h.ToLower());
                patchTypeHints = patchTypeHints.Select(h => h.ToLower());
                patchScopeHints = patchScopeHints.Select(h => h.ToLower());
                patchSubjectHints = patchSubjectHints.Select(h => h.ToLower());
            }

            IEnumerable<GitCommitVersionInfo> SkipedCommits = CommitsList.Where(c => skipTypeHints.Any(th => th == c.Type) 
                                                                            || skipScopeHints.Any(ch => ch == c.Scope) 
                                                                            || skipSubjectHints.Any(uh => uh.Contains(c.Subject)));

            CommitsList = CommitsList.Except(SkipedCommits);

            bool isMajor = CommitsList.Any(c => majorTypeHints.Any(th => th == c.Type)
                                            || majorScopeHints.Any(ch => ch == c.Scope)
                                            || majorSubjectHints.Any(uh => uh.Contains(c.Subject)));
            if (isMajor) return VersionIncrement.Major;

            bool isMinor = CommitsList.Any(c => minorTypeHints.Any(th => th == c.Type)
                                            || minorScopeHints.Any(ch => ch == c.Scope)
                                            || minorSubjectHints.Any(uh => uh.Contains(c.Subject)));
            if (isMinor) return VersionIncrement.Minor;

            bool isPatch = CommitsList.Any(c => patchTypeHints.Any(th => th == c.Type)
                                            || patchScopeHints.Any(ch => ch == c.Scope)
                                            || patchSubjectHints.Any(uh => uh.Contains(c.Subject)));
            if (isPatch) return VersionIncrement.Patch;

            bool isNone = SkipedCommits.Any();
            return isNone ? VersionIncrement.None : VersionIncrement.Unknown;
        }
    }
}
