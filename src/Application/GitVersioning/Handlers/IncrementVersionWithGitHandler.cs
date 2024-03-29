﻿using Application.AssemblyVersioning.Commands;
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
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing the version with git integration.
    /// </summary>
    public class IncrementVersionWithGitHandler : IRequestHandler<IncrementVersionWithGitCommand, Unit>
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
        public IncrementVersionWithGitHandler(
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

            SemVersion originalAssemblyVersion = _assemblyVersioningService.GetLatestAssemblyVersion(request.TargetDirectory, request.SearchOption);

            var command = new IncrementAssemblyVersionCommand
            {
                Directory = request.TargetDirectory,
                SearchOption = request.SearchOption,
                VersionIncrement = request.VersionIncrement,
                ExitBeta = request.ExitBeta
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
