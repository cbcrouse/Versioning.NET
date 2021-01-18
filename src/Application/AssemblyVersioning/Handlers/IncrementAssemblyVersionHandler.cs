using Application.AssemblyVersioning.Commands;
using Application.Interfaces;
using Domain.Enumerations;
using MediatR;
using Microsoft.Extensions.Logging;
using Semver;
using System;
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
            SemVersion assemblyVersion = _assemblyVersioningService.GetLatestAssemblyVersion(request.Directory);

            if (assemblyVersion < new SemVersion(1))
            {
                _logger.LogInformation($"Assembly currently in beta. Lowering increment: {request.VersionIncrement}.");
                switch (request.VersionIncrement)
                {
                    case VersionIncrement.Unknown:
                        request.VersionIncrement = VersionIncrement.None;
                        break;
                    case VersionIncrement.None:
                    case VersionIncrement.Patch:
                        break;
                    case VersionIncrement.Minor:
                        request.VersionIncrement = VersionIncrement.Patch;
                        break;
                    case VersionIncrement.Major:
                        request.VersionIncrement = VersionIncrement.Minor;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.VersionIncrement), request.VersionIncrement, null);
                }
                _logger.LogInformation($"Increment lowered to: {request.VersionIncrement}");
            }

            _assemblyVersioningService.IncrementVersion(request.VersionIncrement, request.Directory);
            _logger.LogInformation($"Incremented assembly versions by {request.VersionIncrement}.");
            return Task.FromResult(Unit.Value);
        }
    }
}
