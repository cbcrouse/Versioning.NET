using Application.AssemblyVersioning.Commands;
using Application.Extensions;
using Application.Interfaces;
using Domain.Enumerations;
using MediatR;
using Microsoft.Extensions.Logging;
using Semver;
using System.Threading;
using System.Threading.Tasks;

namespace Application.AssemblyVersioning.Handlers;

/// <summary>
/// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing assembly versions.
/// </summary>
/// <param name="assemblyVersioningService">An abstraction for working with assembly versions.</param>
/// <param name="logger">A generic interface for logging.</param>
public class IncrementAssemblyVersionHandler(IAssemblyVersioningService assemblyVersioningService, ILogger<IncrementAssemblyVersionHandler> logger)
    : IRequestHandler<IncrementAssemblyVersionCommand, Unit>
{
    /// <inheritdoc />
    public Task<Unit> Handle(IncrementAssemblyVersionCommand request, CancellationToken cancellationToken)
    {
        SemVersion assemblyVersion = assemblyVersioningService.GetLatestAssemblyVersion(request.Directory, request.SearchOption);

        logger.LogInformation("Latest assembly version found: '{AssemblyVersion}'.", assemblyVersion);

        if (IsBetaVersion(assemblyVersion) && !request.ExitBeta)
        {
            logger.LogInformation("Assembly currently in beta. Lowering increment to '{VersionIncrement}'.", request.VersionIncrement);
            request.VersionIncrement = request.VersionIncrement.ToBeta();
            logger.LogInformation("Increment lowered to '{VersionIncrement}'", request.VersionIncrement);
        }

        if (IsBetaVersion(assemblyVersion) && request.ExitBeta)
        {
            logger.LogInformation("Assembly currently in beta. Exit beta: {ExitBeta}.", request.ExitBeta);
            request.VersionIncrement = VersionIncrement.Major;
            logger.LogInformation("Increment changed to '{VersionIncrement}'", request.VersionIncrement);
        }

        assemblyVersioningService.IncrementVersion(request.VersionIncrement, request.Directory, request.SearchOption);
        logger.LogInformation("Incremented assembly versions by '{VersionIncrement}'", request.VersionIncrement);
        return Task.FromResult(Unit.Value);
    }

    private static bool IsBetaVersion(SemVersion assemblyVersion)
    {
        return assemblyVersion.Major < 1;
    }
}