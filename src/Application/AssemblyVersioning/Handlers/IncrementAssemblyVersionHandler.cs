using Application.AssemblyVersioning.Commands;
using Application.Extensions;
using Application.Interfaces;
using Domain.Enumerations;
using MediatR;
using Microsoft.Extensions.Logging;
using Semver;
using System.Threading;
using System.Threading.Tasks;

namespace Application.AssemblyVersioning.Handlers
{
    /// <summary>
    /// The <see cref="IRequestHandler{TRequest,TResponse}"/> responsible for incrementing assembly versions.
    /// </summary>
    public class IncrementAssemblyVersionHandler : IRequestHandler<IncrementAssemblyVersionCommand, Unit>
    {
        private readonly IAssemblyVersioningService _assemblyVersioningService;
        private readonly ILogger<IncrementAssemblyVersionHandler> _logger;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="assemblyVersioningService">An abstraction for working with assembly versions.</param>
        /// <param name="logger">A generic interface for logging.</param>
        public IncrementAssemblyVersionHandler(IAssemblyVersioningService assemblyVersioningService, ILogger<IncrementAssemblyVersionHandler> logger)
        {
            _assemblyVersioningService = assemblyVersioningService;
            _logger = logger;
        }

        /// <summary>
        /// Handles the request to version assemblies.
        /// </summary>
        /// <param name="request">The <see cref="IMediator"/> request object responsible for incrementing assembly versions.</param>
        /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
        public Task<Unit> Handle(IncrementAssemblyVersionCommand request, CancellationToken cancellationToken)
        {
            SemVersion assemblyVersion = _assemblyVersioningService.GetLatestAssemblyVersion(request.Directory, request.SearchOption);

            if (IsBetaVersion(assemblyVersion) && !request.ExitBeta)
            {
                _logger.LogInformation("Assembly currently in beta. Lowering increment to '{VersionIncrement}'.", request.VersionIncrement);
                request.VersionIncrement = request.VersionIncrement.ToBeta();
                _logger.LogInformation("Increment lowered to '{VersionIncrement}'", request.VersionIncrement);
            }

            if (IsBetaVersion(assemblyVersion) && request.ExitBeta)
            {
                _logger.LogInformation("Assembly currently in beta. Exit beta: {ExitBeta}.", request.ExitBeta);
                request.VersionIncrement = VersionIncrement.Major;
                _logger.LogInformation("Increment changed to '{VersionIncrement}'", request.VersionIncrement);
            }

            _assemblyVersioningService.IncrementVersion(request.VersionIncrement, request.Directory, request.SearchOption);
            _logger.LogInformation("Incremented assembly versions by '{VersionIncrement}'", request.VersionIncrement);
            return Task.FromResult(Unit.Value);
        }

        private static bool IsBetaVersion(SemVersion assemblyVersion)
        {
            return assemblyVersion < new SemVersion(1);
        }
    }
}
